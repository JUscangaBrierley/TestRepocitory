using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetPointsByOwnerType function the members points value for the given owner type and owner id.
    /// </summary>
    /// <example>
    ///     Usage : GetPointsByOwnerType([OwnerType], [OwnerId], [RowKey] ,[Start Date], [End Date])
    /// </example>
    /// <remarks>
    ///     RowKey is optional.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the member's points value for the given owner type and owner id.",
        DisplayName = "GetPointsByOwnerType", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Point Balance by owner type",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Object Type", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which type of object?", Helpers = ParameterHelpers.OwnerType)]
    [ExpressionParameter(Order = 1, Name = "Object Id", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which object?")]
    [ExpressionParameter(Order = 2, Name = "Row Key", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Which rowkey?")]
    [ExpressionParameter(Order = 3, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "From which date?")]
    [ExpressionParameter(Order = 4, Name = "End Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "To which date?")]

    public class GetPointsByOwnerType:UnaryOperation
    {
        private Expression ownerTypeExpression = null;
        private Expression ownerIdExpression = null;
        private Expression rowKeyExpression = null;
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointsByOwnerType()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointsByOwnerType(Expression rhs)
            : base("GetPointsByOwnerType", rhs)
        {
			ParameterList plist = rhs as ParameterList;
            if (plist != null)
            {
                if (plist.Expressions.Length <= 1 || plist.Expressions.Length > 5)
                {
                    throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPointsByOwnerType.");
                }

                ownerTypeExpression = ((ParameterList)rhs).Expressions[0];

                if (plist.Expressions.Length >= 2)
                {
                    ownerIdExpression = ((ParameterList)rhs).Expressions[1];
                }

                if (plist.Expressions.Length >= 3)
                {
                    rowKeyExpression = ((ParameterList)rhs).Expressions[2];
                }

                if (plist.Expressions.Length >= 4)
                {
                    startDateExpression = ((ParameterList)rhs).Expressions[3];
                }

                if (plist.Expressions.Length == 5)
                {
                    endDateExpression = ((ParameterList)rhs).Expressions[4];
                }
            }
            else
            {
                ownerTypeExpression = this.GetRight();
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPointsByOwnerType(OwnerType, [OwnerId], [RowKey] ,[Start Date], [End Date])";
            }
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context provided at runtime</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);
            if (member != null)
            {
                string strOwnerType = (string)ownerTypeExpression.evaluate(contextObject);
                PointTransactionOwnerType ownerType = (PointTransactionOwnerType)Enum.Parse(typeof(PointTransactionOwnerType), strOwnerType);                
                long ownerId = -1;

                if (ownerIdExpression != null)
                {
                    string strOwnerId = (string)ownerIdExpression.evaluate(contextObject);
                    ownerId = long.Parse(strOwnerId);
                }
                else
                {
                    ownerId = long.Parse(contextObject.Environment["OwnerId"].ToString());
                    //ownerId = (long)contextObject.Environment["OwnerId"];
                }

				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{

					#region Get The Correct Id
					//switch (ownerType)
					//{
					//    case PointTransactionOwnerType.AttributeSet:
					//        AttributeSetMetaData attSetMeta = svc.GetAttributeSetMetaData(ownerName);
					//        if (attSetMeta != null)
					//        {
					//            ownerId = attSetMeta.ID;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an attribute set with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Store:
					//        IList<StoreDef> stores = svc.GetStoreDef(ownerName);
					//        if (stores != null && stores.Count > 0)
					//        {
					//            ownerId = stores[0].StoreId;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find store with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Bid:
					//        break;
					//    case PointTransactionOwnerType.Bonus:
					//        BonusDef bonus = svc.GetBonusDef(ownerName);
					//        if (bonus != null)
					//        {
					//            ownerId = bonus.Id;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an bonus with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Survey:
					//        ISurveyManager surveyManager = SurveyManager.Instance();
					//        SMSurvey survey = surveyManager.RetrieveSurvey(ownerName);
					//        if (survey != null)
					//        {
					//            ownerId = survey.ID;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an survey with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Reward:
					//        RewardDef reward = svc.GetRewardDef(ownerName);
					//        if (reward != null)
					//        {
					//            ownerId = reward.Id;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an reward with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Coupon:
					//        CouponDef coupon = svc.GetCouponDef(ownerName);
					//        if (coupon != null)
					//        {
					//            ownerId = coupon.Id;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an coupon with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Message:
					//        MessageDef message = svc.GetMessageDef(ownerName);
					//        if (message != null)
					//        {
					//            ownerId = message.Id;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an message with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Promotion:
					//        Promotion promo = svc.GetPromotionByName(ownerName);
					//        if (promo != null)
					//        {
					//            ownerId = promo.Id;
					//        }
					//        else
					//        {
					//            throw new LWBScriptException(string.Format("Unable to find an promotion with name {0}.", ownerName));
					//        }
					//        break;
					//    case PointTransactionOwnerType.Unknown:
					//        throw new LWBScriptException("Unknown object type provided.");
					//}
					#endregion

					long[] rowKeys = new long[1];
					if (rowKeyExpression != null)
					{
						string strRowKey = (string)rowKeyExpression.evaluate(contextObject);
						rowKeys[0] = long.Parse(strRowKey);
					}
					else
					{
						rowKeys[0] = long.Parse(contextObject.Environment["RowKey"].ToString());
						//rowKeys[0] = (long)contextObject.Environment["RowKey"];
					}

					DateTime? startDate = null;
					if (startDateExpression != null)
					{
						startDate = (DateTime)(startDateExpression.evaluate(contextObject));
					}

					DateTime? endDate = null;
					if (endDateExpression != null)
					{
						endDate = (DateTime)(endDateExpression.evaluate(contextObject));
					}

					if (startDate.HasValue && endDate.HasValue && DateTimeUtil.LessThan(endDate.Value, startDate.Value))
					{
						throw new CRMException("GetPointsByOwnerType: end date is less than the start date.");
					}

					#region Loyalty Cards
					IList<VirtualCard> validCards = new List<VirtualCard>();
					foreach (VirtualCard v in member.LoyaltyCards)
					{
						if (v.IsValid())
						{
							validCards.Add(v);
						}
					}
					long[] vcs = new long[validCards.Count];
					int idx = 0;
					foreach (VirtualCard v in validCards)
					{
						vcs[idx++] = v.VcKey;
					}
					#endregion

					decimal points = service.GetPointBalance(vcs, null, null, null, startDate, endDate, null, null, null, string.Empty,
						ownerType, ownerId, rowKeys);

					return points;
				}
            }
            else
            {
                throw new CRMException("GetPointsByOwnerType must be evaluated in the context of a loyalty member.");
            }
        }        
    }
}
