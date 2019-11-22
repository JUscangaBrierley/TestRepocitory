using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{        
    public class MGClientContext
    {
        #region Properties
        public string ParmName { get; set; }
        public string ParmValue { get; set; }
        #endregion

        #region Data Transfer Methods        
        public static MGClientContext[] ConvertFromJson(string contentAttsStr)
        {
            MGClientContext[] ctxList = JsonConvert.DeserializeObject<MGClientContext[]>(contentAttsStr);
            return ctxList;
        }
        #endregion
    }        
}