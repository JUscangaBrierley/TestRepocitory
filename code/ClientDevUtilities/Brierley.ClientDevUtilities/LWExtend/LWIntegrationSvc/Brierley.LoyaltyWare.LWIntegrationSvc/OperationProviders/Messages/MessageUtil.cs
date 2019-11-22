using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages
{
	internal static class MessageUtil
	{
		public static APIStruct SerializeMessageDefinition(string language, string channel, MessageDef message, bool returnAttributes)
		{
			APIArguments rparms = new APIArguments();
			rparms.Add("Id", message.Id);
			rparms.Add("Name", message.Name);
			rparms.Add("StartDate", message.StartDate);
			if (message.ExpiryDate != null)
			{
				rparms.Add("ExpiryDate", message.ExpiryDate);
			}
			if (message.DisplayOrder != null)
			{
				rparms.Add("DisplayOrder", message.DisplayOrder.ToString());
			}

			string subject = message.GetSubject(language, channel);
			if (!string.IsNullOrEmpty(subject))
			{
				rparms.Add("Subject", subject);
			}

			string summary = message.GetSummary(language, channel);
			if (!string.IsNullOrEmpty(summary))
			{
				rparms.Add("Summary", summary);
			}

			string body = message.GetBody(language, channel);
			if (!string.IsNullOrEmpty(body))
			{
				rparms.Add("Body", body);
			}

			if (returnAttributes && message.Attributes.Count > 0)
			{
				APIStruct[] atts = new APIStruct[message.Attributes.Count];
				int idx = 0;
				using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
				{
					foreach (ContentAttribute ra in message.Attributes)
					{
						ContentAttributeDef def = service.GetContentAttributeDef(ra.ContentAttributeDefId);
						APIArguments attparms = new APIArguments();
						attparms.Add("AttributeName", def.Name);
						attparms.Add("AttributeValue", ra.Value);
						APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
						atts[idx++] = v;
					}
				}
				rparms.Add("ContentAttributes", atts);
			}
			APIStruct rv = new APIStruct() { Name = "MessageDefinition", IsRequired = false, Parms = rparms };
			return rv;
		}

		public static APIStruct SerializeMemberMessage(Member member, string language, string channel, MemberMessage message)
		{
			using (ContentService service = LWDataServiceUtil.ContentServiceInstance())
			{
				MessageDef def = service.GetMessageDef(message.MessageDefId);

				APIArguments msgParams = new APIArguments();
				msgParams.Add("Id", message.Id);
				msgParams.Add("MessageDefId", def.Id);
				msgParams.Add("Status", message.Status.ToString());

				string subject = def.GetSubject(language, channel);
				if (!string.IsNullOrEmpty(subject))
				{
					msgParams.Add("Subject", def.EvaluateBScript(member, subject));
				}

				string summary = def.GetSummary(language, channel);
				if (!string.IsNullOrEmpty(summary))
				{
					msgParams.Add("Summary", def.EvaluateBScript(member, summary));
				}

				string body = def.GetBody(language, channel);
				if (!string.IsNullOrEmpty(body))
				{
					msgParams.Add("Body", def.EvaluateBScript(member, body));
				}

				msgParams.Add("DateIssued", message.DateIssued);
				msgParams.Add("StartDate", message.StartDate);
				if (message.ExpiryDate != null)
				{
					msgParams.Add("ExpiryDate", (DateTime)message.ExpiryDate);
				}
				if (message.DisplayOrder != null)
				{
					msgParams.Add("DisplayOrder", (int)message.DisplayOrder);
				}
				APIStruct rv = new APIStruct() { Name = "MemberMessage", IsRequired = false, Parms = msgParams };

				return rv;
			}
		}
	}
}
