using Acontplus.Persistence.SqlServer.Mapping;
using System.Data.Common;
using Acontplus.Persistence.SqlServer.Exceptions;

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
        .Handle<SqlException>(AdoRepositoryException.IsTransientException)
        .Or<TimeoutException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            (ex, timeSpan, retryCount, context) =>
            {
                // Optional: Log each retry attempt
                // context.GetLogger()?.LogWarning(ex, "Retry {RetryCount} for ADO.NET operation.", retryCount);
            });

    // Fields for sharing connection/transaction with UnitOfWork
    private DbTransaction _currentTransaction;
    private DbConnection _currentConnection;

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
    private async Task<DbConnection> GetOpenConnectionAsync(string connectionStringName,
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
            var connection = new SqlConnection(GetConnectionString(connectionStringName));
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
                AdoRepositoryException.HandleSqlException(ex, _logger, nameof(QueryAsync));
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
                AdoRepositoryException.HandleSqlException(ex, _logger, nameof(GetDataSetAsync));
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
                AdoRepositoryException.HandleSqlException(ex, _logger, nameof(ExecuteNonQueryAsync));
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
    public async Task<T> QuerySingleOrDefaultAsync<T>(
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
                AdoRepositoryException.HandleSqlException(ex, _logger, nameof(QuerySingleOrDefaultAsync));
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
}