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
	public class X509CertDao : DaoBase<X509Cert>
	{
		public X509CertDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public X509Cert Retrieve(long id)
		{
			return Database.SingleOrDefault<X509Cert>(id);
		}

		public X509Cert Retrieve(string certName)
		{
			return Database.FirstOrDefault<X509Cert>("select * from LW_X509Cert where CertName = @0", certName);
		}

		public List<X509Cert> RetrieveAll()
		{
			return Database.Fetch<X509Cert>("select * from LW_X509Cert");
		}

		public List<X509Cert> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<X509Cert>("select * from LW_X509Cert where updatedate >= @0", changedSince);
		}

		public List<X509Cert> RetrieveAll(string sortExpression, bool ascending)
		{
			if(string.IsNullOrEmpty(sortExpression))
			{
				sortExpression = "id";
			}
            string sql = string.Empty;
            switch (sortExpression.ToLower())
            {
                case "certname":
                    sql = "select * from LW_X509Cert order by certname";
                    break;
                case "passtypeid":
                    sql = "select * from LW_X509Cert order by passtypeid";
                    break;
                case "certtype":
                    sql = "select * from LW_X509Cert order by certtype";
                    break;
                case "certpassword":
                    sql = "select * from LW_X509Cert order by certpassword";
                    break;
                case "isproduction":
                    sql = "select * from LW_X509Cert order by isproduction";
                    break;
                default:
                    sql = "select * from LW_X509Cert order by id";
                    break;
            }
            sql = string.Format("{0} {1}", sql, ascending ? "ASC" : "DESC");
            return Database.Fetch<X509Cert>(sql);
		}

		public List<X509Cert> RetrieveByPassType(string passType)
		{
			return Database.Fetch<X509Cert>("select * from LW_X509Cert where PassTypeId = @0", passType);
		}

		public List<X509Cert> RetrieveAllByCertType(X509CertType certType)
		{
			return Database.Fetch<X509Cert>("select * from LW_X509Cert where CertType = @0", certType.ToString());
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}