﻿using Acontplus.Persistence.SqlServer.UnitOfWork;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Acontplus.Persistence.SqlServer.DependencyInjection;

public static class SqlServerServiceCollectionExtensions
{
    /// <summary>
    /// Registers a SQL Server DbContext and its corresponding UnitOfWork implementation,
    /// optionally with a service key using .NET 8+ keyed DI.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to register.</typeparam>
    /// <param name="services">The IServiceCollection.</param>
    /// <param name="sqlServerOptions">The SQL Server-specific options for DbContext.</param>
    /// <param name="serviceKey">Optional key to register the services with (for keyed DI).</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddSqlServerPersistence<TContext>(
        this IServiceCollection services,
        Action<SqlServerDbContextOptionsBuilder> sqlServerOptions,
        object serviceKey = null)
        where TContext : DbContext
    {
        services.AddDbContextPool<TContext>((sp, options) =>
        {
            options.UseSqlServer(sqlServerOptions);
        }, poolSize: 128);

        if (serviceKey is not null)
        {
            services.TryAddKeyedScoped<IUnitOfWork, UnitOfWork<TContext>>(serviceKey);
            services.TryAddKeyedScoped<DbContext>(serviceKey, (sp, key) => sp.GetRequiredKeyedService<TContext>(key));
        }
        else
        {
            services.TryAddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            services.TryAddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        }

        return services;
    }
}
