using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// MGMemberBonus is, to the Mobile Gateway, a merge of MemberBonus and BonusDef. To list
	/// only member bonus data is pointless, as the client would need to retrieve the bonus def
	/// in order to have presentable data (i.e., listing the member's bonuses), so we merge the 
	/// two (member bonus + name, headline and image URLs).
	/// </remarks>
	public class MGMemberBonus
	{
		[DataMember]
		public virtual long Id { get; set; }

		[DataMember]
		public virtual long BonusDefId { get; set; }

		[DataMember]
		public virtual string Description { get; set; }

		[DataMember]
		public virtual string LogoImageHero { get; set; }

		[DataMember]
		public virtual string LogoImageWeb { get; set; }

		[DataMember]
		public virtual string LogoImageMobile { get; set; }

		[DataMember]
		public virtual int? DisplayOrder { get; set; }

		[DataMember]
		public virtual string Headline { get; set; }

		[DataMember]
		public virtual string SurveyText { get; set; }

		[DataMember]
		public virtual string MovieUrl { get; set; }

		[DataMember]
		public virtual long? SurveyId { get; set; }

		[DataMember]
		public virtual MemberBonusStatus Status { get; set; }

		[DataMember]
		public virtual bool ReferralCompleted { get; set; }

		[DataMember]
		public virtual decimal? Points { get; set; }

		[DataMember]
		public virtual string GoButtonLabel { get; set; }

		public static MGMemberBonus Hydrate(Member member, MemberBonus memberBonus, string language, string channel)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}

			if (memberBonus == null)
			{
				throw new ArgumentNullException("memberBonus");
			}

			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				var bonus = content.GetBonusDef(memberBonus.BonusDefId);

				var ret = new MGMemberBonus()
				{
					Id = memberBonus.ID,
					BonusDefId = bonus.Id,
					Description = bonus.GetDescription(language, channel),
					LogoImageHero = bonus.LogoImageHero,
					LogoImageWeb = bonus.LogoImageWeb,
					LogoImageMobile = bonus.LogoImageMobile,
					Headline = bonus.GetHeadline(language, channel),
					GoButtonLabel = bonus.GetGoButtonLabel(language, channel),
					Points = bonus.Points,
					SurveyText = bonus.SurveyText,
					SurveyId = bonus.SurveyId,
					MovieUrl = bonus.MovieUrl,
					DisplayOrder = bonus.DisplayOrder.GetValueOrDefault(1),
					Status = memberBonus.Status,
					ReferralCompleted = memberBonus.ReferralCompleted
				};
				return ret;
			}
		}
	}
}