using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGSurvey
    {
        #region Fields        
        #endregion

        #region Properties
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Type { get; set; } // "General, Profile        
        public virtual bool Completed { get; set; }
        #endregion        
    }
}