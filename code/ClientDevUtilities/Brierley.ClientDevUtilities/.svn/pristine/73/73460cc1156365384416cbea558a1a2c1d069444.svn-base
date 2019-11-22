//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data
{
	/// <summary>
	/// This class provides a way to define criteria for executing dynamic queries.
	/// </summary>
	public class LWCriterion
	{
		private const string _className = "LWCriterion";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);


		public enum OperatorType
		{
			AND,
			OR
		}

		public enum Predicate
		{
			Eq,
			Ne,
			Le,
			Lt,
			Ge,
			Gt,
			In,
			Like
		}

		private List<CriteriaComponent> criteria = new List<CriteriaComponent>();
		private Dictionary<string, string> orderBy = new Dictionary<string, string>();
		private List<string> distinct = new List<string>();


		public enum OrderType { Ascending, Descending }

		public string AttributeSetName { get; set; }

		public string Alias { get; set; }

		public bool UseAlias { get; set; }


		public LWCriterion(string attSetName)
		{
			AttributeSetName = attSetName;
			Alias = "a";
		}

		public static string GetSqlPredicate(Predicate predicate)
		{
			string strPred = string.Empty;
			if (predicate == Predicate.Eq)
			{
				strPred = "=";
			}
			else if (predicate == Predicate.Ge)
			{
				strPred = ">=";
			}
			else if (predicate == Predicate.Gt)
			{
				strPred = ">";
			}
			else if (predicate == Predicate.Le)
			{
				strPred = "<=";
			}
			else if (predicate == Predicate.Lt)
			{
				strPred = "<";
			}
			else if (predicate == Predicate.Ne)
			{
				strPred = "<>";
			}
			else if (predicate == Predicate.In)
			{
				strPred = " in(";
			}
			else if (predicate == Predicate.Like)
			{
				strPred = " like ";
			}
			return strPred;
		}


		public void Add(OperatorType op, string name, object value, Predicate predicate)
		{
			CriteriaComponent comp = new CriteriaComponent(name, value, predicate, op);
			criteria.Add(comp);
		}

		public void AddOrderBy(string propertyName, OrderType orderType)
		{
			if (orderType == OrderType.Ascending)
			{
				orderBy.Add(propertyName, "asc");
			}
			else
			{
				orderBy.Add(propertyName, "desc");
			}
		}

		public void AddDistinct(string propertyName)
		{
			if (AttributeSetName != "Member" && propertyName.ToLower() != "lwidentifier" && !propertyName.StartsWith("A_", StringComparison.OrdinalIgnoreCase))
			{
				propertyName = "A_" + propertyName;
			}
			distinct.Add(propertyName);
		}

		public List<string> GetDisticntColumns()
		{
			return distinct;
		}


		public string EvaluateToString()
		{
			string methodName = "EvaluateToString";
            using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                AttributeSetMetaData asd = service.GetAttributeSetMetaData(AttributeSetName);
                if (asd == null)
                {
                    Exception ex = new Exception("Cannot find defintion for " + AttributeSetName);
                    _logger.Error(_className, methodName, "Error loading global attribute set.", ex);
                    throw ex;
                }

                StringBuilder strBuff = new StringBuilder();
                foreach (CriteriaComponent comp in criteria)
                {
                    if (strBuff.Length != 0)
                    {
                        if (comp.Operator == OperatorType.AND)
                        {
                            strBuff.Append(" and ");
                        }
                        else
                        {
                            strBuff.Append(" or ");
                        }
                    }
                    AttributeMetaData ad = asd.GetAttribute(comp.Name);
                    if (ad == null)
                    {
                        Exception ex = new Exception(string.Format("{0} is not a valid attribute in attribute set {1}", comp.Name, AttributeSetName));
                        _logger.Error(_className, methodName, "Unable to evaluate criteria.", ex);
                        throw ex;
                    }
                    if (UseAlias)
                    {
                        if (AttributeSetName != "Member" && comp.Name.ToLower() != "lwidentifier")
                        {
                            strBuff.Append(Alias + ".A_" + comp.Name);
                        }
                        else
                        {
                            strBuff.Append(Alias + "." + comp.Name);
                        }
                    }
                    else
                    {
                        if (AttributeSetName != "Member" && comp.Name.ToLower() != "lwidentifier")
                        {
                            strBuff.Append("A_");
                        }
                        strBuff.Append(comp.Name);
                    }
                    strBuff.Append(GetSqlPredicate(comp.Predicate));
                    if (ad.DataType == "String")
                    {
                        strBuff.Append(string.Format("'{0}'", comp.Value));
                    }
                    else if (ad.DataType == "Date")
                    {
                        DateTime d = new DateTime();
                        // Conditions for needing a different date/time format: Database is Oracle and either the value is a DateTime or the value is a string that parses out to a DateTime
                        if (service.DatabaseType == SupportedDataSourceType.Oracle10g &&
                            (comp.Value is DateTime || (comp.Value is string && DateTime.TryParse(comp.Value.ToString(), out d))))
                        {
                            // If it is a DateTime, then set d
                            if (comp.Value is DateTime)
                            {
                                d = (DateTime)comp.Value;
                            }

                            // 31-DEC-09 11.17.45.0000 AM
                            string strDate = d.ToString("dd-MMM-y hh.mm.ss tt");
                            strBuff.Append(string.Format("'{0}'", strDate));
                        }
                        else
                        {
                            strBuff.Append(string.Format("'{0}'", comp.Value.ToString()));
                        }
                    }
                    else
                    {
                        strBuff.Append(comp.Value);
                    }
                }

                return strBuff.ToString();
            }
		}


		public EvaluatedCriterion Evaluate()
		{
			if (string.IsNullOrEmpty(AttributeSetName) || AttributeSetName == "Member")
			{
				return EvaluateMember();
			}
			else
			{
				return EvaluateAttributeSet();
			}
		}

		public string EvaluateOrderBy()
		{
			string orderByStr = string.Empty;
			if (orderBy.Count > 0)
			{
				foreach (string key in orderBy.Keys)
				{
					string column = key;
					if (AttributeSetName != "Member" && key.ToLower() != "lwidentifier" && !column.StartsWith("A_", StringComparison.OrdinalIgnoreCase))
					{
						column = "A_" + key;
					}
					if (!string.IsNullOrEmpty(orderByStr))
					{
						orderByStr += ", ";
					}
					else
					{
						orderByStr = "order by ";
					}
					if (UseAlias)
					{
						orderByStr += string.Format("{0}.{1} {2}", Alias, column, orderBy[key]);
					}
					else
					{
						orderByStr += string.Format("{0} {1}", column, orderBy[key]);
					}
				}
			}
			return orderByStr;
		}

		private EvaluatedCriterion EvaluateCriterion(Type objectType)
		{
			const string methodName = "EvaluateCriterion";
			EvaluatedCriterion result = new EvaluatedCriterion();
			StringBuilder strBuff = new StringBuilder();
			int parmNmbr = 0;
			foreach (CriteriaComponent comp in criteria)
			{
				if (strBuff.Length != 0)
				{
					if (comp.Operator == OperatorType.AND)
					{
						strBuff.Append(" and ");
					}
					else
					{
						strBuff.Append(" or ");
					}
				}

				PropertyInfo pInfo = objectType.GetProperty(comp.Name);

				if (pInfo == null)
				{
					Exception ex = new Exception(string.Format("{0} is not a valid attribute in attribute set {1}", comp.Name, AttributeSetName));
					_logger.Error(_className, methodName, "Unable to evaluate criteria.", ex);
					throw ex;
				}

                var columnIndex = pInfo.GetCustomAttribute(typeof(ColumnIndexAttribute)) as ColumnIndexAttribute;
                bool useLower = columnIndex != null && columnIndex.RequiresLowerFunction;

                if (useLower)
                    strBuff.Append("lower(");
                if (UseAlias)
                    strBuff.Append(Alias + ".");
                if (objectType != typeof(Member) && comp.Name.ToLower() != "lwidentifier")
                    strBuff.Append("A_");
                strBuff.Append(comp.Name);
                if (useLower)
                    strBuff.Append(")");

				string parmName = "lwp" + parmNmbr++;
				strBuff.Append(GetSqlPredicate(comp.Predicate));
				strBuff.Append(":" + parmName);
				if (comp.Predicate == Predicate.In)
				{
					strBuff.Append(")");
				}
				if (pInfo.PropertyType == typeof(string))
				{
					if (comp.Predicate == Predicate.In && comp.Value is string)
					{
                        string[] tokens = ((string)comp.Value).Trim().Split(',').Select(x => useLower ? x.ToLower() : x).ToArray();
						result.AddParameterList(parmName, tokens);
					}
					else
					{
						if (comp.Predicate == Predicate.In)
						{
                            if (useLower)
                                result.AddParameterList(parmName, ((IEnumerable<object>)comp.Value).Select(x => x.ToString().ToLower()));
                            else
                                result.AddParameterList(parmName, comp.Value);
						}
						else
						{
                            if (useLower)
                                result.AddParameter(parmName, comp.Value.ToString().ToLower());
                            else
                                result.AddParameter(parmName, comp.Value);
						}
					}
				}
				else if (pInfo.PropertyType == typeof(long))
				{
					if (comp.Predicate == Predicate.In && comp.Value is string)
					{
						string[] tokens = ((string)comp.Value).Trim().Split(',');
						object[] vals = new object[tokens.Length];
						for (int i = 0; i < tokens.Length; i++)
						{
							vals[i] = long.Parse(tokens[i]);
						}
						result.AddParameterList(parmName, vals);
					}
					else
					{
						if (comp.Predicate == Predicate.In)
						{
							// expecting an long[]
							long[] arr = (long[])comp.Value;
							object[] oarr = new object[arr.Length];
							arr.CopyTo(oarr, 0);
							result.AddParameterList(parmName, oarr);
						}
						else
						{
							result.AddParameter(parmName, comp.Value);
						}
					}
				}
				else if (pInfo.PropertyType == typeof(int))
				{
					if (comp.Predicate == Predicate.In && comp.Value is string)
					{
						string[] tokens = ((string)comp.Value).Trim().Split(',');
						object[] vals = new object[tokens.Length];
						for (int i = 0; i < tokens.Length; i++)
						{
							vals[i] = int.Parse(tokens[i]);
						}
						result.AddParameterList(parmName, vals);
					}
					else
					{
						if (comp.Predicate == Predicate.In)
						{
							// expecting an int[]
							int[] arr = (int[])comp.Value;
							object[] oarr = new object[arr.Length];
							arr.CopyTo(oarr, 0);
							result.AddParameterList(parmName, oarr);
						}
						else
						{
							result.AddParameter(parmName, comp.Value);
						}
					}
				}
				else if (pInfo.PropertyType == typeof(DateTime))
				{
					if (comp.Value is DateTime)
					{
						if (LWDataServiceUtil.GetServiceConfiguration().DatabaseType == SupportedDataSourceType.Oracle10g)
						{
							// 31-DEC-09 11.17.45.0000 AM
							//DateTime d = (DateTime)comp.Value;
							//string strDate = d.ToString("dd-MMM-y hh.mm.ss tt");                            
							//result.AddParameter(parmName, strDate);
							/*
							 * For some strang reason, this seeemd to have worked for a while.
							 * However, in case of Express, when this was executed to query txn headers,
							 * this resulted in a type cast error.  I tracked it down to the fact that hibernate
							 * was setting the bound query parameters for a datetime field and was expecting
							 * a value of type date but instead it got a string.  Thsi resulted in a typecast error during
							 * the binding of the query parameter.  I am not sure why did we ever start typecasting
							 * a date object to a string value.  Maybe it was a bug in a previous version of
							 * nhibernate.  Anyway, this change was originally put in Version 7 of LW 4.0 source tree
							 * in 3/2/2010.
							 * */
							result.AddParameter(parmName, comp.Value);
						}
						else
						{
							result.AddParameter(parmName, comp.Value);
						}
					}
					else if (comp.Value is string)
					{
						result.AddParameter(parmName, DateTimeUtil.ConvertStringToDate(string.Empty, comp.Value.ToString()));
					}
					else
					{
						//TODO: Exception
					}
				}
				else if (pInfo.PropertyType == typeof(Enum))
				{
					// what to do now.
					result.AddParameter(parmName, comp.Value.ToString());
				}
				else
				{
					//strBuff.Append(comp.Value);
					result.AddParameter(parmName, comp.Value);
				}
			}
			result.Where = strBuff.ToString();

			return result;
		}

		private EvaluatedCriterion EvaluateMember()
		{
			return EvaluateCriterion(typeof(Member));
		}

		private EvaluatedCriterion EvaluateAttributeSet()
		{
			string methodName = "Evaluate";
            using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                AttributeSetMetaData asd = service.GetAttributeSetMetaData(AttributeSetName);
                if (asd == null)
                {
                    Exception ex = new Exception("Cannot find defintion for " + AttributeSetName);
                    _logger.Error(_className, methodName, "Error loading global attribute set.", ex);
                    throw ex;
                }

                IClientDataObject cobj = DataServiceUtil.GetNewClientDataObject(AttributeSetName);
                return EvaluateCriterion(cobj.GetType());
            }
		}
	}

	internal class CriteriaComponent
	{
		public string Name { get; private set; }
		public object Value { get; private set; }
		public LWCriterion.Predicate Predicate { get; private set; }
		public LWCriterion.OperatorType Operator { get; private set; }

		public CriteriaComponent(string name, object value, LWCriterion.Predicate predicate, LWCriterion.OperatorType op)
		{
			Name = name;
			Value = value;
			Predicate = predicate;
			Operator = op;
		}
	}
}
