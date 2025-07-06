using Acontplus.Persistence.SqlServer.Mapping;
using System.Data.Common;

namespace Acontplus.Persistence.SqlServer.Repositories;

/// <summary>
/// Provides ADO.NET data access operations with retry policy and optional transaction sharing.
/// </summary>
public class AdoRepository : IAdoRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdoRepository> _logger;
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new();

    // Polly retry policy for transient SQL errors and timeouts
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
        .Handle<SqlException>(IsTransientException)
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
    /// Injects IConfiguration to retrieve connection strings and ILogger for logging.
    /// </summary>
    public AdoRepository(IConfiguration configuration, ILogger<AdoRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sets the current database transaction from the Unit of Work.
    /// </summary>
    /// <param name="transaction">The database transaction.</param>
    public void SetTransaction(DbTransaction transaction)
    {
        _currentTransaction = transaction;
    }

    /// <summary>
    /// Sets the current database connection from the Unit of Work.
    /// This connection will be used for all operations while a transaction is active.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    public void SetConnection(DbConnection connection)
    {
        _currentConnection = connection;
    }

    /// <summary>
    /// Clears the current transaction and connection, typically called after a UoW commit/rollback.
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
        // Use "DefaultConnection" if name is null or empty
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
    /// Determines if a SqlException is transient (retryable).
    /// </summary>
    private static bool IsTransientException(SqlException ex)
    {
        // List of SQL Server transient error numbers
        // Add or remove based on your specific needs
        var transientErrorNumbers = new[] { 4060, 40197, 40501, 40613, 49918, 49919, 49920, 4221 };
        return transientErrorNumbers.Contains(ex.Number);
    }

    /// <summary>
    /// Creates and opens a new SqlConnection. Prioritizes the shared connection if available.
    /// </summary>
    /// <param name="connectionStringName">Name of the connection string to use if no shared connection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An open SqlConnection.</returns>
    private async Task<DbConnection> GetOpenConnectionAsync(string connectionStringName,
        CancellationToken cancellationToken)
    {
        // If a shared connection from UoW is present and open, use it
        if (_currentConnection != null && _currentConnection.State == ConnectionState.Open)
        {
            return _currentConnection;
        }

        // If a shared connection is present but not open, try to open it
        if (_currentConnection != null && _currentConnection.State != ConnectionState.Open)
        {
            await _currentConnection.OpenAsync(cancellationToken);
            return _currentConnection;
        }

        // Otherwise, create a new connection using the specified connection string name
        try
        {
            var connection = new SqlConnection(GetConnectionString(connectionStringName));
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and opening connection for '{ConnectionName}'.", connectionStringName);
            throw; // Re-throw to propagate the connection error
        }
    }

    /// <summary>
    /// Creates and configures a SqlCommand.
    /// </summary>
    private SqlCommand CreateCommand(
        DbConnection connection, // Use DbConnection for broader compatibility
        string commandText,
        Dictionary<string, object> parameters,
        CommandOptionsDto? options) // Accepts CommandOptionsDto
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = commandText;

        // Apply command options, or defaults if not provided
        options ??= new CommandOptionsDto(); // Ensure options is not null

        cmd.CommandTimeout = options.CommandTimeout ?? 30;

        cmd.CommandType = options.CommandType; // Set command type from options

        // Attach transaction if available
        if (_currentTransaction != null)
        {
            cmd.Transaction = (SqlTransaction)_currentTransaction; // Cast is safe if using SqlConnection/SqlTransaction
        }

        // Add parameters
        foreach (var parameter in parameters.Where(p => !string.IsNullOrEmpty(p.Key)))
        {
            CommandParameterBuilder.AddParameter(cmd, parameter.Key, parameter.Value ?? DBNull.Value);
        }

        return
            (SqlCommand)cmd; // Cast to SqlCommand for specific features if needed later, otherwise DbCommand is fine.
    }

    /// <summary>
    /// Executes a SQL query or stored procedure and maps the results to a list of objects.
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
                var connection =
                    await GetOpenConnectionAsync(null,
                        cancellationToken); // connectionStringName is null as UoW should handle it, or it falls back to Default
                if (_currentConnection == null)
                    connectionToClose = connection; // Only manage connection if it's not shared

                await using var cmd = CreateCommand(connection, sql, parameters, options);

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                return await DbDataReaderMapper.ToListAsync<T>(reader, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing QueryAsync for '{Sql}'.", sql);
                throw; // Re-throw the exception after logging
            }
            finally
            {
                connectionToClose?.Close(); // Ensure connection opened by this method is closed
            }
        });
    }

    /// <summary>
    /// Executes a SQL query or stored procedure and returns a DataSet.
    /// </summary>
    public async Task<DataSet> GetDataSetAsync(
        string sql,
        Dictionary<string, object>? parameters,
        CommandOptionsDto? options,
        CancellationToken cancellationToken)
    {
        parameters ??= new Dictionary<string, object>();
        options ??= new CommandOptionsDto(); // Ensure options is not null

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
                    // Assuming CommandParameterBuilder.AddOutputParameter handles SqlDbType and size
                    CommandParameterBuilder.AddOutputParameter(cmd, "@tableNames", SqlDbType.VarChar,
                        options.TableNamesLength);
                }

                var ds = new DataSet();
                using var adapter = new SqlDataAdapter(cmd); // SqlDataAdapter often needs SqlCommand
                await Task.Run(() => adapter.Fill(ds), cancellationToken); // Fill is synchronous, so wrap in Task.Run

                if (options.WithTableNames)
                {
                    await DataTableNameMapper.ProcessTableNames(cmd, ds);
                }

                return ds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetDataSetAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a non-query SQL command or stored procedure (INSERT, UPDATE, DELETE).
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing ExecuteNonQueryAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }

    /// <summary>
    /// Executes a SQL query or stored procedure designed to return a single row,
    /// and maps the first row to an object of type T.
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
                    : null; // Return null instead of new instance
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing QuerySingleOrDefaultAsync for '{Sql}'.", sql);
                throw;
            }
            finally
            {
                connectionToClose?.Close();
            }
        });
    }
}