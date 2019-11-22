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
    public class ChangeCardExpirationDate : OperationProviderBase
    {
        #region Fields
        //private const string _className = "ChangeCardExpirationDate";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public ChangeCardExpirationDate() : base("ChangeCardExpirationDate") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for changing card date.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string cardId = (string)args["CardID"];
                DateTime? newExpirationDate = args.ContainsKey("NewExpirationDate") ? (DateTime?)args["NewExpirationDate"] : null;                
                                
                if (string.IsNullOrEmpty(cardId))
                {
                    throw new LWOperationInvocationException("No card id provided for member lookup.") { ErrorCode = 3304 };
                }

                if (newExpirationDate == null)
                {
                    throw new LWOperationInvocationException("No card expiration date provided.") { ErrorCode = 3384 };
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

                vc.ExpirationDate = newExpirationDate;
                LoyaltyDataService.SaveMember(member);

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
