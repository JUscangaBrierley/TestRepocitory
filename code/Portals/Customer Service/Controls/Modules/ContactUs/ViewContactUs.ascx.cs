using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.FixedView;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Validators;
using System.Web.UI.HtmlControls;

namespace Brierley.LWModules.ContactUs
{
	public partial class ViewContactUs : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewContactUs";
		private const string _modulePath = "~/Controls/Modules/ContactUs/ViewContactUs.ascx";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private ContactUsConfig _config = null;
		private FixedLayoutManager _layoutManager;
		private Member _member = null;
		#endregion

		#region properties

		#endregion

		#region page life cycle
		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			try
			{
				InitializeConfig();

				_member = PortalState.CurrentMember;

				btnSend.Click += new EventHandler(btnSend_Click);
				btnSend.ValidationGroup = ValidationGroup;

				lblModuleTitle.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.ModuleTitleLabelResourceKey, "ModuleTitleLabel.Text"));
				lblMemberEmailAddress.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.MemberEmailAddressLabelResourceKey, "MemberEmailAddressLabel.Text"));
				lblEmailMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.EmailMessageLabelResourceKey, "EmailMessageLabel.Text"));
				lblCharsRemaining.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.EmailMessageCharsRemainingLabelResourceKey, "EmailMessageCharsRemainingLabel.Text"));
				cbCCMessage.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.CCMessageLabelResourceKey, "CCMessageLabel.Text"));
				cbCCMessage.Visible = _config.EnableCCMember;
				btnSend.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.SendButtonLabelResourceKey, "SendButtonLabel.Text"));
				lblStatus.Text = string.Empty;

				if (!string.IsNullOrEmpty(_config.ModuleTitleLabelCssClass)) lblModuleTitle.CssClass = _config.ModuleTitleLabelCssClass;
				if (!string.IsNullOrEmpty(_config.MemberEmailAddressLabelCssClass)) lblMemberEmailAddress.CssClass = _config.MemberEmailAddressLabelCssClass;
				if (!string.IsNullOrEmpty(_config.EmailMessageLabelCssClass)) lblEmailMessage.CssClass = _config.EmailMessageLabelCssClass;
				if (!string.IsNullOrEmpty(_config.EmailMessageCharsRemainingLabelCssClass))
				{
					lblCharsRemaining.CssClass = _config.EmailMessageCharsRemainingLabelCssClass;
					lblCharsRemainingValue.CssClass = _config.EmailMessageCharsRemainingLabelCssClass;
				}
				if (_config.EmailMessageTextAreaRows > 0) tbEmailMessage.Rows = _config.EmailMessageTextAreaRows;
				if (_config.EmailMessageTextAreaColumns > 0) tbEmailMessage.Columns = _config.EmailMessageTextAreaColumns;
				if (_config.EmailMessageTextAreaMaxLength > 0)
				{
					trCharsRemaining.Visible = true;
					lblCharsRemainingValue.Text = (_config.EmailMessageTextAreaMaxLength - tbEmailMessage.Text.Length).ToString();
					string script = string.Format("TextAreaMaxLength(this,{0})", _config.EmailMessageTextAreaMaxLength);
					tbEmailMessage.Attributes.Add("onkeyup", script);
					tbEmailMessage.Attributes.Add("onchange", script);
				}
				else
				{
					trCharsRemaining.Visible = false;
				}
				tbEmailMessage.Wrap = _config.EmailMessageTextAreaWrap;
				if (!string.IsNullOrEmpty(_config.CCMessageLabelCssClass)) cbCCMessage.CssClass = _config.CCMessageLabelCssClass;
				if (!string.IsNullOrEmpty(_config.SendButtonLabelCssClass)) btnSend.CssClass = _config.SendButtonLabelCssClass;

				BuildEmailSubject();

				if (!Page.IsPostBack)
				{
					if (_member != null && _config.PrepopulateMemberEmailAddress && !string.IsNullOrEmpty(_member.PrimaryEmailAddress))
					{
						tbMemberEmailAddress.Text = _member.PrimaryEmailAddress;
					}	
				}

                this.reqEmailAddress.ValidationGroup = ValidationGroup;
                this.reqMessage.ValidationGroup = ValidationGroup;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}
		#endregion

		#region event handlers
		void btnSend_Click(object sender, EventArgs e)
		{
			const string methodName = "btnSend_Click";
			try
			{
                if (_config.EmailRuleID != -1)
                {
                    RuleTrigger rule = LoyaltyService.GetRuleById(_config.EmailRuleID);
                    if (rule != null)
                    {
                        string emailSubjectFieldName = StringUtils.FriendlyString(_config.EmailSubjectField, "emailsubject");
                        string memberEmailAddressFieldName = StringUtils.FriendlyString(_config.MemberEmailAddressField, "memberemailaddress");
                        string emailContentFieldName = StringUtils.FriendlyString(_config.EmailContentField, "emailcontent");
                        string emailSubject = GetEmailSubject();

                        bool emailAddrValid = false;
                        try
                        {
                            // validate the email address
                            var addr = new System.Net.Mail.MailAddress(tbMemberEmailAddress.Text);
                            emailAddrValid = true;
                        }
                        catch (Exception)
                        {
                            base.AddInvalidField(ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.SendButtonLabelResourceKey, "InvalidEmailMessage.Text")), tbMemberEmailAddress);
                        }

                        ContextObject contextObject = new ContextObject();

                        string emailMessage = tbEmailMessage.Text;

                        emailMessage = emailMessage.Replace(System.Environment.NewLine, "<br/>");

                        if (emailAddrValid)
                        {
                            // send the contact us email                            
                            contextObject.Owner = _member;
                            if (!string.IsNullOrEmpty(_config.ContactEmailAddess))
                            {
                                contextObject.Environment.Add( EnvironmentKeys.RecipientEmail, _config.ContactEmailAddess);
                            }
                            contextObject.Environment.Add(emailSubjectFieldName, emailSubject);
                            contextObject.Environment.Add(memberEmailAddressFieldName, tbMemberEmailAddress.Text);
                            contextObject.Environment.Add(emailContentFieldName, emailMessage);
                            rule.Rule.Invoke(contextObject);
                            tbEmailMessage.Text = string.Empty;

                            lblStatus.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.StatusSuccessLabelResourceKey, "StatusSuccess.Text"));
                            lblStatus.CssClass = _config.StatusSuccessLabelCssClass;
                        }

                        if (emailAddrValid && _config.EnableCCMember && cbCCMessage.Checked && !string.IsNullOrEmpty(tbMemberEmailAddress.Text))
                        {
                            // send the CC to member email
                            contextObject = new ContextObject();
                            contextObject.Owner = _member;
                            contextObject.Environment.Add(EnvironmentKeys.RecipientEmail, tbMemberEmailAddress.Text);
                            contextObject.Environment.Add(emailSubjectFieldName, emailSubject);
                            contextObject.Environment.Add(memberEmailAddressFieldName, tbMemberEmailAddress.Text);
                            contextObject.Environment.Add(emailContentFieldName, emailMessage);
                            rule.Rule.Invoke(contextObject);
                        }

                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				lblStatus.Text = ResourceUtils.GetLocalWebResource(_modulePath, StringUtils.FriendlyString(_config.StatusErrorLabelResourceKey, "StatusError.Text"));
				lblStatus.CssClass = _config.StatusErrorLabelCssClass;
				throw;
			}
		}
		#endregion

		#region private methods
		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<ContactUsConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new ContactUsConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		private string GetEmailSubject()
		{
			string result = string.Empty;

			if (_config.EmailSubjectFieldsToShow != null && _config.EmailSubjectFieldsToShow.Count > 0)
			{
				foreach (ConfigurationItem attribute in _config.EmailSubjectFieldsToShow)
				{
					if (attribute.AttributeType == ItemTypes.Attribute)
					{
						if (!string.IsNullOrEmpty(result)) result += " ";
						result += GetControlValue(attribute);
					}
				}
			}

			if (string.IsNullOrEmpty(result)) result = "<no subject>";
			return result;
		}

		private void BuildEmailSubject()
		{
			_layoutManager = new FixedLayoutManager(LayoutTypes.ResponsiveSingleColumn, phEmailSubject);

			if (_config.EmailSubjectFieldsToShow != null && _config.EmailSubjectFieldsToShow.Count > 0)
			{
				foreach (ConfigurationItem attribute in _config.EmailSubjectFieldsToShow)
				{
					switch (attribute.AttributeType)
					{
						case ItemTypes.Attribute:
							string value = GetControlValue(attribute);
							AddAttributeItem(attribute, value);
							break;
						case ItemTypes.HtmlBlock:
							AddHtmlBlock(attribute);
							break;
						case ItemTypes.SwitchLayout:
							_layoutManager.SwitchLayoutMode(attribute.LayoutType);
							break;
					}
				}
			}
		}

		private void AddAttributeItem(ConfigurationItem item, string value)
		{
			if (!string.IsNullOrEmpty(item.DefaultValue) && string.IsNullOrEmpty(value))
			{
				value = ExpressionUtil.ParseExpressions(item.DefaultValue, new ContextObject() { Owner = _member });
			}

			WebControl control;
			switch (item.DisplayType)
			{
				case DisplayTypes.TextBox:
				case DisplayTypes.DatePicker:
					control = new TextBox();
					((TextBox)control).Text = value;
					if (item.MaxLength > -1)
					{
						((TextBox)control).MaxLength = item.MaxLength;
					}
					break;
				case DisplayTypes.CheckBox:
					control = new CheckBox();
					value = value.ToLower() == "true" || item.DefaultValue == "1" ? "true" : "false";
					((CheckBox)control).Text = GetLabelText(item);
					((CheckBox)control).Checked = value == "1" || value.ToLower() == "true";
					break;
				case DisplayTypes.DropDownList:
					control = new DropDownList();
					break;
				case DisplayTypes.RadioList:
					control = new RadioButtonList();
					((RadioButtonList)control).RepeatLayout = RepeatLayout.Flow;
					((RadioButtonList)control).RepeatDirection = RepeatDirection.Horizontal;
					((RadioButtonList)control).SelectedValue = value;
					break;
				case DisplayTypes.Label:
					control = new Label();
					((Label)control).Text = value;
					break;
				default:
					throw new Exception("Failed to determine display type for attribute " + item.AttributeName);
			}

			if (item.DisplayType == DisplayTypes.DropDownList || item.DisplayType == DisplayTypes.RadioList)
			{
				if (item.ListSource != null && item.ListSource.Count > 0)
				{
					foreach (ItemListSource source in item.ListSource)
					{
						((ListControl)control).Items.Add(new ListItem(source.Value, source.Key));
					}
				}

				switch (item.ListType)
				{
					case ListSourceType.GlobalAttributeSets:
						{
                            if (!string.IsNullOrEmpty(item.GlobalSetName) && !string.IsNullOrEmpty(item.GlobalColumnName))
                            {
                                var crit = new LWCriterion(item.GlobalSetName);
                                crit.AddOrderBy(item.GlobalColumnName, LWCriterion.OrderType.Ascending);

                                IList<IClientDataObject> set = LoyaltyService.GetAttributeSetObjects(null, item.GlobalSetName, crit, null, false);
                                if (set != null)
                                {
                                    foreach (var row in set)
                                    {
                                        var li = new ListItem(row.GetAttributeValue(item.GlobalColumnName).ToString(), row.RowKey.ToString());
                                        ((ListControl)control).Items.Add(li);
                                    }
                                }
                            }
						}
						break;

					case ListSourceType.FrameworkObject:
						{
							if (!string.IsNullOrEmpty(item.LWFrameworkObjectName) && !string.IsNullOrEmpty(item.LWFrameworkObjectProperty))
							{
								Dictionary<long, string> coll = LWObjectBinderUtil.GetObjectValues(item.LWFrameworkObjectName,
									item.LWFrameworkObjectProperty, item.LWFrameworkObjectWhereClause);
								foreach (long key in coll.Keys)
								{
									var li = new ListItem(coll[key], key.ToString());
									((ListControl)control).Items.Add(li);
								}
							}
						}
						break;
				}

				if (item.DisplayType == DisplayTypes.DropDownList)
				{
					if (!IsPostBack && !string.IsNullOrEmpty(item.DefaultValue) && string.IsNullOrEmpty(value))
					{
						DropDownList list = (DropDownList)control;
						foreach (ListItem li in list.Items)
						{
							if (li.Text == item.DefaultValue)
							{
								li.Selected = true;
								break;
							}
						}
					}
					else
					{
						((DropDownList)control).SelectedValue = value;
					}
				}
				else
				{
					((RadioButtonList)control).SelectedValue = value;
				}
			}

			control.ID = item.DataKey;
			control.CssClass = item.ControlCSSClass;

			var controlLabel = new HtmlGenericControl("label");
			controlLabel.InnerText = item.DisplayType == DisplayTypes.CheckBox ? "" : GetLabelText(item);
			controlLabel.Attributes.Add("class", item.LabelCssClass);

			List<BaseValidator> validators = new List<BaseValidator>();

			if (item.DisplayType != DisplayTypes.Label)
			{
				foreach (AttributeValidator validator in item.validators)
				{
					BaseValidator vldBase = null;

					switch (validator.ValidatorType)
					{
						case ValidatorTypes.Compare:
							CompareValidator vldCompare = new CompareValidator();
							vldCompare.ControlToCompare = validator.CompareToID;
							vldCompare.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
							vldBase = vldCompare;
							break;
						case ValidatorTypes.Range:
							RangeValidator vldRange = new RangeValidator();
							vldRange.MinimumValue = validator.MinValue;
							vldRange.MaximumValue = validator.MaxValue;
							vldRange.Type = validator.CompareType.GetValueOrDefault(ValidationDataType.String);
							vldBase = vldRange;
							break;
						case ValidatorTypes.RegularExpression:
							RegularExpressionValidator vldRegex = new RegularExpressionValidator();
							vldRegex.ValidationExpression = validator.RegularExpression;
							vldBase = vldRegex;
							break;
						case ValidatorTypes.RequiredField:
							RequiredFieldValidator vldReq = new RequiredFieldValidator();
							vldBase = vldReq;
							break;
						case ValidatorTypes.Custom:
							CustomValidator vldCustom = new CustomValidator();
							vldCustom.ClientValidationFunction = validator.ClientValidationFunction;
							vldCustom.ValidateEmptyText = true;
							vldBase = vldCustom;
							break;
					}

					vldBase.Display = ValidatorDisplay.Dynamic;
					vldBase.ControlToValidate = control.ID;
					vldBase.ValidationGroup = ValidationGroup;

					Label lblErrorMessage = new Label();
					lblErrorMessage.CssClass = validator.CssClass;
					lblErrorMessage.Text = validator.ErrorMessage;
					vldBase.Controls.Add(lblErrorMessage);

					validators.Add(vldBase);
				}
			}
			_layoutManager.AddItem(control, controlLabel, validators);
			item.Control = control;

			//no date fields exist for this module. The user may still select date picker, but it's probably
			//overkill to put the code in place to use date pickers if they won't be used
			//if (Attribute.DisplayType == DisplayTypes.DatePicker)
			//{
			//    _datePickers.Add(control);
			//}
		}

		private string GetMemberAttributeValue(ConfigurationItem item)
		{
			string val = null;
			try
			{
				switch (item.AttributeName)
				{
					case "Name Prefix":
						val = _member.NamePrefix;
						break;
					case "First Name":
						val = _member.FirstName;
						break;
					case "Middle Name":
						val = _member.MiddleName;
						break;
					case "Last Name":
						val = _member.LastName;
						break;
					case "Name Suffix":
						val = _member.NameSuffix;
						break;
					case "Email Address":
						val = _member.PrimaryEmailAddress;
						break;
					default:
                        AttributeSetMetaData meta = LoyaltyService.GetAttributeSetMetaData("MemberDetails");
						IList<IClientDataObject> details = null;
						IClientDataObject detail = null;
						if (meta != null)
						{
							details = _member.GetChildAttributeSets("MemberDetails");
							if (details != null && details.Count > 0)
							{
								detail = details[0];

								switch (item.AttributeName)
								{
									case "Address Line 1":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineOne"));
										}
										break;
									case "Address Line 2":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineTwo"));
										}
										break;
									case "Address Line 3":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineThree"));
										}
										break;
									case "Address Line 4":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("AddressLineFour"));
										}
										break;
									case "Country":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("Country"));
										}
										break;
									case "City":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("City"));
										}
										break;
									case "State":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("StateOrProvince"));
										}
										break;
									case "Zip Code":
										if (detail != null)
										{
											val = Convert.ToString(detail.GetAttributeValue("ZipOrPostalCode"));
										}
										break;
								}
							}
						}
						break;
				}

			}
			catch (LWMetaDataException ex)
			{
				//This could be thrown because the standard attribute set is not correctly defined, which doesn't completely 
				//kill the functionality of the module, so we'll log the exception and continue on without displaying it to the user.
				_logger.Error(_className, "GetMemberAttributeValue", "Unexpected exception: " + ex.Message, ex);
			}
			catch (Exception)
			{
				throw;
			}
			return val;
		}


		private string GetControlValue(ConfigurationItem item, bool getTextFromListItem = true)
		{
			string val = string.Empty;
			if (item.AttributeType == ItemTypes.Attribute)
			{
				switch (item.DisplayType)
				{
					case DisplayTypes.Label:
						var lbl = item.Control as Label;
						if (lbl != null)
						{
							val = lbl.Text;
						}
						break;
					case DisplayTypes.CheckBox:
						var chk = item.Control as CheckBox;
						if (chk != null)
						{
							val = chk.Checked.ToString();
						}
						break;
					case DisplayTypes.DatePicker:
					case DisplayTypes.TextBox:
						var txt = item.Control as TextBox;
						if (txt != null)
						{
							val = txt.Text;
						}
						break;
					case DisplayTypes.DropDownList:
					case DisplayTypes.RadioList:
						var lst = item.Control as ListControl;
						if (lst != null && lst.SelectedItem != null)
						{
							if (getTextFromListItem)
							{
								val = lst.SelectedItem.Text;
							}
							else
							{
								val = lst.SelectedItem.Value;
							}
						}
						break;
				}
			}
			return val;
		}


		private void AddHtmlBlock(ConfigurationItem item)
		{
			string displayText = string.Empty;

			if (!string.IsNullOrEmpty(item.ResourceKey))
			{
				displayText = ResourceUtils.GetLocalWebResource(_modulePath, item.ResourceKey);
			}
			else
			{
				displayText = item.DisplayText;
			}

			if (!string.IsNullOrEmpty(displayText))
			{
				displayText = ExpressionUtil.ParseExpressions(item.DisplayText, new ContextObject() { Owner = _member });
			}
			_layoutManager.AddHtmlBlock(item.DisplayText);
		}


		private string GetLabelText(ConfigurationItem item)
		{
			return ResourceUtils.GetLocalWebResource(_modulePath, item.ResourceKey, item.DisplayText);
		}
		#endregion
	}
}
