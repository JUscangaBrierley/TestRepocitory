using System;

namespace Brierley.FrameWork.bScript
{
	/// <summary>
	/// Contains constants representing the known list of keys passed to ContextObject's Environment dictionary.
	/// </summary>
	/// <remarks>
	/// The creation of this class was driven mainly by the need to put the GetEnvironmentString bScript function into the
	/// expression builder. The function works better if a known list of values are shown in the suggestion list.
	/// </remarks>
	public sealed class EnvironmentKeys
	{
		public const string OverrideParameters = "OverrideParameters";
		public const string Result = "Result";
		public const string CurrentCampaign = "CurrentCampaign";
		public const string PromotionCode = "PromotionCode";
		public const string Targeted = "Targeted";
		public const string Enrollment = "Enrollment";
		public const string StoreNumber = "StoreNumber";
		public const string StoreName = "StoreName";
		public const string Channel = "Channel";
		public const string Language = "Language";
		public const string BonusName = "BonusName";
		public const string OwnerType = "OwnerType";
		public const string OwnerId = "OwnerId";
		public const string RowKey = "RowKey";
		public const string RecipientEmail = "recipientemail";
		public const string MemberIdentifier = "MemberIdentifier";
	}
}
