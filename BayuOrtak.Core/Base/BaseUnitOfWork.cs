namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Results;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        TContext Context { get; }
        IDapperHelper Dapper { get; }
        DbConnection GetDbConnection();
        Task<SqlServerProperties> ServerPropertyAsync(CancellationToken cancellationToken);
        Task<int> ExecuteRawAsync(string query, object parameters, CancellationToken cancellationToken);
        Task<bool> TableReseedAsync(bool isdebug, CancellationToken cancellationToken, params Type[] types);
    }
    public class BaseUnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        /*
        IDisposable
        #region Dispose
        private bool _disposed;
        ~BaseUnitOfWork()
        {
            this.Dispose(false);
        }
        public void Dispose()
        {
            this.Dispose(true);
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
        */
        public BaseUnitOfWork(TContext context)
        {
            this.Context = context;
        }
        public TContext Context { get; }
        public IDapperHelper Dapper => new DapperHelper(this.Context);
        public DbConnection GetDbConnection() => this.Context.Database.GetDbConnection();
        public Task<SqlServerProperties> ServerPropertyAsync(CancellationToken cancellationToken) => this.Context.Database.SqlQueryRaw<SqlServerProperties>(SqlServerProperties.query()).FirstOrDefaultAsync(cancellationToken);
        public Task<int> ExecuteRawAsync(string sql, object parameters, CancellationToken cancellationToken) => this.Context.Database.ExecuteSqlRawAsync(sql, _to.ToSqlParameterFromObject(parameters), cancellationToken);
        public async Task<bool> TableReseedAsync(bool isdebug, CancellationToken cancellationToken, params Type[] types)
        {
            if (!isdebug)
            {
                types = types ?? Array.Empty<Type>();
                if (types.Length == 0 || !types.All(x => x.IsMappedTable())) { return false; }
                var _sb = new StringBuilder();
                var _index = 0;
                foreach (var type in types)
                {
                    var _pkinfo = this.getPrimaryKeyInfo(type);
                    if (_pkinfo.columnname == "" || _pkinfo.sqldbtypename == "") { continue; }
                    var _tablename = type.GetTableName(true);
                    var _variablename = $"@MAXID_{_index}";
                    _sb.AppendLine($"DECLARE {_variablename} {_pkinfo.sqldbtypename}");
                    _sb.AppendLine($"SELECT {_variablename} = MAX([{_pkinfo.columnname}]) FROM {_tablename}");
                    _sb.AppendLine($"SET {_variablename} = ISNULL({_variablename}, 0)");
                    _sb.AppendLine($"DBCC CHECKIDENT ('{_tablename}', RESEED, {_variablename})");
                    _index++;
                }
                if (_sb.Length == 0) { return false; }
                await this.ExecuteRawAsync(_sb.ToString(), null, cancellationToken);
            }
            return true;
        }
        private (string columnname, string sqldbtypename) getPrimaryKeyInfo(Type type)
        {
            if (_try.TryTableisKeyAttribute(type, out PropertyInfo[] _pks) && _pks.Length == 1 && _pks[0].GetDatabaseGeneratedOption() == DatabaseGeneratedOption.Identity)
            {
                var _propertytype = _pks[0].PropertyType;
                if (_propertytype.IsEnum) { _propertytype = Enum.GetUnderlyingType(_propertytype); }
                var _sqldbtypename = "";
                if (_propertytype == typeof(byte)) { _sqldbtypename = "TINYINT"; }
                else if (_propertytype == typeof(short)) { _sqldbtypename = "SMALLINT"; }
                else if (_propertytype == typeof(int)) { _sqldbtypename = "INT"; }
                else if (_propertytype == typeof(long)) { _sqldbtypename = "BIGINT"; }
                if (_sqldbtypename == "") { return ("", ""); }
                return (_pks[0].GetColumnName(), _sqldbtypename);
            }
            return ("", "");
        }
    }
}