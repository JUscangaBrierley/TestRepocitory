using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Messaging.Consumers
{
	public class BusinessRuleQueueConsumer : IConsumer
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_QueueProcessor);

		public void Consume(object msg)
		{
			var message = (RuleMessage)msg;
			_logger.Debug("BusinessRuleQueueConsumer", "Consume", string.Format("consuming rule message {0} for member", message.MemberId.ToString()));

			using (var l = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				var member = l.LoadMemberFromIPCode(message.MemberId);
				if (member == null)
				{
					throw new Exception(string.Format("Failed to load member with IPCode {0}. The member does not exist.", message.MemberId));
				}

				IAttributeSetContainer owner = null;
				if (message.VCKey > 0)
				{
					owner = member.LoyaltyCards.FirstOrDefault(o => o.VcKey == message.VCKey);
					if (owner == null)
					{
						throw new Exception(string.Format("Failed to locate member virtual card by VCKey {0}. The card either does not exist or does not belong to the member", message.VCKey));
					}
				}
				else
				{
					owner = member;
				}

				using (var txn = l.Database.GetTransaction())
				{

					var co = new ContextObject() { Owner = owner, Environment = message.ContextEnvironment ?? new Dictionary<string, object>() };
					if (!string.IsNullOrEmpty(message.EventName))
					{
						//process event rules
						List<RuleTrigger> rules = l.GetRuleByObjectName(message.EventName);
						foreach (RuleTrigger rule in rules.ToList())
						{
							if (rule.CanQueue && rule.ProperInvocationType == message.InvocationType)
							{
								l.Execute(rule, co);
							}
						}
					}
					else
					{
						//process attribute set rules
						string attributesetName = message.AttributeSetName;
						if (string.IsNullOrEmpty(attributesetName) && message.AttributeSetId > 0)
						{
							AttributeSetMetaData meta = l.GetAttributeSetMetaData(message.AttributeSetId);
							if (meta == null)
							{
								throw new Exception(string.Format("Failed to load attribute set meta data for set id {0} or by name {1}. The attribute set does not exist", message.AttributeSetId, message.AttributeSetName));
							}
							attributesetName = meta.Name;
						}
						var row = l.GetAttributeSetObject(attributesetName, message.InvokingRow, true, false);
						if (row == null)
						{
							throw new Exception(string.Format("Failed to retrieve triggering row with id {0} from attribute set {1}. The row does not exist.", message.InvokingRow, attributesetName));
						}
						co.InvokingRow = row;
						var triggers = row.GetMetaData().RuleTriggers;
						foreach (var trigger in triggers.Where(o => o.ProperInvocationType == message.InvocationType && o.CanQueue))
						{
							l.Execute(trigger, co);
						}
					}
					txn.Complete();
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
