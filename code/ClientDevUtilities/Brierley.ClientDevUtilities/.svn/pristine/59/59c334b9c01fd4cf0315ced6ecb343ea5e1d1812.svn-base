using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class SetRewardChoice : OperationProviderBase
    {
        public SetRewardChoice() : base("SetRewardChoice") { }

        public override string Invoke(string source, string parms)
        {
            if (string.IsNullOrEmpty(parms))
            {
                throw new LWOperationInvocationException("No parameters provided for setting reward choice.") { ErrorCode = 3300 };
            }

            try
            {
                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                Member member = LoadMember(args);
                long rewardId = (long)args["RewardId"];
                base.LoyaltyDataService.SetMemberRewardChoice(member.IpCode, rewardId);
                return null;
            }
            catch (LWException ex)
            {
                //error code does not have to be exclusive to CDIS.
                if(ex.ErrorCode > 0)
                {
                    throw new LWOperationInvocationException(ex.Message) { ErrorCode = ex.ErrorCode };
                }
                throw;
            }
            catch (Exception ex)
            {
                int errorCode = 1;
                if (ex.Message.Contains("The member does not belong to a tier"))
                {
                    errorCode = 10006;
                }

                throw new LWOperationInvocationException(ex.Message) { ErrorCode = errorCode };
            }
        }
    }
}