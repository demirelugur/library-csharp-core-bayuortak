namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Results;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        TContext Context { get; }
        IDapperHelper Dapper { get; }
        DbConnection GetDbConnection();
        Task<SqlServerProperties> ServerPropertyAsync(CancellationToken cancellationToken = default);
        Task<int> ExecuteRawAsync(string query, object parameters, CancellationToken cancellationToken = default);
    }
    public class BaseUnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, IDisposable
    {
        #region Dispose
        private bool _disposed;
        ~BaseUnitOfWork()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) { this.Context.Dispose(); }
                _disposed = true;
            }
        }
        #endregion
        public BaseUnitOfWork(TContext context)
        {
            this.Context = context;
        }
        public TContext Context { get; }
        public IDapperHelper Dapper => new DapperHelper(this.Context);
        public DbConnection GetDbConnection() => this.Context.Database.GetDbConnection();
        public Task<SqlServerProperties> ServerPropertyAsync(CancellationToken cancellationToken = default) => this.Context.Database.SqlQueryRaw<SqlServerProperties>(SqlServerProperties.query()).FirstOrDefaultAsync(cancellationToken);
        public Task<int> ExecuteRawAsync(string sql, object parameters, CancellationToken cancellationToken = default) => this.Context.Database.ExecuteSqlRawAsync(sql, _to.ToSqlParameterFromObject(parameters), cancellationToken);
    }
}