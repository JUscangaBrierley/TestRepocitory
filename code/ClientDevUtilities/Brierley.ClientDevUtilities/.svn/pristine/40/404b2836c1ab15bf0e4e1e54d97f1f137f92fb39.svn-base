using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CertificateDao : DaoBase<PromotionCertificate>
	{
		public CertificateDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public int MarkAsUsed(string[] certNmbrs)
		{
            if (certNmbrs == null || certNmbrs.Length == 0)
                return 0;
			return Database.Execute("update LW_PromotionCertificate set Available = 0 where CertNmbr in (@certs)", new { certs = certNmbrs });
		}

		public int ReclaimCertificates(string[] certNmbrs)
		{
            if (certNmbrs == null || certNmbrs.Length == 0)
                return 0;
			return Database.Execute("update LW_PromotionCertificate set Available = 1 where CertNmbr in (@certs)", new { certs = certNmbrs });
		}

		public PromotionCertificate Retrieve(string certNmbr)
		{
			return Database.FirstOrDefault<PromotionCertificate>("select * from LW_PromotionCertificate where CertNmbr = @0", certNmbr);
		}

		public PromotionCertificate RetrieveFirstAvailable(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate)
		{
			List<PromotionCertificate> certs = RetrieveAvailable(objectType, typeCode, startDate, endDate, 1);
			return certs != null && certs.Count > 0 ? certs[0] : null;
		}

		public List<PromotionCertificate> RetrieveAvailable(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate, int howMany)
		{
			var args = new object[] { objectType.ToString(), typeCode, startDate.GetValueOrDefault(DateTimeUtil.MinValue), endDate.GetValueOrDefault(DateTimeUtil.MaxValue) };

            string where = "cert.ObjectType = @0 and cert.TypeCode = @1 and cert.Available = 1";
            if (startDate != null || endDate != null)
            {
                where += "and (cert.StartDate <= @2 or cert.StartDate is null) and (cert.ExpiryDate > @3 or cert.ExpiryDate is null) ";
            }

            string sql = CreateLimitQuery("LW_PromotionCertificate", "cert", where, string.Empty, howMany, LockingMode.Write);			

			var ret = Database.Fetch<PromotionCertificate>(sql, args);

			if (ret != null && ret.Count > 0)
			{
				var certNmbrs = from x in ret select x.CertNmbr;
                MarkAsUsed(certNmbrs.ToArray<string>());
			}
			return ret;
		}

		public long HowMany(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate, bool? available)
		{
			string sql = "select count(*) from LW_PromotionCertificate where TypeCode = @0";
			var parameters = new List<object>() { typeCode };

			if (objectType != null)
			{
				sql += " and ObjectType = @" + parameters.Count.ToString();
				parameters.Add(objectType.ToString());
			}
			if (startDate.HasValue)
			{
				sql += " and (StartDate is null or StartDate <= @" + parameters.Count.ToString() + ")";
				parameters.Add(startDate.Value);
			}
			if (endDate.HasValue)
			{
				sql += " and (ExpiryDate is null or ExpiryDate > @" + parameters.Count.ToString() + ")";
				parameters.Add(startDate.Value);
			}
			if (available.HasValue)
			{
				sql += " and Available = @" + parameters.Count.ToString();
				parameters.Add(available.Value);
			}
			return Database.ExecuteScalar<int>(sql, parameters.ToArray());
		}

		public void Delete(string certNmbr)
		{
			Database.Execute("delete from LW_PromotionCertificate where CertNmbr = @0", certNmbr);
		}

		//private int Delete(string[] certNmbrs)
		//{
		//	return Database.Execute("delete from LW_PromotionCertificate where CertNmbr in (@certs)", new { certs = certNmbrs });
		//}

		protected override void SaveEntity(object entity)
		{
			var cert = (PromotionCertificate)entity;
			if (cert.StartDate.HasValue)
			{
				cert.StartDate = DateTimeUtil.GetBeginningOfDay(cert.StartDate.Value);
			}
			if (cert.ExpiryDate.HasValue)
			{
				cert.ExpiryDate = DateTimeUtil.GetEndOfDay(cert.ExpiryDate.Value);
			}
			base.SaveEntity(cert);
		}
	}
}
