namespace BayuOrtak.Core.Base
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    /// <summary>
    /// Generic bir repository arayüzü.
    /// </summary>
    /// <typeparam name="T">Repository tarafından yönetilecek entity türü.</typeparam>
    /// <typeparam name="TContext">DbContext türü.</typeparam>
    public interface IGenericRepository<T, TContext> where T : class where TContext : DbContext
    {
        TContext Context { get; }
        DbSet<T> DbSet { get; }
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteByKeyAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task DeleteByPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// Generic bir repository&#39;nin temel implementasyonu.
    /// </summary>
    /// <typeparam name="T">Repository tarafından yönetilecek entity türü.</typeparam>
    /// <typeparam name="TContext">DbContext türü.</typeparam>
    public abstract class BaseGenericRepository<T, TContext> : IGenericRepository<T, TContext> where T : class where TContext : DbContext, new()
    {
        public BaseGenericRepository(TContext context)
        {
            this.Context = context;
        }
        public TContext Context { get; }
        public DbSet<T> DbSet => this.Context.Set<T>();
        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) // Not: cancellationToken kullanılmamasına rağmen eklenmesi gerekiyor. override yapıldığı yerlde gerekirse diye
        {
            if (entity != null)
            {
                if (this.Context.Entry(entity).State == EntityState.Detached) { this.DbSet.Attach(entity); }
                this.DbSet.Remove(entity);
            }
            return Task.CompletedTask;
        }
        public async Task DeleteByKeyAsync(CancellationToken cancellationToken, params object[] keyValues) => await this.DeleteAsync(await this.DbSet.FindAsync(keyValues, cancellationToken), cancellationToken);
        public async Task DeleteByPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await this.DeleteRangeAsync(await this.DbSet.Where(predicate).ToArrayAsync(cancellationToken), cancellationToken);
        public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) => Task.WhenAll(entities.Select(x => this.DeleteAsync(x, cancellationToken)).ToArray());
    }
}