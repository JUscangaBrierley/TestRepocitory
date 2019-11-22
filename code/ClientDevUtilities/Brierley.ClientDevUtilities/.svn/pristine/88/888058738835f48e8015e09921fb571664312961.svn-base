using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway
{
    public class Database : IDatabase
    {
        PetaPoco.Database _database;

        public Database(PetaPoco.Database database)
        {
            _database = database;
        }

        public bool EnableNamedParams { get => _database.EnableNamedParams; set => _database.EnableNamedParams = value; }
        public bool EnableAutoSelect { get => _database.EnableAutoSelect; set => _database.EnableAutoSelect = value; }

        public string LastCommand => _database.LastCommand;

        public object[] LastArgs => _database.LastArgs;

        public string LastSQL => _database.LastSQL;

        public IDbConnection Connection => _database.Connection;

        public bool KeepConnectionAlive { get => _database.KeepConnectionAlive; set => _database.KeepConnectionAlive = value; }
        public int CommandTimeout { get => _database.CommandTimeout; set => _database.CommandTimeout = value; }
        public int OneTimeCommandTimeout { get => _database.OneTimeCommandTimeout; set => _database.OneTimeCommandTimeout = value; }

        public void AbortTransaction()
        {
            _database.AbortTransaction();
        }

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void CloseSharedConnection()
        {
            _database.CloseSharedConnection();
        }

        public void CompleteTransaction()
        {
            _database.CompleteTransaction();
        }

        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
        {
            return _database.CreateCommand(connection, sql, args);
        }

        public int Delete<T>(Sql sql)
        {
            return _database.Delete<T>(sql);
        }

        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return _database.Delete(tableName, primaryKeyName, poco);
        }

        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return _database.Delete(tableName, primaryKeyName, poco, primaryKeyValue);
        }

        public int Delete(object poco)
        {
            return _database.Delete(poco);
        }

        public int Delete<T>(object pocoOrPrimaryKey)
        {
            return _database.Delete<T>(pocoOrPrimaryKey);
        }

        public int Delete<T>(string sql, params object[] args)
        {
            return _database.Delete<T>(sql, args);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public int Execute(string sql, params object[] args)
        {
            return _database.Execute(sql, args);
        }

        public int Execute(Sql sql)
        {
            return _database.Execute(sql);
        }

        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            return _database.ExecuteScalar<T>(sql, args);
        }

        public T ExecuteScalar<T>(Sql sql)
        {
            return _database.ExecuteScalar<T>(sql);
        }

        public bool Exists<T>(object primaryKey)
        {
            return _database.Exists<T>(primaryKey);
        }

        public bool Exists<T>(string sqlCondition, params object[] args)
        {
            return _database.Exists<T>(sqlCondition, args);
        }

        public List<T1> Fetch<T1, T2, T3, T4>(Sql sql)
        {
            return _database.Fetch<T1, T2, T3, T4>(sql);
        }

        public List<T1> Fetch<T1, T2, T3>(Sql sql)
        {
            return _database.Fetch<T1, T2, T3>(sql);
        }

        public List<T1> Fetch<T1, T2>(Sql sql)
        {
            return _database.Fetch<T1, T2>(sql);
        }

        public List<T1> Fetch<T1, T2, T3>(string sql, params object[] args)
        {
            return _database.Fetch<T1, T2, T3>(sql, args);
        }

        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return _database.Fetch<T1, T2, TRet>(cb, sql, args);
        }

        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return _database.Fetch<T>(sql, args);
        }

        public List<T> Fetch<T>(Sql sql)
        {
            return _database.Fetch<T>(sql);
        }

        public List<T1> Fetch<T1, T2>(string sql, params object[] args)
        {
            return _database.Fetch<T1, T2>(sql, args);
        }

        public List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return _database.Fetch<T1, T2, T3, T4>(sql, args);
        }

        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return _database.Fetch<T1, T2, T3, T4, TRet>(cb, sql);
        }

        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return _database.Fetch<T1, T2, TRet>(cb, sql);
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return _database.Fetch<T>(page, itemsPerPage, sql, args);
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
        {
            return _database.Fetch<T>(page, itemsPerPage, sql);
        }

        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return _database.Fetch<T1, T2, T3, T4, TRet>(cb, sql, args);
        }

        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return _database.Fetch<T1, T2, T3, TRet>(cb, sql);
        }

        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return _database.Fetch<T1, T2, T3, TRet>(cb, sql, args);
        }

        public T First<T>(string sql, params object[] args)
        {
            return _database.First<T>(sql, args);
        }

        public T First<T>(Sql sql)
        {
            return _database.First<T>(sql);
        }

        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return _database.FirstOrDefault<T>(sql, args);
        }

        public T FirstOrDefault<T>(Sql sql)
        {
            return _database.FirstOrDefault<T>(sql);
        }

        public string FormatCommand(string sql, object[] args)
        {
            return _database.FormatCommand(sql, args);
        }

        public string FormatCommand(IDbCommand cmd)
        {
            return _database.FormatCommand(cmd);
        }

        public ITransaction GetTransaction()
        {
            return _database.GetTransaction();
        }

        public object Insert(string tableName, string primaryKeyName, object poco)
        {
            return _database.Insert(tableName, primaryKeyName, poco);
        }

        public object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            return _database.Insert(tableName, primaryKeyName, autoIncrement, poco);
        }

        public object Insert(object poco)
        {
            return _database.Insert(poco);
        }

        public bool IsNew(string primaryKeyName, object poco)
        {
            return _database.IsNew(primaryKeyName, poco);
        }

        public bool IsNew(object poco)
        {
            return _database.IsNew(poco);
        }

        public void OnBeginTransaction()
        {
            _database.OnBeginTransaction();
        }

        public void OnConnectionClosing(IDbConnection conn)
        {
            _database.OnConnectionClosing(conn);
        }

        public IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            return _database.OnConnectionOpened(conn);
        }

        public void OnEndTransaction()
        {
            _database.OnEndTransaction();
        }

        public bool OnException(Exception x)
        {
            return _database.OnException(x);
        }

        public void OnExecutedCommand(IDbCommand cmd)
        {
            _database.OnExecutedCommand(cmd);
        }

        public void OnExecutingCommand(IDbCommand cmd)
        {
            _database.OnExecutingCommand(cmd);
        }

        public void OpenSharedConnection()
        {
            _database.OpenSharedConnection();
        }

        public Page<T> Page<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            return _database.Page<T>(page, itemsPerPage, sqlCount, countArgs, sqlPage, pageArgs);
        }

        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return _database.Page<T>(page, itemsPerPage, sql, args);
        }

        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
        {
            return _database.Page<T>(page, itemsPerPage, sql);
        }

        public Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
        {
            return _database.Page<T>(page, itemsPerPage, sqlCount, sqlPage);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return _database.Query<T1, T2, T3, TRet>(cb, sql, args);
        }

        public IEnumerable<T1> Query<T1, T2>(string sql, params object[] args)
        {
            return _database.Query<T1, T2>(sql, args);
        }

        public IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args)
        {
            return _database.Query<TRet>(types, cb, sql, args);
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            return _database.Query<T>(sql, args);
        }

        public IEnumerable<T1> Query<T1, T2>(Sql sql)
        {
            return _database.Query<T1, T2>(sql);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return _database.Query<T1, T2, T3, T4, TRet>(cb, sql);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return _database.Query<T1, T2, T3, TRet>(cb, sql);
        }

        public IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args)
        {
            return _database.Query<T1, T2, T3>(sql, args);
        }

        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return _database.Query<T1, T2, TRet>(cb, sql);
        }

        public IEnumerable<T> Query<T>(Sql sql)
        {
            return _database.Query<T>(sql);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return _database.Query<T1, T2, T3, T4, TRet>(cb, sql, args);
        }

        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return _database.Query<T1, T2, TRet>(cb, sql, args);
        }

        public IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql)
        {
            return _database.Query<T1, T2, T3, T4>(sql);
        }

        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql)
        {
            return _database.Query<T1, T2, T3>(sql);
        }

        public IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return _database.Query<T1, T2, T3, T4>(sql, args);
        }

        public void Save(string tableName, string primaryKeyName, object poco)
        {
            _database.Save(tableName, primaryKeyName, poco);
        }

        public void Save(object poco)
        {
            _database.Save(poco);
        }

        public T Single<T>(string sql, params object[] args)
        {
            return _database.Single<T>(sql, args);
        }

        public T Single<T>(object primaryKey)
        {
            return _database.Single<T>(primaryKey);
        }

        public T Single<T>(Sql sql)
        {
            return _database.Single<T>(sql);
        }

        public T SingleOrDefault<T>(Sql sql)
        {
            return _database.SingleOrDefault<T>(sql);
        }

        public T SingleOrDefault<T>(string sql, params object[] args)
        {
            return _database.SingleOrDefault<T>(sql, args);
        }

        public T SingleOrDefault<T>(object primaryKey)
        {
            return _database.SingleOrDefault<T>(primaryKey);
        }

        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            return _database.SkipTake<T>(skip, take, sql, args);
        }

        public List<T> SkipTake<T>(long skip, long take, Sql sql)
        {
            return _database.SkipTake<T>(skip, take, sql);
        }

        public int Update<T>(Sql sql)
        {
            return _database.Update<T>(sql);
        }

        public int Update(object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return _database.Update(poco, primaryKeyValue, columns);
        }

        public int Update(object poco, object primaryKeyValue)
        {
            return _database.Update(poco, primaryKeyValue);
        }

        public int Update(object poco)
        {
            return _database.Update(poco);
        }

        public int Update(object poco, IEnumerable<string> columns)
        {
            return _database.Update(poco, columns);
        }

        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
        {
            return _database.Update(tableName, primaryKeyName, poco, columns);
        }

        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return _database.Update(tableName, primaryKeyName, poco, primaryKeyValue, columns);
        }

        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return _database.Update(tableName, primaryKeyName, poco, primaryKeyValue);
        }

        public int Update<T>(string sql, params object[] args)
        {
            return _database.Update<T>(sql, args);
        }

        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return _database.Update(tableName, primaryKeyName, poco);
        }
    }
}
