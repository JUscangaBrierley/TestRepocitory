using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.CampaignManagement.DataProvider.Oracle
{
	public class OracleBulkOutput : IBulkOutputProvider
	{
		private const int _batchSize = 2000;
        private const string OracleTimestampFormat = "YYYYMMDDHH24MISS";
        private const string DateTimeOracleFormat = "yyyyMMddHHmmss";

		public int BatchSize
		{
			get
			{
				return _batchSize;
			}
		}

		public void OutputBonuses(System.Data.DataTable table, long bonusId, long surveyId, long languageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config)
		{
			//string query = "INSERT INTO LW_MemberBonus(Id, BonusDefId, MemberId, TimesClicked, CreateDate, UpdateDate, SurveyTaken, ActionTaken, MTouch, ExpiryDate) ";
			string query = "INSERT INTO LW_MemberBonus(Id, BonusDefId, MemberId, TimesClicked, CreateDate, UpdateDate, Status, ReferralCompleted, StartDate, ExpiryDate) ";
			if (expiration.HasValue)
			{
                query += string.Format("VALUES (hibernate_sequence.nextval, {0}, :ipcode, 0, sysdate, sysdate, 'Issued', 0, to_date('{1}','{2}', to_date('{3}','{4}'))", bonusId, start.ToString(DateTimeOracleFormat), OracleTimestampFormat, expiration.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			}
			else
			{
				query += string.Format("VALUES (hibernate_sequence.nextval, {0}, :ipcode, 0, sysdate, sysdate, 'Issued', 0, null, to_date('{1}','{2}')", bonusId, start.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			}

			if (surveyId > 0 && languageId > 0)
			{
				query = "BEGIN " + query;
				query += "; INSERT INTO LW_SM_Respondent(Respondent_Id, IPCode, Survey_Id, Language_Id, CreateDate, UpdateDate, Skipped) ";
				query += string.Format("VALUES (hibernate_sequence.nextval, :ipcode, {0}, {1}, sysdate, sysdate, 'F'); END;", surveyId, languageId);
			}

			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                cmd.Prepare();


                while (index < table.Rows.Count)
                {
                    long[] ipcodes = GetIPCodes(table, index, _batchSize);
                    if (ipcodes.Length > 0)
                    {
                        pipcode.Value = ipcodes;
                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		public void OutputCoupons(System.Data.DataTable table, long couponId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string couponCode)
		{
			string query = string.Empty;
			if (useCertificates)
			{
				query = "INSERT INTO LW_MemberCoupon(Id, CouponDefId, MemberId, CreateDate, UpdateDate, TimesUsed, DateIssued, CertificateNmbr, StartDate ";
			}
			else
			{
				query = "INSERT INTO LW_MemberCoupon(Id, CouponDefId, MemberId, CreateDate, UpdateDate, TimesUsed, DateIssued, StartDate ";
			}
			if (expiration.HasValue)
			{
				query += ", ExpiryDate";
			}
			if (displayOrder.HasValue)
			{
				query += ", DisplayOrder";
			}
			query += ")";
			query += string.Format(" VALUES (hibernate_sequence.nextval, {0}, :ipcode, sysdate, sysdate, 0, sysdate, to_date('{1}','{2}')", couponId, start.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			if (useCertificates)
			{
				query += ", :certNumber";
			}
			if (expiration.HasValue)
			{
                query += string.Format(", to_date('{0}','{1}')", expiration.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			}
			if (displayOrder.HasValue)
			{
				query += string.Format(", {0}", displayOrder.Value.ToString());
			}
			query += ")";

			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                IDbDataParameter pcertNumber = null;
                if (useCertificates)
                {
                    pcertNumber = cmd.CreateParameter();
                    pcertNumber.ParameterName = "certNumber";
                    pcertNumber.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(pcertNumber);
                }

                cmd.Prepare();

                while (index < table.Rows.Count)
                {
                    long[] ipcodes = GetIPCodes(table, index, _batchSize);
                    if (ipcodes.Length > 0)
                    {
                        if (useCertificates)
                        {
                            string[] certs = GetCertificates(config, ContentObjType.Coupon, couponCode, ipcodes.Length);
                            pcertNumber.Value = certs;
                        }
                        pipcode.Value = ipcodes;
                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		public void OutputPromotions(System.Data.DataTable table, string promotionCode, ServiceConfig config, bool useCertificates)
		{
			string query = string.Empty;
			if (useCertificates)
			{
				query = "INSERT INTO LW_MemberPromotion(Id, Code, MemberId, Enrolled, CreateDate, UpdateDate, CertificateNmbr) ";
				query += "VALUES (hibernate_sequence.nextval, :code, :ipcode, 0, sysdate, sysdate, :certNumber)";
			}
			else
			{
				query = "INSERT INTO LW_MemberPromotion(Id, Code, MemberId, Enrolled, CreateDate, UpdateDate) ";
				query += "VALUES (hibernate_sequence.nextval, :code, :ipcode, 0, sysdate, sysdate)";
			}



			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pcode = cmd.CreateParameter();
                pcode.ParameterName = "code";
                pcode.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(pcode);

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                IDbDataParameter pcertNumber = null;
                if (useCertificates)
                {
                    pcertNumber = cmd.CreateParameter();
                    pcertNumber.ParameterName = "certNumber";
                    pcertNumber.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(pcertNumber);
                }

                cmd.Prepare();

                while (index < table.Rows.Count)
                {
                    long[] ipcodes = GetIPCodes(table, index, _batchSize);
                    if (ipcodes.Length > 0)
                    {
                        if (useCertificates)
                        {
                            string[] certs = GetCertificates(config, ContentObjType.Promotion, promotionCode, ipcodes.Length);
                            pcertNumber.Value = certs;
                        }

                        pipcode.Value = ipcodes;

                        string[] promoCodes = new string[ipcodes.Length];
                        for (int i = 0; i < promoCodes.Length; i++)
                        {
                            promoCodes[i] = promotionCode;
                        }
                        pcode.Value = promoCodes;

                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		public void OutputSurveys(System.Data.DataTable table, long surveyId, long languageId, ServiceConfig config)
		{
			string query = "INSERT INTO LW_SM_Respondent(Respondent_Id, IPCode, Survey_Id, Language_Id, CreateDate, UpdateDate, Skipped) ";
			query += string.Format("VALUES (hibernate_sequence.nextval, :ipcode, {0}, {1}, sysdate, sysdate, 'F')", surveyId, languageId);

			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                cmd.Prepare();

                while (index < table.Rows.Count)
                {
                    long[] ipcodes = GetIPCodes(table, index, _batchSize);
                    if (ipcodes.Length > 0)
                    {
                        pipcode.Value = ipcodes;
                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		public void OutputMessages(DataTable table, long messageId, DateTime start, DateTime? expiration, int? displayOrder, ServiceConfig config)
		{
			string query = "INSERT INTO LW_MemberMessage(Id, MessageDefId, MemberId, DateIssued, CreateDate, UpdateDate, Status, StartDate ";
			if (expiration.HasValue)
			{
				query += ", ExpiryDate";
			}
			if (displayOrder.HasValue)
			{
				query += ", DisplayOrder";
			}
			query += ")";
			query += string.Format(" VALUES (hibernate_sequence.nextval, {0}, :ipcode, sysdate, sysdate, sysdate, 0, to_date('{1}','{2}')", messageId, start.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			if (expiration.HasValue)
			{
                query += string.Format(", to_date('{0}','{1}')", expiration.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
			}
			if (displayOrder.HasValue)
			{
				query += string.Format(", {0}", displayOrder.Value.ToString());
			}
			query += ")";

			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                cmd.Prepare();


                while (index < table.Rows.Count)
                {
                    long[] ipcodes = GetIPCodes(table, index, _batchSize);
                    if (ipcodes.Length > 0)
                    {
                        pipcode.Value = ipcodes;
                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		public void OutputNextBestActions(List<MemberNextBestAction> actions, DateTime? start, DateTime? expiration, int? displayOrder, ServiceConfig config, bool useCertificates, string typeCode)
		{
			if (actions == null || actions.Count() == 0)
			{
				return;
			}

			if (actions.Select(o => o.ActionType).Distinct().Count() > 1)
			{
				throw new ArgumentException(
					string.Format(
					"OutputNextBestActions expects a list containing records with a common action type. The list passed has {0} actions types.",
					actions.Select(o => o.ActionType).Distinct().Count()));
			}

			if (actions.Select(o => o.ActionId).Distinct().Count() > 1)
			{
				throw new ArgumentException(
					string.Format(
					"OutputNextBestActions expects a list containing records with a common action id. The list passed has {0} actions ids.",
					actions.Select(o => o.ActionId).Distinct().Count()));
			}

			var actionType = actions.First().ActionType;
			if (actionType == NextBestActionType.Sku)
			{
				//SKUs cannot be assigned. Nothing to do here.
				return;
			}

			long actionId = actions.First().ActionId;

			string query = string.Format(
@"DECLARE
	hibernate number := hibernate_sequence.nextval;
BEGIN
	{{0}};
	INSERT INTO LW_MemberNextBestAction(Id, MemberId, Priority, ActionType, ActionId, MemberActionId, CreateDate)
	VALUES(hibernate_sequence.nextval, :ipcode, :priority, {0}, {1}, hibernate, sysdate);
END;",
			(int)actionType, 
			actionId);

			switch (actionType)
			{
				case NextBestActionType.Coupon:
					{
						string q = string.Empty;
						if (useCertificates)
						{
							q = "INSERT INTO LW_MemberCoupon(Id, CouponDefId, MemberId, CreateDate, UpdateDate, TimesUsed, DateIssued, CertificateNmbr, StartDate ";
						}
						else
						{
							q = "INSERT INTO LW_MemberCoupon(Id, CouponDefId, MemberId, CreateDate, UpdateDate, TimesUsed, DateIssued, StartDate ";
						}
						if (expiration.HasValue)
						{
							q += ", ExpiryDate";
						}
						if (displayOrder.HasValue)
						{
							q += ", DisplayOrder";
						}

                        string startQuery = string.Empty;
                        if (start.HasValue)
                        {
                            startQuery = string.Format("to_date('{1}','{2}')", start.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
                        }
                        else
                        {
                            startQuery = string.Format("(select StartDate from LW_CouponDef where Id = {0})", actionId);
                        }

						q += string.Format(") VALUES (hibernate, {0}, :ipcode, sysdate, sysdate, 0, sysdate, {1}", actionId, startQuery);
						if (useCertificates)
						{
							q += ", :certNumber";
						}
						if (expiration.HasValue)
						{
                            q += string.Format(", to_date('{0}','{1}')", expiration.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
						}
						if (displayOrder.HasValue)
						{
							q += string.Format(", {0}", displayOrder.Value.ToString());
						}
						q += ")";
						query = string.Format(query, q);
					}
					break;
				case NextBestActionType.Message:
					{
						string q = "INSERT INTO LW_MemberMessage(Id, MessageDefId, MemberId, DateIssued, CreateDate, UpdateDate, Status, StartDate ";
						if (expiration.HasValue)
						{
							q += ", ExpiryDate";
						}
						if (displayOrder.HasValue)
						{
							q += ", DisplayOrder";
						}

                        string startQuery = string.Empty;
                        if (start.HasValue)
                        {
                            startQuery = string.Format("to_date('{1}','{2}')", start.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
                        }
                        else
                        {
                            startQuery = string.Format("(select StartDate from LW_MessageDef where Id = {0})", actionId);
                        }

                        q += string.Format(") VALUES (hibernate, {0}, :ipcode, sysdate, sysdate, sysdate, 0, {0}", actionId, startQuery);
						if (expiration.HasValue)
						{
                            q += string.Format(", to_date('{0}','{1}')", expiration.Value.ToString(DateTimeOracleFormat), OracleTimestampFormat);
						}
						if (displayOrder.HasValue)
						{
							q += string.Format(", {0}", displayOrder.Value.ToString());
						}
						q += ")";
						query = string.Format(query, q);
					}
					break;
				default:
					return;
			}

			int index = 0;
            using (var connection = config.CreateDatabase().Connection)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;

                var pipcode = cmd.CreateParameter();
                pipcode.ParameterName = "ipcode";
                pipcode.DbType = System.Data.DbType.Int64;
                cmd.Parameters.Add(pipcode);

                IDbDataParameter pcertNumber = null;
                if (actionType == NextBestActionType.Coupon && useCertificates)
                {
                    pcertNumber = cmd.CreateParameter();
                    pcertNumber.ParameterName = "certNumber";
                    pcertNumber.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(pcertNumber);
                }

                var priority = cmd.CreateParameter();
                priority.ParameterName = "priority";
                priority.DbType = System.Data.DbType.Int32;
                cmd.Parameters.Add(priority);

                cmd.Prepare();

                while (index < actions.Count())
                {
                    long[] ipcodes = Get<long>(actions, index, _batchSize, o => o.MemberId);
                    if (ipcodes.Length > 0)
                    {
                        if (useCertificates && actionType == NextBestActionType.Coupon)
                        {
                            string[] certs = GetCertificates(config, ContentObjType.Coupon, typeCode, ipcodes.Length);
                            pcertNumber.Value = certs;
                        }
                        int[] priorities = Get<int>(actions, index, _batchSize, o => o.Priority);
                        pipcode.Value = ipcodes;
                        priority.Value = priorities;
                        SetArrayBindCount(cmd, ipcodes.Length);
                        cmd.ExecuteNonQuery();
                        index += ipcodes.Length;
                    }
                    else
                    {
                        break;
                    }
                }
            }
		}

		protected string[] GetCertificates(ServiceConfig config, ContentObjType type, string typeCode, int count)
		{
			//todo: need a way to retrieve and kill batches of certificates
			string[] certs = new string[count];
			using (var contentService = new ContentService(config))
			{
				for (long i = 0; i < count; i++)
				{
					var cert = contentService.RetrieveFirstAvailablePromoCertificate(type, typeCode, null, null);
					if (cert == null)
					{
						throw new LWException(string.Format("Cannot retrieve certificates of type {0} for typecode {1}. No more certificates available.", type.ToString(), typeCode));
					}
					certs[i] = cert.CertNmbr;
				}
			}
			return certs;
		}

		private long[] GetIPCodes(System.Data.DataTable table, int index, int size)
		{
			long[] ipcodes = (long[])Array.CreateInstance(typeof(long), Math.Min(_batchSize, table.Rows.Count - index));
			for (int i = 0; i < ipcodes.Length; i++)
			{
				ipcodes[i] = Convert.ToInt64(table.Rows[index + i]["ipcode"]);
			}
			return ipcodes;
		}

		private T[] Get<T>(List<MemberNextBestAction> actions, int index, int size, Func<MemberNextBestAction, T> selector)
		{
			if (index > actions.Count)
			{
				return new T[] { };
			}
			if (index + size > actions.Count)
			{
				size = actions.Count - index;
			}

			T[] ipcodes = actions.GetRange(index, size).Select(selector).ToArray();
			return ipcodes;
		}

		private void SetArrayBindCount(IDbCommand command, int count)
		{
			command.GetType().InvokeMember("ArrayBindCount", System.Reflection.BindingFlags.SetProperty, null, command, new object[] { count });
		}
	}
}
