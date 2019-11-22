using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.CampaignManagement;
using CW=Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Returns the current value of the specified campaign attribute.
	/// </summary>
	/// <example>
	///     Usage : GetCampaignAttributeValue(['Campaign Name'],'Attribute Name')
	/// </example>
	/// <remarks>
	/// Campaign Name must be the name of an existing campaign.
	/// Step Name must be the name of an existing step in the campaign.
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the current value of the specified campaign attribute.",
		DisplayName = "GetCampaignAttributeValue",
		ExcludeContext = ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Campaign,
		ExpressionReturns = ExpressionApplications.Strings,

		WizardDescription = "Get a Campaign attribute value",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = false
		)]

	//Typically, we expect users to use this in the context of an executing/editing campaign, so there's no need to confuse them by asking for the campaign. 
	//[ExpressionParameter(Order = 0, Name = "Campaign Name", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which campaign?", Helpers = ParameterHelpers.CampaignName)]
	[ExpressionParameter(Order = 1, Name = "Attribute Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which attribute?", Helpers = ParameterHelpers.CampaignAttribute)]
	public class GetCampaignAttributeValue : UnaryOperation
	{
		private const string _className = "GetCampaignAttributeValue";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private Expression _campaignName = null;
		private Expression _attributeName = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetCampaignAttributeValue()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetCampaignAttributeValue(Expression rhs)
			: base("GetCampaignAttributeValue", rhs)
		{
			if (rhs == null)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to GetCampaignAttributeValue.");
			}

			if (rhs is StringConstant)
			{
				_attributeName = rhs;
				return;
			}

			ParameterList plist = rhs as ParameterList;

			if (plist.Expressions.Length == 0)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to GetCampaignAttributeValue.");
			}
			else if (plist.Expressions.Length == 1)
			{
				_attributeName = plist.Expressions[0];
			}
			else // > 1
			{
				_campaignName = plist.Expressions[0];
				_attributeName = plist.Expressions[1];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetCampaignAttributeValue(['Campaign Name'],'Attribute Name')";
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			try
			{
				string campaignName = null;
				bool campaignIsCurrent = false; //is the campaign the same as the camapaign held in the context object (the currently executing campaign)?
				string attributeName = null;
				string ret = null;

				if (_campaignName == null)
				{
					//no campaign name passed in expression, we need it from context object
					if (contextObject == null || contextObject.Environment == null || !contextObject.Environment.ContainsKey("CurrentCampaign"))
					{
						throw new Exception("Invalid function call. GetCampaignAttributeValue requires CampaignName either as a parameter or in the context.");
					}
					else
					{
						campaignName = contextObject.Environment["CurrentCampaign"].ToString();
						campaignIsCurrent = true;
					}
				}
				else
				{
					campaignName = (_campaignName.evaluate(contextObject) ?? string.Empty).ToString();

					//make sure campaign name is the same as the current executing campaign, else we'll have to 
					if (contextObject != null && contextObject.Environment != null && contextObject.Environment.ContainsKey("CurrentCampaign"))
					{
						campaignIsCurrent = contextObject.Environment["CurrentCampaign"].ToString().Equals(campaignName, StringComparison.OrdinalIgnoreCase);
					}
				}

				if (string.IsNullOrEmpty(campaignName))
				{
					throw new Exception("Campaign name evaluates to null or empty");
				}


				attributeName = (_attributeName.evaluate(contextObject) ?? string.Empty).ToString();
				if (string.IsNullOrEmpty(attributeName))
				{
					throw new Exception("Attribute name evaluates to null or empty");
				}


				if (
					campaignIsCurrent && 
					contextObject.Environment != null && 
					contextObject.Environment.ContainsKey("OverrideParameters") &&
					contextObject.Environment["OverrideParameters"] is Dictionary<string, string> &&
					((Dictionary<string, string>)contextObject.Environment["OverrideParameters"]).ContainsKey(attributeName)
					)
				{
					//get from overrideParameters
					ret = ((Dictionary<string, string>)contextObject.Environment["OverrideParameters"])[attributeName];
				}
				else
				{
					//The attribute has not been overridden in memory. Go to the database to retrieve the current attribute value
                    using (CampaignManager mgr = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        CW.Campaign campaign = mgr.GetCampaign(campaignName);
                        if (campaign == null)
                        {
                            throw new Exception(string.Format("Campaign {0} does not exist.", campaignName));
                        }

                        CW.Attribute attribute = mgr.GetAttribute(attributeName);
                        if (attribute == null)
                        {
                            throw new Exception(string.Format("the camapign attribute {0} does not exist.", attributeName));
                        }

                        CW.CampaignAttribute campaignAttribute = mgr.GetCampaignAttribute(campaign.Id, attribute.Id);
                        if (campaignAttribute != null)
                        {
                            ret = campaignAttribute.AttributeValue;
                        }
                    }
				}
				return ret;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Evaluate", string.Empty, ex);
				throw;
			}
		}

	}
}
