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
	public class MemberMessagePage
	{
		[DataMember]
		public long TotalPages { get; set; }

		[DataMember]
		public long TotalItems { get; set; }

		[DataMember]
		public List<MGMemberMessage> Messages { get; set; }
	}

	[DataContract]
	public class MGMemberMessage
	{
		[DataMember]
		public virtual Int64 Id { get; set; }

		[DataMember]
		public long MessagedefId { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string Summary { get; set; }

		[DataMember]
		public string Body { get; set; }

		[DataMember]
		public virtual DateTime DateIssued { get; set; }

		[DataMember]
		public virtual DateTime? ExpiryDate { get; set; }

		[DataMember]
		public virtual Int32 DisplayOrder { get; set; }

		[DataMember]
		public virtual List<MGContentAttribute> ContentAttributes { get; set; }

		public static MGMemberMessage Hydrate(Member member, MemberMessage message, string lang, string channel)
		{
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				MessageDef mdef = content.GetMessageDef(message.MessageDefId);
				MGMemberMessage mc = new MGMemberMessage()
				{
					Id = message.Id,
					MessagedefId = message.MessageDefId,
					Subject = mdef.EvaluateBScript(member, mdef.GetSubject(lang, channel)),
					Summary = mdef.EvaluateBScript(member, mdef.GetSummary(lang, channel)),
					Body = mdef.EvaluateBScript(member, mdef.GetBody(lang, channel)),
					DateIssued = message.DateIssued,
					ExpiryDate = message.ExpiryDate,
					DisplayOrder = message.DisplayOrder != null ? message.DisplayOrder.Value : 1
				};
				return mc;
			}
		}
	}
}