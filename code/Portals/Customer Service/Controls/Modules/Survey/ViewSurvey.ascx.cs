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

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.Survey
{
	public partial class ViewSurvey : ModuleControlBase
	{
		#region fields
		private const string _className = "ViewSurvey";
		private const string _modulePath = "~/Controls/Modules/Survey/ViewSurvey.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger("Survey");
		private SurveyConfig _config = null;
		private SurveyRunnerControl _surveyRunner;
		private PlaceHolder _phErrorMessage = new PlaceHolder();
		#endregion

		#region properties
		private long SurveyID
		{
			get { return StringUtils.FriendlyInt64(ViewState["CurrentSurveyID"]); }
			set { ViewState["CurrentSurveyID"] = value; }
		}

		private long LanguageID
		{
			get { return StringUtils.FriendlyInt64(ViewState["CurrentLanguageID"]); }
			set { ViewState["CurrentLanguageID"] = value; }
		}
		#endregion

		#region page life cycle
		protected override void OnLoad(EventArgs e)
		{
			const string methodName = "OnLoad";
			try
			{
				base.OnLoad(e);

				InitializeConfig();
				ResolveSurveyID();
				ResolveLanguageID();
				AddSurveyRunnerControl();
				Controls.Add(_phErrorMessage);
			}
			catch (LWException ex)
			{
				_logger.Error(_className, methodName, "LWException: " + ex.Message);
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}
		#endregion

		#region event handlers
		void _surveyRunner_SurveyCompleted(object sender, EventArgs e)
		{
			if (_surveyRunner == null)
				return;
			const string methodName = "_surveyRunner_SurveyCompleted";

			try
			{
				string redirectURL = string.Empty;
				SurveyConfig.SurveyErrorHandlingModeEnum errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.URL;
				string errorHandlingText = string.Empty;
				switch (_surveyRunner.StateModelStatus)
				{
					case StateModelStatus.AlreadyTookSurvey:
						if (_config.AlreadyTookSurveyMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, "Already took survey, literal text: " + _config.AlreadyTookSurveyURL);
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.AlreadyTookSurveyURL;
						}
						else
						{
							_logger.Trace(_className, methodName, "Already took survey, redirect to: " + _config.AlreadyTookSurveyURL);
							redirectURL = _config.AlreadyTookSurveyURL;
						}
						break;

					case StateModelStatus.Completed:
						switch (_surveyRunner.TerminationType)
						{
							case StateModelTerminationType.Success:
								if (_config.SurveyCompletedSuccessMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
								{
									_logger.Trace(_className, methodName, "Survey completed with success, literal text: " + _config.SurveyCompletedSuccessURL);
									errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
									errorHandlingText = _config.SurveyCompletedSuccessURL;
								}
								else
								{
									_logger.Trace(_className, methodName, "Survey completed with success, redirect to: " + _config.SurveyCompletedSuccessURL);
									redirectURL = _config.SurveyCompletedSuccessURL;
								}
								break;

							case StateModelTerminationType.Skip:
								if (_config.SurveyCompletedSkipMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
								{
									_logger.Trace(_className, methodName, "Survey completed with skip, literal text: " + _config.SurveyCompletedSkipURL);
									errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
									errorHandlingText = _config.SurveyCompletedSkipURL;
								}
								else
								{
									_logger.Trace(_className, methodName, "Survey completed with skip, redirect to: " + _config.SurveyCompletedSkipURL);
									redirectURL = _config.SurveyCompletedSkipURL;
								}
								break;

							default:
								// should never get here
								if (_config.NoAvaliableSurveysMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
								{
									_logger.Trace(_className, methodName, string.Format("Unexpected surveyRunner TerminationType {0}, literal text: {1}", _surveyRunner.TerminationType, _config.NoAvaliableSurveysURL));
									errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
									errorHandlingText = _config.NoAvaliableSurveysURL;
								}
								else
								{
									_logger.Error(_className, methodName, "Unexpected surveyRunner TerminationType: " + _surveyRunner.TerminationType);
									redirectURL = _config.NoAvaliableSurveysURL;
								}
								break;
						}
						break;

					default:
					case StateModelStatus.NoEligibleRespondent:
					case StateModelStatus.NoMatchingRespondent:
					case StateModelStatus.NoRespondent:
					case StateModelStatus.NoLanguage:
					case StateModelStatus.NoState:
					case StateModelStatus.NoSurvey:
					case StateModelStatus.NotInitialized:
						if (_config.NoAvaliableSurveysMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, string.Format("Survey initialization error '{0}', literal text: {1}", _surveyRunner.StateModelStatus, _config.NoAvaliableSurveysURL));
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.NoAvaliableSurveysURL;
						}
						else
						{
							_logger.Error(_className, methodName,
								string.Format("Survey initialization error '{0}', redirect to: {1}",
								_surveyRunner.StateModelStatus, _config.NoAvaliableSurveysURL)
							);
							redirectURL = _config.NoAvaliableSurveysURL;
						}
						break;

					case StateModelStatus.QuotaExceeded:
						if (_config.QuotaExceededMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, "Quota has been exceeded, literal text: " + _config.QuotaExceededURL);
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.QuotaExceededURL;
						}
						else
						{
							_logger.Trace(_className, methodName, "Quota has been exceeded, redirect to: " + _config.QuotaExceededURL);
							redirectURL = _config.QuotaExceededURL;
						}
						break;

					case StateModelStatus.SurveyClosed:
						if (_config.SurveyClosedMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, "Survey is closed, literal text: " + _config.SurveyClosedURL);
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.SurveyClosedURL;
						}
						else
						{
							_logger.Trace(_className, methodName, "Survey is closed, redirect to: " + _config.SurveyClosedURL);
							redirectURL = _config.SurveyClosedURL;
						}
						break;

					case StateModelStatus.SurveyNotEffective:
						if (_config.SurveyNotEffectiveMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, "Survey not effective, literal text: " + _config.SurveyNotEffectiveURL);
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.SurveyNotEffectiveURL;
						}
						else
						{
							_logger.Trace(_className, methodName, "Survey not effective, redirect to: " + _config.SurveyNotEffectiveURL);
							redirectURL = _config.SurveyNotEffectiveURL;
						}
						break;

					case StateModelStatus.SurveyUnpublished:
						if (_config.SurveyUnpublishedMode == SurveyConfig.SurveyErrorHandlingModeEnum.Literal)
						{
							_logger.Trace(_className, methodName, "Survey is not published, literal text: " + _config.SurveyUnpublishedURL);
							errorHandlingMode = SurveyConfig.SurveyErrorHandlingModeEnum.Literal;
							errorHandlingText = _config.SurveyUnpublishedURL;
						}
						else
						{
							_logger.Trace(_className, methodName, "Survey is not published, redirect to: " + _config.SurveyUnpublishedURL);
							redirectURL = _config.SurveyUnpublishedURL;
						}
						break;
				}

				if (errorHandlingMode == SurveyConfig.SurveyErrorHandlingModeEnum.URL)
				{
					if (!string.IsNullOrEmpty(redirectURL))
					{
						Response.Redirect(redirectURL, false);
					}
					else
					{
						_logger.Trace(_className, methodName, "Redirect url is empty, so redirect to error.html.");
						Response.Redirect("error.html", false);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(errorHandlingText))
					{
						_surveyRunner.Visible = false;
						_phErrorMessage.Controls.Add(new LiteralControl(errorHandlingText));
					}
					else
					{
						_logger.Trace(_className, methodName, "Error handling text is empty, so redirect to error.html.");
						Response.Redirect("error.html", false);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		void _surveyRunner_ShowWarning(object sender, string message)
		{
			ShowWarning(message);
		}

		void _surveyRunner_ShowNegative(object sender, string message)
		{
			ShowNegative(message);
		}

		void _surveyRunner_ShowPositive(object sender, string message)
		{
			ShowPositive(message);
		}

		void _surveyRunner_ExceptionThrown(object sender, WebFrameWork.MessageHandling.ExceptionEventArgs e)
		{
			if (e != null)
			{
                string message = StringUtils.FriendlyString(e.Message, e.Exception != null ? e.Exception.Message : string.Empty);
                if (e.Exception.GetType() == (typeof(SurveyQuestionException)))
                {
                    message = string.Format(ResourceUtils.GetLocalWebResource(_modulePath, "QuestionAlreadyAnswered.Text", "You've already answered this question.  <a href=\"{0}\">Please click here to continue.</a>"), Request.UrlReferrer);
                    Control btnNext = _surveyRunner.GetNextButton();
                    if (btnNext != null)
                    {
                        btnNext.Visible = false;
                    }
                }

				ShowNegative(message);
			}
		}
		#endregion

		#region private methods
		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<SurveyConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new SurveyConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		private void ResolveSurveyID()
		{
			const string methodName = "ResolveSurveyID";
			if (_config.SelectionMethod == SurveySelectionMethod.SurveyID)
			{
				if (SurveyID == -1 && !string.IsNullOrEmpty(_config.SurveyName))
				{
					SMSurvey survey = SurveyManager.RetrieveSurvey(_config.SurveyName);
					if (survey == null)
					{
						string msg = ResourceUtils.GetLocalWebResource(_modulePath, "SurveyNameNotFound", "Unable to find survey by name:") + " " + _config.SurveyName;
                        _logger.Error(_className, methodName, "Unable to find survey by name:");
						throw new LWException(msg);
					}
					SurveyID = survey.ID;
				}
			}
		}

		private void ResolveLanguageID()
		{
			const string methodName = "ResolveLanguageID";
			if (LanguageID == -1 && !string.IsNullOrEmpty(_config.LanguageName))
			{
				string languageName = StringUtils.FriendlyString(_config.LanguageName, "English");
				SMLanguage language = SurveyManager.RetrieveLanguage(languageName);
				if (language == null)
				{
					string msg = ResourceUtils.GetLocalWebResource(_modulePath, "LanguageNameNotFound", "Unable to find language by name:") + " " + languageName;
                    _logger.Error(_className, methodName, "Unable to find language by name:");
					throw new LWException(msg);
				}
				LanguageID = language.ID;
			}
		}

		private void AddSurveyRunnerControl()
		{
			Member member = null;
			string mtouch = Request.Params["MTouch"];
			long ipcode = -1;
			if (PortalState.Portal.PortalMode == PortalModes.CustomerFacing && PortalState.IsMemberLoggedIn())
			{
				member = PortalState.GetLoggedInMember();
				if (member != null)
				{
					ipcode = member.IpCode;
				}
			}

			_surveyRunner = new SurveyRunnerControl();
			_surveyRunner.Height = (_config.IsHeightPercent ? Unit.Percentage(_config.Height) : Unit.Pixel(_config.Height));
			_surveyRunner.Width = (_config.IsWidthPercent ? Unit.Percentage(_config.Width) : Unit.Pixel(_config.Width));
			_surveyRunner.NextButtonUseLinkButton = true;
			_surveyRunner.NextButtonButtonText = StringUtils.FriendlyString(ResourceUtils.GetLocalWebResource(_modulePath, _config.NextButtonButtonTextResourceKey), "next");
			_surveyRunner.NextButtonCssStyle = StringUtils.FriendlyString(_config.NextButtonCssStyle);
			_surveyRunner.NextButtonCssClass = StringUtils.FriendlyString(_config.NextButtonCssClass);
			_surveyRunner.NextButtonOnClick = StringUtils.FriendlyString(_config.NextButtonOnClick);
			_surveyRunner.UseAppCache = true;
			_surveyRunner.SurveyID = SurveyID;
			_surveyRunner.LanguageID = LanguageID;
			_surveyRunner.SurveySelectionMethod = _config.SelectionMethod;
			_surveyRunner.IPCode = ipcode;
			_surveyRunner.MTouch = mtouch;
			_surveyRunner.BScript = StringUtils.FriendlyString(_config.BScript);
			_surveyRunner.SurveyCompleted += new EventHandler(_surveyRunner_SurveyCompleted);
			_surveyRunner.ShowPositive += _surveyRunner_ShowPositive;
			_surveyRunner.ShowNegative += _surveyRunner_ShowNegative;
			_surveyRunner.ShowWarning += _surveyRunner_ShowWarning;
			_surveyRunner.ExceptionThrown += _surveyRunner_ExceptionThrown;

			Controls.Add(_surveyRunner);
		}
		#endregion
	}
}
