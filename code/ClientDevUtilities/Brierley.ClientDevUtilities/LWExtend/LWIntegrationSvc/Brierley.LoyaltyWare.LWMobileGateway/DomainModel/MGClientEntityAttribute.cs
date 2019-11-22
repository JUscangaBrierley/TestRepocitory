using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGClientEntityAttribute
    {
        #region Properties
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DataType DataType { get; set; }
        [DataMember]
        public object Value { get; set; }
        #endregion

        #region Data Transfer Methods
        public static MGClientEntityAttribute Hydrate(AttributeMetaData attribute, object value)
        {
            MGClientEntityAttribute att = new MGClientEntityAttribute()
            {
                Name = attribute.Name,
                DataType = (DataType)Enum.Parse(typeof(DataType),attribute.DataType),
                Value = value                
            };
            return att;
        }        
        #endregion
    }
}