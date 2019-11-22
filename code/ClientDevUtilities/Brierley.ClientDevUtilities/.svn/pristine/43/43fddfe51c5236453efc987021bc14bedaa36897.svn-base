//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;

using Brierley.FrameWork.Rules.UIDesign;

namespace Brierley.FrameWork.Rules
{
    #region Results
    public class IssueRewardRuleResult : ContextObject.RuleResult
    {
        public long RewardId = -1; // Values: -1: none awarded, 0: more than one awarded, >0: one awarded
        public string CertNmbr;
        public string RewardIssued;
        public decimal Points;
    }
    #endregion

	public class RewardRuleUtil : CertificateUtil
	{
		private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string className = "RewardRuleUtil";

		private PointsConsumptionOnIssueReward pointsConsumption = PointsConsumptionOnIssueReward.Consume;
        private string emailName = string.Empty;        
        private string changedByExpression = string.Empty;
        private bool markAsRedeemed = false;

		#region Helper Methods		
        
		public void CheckLowCertificateThreshold(Member member, RewardDef rdef)
		{
            CheckLowCertificateThreshold(member, ContentObjType.Reward, rdef.CertificateTypeCode, rdef.Threshold);            
		}

        public void SendIssueRewardEmail(Member member, RewardDef rdef, MemberReward reward, MemberOrder order, string CertificateNmbr)
		{
			if (!string.IsNullOrEmpty(emailName))
			{
				Product p = rdef.Product;
				if (p == null)
				{
					using (var content = LWDataServiceUtil.ContentServiceInstance())
					{
						p = content.GetProduct(rdef.ProductId);
					}
				}
				Dictionary<string, string> fields = new Dictionary<string, string>();
                if (order != null && !string.IsNullOrWhiteSpace(order.EmailAddress))
                {
                    fields.Add("RecipientEmail", order.EmailAddress);
                }
				fields.Add("ItemName", p.Name);
				fields.Add("ItemPrice", rdef.HowManyPointsToEarn.ToString());
				fields.Add("CertificateNmbr", CertificateNmbr);
                SendTriggeredEmail(member, emailName, fields);
			}
		}

