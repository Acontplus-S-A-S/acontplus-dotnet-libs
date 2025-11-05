using System.Runtime.CompilerServices;
using Acontplus.Persistence.SqlServer.Mapping;

namespace Acontplus.Persistence.SqlServer.Repositories;

/// <summary>
/// Provides ADO.NET data access operations with retry policy and optional transaction sharing.
/// Enhanced with SQL Server error handling and domain error mapping.
/// </summary>
public class AdoRepository : IAdoRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdoRepository> _logger;
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new();

    // Polly retry policy for transient SQL errors and timeouts
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
        .Handle<SqlException>(SqlServerExceptionHandler.IsTransientException)
        .Or<TimeoutException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            (ex, timeSpan, retryCount, context) =>
            {
                // Optional: Log each retry attempt
                // context.GetLogger()?.LogWarning(ex, "Retry {RetryCount} for ADO.NET operation.", retryCount);
            });

    // Fields for sharing connection/transaction with UnitOfWork
    private DbTransaction? _currentTransaction;
    private DbConnection? _currentConnection;

    /// <summary>
    /// Constructor for AdoRepository.
    /// </summary>
    public AdoRepository(IConfiguration configuration, ILogger<AdoRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sets the current database transaction from the Unit of Work.
    /// </summary>
    public void SetTransaction(DbTransaction transaction)
    {
        _currentTransaction = transaction;
    }

    /// <summary>
    /// Sets the current database connection from the Unit of Work.
    /// </summary>
    public void SetConnection(DbConnection connection)
    {
        _currentConnection = connection;
    }

    /// <summary>
    /// Clears the current transaction and connection.
    /// </summary>
    public void ClearTransaction()
    {
        _currentTransaction = null;
        _currentConnection = null;
    }

    /// <summary>
    /// Retrieves a connection string from configuration, caching it for subsequent calls.
    /// </summary>
    private string GetConnectionString(string name)
    {
        var key = string.IsNullOrEmpty(name) ? "DefaultConnection" : name;

        return _connectionStrings.GetOrAdd(key, k =>
        {
            var connString = _configuration.GetConnectionString(k);
            if (!string.IsNullOrEmpty(connString)) return connString;
            _logger.LogError("Connection string '{ConnectionName}' not found.", k);
            throw new InvalidOperationException($"Connection string '{k}' not found");
        });
    }

    /// <summary>
    /// Creates and opens a new SqlConnection.
    /// </summary>
    private async Task<DbConnection> GetOpenConnectionAsync(string? connectionStringName,
        CancellationToken cancellationToken)
    {
        if (_currentConnection != null && _currentConnection.State == ConnectionState.Open)
        {
            return _currentConnection;
        }

        if (_currentConnection != null && _currentConnection.State != ConnectionState.Open)
        {
            await _currentConnection.OpenAsync(cancellationToken);
            return _currentConnection;
        }

        try
        {
            var connection = new SqlConnection(GetConnectionString(connectionStringName ?? string.Empty));
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and opening connection for '{ConnectionName}'.", connectionStringName);
            throw;
        }
    }

    /// <summary>
    /// Creates and configures a SqlCommand.
    /// </summary>
    private SqlCommand CreateCommand(
        DbConnection connection,
        string commandText,
        Dictionary<string, object> parameters,
        CommandOptionsDto? options)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = commandText;

        options ??= new CommandOptionsDto();
        cmd.CommandTimeout = options.CommandTimeout ?? 30;
        cmd.CommandType = options.CommandType;

        if (_currentTransaction != null)
        {
            cmd.Transaction = (SqlTransaction)_currentTransaction;
        }

        foreach (var parameter in parameters.Where(p => !string.IsNullOrEmpty(p.Key)))
        {
            CommandParameterBuilder.AddParameter(cmd, parameter.Key, parameter.Value ?? DBNull.Value);
        }

        return (SqlCommand)cmd;
    }

    /// <summary>
    /// Executes a SQL query and maps results to a list of objects.
    /// </summary>
    public async Task<List<T>> QueryAsync<T>(
        string sql,
        Dictionary<string, object>? parameters,
        CommandOptionsDto? options,
        CancellationToken cancellationToken)
    {
        parameters ??= new Dictionary<string, object>();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null)
                    connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                return await DbDataReaderMapper.ToListAsync<T>(reader, cancellationToken);
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(QueryAsync));
                throw; // This line won't be reached, but keeps compiler happy
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing QueryAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a SQL query and returns a DataSet.
    /// </summary>
    public async Task<DataSet> GetDataSetAsync(
        string sql,
        Dictionary<string, object>? parameters,
        CommandOptionsDto? options,
        CancellationToken cancellationToken)
    {
        parameters ??= new Dictionary<string, object>();
        options ??= new CommandOptionsDto();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);

                if (options.WithTableNames)
                {
                    CommandParameterBuilder.AddOutputParameter(cmd, "@tableNames", SqlDbType.VarChar,
                        options.TableNamesLength);
                }

                var ds = new DataSet();
                using var adapter = new SqlDataAdapter(cmd);
                await Task.Run(() => adapter.Fill(ds), cancellationToken);

                if (options.WithTableNames)
                {
                    await DataTableNameMapper.ProcessTableNames(cmd, ds);
                }

                return ds;
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(GetDataSetAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing GetDataSetAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a non-query SQL command.
    /// </summary>
    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        Dictionary<string, object>? parameters,
        CommandOptionsDto? options,
        CancellationToken cancellationToken)
    {
        parameters ??= new Dictionary<string, object>();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                return await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(ExecuteNonQueryAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing ExecuteNonQueryAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a SQL query designed to return a single row.
    /// </summary>
    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default) where T : class
    {
        parameters ??= new Dictionary<string, object>();
        options ??= new CommandOptionsDto();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                return await reader.ReadAsync(cancellationToken)
                    ? await DbDataReaderMapper.MapToObject<T>(reader)
                    : null;
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(QuerySingleOrDefaultAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing QuerySingleOrDefaultAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a SQL query and returns the first row or null.
    /// </summary>
    public async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default) where T : class
    {
        parameters ??= new Dictionary<string, object>();
        options ??= new CommandOptionsDto();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                return await reader.ReadAsync(cancellationToken)
                    ? await DbDataReaderMapper.MapToObject<T>(reader)
                    : null;
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(QueryFirstOrDefaultAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing QueryFirstOrDefaultAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    #region Scalar Query Methods

    /// <summary>
    /// Executes a query and returns a single scalar value.
    /// </summary>
    public async Task<TScalar?> ExecuteScalarAsync<TScalar>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                var result = await cmd.ExecuteScalarAsync(cancellationToken);

                if (result == null || result == DBNull.Value)
                    return default;

                return (TScalar)Convert.ChangeType(result, typeof(TScalar));
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(ExecuteScalarAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing ExecuteScalarAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Checks if any rows exist for the given query.
    /// </summary>
    public async Task<bool> ExistsAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        var count = await ExecuteScalarAsync<int>(sql, parameters, options, cancellationToken);
        return count > 0;
    }

    /// <summary>
    /// Gets the count of rows for the given query.
    /// </summary>
    public async Task<int> CountAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteScalarAsync<int?>(sql, parameters, options, cancellationToken);
        return result ?? 0;
    }

    /// <summary>
    /// Gets the long count of rows for the given query.
    /// </summary>
    public async Task<long> LongCountAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteScalarAsync<long?>(sql, parameters, options, cancellationToken);
        return result ?? 0L;
    }

    #endregion

    #region Paged Query Methods

    /// <summary>
    /// Executes a paginated SQL query with automatic count query.
    /// Uses SQL Server OFFSET-FETCH with optimized query plans.
    /// </summary>
    public async Task<PagedResult<T>> GetPagedAsync<T>(
        string sql,
        PaginationDto pagination,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        // Generate count SQL from the main SQL
        var countSql = GenerateCountSql(sql);
        return await GetPagedAsync<T>(sql, countSql, pagination, parameters, options, cancellationToken);
    }

    /// <summary>
    /// Executes a paginated SQL query with custom count query.
    /// </summary>
    public async Task<PagedResult<T>> GetPagedAsync<T>(
        string sql,
        string countSql,
        PaginationDto pagination,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();
        ValidatePagination(pagination);

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                // Apply pagination filters to parameters
                var enrichedParameters = ApplyPaginationFilters(parameters, pagination);

                // Get total count
                var totalCount = await CountAsync(countSql, enrichedParameters, options, cancellationToken);

                // Build paginated query with ORDER BY and OFFSET-FETCH
                var pagedSql = BuildPagedSql(sql, pagination);

                // Add pagination parameters
                var pagedParameters = new Dictionary<string, object>(enrichedParameters)
                {
                    ["@__Offset"] = (pagination.PageIndex - 1) * pagination.PageSize,
                    ["@__Fetch"] = pagination.PageSize
                };

                // Execute paged query
                await using var cmd = CreateCommand(connection, pagedSql, pagedParameters, options);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                var items = await DbDataReaderMapper.ToListAsync<T>(reader, cancellationToken);

                // Build result with metadata using standardized keys
                var metadata = new Dictionary<string, object>
                {
                    [PaginationMetadataKeys.HasFilters] = pagination.Filters?.Any() ?? false,
                    [PaginationMetadataKeys.HasSearch] = !string.IsNullOrWhiteSpace(pagination.SearchTerm),
                    [PaginationMetadataKeys.SortBy] = pagination.SortBy ?? string.Empty,
                    [PaginationMetadataKeys.SortDirection] = pagination.SortDirection.ToString()
                };

                // Optional: Only add in non-production environments
                // metadata[PaginationMetadataKeys.QuerySource] = PaginationMetadataKeys.QuerySourceRawQuery;

                if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
                {
                    metadata[PaginationMetadataKeys.SearchTerm] = pagination.SearchTerm;
                }

                if (pagination.Filters?.Any() == true)
                {
                    metadata[PaginationMetadataKeys.FilterCount] = pagination.Filters.Count;
                }

                return new PagedResult<T>(items, pagination.PageIndex, pagination.PageSize, totalCount, metadata);
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(GetPagedAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing GetPagedAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a paginated stored procedure.
    /// </summary>
    public async Task<PagedResult<T>> GetPagedFromStoredProcedureAsync<T>(
        string storedProcedureName,
        PaginationDto pagination,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();
        ValidatePagination(pagination);

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                // Add pagination parameters
                var spParameters = new Dictionary<string, object>(parameters)
                {
                    ["@PageIndex"] = pagination.PageIndex,
                    ["@PageSize"] = pagination.PageSize
                };

                // Add sort parameters if provided
                if (!string.IsNullOrWhiteSpace(pagination.SortBy))
                {
                    spParameters["@SortBy"] = ValidateAndSanitizeSortColumn(pagination.SortBy);
                    spParameters["@SortDirection"] = pagination.SortDirection.ToString();
                }

                // Add search term if provided
                if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
                {
                    spParameters["@SearchTerm"] = pagination.SearchTerm;
                }

                // Add individual filters from pagination
                if (pagination.Filters != null && pagination.Filters.Any())
                {
                    foreach (var filter in pagination.Filters)
                    {
                        var paramName = filter.Key.StartsWith("@") ? filter.Key : $"@{filter.Key}";
                        spParameters[paramName] = filter.Value;
                    }
                }

                var spOptions = options ?? new CommandOptionsDto { CommandType = CommandType.StoredProcedure };

                await using var cmd = CreateCommand(connection, storedProcedureName, spParameters, spOptions);

                // Add output parameter for total count
                CommandParameterBuilder.AddOutputParameter(cmd, "@TotalCount", SqlDbType.Int, 0);

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                var items = await DbDataReaderMapper.ToListAsync<T>(reader, cancellationToken);

                // Close reader to get output parameters
                await reader.CloseAsync();

                var totalCount = cmd.Parameters["@TotalCount"].Value != DBNull.Value
                    ? Convert.ToInt32(cmd.Parameters["@TotalCount"].Value)
                    : 0;

                // Build result with metadata using standardized keys
                var metadata = new Dictionary<string, object>
                {
                    [PaginationMetadataKeys.HasFilters] = pagination.Filters?.Any() ?? false,
                    [PaginationMetadataKeys.HasSearch] = !string.IsNullOrWhiteSpace(pagination.SearchTerm),
                    [PaginationMetadataKeys.SortBy] = pagination.SortBy ?? string.Empty,
                    [PaginationMetadataKeys.SortDirection] = pagination.SortDirection.ToString()
                };

                // ⚠️ Security: Only add stored procedure name in development/staging
                // Uncomment if PaginationMetadataOptions.IncludeStoredProcedureName is enabled:
                // metadata[PaginationMetadataKeys.QuerySource] = PaginationMetadataKeys.QuerySourceStoredProcedure;
                // metadata["storedProcedureName"] = storedProcedureName;

                if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
                {
                    metadata[PaginationMetadataKeys.SearchTerm] = pagination.SearchTerm;
                }

                if (pagination.Filters?.Any() == true)
                {
                    metadata[PaginationMetadataKeys.FilterCount] = pagination.Filters.Count;
                }

                return new PagedResult<T>(items, pagination.PageIndex, pagination.PageSize, totalCount, metadata);
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(GetPagedFromStoredProcedureAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing GetPagedFromStoredProcedureAsync for '{storedProcedureName}'.", storedProcedureName);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    #endregion

    #region Batch Operations

    /// <summary>
    /// Executes multiple queries in a single batch.
    /// </summary>
    public async Task<List<List<T>>> QueryMultipleAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                await using var cmd = CreateCommand(connection, sql, parameters, options);
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

                var results = new List<List<T>>();

                do
                {
                    var resultSet = await DbDataReaderMapper.ToListAsync<T>(reader, cancellationToken);
                    results.Add(resultSet);
                } while (await reader.NextResultAsync(cancellationToken));

                return results;
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(QueryMultipleAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing QueryMultipleAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes multiple non-query commands in a batch.
    /// </summary>
    public async Task<int> ExecuteBatchNonQueryAsync(
        IEnumerable<(string Sql, Dictionary<string, object>? Parameters)> commands,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
    {
        var commandList = commands.ToList();
        if (!commandList.Any())
            return 0;

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            DbTransaction? transaction = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                // Start transaction if not already in one
                if (_currentTransaction == null)
                    transaction = await connection.BeginTransactionAsync(cancellationToken);

                var totalAffected = 0;

                foreach (var (sql, parameters) in commandList)
                {
                    var cmdParams = parameters ?? new Dictionary<string, object>();
                    await using var cmd = CreateCommand(connection, sql, cmdParams, options);
                    if (transaction != null)
                        cmd.Transaction = (SqlTransaction)transaction;

                    totalAffected += await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                if (transaction != null)
                    await transaction.CommitAsync(cancellationToken);

                return totalAffected;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync(cancellationToken);

                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(ExecuteBatchNonQueryAsync));
                throw;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(ex, "Unexpected error executing ExecuteBatchNonQueryAsync.");
                throw;
            }
            finally
            {
                transaction?.Dispose();
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Bulk insert using SqlBulkCopy for optimal performance.
    /// </summary>
    public async Task<int> BulkInsertAsync<T>(
        IEnumerable<T> data,
        string tableName,
        Dictionary<string, string>? columnMappings = null,
        int batchSize = 10000,
        CancellationToken cancellationToken = default)
    {
        var dataTable = ConvertToDataTable(data, columnMappings);
        return await BulkInsertAsync(dataTable, tableName, columnMappings, batchSize, cancellationToken);
    }

    /// <summary>
    /// Bulk insert from DataTable using SqlBulkCopy.
    /// </summary>
    public async Task<int> BulkInsertAsync(
        DataTable dataTable,
        string tableName,
        Dictionary<string, string>? columnMappings = null,
        int batchSize = 10000,
        CancellationToken cancellationToken = default)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return 0;

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            DbConnection? connectionToClose = null;
            try
            {
                var connection = await GetOpenConnectionAsync(null, cancellationToken);
                if (_currentConnection == null) connectionToClose = connection;

                using var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, (SqlTransaction?)_currentTransaction)
                {
                    DestinationTableName = tableName,
                    BatchSize = batchSize,
                    BulkCopyTimeout = 300, // 5 minutes for large batches
                    EnableStreaming = true
                };

                // Add column mappings
                if (columnMappings != null)
                {
                    foreach (var mapping in columnMappings)
                    {
                        bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
                    }
                }
                else
                {
                    // Auto-map columns by name
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }

                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                return dataTable.Rows.Count;
            }
            catch (SqlException ex)
            {
                SqlServerExceptionHandler.HandleSqlException(ex, _logger, nameof(BulkInsertAsync));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing BulkInsertAsync for table '{TableName}'.", tableName);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    #endregion

    #region Streaming Methods

    /// <summary>
    /// Streams query results as an async enumerable for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<T> QueryAsyncEnumerable<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new Dictionary<string, object>();
        DbConnection? connectionToClose = null;
        SqlCommand? cmd = null;
        DbDataReader? reader = null;

        try
        {
            var connection = await GetOpenConnectionAsync(null, cancellationToken);
            if (_currentConnection == null) connectionToClose = connection;

            cmd = CreateCommand(connection, sql, parameters, options);
            reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

            var type = typeof(T);
            var isRecord = type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Any()
                           && type.BaseType == typeof(object);

            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                               .Where(p => p.CanWrite || (isRecord && p.CanRead))
                               .ToArray();

            var columnMap = new Dictionary<string, System.Reflection.PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                if (string.IsNullOrEmpty(columnName)) continue;
                var property = properties.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
                if (property != null) columnMap[columnName] = property;
            }

            while (await reader.ReadAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var instance = Activator.CreateInstance<T>();

                foreach (var kvp in columnMap)
                {
                    var ordinal = reader.GetOrdinal(kvp.Key);
                    if (await reader.IsDBNullAsync(ordinal, cancellationToken)) continue;

                    var value = reader.GetValue(ordinal);
                    var property = kvp.Value;
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    try
                    {
                        var convertedValue = propertyType.IsEnum ? Enum.ToObject(propertyType, value) :
                                           propertyType == typeof(Guid) ? (value is string strGuid ? Guid.Parse(strGuid) : (Guid)value) :
                                           Convert.ChangeType(value, propertyType);
                        property.SetValue(instance, convertedValue);
                    }
                    catch { /* Skip properties that fail to map */ }
                }

                yield return instance;
            }
        }
        finally
        {
            if (reader != null)
                await reader.DisposeAsync();
            if (cmd != null)
                await cmd.DisposeAsync();
            connectionToClose?.Close();
        }
    }

    #endregion

    #region Helper Methods

    private static string GenerateCountSql(string sql)
    {
        // Simple count SQL generation - removes ORDER BY and wraps in COUNT
        var cleanSql = System.Text.RegularExpressions.Regex.Replace(
            sql,
            @"\s+ORDER\s+BY\s+[^)]*$",
            string.Empty,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return $"SELECT COUNT(*) FROM ({cleanSql}) AS CountQuery";
    }

    private string BuildPagedSql(string sql, PaginationDto pagination)
    {
        var builder = new System.Text.StringBuilder(sql);

        // Add ORDER BY if not present and if SortBy is provided
        if (!sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(pagination.SortBy))
            {
                // Validate SortBy to prevent SQL injection
                var safeSortBy = ValidateAndSanitizeSortColumn(pagination.SortBy);
                var direction = pagination.SortDirection == Core.Enums.SortDirection.Desc ? "DESC" : "ASC";
                builder.Append($" ORDER BY [{safeSortBy}] {direction}");
            }
            else
            {
                // Default to first column if no sort specified
                builder.Append(" ORDER BY 1 ASC");
            }
        }

        // Add OFFSET-FETCH (SQL Server optimized)
        builder.Append(" OFFSET @__Offset ROWS FETCH NEXT @__Fetch ROWS ONLY");

        return builder.ToString();
    }

    /// <summary>
    /// Validates and sanitizes sort column names to prevent SQL injection.
    /// Only allows alphanumeric characters, underscores, and dots for qualified names.
    /// </summary>
    private string ValidateAndSanitizeSortColumn(string columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            throw new ArgumentException("Column name cannot be empty", nameof(columnName));

        // Remove any potential SQL injection attempts
        // Only allow: letters, numbers, underscores, dots (for table.column notation)
        var pattern = @"^[a-zA-Z0-9_\.]+$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(columnName, pattern))
        {
            _logger.LogWarning("Potential SQL injection attempt detected in sort column: {ColumnName}", columnName);
            throw new ArgumentException($"Invalid column name: {columnName}. Only alphanumeric characters, underscores, and dots are allowed.", nameof(columnName));
        }

        // Additional check: prevent common SQL keywords
        var upperColumn = columnName.ToUpperInvariant();
        var dangerousKeywords = new[] { "DROP", "DELETE", "INSERT", "UPDATE", "EXEC", "EXECUTE", "SELECT", "UNION", "DECLARE", "CAST", "CONVERT" };

        if (dangerousKeywords.Any(keyword => upperColumn.Contains(keyword)))
        {
            _logger.LogWarning("SQL keyword detected in sort column: {ColumnName}", columnName);
            throw new ArgumentException($"Column name contains restricted SQL keywords: {columnName}", nameof(columnName));
        }

        return columnName;
    }

    /// <summary>
    /// Applies pagination filters to parameters dictionary.
    /// Merges pagination filters with existing parameters.
    /// </summary>
    private Dictionary<string, object> ApplyPaginationFilters(
        Dictionary<string, object> parameters,
        PaginationDto pagination)
    {
        var result = new Dictionary<string, object>(parameters);

        // Add search term if provided
        if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
        {
            result["@__SearchTerm"] = $"%{pagination.SearchTerm}%"; // For LIKE queries
        }

        // Add individual filters from pagination
        if (pagination.Filters != null && pagination.Filters.Any())
        {
            foreach (var filter in pagination.Filters)
            {
                // Use a prefix to avoid parameter name conflicts
                var paramName = filter.Key.StartsWith("@") ? filter.Key : $"@{filter.Key}";
                result[paramName] = filter.Value;
            }
        }

        return result;
    }

    private void ValidatePagination(PaginationDto pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);
        if (pagination.PageIndex < 1)
            throw new ArgumentException("PageIndex must be greater than 0", nameof(pagination));
        if (pagination.PageSize < 1 || pagination.PageSize > 10000)
            throw new ArgumentException("PageSize must be between 1 and 10000", nameof(pagination));
    }

    private DataTable ConvertToDataTable<T>(IEnumerable<T> data, Dictionary<string, string>? columnMappings)
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties();

        // Add columns
        foreach (var prop in properties)
        {
            var columnName = columnMappings?.ContainsKey(prop.Name) == true
                ? columnMappings[prop.Name]
                : prop.Name;

            var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dataTable.Columns.Add(columnName, columnType);
        }

        // Add rows
        foreach (var item in data)
        {
            var row = dataTable.NewRow();
            foreach (var prop in properties)
            {
                var columnName = columnMappings?.ContainsKey(prop.Name) == true
                    ? columnMappings[prop.Name]
                    : prop.Name;

                var value = prop.GetValue(item);
                row[columnName] = value ?? DBNull.Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    #endregion
}
