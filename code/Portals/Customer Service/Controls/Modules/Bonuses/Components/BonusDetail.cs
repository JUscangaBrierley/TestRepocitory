using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.Bonuses.Components
{
	public enum Actions
	{
		Video,
		Survey,
		Html
	}

	public class BonusDetail
	{
		public long Id { get; set; }

        public long BonusDefId { get; set; }
		
		public string Description { get; set; }

		public string LogoImageHero { get; set; }

		public string LogoImageWeb { get; set; }

		public string LogoImageMobile { get; set; }

		public int? DisplayOrder { get; set; }

		public String Headline { get; set; }

		public string HtmlContent { get; set; }

		public String FinishedHtml { get; set; }

		public String QuotaMetHtml { get; set; }

		public String SurveyText { get; set; }

        public String ReferralLabel { get; set; }

		public String MovieUrl { get; set; }

		public string ReferralUrl { get; set; }

		public long? SurveyId { get; set; }

		public MemberBonusStatus Status { get; set; }

		public bool ReferralCompleted { get; set; }

		public bool IsVideo { get; set; }

        public decimal? Points { get; set; }

        public string SurveyPointsExpression { get; set; }

		public Int64? Quota { get; set; }

		public bool ApplyQuotaToReferral { get; set; }

		public string GoButtonLabel { get; set; }

		public Actions NextAction
		{
			get
			{
				Actions ret = Actions.Video;
				switch (Status)
				{
					case MemberBonusStatus.Issued:
					case MemberBonusStatus.Saved:
						ret = string.IsNullOrEmpty(MovieUrl) ? Actions.Html : Actions.Video;
						break;
					case MemberBonusStatus.Viewed:
						ret = Actions.Survey;
						break;
				}
				return ret;
			}

		}


	}
}