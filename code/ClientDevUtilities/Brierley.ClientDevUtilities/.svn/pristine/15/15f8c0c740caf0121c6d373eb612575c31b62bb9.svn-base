using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.FrameWork.Common;


namespace Brierley.LoyaltyWare.LWMobileGateway
{
	[ServiceContract]
	public interface IMobileGateway
	{
		#region Member Account

		#region Authentication
		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/Authenticate?devicetype=iPhone&version=iOS6&username=memberreward&password=Password01
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Authenticate?devicetype={devicetype}&version={version}&username={username}&password={password}&resetcode={resetcode}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGAuthenticateMember AuthenticateMember(string deviceType, string version, string username, string password, string resetcode);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Authenticate/{Provider}/{ProviderUId}?devicetype={devicetype}&version={version}&token={token}&secret={secret}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGAuthenticateMember AuthenticateMemberBySocialHandle(string provider, string providerUId, string token, string secret, string deviceType, string version);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Logout",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		void Logout();
		#endregion

		#region Member
		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/GetMember
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGMember GetMember();

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		MGAuthenticateMember CreateMember(MGMember member, string password, string executionmode, string deviceType, string version);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		void UpdateMember(MGMember member, string executionmode);

		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/GetAccountSummary
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/AccountSummary",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGAccountSummary GetAccountSummary();

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Password",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		string ChangePassword(string username, string oldPassword, string newPassword);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/PasswordReset?username={username}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
		PasswordResetOptions InitiatePasswordReset(string username);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/PasswordReset",
            Method = "PUT",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void ConfirmPasswordReset(string username, string channel);
		#endregion

		#region Loyalty Cards
        //http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/AddLoyaltyCard
        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/LoyaltyCard",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        void AddLoyaltyCard(MGLoyaltyCard loyaltyCard, string executionmode);

        //http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/SetLoyaltyCardAsPrimary
        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/LoyaltyCard/Primary",
            Method = "PUT",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        void SetLoyaltyCardAsPrimary(string loyaltyId);

		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/GetLoyaltyCards
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/LoyaltyCards",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGLoyaltyCard> GetLoyaltyCards();

		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/GetPrimaryLoyaltyCard
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/LoyaltyCard/Primary",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGLoyaltyCard GetPrimaryLoyaltyCard();

		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/GetLoyaltyCard?loyaltyid=membercoupon
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/LoyaltyCard?loyaltyid={loyaltyid}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGLoyaltyCard GetLoyaltyCard(string loyaltyid);

		//http://localhost/LWMobileGateway/MobileGateway.svc/Member/LoyaltyCard/GetLoyaltyCardBalance?loyaltyid={loyaltyid}
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/LoyaltyCard/Balance?loyaltyid={loyaltyid}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
        decimal GetLoyaltyCardBalance(string loyaltyid);
		#endregion

        #region Account Activity
        //http://localhost/LWMobileGateway/MobileGateway.svc/Member/AccountActivity/GetAccountActivitySummary
        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/AccountActivity/Count?startDate={startDate}&endDate={endDate}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        int GetAccountActivityCount(string startDate, string endDate);

        //http://localhost/LWMobileGateway/MobileGateway.svc/Member/AccountActivity/GetAccountActivitySummary
        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/AccountActivity/Summary?startDate={startDate}&endDate={endDate}&getPointsHistory={getPointsHistory}&getOtherPointsHistory={getOtherPointsHistory}&summaryStartIndex={summaryStartIndex}&summaryBatchSize={summaryBatchSize}&otherStartIndex={otherStartIndex}&otherBatchSize={otherBatchSize}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        MGAccountActivitySummaryResponse GetAccountActivitySummary(
            string startDate,
            string endDate,
            string getPointsHistory,
            string getOtherPointsHistory,
            string summaryStartIndex,
            string summaryBatchSize,
            string otherStartIndex,
            string otherBatchSize
            );

