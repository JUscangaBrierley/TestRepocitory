using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Common
{
	/// <summary>
	/// The type of enrollment supported by a particular promotion.
	/// </summary>
	public enum PromotionEnrollmentSupportType
	{
		None = 0,
		Supported = 1,
		Required = 2
	}

	public enum NextBestActionType
	{
		Coupon = 1,
		Message = 2,
		Sku = 3
	}

	public enum LockingMode
	{
		None,
		Read,
		Upgrade,
		UpgradeNoWait,
		Write,
		Force
	}

	#region Custom Components
	public enum CustomComponentTypeEnum
	{
		BScript, // custom bScripts
		Rule, // custom rules
		Form, // bookmark forms
		XsltPostProcessor, // post processors for email
        DataModel, // attribute set classes
	}

	#endregion

	#region authentication
	public enum LoginStatusEnum
	{
		/// <summary>
		/// login failed due to a invalid username/password
		/// </summary>
		Failure = 0,
		Success = 1,
		LockedOut = 3,
		PasswordResetRequired = 4,
		Disabled = 5,
		Terminated = 6
	}

	public enum AuthenticationFields
	{
		Username,
		PrimaryEmailAddress,
		AlternateId,
		LoyaltyIdNumber
	}
	#endregion

	#region Language/Channels
	public enum ContentObjType { Product, Reward, Message, Coupon, Bonus, Promotion, LoyaltyCard, ReferenceData, Notification, TextBlock, Tier, ExchangeRate };
	public enum ContentAttributeDataType { String, Date, DateTime };
	#endregion

	#region LIBJob
	public enum LIBJobStatusEnum { InProcess = 0, Finished = 1 };
	public enum LIBJobDirectionEnum { InBound = 0, OutBound = 1 };
	#endregion

	#region Data Model

	#region Status
	public enum SchemaStatus
	{
		NotCreated = 1,
		Created = 2,
		Modified = 3
	}
	#endregion

	#region Attribute/ Attribute Sets
	public enum SupportedDataSourceType
	{
		Oracle10g = 1,
		MySQL55 = 2,
		MsSQL2005 = 3
	}

	public enum AttributeSetType
	{
		Member = 1,
		VirtualCard = 2,
		Global = 3
	}

	public enum AttributeSetCategory
	{
		NotSet = 0,
		TransactionProfiles = 1,
		BasicProfiles = 3
	}

	public enum AttributeEncryptionType
	{
		None = 0,
		Encoded = 1,
		Symmetric = 2,
		Asymmetric = 3
	}
	#endregion

	#endregion

	#region Member

	public enum MemberStatusEnum
	{
		Active = 1,
		Disabled = 2,
		Terminated = 3,
		Locked = 4,
		NonMember = 5,
		Merged = 6,
		PreEnrolled = 7
	}

	public enum MemberLoginEventType
	{
		LoginSuccess,
		PasswordInvalid,
		PasswordExpired,
		UsernameInvalid,
		AccountDisabled,
		AccountLocked,
		AccountTerminated,
		AccountNonMember,
        ResetCodeInvalid,
        ResetCodeExpired
	}
	#endregion

	#region Virtual Cards / Point Transactions
	public enum VirtualCardStatusType
	{
		Active = 1,
		InActive = 2,
		//Hold = 3,
		Cancelled = 4,
		Replaced = 5,
		Expired = 6,
	}

	public enum VirtualCardSearchType
	{
		/// <summary>
		/// 
		/// </summary>
		PrimaryCard = 0,

		/// <summary>
		/// 
		/// </summary>
		MostRecentRegistered = 1,

		/// <summary>
		/// 
		/// </summary>
		MostRecentIssued = 2,

		/// <summary>
		/// 
		/// </summary>
		EarliestRegistered = 3,

		/// <summary>
		/// 
		/// </summary>
		EarliestIssued = 4,

	};

	/// <summary>
	/// 
	/// </summary>
	//public enum PointConsumptionMethod
	//{
	//    /// <summary>
	//    /// 
	//    /// </summary>
	//    FIFO,

	//    /// <summary>
	//    /// 
	//    /// </summary>
	//    PointType,

	//    /// <summary>
	//    /// 
	//    /// </summary>
	//    LumpSum
	//}

	/// <summary>
	/// 
	/// </summary>
	public enum PointBankTransactionType
	{
		/// <summary>
		/// 
		/// </summary>
		Credit = 1,
		/// <summary>
		/// 
		/// </summary>
		Debit = 2,
		/// <summary>
		/// 
		/// </summary>
		Hold = 3,
		/// <summary>
		/// 
		/// </summary>
		Consumed = 4,
		/// <summary>
		/// 
		/// </summary>
		Transferred = 5,
		/// <summary>
		/// 
		/// </summary>
		Expired = 6,
	};

	public enum PointTransactionOwnerType
	{
		Unknown = 0,
		/// <summary>
		/// The point transaction was created as a result of an attribute set based event
		/// </summary>
		AttributeSet = 1,
		/// <summary>
		/// The point transaction was created as a result of a member checking in a store
		/// </summary>
		Store = 2,
		/// <summary>
		/// The point transaction was created as a result of a member bidding for an auction item
		/// </summary>
		Bid = 3,
		/// <summary>
		/// The point transaction was created as a result of a member performing an action for a bonus
		/// </summary>
		Bonus = 4,
		/// <summary>
		/// The point transaction was created as a result of a member taking a survey
		/// </summary>
		Survey = 5,
		/// <summary>
		/// When points are consumed for a reward
		/// </summary>
		Reward = 6,
		/// <summary>
		/// Nothing to do with point transactions but used to track rule actions
		/// </summary>
		Coupon = 7,
		/// <summary>
		/// Nothing to do with point transactions but used to track rule actions
		/// </summary>
		Message = 8,
		/// <summary>
		/// 
		/// </summary>
		Promotion = 9,
		/// <summary>
		/// 
		/// </summary>
		Social = 10,
	};

	/// <summary>
	/// This enumeration is used to record the reson for expiration of a point transaction.
	/// </summary>
	public enum PointExpirationReason
	{
		/// <summary>
		/// Normal expiration of this transaction
		/// </summary>
		Normal = 0,
		/// <summary>
		/// Transaction was expired pre-muturely due to card/member cancellation
		/// </summary>
		Cancellation = 1,
		/// <summary>
		/// Transaction was expired pre-muturely due to member termination
		/// </summary>
		Termination = 2,
		/// <summary>
		/// Transaction was expired pre-muturely due to card replacement
		/// </summary>
		Replacement = 3,
		/// <summary>
		/// Transaction was expired pre-muturely due to member merge
		/// </summary>
		MemberMerge = 4
	}

	/// <summary>
	/// This enumeration is used by the methods to retrieve members based on a particular balance type
	/// </summary>
	public enum PointBalanceType
	{
		/// <summary>
		/// This is the current point balance
		/// </summary>
		PointBalance = 1,
		/// <summary>
		/// This is the earned balance
		/// </summary>
		EarnedBalance = 2,
		/// <summary>
		/// Points on hold
		/// </summary>
		PointsOnHold = 3
	};
	#endregion

	#region Rules

	public enum RuleExecutionMode { Real, Simulation }

	public enum RuleExecutionStatus { Success, Skipped, Error }

	public enum RuleTriggerObjectType
	{
		AttributeSet = 1,
		MemberAuthenticationRules = 2,
		MemberLoadRules = 3,
		MemberSaveRules = 4,
		MemberUpdateRules = 5,
		MemberOthersRules = 6,
		MemberCheckinRules = 6
	}

	/// <summary>
	/// Lists items that can be "issued" during rule execution
	/// </summary>
	public enum RuleTriggerIssueType
	{
		Points,
		VirtualCard,
		Tier,
		Coupon,
		Message,
		Notification,
		Reward,
		Promotion,
		Survey,
		Email,
		Sms,
		Bonus
	}


	/// <summary>
	/// 
	/// </summary>
	public enum PointBatchingMode
	{
		/// <summary>
		/// 
		/// </summary>
		Batched,

		/// <summary>
		/// 
		/// </summary>
		PerRecord
	};

	/// <summary>
	/// 
	/// </summary>
	public enum PointAwardMethod
	{
		/// <summary>
		/// 
		/// </summary>
		Normal,

		/// <summary>
		/// 
		/// </summary>
		UseChild
	};

	/// <summary>
	/// 
	/// </summary>
	public enum PointExpirationLogic
	{
		/// <summary>
		/// 
		/// </summary>
		Default,

		/// <summary>
		/// 
		/// </summary>
		EndOfQtr,

		/// <summary>
		/// 
		/// </summary>
		EndOfNextQtr,

		/// <summary>
		/// 
		/// </summary>
		EndOfMonth,

		/// <summary>
		/// 
		/// </summary>
		EndOfNextMonth,

		/// <summary>
		/// 
		/// </summary>
		EndOfYear,

		/// <summary>
		/// 
		/// </summary>
		Days
	};

	/// <summary>
	/// 
	/// </summary>
	public enum TierPointActivityMethod
	{
		/// <summary>
		/// 
		/// </summary>
		CalendarYear = 0,

		/// <summary>
		/// 
		/// </summary>
		Quarter = 1,

		/// <summary>
		/// 
		/// </summary>
		SpecifiedUnit = 2,
	};

	/// <summary>
	/// 
	/// </summary>
	public enum PeriodUnit
	{
		/// <summary>
		/// 
		/// </summary>
		Day = 0,

		/// <summary>
		/// 
		/// </summary>
		Month = 1,

		/// <summary>
		/// 
		/// </summary>
		Year = 2,
	};

	/// <summary>
	/// 
	/// </summary>
	public enum VirtualCardLocation
	{
		/// <summary>
		/// 
		/// </summary>
		UseExpression = 0,

		/// <summary>
		/// 
		/// </summary>
		FirstCardInList = 1,

		/// <summary>
		/// 
		/// </summary>
		PrimaryCard = 2,

		/// <summary>
		/// 
		/// </summary>
		AllCards = 3,
	};

	/// <summary>
	/// 
	/// </summary>
	public enum RuleInvocationType
	{
		/// <summary>
		/// 
		/// </summary>
		BeforeInsert = 1,
		/// <summary>
		/// 
		/// </summary>
		AfterInsert = 2,
		/// <summary>
		/// 
		/// </summary>
		BeforeUpdate = 3,
		/// <summary>
		/// 
		/// </summary>
		AfterUpdate = 4,
		/// <summary>
		/// 
		/// </summary>
		BeforeDelete = 5,
		/// <summary>
		/// 
		/// </summary>
		AfterDelete = 6,

		/// <summary>
		/// 
		/// </summary>
		Manual = 7
	};

	/// <summary>
	/// The type of rewawrd being issued.
	/// </summary>
	public enum IssueRewardType
	{
		/// <summary>
		/// This reward is earned
		/// </summary>
		Earned = 1,
		/// <summary>
		/// This is given away
		/// </summary>
		Entitlement = 2
	}

	/// <summary>
	/// This enumeration provides the directive for what should be done to points after
	/// issuign an earned reward.
	/// </summary>
	public enum PointsConsumptionOnIssueReward
	{
		/// <summary>
		/// Do nothing
		/// </summary>
		NoAction = 1,
		/// <summary>
		/// Consume the points after issueing the reward
		/// </summary>
		Consume = 2,
		/// <summary>
		/// Put the points on hold after issueing the reward
		/// </summary>
		Hold = 3
	}

	#endregion

	#region Data Types
	/// <summary>
	/// 
	/// </summary>
	public enum DataType
	{
		/// <summary>
		/// 
		/// </summary>
		Integer,
		/// <summary>
		/// 
		/// </summary>
		Number,
		/// <summary>
		/// 
		/// </summary>
		String,
		/// <summary>
		/// 
		/// </summary>
		Date,
		/// <summary>
		/// 
		/// </summary>
		Decimal,
		/// <summary>
		/// 
		/// </summary>
		//Double,
		//Text,
		/// <summary>
		/// 
		/// </summary>
		Long,
		/// <summary>
		/// 
		/// </summary>
		//Cursor,
		/// <summary>
		/// 
		/// </summary>
		//Blob,
		/// <summary>
		/// 
		/// </summary>
		//Clob,

		/// <summary>
		/// 
		/// </summary>
		Money,

		/// <summary>
		/// 
		/// </summary>
		Boolean
	};

	public enum StructuredDataType { STRING = 0, INT, REAL, BOOL, DATETIME };

	/// <summary>
	/// 
	/// </summary>
	public enum SupportedDateFormats
	{
		/// <summary>
		/// 
		/// </summary>
		d,
		/// <summary>
		/// 
		/// </summary>
		f,
		/// <summary>
		/// 
		/// </summary>
		g,
		/// <summary>
		/// 
		/// </summary>
		G
	};

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum LogLevel
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,

		/// <summary>
		/// 
		/// </summary>
		Debug = 1,

		/// <summary>
		/// 
		/// </summary>
		Event = 2,

		/// <summary>
		/// 
		/// </summary>
		Exception = 4,

		/// <summary>
		/// 
		/// </summary>
		Information = 8,

		/// <summary>
		/// 
		/// </summary>
		Performance = 16,

		/// <summary>
		/// 
		/// </summary>
		RuleProcessingError = 32,

		/// <summary>
		/// 
		/// </summary>
		FileProcessingError = 64,

	};
	#endregion

	#region Email

	public enum EmailProviderType
	{
		Dmc,
		Aws,
		Custom
	}

	public enum ContentTransferEncoding
	{
		None = 0,
		QuotedPrintable = 1,
		Base64 = 2
	}

	public enum CommunicationType
	{
		DmcGetPersonalizations = 1,
		DmcGetUser = 2,
		DmcCreateUser = 3,
		DmcUpdateProfileByMobileNumber = 4,
		DmcSendSingle = 5,
		AwsSendEmail = 6
	}

	public enum DmcResult
	{
		Unknown = 0,
		ConnectionFailure = 1,
		InvalidContent = 2,
		DmcException = 3,
		Success = 4
	}

	//todo: these are covered by DmcResult and should be removed:
	public enum EmailFailureType
	{
		Unknown = 0,
		ConnectionFailure = 1,
		InvalidContent = 2,
		SentSuccessfully = 3
	}

	public enum SmsFailureType
	{
		Unknown = 0,
		ConnectionFailure = 1,
		InvalidContent = 2,
		SentSuccessfully = 3
	}

	public enum PushFailureType
	{
		Unknown = 0,
		ConnectionFailure = 1,
		InvalidContent = 2,
		SentSuccessfully = 3
	}

	public enum EmailFeedbackType
	{
		//unknown
		//  this is not an SES feedback type, but is used in the event that a feedback type is received that does not match our enumeration
		Unknown = 0,

		Permanent = 1, 
		Transient = 2, 
		Undetermined = 3, 
		Complaint = 4
	}

	public enum EmailFeedbackSubtype
	{
		//unknown
		//  this is not an SES feedback type, but is used in the event that a feedback type is received that does not match our enumeration
		Unknown = 0, 

		//undetermined 
		//  unable to determine a specific bounce reason
		Undetermined = 1, 

		//permanent
		//  a general hard bounce
		General = 2, 
		//  the target email address does not exist
		NoEmail = 3, 
		//  email provider has suppressed the email address
		Suppressed = 4, 

		//transient
		//  general transient bounce
		Transient = 5, 
		//  the recipient's mailbox is full
		MailboxFull = 6, 
		//  the message sent was too large
		MessageTooLarge = 7, 
		//  message content was rejected
		ContentRejected = 8, 
		//  email attachment was rejected
		AttachmentRejected = 9,

		//complaint
		//  unsolicited email complaint
		Abuse = 10, 
		//  email authentication failed
		AuthFailure = 11, 
		//  email reported as fraud/phishing
		Fraud = 12, 
		//  not spam: the email may have landed in the recipient's spam folder and the recipient marked it as "not spam." 
		//  This bounce types is generally ignored as it is not an indication that there was any problem delivering the email
		NotSpam = 13, 
		//  any feedback that doesn't fit into regular complaint types
		Other = 14, 
		//  a virus was reported in the email message
		Virus = 15
	}

	public enum EmailBounceRuleType
	{
		None = 0, 
		Strict = 1, 
		Sliding = 2
	}

	#endregion

	#region Documents
	public enum TemplateType
	{
		/// <summary>
		/// 
		/// </summary>
		Webpage = 1,

		/// <summary>
		/// 
		/// </summary>
		Email = 2,
		Sms = 3
	};

	public enum ContentAreaType
	{
		ContentArea = 1,
		DynamicContentArea,
		Field,
		TextArea
	}

	public enum ContentElementType
	{
		/// <summary>
		/// represents a block of text
		/// </summary>
		Text = 1,

		/// <summary>
		/// represents a block of text/html. 
		/// </summary>
		TextHtml,

		[Obsolete]
		Image,

		/// <summary>
		/// The master template for the element/area
		/// </summary>
		Template,

		[Obsolete]
		Xml,

		/// <summary>
		/// bScript expressions placed in the element/area
		/// </summary>
		bScript,

		/// <summary>
		/// Email field
		/// </summary>
		Field,

		Structured,
		UnStructured,
	}

	public enum BatchSelectionTypes
	{
		BatchName, ActiveBatch, bScript
	}

	public enum RowSelectionTypes
	{
		FirstRow, bScript, IterateAll
	}

	public enum DocumentType
	{
		/// <summary>
		/// 
		/// </summary>
		HTMLDocument = 1,

		/// <summary>
		/// 
		/// </summary>
		EmailDocument = 2,
		SmsDocument = 3
	};

	#endregion

	#region Reward Catalog
    public enum RewardType
    {
        Regular,
        Payment,
        //Variable
    }

	public enum RewardFulfillmentOption
	{
		Printed,
		Mobile,
		Electronic,
		ThirdParty,
	}

	public enum RewardOrderStatus
	{
		Created,
		Pending,
		InProcess,
		Shipped,
		Cancelled,
		Returned,
		Hold,
		SentForFulfillment,
		Delivered
	}

	#endregion

	#region Customer Service
	public enum AgentAccountStatus
	{
		InActive,
		Active,
		Locked
	}

	public enum CSLoginEventType
	{
		LoginSuccess,
		PasswordInvalid,
		PasswordExpired,
		UsernameInvalid,
		AccountInactive,
		AccountLocked
	}
	#endregion

	#region Categories
	public enum CategoryType
	{
		Product = 1,
		Email = 2,
		Pages = 3,
		Bonus = 4,
		Coupon = 5,

		CampaignManagementCampaigns = 10,
		CampaignManagementTemplates = 11,
		CampaignManagementPlans = 12,

		ReportingReports = 20,
		ReportingDatasets = 21,

		//folder categories for all LNFilterBrowser enabled LNAspDynamicGrids 
		ProductFolder = 101,
		EmailFolder = 102,
		PagesFolder = 103,
		BonusFolder = 104,
		CouponFolder = 105,
		PromotionFolder = 106,
		MigrationSetFolder = 107,
		MessageFolder = 108,
		SurveyFolder = 109,
		ClientConfigurationFolder = 110,
		WorkflowFolder = 111,
		EmailDesignBriefFolder = 112,
		PromotionTestSetFolder = 113,
		EmailTemplatesFolder = 114,
		NotificationFolder = 115
	}
	#endregion

	#region Job Schedule Enums
	public enum ScheduleJobStatus { InProcess, Success, Failure, Cancelled };
	public enum ScheduleType { OneTime, Recurring };
	public enum DailyScheduleType { DailyFrequency, RunEvery };
	public enum HourlyScheduleType { TopOfTheHour, HourFrequency };
	#endregion

	#region MTouch
	public enum MTouchType
	{
		Survey = 0,
		Email = 1,
		Coupon = 2,
		Bonus = 3,
		Promotion = 4,
		LoyaltyCard = 5
	};
	#endregion

	#region survey
	public enum SurveyType
	{
		/// <summary>
		/// General survey
		/// </summary>
		General = 0,

		/// <summary>
		/// Profile survey
		/// </summary>
		Profile = 1
	}

	public enum SurveyStatus
	{
		Design, Active, Closed
	}

	public enum SurveyAudience
	{
		/// <summary>
		/// Respondents are pre-selected
		/// </summary>
		PreSelected = 0,

		/// <summary>
		/// Any member can take
		/// </summary>
		OpenToAnyMember = 1,

		/// <summary>
		/// Everyone can take
		/// </summary>
		OpenToEveryone = 2
	}

	public static class SurveyConstants
	{
		public const string LOG_APP_NAME = "LW_SM";

		public const long SMALL_TEXTBOX_MAXCHARS = 25;
		public const long LARGE_TEXTBOX_MAXCHARS = 2000;
	}

	public enum SurveySelectionMethod
	{
		SurveyID = 1,
		BScript = 2,
		AnyAvailable = 3
	}

	/// <summary>
	/// The type of state in a state diagram.
	/// </summary>
	[Serializable]
	public enum StateType
	{
		InvalidStateType = 0,
		Start = 1,
		Question = 2,
		Decision = 3,
		Terminate = 4,
		Skip = 5,
		Message = 6,
		MatrixQuestion = 7,
		Page = 8,
		PageStart = 9,
		PageEnd = 10
	}

	/// <summary>
	/// The type of a TransitionCollection.
	/// </summary>
	public enum TransitionCollectionType
	{
		Input,
		Output
	}

	/// <summary>
	/// The number of points in a point scale matrix question
	/// </summary>
	[Serializable]
	public enum QA_PointScale
	{
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4,
		Five = 5,
		Six = 6,
		Seven = 7,
		Eight = 8,
		Nine = 9,
		Ten = 10,
		Eleven = 11,
		Twelve = 12
	}

	/// <summary>
	/// The type of control used to render an answer in a questionnaire.
	/// </summary>
	[Serializable]
	public enum QA_AnswerControlType
	{
		RADIO = 0,
		LIST = 1,
		SHORT_TEXT = 2,
		LONG_TEXT = 3,
		CHECK = 4,
		DATETIME = 5,
		DATE = 6,
		TIME = 7,
		INTEGER = 8,
		REAL = 9,
		DOLLAR = 10,
		PERCENT = 11,
		DROPDOWN = 12
	}

	///// <summary>
	///// The text justification of a question or answer in a questionnaire.
	///// </summary>
	//[Serializable]
	//public enum QA_JustificationType
	//{
	//    LEFT = 0,
	//    RIGHT,
	//    CENTER
	//}

	/// <summary>
	/// The orientation of a question or answer in a questionnaire.
	/// </summary>
	[Serializable]
	public enum QA_OrientationType
	{
		HORIZONTAL = 0,
		VERTICAL
	}

	///// <summary>
	///// The layout of the answer relative to the question in a questionnaire.
	///// </summary>
	//[Serializable]
	//public enum QA_LayoutType
	//{
	//    LEFT = 0,
	//    RIGHT,
	//    TOP,
	//    BOTTOM
	//}

	/// <summary>
	/// The type of content stored in a QuestionContent object.
	/// </summary>
	[Serializable]
	public enum QuestionContentType
	{
		HEADER_TEXT = 0,
		BODY_TEXT,
		OTHER_SPECIFY_TEXT,
		ANCHOR_TEXT
	}

	public enum StateModelStatus
	{
		NotInitialized = 0,
		NoLanguage,
		NoSurvey,
		SurveyUnpublished,
		SurveyClosed,
		SurveyNotEffective,
		NoRespondent,
		NoState,
		Initialized,
		NoEligibleRespondent,
		NoMatchingRespondent,
		AlreadyTookSurvey,
		QuotaExceeded,
		OnRestingState,
		Completed,
	}

	[Serializable]
	public enum StateModelTerminationType
	{
		NotInitialized = 0,
		NotYetTerminated,
		Success,
		Skip,
		MissingTransition
	}

	[Serializable]
	public enum SMRespondentListStatusEnum
	{
		NotInitialized = 0,
		ReadyToStage,
		Staging,
		Staged,
		Loading,
		Loaded,
		StagingError,
		LoadingError,
		DeletingData,
		DataDeleted
	}
	#endregion

	#region Device Types
	public enum DeviceType
	{
		Unknown = 0,
		Apple = 1,
		Android = 2
	}
	#endregion

	#region Auctions
	public enum ListingStatus { None, Expired, Ended, CSTerminated, EarlyTerminated };
	#endregion

	#region Transactional Activity
	/// <summary>
	/// Enum ProcessCode, for Processed transaction
	/// </summary>
	public enum ProcessCode
	{
		Processed = 1,
		RequestCreditApplied = 7,
	}
	/// <summary>
	/// Enum TransactionType
	/// </summary>
	public enum TransactionType
	{
		Store = 0,
		Online = 1,
		Mobile = 2,
		Social = 3,
	}
	#endregion

	#region social network
	public enum SocialNetworkProviderType
	{
		None = 0, Composite = 1, Custom = 3, Facebook = 4, Twitter = 5, Google = 6
	}
	#endregion

	#region maps
	public enum MapType
	{
		ROADMAP, SATELLITE, HYBRID, TERRAIN
	}

	public enum MarkerType
	{
		Default, UserSpecifiedURL, RedBalloon, RedDot, GreenBalloon, LightBlueBalloon, BlueBalloon, YellowBalloon, PurpleBalloon, PinkBalloon
	}
	#endregion

	#region Coupons
	public enum CouponStatus
	{
		Active, Killed, Redeemed
	}

	[Flags]
	public enum CouponRedemptionViolation
	{
		None = 0,
		MemberCouponNotActive = 1,
		MemberCouponExpired = 2,
		CouponInKilledStatus = 4,
		RedemptionsPerDayExceeded = 8,
		RedemptionsPerWeekExceeed = 16,
		RedemptionsPerMonthExceeded = 32,
		RedemptionsPerYearExceeded = 64,
		TotalAllowedRedemptionsExceeded = 128
	}


	public enum GlobalCouponRestriction
	{
		// no restrictions are made for global coupons
		None = 0,

		// global coupons are restricted (returns only non-global coupons)
		RestrictGlobal = 1,

		// non-global coupons are restricted (returns only global coupons)
		RestrictNonGlobal = 2
	}
	#endregion

	#region Bonuses
	public enum MemberBonusStatus
	{
		/// <summary>
		/// Expired or not; Not viewed/watched; Survey not taken;
		/// </summary>
		Issued = 0,

		/// <summary>
		/// Expired or not; HtmlOffer viewed, or VideoOffer watched; Survey not taken;
		/// </summary>
		Viewed = 1,

		/// <summary>
		/// Expired or not; HtmlOffer viewed, or VideoOffer watched; Survey taken;
		/// </summary>
		Completed = 2,

		/// <summary>
		/// Expired or not; Not Viewed/watched; Not 'Completed'; Was marked as Save For Later;
		/// </summary>
		Saved = 3
	}

	#endregion

	public enum MemberMessageStatus
	{
		Unread = 0,
		Read = 1,
		Deleted = 2
	}

	public enum MemberMessageOrder
	{
		Newest = 1,
		Oldest = 2
	}

	#region Data Migration
	public enum ArchiveObjectAction
	{
		Created = 0,
		Updated = 1,
		Overwritten = 2,
		None = 3
	};
	public enum SupportedObjectType
	{
		// Data modeling  
		Attribute = 1,
		AttributeSets = 2,
		BusinessRules = 3,
		BscriptExpressions = 4,
		Validator = 5,
		ValidatorTrigger = 6,
		UserEvents = 7,
		GlobalValues = 8,
		LoyaltyEvents = 9,
		LoyaltyCurrency = 10,
		Tiers = 11,
		ContentAttributeDef = 12,
		ContentAttribute = 13,
		Category = 14,
		Products = 15,
		ProductVariant = 16,
		ProductImage = 17,
		Rewards = 18,
		Promotions = 19,
		Messages = 20,
		Bonuses = 21,
		Coupons = 22,
		Stores = 23,
		AppleWalletLoyaltyCards = 24,
		AppleWalletExtendedFields = 25,
		AndroidPayLoyaltyCards = 26,
		X509Certs = 27,
		TextBlocks = 28,
		ContentType = 29,
		Batches = 30,
		SCElement = 31,
		SCAttribute = 32,
		SCDatum = 33,
		EmailCategory = 34,
		EmailTemplate = 35,
		Emails = 36,
		TestLists = 37,
		Websites = 38,
		Skins = 39,
		SkinItems = 40,
		Module = 41,
		Pagelets = 42,
		PageTemplates = 43,
		UserAgents = 44,
		Campaigns = 45,
		Audience = 46,
		TableMapping = 47,
		Channels = 48,

		// Surveys
		Surveys = 49,

		//Reference Data
		ReferenceData = 50,

		// Promotion Test Set
		PromoTestSets = 51,
		PromoTemplates = 52,
		PromoMappingFiles = 53,
		PromoDataFiles = 54,

		SmsTemplate = 55,
		Sms = 56,

		Notifications = 57,
        RemoteAssemblies = 58, 

        DefaultRewards = 59
	}

	#endregion

	#region contact history
	public enum ContactTypeEnum
	{
		Email = 1,
		SMS = 2,
		DirectMail = 3
	}
	#endregion

	#region Data Archiving

	/// <summary>
	/// CS: CS Portal;
	/// CF: Customer Facing web site;
	/// LN: Loyalty Navigator;
	/// DA: DAP;
	/// AP: API;
	/// RU: Rule Processor;
	/// OT: Other
	/// </summary>
	public enum ApplicationType { CS = 1, CF, LN, DA, AP, RU, OT };

	/// <summary>
	/// U: Update;
	/// I: Insert;
	/// D: Delete
	/// </summary>
	public enum DataChangeType { U, I, D };

	/// <summary>
	/// The value type stored in authenticated user's Identity.Name field. 
	/// This only applies to web applications (CS portal, CF site and Loyalty Navigator).
	///   Ipcode: what we normally use for CF portal;
	///   AgentUserName: what we normally use for CS portal;
	///   UserName: what we use for Loyalty Navigator
	/// </summary>
	public enum UserIdentityType { Ipcode, AgentUserName, UserName, Unknown };
	#endregion

	#region PassBook Service
	public enum PassbookPassStyle
	{
		storeCard,
		coupon
	}

	public enum PassbookBarcodeFormat
	{
		PKBarcodeFormatQR,
		PKBarcodeFormatPDF417,
		PKBarcodeFormatAztec
	}
	#endregion

	#region X509Cert
	public enum X509CertType
	{
		AppleWWDRCACert,
		OtherCACert,
		PassbookSigningCert,
		AppleApplicationCert,
		OtherCert,
		SSOPartnerCert,
		SSOLWSiteCert
	}
	#endregion

	#region Mobile Events
	public enum MemberMobileEventActionType
	{
		CheckIn = 1
	}

	public enum MemberMobileCheckInStatus
	{
		Ok = 1,
		AlreadyCheckedIn = 2,
		NotInStore = 3,
		ExceededMaxAllowed = 4
	}
	#endregion

	#region Message Queueing
	public enum ConsumerLifecyclePolicy { Singleton, PerInstanceConsumption, ConsumerPool };
	#endregion

	#region Social Listening
	//public enum SocialPublishers { Twitter, Facebook };
	public enum SocialSentiment { Neutral, Positive, Negative };
    #endregion

    #region REST API

    public enum RestResourceType
    {
        /// <summary>
        /// A REST API Route
        /// </summary>
        RestRoute = 1
    }

    public enum RestPermissionType
    {
        /// <summary>
        /// Create Permission
        /// </summary>
        Create = 1,

        /// <summary>
        /// Read Permission
        /// </summary>
        Read = 2,

        /// <summary>
        /// Update Permission
        /// </summary>
        Update = 3,

        /// <summary>
        /// Delete Permission
        /// </summary>
        Delete = 4,

        /// <summary>
        /// Write Permission
        /// </summary>
        Write = 5,

        /// <summary>
        /// Full Permissions
        /// </summary>
        Full = 6
    }

    #endregion
}
