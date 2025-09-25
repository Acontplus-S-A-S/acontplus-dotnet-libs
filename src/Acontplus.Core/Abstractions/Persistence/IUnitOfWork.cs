using System.Data.Common;

namespace Acontplus.Core.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    IAdoRepository AdoRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    DbTransaction CurrentDbTransaction { get; }
    DbConnection CurrentDbConnection { get; }
    bool HasActiveTransaction { get; }
}
