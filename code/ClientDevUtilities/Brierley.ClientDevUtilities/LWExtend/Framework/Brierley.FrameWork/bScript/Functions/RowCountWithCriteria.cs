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
	[ExpressionContext(Description = "Counts the number of rows that match the provided criteria.",
		DisplayName = "RowCountWithCriteria",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,
		EvalRequiresMember = true)]
	public class RowCountWithCriteria : UnaryOperation
	{
		private const string _className = "RowCountWithCriteria";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public RowCountWithCriteria() { }

		internal RowCountWithCriteria(Expression rhs)
			: base("RowCountWithCriteria", rhs)
		{
			ParameterList plist = rhs as ParameterList;
			if (plist.Expressions.Length != 2)
			{
				throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to RowCountWithCriteria.");
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "RowCountWithCriteria('AttributeSetName',bScriptExpression)";
			}
		}

		public override object evaluate(ContextObject contextObject)
		{
			string methodName = "evaluate";
			string errorMessage = string.Empty;

			long count = 0;

			try
			{
				Member member = ResolveMember(contextObject.Owner);				
				if (member != null)
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
						string whereClause = plist.Expressions[1].evaluate(contextObject).ToString();
						count = svc.CountAttributeSetObjects(contextObject.Owner, attSetMeta, whereClause);
					}
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
			return count;
		}
	}
}
