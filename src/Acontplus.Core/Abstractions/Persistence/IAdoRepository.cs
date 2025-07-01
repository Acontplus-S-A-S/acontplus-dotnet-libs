using Acontplus.Core.DTOs.Requests;
using System.Data.Common;

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

    // Renamed from QuerySingleOrDefaultAsync to be more descriptive of its action
    // and adapted to return T where T is a class (for single object mapping).
    Task<T> QuerySingleOrDefaultAsync<T>( // New name for single object mapping
        string sql,
        Dictionary<string, object>? parameters = null,
        CommandOptionsDto? options = null,
        CancellationToken cancellationToken = default)
        where T : class;

    // Methods for Unit of Work transaction integration
    void SetTransaction(DbTransaction transaction);
    void SetConnection(DbConnection connection);
    void ClearTransaction();
}
