using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGRewardCategory
    {
        #region Data Members
        [DataMember]
        public virtual Int64 Id { get; set; }

        [DataMember]
        public virtual String Name { get; set; }        
        #endregion

        #region Data Transfer Methods
        public static MGRewardCategory Hydrate(Brierley.FrameWork.Data.DomainModel.Category cat)
        {
            MGRewardCategory mgCat = new MGRewardCategory()
            {
                Id = cat.ID,
                Name = cat.Name,                
            };
            return mgCat;
        }
        #endregion
    }
}