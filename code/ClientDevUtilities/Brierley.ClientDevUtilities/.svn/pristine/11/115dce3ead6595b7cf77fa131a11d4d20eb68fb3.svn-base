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
    public class DeactivateCard : OperationProviderBase
    {
        #region Fields
        //private const string _className = "DeactivateCard";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public DeactivateCard() : base("DeactivateCard") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for deactivate member.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string cardId = (string)args["CardID"];
                DateTime effectiveDate = args.ContainsKey("EffectiveDate") ? (DateTime)args["EffectiveDate"] : DateTime.Now;
                string reason = args.ContainsKey("UpdateCardStatusReason") ? (string)args["UpdateCardStatusReason"] : string.Empty;
                
                if (string.IsNullOrEmpty(cardId))
                {
                    throw new LWOperationInvocationException("No card id provided for member search.") { ErrorCode = 3304 };
                }

                Member member = LoyaltyDataService.LoadMemberFromLoyaltyID(cardId);                             
                if (member == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member with card id = {0}.", cardId)) { ErrorCode = 3305 };
                }
                VirtualCard vc = member.GetLoyaltyCard(cardId);
                if (vc == null)
                {
                    throw new LWOperationInvocationException(string.Format("Member has no card with id = {0}.", cardId)) { ErrorCode = 3306 };
                }
                if (member.MemberStatus != MemberStatusEnum.Active)
                {
                    throw new LWOperationInvocationException(string.Format("Member is not active.  Its card canot be activated.")) { ErrorCode = 3314 };
                }
                switch(vc.Status)
                {                    
                    case VirtualCardStatusType.InActive:                        
                        break;
                    case VirtualCardStatusType.Active:
                        vc.NewStatus = VirtualCardStatusType.InActive;
                        vc.NewStatusEffectiveDate = effectiveDate;
                        vc.StatusChangeReason = reason;
                        LoyaltyDataService.SaveMember(member);
                        break;
                    case VirtualCardStatusType.Cancelled:
                        throw new LWOperationInvocationException(string.Format("Virtual card has been cancelled.  Its card canot be activated.")) { ErrorCode = 3307 };                        
                    case VirtualCardStatusType.Replaced:
                        throw new LWOperationInvocationException(string.Format("Virtual card has been replaced.  Its card canot be activated.")) { ErrorCode = 3307 };
                }

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("card", vc);
                PostProcessSuccessfullInvocation(context);
                #endregion

                return response;
            }
            catch (LWOperationInvocationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message);
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