        //http://localhost/LWMobileGateway/MobileGateway.svc/Member/AccountActivity/GetAccountActivityDetails
        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/AccountActivity/Details?txnHeaderId={txnHeaderId}&getPointsHistory={getPointsHistory}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        List<MGAccountActivityDetails> GetAccountActivityDetails(
            string txnHeaderId,            
            string getPointsHistory
            );
        #endregion

        #endregion

        #region Stores
        [OperationContract]
		[WebInvoke(UriTemplate = "/Stores?startindex={startindex}&batchsize={batchsize}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGStoreDef> GetAllStores(string startIndex, string batchSize);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Stores/Nearby?location={location}&mapRadiusInKM={mapRadiusInKM}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGStoreDef> GetNearbyStores(string location, string mapRadiusInKM);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Stores/Top10",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGStoreDef> GetTop10Stores();

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Stores/Top10",
			Method = "PUT",
			//BodyStyle = WebMessageBodyStyle.WrappedResponse,
			ResponseFormat = WebMessageFormat.Json,
			RequestFormat = WebMessageFormat.Json)]
		void SaveTop10Stores(List<long> top10stores);
		#endregion

		#region Messages

		#region Message Definitions
		[OperationContract]
		[WebInvoke(UriTemplate = "/Messages/Definitions/Count?activeonly={activeOnly}&contentAttributes={contentAttributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		Int32 GetMessageDefinitionCount(string activeOnly, string contentAttributes);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Messages/Definitions?language={language}&channel={channel}&activeonly={activeonly}&contentAttributes={contentAttributes}&startindex={startindex}&batchsize={batchsize}&returnattributes={returnattributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGMessageDef> GetMessageDefinitions(string language, string channel, string activeOnly, string contentAttributes, string startIndex, string batchSize, string returnAttributes);
		#endregion

		#region Member Messages
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Messages?language={language}&channel={channel}&startdate={startdate}&enddate={enddate}&pagenumber={pagenumber}&resultsperpage={resultsperpage}&status={status}&order={order}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MemberMessagePage GetMemberMessages(string language, string channel, long pageNumber, long resultsPerPage, string status, string order, string startDate, string endDate);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Message/status?id={id}",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		void SetMessageStatus(long id, MemberMessageStatus status);
		#endregion

		#endregion

		#region Coupons

		#region Coupon Definitions
		[OperationContract]
		[WebInvoke(UriTemplate = "/Coupons/Definitions/Count?activeonly={activeOnly}&contentAttributes={contentAttributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		Int32 GetCouponDefinitionCount(string activeOnly, string contentAttributes);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Coupons/Definitions?language={language}&channel={channel}&activeonly={activeonly}&contentAttributes={contentAttributes}&startindex={startindex}&batchsize={batchsize}&returnattributes={returnattributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGCouponDef> GetCouponDefinitions(string language, string channel, string activeOnly, string contentAttributes, string startIndex, string batchSize, string returnAttributes);
		#endregion

		#region Member Coupons
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Coupons?language={language}&channel={channel}&typecode={typecode}&startindex={startindex}&batchsize={batchsize}&returnattributes={returnattributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGMemberCoupon> GetMemberCoupons(string language, string channel, string typeCode, string startIndex, string batchSize, string returnAttributes);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Coupons/RedeemById?couponid={couponid}&redeemdate={redeemdate}&timesused={timesused}",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		long RedeemMemberCouponById(string couponId, string redeemDate, string timesUsed);
		#endregion

		#endregion

		#region Rewards
				
        #region Reward Catalog 
       
        [OperationContract]
        [WebInvoke(UriTemplate = "/Rewards/Categories?categoryId={categoryId}&startindex={startindex}&batchsize={batchsize}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        List<MGRewardCategory> GetRewardCategories(string categoryId, string startIndex, string batchSize);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Rewards/Count?categoryId={categoryId}&activeOnly={activeOnly}&tierName={tierName}&contentAttributes={contentAttributes}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        Int32 GetRewardCatalogCount(string categoryId, string activeOnly, string tierName, string contentAttributes);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Rewards?activeOnly={activeOnly}&tierName={tierName}&categoryId={categoryId}&language={language}&channel={channel}&contentAttributes={contentAttributes}&currencyRange={currencyRange}&returnAttributes={returnAttributes}&startindex={startindex}&batchsize={batchsize}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        List<MGRewardDefinition> GetRewardCatalog(
            string activeOnly, 
            string tierName,
            string categoryId,
            string language, 
            string channel, 
            string contentAttributes, 
            string currencyRange,
            string returnAttributes,
            string startIndex, 
            string batchSize);

        //[OperationContract]
        //[WebInvoke(UriTemplate = "/Member/Rewards/GetRewardCatalogItem?rewardId={rewardId}&language={language}&channel={channel}&returnAttributes={returnAttributes}",
        //    Method = "GET",
        //    BodyStyle = WebMessageBodyStyle.Wrapped,
        //    ResponseFormat = WebMessageFormat.Json)]
        //MGRewardDefinition GetRewardCatalogItem(
        //    string rewardId,
        //    string language,
        //    string channel,
        //    string returnAttributes);
        #endregion
        
		#region Member Rewards
		[OperationContract]
        [WebInvoke(UriTemplate = "/Member/Rewards?categoryId={categoryId}&startDate={startDate}&endDate={endDate}&unRedeemedOnly={unRedeemedOnly}&unExpiredOnly={unExpiredOnly}&language={language}&channel={channel}&startindex={startindex}&batchsize={batchsize}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
        List<MGMemberReward> GetMemberRewards(
            string categoryId,
            string startDate,
            string endDate,
            string unRedeemedOnly,
            string unExpiredOnly,
            string language, 
            string channel, 
            string startIndex, 
            string batchSize);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/Reward/ById?rewardId={rewardId}&language={language}&channel={channel}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        MGMemberReward GetMemberRewardById(
            string rewardId,
            string language,
            string channel);


        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/Reward/ByCert?certNumber={certNumber}&language={language}&channel={channel}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        MGMemberReward GetMemberRewardByCert(
            string certNumber,
            string language,
            string channel);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/Reward/Redeem?rewardId={rewardId}",
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        bool IsMemberRewardRedeemed(
            string rewardId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Member/Reward/Redeem?rewardId={rewardId}&availableBalance={availableBalance}&expirationDate={expirationDate}&redemptionDate={redemptionDate}",
            Method = "PUT",
            BodyStyle = WebMessageBodyStyle.Wrapped,
            ResponseFormat = WebMessageFormat.Json)]
        void RedeemMemberReward(
            string rewardId,
            string availableBalance,
            string expirationDate,
            string redemptionDate);
		#endregion

		#endregion
		
		#region RFE
		
		#region Bonus Definitions
		[OperationContract]
		[WebInvoke(UriTemplate = "/bonus/definitions/Count?activeonly={activeOnly}&contentAttributes={contentAttributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		int GetBonusDefinitionCount(bool activeOnly, string contentAttributes);

		[OperationContract]
		[WebInvoke(UriTemplate = "/bonus/definitions?language={language}&channel={channel}&activeonly={activeonly}&contentAttributes={contentAttributes}&startindex={startindex}&batchsize={batchsize}&returnattributes={returnattributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGBonusDef> GetBonusDefinitions(string language, string channel, bool activeOnly, string contentAttributes, int startIndex, int batchSize, bool returnAttributes);

		[OperationContract]
		[WebInvoke(UriTemplate = "/bonus/definition?id={id}&language={language}&channel={channel}&returnattributes={returnattributes}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGBonusDef GetBonusDefinition(long id, string language, string channel, bool returnAttributes);

		#endregion

		#region Member Bonuses

		[OperationContract]
		[WebInvoke(UriTemplate = "/member/bonuses/count?activeonly={activeOnly}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		int GetMemberBonusCount(bool activeOnly);

		[OperationContract]
		[WebInvoke(UriTemplate = "/member/bonuses?activeonly={activeOnly}&language={language}&channel={channel}&startindex={startindex}&batchsize={batchsize}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGMemberBonus> GetMemberBonuses(bool activeOnly, string language, string channel, int startIndex, int batchSize);

		[OperationContract]
		//[WebInvoke(UriTemplate = "/member/bonus?id={id}",
		[WebInvoke(UriTemplate = "/member/bonus",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		MemberBonusStatus BonusAction(long id, MemberBonusStatus newStatus, string language, string channel);

		[OperationContract]
		[WebInvoke(UriTemplate = "/member/bonus?id={id}",
			Method = "DELETE",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		void RemoveBonus(long id);

		[OperationContract]
		//[WebInvoke(UriTemplate = "/member/bonus/referral?id={id}",
		[WebInvoke(UriTemplate = "/member/bonus/referral",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		void ReferralCompleted(long id);
		
		
		#endregion
		
		#endregion
		
		#region Inbound Marketing

		#region General
		[OperationContract]
		//[WebInvoke(UriTemplate = "/Member/Members/TriggerUserEvent?eventname={eventname}&language={language}&channel={channel}&clientcontext={clientcontext}",
		[WebInvoke(UriTemplate = "/Member/Members/TriggerUserEvent",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		MGTriggerUserEvent TriggerUserEvent(string eventName, string language, string channel, Dictionary<string, string> clientcontext);
		#endregion

		#region Check-in
		//TODO: Check in with location coordinates
		#endregion

		#endregion

		#region Preferences and Surveys
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Surveys/Profile?language={language}&channel={channel}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGSurvey> GetProfileSurveys(string language, string channel);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Surveys/General?language={language}&channel={channel}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		List<MGSurvey> GetGeneralSurveys(string language, string channel);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/Survey?language={language}&surveyid={surveyId}&stateid={stateId}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		MGSurveyState GetNextSurveyState(string language, string surveyId, string stateId);

		[OperationContract]
		//[ServiceKnownType(typeof(MGSurveySimpleResponse))]
		//[ServiceKnownType(typeof(MGSurveyMatrixResponse))]
		[WebInvoke(UriTemplate = "/Member/Survey",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		bool PostSurveyResponse(string language, string surveyId, string stateId, string surveyResponse);
		#endregion

		#region CheckIn
		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/CheckIn/PreCheckIn",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		MGPreCheckInResponse PreCheckIn(MGLocation location, double mapRadiusInKM);

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/CheckIn/CheckIn",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		MGCheckInResponse CheckIn(MGLocation location, double mapRadiusInKM, string language, string channel);
		#endregion

		#region Content

		[OperationContract]
		[WebInvoke(UriTemplate = "/Content/TextBlock?name={name}&language={language}&channel={channel}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		string GetTextBlock(string name, string language, string channel);

		#endregion

		#region Apple Passbook
		[OperationContract]
		[WebInvoke(UriTemplate = "/Passbook/FromMTouch?type={passType}&MTouch={mtouch}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Bare)]
		Stream GetPassFromMTouch(string passType, string mtouch);


		[OperationContract]
		[WebInvoke(UriTemplate = "/Passbook/LoyaltyCardPass?loyaltyId={loyaltyId}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Bare //, 
			//ResponseFormat = WebMessageFormat.Json
			)]
		Stream GetLoyaltyCardPass(string loyaltyId);


		[OperationContract]
		[WebInvoke(UriTemplate = "/Passbook/CouponPass?couponId={couponId}",
			Method = "GET",
			BodyStyle = WebMessageBodyStyle.Bare //,
			//ResponseFormat = WebMessageFormat.Json
			)]
		Stream GetCouponPass(string couponId);
		

		#endregion

		#region Apple Push Notification

		[OperationContract]
		[WebInvoke(UriTemplate = "/Notification/RegisterForPushNotification",
			Method = "POST",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json
			)]
		void RegisterForPushNotification(string deviceToken, bool forLoyaltyCards, bool forCoupons, bool forRewards, bool forOffers, bool forPromotions);

		#endregion

		[OperationContract]
		[WebInvoke(UriTemplate = "/Member/SocialHandle/{Provider}/{ProviderUId}",
			Method = "PUT",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		void AssociateMemberSocialHandle(string provider, string providerUid, string token, string verifier);
	}
}
