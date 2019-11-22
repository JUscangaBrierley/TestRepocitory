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
    public class EmailAssociationDao : DaoBase<EmailAssociation>
    {
        public EmailAssociationDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public EmailAssociation Retrieve(long Id)
        {
            return GetEntity(Id);
        }

        public List<EmailAssociation> Retrieve(PointTransactionOwnerType ownerType, long ownerId, long? rowKey)
        {
            string sql = "select * from LW_EmailAssociation where OwnerType = @0 and OwnerId = @1";
            if (rowKey != null)
            {
                sql += " and RowKey = @2";
            }
            return Database.Fetch<EmailAssociation>(sql, ownerType, ownerId, rowKey);
        }

        public void Delete(long Id)
        {
            DeleteEntity(Id);
        }
    }
}
