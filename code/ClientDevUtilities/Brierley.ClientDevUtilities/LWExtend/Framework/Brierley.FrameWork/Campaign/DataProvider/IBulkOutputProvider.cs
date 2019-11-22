using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.CampaignManagement.DataProvider
{
	public interface IBulkOutputProvider
	{
		int BatchSize { get; }
		void OutputBonuses(DataTable table, long bonusId, long surveyId, long languageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config);
		void OutputCoupons(DataTable table, long couponId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string couponCode);
		void OutputPromotions(DataTable table, string promotionCode, ServiceConfig config, bool useCertificates);
		void OutputSurveys(DataTable table, long surveyId, long languageId, ServiceConfig config);
		void OutputMessages(DataTable table, long messageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config);
		void OutputNextBestActions(List<MemberNextBestAction> actions, DateTime? start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string typeCode);
	}
}
