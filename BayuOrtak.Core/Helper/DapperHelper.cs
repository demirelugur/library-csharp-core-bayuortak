namespace BayuOrtak.Core.Helper
{
    using Dapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using static Dapper.SqlMapper;
    public interface IDapperHelper
    {
        Task EnsureConnectionOpenAsync(CancellationToken cancellationToken = default);
        Task EnsureConnectionCloseAsync();
        Task<IEnumerable<dynamic>> QueryDynamicAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default);
        Task<GridReader> QueryMultipleAsync(string commandtext, object parameters, int? commandTimeout, CommandType commandType, CancellationToken cancellationToken = default);
        Task<int> ExecuteAsync(string commandtext, object parameters, int? commandTimeout, CommandType commandType, CancellationToken cancellationToken = default);
        Task<DbDataReader> ExecuteReaderAsync(string commandtext, object parameters, int? commandTimeout, CommandType commandType, CommandBehavior commandBehavior, CancellationToken cancellationToken = default);
        Task<T> ExecuteScalarAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default);
    }
    public sealed class DapperHelper : IDapperHelper
    {
        private readonly DbConnection con;
        private readonly IDbTransaction? dbtransaction;
        public DapperHelper(DbContext dbcontext) : this(dbcontext.Database.GetDbConnection(), dbcontext.Database.CurrentTransaction?.GetDbTransaction()) { }
        public DapperHelper(DbConnection con, IDbTransaction? dbtransaction)
        {
            this.con = con;
            this.dbtransaction = dbtransaction;
        }
        public async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken = default)
        {
            if (this.con.State != ConnectionState.Open)
            {
                await this.con.OpenAsync(cancellationToken);
            }
        }
        public async Task EnsureConnectionCloseAsync()
        {
            if (this.con.State != ConnectionState.Closed)
            {
                await this.con.CloseAsync();
            }
        }
        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default)
        {
            await this.EnsureConnectionOpenAsync(cancellationToken);
            return await this.con.QueryAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationToken));
        }
        public async Task<GridReader> QueryMultipleAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default)
        {
            await this.EnsureConnectionOpenAsync(cancellationToken);
            return await this.con.QueryMultipleAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationToken));
        }
        public async Task<int> ExecuteAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default)
        {
            await this.EnsureConnectionOpenAsync(cancellationToken);
            return await this.con.ExecuteAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationToken));
        }
        public async Task<DbDataReader> ExecuteReaderAsync(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CommandBehavior commandbehavior, CancellationToken cancellationToken = default)
        {
            await this.EnsureConnectionOpenAsync(cancellationToken);
            return await this.con.ExecuteReaderAsync(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationToken), commandbehavior);
        }
        public async Task<T> ExecuteScalarAsync<T>(string commandtext, object parameters, int? commandtimeout, CommandType commandtype, CancellationToken cancellationToken = default)
        {
            await this.EnsureConnectionOpenAsync(cancellationToken);
            return await this.con.ExecuteScalarAsync<T>(new CommandDefinition(commandtext, parameters, this.dbtransaction, commandtimeout, commandtype, CommandFlags.Buffered, cancellationToken));
        }
    }
}