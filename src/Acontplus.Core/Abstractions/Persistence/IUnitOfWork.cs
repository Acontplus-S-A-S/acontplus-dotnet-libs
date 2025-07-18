﻿using Acontplus.Core.Domain.Common.Entities;
using System.Data.Common;

namespace Acontplus.Core.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRepository<TEntity, TId> GetRepository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull;

    IAuditableRepository<TEntity, TId> GetAuditableRepository<TEntity, TId>()
        where TEntity : AuditableEntity<TId>
        where TId : notnull;

    IAdoRepository AdoRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    DbTransaction CurrentDbTransaction { get; }
    DbConnection CurrentDbConnection { get; }
    bool HasActiveTransaction { get; }
}