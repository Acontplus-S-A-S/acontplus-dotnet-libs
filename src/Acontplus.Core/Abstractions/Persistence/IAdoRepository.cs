namespace Acontplus.Core.Abstractions.Persistence;

public interface IAdoRepository
{
    Task<List<T>> QueryAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    Task<DataSet> GetDataSetAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteNonQueryAsync(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default);

    Task<T> QuerySingleOrDefaultAsync<T>(
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
        where T : class;

    void SetTransaction(DbTransaction transaction);
    void SetConnection(DbConnection connection);
    void ClearTransaction();
}
