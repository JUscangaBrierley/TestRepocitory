using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;


namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The GetConfigValue function return the string value from the client configuration store that has the key
	/// value matching the value supplied by ConfigurationKeyName.
	/// </summary>
	/// <example>
	///     Usage : GetConfigValue('ConfigurationKeyName')
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the string value from the client configuration store that has the key value matching the value supplied by ConfigurationKeyName.",
		DisplayName = "GetConfigValue",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Numbers | ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Strings,
		WizardDescription = "Client configuration store value",
		AdvancedWizard = true
		)]

	[ExpressionParameter(Name = "Configuration key name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which configuration key?", Helpers = ParameterHelpers.ConfigurationKey)]
	public class GetConfigValue : UnaryOperation
	{
		private string _className = "GetConfigValue";
		LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetConfigValue()
		{

		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetConfigValue(Expression rhs)
			: base("GetConfigValue", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetConfigValue('ConfigurationKeyName')";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			string methodName = "evaluate";

			string keyName = this.GetRight().evaluate(contextObject).ToString();
			try
			{
				using (var service = LWDataServiceUtil.DataServiceInstance())
				{
					ClientConfiguration config = service.GetClientConfiguration(keyName);
					string value = config.Value;
					DateTime theDate = DateTimeUtil.MinValue;
					Decimal theValue = 0;

					if (DateTime.TryParseExact(value, Enum.GetNames(typeof(SupportedDateFormats)), System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out theDate))
					{
						return theDate;
					}
					if (Decimal.TryParse(value, out theValue))
					{
						return theValue;
					}
					return value;
				}
			}
			catch (Exception ex)
			{
				string msg = string.Format("Unable to get config value for {0}", keyName);
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
		}
	}
}
