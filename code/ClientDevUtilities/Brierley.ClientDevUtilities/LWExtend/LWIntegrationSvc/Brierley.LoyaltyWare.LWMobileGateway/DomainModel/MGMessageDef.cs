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
    public class MGMessageDef
    {
        [DataMember]
        public virtual Int64 Id { get; set; }

		[DataMember]
        public virtual string Name { get; set; }

		[DataMember]
        public virtual string Subject { get; set; }

		[DataMember]
		public virtual string Summary { get; set; }

		[DataMember]
        public virtual string Body { get; set; }

		[DataMember]
        public virtual DateTime StartDate { get; set; }

		[DataMember]
        public virtual DateTime? ExpiryDate { get; set; }

		[DataMember]
        public virtual Int32 DisplayOrder { get; set; }

		[DataMember]
        public virtual List<MGContentAttribute> ContentAttributes { get; set; }

		public static MGMessageDef Hydrate(MessageDef message, string lang, string channel, bool returnAttributes)
        {
            MGMessageDef mc = new MGMessageDef()
            {
                Id = message.Id,
                Name = message.Name,
                Subject = message.GetSubject(lang, channel),
				Summary = message.GetSummary(lang, channel),
                Body = message.GetBody(lang, channel),
                StartDate = message.StartDate,
                ExpiryDate = message.ExpiryDate,
                DisplayOrder = message.DisplayOrder != null ? message.DisplayOrder.Value : 1
            };
            if (returnAttributes)
            {
                mc.ContentAttributes = new List<MGContentAttribute>();
                foreach (ContentAttribute att in message.ContentAttributes)
                {
                    mc.ContentAttributes.Add(MGContentAttribute.Hydrate(att));
                }
            }
            return mc;
        }
    }
}