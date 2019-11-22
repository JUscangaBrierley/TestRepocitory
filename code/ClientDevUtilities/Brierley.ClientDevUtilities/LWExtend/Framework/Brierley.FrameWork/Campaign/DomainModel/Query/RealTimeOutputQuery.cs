using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Facebook;
using LinqToTwitter;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class RealTimeOutputQuery : OutputQuery
	{
		private const string _className = "RealTimeOutputQuery";
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);		

		private static Dictionary<SocialNetworkProviderType, bool> _followsOptInConvention = new Dictionary<SocialNetworkProviderType, bool>();
        private static Dictionary<OutputType, bool> _optInConvention = new Dictionary<OutputType,bool>();
		private List<ContentAttributeDef> _defs = null;

		public override List<SqlStatement> GetSqlStatement(bool isValidationTest, Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}

		/// <summary>
		/// Assembles the SQL statement for the verification (commit) process of the step
		/// </summary>
		/// <returns></returns>
		public override List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}

		public override bool Validate(List<ValidationMessage> warnings, bool validateSql)
		{
			return true;
		}

		internal override List<CampaignResult> Execute(ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			if (co == null)
			{
				throw new ArgumentNullException("co");
			}
			if (co.Owner == null)
			{
				throw new ArgumentNullException("co.Owner");
			}

			Member m = co.ResolveMember();
			List<CampaignResult> results = new List<CampaignResult>();

			using(CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
			using (LoyaltyDataService mgr = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{

				List<long> refIds = null;

				List<int> conditionRows = GetConditionRows();
				if (conditionRows.Count > 0 &&
					(OutputOption.OutputType == OutputType.Coupon || OutputOption.OutputType == OutputType.Promotion || OutputOption.OutputType == OutputType.Offer || OutputOption.OutputType == OutputType.Message)
					)
				{
					refIds = new List<long>();
					foreach (long refId in OutputOption.RefId)
					{
						ContentDefBase outputDef = null;
						switch (OutputOption.OutputType)
						{
							case OutputType.Coupon:
								outputDef = content.GetCouponDef(refId);
								break;
							case OutputType.Offer:
								outputDef = content.GetBonusDef(refId);
								break;
							case OutputType.Promotion:
								outputDef = content.GetPromotion(refId);
								break;
							case OutputType.Message:
								outputDef = content.GetMessageDef(refId);
								break;
						}
						foreach (int row in conditionRows)
						{
							if (EvaluateRow(row, outputDef, co))
							{
								refIds.Add(refId);
								break;
							}
						}
					}
				}
				else
				{
					refIds = OutputOption.RefId;
				}


				if (co.Mode == RuleExecutionMode.Simulation)
				{
					foreach (long refId in refIds)
					{
						results.Add(new CampaignResult(OutputOption.OutputType, refId, 0));
					}
					return results;
				}

				switch (OutputOption.OutputType)
				{
					case OutputType.Coupon:
						foreach (long couponId in refIds)
						{
							CouponDef coupon = null;
							DateTime? startDate = GetExpressionDate(OutputOption.StartDate, null);
							DateTime? defaultExpiration = null;

							if (string.IsNullOrEmpty(OutputOption.ExpirationDate) || OutputOption.UseCertificates || !startDate.HasValue)
							{
								coupon = content.GetCouponDef(couponId);
								if (coupon == null)
								{
									throw new Exception("Cannot create coupons because the coupon (" + couponId.ToString() + ") could not be found. Please ensure that the coupon selected for the output step exists.");
								}
								if (string.IsNullOrEmpty(OutputOption.ExpirationDate))
								{
									defaultExpiration = coupon.ExpiryDate;
								}
								if (!startDate.HasValue)
								{
									startDate = coupon.StartDate;
								}
							}

							MemberCoupon memberCoupon = new MemberCoupon();
							memberCoupon.CouponDefId = couponId;
							memberCoupon.MemberId = m.IpCode;
							memberCoupon.DateIssued = DateTime.Now;
							memberCoupon.TimesUsed = 0;
                            memberCoupon.StartDate = startDate.Value;
                            memberCoupon.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, defaultExpiration);
							memberCoupon.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
							if (OutputOption.UseCertificates)
							{
								memberCoupon.CertificateNmbr = GetNextCertificateNumber(content, ContentObjType.Coupon, coupon.CouponTypeCode);
							}
							mgr.CreateMemberCoupon(memberCoupon);

                            //Send Push notification if one is attached to the coupon definition
                            if (coupon != null && coupon.PushNotificationId != null)
                            {
                                SendAssociatedPush(m, mgr, (long)coupon.PushNotificationId);
                            }

							results.Add(new CampaignResult(OutputType.Coupon, couponId, memberCoupon.ID));
						}
						break;
					case OutputType.Message:
						{
							foreach (long messageId in refIds)
							{
								DateTime? defaultExpiration = null;
                                MessageDef message = content.GetMessageDef(messageId);
								if (string.IsNullOrEmpty(OutputOption.ExpirationDate))
								{
									if (message == null)
									{
										throw new Exception("Cannot create message because the message (" + messageId.ToString() + ") could not be found. Please ensure that the message selected for the output step exists.");
									}
									defaultExpiration = message.ExpiryDate;
								}

                                long memberMessageId = 0;

                                if (!OutputOption.Transient)
                                {
                                    MemberMessage memberMessage = new MemberMessage();
                                    memberMessage.MessageDefId = messageId;
                                    memberMessage.MemberId = m.IpCode;
                                    memberMessage.DateIssued = DateTime.Now;
                                    memberMessage.StartDate = GetExpressionDate(OutputOption.StartDate, message.StartDate);
                                    memberMessage.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, defaultExpiration);
                                    memberMessage.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
                                    mgr.CreateMemberMessage(memberMessage);
                                    memberMessageId = memberMessage.Id;
                                }

                                //Send Push notification if one is attached to the message definition
                                if (message.PushNotificationId != null)
                                {
                                    SendAssociatedPush(m, mgr, (long)message.PushNotificationId);
                                }

								results.Add(new CampaignResult(OutputType.Message, messageId, memberMessageId));
							}
						}
						break;

					case OutputType.Offer:
						foreach (long bonusId in refIds)
						{
							BonusDef bonus = content.GetBonusDef(bonusId);
							if (bonus == null)
							{
								throw new Exception("Cannot create bonuses because the bonus (" + bonusId.ToString() + ") could not be found. Please ensure that the bonus selected for the output step exists.");
							}
							if (bonus.Quota.HasValue && bonus.Completed.HasValue && bonus.Completed.Value >= bonus.Quota.Value)
							{
								//quota has been met for this bonus, so we won't assign it.
								continue;
							}

							if (bonus.SurveyId.HasValue)
							{
								CreateSurvey(bonus.SurveyId.Value, m.IpCode);
							}

							MemberBonus memberBonus = new MemberBonus();
							memberBonus.BonusDefId = bonusId;
							memberBonus.MemberId = m.IpCode;
                            memberBonus.StartDate = GetExpressionDate(OutputOption.StartDate, bonus.StartDate);
                            memberBonus.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, bonus.ExpiryDate);
							memberBonus.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
							mgr.CreateMemberOffer(memberBonus);
							results.Add(new CampaignResult(OutputType.Offer, bonusId, memberBonus.ID));
						}
						break;
					case OutputType.Promotion:
						foreach (long promotionId in refIds)
						{
							Promotion promotion = content.GetPromotion(promotionId);
							if (promotion == null)
							{
								throw new Exception("Cannot create promotions because the promotion (" + promotionId.ToString() + ") could not be found. Please ensure that the promotion selected for the output step exists.");
							}

							MemberPromotion memberPromotion = new MemberPromotion();
							memberPromotion.Code = promotion.Code;
							memberPromotion.MemberId = m.IpCode;
							if (OutputOption.UseCertificates)
							{
								memberPromotion.CertificateNmbr = GetNextCertificateNumber(content, ContentObjType.Promotion, promotion.Code);
							}
							mgr.CreateMemberPromotion(memberPromotion);
							results.Add(new CampaignResult(OutputType.Promotion, promotionId, memberPromotion.Id));
						}
						break;
					case OutputType.Survey:
						foreach (long surveyId in refIds)
						{
							long memberReferenceId = CreateSurvey(surveyId, m.IpCode);
							results.Add(new CampaignResult(OutputType.Survey, surveyId, memberReferenceId));
						}
						break;
					case OutputType.Email:
						foreach (long emailId in refIds)
						{
							using (var email =  TriggeredEmailFactory.Create(emailId))
							{
								var additionalFields = new Dictionary<string, string>();
								foreach (var field in co.Environment)
								{
									additionalFields.Add(field.Key, field.Value.ToString());
								}
								email.SendAsync(m, additionalFields).Wait();
								results.Add(new CampaignResult(OutputType.Email, emailId, 0));
							}
						}
						break;
                    case OutputType.Sms:
                        foreach (long smsId in refIds)
                        {
                            if (FollowsOptInConvention(OutputType.Sms))
							{
								//we're following the opt in convention, make sure the member has opted in, otherwise we quietly exit
								bool optedIn = false;
								var details = m.GetChildAttributeSets("MemberDetails", false);
								if (details != null && details.Count > 0)
								{
									var detail = details[0];
									optedIn = Convert.ToBoolean(detail.GetAttributeValue("SmsOptIn"));
								}
								if (optedIn)
								{
									using (var sms = new Brierley.FrameWork.Sms.TriggeredSms(LWConfigurationUtil.GetCurrentConfiguration(), smsId))
                                    {
                                        var additionalFields = new Dictionary<string, string>();
                                        foreach (var field in co.Environment)
                                        {
                                            additionalFields.Add(field.Key, field.Value.ToString());
                                        }
                                        sms.Send(m, additionalFields);
                                        results.Add(new CampaignResult(OutputType.Sms, smsId, 0));
                                    }
								}
							}
                        }
                        break;
					case OutputType.SocialMedia:
						{
							//First, make sure the member has a linked social network account matching the provider

							//todo: this needs some caching, we can't go to the database and deserialize this every time
							using (var social = LWDataServiceUtil.SocialServiceInstance())
							{
								var apps = social.GetSocialMediaApps();
								if (apps == null)
								{
									throw new Exception("No social media apps have been configured.");
								}

								var app = apps.Where(o => o.Name == OutputOption.SocialMediaAppName).FirstOrDefault();
								if (app == null)
								{
									throw new Exception(string.Format("Failed to load social media app. The app {0} does not exist.", OutputOption.SocialMediaAppName));
								}

								if (app.Type != SocialNetworkProviderType.Twitter && app.Type != SocialNetworkProviderType.Facebook)
								{
									throw new Exception(string.Format("Unsupported provider type for social media post. Supported types are Twitter and Facebook. Supplied type was ", app.Type.ToString()));
								}

								//No that we know the provider, check that the client follows the opt in convention and make sure the member has explicitly opted in to allow posting on their behalf
								if (FollowsOptInConvention(app.Type))
								{
									//we're following the opt in convention, make sure the member has opted in, otherwise we quietly exit
									bool optedIn = false;
									var details = m.GetChildAttributeSets("MemberDetails", false);
									if (details != null && details.Count > 0)
									{
										var detail = details[0];
										optedIn = Convert.ToBoolean(detail.GetAttributeValue(app.Type == SocialNetworkProviderType.Twitter ? "TwitterOptIn" : "FacebookOptIn"));
									}
									if (!optedIn)
									{
										//member has not opted in to allow us to post on their behalf. Exit.
										return results;
									}
								}

								MemberSocNet socNet = mgr.GetSocNetForMember(app.Type, m.IpCode);
								if (socNet == null)
								{
									//member does not have a social network account linked. Exit.
									return results;
								}

								string message = string.Empty;
								if (OutputOption.TextBlockId > 0)
								{
									var block = content.GetTextBlock(OutputOption.TextBlockId);
									if (block == null)
									{
										throw new Exception(string.Format("Failed to load text block. Text block with id {0} does not exist.", OutputOption.TextBlockId));
									}
									else
									{
										//in the future, we may want to add language support. 
										message = block.GetContent();
									}
								}
								else
								{
									message = OutputOption.Text;
								}

								if (string.IsNullOrWhiteSpace(message))
								{
									throw new Exception("Cannot send empty social media post");
								}

								//evaluate bScript expressions
								message = ExpressionUtil.ParseExpressions(message, co);

								if (string.IsNullOrWhiteSpace(message))
								{
									throw new Exception("Social media post bScript evaluation has resulted in an empty message that cannot be posted.");
								}

								//we have social network linkage and a message to post. We just need to post it.
								if (app.Type == SocialNetworkProviderType.Twitter)
								{
									if (Tweet(app, socNet, message))
									{
										results.Add(new CampaignResult(OutputType.SocialMedia, OutputOption.TextBlockId, -1));
									}
								}
								else if (app.Type == SocialNetworkProviderType.Facebook)
								{
									if (FacebookPost(app, socNet, message))
									{
										results.Add(new CampaignResult(OutputType.SocialMedia, OutputOption.TextBlockId, -1));
									}
								}
							}
						}
						break;
					case OutputType.NextBestAction:
						if (string.IsNullOrEmpty(OutputOption.Text))
						{
							throw new Exception("No expression was provided for number of actions to assign");
						}

						Expression exp = new ExpressionFactory().Create(OutputOption.Text);
						object result = exp.evaluate(co);
						if (result == null)
						{
							throw new Exception("expression provided for number of actions to assign has resulted in a null value");
						}

						int nbaCount = Convert.ToInt32(result);

						var nbas = mgr.AssignNextBestActions(
							m.IpCode,
							nbaCount,
							true,
							OutputOption.IncludedActionTypes,
							OutputOption.UseCertificates,
							GetDisplayOrder(OutputOption.DisplayOrder, null),
							GetExpressionDate(OutputOption.ExpirationDate, null));
						if (nbas != null)
						{
							Func<NextBestActionType, OutputType> getOutputType = delegate(NextBestActionType actionType)
							{
								OutputType ret = OutputType.NextBestAction;
								switch (actionType)
								{
									case NextBestActionType.Coupon:
										ret = OutputType.Coupon;
										break;
									case NextBestActionType.Message:
										ret = OutputType.Message;
										break;
								}
								return ret;
							};

							foreach (var nba in nbas)
							{
								results.Add(new CampaignResult(getOutputType(nba.ActionType), nba.ActionId, nba.MemberActionId.GetValueOrDefault()));
							}
						}
						break;
                    case OutputType.Notification:
                        {
                            foreach (long notificationId in refIds)
                            {
                                DateTime? defaultExpiration = null;
                                if (string.IsNullOrEmpty(OutputOption.ExpirationDate))
                                {
                                    NotificationDef notification = content.GetNotificationDef(notificationId);
                                    if (notification == null)
                                    {
                                        throw new Exception("Cannot send push notification because the notification (" + notificationId.ToString() + ") could not be found. Please ensure that the notification selected for the output step exists.");
                                    }
                                    defaultExpiration = notification.ExpiryDate;
                                }                              

                                List<MobileDevice> mobileDevices = mgr.GetMobileDevices(m, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = 1000 });
                                if (mobileDevices.Count > 0)
                                {
                                    foreach (MobileDevice mobileDevice in mobileDevices)
                                    {
                                        if (mobileDevice.AcceptsPush)
                                        {
                                            //Does Device have an active session
                                            PushSession activeSession = mgr.GetActivePushSessions(mobileDevice.Id);
                                            if (activeSession != null)
                                            {
                                                try
                                                {
                                                    using (var push = LWDataServiceUtil.PushServiceInstance())
                                                    {
                                                        push.Send(m, notificationId, mobileDevice.Id, null);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.Error(_className, "SendPushNotification", ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                                results.Add(new CampaignResult(OutputType.Notification, notificationId, m.IpCode));
                            }
                        }
                        break;
                    default:
						throw new Exception(string.Format("Cannot create output type {0} in real-time", OutputOption.OutputType.ToString()));
				}
				return results;
			}
		}

        private bool FollowsOptInConvention(OutputType outputType)
        {
            lock (_optInConvention)
            {
                if (outputType != null && _optInConvention.ContainsKey(outputType))
                {
                    return _optInConvention[outputType];
                }

                using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    AttributeSetMetaData meta = svc.GetAttributeSetMetaData("MemberDetails");
                    if (meta != null)
                    {
                        switch (outputType)
                        {
                            case OutputType.Sms:
                                AttributeMetaData smsOptIn = meta.GetAttribute("SmsOptIn");
                                AttributeMetaData SmsConsentChangeDate = meta.GetAttribute("SmsConsentChangeDate");
                                if (smsOptIn != null && smsOptIn.DataType == "Boolean")
                                {
                                    _optInConvention.Add(outputType, true);
                                    return true;
                                }
                                break;
                            default:
                                return true;
                                break;
                        }
                    }
                    _optInConvention.Add(outputType, false);
                    return false;
                }
            }
        }

		private bool FollowsOptInConvention(SocialNetworkProviderType providerType)
		{
			lock (_followsOptInConvention)
			{
				if (_followsOptInConvention != null && _followsOptInConvention.ContainsKey(providerType))
				{
					return _followsOptInConvention[providerType];
				}

                using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    AttributeSetMetaData meta = svc.GetAttributeSetMetaData("MemberDetails");
                    if (meta != null)
                    {
                        AttributeMetaData attMeta = meta.GetAttribute(providerType == SocialNetworkProviderType.Twitter ? "TwitterOptIn" : "FacebookOptIn");
                        if (attMeta != null && attMeta.DataType == "Boolean")
                        {
                            _followsOptInConvention.Add(providerType, true);
                            return true;
                        }
                    }
                    _followsOptInConvention.Add(providerType, false);
                    return false;
                }
			}
		}

		private long CreateSurvey(long surveyId, long ipCode)
		{
			using (var svc = LWDataServiceUtil.SurveyManagerInstance())
			{
				var survey = svc.RetrieveSurvey(surveyId);
				if (survey == null)
				{
					throw new Exception("Cannot create surveys because the survey (" + surveyId.ToString() + ") could not be found. Please ensure that the survey selected for the output step exists.");
				}

				SMLanguage english = svc.RetrieveLanguage("English");

				if (english != null)
				{
					SMRespondent respondent = new SMRespondent();
					respondent.CreateDate = DateTime.Today;
					respondent.IPCode = ipCode;
					respondent.LanguageID = english.ID;
					respondent.SurveyID = survey.ID;
					svc.CreateRespondent(respondent);
					return respondent.ID;
				}
				return 0;
			}
		}

		private bool EvaluateRow(int row, ContentDefBase outputDef, ContextObject co)
		{
			foreach (var qc in Columns)
			{
				foreach (var cc in qc.Conditions.Where(o => o.RowOrder == row))
				{
					if (!EvaluateCondition(qc.TableName, cc, outputDef, co))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool EvaluateCondition(string key, ColumnCondition condition, ContentDefBase outputDef, ContextObject co)
		{
			if (string.IsNullOrEmpty(condition.ConditionExpression))
			{
				return true;
			}

			if (_defs == null)
			{
				using (var content = LWDataServiceUtil.ContentServiceInstance())
				{
					_defs = (List<ContentAttributeDef>)content.GetAllContentAttributeDefs(outputDef.ContentType);
				}
			}

			if (_defs == null || _defs.Count == 0)
			{
				return false;
			}

			bool ret = false;
			string val = string.Empty;

			Expression exp = new ExpressionFactory().Create(condition.ConditionExpression);
			object result = exp.evaluate(co);
			if (result != null)
			{
				val = result.ToString();
			}

			long attId = 0;
			foreach (var def in _defs)
			{
				if (def.Name == key)
				{
					attId = def.ID;
					break;
				}
			}

			ContentAttribute attribute = outputDef.Attributes.Where(o => o.ContentAttributeDefId == attId).FirstOrDefault();
			if (attribute == null)
			{
				return false;
			}

			switch (condition.Operator)
			{
				case "<":
					ret = string.Compare(attribute.Value, val) == -1;
					break;
				case "<=":
					ret = string.Compare(attribute.Value, val) <= 0;
					break;
				case ">=":
					ret = string.Compare(attribute.Value, val) >= 0;
					break;
				case ">":
					ret = string.Compare(attribute.Value, val) == 1;
					break;
				case "<>":
					ret = attribute.Value != val;
					break;
				case "like":
				case "in":
					throw new Exception("Invalid operator " + condition.Operator);
				case "=":
				default:
					ret = val == attribute.Value;
					break;
			}
			return ret;
		}

		private List<int> GetConditionRows()
		{
			var rows = new List<int>();

			foreach (var qc in Columns)
			{
				foreach (var cc in qc.Conditions)
				{
					if (!rows.Contains(cc.RowOrder))
					{
						rows.Add(cc.RowOrder);
					}
				}
			}
			return rows;
		}

		private bool Tweet(SocialMediaApp app, MemberSocNet socNet, string message)
		{
			const string methodName = "Tweet";

			if (app == null)
			{
				throw new ArgumentNullException("app");
			}

			if (socNet == null)
			{
				throw new ArgumentNullException("socNet");
			}

			try
			{
				object twitter = ClassLoaderUtil.CreateInstance("Brierley.WebFrameWork", "Brierley.WebFrameWork.SocialNetwork.TwitterProvider+TwitterAccessToken");
				Type tokenType = twitter.GetType();
				object token = JsonConvert.DeserializeObject(socNet.Properties, tokenType);
				string oAuthToken = tokenType.GetProperty("oauth_token").GetValue(token) as string;
				string oAuthSecret = tokenType.GetProperty("oauth_token_secret").GetValue(token) as string;
				string userId = tokenType.GetProperty("user_id").GetValue(token) as string;
				string screenName = tokenType.GetProperty("screen_name").GetValue(token) as string;

				SingleUserAuthorizer authToken = new SingleUserAuthorizer()
				{
					Credentials = new InMemoryCredentials
					{
						ConsumerKey = app.ConsumerKey,
						ConsumerSecret = app.ConsumerSecret,
						OAuthToken = oAuthToken,
						AccessToken = oAuthSecret,
						ScreenName = screenName,
					}
				};
				authToken.Authorize();

				if (!authToken.IsAuthorized)
				{
					throw new Exception("access token is not authorized");
				}

				var twitterCtx = new TwitterContext(authToken);
				twitterCtx.UpdateStatus(message);

				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Format("Unable to send tweet for member {0}", socNet.MemberId), ex);
			}
			return false;
		}

		private bool FacebookPost(SocialMediaApp app, MemberSocNet socNet, string message)
		{
			const string methodName = "FacebookPost";

			if (app == null)
			{
				throw new ArgumentNullException("app");
			}

			if (socNet == null)
			{
				throw new ArgumentNullException("socNet");
			}


			try
			{
				string accessToken = string.Empty;
				object instance = ClassLoaderUtil.CreateInstance("Brierley.WebFrameWork", "Brierley.WebFrameWork.SocialNetwork.FBUser");
				Type tokenType = instance.GetType();
				object token = JsonConvert.DeserializeObject(socNet.Properties, tokenType);
				accessToken = tokenType.GetProperty("access_token").GetValue(token) as string;

				var client = new FacebookClient(accessToken);

				var parameters = new Dictionary<string, object>();
				parameters["message"] = message;
				parameters["data"] = "Message sent from my test program";
				var response = client.Post("me/feed", parameters);

				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Format("Unable to send Facebook post for member {0}", socNet.MemberId), ex);
			}
			return false;
		}

        private void SendAssociatedPush(Member member, LoyaltyDataService loyalty, long pushNotificationId)
        {
            string methodName = "SendAssociatedPush";
            List<MobileDevice> mobileDevices = loyalty.GetMobileDevices(member, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = 1000 });
            if (mobileDevices.Count > 0)
            {
                foreach (MobileDevice mobileDevice in mobileDevices)
                {
                    if (mobileDevice.AcceptsPush)
                    {
                        //Does Device have an active session
                        PushSession activeSession = loyalty.GetActivePushSessions(mobileDevice.Id);
                        if (activeSession != null)
                        {
                            try
                            {
                                using (var push = LWDataServiceUtil.PushServiceInstance())
                                {
                                    push.Send(member, pushNotificationId, mobileDevice.Id, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(_className, methodName, ex.Message);
                            }
                        }
                    }
                }
            }
        }
	}
}