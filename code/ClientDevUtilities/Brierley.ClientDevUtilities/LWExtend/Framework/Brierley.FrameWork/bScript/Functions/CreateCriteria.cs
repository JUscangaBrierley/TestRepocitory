//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	[Serializable]
	[ExpressionContext(Description = "Create a criteria string that can be used with other bScripts expressions.",
		DisplayName = "CreateCriteria",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Strings)]
	public class CreateCriteria : UnaryOperation
	{
		private const string _className = "CreateCriteria";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public CreateCriteria() { }

		internal CreateCriteria(Expression rhs)
			: base("CreateCriteria", rhs)
		{
			ParameterList plist = rhs as ParameterList;
			if (plist.Expressions.Length < 4)
			{
				throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to CreateCriteria.");
			}
		}

		public new string Syntax
		{
			get
			{
				return "CreateCriteria('AttributeSetName','AttributeName','Predicate','AttributeValue','AND|OR',...)";
			}
		}

		public override object evaluate(ContextObject contextObject)
		{
			string methodName = "evaluate";
			string errorMessage = string.Empty;

			string critStr = string.Empty;

			try
			{
				using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					ParameterList plist = GetRight() as ParameterList;
					string attributeSetName = plist.Expressions[0].evaluate(contextObject).ToString();
					if (string.IsNullOrEmpty(attributeSetName))
					{
						throw new LWBScriptException("Attribute set name is empty.");
					}
					AttributeSetMetaData attSetMeta = svc.GetAttributeSetMetaData(attributeSetName);
					if (attSetMeta == null)
					{
						throw new LWBScriptException("No attribute set meta data could be found for " + attributeSetName);
					}
					LWCriterion crit = new Brierley.FrameWork.Data.LWCriterion(attributeSetName);
					for (int i = 1; i < plist.Expressions.Length; )
					{
						LWCriterion.OperatorType op = LWCriterion.OperatorType.AND;
						if (i != 1)
						{
							string opStr = plist.Expressions[i++].evaluate(contextObject).ToString().ToUpper();
							op = (LWCriterion.OperatorType)Enum.Parse(typeof(LWCriterion.OperatorType), opStr);
						}
						string attName = plist.Expressions[i++].evaluate(contextObject).ToString();
						if (string.IsNullOrEmpty(attName))
						{
							throw new LWBScriptException("Attribute name is empty.");
						}
						AttributeMetaData attMeta = attSetMeta.GetAttribute(attName);
						if (attMeta == null)
						{
							throw new LWBScriptException("No attribute meta data could be found for " + attName);
						}
						string predicate = StringUtils.CapitalizeWord(plist.Expressions[i++].evaluate(contextObject).ToString().Trim());
						LWCriterion.Predicate p = (LWCriterion.Predicate)Enum.Parse(typeof(LWCriterion.Predicate), predicate);
						object attValue = plist.Expressions[i++].evaluate(contextObject).ToString();
						if (attValue == null)
						{
							throw new LWBScriptException("Null value provided for attribuute " + attValue);
						}
						crit.Add(op, attName, attValue, p);
					}
					critStr = crit.EvaluateToString();
					_logger.Debug(_className, methodName, "Criteria: " + critStr);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error evaluating criteria:", ex);
				if (ex is LWBScriptException)
				{
					throw;
				}
				else
				{
					if (!string.IsNullOrEmpty(errorMessage))
					{
						throw new LWBScriptException(errorMessage, ex);
					}
					else
					{
						throw new LWBScriptException("Error evaluating criteria:", ex);
					}
				}
			}
			return critStr;
		}
	}
}