        public long IssueRewardCertificate(
            ContextObject context, 
            Member member, 
            RewardDef rdef, 
            DateTime? expiryDate, 
            RewardFulfillmentOption fulfillmentOption, 
            long? fulfillmentProviderId, 
            ref string certNmbr, 
            long variantId, 
            string lworderNumber, 
            string fporderNumber,
            string changedBy,
            decimal? pointsConsumed = null,
            string fromCurrency = null,
            string toCurrency = null,
            decimal? conversionRate = null,
            decimal? exchangeRate = null,
            decimal? monetaryValue = null,
            decimal? cartTotalMonetaryValue = null)
		{
			string methodName = "IssueRewardCertificate";

            string msg = string.Format("Issuing reward {0} for member {1}.", rdef.Name, member.MyKey);

            if (member.MemberStatus == MemberStatusEnum.NonMember)
            {
                msg = string.Format("Cannot issue reward to non-member with ipcode {0}.", member.IpCode);
                logger.Error(className, methodName, msg);
                throw new LWDataServiceException(msg) { ErrorCode = 9969 };
            }
			
			logger.Trace(className, methodName, msg);

			MemberReward reward = new MemberReward();
			reward.RewardDefId = rdef.Id;
            reward.OfferCode = OfferCode;
            reward.LWOrderNumber = lworderNumber;
            reward.FPOrderNumber = fporderNumber;
            reward.FulfillmentProviderId = fulfillmentProviderId;
            reward.PointsConsumed = pointsConsumed;
            reward.FromCurrency = fromCurrency;
            reward.ToCurrency = toCurrency;
            reward.PointConversionRate = conversionRate;
            reward.ExchangeRate = exchangeRate;
            reward.MonetaryValue = monetaryValue;
            reward.CartTotalMonetaryValue = cartTotalMonetaryValue;
            if (markAsRedeemed)
            {
                reward.RedemptionDate = DateTime.Now;
            }

			using (var content = LWDataServiceUtil.ContentServiceInstance())
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				reward.FulfillmentOption = fulfillmentOption;
				reward.MemberId = long.Parse(member.MyKey.ToString());
				if (variantId > 0)
				{
					ProductVariant pv = content.GetProductVariant(variantId);
					reward.ProductId = pv.ProductId;
					reward.ProductVariantId = variantId;
				}
				else
				{
					reward.ProductId = rdef.ProductId;
				}
				reward.DateIssued = DateTime.Now;
				reward.Expiration = expiryDate;

				if (!string.IsNullOrWhiteSpace(changedBy))
				{
					reward.ChangedBy = changedBy;
				}
				else if (!string.IsNullOrEmpty(changedByExpression))
				{
					ExpressionFactory exprF = new ExpressionFactory();
					if (context == null)
					{
						context = new ContextObject() { Owner = member };
					}
					reward.ChangedBy = exprF.Create(this.changedByExpression).evaluate(context).ToString();
				}

				PromotionCertificate cert = null;
				if (string.IsNullOrEmpty(certNmbr))
				{
					cert = IssueCertificate(ContentObjType.Reward, rdef.CertificateTypeCode);
				}

				try
				{
					if (string.IsNullOrEmpty(certNmbr))
					{
						if (cert != null)
						{
							reward.CertificateNmbr = cert.CertNmbr;
                            certNmbr = cert.CertNmbr;
							cert.Available = false;
						}
					}
					else
					{
						msg = string.Format("Using certificate number {0} as provided.", certNmbr);
						logger.Trace(className, methodName, msg);
						reward.CertificateNmbr = certNmbr;
					}
                    loyalty.CreateMemberReward(reward);
                    return reward.Id;
				}
				catch (Exception ex)
				{
					logger.Error(className, methodName, "Error issuing reward.", ex);
					throw;
				}
				finally
				{
				}
			}
		}

        private string[] GetPointTypes(RewardDef rdef, ServiceConfig config)
        {
			using (var svc = new LoyaltyDataService(config))
			{
				string[] pointTypes = rdef.GetPointTypes();
				if (pointTypes == null || pointTypes.Length < 1)
				{
					IList<PointType> listPointTypes = svc.GetAllPointTypes();
					pointTypes = listPointTypes.Select(s => s.Name).ToArray();
				}
				return pointTypes;
			}
        }

        private string[] GetPointEvents(RewardDef rdef, ServiceConfig config)
        {
			using (var svc = new LoyaltyDataService(config))
			{
				string[] pointEvents = rdef.GetPointEvents();
				if (pointEvents == null || pointEvents.Length < 1)
				{
					IList<PointEvent> listPointEvents = svc.GetAllPointEvents();
					pointEvents = listPointEvents.Select(s => s.Name).ToArray();
				}
				return pointEvents;
			}
        }

