using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public class PointTransactionDao : Brierley.FrameWork.Data.DataAccess.PointTransactionDao, IPointTransactionDao
    {
        public PointTransactionDao(Database database, ServiceConfig config) : base(database, config) { }
    }
}
