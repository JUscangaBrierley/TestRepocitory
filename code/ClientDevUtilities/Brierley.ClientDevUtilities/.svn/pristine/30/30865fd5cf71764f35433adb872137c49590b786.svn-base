using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.CampaignManagement.DataProvider.MsSQL
{
	public class MSSqlBulkOutput : IBulkOutputProvider
	{
		const string _notImplemented = "Bulk output operations to an MS SQL Server database are not supported in this release.";

		public int BatchSize
		{
			get
			{
				return 0;
			}
		}

		public void OutputBonuses(DataTable table, long bonusId, long surveyId, long languageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config)
		{
			throw new NotImplementedException(_notImplemented);
		}

		public void OutputCoupons(DataTable table, long couponId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string couponCode)
		{
			throw new NotImplementedException(_notImplemented);
		}

		public void OutputPromotions(DataTable table, string promotionCode, ServiceConfig config, bool useCertificates)
		{
			throw new NotImplementedException(_notImplemented);
		}

		public void OutputSurveys(DataTable table, long surveyId, long languageId, ServiceConfig config)
		{
			throw new NotImplementedException(_notImplemented);
		}

		public void OutputMessages(DataTable table, long messageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config)
		{
			throw new NotImplementedException(_notImplemented);
		}

		public void OutputNextBestActions(List<MemberNextBestAction> actions, DateTime? start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string typeCode)
		{
			throw new NotImplementedException(_notImplemented);
		}
	}
}
