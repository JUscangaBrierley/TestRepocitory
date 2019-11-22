using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class CSAgentDao : DaoBase<CSAgent>
    {
        public CSAgentDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public CSAgent Retrieve(long id)
        {
            return GetEntity(id);
        }

        public CSAgent Retrieve(string userName, AgentAccountStatus? status)
        {
            return Database.FirstOrDefault<CSAgent>("select * from LW_CSAgent where Username = @0" + (status != null ? " and Status = @1" : ""), userName, status);
        }

        public CSAgent Retrieve(string userName, string password)
        {
            return Database.FirstOrDefault<CSAgent>("select * from LW_CSAgent where Username = @0 and Password = @1 and Status = @2", userName, password, AgentAccountStatus.Active);
        }

        public CSAgent RetrieveByAgentNumber(long agentNmbr)
        {
            return Database.FirstOrDefault<CSAgent>("select * from LW_CSAgent where AgentNumber = @0 and Status = @1", agentNmbr, AgentAccountStatus.Active);
        }

        public CSAgent RetrieveByResetCode(string resetCode)
        {
            return Database.FirstOrDefault<CSAgent>("select * from LW_CSAgent where ResetCode = @0 and Status = @1", resetCode, AgentAccountStatus.Active);
        }

        public List<CSAgent> Retrieve(string firstName, string lastName, string email, long? roleId, string phoneNumber, AgentAccountStatus? status)
        {
            string sql = "select * from LW_CSAgent where 1=1";
            if (string.IsNullOrEmpty(firstName) ^ string.IsNullOrEmpty(lastName))
            {
                string name = firstName + lastName;
                sql += " and (lower(FirstName) = lower(@0) or lower(LastName) = lower(@1))";
            }
            else
            {
                if (!string.IsNullOrEmpty(firstName))
                {
                    sql += " and lower(FirstName) = lower(@0)";
                }
                if (!string.IsNullOrEmpty(lastName))
                {
                    sql += " and lower(LastName) = lower(@1)";
                }
            }
            if (!string.IsNullOrEmpty(email))
            {
                sql += " and EmailAddress = @2";
            }
            if (roleId != null)
            {
                sql += " and RoleId = @3";
            }
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                sql += " and PhoneNumber = @4";
            }
            if (status != null)
            {
                sql += " and Status = @5";
            }
            return Database.Fetch<CSAgent>(sql, firstName, lastName, email, roleId, phoneNumber, status);
        }

        public List<CSAgent> RetrieveAll()
        {
            return Database.Fetch<CSAgent>("select * from LW_CSAgent where FirstName not in('bpadmin', 'host') or FirstName is null");
        }

        public void Delete(long id)
        {
            DeleteEntity(id);
        }
    }
}
