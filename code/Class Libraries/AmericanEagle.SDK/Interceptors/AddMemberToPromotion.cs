using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AmericanEagle.SDK.Interceptors
{
    public class AddMemberToPromotion : IInboundInterceptor
    {
        #region Fields
        private readonly LWLogger _logger = LWLoggerManager.GetLogger("AEAddMemberToPromotion");
        private const string ClassName = "AddMemberToPromotion";
        private NameValueCollection parms;
        private const string region = "AmericanEagleCache";
        private string promotionKey = "PromotionsList";
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        #endregion

        #region Overrides
        /// <summary>
        /// Loads the member
        /// </summary>
        /// <param name="config">The config</param>
        /// <param name="memberNode">The member</param>
        /// <returns>The member</returns>
        public virtual Member LoadMember(LWIntegrationConfig config, XElement memberNode)
        {
            //string _errorMessage;
            //// 8000 : Customer ID
            //string cust_id = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/AlternateID");
            //if (string.IsNullOrEmpty(cust_id))
            //{
            //    _errorMessage = "CustomerID is required.";
            //    _logger.Error(ClassName, "LoadMember", _errorMessage);
            //    _errorMessage = "8000";
            //    throw new LWException(_errorMessage) { ErrorCode = 8000 };
            //}
            //return LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromAlternateID(cust_id);
            return null;
        }

        /// <summary>
        /// Processes the member before population.  This method is used exclusively for updating a member.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberNode">The member node.</param>
        /// <returns></returns>
        public virtual Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            return member;
        }

        /// <summary>
        /// Processes the member before save.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberNode">The member node.</param>
        /// <returns></returns>
        public virtual Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            /*
            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            {
                ldService.SaveMember(member);
            }
            */
            return member;
        }

        /// <summary>
        /// Processes the member after save.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="member">The member.</param>
        /// <param name="memberNode">The member node.</param>
        /// <returns></returns>
        public virtual Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, System.Collections.Generic.IList<Brierley.FrameWork.ContextObject.RuleResult> results)
        {

            MemberPromotion mp = new MemberPromotion();
            mp.Code = LWIntegrationUtilities.GetNodeValueByPath(memberNode, "Member/CODE");
            mp.MemberId = member.IpCode;
            mp.Enrolled = true;

            //using (var dataService = LWDataServiceUtil.LoyaltyDataServiceInstance())
            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            { ldService.CreateMemberPromotion(mp); }

            return member;
        }
        #endregion        

        public void HandleMemberNotFound(LWIntegrationConfig config, XElement memberNode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that can to process the raw xml. for e.g if we need to any new field which is not there in xml
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Return current member node</returns>
        public virtual XElement ProcessRawXml(LWIntegrationConfig config, XElement memberNode)
        {
            return memberNode;
        }

        /// <summary>
        /// Initialize with any parameters
        /// </summary>
        /// <param name="parameters">List of parameters</param>
        public virtual void Initialize(NameValueCollection parameters)
        {
            this.parms = parameters;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ValidateOperationParameter(string operationName, string source, string payload)
        {
            throw new NotImplementedException();
        }
    }
}

