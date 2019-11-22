using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.Rules
{
    public static class ContextUtility
    {
        private const string CLASS_NAME = "ContextUtility";
        private static readonly LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public static ResolveOwnersResult ResolveOwners(IAttributeSetContainer owner)
        {
            const string METHOD_NAME = "ResolveOwners";

            if (owner == null)
            {
                _logger.Error(CLASS_NAME, METHOD_NAME, "Owner is null");
                return new ResolveOwnersResult();
            }

            if (owner is Member)
            {
                var result = new ResolveOwnersResult();
                result.Member = (Member)owner;
                result.VirtualCard = null;
                _logger.Debug(CLASS_NAME, METHOD_NAME, "Owner resolved from a member = Ipcode = " + result.Member.IpCode);

                return result;
            }
            
            if (owner is VirtualCard)
            {
                var result = new ResolveOwnersResult();
                result.VirtualCard = (VirtualCard)owner;
                result.Member = result.VirtualCard.Member;
                _logger.Debug(CLASS_NAME, METHOD_NAME,
                    string.Format("Owner resolved from virtual card. Member Ipcode = {0}, Virtual card key = {1}, Loyalty id = {2}.", result.Member.IpCode, result.VirtualCard.VcKey, result.VirtualCard.LoyaltyIdNumber));

                return result;
            }

            _logger.Debug(CLASS_NAME, METHOD_NAME,
            string.Format("Owner resolved from attribute set."));
            return ResolveOwners(owner.Parent);
        }

        public class ResolveOwnersResult
        {
            public Member Member { get; set; }
            public VirtualCard VirtualCard { get; set; }
        }
    }
}
