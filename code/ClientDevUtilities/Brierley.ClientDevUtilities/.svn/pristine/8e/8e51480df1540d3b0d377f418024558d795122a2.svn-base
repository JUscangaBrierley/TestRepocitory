using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// GetListfileValue is used to instruct the email engine that it should use a value from a list file rather than try to evaluate 
	/// a member attribute to retrieve structured content data.
	/// </summary>
	/// <example>
	///     Usage : GetListFileValue('FieldName')
	/// </example>
	/// <remarks>
	/// In order to evaluate structured content data, a data filter may exist that filters the data based on member attributes. In the email
	/// system, not all clients have a loyalty member database, and instead the system may be completely list based. In this scenario, instructing
	/// the content system to filter based on a member attribute (i.e., AttrValue('MemberDetails', 'HomeCenterID', 0)) would not work. Instead, 
	/// GetListFileValue may be used to instruct the email system that a member attribute does not exist, but the value will be present in the 
	/// list file.
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Instructs the email engine to use a value from a list file rather than try to evaluate a member attribute to retrieve structured content data.", 
		DisplayName = "GetListFileValue", 
		ExcludeContext = ExpressionContexts.Survey | ExpressionContexts.Member | ExpressionContexts.Content, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Content, 
		ExpressionReturns = ExpressionApplications.Strings)]
	public class GetListFileValue : UnaryOperation
	{
		string _fieldName = "";

		/// <summary>
		/// Default Constructor
		/// </summary>
		public GetListFileValue()
		{
		}


		/// <summary>
		/// Constructor
		/// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public GetListFileValue(Expression rhs) : base("GetListFileValue", rhs)
		{
			if (!string.IsNullOrEmpty(rhs.ToString()))
			{
				_fieldName = rhs.ToString();
				return;
			}
			else
			{
				throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetListFileValue.");
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetListFileValue('FieldName')";
			}
		}


		/// <summary>
		/// Performs the operation defined by this function
		/// </summary>
		/// <param name="contextObject">An instance of ContextObject</param>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			return _fieldName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string parseMetaData()
		{
			return _fieldName;
		}
	}
}
