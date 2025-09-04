namespace BayuOrtak.Core.Helper
{
    using Dapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using static Dapper.SqlMapper;
    public interface IDapperHelper : IDisposable, IAsyncDisposable
    {
        Task EnsureConnectionOpenAsync(CancellationToken cancellationtoken);
        Task EnsureConnectionCloseAsync();
        Task<IEnumerable<T>> QueryAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken) where T : class;
        Task<IEnumerable<dynamic>> QueryDynamicAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken);
        Task<GridReader> QueryMultipleAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken);
        Task<int> ExecuteAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken);
        Task<DbDataReader> ExecuteReaderAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CommandBehavior commandBehavior, CancellationToken cancellationtoken);
        Task<T> ExecuteScalarAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken);
    }
    public sealed class DapperHelper : IDapperHelper
    {
        private readonly DbConnection con;
        private readonly IDbTransaction? dbtransaction;
        private bool disposed = false;
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.con.State != ConnectionState.Closed) { this.con.Close(); }
                this.con.Dispose();
                this.disposed = true;
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (!this.disposed)
            {
                await this.EnsureConnectionCloseAsync();
                await this.con.DisposeAsync();
                this.disposed = true;
            }
        }
        public DapperHelper(DbContext dbcontext) : this(dbcontext.Database.GetDbConnection(), dbcontext.Database.CurrentTransaction?.GetDbTransaction()) { }
        public DapperHelper(DbConnection con, IDbTransaction? dbtransaction)
        {
            this.con = con;
            this.dbtransaction = dbtransaction;
        }
        public async Task EnsureConnectionOpenAsync(CancellationToken cancellationtoken)
        {
            if (this.con.State != ConnectionState.Open)
            {
                await this.con.OpenAsync(cancellationtoken);
            }
        }
        public async Task EnsureConnectionCloseAsync()
        {
            if (this.con.State != ConnectionState.Closed)
            {
                await this.con.CloseAsync();
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken) where T : class
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.QueryAsync<T>(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken));
        }
        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken)
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.QueryAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken));
        }
        public async Task<GridReader> QueryMultipleAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken)
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.QueryMultipleAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken));
        }
        public async Task<int> ExecuteAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken)
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.ExecuteAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken));
        }
        public async Task<DbDataReader> ExecuteReaderAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CommandBehavior commandbehavior, CancellationToken cancellationtoken)
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.ExecuteReaderAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken), commandbehavior);
        }
        public async Task<T> ExecuteScalarAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationtoken)
        {
            await this.EnsureConnectionOpenAsync(cancellationtoken);
            return await this.con.ExecuteScalarAsync<T>(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationtoken));
        }
    }
}