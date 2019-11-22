using System.Collections.Generic;
using Brierley.FrameWork.Interfaces;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class RestDaoBase<T> : DaoBase<T>
    {
        public RestDaoBase(Database database, ServiceConfig config) : base(database, config)
        {
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }

        /// <summary>
        /// Delete for XRef tables
        /// </summary>
        /// <param name="column1Name">XRef column 1 name</param>
        /// <param name="column2Name">XRef column 2 name</param>
        /// <param name="column1Val">XRef column 1 value</param>
        /// <param name="column2Val">XRef column 2 value</param>
        public void Delete(string column1Name, string column2Name, long column1Val, long column2Val)
        {
            var t = typeof(T);
            var sql = string.Format("delete from LW_{0} t where t.{1} = @0 and t.{2} = @1", t.Name, column1Name, column2Name);

            Database.Execute(sql, column1Val, column2Val);
        }

        public void SoftDelete(long id)
        {
            var t = typeof(T);            
            var sql = string.Format("update LW_{0} t set t.isdeleted='Y' where t.id = @0", t.Name);

            Database.Execute(sql, id);
        }

        /// <summary>
        /// SoftDelete for XRef tables
        /// </summary>
        /// <param name="column1Name">XRef column 1 name</param>
        /// <param name="column2Name">XRef column 2 name</param>
        /// <param name="column1Val">XRef column 1 value</param>
        /// <param name="column2Val">XRef column 2 value</param>
        public void SoftDelete(string column1Name, string column2Name, long column1Val, long column2Val)
        {
            var t = typeof(T);
            var sql = string.Format("update LW_{0} t set t.isdeleted='Y' where t.{1} = @0 and t.{2} = @1", t.Name, column1Name, column2Name);

            Database.Execute(sql, column1Val, column2Val);
        }

        public void Undelete(long id)
        {
            var t = typeof(T);
            var sql = string.Format("update LW_{0} t set t.isdeleted='N' where t.id = @0", t.Name);

            Database.Execute(sql, id);
        }

        /// <summary>
        /// Undelete for XRef tables
        /// </summary>
        /// <param name="column1Name">XRef column 1 name</param>
        /// <param name="column2Name">XRef column 2 name</param>
        /// <param name="column1Val">XRef column 1 value</param>
        /// <param name="column2Val">XRef column 2 value</param>
        public void Undelete(string column1Name, string column2Name, long column1Val, long column2Val)
        {
            var t = typeof(T);
            var sql = string.Format("update LW_{0} t set t.isdeleted='N' where t.{1} = @0 and t.{2} = @1", t.Name, column1Name, column2Name);

            Database.Execute(sql, column1Val, column2Val);
        }
    }
}
