using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using System.IO;
using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, Namespace = "urn:Brierley.LoyaltyWare.LWMobileGateway")]
	public class MobileGateway : MobileGatewayImpl, IMobileGateway
	{
		#region Member Account

		#region Authentication
		public MGAuthenticateMember AuthenticateMember(string deviceType, string version, string username, string password, string resetcode)
		{
			string opName = "AuthenticateMember";
			object[] parms = new object[5] { deviceType, version, username, password, resetcode };
			return InvokeOperation(opName, parms) as MGAuthenticateMember;
		}

		public MGAuthenticateMember AuthenticateMemberBySocialHandle(string provider, string providerUId, string token, string secret, string deviceType, string version)
		{
			string opName = "AuthenticateMemberBySocialHandle";
			object[] parms = new object[6] { provider, providerUId, token, secret, deviceType, version };
			return InvokeOperation(opName, parms) as MGAuthenticateMember;
		}


		public void Logout()
		{
			string opName = "Logout";
			InvokeOperation(opName, null);
		}
		#endregion

		public MGMember GetMember()
		{
			string opName = "GetMember";
			return InvokeOperation(opName, null) as MGMember;
		}

		public MGAccountSummary GetAccountSummary()
		{
			string opName = "GetAccountSummary";
			return InvokeOperation(opName, null) as MGAccountSummary;
		}

		public MGAuthenticateMember CreateMember(MGMember member, string password, string executionmode, string deviceType, string version)
		{
			string opName = "CreateMember";
			object[] parms = new object[5] { member, password, executionmode, deviceType, version };
			return (MGAuthenticateMember)InvokeOperation(opName, parms);
		}

		public void UpdateMember(MGMember member, string executionmode)
		{
			string opName = "UpdateMember";
			object[] parms = new object[2] { member, executionmode };
			InvokeOperation(opName, parms);
		}

		public string ChangePassword(string username, string oldPassword, string newPassword)
		{
			string opName = "ChangePassword";
			object[] parms = new object[3] { username, oldPassword, newPassword };
			return (string)InvokeOperation(opName, parms);
		}

        public PasswordResetOptions InitiatePasswordReset(string username)
        {
            string opName = "InitiatePasswordReset";
            object[] parms = new object[1] { username };
            return (PasswordResetOptions)InvokeOperation(opName, parms);
        }

        public void ConfirmPasswordReset(string username, string channel)
        {
            string opName = "ConfirmPasswordReset";
            object[] parms = new object[2] { username, channel };
            InvokeOperation(opName, parms);
        }

		#region Loyalty Cards

        public void AddLoyaltyCard(MGLoyaltyCard loyaltyCard, string executionmode)
        {
            string opName = "AddLoyaltyCard";
            object[] parms = new object[2] { loyaltyCard, executionmode };
            InvokeOperation(opName, parms);
        }

        public void SetLoyaltyCardAsPrimary(string loyaltyid)
        {
            string opName = "SetLoyaltyCardAsPrimary";
            object[] parms = new object[1] { loyaltyid };
            InvokeOperation(opName, parms);
        }

		public List<MGLoyaltyCard> GetLoyaltyCards()
		{
			string opName = "GetLoyaltyCards";
			return InvokeOperation(opName, null) as List<MGLoyaltyCard>;
		}

		public MGLoyaltyCard GetPrimaryLoyaltyCard()
		{
			string opName = "GetPrimaryLoyaltyCard";
			return InvokeOperation(opName, null) as MGLoyaltyCard;
		}

		public MGLoyaltyCard GetLoyaltyCard(string loyaltyid)
		{
			string opName = "GetLoyaltyCard";
			object[] parms = new object[1] { loyaltyid };
			return InvokeOperation(opName, parms) as MGLoyaltyCard;
		}

        public decimal GetLoyaltyCardBalance(string loyaltyid)
		{
			string opName = "GetLoyaltyCardBalance";
			object[] parms = new object[1] { loyaltyid };
            return (decimal)InvokeOperation(opName, parms);
		}
		#endregion

        #region Account Activity

        public int GetAccountActivityCount(string startDate, string endDate)
        {
            string opName = "GetAccountActivityCount";
            object[] parms = new object[2] { startDate, endDate };
            return (int)InvokeOperation(opName, parms);
        }

        public MGAccountActivitySummaryResponse GetAccountActivitySummary(
            string startDate,
            string endDate,
            string getPointsHistory,
            string getOtherPointsHistory,
            string summaryStartIndex,
            string summaryBatchSize,
            string otherStartIndex,
            string otherBatchSize
            )
        {
            string opName = "GetAccountActivitySummary";
            object[] parms = new object[8] { startDate, endDate, getPointsHistory, getOtherPointsHistory, summaryStartIndex, summaryBatchSize, otherStartIndex, otherBatchSize };
            return (MGAccountActivitySummaryResponse)InvokeOperation(opName, parms);
        }

        public List<MGAccountActivityDetails> GetAccountActivityDetails(
            string txnHeaderId,
            string getPointsHistory
            )
        {
            string opName = "GetAccountActivityDetails";
            object[] parms = new object[2] { txnHeaderId, getPointsHistory };
            return (List<MGAccountActivityDetails>)InvokeOperation(opName, parms);
        }
        #endregion

		#endregion

		#region Stores
		public List<MGStoreDef> GetAllStores(string startIndex, string batchSize)
		{
			string opName = "GetAllStores";
			object[] parms = new object[2] { startIndex, batchSize };
			return (List<MGStoreDef>)InvokeOperation(opName, parms);
		}

		public List<MGStoreDef> GetNearbyStores(string location, string mapRadiusInKM)
		{
			string opName = "GetNearbyStores";
			object[] parms = new object[2] { location, mapRadiusInKM };
			return (List<MGStoreDef>)InvokeOperation(opName, parms);
		}

		public List<MGStoreDef> GetTop10Stores()
		{
			string opName = "GetTop10Stores";
			return InvokeOperation(opName, null) as List<MGStoreDef>;
		}

		/*
        public void SaveTop10Stores(string top10stores)
        {
            string opName = "SaveTop10Stores";
			
            object[] parms =   new object[1] { top10stores };
            InvokeOperation(opName, parms);
        }
		*/

		public void SaveTop10Stores(List<long> top10stores)
		{
			string opName = "SaveTop10Stores";
			object[] parms = new object[1] { top10stores };
			InvokeOperation(opName, parms);
		}

		#endregion

		#region Messages

		#region Message Definitions
		public Int32 GetMessageDefinitionCount(string activeOnly, string contentAttributes)
		{
			string opName = "GetMessageDefinitionCount";
			object[] parms = new object[2] { activeOnly, contentAttributes };
			return (Int32)InvokeOperation(opName, parms);
		}

		public List<MGMessageDef> GetMessageDefinitions(string language, string channel, string activeOnly, string contentAttributes, string startIndex, string batchSize, string returnAttributes)
		{
			string opName = "GetMessageDefinitions";
			object[] parms = new object[7] { language, channel, activeOnly, contentAttributes, startIndex, batchSize, returnAttributes };
			return (List<MGMessageDef>)InvokeOperation(opName, parms);
		}
		#endregion

		#region Member Messages
		public MemberMessagePage GetMemberMessages(string language, string channel, long pageNumber, long resultsPerPage, string status, string order, string startDate, string endDate)
		{
			string opName = "GetMemberMessages";
			object[] parms = new object[] { language, channel, pageNumber, resultsPerPage, status, order, startDate, endDate};
			return (MemberMessagePage)InvokeOperation(opName, parms);
		}

		public void SetMessageStatus(long id, MemberMessageStatus status)
		{
			string opName = "SetMessageStatus";
			object[] parms = new object[] { id, status };
			InvokeOperation(opName, parms);
		}

		#endregion

		#endregion

		#region Coupons

		#region Coupon Definitions
		public Int32 GetCouponDefinitionCount(string activeOnly, string contentAttributes)
		{
			string opName = "GetCouponDefinitionCount";
			object[] parms = new object[2] { activeOnly, contentAttributes };
			return (Int32)InvokeOperation(opName, parms);
		}

		public List<MGCouponDef> GetCouponDefinitions(string language, string channel, string activeOnly, string contentAttributes, string startIndex, string batchSize, string returnAttributes)
		{
			string opName = "GetCouponDefinitions";
			object[] parms = new object[7] { language, channel, activeOnly, contentAttributes, startIndex, batchSize, returnAttributes };
			return (List<MGCouponDef>)InvokeOperation(opName, parms);
		}
		#endregion

		#region Member Coupons
		public List<MGMemberCoupon> GetMemberCoupons(string language, string channel, string typeCode, string startIndex, string batchSize, string returnAttributes)
		{
			string opName = "GetMemberCoupons";
			object[] parms = new object[6] { language, channel, typeCode, startIndex, batchSize, returnAttributes };
			return (List<MGMemberCoupon>)InvokeOperation(opName, parms);
		}

		public long RedeemMemberCouponById(string couponId, string redeemDate, string timesUsed)
		{
			string opName = "RedeemMemberCouponById";
			object[] parms = new object[3] { couponId, redeemDate, timesUsed };
			return (long)InvokeOperation(opName, parms);
		}
		#endregion

		#endregion

		#region RFE

		public int GetBonusDefinitionCount(bool activeOnly, string contentAttributes)
		{
			const string opName = "GetBonusDefinitionCount";
			object[] parms = new object[] { activeOnly, contentAttributes };
			return (int)InvokeOperation(opName, parms);
		}

		public List<MGBonusDef> GetBonusDefinitions(string language, string channel, bool activeOnly, string contentAttributes, int startIndex, int batchSize, bool returnAttributes)
		{
			const string opName = "GetBonusDefinitions";
			object[] parms = new object[] { language, channel, activeOnly, contentAttributes, startIndex, batchSize, returnAttributes };
			return (List<MGBonusDef>)InvokeOperation(opName, parms);
		}

		public MGBonusDef GetBonusDefinition(long id, string language, string channel, bool returnAttributes)
		{
			const string opName = "GetBonusDefinition";
			object[] parms = new object[] { id, language, channel, returnAttributes };
			return (MGBonusDef)InvokeOperation(opName, parms);
		}

		public int GetMemberBonusCount(bool activeOnly)
		{
			const string opName = "GetMemberBonusCount";
			object[] parms = new object[] { activeOnly };
			return (int)InvokeOperation(opName, parms);
		}

		public List<MGMemberBonus> GetMemberBonuses(bool activeOnly, string language, string channel, int startIndex, int batchSize)
		{
			const string opName = "GetMemberBonuses";
			object[] parms = new object[] { activeOnly, language, channel, startIndex, batchSize };
			return (List<MGMemberBonus>)InvokeOperation(opName, parms);
		}

		public MemberBonusStatus BonusAction(long id, MemberBonusStatus newStatus, string language, string channel)
		{
			const string opName = "BonusAction";
			object[] parms = new object[] { id, newStatus, language, channel };
			return (MemberBonusStatus)InvokeOperation(opName, parms);
		}

		public void RemoveBonus(long id)
		{
			const string opName = "RemoveBonus";
			object[] parms = new object[] { id };
			InvokeOperation(opName, parms);
		}

		public void ReferralCompleted(long id)
		{
			const string opName = "ReferralCompleted";
			object[] parms = new object[] { id };
			InvokeOperation(opName, parms);
		}

		#endregion

		#region Rewards

		#region Reward Catalog
		public List<MGRewardCategory> GetRewardCategories(string categoryId, string startIndex, string batchSize)
        {
            string opName = "GetRewardCategories";
            object[] parms = new object[3] { categoryId, startIndex, batchSize };
            return (List<MGRewardCategory>)InvokeOperation(opName, parms);
        }

        public Int32 GetRewardCatalogCount(string categoryId, string activeOnly, string tierName, string contentAttributes)
        {
            string opName = "GetRewardCatalogCount";
            object[] parms = new object[4] { categoryId, activeOnly, tierName, contentAttributes };
            return (Int32)InvokeOperation(opName, parms);
        }

        public List<MGRewardDefinition> GetRewardCatalog(
            string activeOnly,
            string tierName,
            string categoryId,
            string language,
            string channel,
            string contentAttributes,
            string currencyRange,
            string returnAttributes,
            string startIndex,
            string batchSize)
        {
            string opName = "GetRewardCatalog";
            object[] parms = new object[10] { activeOnly, tierName, categoryId, language, channel, contentAttributes, currencyRange, returnAttributes, startIndex, batchSize };
            return (List<MGRewardDefinition>)InvokeOperation(opName, parms);
        }
		#endregion

		#region Member Rewards
        public List<MGMemberReward> GetMemberRewards(
            string categoryId,
            string startDate,
            string endDate,
            string unRedeemedOnly,
            string unExpiredOnly,
            string language,
            string channel,
            string startIndex,
            string batchSize)
        {
            string opName = "GetMemberRewards";
            object[] parms = new object[9] { categoryId, startDate, endDate, unRedeemedOnly, unExpiredOnly, language, channel, startIndex, batchSize };
            return (List<MGMemberReward>)InvokeOperation(opName, parms);
        }

		public List<MGMemberReward> GetMemberRewards(string language, string channel, string startIndex, string batchSize)
		{
			string opName = "GetMemberRewards";
			object[] parms = new object[4] { language, channel, startIndex, batchSize };
			return (List<MGMemberReward>)InvokeOperation(opName, parms);
		}

        public MGMemberReward GetMemberRewardById(
            string rewardId,
            string language,
            string channel)
        {
            string opName = "GetMemberRewardById";
            object[] parms = new object[3] { rewardId, language, channel };
            return (MGMemberReward)InvokeOperation(opName, parms);
        }

        public MGMemberReward GetMemberRewardByCert(
            string certNumber,
            string language,
            string channel)
        {
            string opName = "GetMemberRewardByCert";
            object[] parms = new object[3] { certNumber, language, channel };
            return (MGMemberReward)InvokeOperation(opName, parms);
        }

        public bool IsMemberRewardRedeemed(string rewardId)
        {
            string opName = "IsMemberRewardRedeemed";
            object[] parms = new object[1] { rewardId };
            return (bool)InvokeOperation(opName, parms);
        }

        public void RedeemMemberReward(
            string rewardId,
            string availableBalance,
            string expirationDate,
            string redemptionDate)
        {
            string opName = "RedeemMemberReward";
            object[] parms = new object[4] { rewardId, availableBalance, expirationDate, redemptionDate };
            InvokeOperation(opName, parms);
        }
		#endregion

		#endregion

		#region Inbound Marketing
		public MGTriggerUserEvent TriggerUserEvent(string eventName, string language, string channel, Dictionary<string, string> clientcontext)
		{
			string opName = "TriggerUserEvent";
			object[] parms = new object[4] { eventName, language, channel, clientcontext };
			return (MGTriggerUserEvent)InvokeOperation(opName, parms);
		}
		#endregion

		#region Preferences and Surveys
		public List<MGSurvey> GetProfileSurveys(string language, string channel)
		{
			string opName = "GetProfileSurveys";
			object[] parms = new object[2] { language, channel };
			return (List<MGSurvey>)InvokeOperation(opName, parms);
		}

		public List<MGSurvey> GetGeneralSurveys(string language, string channel)
		{
			string opName = "GetGeneralSurveys";
			object[] parms = new object[2] { language, channel };
			return (List<MGSurvey>)InvokeOperation(opName, parms);
		}

		public MGSurveyState GetNextSurveyState(string language, string surveyId, string stateId)
		{
			string opName = "GetNextSurveyState";
			object[] parms = new object[3] { language, surveyId, stateId };
			object response = InvokeOperation(opName, parms);
			return response as MGSurveyState;
			//string jsonStr = ((MGSurveyState)response).Serialize();
			//return jsonStr;
		}

		//[ServiceKnownType(typeof(MGSurveySimpleResponse))]
		//[ServiceKnownType(typeof(MGSurveyMatrixResponse))]
		public bool PostSurveyResponse(string language, string surveyId, string stateId, string surveyResponse)
		{
			string opName = "PostSurveyResponse";
			object[] parms = new object[4] { surveyId, stateId, surveyResponse, language };
			object result = InvokeOperation(opName, parms);
			return (bool)result;
		}
		#endregion

		#region CheckIn
		public MGPreCheckInResponse PreCheckIn(MGLocation location, double mapRadiusInKM)
		{
			string opName = "PreCheckIn";
			object[] parms = new object[2] { location, mapRadiusInKM };
			return (MGPreCheckInResponse)InvokeOperation(opName, parms);
		}

		public MGCheckInResponse CheckIn(MGLocation location, double mapRadiusInKM, string language, string channel)
		{
			string opName = "CheckIn";
			object[] parms = new object[4] { location, mapRadiusInKM, language, channel };
			return (MGCheckInResponse)InvokeOperation(opName, parms);
		}
		#endregion

		#region Content

		public string GetTextBlock(string name, string language, string channel)
		{
			string opName = "GetTextBlock";
			object[] parms = new object[3] { name, language, channel };
			return (string)InvokeOperation(opName, parms);
		}

		#endregion

		#region Apple Passbook
		public Stream GetPassFromMTouch(string passType, string mtouch)
		{
			string opName = "GetPassFromMTouch";
			object[] parms = new object[2] { passType, mtouch };
			Stream response = InvokeOperation(opName, parms) as Stream;
			return response;
		}

		public Stream GetLoyaltyCardPass(string loyaltyId)
		{
			string opName = "GetLoyaltyCardPass";
			object[] parms = new object[] { loyaltyId };
			byte[] response = InvokeOperation(opName, parms) as byte[];

			return new MemoryStream(response);
		}

		public Stream GetCouponPass(string couponId)
		{
			string opName = "GetCouponPass";
			object[] parms = new object[] { couponId };
			byte[] response = InvokeOperation(opName, parms) as byte[];
			return new MemoryStream(response);
		}

		#endregion

		#region Apple Push Notification
		public void RegisterForPushNotification(string deviceToken, bool forLoyaltyCards, bool forCoupons, bool forRewards, bool forOffers, bool forPromotions)
		{
			string opName = "RegisterForPushNotification";
			object[] parms = new object[] { deviceToken, forLoyaltyCards, forCoupons, forRewards, forOffers, forPromotions };
			InvokeOperation(opName, parms);
		}
		#endregion

		public void AssociateMemberSocialHandle(string provider, string providerUid, string token, string secret)
		{
			string opName = "AssociateSocialHandle";
			object[] parms = new object[4] { provider, providerUid, token, secret };
			InvokeOperation(opName, parms);
		}
	}
}
