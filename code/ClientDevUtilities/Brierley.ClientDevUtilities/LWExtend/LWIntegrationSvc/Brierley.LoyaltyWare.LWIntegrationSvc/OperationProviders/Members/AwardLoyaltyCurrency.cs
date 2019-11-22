using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class AwardLoyaltyCurrency : OperationProviderBase
    {
        #region Fields
        private const string _className = "AwardLoyaltyCurrency";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public AwardLoyaltyCurrency() : base("AwardLoyaltyCurrency") { }
        #endregion

        #region Helper Methods        
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for add loyalty event.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string cardId = (string)args["CardID"];
                string pointEventName = (string)args["LoyaltyEvent"];
                string pointTypeName = (string)args["LoyaltyCurrency"];
                decimal lpoints = args.ContainsKey("CurrencyAmount") ? (decimal)args["CurrencyAmount"] : -1;
                DateTime txnDate = args.ContainsKey("TransactionDate") ? (DateTime)args["TransactionDate"] : DateTime.Now;                
                string note = args.ContainsKey("Note") ? (string)args["Note"] : string.Empty;
                string changedBy = args.ContainsKey("ChangedBy") ? (string)args["ChangedBy"] : string.Empty;

                if (!args.ContainsKey("ExpirationDate"))
                {
                    throw new LWOperationInvocationException("No expiration date provovided.") { ErrorCode = 3341 };
                }

                DateTime expDate = (DateTime)args["ExpirationDate"];

                if (string.IsNullOrEmpty(cardId))
                {
                    throw new LWOperationInvocationException("No card id provided for member lookup.") { ErrorCode = 3304 };
                }
                if (string.IsNullOrEmpty(pointEventName))
                {
                    throw new LWOperationInvocationException("No loyalty event provided.") { ErrorCode = 3308 };
                }
                if (string.IsNullOrEmpty(pointTypeName))
                {
                    throw new LWOperationInvocationException("No loyalty currency provided.") { ErrorCode = 3309 };
                }

                Member member = LoyaltyDataService.LoadMemberFromLoyaltyID(cardId);                             
                if (member == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member with loyalty id = {0}.", cardId)) { ErrorCode = 3305 };
                }

                if (member.MemberStatus != MemberStatusEnum.Active && member.MemberStatus != MemberStatusEnum.NonMember && member.MemberStatus != MemberStatusEnum.PreEnrolled)
                {
                    throw new LWOperationInvocationException(string.Format("Member is not active.  No points can be awarded.")) { ErrorCode = 3314 };
                }

                PointEvent pe = LoyaltyDataService.GetPointEvent(pointEventName);
                if (pe == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find loyalty event {0}.", pointEventName)) { ErrorCode = 3310 };
                }

                PointType pt = LoyaltyDataService.GetPointType(pointTypeName);
                if (pt == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find loyalty currency {0}.", pointTypeName)) { ErrorCode = 3311 };
                }

                decimal points = -1;
                if (lpoints == -1)
                {
                    if (pe.DefaultPoints == null)
                    {
                        throw new LWOperationInvocationException(string.Format("Unable to find loyalty currency {0}.", pointTypeName)) { ErrorCode = 3312 };
                    }
                    else
                    {
                        points = (decimal)pe.DefaultPoints;
                    }
                }
                else
                {
                    points = (decimal)lpoints;
                }

                if (points == 0)
                {
                    throw new LWOperationInvocationException(string.Format("{0} points were requested.  Loyalty currency canot be 0.", points)) { ErrorCode = 3313 };
                }

                #region Check for correct points
                bool allowNegativePoints = true;
                bool allowFractionalPoints = true;
                bool roundUpToNextWholeNumber = false;
                if (!string.IsNullOrEmpty(GetFunctionParameter("AllowNegativePoints")))
                {
                    allowNegativePoints = bool.Parse(GetFunctionParameter("AllowNegativePoints"));
                }
                if (points < 0 && !allowNegativePoints)
                {
                    string errMsg = string.Format("Negative points cannot be awarded.");
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWOperationInvocationException(errMsg) { ErrorCode = 3313 }; //TODO: assign correct error code
                }

                if (!string.IsNullOrEmpty(GetFunctionParameter("AllowFractionalPoints")))
                {
                    allowFractionalPoints = bool.Parse(GetFunctionParameter("AllowFractionalPoints"));
                }

                if (allowFractionalPoints)
                {
                    decimal p = System.Math.Round(points);
                    if (p != points)
                    {
                        string errMsg = string.Format("Fractional points cannot be awarded.");
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWOperationInvocationException(errMsg) { ErrorCode = 3313 }; //TODO: assign correct error code
                    }
                }

                if (!string.IsNullOrEmpty(GetFunctionParameter("RoundPointsToNextWholeNumber")))
                {
                    roundUpToNextWholeNumber = bool.Parse(GetFunctionParameter("RoundPointsToNextWholeNumber"));
                }
                if (roundUpToNextWholeNumber)
                {
                    decimal p = System.Math.Ceiling(points);
                    if (p != points)
                    {
                        string errMsg = string.Format("Fractional points {0} are being rounded up to {1}.", points, p);
                        _logger.Debug(_className, methodName, errMsg);
                        points = p;
                    }
                }

                #endregion

                VirtualCard vc = member.GetLoyaltyCard(cardId);
                if (vc == null)
                {
                    throw new LWOperationInvocationException(string.Format("Member has no card with id = {0}.", cardId)) { ErrorCode = 3306 };
                }
                if (vc.Status != VirtualCardStatusType.Active)
                {
                    throw new LWOperationInvocationException(string.Format("VirtualCard is not active.  No points can be awarded.")) { ErrorCode = 3307 };
                }

                if (points > 0)
                {
                    LoyaltyDataService.Credit(vc, pt, pe, points, string.Empty, txnDate, expDate, PointTransactionOwnerType.Unknown, -1, -1, note, null, changedBy);
                    _logger.Trace(_className, methodName,
                        string.Format("{0} points credited to virtual card {1}", points, vc.LoyaltyIdNumber));
                }
                else
                {
                    points = Math.Abs(points);
                    LoyaltyDataService.Debit(vc, pt, pe, points, txnDate, expDate, PointTransactionOwnerType.Unknown, -1, -1, note, null, changedBy);
                    _logger.Trace(_className, methodName,
                        string.Format("{0} points debited from virtual card {1}", points, vc.LoyaltyIdNumber));
                }

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                PostProcessSuccessfullInvocation(context);
                #endregion

                APIArguments resultParams = new APIArguments();
                resultParams.Add("CurrencyBalance", member.GetPoints(null, null, null, null).ToString());                
                resultParams.Add("CurrencyToNextTier", member.GetPointsToNextTier());
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                return response;                
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