        public decimal GetPoints(RewardDef rdef, string[] pointTypes, string[] pointEvents, Member lwmember, VirtualCard lwvirtualCard)
		{
            //string methodName = "GetPoints";

			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				DateTime from = DateTimeUtil.MinValue;
				DateTime to = DateTimeUtil.MaxValue;

				long[] vcKeys = null;
				long[] ptKeys = null;
				long[] peKeys = null;

				if (lwvirtualCard != null)
				{
					vcKeys = new long[1];
					vcKeys[0] = lwvirtualCard.VcKey;
				}
				else
				{
					var cardKeys = (from vc in lwmember.LoyaltyCards where vc.IsValid() select vc.VcKey);
					vcKeys = cardKeys.ToArray<long>();
				}

				if (pointTypes != null && pointTypes.Length > 0)
				{
					IList<PointType> ptList = new List<PointType>();
					foreach (string pointTypeName in pointTypes)
					{
						PointType pt = svc.GetPointType(pointTypeName);
						if (pt == null)
						{
							throw new LWException("No point type could be found with name " + pointTypeName + ".") { ErrorCode = 1 };
						}
						ptList.Add(pt);
					}
					ptKeys = (from x in ptList select x.ID).ToArray<long>();
				}
				if (pointEvents != null && pointEvents.Length > 0)
				{
					IList<PointEvent> peList = new List<PointEvent>();
					foreach (string pointEventName in pointEvents)
					{
						PointEvent pe = svc.GetPointEvent(pointEventName);
						if (pe == null)
						{
							throw new LWException("No point event could be found with name " + pointEventName + ".") { ErrorCode = 1 };
						}
						peList.Add(pe);
					}
					peKeys = (from x in peList select x.ID).ToArray<long>();
				}
				return svc.GetPointBalance(vcKeys, ptKeys, peKeys, null, from, to, null, null, null, null, null, null, null);
			}
		}

        public decimal GetOnHoldPoints(RewardDef rdef, string[] pointTypes, string[] pointEvents, Member lwmember, VirtualCard lwvirtualCard)
		{
			DateTime from = DateTimeUtil.MinValue;
			DateTime to = DateTimeUtil.MaxValue;
			
            IList<VirtualCard> vcList = null;
            IList<PointType> ptList = null;
            IList<PointEvent> peList = null;
            if (lwvirtualCard == null)
            {
                vcList = lwmember.LoyaltyCards;
            }
            else
            {
                vcList = new List<VirtualCard>();
                vcList.Add(lwvirtualCard);
            }

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{

				if (pointTypes != null && pointTypes.Length > 0)
				{
					ptList = new List<PointType>();
					foreach (string pointTypeName in pointTypes)
					{
						PointType pt = loyalty.GetPointType(pointTypeName);
						if (pt == null)
						{
							throw new LWException("No point type could be found with name " + pointTypeName + ".") { ErrorCode = 1 };
						}
						ptList.Add(pt);
					}
				}
				if (pointEvents != null && pointEvents.Length > 0)
				{
					peList = new List<PointEvent>();
					foreach (string pointEventName in pointEvents)
					{
						PointEvent pe = loyalty.GetPointEvent(pointEventName);
						if (pe == null)
						{
							throw new LWException("No point event could be found with name " + pointEventName + ".") { ErrorCode = 1 };
						}
						peList.Add(pe);
					}
				}

				return loyalty.GetPointsOnHold(vcList, ptList, peList, from, to);
			}
		}

		public IssueRewardRuleResult ConsumePoints(Member lwmember, VirtualCard lwvirtualCard, RewardDef rdef, long rewardId, MemberOrder order, IssueRewardRuleResult result, decimal? pointsToConsume = null)
		{
            string methodName = "ConsumePoints";

			
			// consume points.            
			long rowkey = rewardId;

            IList<VirtualCard> vcList = new List<VirtualCard>();
            IList<PointType> ptList = new List<PointType>();
            IList<PointEvent> peList = new List<PointEvent>();

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				string[] pointTypes = rdef.GetPointTypes();
				if (pointTypes != null && pointTypes.Length > 0)
				{
					ptList = new List<PointType>();
					foreach (string pointTypeName in pointTypes)
					{
						PointType pt = loyalty.GetPointType(pointTypeName);
						if (pt == null)
						{
							throw new LWException("No point type could be found with name " + pointTypeName + ".") { ErrorCode = 1 };
						}
						ptList.Add(pt);
					}
				}
				else
				{
					// use all points
					ptList = loyalty.GetAllPointTypes();
				}

				string[] pointEvents = rdef.GetPointEvents();
				if (pointEvents != null && pointEvents.Length > 0)
				{
					peList = new List<PointEvent>();
					foreach (string pointEventName in pointEvents)
					{
						PointEvent pe = loyalty.GetPointEvent(pointEventName);
						if (pe == null)
						{
							throw new LWException("No point event could be found with name " + pointEventName + ".") { ErrorCode = 1 };
						}
						peList.Add(pe);
					}
				}
				else
				{
					// use all event types
					peList = loyalty.GetAllPointEvents();
				}

				if (lwvirtualCard != null)
				{
					vcList.Add(lwmember.GetLoyaltyCard(lwvirtualCard.VcKey));
				}
				else
				{
					vcList = lwmember.LoyaltyCards;
				}

                decimal pointsLeftToConsume = pointsToConsume.HasValue ? pointsToConsume.Value : rdef.HowManyPointsToEarn;
				decimal consumed = loyalty.ConsumePoints(vcList, ptList, peList, DateTime.Now, pointsLeftToConsume, PointTransactionOwnerType.Reward, string.Empty, rdef.Id, rowkey);
				if (consumed < pointsLeftToConsume)
				{
					string errMsg = string.Format("Unable to consume {0} points for reward {1} for member {2}",
						pointsLeftToConsume, rdef.Name, lwmember.IpCode);
					logger.Error(className, methodName, errMsg);
					throw new LWRulesException(errMsg);
				}

				result.OwnerType = PointTransactionOwnerType.Reward;
				result.OwnerId = rdef.Id;
				result.RowKey = rewardId;
				result.Points = consumed;

				return result;
			}
		}

		public IssueRewardRuleResult HoldPoints(Member lwmember, VirtualCard lwvirtualCard, RewardDef rdef, long rewardId, IssueRewardRuleResult result)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				string notes = "Points put on hold for member reward.";

				var ptList = new List<PointType>();
				var peList = new List<PointEvent>();

				string[] pointTypes = rdef.GetPointTypes();
				if (pointTypes != null && pointTypes.Length > 0)
				{
					foreach (string pointTypeName in pointTypes)
					{
						PointType pt = loyalty.GetPointType(pointTypeName);
						if (pt == null)
						{
							throw new LWException("No point type could be found with name " + pointTypeName + ".") { ErrorCode = 1 };
						}
						ptList.Add(pt);
					}
				}
				else
				{
					ptList = loyalty.GetAllPointTypes();
				}

				string[] pointEvents = rdef.GetPointEvents();
				if (pointEvents != null && pointEvents.Length > 0)
				{
					peList = new List<PointEvent>();
					foreach (string pointEventName in pointEvents)
					{
						PointEvent pe = loyalty.GetPointEvent(pointEventName);
						if (pe == null)
						{
							throw new LWException("No point event could be found with name " + pointEventName + ".") { ErrorCode = 1 };
						}
						peList.Add(pe);
					}
				}
				else
				{
					peList = loyalty.GetAllPointEvents();
				}

				foreach (PointType ptype in ptList)
				{
					if (lwvirtualCard != null)
					{
						foreach (PointEvent pe in peList)
						{
							loyalty.HoldPoints(lwvirtualCard, ptype, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
						}
					}
					else
					{
						foreach (PointEvent pe in peList)
						{
							lwmember.HoldPoints(ptype, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
						}
					}
				}

				result.OwnerType = PointTransactionOwnerType.Reward;
				result.OwnerId = rdef.Id;
				result.RowKey = rewardId;
				result.Points = rdef.HowManyPointsToEarn;

				return result;
			}
		}

		#endregion

		#region Properties

		public PointsConsumptionOnIssueReward PointsConsumption
		{
			get
			{
				return pointsConsumption;
			}
			set
			{
				pointsConsumption = value;
			}
		}

        public string ChangedByExpression
        {
            get { return changedByExpression; }
            set { changedByExpression = value; }
        }

        public bool MarkAsRedeemed
        {
            get { return markAsRedeemed; }
            set { markAsRedeemed = value; }
        }

        public string TriggeredEmailName
        {
            get
            {
                if (emailName != null)
                {
                    return emailName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                emailName = value;
            }
        }                
		#endregion

	}
}
