using System;
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
    public class ReplaceCard : OperationProviderBase
    {
        #region Fields
        //private const string _className = "ReplaceCard";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public ReplaceCard() : base("ReplaceCard") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for replace card.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string cardId = args.ContainsKey("CardID") ? (string)args["CardID"] : string.Empty;
                string replacementCardId = args.ContainsKey("ReplacementCardID") ? (string)args["ReplacementCardID"] : string.Empty;                
                DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
                bool transferPoints = args.ContainsKey("TransferPoints") ? (bool)args["TransferPoints"] : false;
                string reason = args.ContainsKey("UpdateCardStatusReason") ? (string)args["UpdateCardStatusReason"] : string.Empty;
                                
                if (string.IsNullOrEmpty(cardId))
                {
                    throw new LWOperationInvocationException("No card id provided for member lookup.") { ErrorCode = 3304 };
                }

                Member member = LoyaltyDataService.LoadMemberFromLoyaltyID(cardId);                             
                if (member == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member with card id = {0}.",cardId)) { ErrorCode = 3305 };
                }
                if (member.MemberStatus != MemberStatusEnum.Active)
                {
                    throw new LWOperationInvocationException(string.Format("Member is not active.")) { ErrorCode = 3314 };
                }
                VirtualCard vc = member.GetLoyaltyCard(cardId);
                if (vc == null)
                {
                    throw new LWOperationInvocationException(string.Format("Member has no card with id = {0}.", cardId)) { ErrorCode = 3306 };
                }
                switch(vc.Status)
                {
                    case VirtualCardStatusType.Active:
                        LoyaltyDataService.ReplaceVirtualCard(member, vc.VcKey, replacementCardId, effectiveDate, transferPoints, reason);                         
                        break;
                    default:                    
                        throw new LWOperationInvocationException(string.Format("Invalid card status.")) { ErrorCode = 3307 };
                }

                APIArguments resultParams = new APIArguments();
                resultParams.Add("member", member);
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);


                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("card", vc);
                context.Add("replacementCardId", replacementCardId);
                PostProcessSuccessfullInvocation(context);
                #endregion

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
