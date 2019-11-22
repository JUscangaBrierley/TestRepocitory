using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    [DataContract]
    public class MGClientEntity
    {
        #region Fields
        private const string _className = "MGClientEntity";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Properties

		[DataMember]
		public string Name { get; set; }

		[DataMember]
        public IList<MGClientEntityAttribute> Attributes { get; set; }

		[DataMember]
		public List<MGClientEntity> ChildEntities { get; set; }

        #endregion

        #region Private Helpers        
        #endregion

        #region Data Transfer Methods
        public static MGClientEntity Hydrate(IClientDataObject entity)
        {
            AttributeSetMetaData attSetMeta = entity.GetMetaData();
            MGClientEntity e = new MGClientEntity() 
            { 
                Name = attSetMeta.Name,
                Attributes = new List<MGClientEntityAttribute>()
            };            
            foreach (AttributeMetaData attMeta in attSetMeta.AsetAttributes)
            {                
                object value = entity.GetAttributeValue(attMeta.Name);
                MGClientEntityAttribute mgAtt = MGClientEntityAttribute.Hydrate(attMeta, value);
                e.Attributes.Add(mgAtt);
            }

            Dictionary<string, List<IClientDataObject>> children = entity.GetChildAttributeSets();
            if (children != null && children.Count > 0)
            {
				e.ChildEntities = new List<MGClientEntity>();
                foreach (KeyValuePair<string, List<IClientDataObject>> kv in children)
                {                    
                    foreach (IClientDataObject childEntity in kv.Value)
                    {
						MGClientEntity mgEntity = MGClientEntity.Hydrate(childEntity);
                        e.ChildEntities.Add(mgEntity);
                    }
                }
            }

            return e;
        }

        public MGClientEntityAttribute GetAttribute(string name)
        {
            MGClientEntityAttribute attribute = null;
            foreach (MGClientEntityAttribute att in Attributes)
            {
                if (att.Name == name)
                {
                    attribute = att;
                    break;
                }
            }
            return attribute;
        }        
        #endregion
    }
}