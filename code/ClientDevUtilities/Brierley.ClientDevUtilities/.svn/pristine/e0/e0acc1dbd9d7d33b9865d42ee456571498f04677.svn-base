using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway.PetaPocoGateway
{
    public interface IDatabase : IDisposable
    {
        bool EnableNamedParams { get; set; }
        bool EnableAutoSelect { get; set; }
        string LastCommand { get; }
        object[] LastArgs { get; }
        string LastSQL { get; }
        IDbConnection Connection { get; }
        bool KeepConnectionAlive { get; set; }
        int CommandTimeout { get; set; }
        int OneTimeCommandTimeout { get; set; }

        void AbortTransaction();
        void BeginTransaction();
        void CloseSharedConnection();
        void CompleteTransaction();
        IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args);
        int Delete<T>(Sql sql);
        int Delete(string tableName, string primaryKeyName, object poco);
        int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue);
        int Delete(object poco);
        int Delete<T>(object pocoOrPrimaryKey);
        int Delete<T>(string sql, params object[] args);
        int Execute(string sql, params object[] args);
        int Execute(Sql sql);
        T ExecuteScalar<T>(string sql, params object[] args);
        T ExecuteScalar<T>(Sql sql);
        bool Exists<T>(object primaryKey);
        bool Exists<T>(string sqlCondition, params object[] args);
        List<T1> Fetch<T1, T2, T3, T4>(Sql sql);
        List<T1> Fetch<T1, T2, T3>(Sql sql);
        List<T1> Fetch<T1, T2>(Sql sql);
        List<T1> Fetch<T1, T2, T3>(string sql, params object[] args);
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);
        List<T> Fetch<T>(string sql, params object[] args);
        List<T> Fetch<T>(Sql sql);
        List<T1> Fetch<T1, T2>(string sql, params object[] args);
        List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args);
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);
        List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args);
        List<T> Fetch<T>(long page, long itemsPerPage, Sql sql);
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);
        T First<T>(string sql, params object[] args);
        T First<T>(Sql sql);
        T FirstOrDefault<T>(string sql, params object[] args);
        T FirstOrDefault<T>(Sql sql);
        string FormatCommand(string sql, object[] args);
        string FormatCommand(IDbCommand cmd);
        ITransaction GetTransaction();
        object Insert(string tableName, string primaryKeyName, object poco);
        object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco);
        object Insert(object poco);
        bool IsNew(string primaryKeyName, object poco);
        bool IsNew(object poco);
        void OnBeginTransaction();
        void OnConnectionClosing(IDbConnection conn);
        IDbConnection OnConnectionOpened(IDbConnection conn);
        void OnEndTransaction();
        bool OnException(Exception x);
        void OnExecutedCommand(IDbCommand cmd);
        void OnExecutingCommand(IDbCommand cmd);
        void OpenSharedConnection();
        Page<T> Page<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs);
        Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args);
        Page<T> Page<T>(long page, long itemsPerPage, Sql sql);
        Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage);
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);
        IEnumerable<T1> Query<T1, T2>(string sql, params object[] args);
        IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args);
        IEnumerable<T> Query<T>(string sql, params object[] args);
        IEnumerable<T1> Query<T1, T2>(Sql sql);
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);
        IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args);
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);
        IEnumerable<T> Query<T>(Sql sql);
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);
        IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql);
        IEnumerable<T1> Query<T1, T2, T3>(Sql sql);
        IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args);
        void Save(string tableName, string primaryKeyName, object poco);
        void Save(object poco);
        T Single<T>(string sql, params object[] args);
        T Single<T>(object primaryKey);
        T Single<T>(Sql sql);
        T SingleOrDefault<T>(Sql sql);
        T SingleOrDefault<T>(string sql, params object[] args);
        T SingleOrDefault<T>(object primaryKey);
        List<T> SkipTake<T>(long skip, long take, string sql, params object[] args);
        List<T> SkipTake<T>(long skip, long take, Sql sql);
        int Update<T>(Sql sql);
        int Update(object poco, object primaryKeyValue, IEnumerable<string> columns);
        int Update(object poco, object primaryKeyValue);
        int Update(object poco);
        int Update(object poco, IEnumerable<string> columns);
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);
        int Update<T>(string sql, params object[] args);
        int Update(string tableName, string primaryKeyName, object poco);
    }
}
