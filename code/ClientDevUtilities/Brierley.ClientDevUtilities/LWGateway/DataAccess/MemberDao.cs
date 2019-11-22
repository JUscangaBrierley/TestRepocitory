using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Data;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway.DataAccess
{
    public class MemberDao : Brierley.FrameWork.Data.DataAccess.MemberDao, IMemberDao
    {
        public MemberDao(Database db, ServiceConfig config)
            : base(db, config)
        {

        }
    }
}
