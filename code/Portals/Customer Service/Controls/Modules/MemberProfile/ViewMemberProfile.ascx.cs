using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile = Brierley.WebFrameWork.Controls.Mobile;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Exceptions.Authentication;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.FixedView;
using Brierley.WebFrameWork.Interceptors;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal.Security;
using Brierley.WebFrameWork.Portal.Validators;
using Brierley.FrameWork.Email;
using Brierley.FrameWork.Common.Config;
using Brierley.WebFrameWork.SocialNetwork;
using System.Web.UI.HtmlControls;


namespace Brierley.LWModules.MemberProfile
{
	public class NewTier
	{
		public string TierName { get; set; }
		public DateTime EnrollmentDate { get; set; }
		public string GlobalNote { get; set; }

		public NewTier(string tierName, DateTime enrollmentDate, string globalNote)
		{
			TierName = tierName;
			EnrollmentDate = enrollmentDate;
			GlobalNote = globalNote;
		}
	}

	public partial class ViewMemberProfile : ModuleControlBase, IIpcEventHandler
	{
		private const string _moduleName = "MemberProfile";
		private const string _className = "ViewMemberProfile";
		private const string _modulePath = "~/Controls/Modules/MemberProfile/ViewMemberProfile.ascx";

		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

		private FixedLayoutManager _layoutManager;
		private MemberProfileConfig _configuration = null;
		private Table _contentTable = null;
		private LayoutTypes _currentLayout;
		private bool _actionButtonsAdded = false;
		private ContextObject _context = new ContextObject();
		private Member _member = null;
		//private VirtualCard _vc = null;
		private NewTier _newTier = null;

		private List<Control> _datePickers = new List<Control>();
        //private bool _bUpdateAccountAllowed = true;

		private Button _btnResetPassword = new Button();
		private LinkButton _lnkResetPassword = new LinkButton();

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			const string methodName = "OnInit";
			if (!_IsUserControlPostBack)
			{
                litTitle.Text = StringUtils.FriendlyString(ResourceUtils.GetLocalWebResource(_modulePath, PortalState.CurrentPage.DisplayNameResourceKey), PortalState.CurrentPage.Name);
			}

