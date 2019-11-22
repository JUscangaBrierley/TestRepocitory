using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	[Serializable]
	[ExpressionContext(Description = "Returns the value for the named expression wizard set.",
		DisplayName = "ExprWizSet",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings,
		WizardDescription = "Retrieve set",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function)]
	[ExpressionParameter(Order = 0, Name = "Set Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which set name?", Helpers = ParameterHelpers.ExprWizSet)]
	public class ExprWizSet : UnaryOperation
	{
		private const string _className = "ExprWizSet";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private Expression _setName = null;

        public ExprWizSet()
        {
        }

		internal ExprWizSet(Expression rhs)
			: base("ExprWizSet", rhs)
        {
			if (rhs == null)
            {
				throw new CRMException("Invalid Function Call: Missing set name argument for ExprWizSet().");
            }
            _setName = (StringConstant)rhs;
        }

        public new string Syntax
        {
            get
            {
				return "ExprWizSet('SetName')";
            }
        }
        
        public override object evaluate(ContextObject contextObject)
        {
            //string methodName = "evaluate";

			if (string.IsNullOrWhiteSpace(_setName.evaluate(contextObject).ToString()))
			{
				throw new CRMException("No set name passed to ExprWizSet()");
			}
            string key = "ExprWizSet:" + _setName.evaluate(contextObject).ToString();

			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				ClientConfiguration config = service.GetClientConfiguration(key);
				if (config == null)
				{
					throw new CRMException(string.Format("ExprWizSet('{0}'): Set named '{0}' does not exist.", _setName.evaluate(contextObject).ToString()));
				}
				return config.Value;
			}
        }
	}
}
