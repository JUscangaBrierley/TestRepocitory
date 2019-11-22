using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class ExchangeRateDao : DaoBase<ExchangeRate>
    {

        public ExchangeRateDao(Database database, ServiceConfig serviceConfig) 
            : base(database, serviceConfig)
        {
        }

        public ExchangeRate Retrive(long exchangeRateId)
        {
            ExchangeRate exchangeRate = GetEntity(exchangeRateId);

            return exchangeRate;
        }

        public ExchangeRate Retrive(string fromCurrency, string toCurrency)
        {
            return Database.FirstOrDefault<ExchangeRate>("select * from LW_ExchangeRate where FromCurrency = @0 and ToCurrency = @1 order by CreateDate desc", fromCurrency, toCurrency);
        }

        public PetaPoco.Page<ExchangeRate> RetrieveAll(long page, long resultsPerPage)
        {
            return Database.Page<ExchangeRate>(page, resultsPerPage, "select r.* from LW_ExchangeRate r order by CreateDate desc");
        }

        public PetaPoco.Page<ExchangeRate> RetrieveAllByToCurrency(string toCurrency, long page, long resultsPerPage)
        {
            return Database.Page<ExchangeRate>(page, resultsPerPage, "select r.* from LW_ExchangeRate r where r.toCurrency = @0 order by CreateDate desc", toCurrency);
        }

        public PetaPoco.Page<ExchangeRate> RetrieveAllByFromCurrency(string fromCurrency, long page, long resultsPerPage)
        {
            return Database.Page<ExchangeRate>(page, resultsPerPage, "select r.* from LW_ExchangeRate r where r.fromcurrency = @0 order by CreateDate desc", fromCurrency);
        }

        public PetaPoco.Page<ExchangeRate> RetriveByProperty(List<Dictionary<string, object>> properties, long page, long resultsPerPage)
        {
            var parameters = new List<object>();
            string sql = "select t.* from LW_ExchangeRate t";
            bool hasWhere = false;
            bool first = true;

            foreach (var cond in properties)
            {
                if (first)
                {
                    if (!hasWhere)
                    {
                        sql += " where ";
                        hasWhere = true;
                    }
                    else
                    {
                        sql += " and ";
                    }
                    first = false;
                }
                else
                {
                    string op = cond.ContainsKey("Operator") ? cond["Operator"].ToString() : " and ";
                    sql += string.Format(" {0} ", op);
                }

                sql += "(";
                sql += HandleSqlForProperties((string)cond["Property"], (LWCriterion.Predicate)cond["Predicate"], cond["Value"], parameters);
                sql += ")";

            }

            sql += " order by CreateDate desc";

            return Database.Page<ExchangeRate>(page, resultsPerPage, sql, parameters.ToArray());

        }

        private string HandleSqlForProperties(string propertyName, LWCriterion.Predicate op, object propertyValue, List<object> parameters)
        {
            string sql = string.Empty;

            bool propValNotNull = propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString());
            if (op == LWCriterion.Predicate.Ne)
            {
                if (propValNotNull)
                {
                    sql += string.Format("(t.{0} {1} @" + parameters.Count.ToString() + " or t.{0} is null)", propertyName, LWCriterion.GetSqlPredicate(op));
                    parameters.Add(propertyValue);
                }
                else
                {
                    sql += string.Format("t.{0} is not null", propertyName);
                }
            }
            else if (op == LWCriterion.Predicate.Eq && !propValNotNull)
            {
                sql += string.Format("(t.{0} is null or t.{0}='')", propertyName);
            }
            else
            {
                sql += string.Format("t.{0} {1} @{2}", propertyName, LWCriterion.GetSqlPredicate(op), parameters.Count.ToString());
                parameters.Add(propertyValue);
            }

            return sql;
        }

        public void Delete(long exchangeRateId)
        {
            DeleteEntity(exchangeRateId);
        }
    }
}
