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
    public class TransferCard : OperationProviderBase
    {
        public TransferCard() : base("TransferCard") { }

		public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for transfer card.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string cardId = (string)args["CardID"];
                bool makeCardPrimary = args.ContainsKey("MakeCardPrimary") ? (bool)args["MakeCardPrimary"] : false;
                bool deactivateMember = args.ContainsKey("DeactivateMember") ? (bool)args["DeactivateMember"] : false;

                Member memberFrom = LoyaltyDataService.LoadMemberFromLoyaltyID(cardId);
                if (memberFrom == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member with card id = {0}.", cardId)) { ErrorCode = 3305 };
                }
                if (memberFrom.MemberStatus != MemberStatusEnum.Active)
                {
                    throw new LWOperationInvocationException(string.Format("Member is not active.")) { ErrorCode = 3314 };
                }
                VirtualCard vc = memberFrom.GetLoyaltyCard(cardId);
                if (vc == null)
                {
                    throw new LWOperationInvocationException(string.Format("Member has no card with id = {0}.", cardId)) { ErrorCode = 3306 };
                }

                Member memberTo = LoadMember(args);
                if (memberTo.MemberStatus != MemberStatusEnum.Active)
                {
                    throw new LWOperationInvocationException(string.Format("Member is not active.")) { ErrorCode = 3314 };
                }

                if (memberFrom.IpCode == memberTo.IpCode)
                {
                    // they are both same mebers
                    throw new LWOperationInvocationException(string.Format("Transfer card requires the target member to be different from the member containing the card being transferred.")) { ErrorCode = 3349 };
                }

                switch (vc.Status)
                {
                    case VirtualCardStatusType.Active:
                        LoyaltyDataService.TransferVirtualCard(vc, memberTo, makeCardPrimary, deactivateMember);                        
                        break;
                    default:
                        throw new LWOperationInvocationException(string.Format("Invalid card status.")) { ErrorCode = 3307 };
                }

                APIArguments resultParams = new APIArguments();
                resultParams.Add("member", memberTo);
                resultParams.Add("card", vc);
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("card", vc);
                context.Add("memberTo", memberTo);
                PostProcessSuccessfullInvocation(context);

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
    }
}