			try
			{
				InitializeModuleConfiguration();
				if (_configuration != null)
				{
					if (_configuration.IsUpdateMode)
					{
						_member = PortalState.CurrentMember;
						if (_member != null)
						{
							_logger.Debug(_className, methodName, "Current loyalty member ipcode " + _member.IpCode.ToString());
						}
						else
						{
							_logger.Error(_className, methodName, "Failed to retrieve current logged in or selected loyalty member");
							//throw new Exception("Error retrieving loyalty member.");
							this.Visible = false;
							return;
						}
					}
					BuildPage();
				}
				else
				{
					this.Visible = false;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception", ex);
				throw;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			const string methodName = "Page_Load";
			_logger.Trace(_className, methodName, "Begin");

			if (_configuration != null)
			{
				plcContent.Visible = true;

				btnCancel.Click += new EventHandler(Cancel_Click);
				btnSubmitOne.Click += new EventHandler(SubmitOne_Click);
				btnSubmitTwo.Click += new EventHandler(SubmitTwo_Click);
				_btnResetPassword.Click += ResetPassword_Click;

				lnkCancel.Click += new EventHandler(Cancel_Click);
				lnkSubmitOne.Click += new EventHandler(SubmitOne_Click);
				lnkSubmitTwo.Click += new EventHandler(SubmitTwo_Click);
				_lnkResetPassword.Click += ResetPassword_Click;

				lnkSubmitOne.ValidationGroup = ValidationGroup;
				lnkSubmitTwo.ValidationGroup = ValidationGroup;
				btnSubmitOne.ValidationGroup = ValidationGroup;
				btnSubmitTwo.ValidationGroup = ValidationGroup;

				IpcManager.RegisterEventHandler("MemberSelected", this, false);
				IpcManager.RegisterEventHandler("MemberUpdated", this, false);
			}
			_logger.Trace(_className, methodName, "End");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Func<string, string> javascriptEscape = delegate(string s)
			{
				if (string.IsNullOrEmpty(s))
				{
					return string.Empty;
				}
				return s.Replace("'", "\\'");
			};

			//create client side scripts for parent/child list controls
			if (_configuration != null && _configuration.Attributes != null)
			{
				var relations =
					from p in _configuration.Attributes
					join c in _configuration.Attributes on p.DataKey equals c.ParentId
					where
						//p.ListSource != null &&
						//c.ListSource != null &&
						//p.ListSource.Count > 0 &&
						//c.ListSource.Count > 0 &&
						p.Control != null &&
						c.Control != null &&
						p.Control is ListControl &&
						c.Control is ListControl
					select new { ParentAttribute = p, ChildAttribute = c };

				if (relations.Count() > 0)
				{
					bool first = true;
					var sb = new StringBuilder("<script type=\"text/javascript\">\r\n");

					sb.Append("_parentLists = [");
					var distinctParents = (from r in relations select new { id = r.ParentAttribute.Control.ClientID }).Distinct();
					foreach (var parent in distinctParents)
					{
						if (!first)
						{
							sb.Append(", ");
						}
						sb.AppendFormat("'{0}'", parent.id);
						first = false;
					}
					sb.AppendLine("];");

					sb.AppendLine("_relations = [");
					first = true;
					foreach (var relation in relations)
					{
						if (!first)
						{
							sb.Append(", ");
						}
						sb.AppendLine();
						sb.AppendFormat("new Relation('{0}', '{1}', [", relation.ParentAttribute.Control.ClientID, relation.ChildAttribute.Control.ClientID);

						bool firstItem = true;
						List<ItemListSource> items = relation.ChildAttribute.ListSource ?? new List<ItemListSource>();
						if (!string.IsNullOrEmpty(relation.ChildAttribute.GlobalSetName) && !string.IsNullOrEmpty(relation.ChildAttribute.GlobalColumnName))
						{
							//add global set values to list
							var crit = new LWCriterion(relation.ChildAttribute.GlobalSetName);
							crit.AddOrderBy(relation.ChildAttribute.GlobalColumnName, LWCriterion.OrderType.Ascending);
							IList<IClientDataObject> set = LoyaltyService.GetAttributeSetObjects(null, relation.ChildAttribute.GlobalSetName, crit, null, false);
							if (set != null)
							{
								foreach (var row in set)
								{
									items.Add(new ItemListSource(row.RowKey.ToString(), row.GetAttributeValue(relation.ChildAttribute.GlobalColumnName).ToString(), row.ParentRowKey.ToString()));
								}
							}


						}
						foreach (var li in relation.ChildAttribute.ListSource)
						{
							if (!firstItem)
							{
								sb.Append(", ");
							}
							sb.AppendFormat("new Item('{0}', '{1}', '{2}')", javascriptEscape(li.ParentKey), javascriptEscape(li.Key), javascriptEscape(li.Value));
							firstItem = false;
						}
						sb.Append("])");

						first = false;
					}
					sb.AppendLine("];");

					sb.AppendLine("</script>");
					Page.ClientScript.RegisterStartupScript(this.GetType(), "ParentChildLists", sb.ToString());
				}
				
				if (_datePickers.Count > 0)
				{
					var sb = new StringBuilder("<script type=\"text/javascript\">\r\n$(document).ready(function() { ");

					sb.Append("var datePickers = [");
					for (int i=0; i < _datePickers.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						sb.AppendFormat("'#{0}'", _datePickers[i].ClientID);
					}
					sb.AppendLine("];");

					sb.AppendLine(@"
					for(var i=0, len=datePickers.length; i < len; i++) {
						$(datePickers[i]).datepicker();
					}");
					sb.AppendLine("});");
					sb.AppendLine("</script>");
					Page.ClientScript.RegisterStartupScript(this.GetType(), "DatePickers", sb.ToString());
				}
			}
		}


		protected override bool ControlRequiresJQueryUI()
		{
			if (_configuration != null)
			{
				foreach (var att in _configuration.Attributes)
				{
					if (att.DisplayType == DisplayTypes.DatePicker)
					{
						return true;
					}
				}
			}
			return false;
		}


		void SubmitOne_Click(object sender, EventArgs e)
		{
			try
			{
				if (SaveProfile())
				{
					if (!string.IsNullOrEmpty(_configuration.FormActions[ActionButtons.SubmitOne].RedirectUrl))
					{
						RedirectUser(_configuration.FormActions[ActionButtons.SubmitOne].RedirectUrl);
					}
				}
			}
            catch (LWException lwEx) //To do: THis needs to be changed to only catch LWValidationException messages
            {
                lblError.Text = lwEx.Message;
            }
			catch (Exception ex)
			{
				_logger.Error(_className, "SubmitOne_Click", "Unexpected exception", ex);
				throw;
			}
		}


		void SubmitTwo_Click(object sender, EventArgs e)
		{
			try
			{
				if (SaveProfile())
				{
					if (!string.IsNullOrEmpty(_configuration.FormActions[ActionButtons.SubmitTwo].RedirectUrl))
					{
						RedirectUser(_configuration.FormActions[ActionButtons.SubmitTwo].RedirectUrl);
					}
				}
			}
            catch (LWException lwEx) //To do: THis needs to be changed to only catch LWValidationException messages
            {
                lblError.Text = lwEx.Message;
            }
			catch (Exception ex)
			{
				_logger.Error(_className, "SubmitTwo_Click", "Unexpected exception", ex);
				throw;
			}
		}


		void Cancel_Click(object sender, EventArgs e)
		{
			try
			{
				InitializeModuleConfiguration();
				if (!string.IsNullOrEmpty(_configuration.FormActions[ActionButtons.Cancel].RedirectUrl))
				{
					RedirectUser(_configuration.FormActions[ActionButtons.Cancel].RedirectUrl);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Cancel_Click", "Unexpected exception", ex);
				throw;
			}
		}

		void ResetPassword_Click(object sender, EventArgs e)
		{
			const string methodName = "ResetPassword_Click";

			if (PortalState.Portal.PortalMode != PortalModes.CustomerService)
			{
                string msg = ResourceUtils.GetLocalWebResource(_modulePath, "OnlyCSPortalMessage.Text", "This functionality is only valid in a customer service portal");
                _logger.Error(_className, methodName, "This functionality is only valid in a customer service portal");
				throw new Exception(msg);
			}

			Member member = PortalState.CurrentMember;
			if (member == null)
			{
				string msg = ResourceUtils.GetLocalWebResource(_modulePath, "NoCurrentMember.Text", "No member is currently selected");
                _logger.Error(_className, methodName, "No member is currently selected");
				throw new Exception(msg);
			}

			try
			{
				ConfigurationItem resetPasswordAttribute = null;
				foreach (ConfigurationItem attribute in _configuration.Attributes)
				{
					if (attribute.AttributeType == ItemTypes.ResetPasswordButton)
					{
						resetPasswordAttribute = attribute;
						break;
					}
				}

				// send the email
                if (resetPasswordAttribute.ResetPasswordEmailID > 0)
                {
                    // generate member's single-use code
                    string resetCode = LoyaltyService.GenerateMemberResetCode(member, (int)resetPasswordAttribute.ResetPasswordSUIDExpiryMinutes);
                    Dictionary<string, string> emailFields = new Dictionary<string, string>();

					using (var email = TriggeredEmailFactory.Create(resetPasswordAttribute.ResetPasswordEmailID))
                    {
						email.SendAsync(member, emailFields).Wait();
                    }
                    ShowPositive(ResourceUtils.GetLocalWebResource(_modulePath, "ResetPasswordSuccess.Text", "Reset password email was sent successfully."));
                }
                else
                {
                    string msg = ResourceUtils.GetLocalWebResource(_modulePath, "NoEmailConfigured.Text", "Reset password email not sent. No email is configured.");
                    _logger.Error(_className, methodName, "Reset password email not sent. No email is configured.");
                    ShowNegative(msg);
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void RedirectUser(string url)
		{
			_logger.Debug(_className, "RedirectUser", "Redirecting user to " + url);
			Response.Redirect(url, false);
		}


		public ModuleConfigurationKey GetConfigurationKey()
		{
			return base.ConfigurationKey;
		}


		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			if (
				info.PublishingModule != base.ConfigurationKey &&
				(info.EventName == "MemberSelected" || info.EventName == "MemberUpdated") &&
				_configuration != null &&
				!_configuration.IgnoreMemberEvents)
			{
				_member = PortalState.CurrentMember;
				if (_member != null)
				{
					_logger.Debug(_className, methodName, "Retrieved member from cache having ipcode " + _member.IpCode.ToString());
				}
				InitializeModuleConfiguration();
				BuildPage();
			}

			_logger.Trace(_className, methodName, "End");
		}


		bool SaveProfile()
		{
			const string methodName = "SaveProfile";
			List<string> notes = new List<string>();
			IMemberSaveInterceptor<MemberProfileConfig> callback = null;
			_logger.Trace(_className, methodName, "Begin");

			Page.Validate(ValidationGroup);
			if (!Page.IsValid)
			{
				_logger.Debug(_className, methodName, "Page did not validate. Returning.");
				return false;
			}

			if (_configuration.IsUpdateMode && _member == null)
			{
				_logger.Error(_className, methodName, "Cannot update profile. No member has been set.");
				throw new Exception(ResourceUtils.GetLocalWebResource(_modulePath, "UpdateProfileFailed.Text", "Cannot update profile. No member has been set."));
			}
			if (!_configuration.IsUpdateMode && _member == null)
			{
				_member = new Member();
			}


			string refError = string.Empty;
			if (!string.IsNullOrEmpty(_configuration.PrePopulateCallbackAssembly) && !string.IsNullOrEmpty(_configuration.PrePopulateCallbackClass))
			{
				System.Reflection.Assembly assembly = Brierley.FrameWork.Common.ClassLoaderUtil.LoadAssembly(_configuration.PrePopulateCallbackAssembly);
				if (assembly == null)
				{
                    string msg = ResourceUtils.GetLocalWebResource(_modulePath, "VCAssemblyFailed.Text", "Failed to load assembly for Virtual Card validation:") + " ";
                    _logger.Error(_className, methodName, "Failed to load assembly for Virtual Card validation: " + _configuration.PrePopulateCallbackAssembly);
					throw new Exception(msg + _configuration.PrePopulateCallbackAssembly);
				}
				callback = (IMemberSaveInterceptor<MemberProfileConfig>)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(assembly, _configuration.PrePopulateCallbackClass);
				if (callback == null)
				{
                    string msg = ResourceUtils.GetLocalWebResource(_modulePath, "AssemblyLoadFailed.Text", "Failed to load instance of") + " ";
                    _logger.Error(_className, methodName, "Failed to load instance of " + assembly + ", " + _configuration.PrePopulateCallbackClass);
					throw new Exception(msg + assembly + ", " + _configuration.PrePopulateCallbackClass);
				}
				Dictionary<string, ConfigurationItem> retVal = callback.BeforePopulate(_member, _configuration);

				if (retVal != null && retVal.Count > 0)
				{
					AddInterceptorErrors(retVal);
					return false;
				}
			}

			_logger.Trace(_className, methodName, "Setting member attributes");

			string unhashedPassword = string.Empty;
			foreach (ConfigurationItem attribute in _configuration.Attributes)
			{
				if ((attribute.AttributeType != ItemTypes.Attribute && attribute.AttributeType != ItemTypes.AccountStatus && attribute.AttributeType != ItemTypes.Tier) || attribute.DisplayType == DisplayTypes.Label)
				{
					continue;
				}

				if (!string.IsNullOrEmpty(attribute.GlobalNoteFormat) && PortalState.Portal.PortalMode == PortalModes.CustomerService && attribute.AttributeType != ItemTypes.Tier)
				{
					string note = GetNoteForAttribute(attribute);
					if (!string.IsNullOrEmpty(note))
					{
						notes.Add(note);
					}
				}

				if (attribute.AttributeSetID == -1)
				{
					if (attribute.AttributeName == "PrimaryEmailAddress")
					{
						//string postbackValue = GetPostbackValue(attribute).Trim().ToLower();
                        string postbackValue = GetPostbackValue(attribute).Trim();
						//if (!string.IsNullOrEmpty(postbackValue) && (!_configuration.IsUpdateMode || ((_member.PrimaryEmailAddress ?? string.Empty).Trim().ToLower() != postbackValue)))
                        if (!string.IsNullOrEmpty(postbackValue) && (!_configuration.IsUpdateMode || ((_member.PrimaryEmailAddress ?? string.Empty).Trim() != postbackValue)))
						{
							try
							{
								Member member = LoyaltyService.LoadMemberFromEmailAddress(GetPostbackValue(attribute)) as Member;
								if (member != null)
								{
									AddInvalidField(GetLocalResourceObject("EmailConstraintViolation.Text").ToString(), attribute.Control);
									return false;
								}
							}
							catch (Exception ex)
							{
								_logger.Debug(_className, methodName, "Error checking for duplicate email address. This is non-fatal: " + ex.Message);
							}
						}
					}
					if (attribute.AttributeName == "Username")
					{
						string postbackValue = GetPostbackValue(attribute).Trim().ToLower();
						if (!string.IsNullOrEmpty(postbackValue) && (!_configuration.IsUpdateMode || ((_member.Username ?? string.Empty).Trim().ToLower() != postbackValue)))
						{
							try
							{
								Member member = LoyaltyService.LoadMemberFromUserName(GetPostbackValue(attribute)) as Member;
								if (member != null)
								{
									AddInvalidField(GetLocalResourceObject("UsernameConstraintViolation.Text").ToString(), attribute.Control);
									return false;
								}
							}
							catch (Exception ex)
							{
								_logger.Debug(_className, methodName, "Error checking for duplicate username. This is non-fatal: " + ex.Message);
							}
						}
					}

					if (attribute.IsVirtualCard)
					{
						//if (_vc == null)
						//{
						//    _vc = new VirtualCard(); // _member.CreateNewVirtualCard() as VirtualCard;
						//}
						if (attribute.AttributeName == "Loyalty ID Number" &&
							attribute.VirtualCardConfiguration != null &&
							!string.IsNullOrEmpty(attribute.VirtualCardConfiguration.ValidationAssembly) &&
							!string.IsNullOrEmpty(attribute.VirtualCardConfiguration.ValidationClass))
						{
							try
							{
								int cardType = -1;                                
								if (!attribute.VirtualCardConfiguration.UserProvidesCardType)
								{
									cardType = attribute.VirtualCardConfiguration.DefaultCardType;
								}
								else
								{
									cardType = GetCardType();
								}
                                DateTime? expirationDate = GetExpirationDate();

								// check to see that Loyalty Id has a value
								string loyaltyIdNumber = GetPostbackValue(attribute);
								if (string.IsNullOrEmpty(loyaltyIdNumber))
								{
									continue;
								}
                                if (loyaltyIdNumber.Length > 255)
                                {
                                    AddInvalidField(GetLocalResourceObject("LoyaltyIdConstraintViolation.Text").ToString(), attribute.Control);
                                    return false;
                                }

								// make sure that another member does not already have this card
								try
								{
									Member member = LoyaltyService.LoadMemberFromLoyaltyID(loyaltyIdNumber);
									if (member != null)
									{
										AddInvalidField(GetLocalResourceObject("LoyaltyIdConstraintViolation.Text").ToString(), attribute.Control);
										return false;
									}
								}
								catch (Exception ex)
								{
									_logger.Debug(_className, methodName, "Error checking for duplicate loyalty id. This is non-fatal: " + ex.Message);
								}

								// make sure that the member does not already have the loyalty card
								VirtualCard vc = _member.GetLoyaltyCard(loyaltyIdNumber);
								if (vc != null)
								{
									// the card already exists
									string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "LoyaltyCardExistsMember.Text", "Loyalty card id {0} already exists on the member."), loyaltyIdNumber);
									_logger.Error(_className, methodName, errMsg);
                                    throw new LWValidationException(errMsg);
								}

                                vc = _member.CreateNewVirtualCard(expirationDate);
                                vc.LoyaltyIdNumber = loyaltyIdNumber;
                                vc.CardType = cardType;
                                vc.IsPrimary = false;

								string assemblyName = attribute.VirtualCardConfiguration.ValidationAssembly;
								string typeName = attribute.VirtualCardConfiguration.ValidationClass;

								ILoyaltyCardsInterceptor interceptor = (ILoyaltyCardsInterceptor)ClassLoaderUtil.CreateInstance(assemblyName, typeName);

								//IVirtualCardValidator validator = null;
								//System.Reflection.Assembly assembly = Brierley.FrameWork.Common.ClassLoaderUtil.LoadAssembly(assemblyName);
								//if (assembly == null)
								//{
								//    _logger.Error(_className, methodName, "Failed to load assembly for Virtual Card validation: " + assembly);
								//    throw new Exception("Failed to load assembly for Virtual Card validation: " + assembly);
								//}
								//validator = (IVirtualCardValidator)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(assembly, typeName);
								//if (validator == null)
								//{
								//    _logger.Error(_className, methodName, "Failed to load instance of " + assembly + ", " + typeName);
								//    throw new Exception("Failed to load instance of " + assembly + ", " + typeName);
								//}

								try
								{
									interceptor.Validate(_member, GetPostbackValue(attribute), cardType, expirationDate);
									//vcList.Add(vc); // Why are we adding to the vcList here if we add to it again in SetMemberAttributeValue?
								}
								catch (LWValidationException ex)
								{
									string message = string.IsNullOrEmpty(ex.Message) ? ResourceUtils.GetLocalWebResource(_modulePath, "VCValidationFailedLocatingResource.Text", "Virtual Card validation failed. Unable to locate resource entry for message.") : ex.Message;
									AddInvalidField(message, attribute.Control);
									return false;
								}
							}
							catch (Exception ex)
							{
                                string msg = ResourceUtils.GetLocalWebResource(_modulePath, "VCValidationFailed.Text", "Virtual card validation failed.");
                                _logger.Error(_className, methodName, "Virtual card validation failed.", ex);
								throw new Exception(msg, ex);
							}
						}
					}
				}
				SetMemberAttributeValue(attribute);

				if (attribute.AttributeName == "Password")
				{
                    unhashedPassword = GetPostbackValue(attribute);
                    LoyaltyService.ChangeMemberPassword(_member, unhashedPassword, false);

					if (_configuration.PasswordChangedEmailId != 0)
					{
						try
						{
							// send password changed email
							using (var email = TriggeredEmailFactory.Create(_configuration.PasswordChangedEmailId))
							{
								email.SendAsync(_member).Wait();
								_logger.Debug(_className, methodName, "Password change email was sent to: " + _member.PrimaryEmailAddress);
							}
						}
						catch (Exception ex)
						{
							_logger.Error(_className, methodName, "Error sending password changed email:" + ex.Message, ex);
						}
					}
					else
					{
						_logger.Debug(_className, methodName, "No password change email is configured.");
					}
				}
			}

			_logger.Debug(_className, methodName, "Saving loyalty member " + _member.IpCode.ToString());

			if (!_configuration.IsUpdateMode)
			{
				_member.MemberCreateDate = DateTime.Now;
			}

			if (!string.IsNullOrEmpty(_configuration.PreSaveCallbackAssembly) && !string.IsNullOrEmpty(_configuration.PreSaveCallbackClass))
			{
				//We only need to load the assembly if it is currently null or if the developer has defined a separate class/assembly for PreSave.
				//If the same assembly is defined, then we'll hold on to the instance that we already have loaded. This will allow the developer
				//to hold references to any data collected during the previous call (and save some processing time).
				if (callback == null || _configuration.PrePopulateCallbackClass != _configuration.PreSaveCallbackClass || _configuration.PrePopulateCallbackAssembly != _configuration.PreSaveCallbackAssembly)
				{
					System.Reflection.Assembly assembly = Brierley.FrameWork.Common.ClassLoaderUtil.LoadAssembly(_configuration.PreSaveCallbackAssembly);
					if (assembly == null)
					{
                        string msg = ResourceUtils.GetLocalWebResource(_modulePath, "VCAssemblyFailed.Text", "Failed to load assembly for Virtual Card validation:") + " ";
                        _logger.Error(_className, methodName, "Failed to load assembly for Virtual Card validation: " + _configuration.PreSaveCallbackAssembly);
						throw new Exception(msg + _configuration.PreSaveCallbackAssembly);
					}
					callback = (IMemberSaveInterceptor<MemberProfileConfig>)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(assembly, _configuration.PreSaveCallbackClass);
					if (callback == null)
					{
                        string msg = ResourceUtils.GetLocalWebResource(_modulePath, "AssemblyLoadFailed.Text", "Failed to load instance of") + " ";
                        _logger.Error(_className, methodName, "Failed to load instance of " + assembly + ", " + _configuration.PreSaveCallbackClass);
						throw new Exception(msg + assembly + ", " + _configuration.PreSaveCallbackClass);
					}
				}

				var retVal = callback.BeforeSave(_member, _configuration);

				if (retVal != null && retVal.Count > 0)
				{
					//callback failed. display error
					AddInterceptorErrors(retVal);
					return false;
				}
			}

			LoyaltyService.SaveMember(_member);

			LinkSocial();

			if (_newTier != null)
			{
				_member.AddTier(_newTier.TierName, _newTier.EnrollmentDate, null, _newTier.GlobalNote);
				if (!string.IsNullOrEmpty(_newTier.GlobalNote) && PortalState.Portal.PortalMode == PortalModes.CustomerService)
				{
					notes.Add(_newTier.GlobalNote);
				}
			}

			if (notes.Count > 0)
			{
				var agent = PortalState.GetLoggedInCSAgent();
				foreach (var note in notes)
				{
					var csNote = new CSNote();
					csNote.Note = note;
					csNote.MemberId = _member.IpCode;
					csNote.CreatedBy = agent.Id;
					CSService.CreateNote(csNote);
				}
			}

			if (!string.IsNullOrEmpty(_configuration.PostSaveCallbackAssembly) && !string.IsNullOrEmpty(_configuration.PostSaveCallbackClass))
			{
				//We only need to load the assembly if it is currently null or if the developer has defined a separate class/assembly for PreSave.
				//If the same assembly is defined, then we'll hold on to the instance that we already have loaded. This will allow the developer
				//to hold references to any data collected during the previous call (and save some processing time).
				bool needCallbackInstance = false;

				if (callback == null)
				{
					needCallbackInstance = true;
				}
				else if (!string.IsNullOrEmpty(_configuration.PreSaveCallbackAssembly) && !string.IsNullOrEmpty(_configuration.PreSaveCallbackClass))
				{
					if (_configuration.PreSaveCallbackAssembly != _configuration.PostSaveCallbackAssembly || _configuration.PreSaveCallbackClass != _configuration.PostSaveCallbackClass)
					{
						needCallbackInstance = true;
					}
				}
				else if (!string.IsNullOrEmpty(_configuration.PrePopulateCallbackAssembly) && !string.IsNullOrEmpty(_configuration.PrePopulateCallbackClass))
				{
					if (_configuration.PrePopulateCallbackAssembly != _configuration.PostSaveCallbackAssembly || _configuration.PrePopulateCallbackClass != _configuration.PostSaveCallbackClass)
					{
						needCallbackInstance = true;
					}
				}

				if (needCallbackInstance)
				{
					System.Reflection.Assembly assembly = Brierley.FrameWork.Common.ClassLoaderUtil.LoadAssembly(_configuration.PostSaveCallbackAssembly);
					if (assembly == null)
					{
                        string msg = ResourceUtils.GetLocalWebResource(_modulePath, "VCAssemblyFailed.Text", "Failed to load assembly for Virtual Card validation:") + " ";
                        _logger.Error(_className, methodName, "Failed to load assembly for Virtual Card validation: " + _configuration.PostSaveCallbackAssembly);
						throw new Exception(msg + _configuration.PostSaveCallbackAssembly);
					}
					callback = (IMemberSaveInterceptor<MemberProfileConfig>)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(assembly, _configuration.PostSaveCallbackClass);
					if (callback == null)
					{
                        string msg = ResourceUtils.GetLocalWebResource(_modulePath, "AssemblyLoadFailed.Text", "Failed to load instance of") + " ";
                        _logger.Error(_className, methodName, "Failed to load instance of " + assembly + ", " + _configuration.PostSaveCallbackClass);
						throw new Exception(msg + assembly + ", " + _configuration.PostSaveCallbackClass);
					}
				}

				callback.AfterSave(_member, _configuration, ref refError);
			}
			
            if (_configuration.UserEventId != 0)
            {
                LWEvent lwevent = LoyaltyService.GetLWEvent(_configuration.UserEventId);
                _logger.Trace(_className, methodName, "Executing business rules in user defined event " + lwevent.Name);
                ContextObject context = new ContextObject() { Owner = _member };
                LoyaltyService.ExecuteEventRules(context, lwevent.Name, RuleInvocationType.Manual);
            }

			ShowPositive(GetLocalResourceObject(_configuration.IsUpdateMode ? "UpdateComplete.Text" : "RegistrationComplete.Text").ToString());
			//plcContent.Visible = false;

			if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing && _configuration.IsUpdateMode == false && _configuration.LoginOnSave /*&& string.IsNullOrEmpty(UserName)*/)
			{
				try
				{
					LoginStatusEnum loginStatus = LoginStatusEnum.Failure;
					Member member = null;
                    switch (PortalState.Portal.AuthenticationField)
                    {
                        case AuthenticationFields.Username:
                            if (!string.IsNullOrEmpty(_member.Username))
					        {
						        member = LoyaltyService.LoginMember(AuthenticationFields.Username, _member.Username, unhashedPassword, null, ref loginStatus);
					        }
                            break;
                        case AuthenticationFields.PrimaryEmailAddress:
                            if (!string.IsNullOrEmpty(_member.PrimaryEmailAddress))
					        {
						        member = LoyaltyService.LoginMember(AuthenticationFields.PrimaryEmailAddress, _member.PrimaryEmailAddress, unhashedPassword, null, ref loginStatus);
					        }
                            break;
                        case AuthenticationFields.AlternateId:
                            if (!string.IsNullOrEmpty(_member.AlternateId))
					        {
						        member = LoyaltyService.LoginMember(AuthenticationFields.AlternateId, _member.AlternateId, unhashedPassword, null, ref loginStatus);
					        }
                            break;
                        case AuthenticationFields.LoyaltyIdNumber:
                            var card = _member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
						    if (card != null)
						    {
							    member = LoyaltyService.LoginMember(AuthenticationFields.LoyaltyIdNumber, card.LoyaltyIdNumber, unhashedPassword, null, ref loginStatus);
						    }
                            break;
                    }
                    if (member != null && loginStatus == LoginStatusEnum.Success)
                    {
                        SecurityManager.LoginMember(_member, false);
                    }
					
				}
				catch(Exception ex)
				{
					_logger.Error(_className, methodName, "Unable to log in member: " + ex.Message, ex);
				}
			}

			if (PortalState.Portal.PortalMode == PortalModes.CustomerService)
			{
				PortalState.CurrentMember = _member;
			}
			if (_configuration.IsUpdateMode)
			{
				IpcManager.PublishEvent("MemberUpdated", base.ConfigurationKey, _member);
			}
			else
			{
				IpcManager.PublishEvent("MemberCreated", base.ConfigurationKey, _member);
			}

			_logger.Trace(_className, methodName, "End");
			return true;
		}

		private void LinkSocial()
		{
			const string methodName = "LinkSocial";
			try
			{
				string socialName = StringUtils.FriendlyString(Session["AfterRegistrationLinkSocialAccount"]);
				if (!string.IsNullOrEmpty(socialName))
				{
					_logger.Debug(_className, methodName, string.Format("Attempting to link {0} account with member {1} ({2})", socialName, _member.Username, _member.IpCode));
					switch (socialName)
					{
						case "facebook":
							FBUser fbuser = Session["AfterRegistrationLinkSocialUser"] as FBUser;
							if (fbuser != null)
							{
								IList<MemberSocNet> socnets = LoyaltyService.GetMemberSocNets(SocialNetworkProviderType.Facebook, new string[] { fbuser.id });
								if (socnets == null || socnets.Count < 1)
								{
                                    string properties = Newtonsoft.Json.JsonConvert.SerializeObject(fbuser);
									LoyaltyService.CreateMemberSocNet(_member.IpCode, SocialNetworkProviderType.Facebook, fbuser.id, properties);
									_logger.Trace(_className, methodName, string.Format("Linked {0} account '{1}' with member {2} ({3})", socialName, fbuser.id, _member.Username, _member.IpCode));
								}
								else
								{
									_logger.Warning(_className, methodName, string.Format("Can't link {0} account '{1}' with member {2} ({3}) since already linked to member with ipcode {4}", socialName, fbuser.id, _member.Username, _member.IpCode, socnets[0].MemberId));
								}
							}
							else
							{
								_logger.Debug(_className, methodName, "No social user in session");
							}
							break;

						case "twitter":
							TWUser twuser = Session["AfterRegistrationLinkSocialUser"] as TWUser;
                            TwitterProvider.TwitterAccessToken twAccessToken = Session["AfterRegistrationLinkSocialUserProperties"] as TwitterProvider.TwitterAccessToken;
							if (twuser != null)
							{
								List<MemberSocNet> socnets = LoyaltyService.GetMemberSocNets(SocialNetworkProviderType.Twitter, new string[] { twuser.id_str });
								if (socnets == null || socnets.Count < 1)
								{
                                    string properties = twAccessToken != null ? Newtonsoft.Json.JsonConvert.SerializeObject(twAccessToken) : string.Empty;

									LoyaltyService.CreateMemberSocNet(_member.IpCode, SocialNetworkProviderType.Twitter, twuser.id_str, properties);
									_logger.Trace(_className, methodName, string.Format("Linked {0} account '{1}' with member {2} ({3})", socialName, twuser.id_str, _member.Username, _member.IpCode));
								}
								else
								{
									_logger.Warning(_className, methodName, string.Format("Can't link {0} account '{1}' with member {2} ({3}) since already linked to member with ipcode {4}", socialName, twuser.id_str, _member.Username, _member.IpCode, socnets[0].MemberId));
								}
							}
							else
							{
								_logger.Debug(_className, methodName, "No social user in session");
							}
							break;

						case "googleplus":
							GPUser gpuser = Session["AfterRegistrationLinkSocialUser"] as GPUser;
							if (gpuser != null)
							{
								IList<MemberSocNet> socnets = LoyaltyService.GetMemberSocNets(SocialNetworkProviderType.Google, new string[] { gpuser.id });
								if (socnets == null || socnets.Count < 1)
								{
									LoyaltyService.CreateMemberSocNet(_member.IpCode, SocialNetworkProviderType.Google, gpuser.id, string.Empty);
									_logger.Trace(_className, methodName, string.Format("Linked {0} account '{1}' with member {2} ({3})", socialName, gpuser.id, _member.Username, _member.IpCode));
								}
								else
								{
									_logger.Warning(_className, methodName, string.Format("Can't link {0} account '{1}' with member {2} ({3}) since already linked to member with ipcode {4}", socialName, gpuser.id, _member.Username, _member.IpCode, socnets[0].MemberId));
								}
							}
							else
							{
								_logger.Debug(_className, methodName, "No social user in session");
							}
							break;
					}
					Session["AfterRegistrationLinkSocialAccount"] = null;
					Session["AfterRegistrationLinkSocialUser"] = null;
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error linking social network: " + ex.Message, ex);
			}
		}

        private string GetLabelText(ConfigurationItem item)
        {
            return ResourceUtils.GetLocalWebResource(_modulePath, item.ResourceKey, item.DisplayText);
        }

		private void InitializeModuleConfiguration()
		{
			const string methodName = "InitializeModuleConfiguration";
			_logger.Trace(_className, methodName, "Begin");
			plcContent.Controls.Clear();
			_configuration = ConfigurationUtil.GetConfiguration<MemberProfileConfig>(base.ConfigurationKey);
			if (_configuration != null)
			{
				_currentLayout = _configuration.Layout;
				_layoutManager = new FixedLayoutManager(_configuration.Layout, plcContent);
			}
			_logger.Trace(_className, methodName, "End");
		}


		private void BuildPage()
		{
			const string methodName = "BuildPage";
			_logger.Trace(_className, methodName, "Begin");

            //this.CheckUpdateAccountFunction();

			foreach (ConfigurationItem attribute in _configuration.Attributes)
			{
				switch (attribute.AttributeType)
				{
					case ItemTypes.Attribute:
					case ItemTypes.Tier:
						AddAttributeItem(attribute);
						break;
					case ItemTypes.HtmlBlock:
						AddHtmlBlock(attribute);
						break;
					case ItemTypes.SwitchLayout:
						_layoutManager.SwitchLayoutMode(attribute.LayoutType);
						break;
					case ItemTypes.ActionButtons:
						AddActionButtons();
						break;
					case ItemTypes.ResetPasswordButton:
						AddResetPasswordButton(attribute);
						break;
					case ItemTypes.AccountStatus:
						AddAccountStatus(attribute);
						break;
				}
			}
			if (!_actionButtonsAdded)
			{
				AddActionButtons();
			}
			_logger.Trace(_className, methodName, "End");
		}


		private void AddActionButtons()
		{
			if (_actionButtonsAdded)
			{
				return;
			}

			bool canUpdate = false;
			foreach (ConfigurationItem item in _configuration.Attributes)
			{
				if ((item.AttributeType == ItemTypes.Attribute && item.DisplayType != DisplayTypes.Label) ||
					item.AttributeType == ItemTypes.AccountStatus ||
					item.AttributeType == ItemTypes.Tier)
				{
					canUpdate = true;
					break;
				}
			}
			if (!canUpdate) return;

			//build default button config if none exists. For backward compatability to versions prior 
			//to 3.3, where only a non-configurable reset and submit button existed
			bool buttonsConfigured = false;
			if (_configuration.FormActions != null)
			{
				foreach (ActionConfig action in _configuration.FormActions)
				{
					if (action != null)
					{
						buttonsConfigured = true;
						break;
					}
				}
			}
			if (!buttonsConfigured)
			{
				_configuration.FormActions[ActionButtons.Cancel] = new ActionConfig();
				_configuration.FormActions[ActionButtons.Cancel].Enabled = false;
				_configuration.FormActions[ActionButtons.Cancel].TextResourceKey = "btnCancel.Text";

				_configuration.FormActions[ActionButtons.Reset] = new ActionConfig();
				_configuration.FormActions[ActionButtons.Reset].Enabled = true;
				_configuration.FormActions[ActionButtons.Reset].TextResourceKey = "btnReset.Text";

				_configuration.FormActions[ActionButtons.SubmitOne] = new ActionConfig();
				_configuration.FormActions[ActionButtons.SubmitOne].Enabled = true;
				_configuration.FormActions[ActionButtons.SubmitOne].TextResourceKey = "btnSubmit.Text";

				_configuration.FormActions[ActionButtons.SubmitTwo] = new ActionConfig();
				_configuration.FormActions[ActionButtons.SubmitTwo].Enabled = false;
				_configuration.FormActions[ActionButtons.SubmitTwo].TextResourceKey = "btnNext.Text";
			}
			/////////////////////////////////////////////////////////////////////////////////////////

			var buttonStyle = PortalState.Portal.ButtonStyle;

			//List<WebControl> buttons = new List<WebControl>();
			List<Control> buttons = new List<Control>();

			switch (buttonStyle)
			{
				case PortalButtonStyle.Button:
					if (_configuration.FormActions[ActionButtons.Reset].Enabled)
					{
						btnReset.Visible = true;
						btnReset.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.Reset].TextResourceKey);
						btnReset.CssClass = _configuration.FormActions[ActionButtons.Reset].CssClass;
                        //btnReset.Enabled = _bUpdateAccountAllowed;
						buttons.Add(btnReset);
					}
					else
					{
						btnReset.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.SubmitOne].Enabled)
					{
						btnSubmitOne.Visible = true;
						btnSubmitOne.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.SubmitOne].TextResourceKey);
						btnSubmitOne.CssClass = _configuration.FormActions[ActionButtons.SubmitOne].CssClass;
                        //btnSubmitOne.Enabled = _bUpdateAccountAllowed;
						buttons.Add(btnSubmitOne);
					}
					else
					{
						btnSubmitOne.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.SubmitTwo].Enabled)
					{
						btnSubmitTwo.Visible = true;
						btnSubmitTwo.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.SubmitTwo].TextResourceKey);
						btnSubmitTwo.CssClass = _configuration.FormActions[ActionButtons.SubmitTwo].CssClass;
                        //btnSubmitTwo.Enabled = _bUpdateAccountAllowed;
						buttons.Add(btnSubmitTwo);
					}
					else
					{
						btnSubmitTwo.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.Cancel].Enabled)
					{
						btnCancel.Visible = true;
						btnCancel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.Cancel].TextResourceKey);
						btnCancel.CssClass = _configuration.FormActions[ActionButtons.Cancel].CssClass;
						//btnCancel.Enabled = _bUpdateAccountAllowed;
						buttons.Add(btnCancel);
					}
					else
					{
						btnCancel.Visible = false;
					}
					break;
				case PortalButtonStyle.LinkButton:
					if (_configuration.FormActions[ActionButtons.Reset].Enabled)
					{
						lnkReset.Visible = true;
						lnkReset.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.Reset].TextResourceKey);
						lnkReset.CssClass = _configuration.FormActions[ActionButtons.Reset].CssClass;
                        //lnkReset.Enabled = _bUpdateAccountAllowed;
						buttons.Add(lnkReset);
					}
					else
					{
						lnkReset.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.SubmitOne].Enabled)
					{
						lnkSubmitOne.Visible = true;
						lnkSubmitOne.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.SubmitOne].TextResourceKey);
						lnkSubmitOne.CssClass = _configuration.FormActions[ActionButtons.SubmitOne].CssClass;
                        //lnkSubmitOne.Enabled = _bUpdateAccountAllowed;
						buttons.Add(lnkSubmitOne);
					}
					else
					{
						lnkSubmitOne.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.SubmitTwo].Enabled)
					{
						lnkSubmitTwo.Visible = true;
						lnkSubmitTwo.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.SubmitTwo].TextResourceKey);
						lnkSubmitTwo.CssClass = _configuration.FormActions[ActionButtons.SubmitTwo].CssClass;
                        //lnkSubmitTwo.Enabled = _bUpdateAccountAllowed;
						buttons.Add(lnkSubmitTwo);
					}
					else
					{
						lnkSubmitTwo.Visible = false;
					}

					if (_configuration.FormActions[ActionButtons.Cancel].Enabled)
					{
						lnkCancel.Visible = true;
						lnkCancel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _configuration.FormActions[ActionButtons.Cancel].TextResourceKey);
						lnkCancel.CssClass = _configuration.FormActions[ActionButtons.Cancel].CssClass;
						//lnkCancel.Enabled = _bUpdateAccountAllowed;
						buttons.Add(lnkCancel);
					}
					else
					{
						lnkCancel.Visible = false;
					}
					break;
			}

			_layoutManager.AddActionButtons(buttons);
			_actionButtonsAdded = true;
		}

		private void AddResetPasswordButton(ConfigurationItem attribute)
		{
			if (!string.IsNullOrEmpty(attribute.ResourceKey))
			{
				attribute.DisplayText = ResourceUtils.GetLocalWebResource(_modulePath, attribute.ResourceKey);
			}
			if (_configuration.IsUpdateMode && _member != null)
			{
				if (_context.Owner == null)
				{
					_context.Owner = _member;
				}
				attribute.DisplayText = ExpressionUtil.ParseExpressions(attribute.DisplayText, _context);
			}

			var buttonStyle = PortalState.Portal.ButtonStyle;
			switch (buttonStyle)
			{
				case PortalButtonStyle.Button:
					_btnResetPassword.Text = attribute.DisplayText;
					_layoutManager.AddResetPasswordButton(_btnResetPassword);
					break;
				case PortalButtonStyle.LinkButton:
					_lnkResetPassword.Text = attribute.DisplayText;
					_layoutManager.AddResetPasswordButton(_lnkResetPassword);
					break;
			}
		}

		private void SwitchLayoutMode(LayoutTypes NewLayoutType)
		{
			if (NewLayoutType != _currentLayout)
			{
				if (NewLayoutType == LayoutTypes.TableSingleColumn || NewLayoutType == LayoutTypes.TableMultiColumn)
				{
					_contentTable = new Table();
					plcContent.Controls.Add(_contentTable);
				}
				_currentLayout = NewLayoutType;
			}
		}

        /// <summary>
        /// For CS portal, find out if the agent has access to UpdateAccount function
        /// and set the flag.
        /// </summary>
        //private void CheckUpdateAccountFunction()
        //{
        //    if (PortalState.IsCSAgentLoggedIn())
        //    {
        //        CSAgent agent = null;
        //        ILWCSService cs = null;
        //        string username = PortalState.GetUsername();
        //        cs = LWDataServiceUtil.CSServiceInstance();
        //        agent = cs.GetCSAgentByUserName(username, Brierley.FrameWork.Common.AgentAccountStatus.Active);
        //        _bUpdateAccountAllowed = agent.HasPermission(CSFunction.LW_CSFUNCTION_UPDATEACCOUNT);
        //    }
        //}

		private void AddAttributeItem(ConfigurationItem Attribute)
		{
			WebControl control;
			string value = string.Empty;
			if (_configuration.IsUpdateMode && _member != null)
			{
				value = GetMemberAttributeValue(Attribute);
			}
			switch (Attribute.DisplayType)
			{
				case DisplayTypes.TextBox:
				case DisplayTypes.DatePicker:
					control = new TextBox();
					if (!IsPostBack && !string.IsNullOrEmpty(Attribute.DefaultValue) && string.IsNullOrEmpty(value))
					{
						value = Attribute.DefaultValue;
					}
					((TextBox)control).Text = value;
					if (Attribute.AttributeSetID == -1 && Attribute.AttributeName == "Password")
					{
						((TextBox)control).TextMode = TextBoxMode.Password;
						control.Attributes.Add("value", value);
					}
					if (Attribute.MaxLength > -1)
					{
						checked
						{
							try
							{
								((TextBox)control).MaxLength = (int)Attribute.MaxLength;
							}
							catch { }
						}
					}

                    if (Attribute.AttributeName.ToLower() == "mobilephone")
                    {
                        List<ConfigurationItem> smsCollection = new List<ConfigurationItem>();
                        smsCollection.Add(Attribute);
                        foreach (var att in _configuration.Attributes)
                        {
                            if (att.AttributeName.ToLower() == "smsoptin" || att.AttributeName.ToLower() == "mobilephonecountrycode")
                            {
                                smsCollection.Add(att);
                            }
                        }

                        if (smsCollection.Count >= 3)
                        {
                            AddSmsClientValidation(smsCollection);
                            AttributeValidator attValidator = new AttributeValidator() { ValidatorType = ValidatorTypes.Custom, ClientValidationFunction = "sms_validation" };
                            Attribute.validators.Add(attValidator);
                        }
                    }
					break;
				case DisplayTypes.CheckBox:
					control = new Mobile.CheckBox();
					if (!IsPostBack && !string.IsNullOrEmpty(Attribute.DefaultValue) && string.IsNullOrEmpty(value))
					{
						value = Attribute.DefaultValue.ToLower() == "true" || Attribute.DefaultValue == "1" ? "true" : "false";
					}                    
                    //((CheckBox)control).Text = Attribute.DisplayText;
                    ((CheckBox)control).Text = GetLabelText(Attribute);
					((CheckBox)control).Checked = value == "1" || value.ToLower() == "true";

                    if (Attribute.AttributeName.ToLower() == "smsoptin")
                    {
                        List<ConfigurationItem> smsCollection = new List<ConfigurationItem>();
                        smsCollection.Add(Attribute);
                        foreach (var att in _configuration.Attributes)
                        {
                            if (att.AttributeName.ToLower() == "mobilephone" || att.AttributeName.ToLower() == "mobilephonecountrycode")
                            {
                                smsCollection.Add(att);
                            }
                        }

                        if (smsCollection.Count >= 3)
                        {
                            AddSmsClientValidation(smsCollection);
                            AttributeValidator attValidator = new AttributeValidator() { ValidatorType = ValidatorTypes.Custom, ClientValidationFunction = "sms_validation" };
                            Attribute.validators.Add(attValidator);
                        }
                    }
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
					throw new Exception(ResourceUtils.GetLocalWebResource(_modulePath, "FailedDisplayType.Text", "Failed to determine display type for attribute") + " " + Attribute.AttributeName);
			}

			if (Attribute.DisplayType == DisplayTypes.DropDownList || Attribute.DisplayType == DisplayTypes.RadioList)
			{
				List<ItemListSource> items = null;
				if (!string.IsNullOrEmpty(Attribute.ParentId))
				{

				}

				if (Attribute.ListSource != null)
				{
					foreach (ItemListSource source in Attribute.ListSource)
					{
						((ListControl)control).Items.Add(new ListItem(source.Value, source.Key));
					}
				}
				//else 
				if (Attribute.ListType == ListSourceType.GlobalAttributeSets)
				{
					if (!string.IsNullOrEmpty(Attribute.GlobalSetName) && !string.IsNullOrEmpty(Attribute.GlobalColumnName))
					{
						var crit = new LWCriterion(Attribute.GlobalSetName);
						crit.AddOrderBy(Attribute.GlobalColumnName, LWCriterion.OrderType.Ascending);
						IList<IClientDataObject> set = LoyaltyService.GetAttributeSetObjects(null, Attribute.GlobalSetName, crit, null, false);
						if (set != null)
						{
							foreach (var row in set)
							{
								var li = new ListItem(row.GetAttributeValue(Attribute.GlobalColumnName).ToString(), row.RowKey.ToString());

								if (Attribute.GlobalSetName == "RefCountry" && row.HasAttribute("SmsEnabled"))
								{
									li.Attributes.Add("SmsEnabled", row.GetAttributeValue("SmsEnabled").ToString().ToLower());
								}
								((ListControl)control).Items.Add(li);
							}
						}
					}
				}
				else if (Attribute.ListType == ListSourceType.FrameworkObject)
				{
					if (!string.IsNullOrEmpty(Attribute.LWFrameworkObjectName) && !string.IsNullOrEmpty(Attribute.LWFrameworkObjectProperty))
					{
						Dictionary<long, string> coll = LWObjectBinderUtil.GetObjectValues(Attribute.LWFrameworkObjectName,
							Attribute.LWFrameworkObjectProperty, Attribute.LWFrameworkObjectWhereClause);
						foreach (long key in coll.Keys)
						{
                            var li = new ListItem(coll[key], key.ToString());
							((ListControl)control).Items.Add(li);
						}
					}
				}

				if (Attribute.DisplayType == DisplayTypes.DropDownList || Attribute.DisplayType == DisplayTypes.RadioList)
				{
					if (!IsPostBack && !string.IsNullOrEmpty(Attribute.DefaultValue) && string.IsNullOrEmpty(value))
					{
						var list = (ListControl)control;
						foreach (ListItem item in list.Items)
						{
							if (item.Text == Attribute.DefaultValue)
							{
								item.Selected = true;
								break;
							}
						}
					}
					else
					{
						((ListControl)control).SelectedValue = value;
					}

                    if (Attribute.AttributeName.ToLower() == "mobilephonecountrycode" && Attribute.DisplayType == DisplayTypes.DropDownList)
                    {
                        List<ConfigurationItem> smsCollection = new List<ConfigurationItem>();
                        smsCollection.Add(Attribute);
                        foreach (var att in _configuration.Attributes)
                        {
                            if (att.AttributeName.ToLower() == "mobilephone" || att.AttributeName.ToLower() == "smsoptin")
                            {
                                smsCollection.Add(att);
                            }
                        }

                        if (smsCollection.Count >= 3)
                        {
                            AddSmsClientValidation(smsCollection);
                            AttributeValidator attValidator = new AttributeValidator() { ValidatorType = ValidatorTypes.Custom, ClientValidationFunction = "sms_validation" };
                            Attribute.validators.Add(attValidator);
                        }
                    }
				}
			}

			control.ID = Attribute.DataKey;
			control.CssClass = Attribute.ControlCSSClass;

			var controlLabel = new HtmlGenericControl("label");
            controlLabel.InnerHtml = Attribute.DisplayType == DisplayTypes.CheckBox ? "" : GetLabelText(Attribute);
			//if (!IsPostBack)
                controlLabel.Attributes.Add("class", Attribute.LabelCssClass);


			List<BaseValidator> validators = new List<BaseValidator>();

			if (Attribute.DisplayType != DisplayTypes.Label)
			{
				foreach (AttributeValidator validator in Attribute.validators)
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
							if (Attribute.DisplayType == DisplayTypes.CheckBox)
							{
								CustomValidator cv = new CustomValidator();
								cv.ServerValidate += delegate(object source, ServerValidateEventArgs args) { args.IsValid = ((CheckBox)Attribute.Control).Checked; };
								vldBase = cv;
							}
							else
							{
								RequiredFieldValidator vldReq = new RequiredFieldValidator();
								vldBase = vldReq;
							}
							break;
						case ValidatorTypes.Custom:
							CustomValidator vldCustom = new CustomValidator();
							vldCustom.ClientValidationFunction = validator.ClientValidationFunction;
							vldCustom.ValidateEmptyText = true;
							vldBase = vldCustom;
							break;
					}

					vldBase.Display = ValidatorDisplay.Dynamic;
					if (Attribute.DisplayType != DisplayTypes.CheckBox)
					{
						vldBase.ControlToValidate = control.ID;
					}
					vldBase.ValidationGroup = ValidationGroup;

					if(!string.IsNullOrEmpty(validator.ResourceKey))
					{
						vldBase.ErrorMessage = GetLocalResourceObject(validator.ResourceKey).ToString();
					}
					else
					{
						vldBase.ErrorMessage = validator.ErrorMessage;
					}

					vldBase.CssClass = validator.CssClass;

					validators.Add(vldBase);
				}

				if (Attribute.AttributeSetID == -1 && Attribute.AttributeName == "Password")
				{
					bool firstPassword = true;
					foreach (var att in _configuration.Attributes)
					{
						if (att.AttributeSetID == -1 && att.AttributeName == "Password")
						{
							if (att.DataKey != Attribute.DataKey)
							{
								firstPassword = false;
							}
							break;
						}
					}
					if (firstPassword)
					{
						var cv = new CustomValidator();
						cv.Display = ValidatorDisplay.Dynamic;
						cv.CssClass = "Validator";
						cv.ControlToValidate = control.ID;
						cv.ValidationGroup = ValidationGroup;
						cv.ServerValidate += new ServerValidateEventHandler(ValidPassword_ServerValidate);
						validators.Add(cv);
					}
				}

                if (Attribute.AttributeName == "MobilePhoneCountryCode")
                {
                    List<ConfigurationItem> smsCollection = new List<ConfigurationItem>();
                    smsCollection.Add(Attribute);
                    foreach (var att in _configuration.Attributes)
                    {
                        if (att.AttributeName.ToLower() == "mobilephone" || att.AttributeName.ToLower() == "smsoptin")
                        {
                            smsCollection.Add(att);
                        }
                    }

                    if (smsCollection.Count >= 3)
                    {
                        var cv = new CustomValidator();
                        cv.Display = ValidatorDisplay.Dynamic;
                        cv.CssClass = "Validator";
                        cv.ControlToValidate = control.ID;
                        cv.ValidationGroup = ValidationGroup;
                        cv.ServerValidate += new ServerValidateEventHandler(ValidMobilePhone_ServerValidate);
                        if (!validators.Contains(cv))
                            validators.Add(cv);
                    }
                }
			}

            //if (!_bUpdateAccountAllowed && !(control is Label))
            //    control.Enabled = false;

			_layoutManager.AddItem(control, controlLabel, validators);
			Attribute.Control = control;

			if (Attribute.DisplayType == DisplayTypes.DatePicker)
			{
				_datePickers.Add(control);
			}
		}

        void ValidMobilePhone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string phoneCode = string.Empty;
                string smsOptIn = string.Empty;
                string mobilePhone = string.Empty;
                foreach (ConfigurationItem item in _configuration.Attributes)
                {
                    switch (item.AttributeName.ToLower())
                    {
                        case "MobilePhoneCountryCode":
                            phoneCode = item != null ? GetPostbackValue(item) : phoneCode;
                            break;
                        case "SmsOptIn":
                            smsOptIn = item != null ? GetPostbackValue(item) : smsOptIn;
                            break;
                        case "MobilePhone":
                            mobilePhone = item != null ? GetPostbackValue(item) : mobilePhone;
                            break;
                    }
                }

                if (smsOptIn == "1")
                {
                    // All fields are present and populated
                    string errorMessage = string.Empty;
                    errorMessage = string.IsNullOrEmpty(phoneCode) || phoneCode == "-1" ? GetLocalResourceObject("MobilePhoneCountryCodeNotSelected.Text").ToString() : errorMessage;
                    errorMessage += string.IsNullOrEmpty(mobilePhone)
                                    ? !string.IsNullOrEmpty(errorMessage)
                                      ? "\r\n" + GetLocalResourceObject("MobilePhoneEmpty.Text").ToString()
                                      : GetLocalResourceObject("MobilePhoneEmpty.Text").ToString()
                                    : errorMessage;

                    

                    if (!string.IsNullOrEmpty(errorMessage))
                        throw new Exception(errorMessage);

                    // Check for Sms enabled country code
                    if (!string.IsNullOrEmpty(phoneCode))
                    {
                        if (!IsCountrySmsEnabled(phoneCode))
                            throw new Exception(GetLocalResourceObject("MobilePhoneCountryCodeNotSmsEnabled.Text").ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
                CustomValidator cv = source as CustomValidator;
                if (cv != null)
                {
                    cv.ErrorMessage = ex.Message;
                }
            }
        }

		void ValidPassword_ServerValidate(object source, ServerValidateEventArgs args)
		{
			try
			{
				string userName = string.Empty;
                string authField = "username";
                
                //var portalConfig = PortalManagementUtil.GetConfiguration();
                switch (PortalState.Portal.AuthenticationField)
                {
                    case AuthenticationFields.AlternateId:
                        authField = "alternateid";
                        break;
                    case AuthenticationFields.LoyaltyIdNumber:
                        authField = "loyalty id number";
                        break;
                    case AuthenticationFields.PrimaryEmailAddress:
                        authField = "primaryemailaddress";
                        break;
                    case AuthenticationFields.Username:
                    default:
                        authField = "username";
                        break;
                }
                ConfigurationItem att = null;
                foreach (ConfigurationItem item in _configuration.Attributes)
                {
                    if (item.AttributeName.ToLower() == authField)
                    {
                        att = item;
                        break;
                    }
                }
                if (att != null)
                {
                    userName = GetPostbackValue(att);
                }
				
				if (!string.IsNullOrEmpty(userName))
				{
					LWPasswordUtil.ValidatePassword(userName, args.Value);
					args.IsValid = true;
				}
				else if (_member != null && !string.IsNullOrEmpty(_member.Username))
				{
					LWPasswordUtil.ValidatePassword(_member.Username, args.Value);
					args.IsValid = true;
				}
				else
				{
					throw new AuthenticationException(ResourceUtils.GetLocalWebResource(_modulePath, "UsernameNotFound.Text","No username could be found.")) { ErrorCode = 1 };
				}
			}
			catch (AuthenticationException ex)
			{
				args.IsValid = false;
				CustomValidator cv = source as CustomValidator;
				if (cv != null)
				{
					cv.ErrorMessage = ex.Message;
				}
			}
		}


		private void AddAccountStatus(ConfigurationItem Attribute)
		{
			WebControl control;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("<script type=\"text/javascript\">");

			string value = string.Empty;
			IList<CSFunction> allFunctions = null;
			CSAgent agent = null;
			

			if (PortalState.IsCSAgentLoggedIn())
			{
				string username = PortalState.GetUsername();
				agent = CSService.GetCSAgentByUserName(username, Brierley.FrameWork.Common.AgentAccountStatus.Active);
				allFunctions = CSService.GetAllFunctions();
			}

			if (_configuration.IsUpdateMode && _member != null)
			{
				value = ((int)_member.MemberStatus).ToString();
			}

			sb.AppendFormat("_originalStatus = '{0}';\r\n", value);

			switch (Attribute.DisplayType)
			{
				case DisplayTypes.DropDownList:
					control = new DropDownList();
					sb.AppendLine("_accountStatusDisplayType = 'dropdown';");
					break;
				case DisplayTypes.RadioList:
					control = new RadioButtonList();
					((RadioButtonList)control).RepeatLayout = RepeatLayout.Flow;
					((RadioButtonList)control).RepeatDirection = RepeatDirection.Horizontal;
					((RadioButtonList)control).SelectedValue = value;
					sb.AppendLine("_accountStatusDisplayType = 'radio';");
					break;
				default:
					throw new Exception(ResourceUtils.GetLocalWebResource(_modulePath, "InvalidDisplayTypeAccountStatus.Text", "Failed to determine display type for account status. Invalid display type") + " " + Attribute.DisplayType.ToString());
			}

			List<ItemListSource> confirmationPrompts = new List<ItemListSource>();

			foreach (ItemListSource source in Attribute.ListSource)
			{
				if (_member != null)
				{
					MemberStatusEnum sourceStatus = (MemberStatusEnum)Enum.Parse(typeof(MemberStatusEnum), source.Key);
					if (sourceStatus == MemberStatusEnum.NonMember && _member.MemberStatus != MemberStatusEnum.NonMember)
					{
						//cannot set a member to non-member. skip this item.
						continue;
					}

					if (_member.MemberStatus == MemberStatusEnum.NonMember && sourceStatus != MemberStatusEnum.NonMember && sourceStatus != MemberStatusEnum.Active)
					{
						//cannot set non-member to anything other than member. skip this item.
						continue;
					}
				}

				ListItem item = new ListItem(source.Value, source.Key);

				if (source.Key == value && source.CanUpdate == false)
				{
					control.Enabled = false;
				}
				else
				{
					if (
						agent != null &&
						allFunctions != null && 
						allFunctions.Count > 0 &&
						source.RequiredFunctions != null && 
						source.RequiredFunctions.Count > 0
						)
					{
						bool hasFunction = false;
						foreach (long functionId in source.RequiredFunctions)
						{

							CSFunction requiredFunction = null;
							foreach (CSFunction function in allFunctions)
							{
								if (function.Id == functionId)
								{
									requiredFunction = function;
									break;
								}
							}
							if (requiredFunction != null)
							{
								if (agent.HasPermission(requiredFunction.Name))
								{
									hasFunction = true;
									break;
								}
							}
						}
						if (!hasFunction)
						{
							item.Enabled = false;
						}

					}
				}
				if (!string.IsNullOrEmpty(source.Confirmation))
				{
					confirmationPrompts.Add(source);
				}

				((ListControl)control).Items.Add(item);
			}

			if (Attribute.DisplayType == DisplayTypes.DropDownList)
			{
				((DropDownList)control).SelectedValue = value;
			}
			else
			{
				((RadioButtonList)control).SelectedValue = value;
			}

			control.ID = Attribute.DataKey;
			control.CssClass = Attribute.ControlCSSClass;

			var controlLabel = new HtmlGenericControl("label");
            controlLabel.InnerHtml = GetLabelText(Attribute);
			controlLabel.Attributes.Add("class", Attribute.LabelCssClass);

			_layoutManager.AddItem(control, controlLabel, null);
			Attribute.Control = control;

			sb.AppendFormat("_accountStatusClientId = '{0}';\r\n", control.ClientID);

			btnSubmitOne.Attributes.Add("onclick", "return ConfirmStatusChange();");
			btnSubmitTwo.Attributes.Add("onclick", "return ConfirmStatusChange();");
			lnkSubmitOne.Attributes.Add("onclick", "return ConfirmStatusChange();");
			lnkSubmitTwo.Attributes.Add("onclick", "return ConfirmStatusChange();");

			if (confirmationPrompts.Count > 0)
			{
				sb.Append("_confirmationPrompts = [");

				for (int i = 0; i < confirmationPrompts.Count; i++)
				{
					sb.AppendFormat("['{0}', '{1}']", confirmationPrompts[i].Key, confirmationPrompts[i].Confirmation.Replace("'", "\\'"));
					if (i < confirmationPrompts.Count - 1)
					{
						sb.Append(", ");
					}
				}
				sb.Append("];\r\n");
			}

			sb.AppendFormat("_valueCount = {0};\r\n", Attribute.ListSource.Count);
			sb.AppendLine("</script>");
			Page.ClientScript.RegisterStartupScript(this.GetType(), "AccountStatusChange", sb.ToString());
		}


		//todo: adding control to config gives us access to the control at any time. Change this so that 
		//we reference the control instead of request.form
		private string GetPostbackValue(ConfigurationItem Attribute)
		{
			foreach (string key in Request.Form.AllKeys)
			{
				if (key.Contains(Attribute.DataKey))
				{
					if (Attribute.DisplayType == DisplayTypes.CheckBox)
					{
						return "1";
					}
					else
					{
						return Request.Form[key];
					}
				}
			}
			if (Attribute.DisplayType == DisplayTypes.CheckBox)
			{
				//checkboxes don't come back unless they're checked
				return "0";
			}
			return "";
		}


		private int GetCardType()
		{
			int cardType = -1;
			foreach (ConfigurationItem attribute in _configuration.Attributes)
			{
				if (attribute.IsVirtualCard && attribute.AttributeName == "Card Type")
				{
					cardType = int.Parse(GetPostbackValue(attribute));
					break;
				}
			}
			return cardType;
		}

        private DateTime? GetExpirationDate()
        {
            DateTime? expirationDate = null;
            foreach (ConfigurationItem attribute in _configuration.Attributes)
            {
                if (attribute.IsVirtualCard && attribute.AttributeName == "Expiration Date")
                {
                    expirationDate = DateTime.Parse(GetPostbackValue(attribute));
                    break;
                }
            }
            return expirationDate;
        }

		private string GetMemberAttributeValue(ConfigurationItem Item)
		{
			string value = string.Empty;
			if (_member != null)
			{
				if (Item.AttributeType == ItemTypes.Tier)
				{
					var memberTier = _member.GetTier(DateTime.Now);
					if (memberTier != null)
					{
						var tierDef = LoyaltyService.GetTierDef(memberTier.TierDefId);
						if (tierDef != null)
						{
							value = tierDef.Name;
						}
					}
				}
				else if (Item.AttributeSetID < 0)
				{
					Type t = typeof(Member);
					System.Reflection.PropertyInfo[] properties = t.GetProperties();
					foreach (System.Reflection.PropertyInfo pi in properties)
					{
						if (pi.Name == Item.AttributeName)
						{
							if (pi.PropertyType == typeof(DateTime))
							{
								value = ((DateTime)pi.GetValue(_member, null)).ToShortDateString();
							}
							else if (pi.PropertyType == typeof(DateTime?))
							{
								DateTime? dt = ((DateTime?)pi.GetValue(_member, null));
								if (dt.HasValue)
								{
									value = dt.Value.ToShortDateString();
								}
							}
							else
							{
								object val = pi.GetValue(_member, null);
								if (val != null)
								{
									value = val.ToString();
								}
							}
							break;
						}
					}
				}
				else
				{
					IList<IClientDataObject> atsList = _member.GetChildAttributeSets(Item.AttributeSetName);
					if (atsList.Count < 1)
					{
						//no rows exist
					}
					else if (atsList.Count == 1)
					{
						object val = atsList[0].GetAttributeValue(Item.AttributeName);
						if (val != null)
						{
							if (val is DateTime)
							{
								value = ((DateTime)val).ToShortDateString();
							}
							else
							{
								value = val.ToString();
							}
						}
					}
					else
					{
						int rowIndex = -1;
						bool hasPrimary = false;

						try
						{
							object primaryTest = atsList[0].GetAttributeValue("isprimary");
							if (primaryTest != null)
							{
								hasPrimary = true;
							}
						}
						catch
						{
							//no "isprimary" exists.
						}

						if (!hasPrimary)
						{
							rowIndex = 0;
						}
						else
						{
							for (int i = 0; i < atsList.Count; i++)
							{
								if (atsList[i].GetAttributeValue("IsPrimary").ToString() != "1")
								{
									continue;
								}
								rowIndex = i;
							}
						}

						if (rowIndex > -1)
						{
							object val = atsList[rowIndex].GetAttributeValue(Item.AttributeName);
							if (val != null)
							{
								if (val is DateTime)
								{
									value = ((DateTime)val).ToShortDateString();
								}
								else
								{
									value = val.ToString();
								}
							}
						}
					}
				}
			}
			return value;
		}


		private void SetMemberAttributeValue(ConfigurationItem Item)
		{
			string methodName = "SetMemberAttributeValue";

			string value = GetPostbackValue(Item);

			if (Item.AttributeType == ItemTypes.Tier)
			{
				var currentTier = _member.GetTier(DateTime.Now);
				if (currentTier == null || value != GetMemberAttributeValue(Item))
				{
					//tier has changed or did not exist before. Create new tier
					DateTime enrollmentDate = DateTime.Now;
					if (!string.IsNullOrEmpty(Item.EnrollmentDateExpression))
					{
						if (_context.Owner == null)
						{
							_context.Owner = _member;
						}
						Expression e = new ExpressionFactory().Create(Item.EnrollmentDateExpression);
						object d = e.evaluate(_context);
						if (d != null)
						{
							if (d is DateTime)
							{
								enrollmentDate = (DateTime)d;
							}
							else
							{
								enrollmentDate = DateTime.Parse(d.ToString());
							}
						}
					}
					string note = null;
					if (!string.IsNullOrEmpty(Item.GlobalNoteFormat))
					{
						string oldTierName = currentTier == null || currentTier.TierDef == null ? string.Empty : currentTier.TierDef.Name;
						string newTierName = value;

						note = string.Format(Item.GlobalNoteFormat, "Tier", oldTierName, newTierName);
						if (note.Length > 150)
						{
							note = note.Substring(0, 150);
						}
					}
					_newTier = new NewTier(value, enrollmentDate, note);
					
				}
			}
			else if (Item.AttributeSetID < 0)
			{
				if (Item.IsVirtualCard)
				{
					//if (_vc == null)
					//{
					//    _vc = new VirtualCard(); // _member.CreateNewVirtualCard() as VirtualCard;
					//}
					if (Item.AttributeName == "Loyalty ID Number" && !string.IsNullOrEmpty(value))
					{
						if (_configuration.IsUpdateMode)
						{
							if (_member.LoyaltyCards.Count > 0)
							{
								foreach (VirtualCard vc in _member.LoyaltyCards)
								{
									vc.IsPrimary = false;
								}
							}
						}

						Member member = LoyaltyService.LoadMemberFromLoyaltyID(value);
						if (member != null)
						{
							string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "LoyaltyCardExistsOtherMember.Text", "Loyalty card id {0} already exists on another member."), value);
							_logger.Error(_className, methodName, errMsg);
							throw new LWValidationException(errMsg);
						}

						VirtualCard newvc = _member.GetLoyaltyCard(value);
						if (newvc != null)
						{
							// the card already exists
							string errMsg = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "LoyaltyCardExistsMember.Text", "Loyalty card id {0} already exists on the member."), value);
							_logger.Error(_className, methodName, errMsg);
                            throw new LWValidationException(errMsg);
						}
						else
						{
                            newvc = _member.CreateNewVirtualCard();
							newvc.LoyaltyIdNumber = value;

							if (Item.VirtualCardConfiguration != null && !Item.VirtualCardConfiguration.UserProvidesCardType)
							{
								newvc.CardType = Item.VirtualCardConfiguration.DefaultCardType;
							}
							else
							{
								newvc.CardType = GetCardType();
							}
						}
					}
				}
				else
				{
					if (Item.AttributeType == ItemTypes.AccountStatus)
					{
						if (!string.IsNullOrEmpty(value))
						{
							int accountStatus = 0;
							if (int.TryParse(value, out accountStatus))
							{
								var newStatus = (Brierley.FrameWork.Common.MemberStatusEnum)accountStatus;
								if (newStatus != _member.MemberStatus)
								{
									if (newStatus == MemberStatusEnum.Disabled || newStatus == MemberStatusEnum.Terminated)
									{
										MemberCancelOptions options = null;
										ItemListSource source = Item.ListSource.Where(o => o.Key == value).FirstOrDefault();
										if (source != null)
										{
											options = source.MemberCancelOptions;
										}
										LoyaltyService.CancelOrTerminateMember(_member, DateTime.Now, string.Empty, newStatus == MemberStatusEnum.Terminated, (options ?? new MemberCancelOptions()));
									}
									else
									{
										if (_member.MemberStatus == MemberStatusEnum.NonMember)
										{
											LoyaltyService.ConvertToMember(_member, DateTime.Now);
										}
										else
										{
											_member.NewStatus = newStatus;
											_member.NewStatusEffectiveDate = DateTime.Now;
										}
									}
								}
							}
							else
							{
								throw new Exception(string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "AccountStatusInvalid.Text", "Cannot update member account status. The account status {0} is not valid."), value));
							}
						}
					}
					else
					{
						SetPropertyValue(typeof(Member), _member, Item.AttributeName, value);
					}
				}
			}
			else
			{
				IList<IClientDataObject> atsList = _member.GetChildAttributeSets(Item.AttributeSetName);
				IClientDataObject set = null;

				if (atsList.Count > 0)
				{
					if (atsList.Count == 1)
					{
						set = atsList[0];
					}
					else
					{
						int rowIndex = -1;
						bool hasPrimary = false;

						System.Reflection.PropertyInfo[] info = atsList[0].GetType().GetProperties();
						foreach (System.Reflection.PropertyInfo pi in info)
						{
							if (pi.Name.ToLower() == "isprimary")
							{
								hasPrimary = true;
								break;
							}
						}

						if (!hasPrimary)
						{
							rowIndex = 0;
						}
						else
						{
							for (int i = 0; i < atsList.Count; i++)
							{
								if (atsList[i].GetAttributeValue("isprimary").ToString() != "1")
								{
									continue;
								}
								rowIndex = i;
								break;
							}
						}
						if (rowIndex > -1)
						{
							set = atsList[rowIndex];
						}
					}
				}
				else
				{
					//add new row
					set = DataServiceUtil.GetNewClientDataObject(Item.AttributeSetName);
				}

				object propertyValue = GetPropertyValue(set.GetType(), Item.AttributeName, value);
				set.SetAttributeValue(Item.AttributeName, propertyValue);

				if (atsList.Count < 1)
				{
					_member.AddChildAttributeSet(set);
				}
			}
		}


		private object GetPropertyValue(Type type, string attributeName, string value)
		{
            string methodName = "GetPropertyValue";

			object ret = null;
            try
            {
                System.Reflection.PropertyInfo[] propertyInfo = type.GetProperties();
                foreach (System.Reflection.PropertyInfo pi in propertyInfo)
                {
                    if (pi.Name.ToLower() == attributeName.ToLower())
                    {
                        if (pi.PropertyType == typeof(DateTime))
                        {
                            ret = DateTime.Parse(value);
                        }
                        else if (pi.PropertyType == typeof(DateTime?))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                ret = DateTime.Parse(value);
                            }
                        }
                        else if (pi.PropertyType == typeof(bool))
                        {
                            ret = (value != "0");
                        }
                        else if (pi.PropertyType == typeof(bool?))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                ret = (value != "0");
                            }
                        }
                        else if (pi.PropertyType == typeof(int))
                        {
                            ret = int.Parse(value);
                        }
                        else if (pi.PropertyType == typeof(int?))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                ret = int.Parse(value);
                            }
                        }
                        else if (pi.PropertyType == typeof(double))
                        {
                            ret = double.Parse(value);
                        }
                        else if (pi.PropertyType == typeof(double?))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                ret = double.Parse(value);
                            }
                        }
                        else if (pi.PropertyType == typeof(long))
                        {
                            ret = long.Parse(value);
                        }
                        else if (pi.PropertyType == typeof(long?))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                ret = long.Parse(value);
                            }
                        }
                        else if (pi.PropertyType == typeof(string))
                        {
                            ret = value;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error getting value for property " + attributeName + ".", ex);
                throw;
            }
			return ret;
		}


		private void SetPropertyValue(Type type, object obj, string attributeName, string value)
		{
			System.Reflection.PropertyInfo[] properties = type.GetProperties();
			foreach (System.Reflection.PropertyInfo pi in properties)
			{
				if (pi.Name == attributeName)
				{
					if (pi.PropertyType == typeof(DateTime))
					{
						pi.SetValue(obj, DateTime.Parse(value), null);
					}
					else if (pi.PropertyType == typeof(DateTime?))
					{
						if (!string.IsNullOrEmpty(value))
						{
							pi.SetValue(obj, DateTime.Parse(value), null);
						}
						else
						{
							pi.SetValue(obj, null, null);
						}
					}
					else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?))
					{
						pi.SetValue(obj, value != "0", null);
					}
					else if (pi.PropertyType == typeof(int))
					{
						pi.SetValue(obj, int.Parse(value), null);
					}
					else if (pi.PropertyType == typeof(int?))
					{
						if (!string.IsNullOrEmpty(value))
						{
							pi.SetValue(obj, int.Parse(value), null);
						}
						else
						{
							pi.SetValue(obj, null, null);
						}
					}
					else if (pi.PropertyType == typeof(double))
					{
						pi.SetValue(obj, double.Parse(value), null);
					}
					else if (pi.PropertyType == typeof(double?))
					{
						if (!string.IsNullOrEmpty(value))
						{
							pi.SetValue(obj, int.Parse(value), null);
						}
						else
						{
							pi.SetValue(obj, null, null);
						}
					}
					else if (pi.PropertyType == typeof(long))
					{
						pi.SetValue(obj, long.Parse(value), null);
					}
					else if (pi.PropertyType == typeof(long?))
					{
						if (!string.IsNullOrEmpty(value))
						{
							pi.SetValue(obj, long.Parse(value), null);
						}
						else
						{
							pi.SetValue(obj, null, null);
						}
					}
					else if (pi.PropertyType == typeof(string))
					{
						pi.SetValue(obj, value, null);
					}
					break;
				}
			}
		}


		private void AddHtmlBlock(ConfigurationItem Attribute)
		{
			if (!string.IsNullOrEmpty(Attribute.ResourceKey))
			{
				Attribute.DisplayText = ResourceUtils.GetLocalWebResource(_modulePath, Attribute.ResourceKey);
			}
            if (_configuration.IsUpdateMode && _member != null)
            {
				if (_context.Owner == null)
				{
					_context.Owner = _member;
				}
                Attribute.DisplayText = ExpressionUtil.ParseExpressions(Attribute.DisplayText, _context);
            }
			_layoutManager.AddHtmlBlock(Attribute.DisplayText);
		}


		private void AddInterceptorErrors(Dictionary<string, ConfigurationItem> errors)
		{
			
			foreach (var error in errors.Keys)
			{
				var item = errors[error];
				Control offender = null;
				if (item != null)
				{
					offender = item.Control;
				}

				base.AddInvalidField(error, offender);

				/*
				var vld = new CustomValidator() { IsValid = false, ErrorMessage = error, CssClass = "Validator", ValidationGroup = ValidationGroup };
				if (offender != null)
				{
					vld.ControlToValidate = offender.ID;
					offender.Parent.Controls.Add(vld);
				}
				else
				{
					this.Controls.Add(vld);
				}
				*/
			}
		}

		private string GetNoteForAttribute(ConfigurationItem item)
		{
			if (string.IsNullOrEmpty(item.GlobalNoteFormat))
			{
				return null;
			}

			string attributeName = GetLabelText(item);
			string oldValue = GetMemberAttributeValue(item) ?? string.Empty;
			string newValue = GetPostbackValue(item) ?? string.Empty;

			if (oldValue == newValue)
			{
				return null;
			}

			if (item.DisplayType == DisplayTypes.CheckBox)
			{
				//convert to yes/no
				oldValue = oldValue == "1" || oldValue == "true" ? "true" : "false";
				newValue = newValue == "1" || newValue == "true" ? "true" : "false";
			}
			else if (item.DisplayType == DisplayTypes.DropDownList || item.DisplayType == DisplayTypes.RadioList)
			{
				var lc = item.Control as ListControl;
				if (lc != null)
				{
					if (!string.IsNullOrEmpty(oldValue))
					{
						foreach (ListItem li in lc.Items)
						{
							if (li.Value == oldValue)
							{
								oldValue = li.Text;
								break;
							}
						}
					}

					if (!string.IsNullOrEmpty(newValue))
					{
						foreach (ListItem li in lc.Items)
						{
							if (li.Value == newValue)
							{
								newValue = li.Text;
								break;
							}
						}
					}
				}
			}

			return string.Format(item.GlobalNoteFormat, attributeName, oldValue, newValue);

		}

        private void AddSmsClientValidation(List<ConfigurationItem> smsCollection)
        {
            StringBuilder sb = new System.Text.StringBuilder();
            string smsOptInId = string.Empty;
            string mobilePhoneCountryCode = string.Empty;
            string mobilePhone = string.Empty;
            string functionName = "sms_validation";
            string mpCountryCodeNotSelected = GetLocalResourceObject("MobilePhoneCountryCodeNotSelected.Text").ToString();
            string mpCountryCodeNotSmsEnabled = GetLocalResourceObject("MobilePhoneCountryCodeNotSmsEnabled.Text").ToString();
            string mobilePhoneEmpty = GetLocalResourceObject("MobilePhoneEmpty.Text").ToString();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("function " + functionName + "(source, arguments){");

            foreach (ConfigurationItem item in smsCollection)
            {
                switch (item.AttributeName.ToLower())
                {
                    case "smsoptin":
                        smsOptInId = item.DataKey;
                        sb.AppendLine("var smsOptIn = $(\"[id*='" + smsOptInId + "']\").is(':checked');");
                        break;
                    case "mobilephonecountrycode":
                        mobilePhoneCountryCode = item.DataKey;
                        sb.AppendLine("var mobilePhoneCountryCode = $(\"[id*='" + mobilePhoneCountryCode + "']\").val();");
                        break;
                    case "mobilephone":
                        mobilePhone = item.DataKey;
                        sb.AppendLine("var mobilePhone = $(\"[id*='" + mobilePhone + "']\").val();");
                        break;
                }
            }

            sb.AppendLine("var errorMessage = '';");
            sb.AppendLine("if (smsOptIn) {\r\nswitch (source.controltovalidate) {\r\ncase $(\"[id*='" + mobilePhoneCountryCode + "']\").attr('id'):\r\n");
            sb.AppendLine("if (mobilePhoneCountryCode == '' || mobilePhoneCountryCode == '-1') {\r\nerrorMessage = '" + mpCountryCodeNotSelected + "';\r\n$(\"[id*='" + mobilePhoneCountryCode + "']\").addClass('ValidatorInvalid');\r\n}");
            sb.AppendLine("var smsEnabled = $(\"[id*='" + mobilePhoneCountryCode + "']\").find('option:selected').attr('SmsEnabled');");
            sb.AppendLine("if (smsEnabled == 'false') {\r\nerrorMessage = '" + mpCountryCodeNotSmsEnabled + "';\r\n$(\"[id*='" + mobilePhoneCountryCode + "']\").addClass('ValidatorInvalid'); }\r\nbreak;");
            sb.AppendLine("case $(\"[id*='" + mobilePhone + "']\").attr('id'):\r\nif (mobilePhone == '') {\r\nerrorMessage = '" + mobilePhoneEmpty + "';\r\n$(\"[id*='" + mobilePhone + "']\").addClass('ValidatorInvalid'); } break; }\r\n}");
            sb.AppendLine("if (errorMessage == '') {\r\narguments.IsValid = true;\r\n} else {\r\narguments.IsValid = false;\r\nsource.errormessage = errorMessage;\r\n}\r\n}");
            sb.AppendLine("</script>");
            Page.ClientScript.RegisterStartupScript(this.GetType(), functionName, sb.ToString());
        }

        private bool IsCountrySmsEnabled(string phoneCode)
        {
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            ServiceConfig _config = LWDataServiceUtil.GetServiceConfiguration(ctx.Organization, ctx.Environment);
            using (var svc = new LoyaltyDataService(_config))
            {
                LWCriterion lwCriteria = new LWCriterion("RefCountry");
                List<IClientDataObject> clientDataObjects = svc.GetAttributeSetObjects(null, "RefCountry", lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
                IClientDataObject refCountryObj = new ClientDataObject();
                if (clientDataObjects.Count > 0)
                {
                    foreach (IClientDataObject cObj in clientDataObjects)
                    {
                        if (cObj.GetAttributeValue("PhoneCode").ToString() == phoneCode)
                            return Convert.ToBoolean(cObj.GetAttributeValue("SmsEnabled"));
                    }
                }
            }
            return false;
        }
	}
}
