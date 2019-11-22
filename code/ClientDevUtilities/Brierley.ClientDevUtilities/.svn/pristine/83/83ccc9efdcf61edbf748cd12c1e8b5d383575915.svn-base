using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Security;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class OutputQuery : Query
	{
		private const string _className = "OutputQuery";
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);

		private string _evaluatedFileLocation = null;
		private string _evaluatedTriggerFileLocation = null;

		[System.Xml.Serialization.XmlElement(/*Order = 15*/)]
		public virtual OutputOption OutputOption { get; set; }

		public override List<SqlStatement> GetSqlStatement(bool isValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                string altSchemaName = manager.BatchProvider.DataSchemaPrefix;
                if (!string.IsNullOrEmpty(altSchemaName))
                {
                    if (!altSchemaName.EndsWith("."))
                    {
                        altSchemaName += ".";
                    }
                }

                base.ApplyTableNames();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Output;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;

                //begin select specific logic

                if (Step.StepType == StepType.Output)
                {
                    sqlQuery.RootTableName = RootTable.Name;
                    if (!isValidationTest)
                    {
                        sqlQuery.ActionTable = Step.OutputTableName;
                    }
                    sqlQuery.Columns = Columns;

                    sqlQuery.CreateTable = !isValidationTest;
                    //we always need to create this because it can't be created in VerifySchema
                    //because we don't actually know the schema, since the user is selecting any number/combination of
                    //fields for output
                }
                //end select specific logic

                MapTableJoins(sqlQuery);

                //every formula column will need a name. Assign any that are missing, if possible:
                foreach (var c in Columns.Where(o => o.IncludeInOutput && string.IsNullOrWhiteSpace(o.OutputAs)))
                {
                    c.OutputAs = GetOutputAsFieldName(c);
                }

                if (isValidationTest)
                {
                    //override any existing limit and force it to 0, if we're validating.
                    sqlQuery.Limit = 0;
                    sqlQuery.IsLimitPercentage = false;
                }

                List<SqlStatement> statements = new List<SqlStatement>();
                List<string> sqlStatements = manager.BatchProvider.CreateSqlStatement(sqlQuery);
                foreach (SqlStatement statement in sqlStatements)
                {
                    ApplyCampaignParameters(statement, overrideParameters);
                    statements.Add(statement);
                }

                return statements;
            }
		}

		/// <summary>
		/// Assembles the SQL statement for the verification (commit) process of the step
		/// </summary>
		/// <returns></returns>
		public override List<SqlStatement> GetVerifySqlStatement()
		{
			//basically, all we really need here is a "select * from <output table name>", but we'll whittle it down to just 
			//the key and ipcode for promotions/coupons/bonuses/surveys and we may have to join back to the input table to get
			//the key for the history table

            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                var sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Output;
                sqlQuery.RootTableName = this.Step.OutputTableName;

                if (
                    OutputOption.OutputType == OutputType.Coupon ||
                    OutputOption.OutputType == OutputType.Offer ||
                    OutputOption.OutputType == OutputType.Promotion ||
                    OutputOption.OutputType == OutputType.Survey ||
                    OutputOption.OutputType == OutputType.Message ||
                    OutputOption.OutputType == OutputType.Notification
                    )
                {
                    //all we need to retrieve is the key (for history) and the ipcode (for creating the output)
                    string keyName = (from x in Columns where x.TableId == Key.TableId && x.FieldName == Key.FieldName && x.IncludeInOutput == true select string.IsNullOrEmpty(x.OutputAs) ? x.FieldName : x.OutputAs).FirstOrDefault();
                    if (string.IsNullOrEmpty(keyName))
                    {
                        keyName = Key.FieldName;
                    }
                    if (keyName.ToLower() != "ipcode")
                    {
                        sqlQuery.SelectFieldList.Add("IPCode");
                    }
                    sqlQuery.SelectFieldList.Add(keyName);
                }
                else
                {
                    //we need everything that's included in the output
                    foreach (var c in from x in Columns where x.IncludeInOutput == true select GetOutputAsFieldName(x, true))
                    {
                        sqlQuery.InsertFieldList.Add(c);
                        sqlQuery.SelectFieldList.Add(c);
                    }
                    if (OutputOption.OutputType == OutputType.Table)
                    {
                        sqlQuery.CreateTable = OutputOption.CreateTable;
                        sqlQuery.ActionTable = OutputOption.TableName;
                    }
                }

                return new List<SqlStatement>() { manager.BatchProvider.CreateSqlStatement(sqlQuery)[0] };
            }
		}


		public override bool Validate(List<ValidationMessage> warnings, bool validateSql)
		{
			bool fatal = false;

			//Check for usage of parameters. Parameters cannot be used against Oracle for output steps because the output step is executed
			//as a DDL statement ("create table x as ..."), and Oracle does not allow parameters in DDL statements.
			using(var manager = LWDataServiceUtil.CampaignManagerInstance())
			if (manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
			{
				var parametersUsed = new List<string>();
				IList<Attribute> attributes = manager.GetAllAttributes();
				if (attributes != null && attributes.Count > 0)
				{
					Func<string, IEnumerable<Attribute>, bool> hasParameter = delegate(string expression, IEnumerable<Attribute> parameters)
					{
						foreach (var p in parameters)
						{
							if (expression.IndexOf(string.Format("@\"{0}\"", p.Name), StringComparison.OrdinalIgnoreCase) > -1)
							{
								return true;
							}
						}
						return false;
					};

					foreach (var col in Columns)
					{
						if (hasParameter(col.ColumnExpression, attributes))
						{
							parametersUsed.Add(col.ColumnExpression);
						}
						foreach (var condition in col.Conditions)
						{
							if (hasParameter(condition.ConditionExpression, attributes))
							{
								parametersUsed.Add(condition.ConditionExpression);
							}
						}
					}
				}

				if (parametersUsed.Count > 0)
				{
					fatal = true;
					string error = "Campaign attributes may not be used as parameters for output steps. The following expressions were detected as having parameters:\r\n" + string.Join("\r\n", parametersUsed);
					warnings.Add(new ValidationMessage(ValidationLevel.Exception, error));
					return false;
				}
			}



			//need a key in order to perform this one
			if (Key == null)
			{
				warnings.Add(new ValidationMessage(ValidationLevel.Exception, "No key exists for query " + Step.UIName));
				fatal = true;
			}
			else
			{
				if (
					OutputOption != null &&
					(
						OutputOption.OutputType == OutputType.Promotion ||
						OutputOption.OutputType == OutputType.Coupon ||
						OutputOption.OutputType == OutputType.Message ||
						OutputOption.OutputType == OutputType.Offer ||
						OutputOption.OutputType == OutputType.Survey ||
                        OutputOption.OutputType == OutputType.Notification
                    )
					)
				{
					bool hasIPCode = false;
					if (Key.FieldName.ToLower() == "ipcode")
					{
						hasIPCode = true;
					}
					else
					{
						foreach (QueryColumn column in this.Columns)
						{
							if (column.IncludeInOutput && column.OutputAs.ToLower() == "ipcode")
							{
								hasIPCode = true;
								break;
							}
						}
					}
					if (!hasIPCode)
					{
						warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" cannot create a promotion because the key is invalid. A member level key (IPCode) is required in order to create promotions.", Step.UIName)));
						fatal = true;
					}
				}
			}


			if (!fatal)
			{
				return base.Validate(warnings, validateSql);
			}
			else
			{
				return false;
			}
		}


		public override int Verify(ContextObject co = null, Dictionary<string, string> overrideParameters = null)
		{
			const string methodName = "Verify";
			Step.UILastStatus = "Verifying...";

			int rowCount = 0;

			using (var manager = LWDataServiceUtil.CampaignManagerInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
                manager.UpdateStep(Step);
				if (OutputOption.OutputType == OutputType.Table)
				{
					bool tableExists = manager.BatchProvider.TableExists(OutputOption.TableName, false);
					if (!tableExists && OutputOption.CreateTable == false)
					{
						throw new Exception("Cannot execute query because the output table does not exist.");
					}
					if (tableExists && OutputOption.AppendData == false)
					{
						_logger.Debug(_className, methodName, string.Format("Truncating output table for step {0}", Step.Id));
						Step.UILastStatus = "Truncating output table...";
						manager.UpdateStep(Step);

						manager.BatchProvider.TruncateTable(OutputOption.TableName);
					}
					else
					{
						//this is likely the first time the query is being executed, so we'll try to map the table in CampaignManagement.
						foreach (CampaignTable table in manager.GetAllCampaignTables())
						{
							if (table.Name.ToLower() == OutputOption.TableName.ToLower())
							{
								tableExists = true;
								break;
							}
						}
						if (!tableExists)
						{
							_logger.Debug(_className, methodName, string.Format("Creating new campaign table for step {0}", Step.Id));

							Step.UILastStatus = "Creating new campaign table...";
							manager.UpdateStep(Step);

							CampaignTable table = new CampaignTable(OutputOption.TableName, TableType.Output);
							manager.CreateCampaignTable(table);


							foreach (QueryColumn column in Columns.Where(o => o.IncludeInOutput))
							{
								string fieldName = column.ColumnExpression ?? column.FieldName;
								if (fieldName != null && fieldName.Contains("."))
								{
									fieldName = fieldName.Substring(fieldName.LastIndexOf(".") + 1);
								}
								if (string.IsNullOrWhiteSpace(fieldName))
								{
									continue;
								}

								if (fieldName.Equals(Key.FieldName, StringComparison.OrdinalIgnoreCase))
								{
									//the root key exists in the output table. We can map a key to the table
									TableKey key = new TableKey();
									key.AudienceId = Key.AudienceId;
									key.FieldName = string.IsNullOrWhiteSpace(column.OutputAs) ? Key.FieldName : column.OutputAs;
									key.TableId = table.Id;
									key.FieldType = Key.FieldType;
									manager.CreateTableKey(key);
									break;
								}
							}
						}
					}

					_logger.Debug(_className, methodName, string.Format("Executing database query for step {0}", Step.Id));
					Step.UILastStatus = "Executing database query...";
					manager.UpdateStep(Step);

					//"create table as" statement does not return row count in Oracle, so execute and then get the count from the table:
					manager.BatchProvider.Execute(GetVerifySqlStatement()[0].ToString(), null);
					if (OutputOption.CreateTable)
					{
						//table now exists and cannot be created a second time. 
						OutputOption.CreateTable = false;

						foreach (QueryColumn column in Columns)
						{
							if (column.IncludeInOutput && string.IsNullOrEmpty(column.OutputAs))
							{
								column.OutputAs = column.FieldName;
							}
						}
						manager.UpdateStep(Step);
					}
					rowCount = manager.BatchProvider.RowCountExact(OutputOption.TableName);
				}
				else
				{
					//the promotion output option can be handled in one of two ways: 
					//1. Set the lw_promotion table as the action table of an insert query, and pass control off to the data provider to execute.
					//	   ** This can only be accomplished if the campaign is set to run in the framework database, which may not always be the case.
					//2. Pass the query as a select query and read each record, assigning a promotion to each member. More control is gained by this 
					//   method, which is implemented here, although it doesn't perform as fast as a stright insert into a table...

					if (
						OutputOption.OutputType == OutputType.Coupon ||
						OutputOption.OutputType == OutputType.Offer ||
						OutputOption.OutputType == OutputType.Promotion ||
						OutputOption.OutputType == OutputType.Survey ||
						OutputOption.OutputType == OutputType.Message ||
						OutputOption.OutputType == OutputType.NextBestAction ||
                        OutputOption.OutputType == OutputType.Notification
                        )
					{
						_logger.Debug(_className, methodName, string.Format("Excuting database query for step {0}", Step.Id));

						Step.UILastStatus = "Executing database query...";
						manager.UpdateStep(Step);

						DataTable table = manager.BatchProvider.ExecuteDataTable(GetVerifySqlStatement()[0].ToString());


						//these output types all require IPCode. Check for IPCode field on the results table
						bool ipCodeExists = false;
						foreach (DataColumn column in table.Columns)
						{
							if (column.ColumnName.ToLower() == "ipcode")
							{
								ipCodeExists = true;
								break;
							}
						}
						if (!ipCodeExists)
						{
							throw new Exception("Cannot create " + OutputOption.OutputType.ToString() + " because the result set does not contain an IPCode field.");
						}


						if (OutputOption.OutputType == OutputType.Promotion)
						{
							foreach (long promotionId in OutputOption.RefId)
							{
								Promotion promotion = content.GetPromotion(promotionId);
								if (promotion == null)
								{
									throw new Exception("Cannot create promotions because the promotion (" + promotionId.ToString() + ") could not be found. Please ensure that the promotion selected for the output step exists.");
								}

								_logger.Debug(_className, methodName, string.Format("Assigning promotions to members for step {0}", Step.Id));

								Step.UILastStatus = "Assigning promotions to members...";
								manager.UpdateStep(Step);

								if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
								{
									manager.BatchProvider.BulkOutputProvider.OutputPromotions(table, promotion.Code, LWDataServiceUtil.GetServiceConfiguration(), OutputOption.UseCertificates);
								}
								else
								{
									foreach (DataRow row in table.Rows)
									{
										MemberPromotion memberPromotion = new MemberPromotion();
										memberPromotion.Code = promotion.Code;
										memberPromotion.MemberId = long.Parse(row["IPCode"].ToString());
										if (OutputOption.UseCertificates)
										{
											memberPromotion.CertificateNmbr = GetNextCertificateNumber(content, ContentObjType.Promotion, promotion.Code);
										}
										loyalty.CreateMemberPromotion(memberPromotion);
									}
								}
							}
						}
						else if (OutputOption.OutputType == OutputType.Offer)
						{
							_logger.Debug(_className, methodName, string.Format("Assigning bonuses to members for step {0}", Step.Id));

							Step.UILastStatus = "Assigning bonuses to members...";
							manager.UpdateStep(Step);
							using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
							{
								foreach (long bonusId in OutputOption.RefId)
								{
									BonusDef bonus = content.GetBonusDef(bonusId);
									if (bonus == null)
									{
										throw new Exception("Cannot create bonuses because the bonus (" + bonusId.ToString() + ") could not be found. Please ensure that the bonus selected for the output step exists.");
									}

									long surveyId = -1;
									long languageId = -1;
									if (bonus.SurveyId.HasValue)
									{

										SMSurvey survey = surveyManager.RetrieveSurvey(bonus.SurveyId.Value);
										if (survey != null)
										{
											SMLanguage english = surveyManager.RetrieveLanguage("English");
											if (english != null)
											{
												languageId = english.ID;
												surveyId = survey.ID;
											}

										}
									}

									if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
									{
										manager.BatchProvider.BulkOutputProvider.OutputBonuses(
											table,
											bonusId,
											surveyId,
											languageId,
                                            GetExpressionDate(OutputOption.StartDate, bonus.StartDate),
                                            GetExpressionDate(OutputOption.ExpirationDate, bonus.ExpiryDate),
											GetDisplayOrder(OutputOption.DisplayOrder, null),
											LWDataServiceUtil.GetServiceConfiguration());
									}
									else
									{
										foreach (DataRow row in table.Rows)
										{
											if (surveyId > 0 && languageId > 0)
											{
												SMRespondent respondent = new SMRespondent();
												respondent.CreateDate = DateTime.Today;
												respondent.IPCode = long.Parse(row["IPCode"].ToString());
												respondent.LanguageID = languageId;
												respondent.SurveyID = surveyId;

												surveyManager.CreateRespondent(respondent);
											}

											MemberBonus memberBonus = new MemberBonus();
											memberBonus.BonusDefId = bonusId;
											memberBonus.MemberId = long.Parse(row["IPCode"].ToString());
											memberBonus.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, bonus.ExpiryDate);
											memberBonus.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
											loyalty.CreateMemberOffer(memberBonus);
										}
									}
								}
							}
						}
						else if (OutputOption.OutputType == OutputType.Survey)
						{
							_logger.Debug(_className, methodName, string.Format("Assigning surveys to members for step {0}", Step.Id));

							Step.UILastStatus = "Assigning surveys to members...";
							manager.UpdateStep(Step);

							foreach (long surveyId in OutputOption.RefId)
							{
								using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
								{
									SMSurvey survey = surveyManager.RetrieveSurvey(surveyId);
									if (survey == null)
									{
										throw new Exception("Cannot create surveys because the survey (" + surveyId.ToString() + ") could not be found. Please ensure that the survey selected for the output step exists.");
									}

									SMLanguage english = surveyManager.RetrieveLanguage("English");

									if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
									{
										manager.BatchProvider.BulkOutputProvider.OutputSurveys(table, survey.ID, english.ID, LWDataServiceUtil.GetServiceConfiguration());
									}
									else
									{
										foreach (DataRow row in table.Rows)
										{
											SMRespondent respondent = new SMRespondent();
											respondent.CreateDate = DateTime.Today;
											respondent.IPCode = long.Parse(row["IPCode"].ToString());
											respondent.LanguageID = english.ID;
											respondent.SurveyID = survey.ID;
											surveyManager.CreateRespondent(respondent);
										}
									}
								}
							}
						}
						else if (OutputOption.OutputType == OutputType.Coupon)
						{
							_logger.Debug(_className, methodName, string.Format("Assigning coupons to members for step {0}", Step.Id));

							Step.UILastStatus = "Assigning coupons to members...";
							manager.UpdateStep(Step);

							foreach (long couponId in OutputOption.RefId)
							{
								CouponDef coupon = content.GetCouponDef(couponId);
								if (coupon == null)
								{
									throw new Exception("Cannot create coupons because the coupon (" + couponId.ToString() + ") could not be found. Please ensure that the coupon selected for the output step exists.");
								}

								if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
								{
									manager.BatchProvider.BulkOutputProvider.OutputCoupons(
										table,
										couponId,
                                        GetExpressionDate(OutputOption.StartDate, coupon.StartDate),
                                        GetExpressionDate(OutputOption.ExpirationDate, coupon.ExpiryDate),
										GetDisplayOrder(OutputOption.DisplayOrder, null),
										LWDataServiceUtil.GetServiceConfiguration(),
										OutputOption.UseCertificates,
										coupon.CouponTypeCode);
								}
								else
								{
									foreach (DataRow row in table.Rows)
									{
										MemberCoupon memberCoupon = new MemberCoupon();
										memberCoupon.CouponDefId = couponId;
										memberCoupon.MemberId = long.Parse(row["IPCode"].ToString());
										memberCoupon.DateIssued = DateTime.Now;
										memberCoupon.TimesUsed = 0;
                                        memberCoupon.StartDate = GetExpressionDate(OutputOption.StartDate, coupon.StartDate);
                                        memberCoupon.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, coupon.ExpiryDate);
										memberCoupon.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
										if (OutputOption.UseCertificates)
										{
											memberCoupon.CertificateNmbr = GetNextCertificateNumber(content, ContentObjType.Coupon, coupon.CouponTypeCode);
										}
										loyalty.CreateMemberCoupon(memberCoupon);

                                        //Send Push notification if one is attached to the coupon definition
                                        if (coupon.PushNotificationId != null)
                                        {
                                            SendAssociatedPush(loyalty, long.Parse(row["IPCode"].ToString()), (long)coupon.PushNotificationId);
                                        }
									}
								}
							}
						}
						else if (OutputOption.OutputType == OutputType.Message)
						{
							_logger.Debug(_className, methodName, string.Format("Assigning messages to members for step {0}", Step.Id));

							Step.UILastStatus = "Assigning messages to members...";
							manager.UpdateStep(Step);

							foreach (long messageId in OutputOption.RefId)
							{
								MessageDef message = content.GetMessageDef(messageId);
								if (message == null)
								{
									throw new Exception("Cannot create messages because the message (" + messageId.ToString() + ") could not be found. Please ensure that the message selected for the output step exists.");
								}

								if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
								{
									manager.BatchProvider.BulkOutputProvider.OutputMessages(
										table,
										messageId,
                                        GetExpressionDate(OutputOption.StartDate, message.StartDate),
                                        GetExpressionDate(OutputOption.ExpirationDate, message.ExpiryDate),
										GetDisplayOrder(OutputOption.DisplayOrder, null),
										LWDataServiceUtil.GetServiceConfiguration());
								}
								else
								{
									foreach (DataRow row in table.Rows)
									{
										MemberMessage memberMessage = new MemberMessage();
										memberMessage.MessageDefId = message.Id;
										memberMessage.MemberId = long.Parse(row["IPCode"].ToString());
										memberMessage.DateIssued = DateTime.Now;
                                        memberMessage.StartDate = GetExpressionDate(OutputOption.StartDate, message.StartDate);
										memberMessage.ExpiryDate = GetExpressionDate(OutputOption.ExpirationDate, message.ExpiryDate);
										memberMessage.DisplayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);
										loyalty.CreateMemberMessage(memberMessage);

                                        //Send Push notification if one is attached to the message definition
                                        if (message.PushNotificationId != null)
                                        {
                                            SendAssociatedPush(loyalty, long.Parse(row["IPCode"].ToString()), (long)message.PushNotificationId);
                                        }
									}
								}
							}
						}
						else if (OutputOption.OutputType == OutputType.NextBestAction)
						{
							_logger.Debug(_className, methodName, "assigning next best actions");
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
							DateTime? expiration = GetExpressionDate(OutputOption.ExpirationDate, null);
							int? displayOrder = GetDisplayOrder(OutputOption.DisplayOrder, null);

							if (manager.BatchProvider.UseArrayBinding && manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
							{
								int batchSize = manager.BatchProvider.BulkOutputProvider.BatchSize;
								_logger.Debug(_className, methodName, "Using array binding to output next best action list");
								//structure to hold member next best actions by 
								var buffer = new Dictionary<Tuple<NextBestActionType, long>, List<MemberNextBestAction>>();
								Action<Tuple<NextBestActionType, long>> flush = delegate(Tuple<NextBestActionType, long> key)
								{
									_logger.Debug(_className, methodName, string.Format("Flushing buffer for key {0}, {1}", key.Item1.ToString(), key.Item2.ToString()));
									string couponCode = null;
									if (key.Item1 == NextBestActionType.Coupon && OutputOption.UseCertificates)
									{
										var coupon = content.GetCouponDef(key.Item2);
										if (coupon == null)
										{
											//we should never land here - business logic before this method ensures the items exist.
											//**just in case** the item is deleted between there and here, let's prevent the null pointer exception:
											throw new Exception("Could not load coupon for next best action with coupon id " + key.Item2.ToString());
										}
										couponCode = coupon.CouponTypeCode;
									}

									manager.BatchProvider.BulkOutputProvider.OutputNextBestActions(
										buffer[key],
                                        GetExpressionDate(OutputOption.StartDate, null), 
										expiration,
										displayOrder,
										LWDataServiceUtil.GetServiceConfiguration(),
										OutputOption.UseCertificates,
										couponCode);

									buffer[key].Clear();
								};

								int index = 0;
								foreach (DataRow row in table.Rows)
								{
									long ipcode = Convert.ToInt64(row["IPCode"]);

									//if this ipcode is already in the buffer, we need to flush before getting next best actions.
									//Otherwise, we'll get a duplicate list of actions.
									var keysWithMember = buffer.Keys.Where(o => buffer[o].Where(x => x.MemberId == ipcode).Count() > 0).ToList();
									if (keysWithMember.Count() > 0)
									{
										foreach (var key in keysWithMember)
										{
											_logger.Debug(_className, methodName, string.Format("IPCode {0} is already in the buffer. Flushing to prevent duplicate actions", ipcode.ToString()));
											flush(key);
										}
									}

									var mnbas = loyalty.AssignNextBestActions(
										long.Parse(row["IPCode"].ToString()),
										nbaCount,
										false,
										OutputOption.IncludedActionTypes,
										OutputOption.UseCertificates,
										displayOrder,
										expiration);

									foreach (var mnba in mnbas)
									{
										List<MemberNextBestAction> list = null;
										var key = new Tuple<NextBestActionType, long>(mnba.ActionType, mnba.ActionId);
										if (!buffer.ContainsKey(key))
										{
											list = new List<MemberNextBestAction>();
											buffer.Add(key, list);
										}
										else
										{
											list = buffer[key];
											if (list.Count >= batchSize)
											{
												flush(key);
											}
										}
										list.Add(mnba);
									}
								}

								//write any remaining data in the buffer
								foreach (var key in buffer.Keys)
								{
									_logger.Debug(_className, methodName, "Flushing remaining data from next best action buffer.");
									flush(key);
								}
							}
							else
							{
								_logger.Debug(_className, methodName, "Writing next best actions without array binding.");
								foreach (DataRow row in table.Rows)
								{
									var nbas = loyalty.AssignNextBestActions(
										long.Parse(row["IPCode"].ToString()),
										nbaCount,
										true,
										OutputOption.IncludedActionTypes,
										OutputOption.UseCertificates,
										displayOrder,
										expiration);
								}
							}
						}
                        else if (OutputOption.OutputType == OutputType.Notification)
                        {
                            _logger.Debug(_className, methodName, string.Format("Sending push notifications to members for step {0}", Step.Id));

                            Step.UILastStatus = "Sending push notifications to members...";
                            manager.UpdateStep(Step);

                            foreach (long notificationId in OutputOption.RefId)
                            {
                                NotificationDef notification = content.GetNotificationDef(notificationId);
                                if (notification == null)
                                {
                                    throw new Exception("Cannot send push notification because the notification (" + notificationId.ToString() + ") could not be found. Please ensure that the notification selected for the output step exists.");
                                }

                                foreach (DataRow row in table.Rows)
                                {
                                    SendAssociatedPush(loyalty, long.Parse(row["IPCode"].ToString()), notification.Id);                                    
                                }
                            }
                        }

                        rowCount = table.Rows.Count;
					}
					else if (OutputOption.OutputType == OutputType.FlatFile)
					{
						_logger.Debug(_className, methodName, string.Format("Writing output file for step {0}", Step.Id));

						Step.UILastStatus = "Writing file...";
						manager.UpdateStep(Step);

						string fileName = GetTempFileName();
						try
						{
							_evaluatedFileLocation = ExpressionUtil.ParseExpressions(OutputOption.FileLocation ?? string.Empty, co);
							_evaluatedTriggerFileLocation = ExpressionUtil.ParseExpressions(OutputOption.SftpTriggerFile ?? string.Empty, co);

							using (IDataReader reader = manager.BatchProvider.ExecuteReader(GetVerifySqlStatement()[0].ToString()))
							{
								using (FileStream file = File.Create(fileName))
								{
									string rowDelimiter = null;
									string columnDelimiter = null;
									string textQualifier = null;
									bool includeColumnNames = false;
									int memberIdIndex = -1;

									includeColumnNames = OutputOption.IncludeColumnNames;
									rowDelimiter = ApplyEscapeSequences(OutputOption.RowDelimiter);
									columnDelimiter = ApplyEscapeSequences(OutputOption.ColumnDelimiter);
									textQualifier = ApplyEscapeSequences(OutputOption.TextQualifier);

									using (StreamWriter writer = new StreamWriter(file, System.Text.Encoding.UTF8))
									{
										if (includeColumnNames)
										{
											for (int i = 0; i < reader.FieldCount; i++)
											{
												string columnName = string.Empty;

												Func<string, string> stripQuotes = delegate(string val)
												{
													if (val != null && val.StartsWith("\"") && val.EndsWith("\"") && val.Length > 2)
													{
														return val.Substring(1, val.Length - 2);
													}
													return val;
												};

												//convert column name from upper case back to the case provided by the user. Oracle thinks it 
												//needs to scream everything and therefore converts all object names to upper case.
												foreach (QueryColumn column in Columns)
												{
													if (column.IncludeInOutput && !string.IsNullOrEmpty(column.OutputAs)
														&& stripQuotes(column.GetOutputAsToken(Columns)).Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase))
													{
														columnName = stripQuotes(column.OutputAs);
														break;
													}
												}

												//"OutputAs" was not provided, so the column name is the name of the field or its alias, if given:
												if (string.IsNullOrEmpty(columnName))
												{
													foreach (QueryColumn column in Columns)
													{
														if (column.IncludeInOutput && column.FieldName.ToLower() == reader.GetName(i).ToLower())
														{
															columnName = column.FieldName;
															break;
														}
													}
												}

												writer.Write(textQualifier + columnName + textQualifier);
												if (i < reader.FieldCount - 1)
												{
													writer.Write(columnDelimiter);
												}
											}
											writer.Write(rowDelimiter);
										}

                                        // Store which fields are encrypted
                                        List<CampaignTable> tables;
                                        using (var campaignManager = LWDataServiceUtil.CampaignManagerInstance())
                                        {
                                            tables = campaignManager.GetAllCampaignTables();
                                            foreach (var table in tables)
                                                table.Fields = campaignManager.GetTableFields(table.Id);
                                        }

                                        AttributeEncryptionType[] encryptionTypes = Columns.Where(x => x.IncludeInOutput).Select(
                                            x => string.IsNullOrEmpty(OutputOption.PgpName) ? AttributeEncryptionType.None :
                                                 GetAttributeEncryptionType(x, tables)).ToArray();
                                        LWKeystore keystore = LWConfigurationUtil.GetCurrentConfiguration().LWKeystore;


                                        while (reader.Read())
										{
											rowCount++;
                                            string value;

											for (int i = 0; i < reader.FieldCount; i++)
											{
                                                value = reader[i].ToString();

                                                if (!string.IsNullOrWhiteSpace(value))
                                                {
                                                    switch (encryptionTypes[i])
                                                    {
                                                        case AttributeEncryptionType.Asymmetric:
                                                            value = CryptoUtil.DecryptAsymmetricUTF8(keystore, value);
                                                            break;

                                                        case AttributeEncryptionType.Encoded:
                                                            value = CryptoUtil.DecodeUTF8(value);
                                                            break;

                                                        case AttributeEncryptionType.Symmetric:
                                                            value = CryptoUtil.Decryptv2(keystore, value);
                                                            break;
                                                    }
                                                }

												writer.Write(textQualifier + value + textQualifier);
												if (i < reader.FieldCount - 1)
												{
													writer.Write(columnDelimiter);
												}
											}
											writer.Write(rowDelimiter);

											if (rowCount % 1000 == 0)
											{
												Step.UILastStatus = string.Format("Writing file ({0:#,#} records)...", rowCount);
												manager.UpdateStep(Step);
											}
										}
                                        writer.Flush();
										writer.Close();
									}

									file.Close();
								}
							}

							if (OutputOption.OutputType == OutputType.FlatFile && !string.IsNullOrEmpty(OutputOption.PgpName))
							{
								_logger.Debug(_className, methodName, string.Format("Encrypting output file for step {0}", Step.Id));

								Step.UILastStatus = "Encrypting the output file...";
								manager.UpdateStep(Step);

								CryptoUtil.PGPEncrypt(OutputOption.PgpName, fileName);
							}

							if (!string.IsNullOrEmpty(OutputOption.SftpName))
							{
								using (var mgr = LWDataServiceUtil.DataServiceInstance())
								{
									string json = mgr.GetClientConfigProp("SFTPKeys");
									if (string.IsNullOrWhiteSpace(json))
									{
										throw new Exception(string.Format("Failed to load SFTP configuration for {0}. There are no SFTP keys configured.", OutputOption.SftpName));
									}
									SFTPKeyInfoCollection keys = JsonConvert.DeserializeObject<SFTPKeyInfoCollection>(json);
									if (!keys.ContainsKey(OutputOption.SftpName))
									{
										throw new Exception(string.Format("Failed to load SFTP configuration for {0}. The specified key does not exist.", OutputOption.SftpName));
									}
									SFTPKeyInfo key = keys[OutputOption.SftpName];

									_logger.Debug(_className, methodName, string.Format("Transferring output file via SFTP for step {0}", Step.Id));

									Step.UILastStatus = "Transferring the file (SFTP) to its destination...";
									manager.UpdateStep(Step);

									//move the file to SFTP server
									using (var sftp = new FrameWork.Common.Security.Sftp())
									{
										sftp.Hostname = key.HostName;
										sftp.Portnumber = key.PortNumber;
										sftp.Login = key.UserName;
										if (!string.IsNullOrEmpty(key.EncodedPassword))
										{
											sftp.Password = CryptoUtil.DecodeUTF8(key.EncodedPassword);
										}

										if (key.Key != null)
										{
											KeyManager km = new KeyManager();
											km.Load(new MemoryStream(CryptoUtil.Decode(key.Key.EncodedKey)));
											sftp.PrivateKey = km.PrivateKey();
										}

										sftp.Connect();

										sftp.RemotePath = _evaluatedFileLocation;
										System.IO.FileStream fs = new FileStream(fileName, FileMode.Open);
										sftp.PutFile(fs);
										fs.Close();

										if (!string.IsNullOrEmpty(_evaluatedTriggerFileLocation))
										{
											sftp.RemotePath = _evaluatedTriggerFileLocation;
											sftp.PutFile(new MemoryStream());
										}
									}
								}
							}
							else if (OutputOption.OutputType == OutputType.FlatFile && string.IsNullOrEmpty(OutputOption.SftpName))
							{
								_logger.Debug(_className, methodName, string.Format("Moving output file to its destination for step {0}", Step.Id));

								Step.UILastStatus = "Moving the file to its destination...";
								manager.UpdateStep(Step);

								if (System.IO.File.Exists(_evaluatedFileLocation))
								{
									System.IO.File.Delete(_evaluatedFileLocation);
								}
								System.IO.File.Move(fileName, _evaluatedFileLocation);
							}
						}
						catch
						{
							throw;
						}
						finally
						{
							//delete the temp file
							try
							{
								if (System.IO.File.Exists(fileName))
								{
									File.Delete(fileName);
								}
							}
							catch (Exception ex)
							{
								_logger.Warning(_className, methodName, string.Format("Unable to delete campaign output temp file", fileName ?? string.Empty), ex);
							}
						}
					}
				}

				//Add table's output to LW_CLHistory table
				_logger.Debug(_className, methodName, string.Format("Creating output history records for step {0}", Step.Id));

				Step.UILastStatus = "Creating output history records...";
				manager.UpdateStep(Step);

				if (!manager.BatchProvider.TableExists(Constants.CampaignHistoryTableName, false))
				{
					manager.BatchProvider.CreateCampaignHistoryTable();
				}

				string keyFieldName = string.Empty;
				foreach (var column in Columns)
				{
					if (column.IncludeInOutput && column.TableId == Key.TableId && column.FieldName == Key.FieldName)
					{
						keyFieldName = string.IsNullOrEmpty(column.OutputAs) ? Key.FieldName : column.OutputAs;
						break;
					}
				}
				if (!string.IsNullOrEmpty(keyFieldName))
				{
					int historyInserts = 1;
					if (
						(
						OutputOption.OutputType == OutputType.Coupon ||
						OutputOption.OutputType == OutputType.Message ||
						OutputOption.OutputType == OutputType.Offer ||
						OutputOption.OutputType == OutputType.Promotion ||
						OutputOption.OutputType == OutputType.Survey
						) && OutputOption.RefId.Count > 1
						)
					{
						//loop
						historyInserts = OutputOption.RefId.Count;
					}

					for (int i = 0; i < historyInserts; i++)
					{
						SqlQuery q = new SqlQuery();
						q.StepType = StepType.Select;
						q.RootTableName = Step.OutputTableName;
						q.ActionTable = Constants.CampaignHistoryTableName;
						q.InsertFieldList.Add("AudienceKey");
						q.InsertFieldList.Add("CampaignId");
						q.InsertFieldList.Add("StepId");
						q.InsertFieldList.Add("OutputType");
						q.InsertFieldList.Add("OutputFile");
						q.InsertFieldList.Add("OutputId");

						q.SelectFieldList.Add(keyFieldName);
						q.SelectFieldList.Add(Step.CampaignId.ToString());
						q.SelectFieldList.Add(Step.Id.ToString());
						q.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(OutputOption.OutputType.ToString()));
						q.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(string.IsNullOrEmpty(_evaluatedFileLocation) ? "''" : _evaluatedFileLocation));
						if (OutputOption.RefId.Count > i)
						{
							q.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(OutputOption.RefId[i].ToString()));
						}
						else
						{
							q.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(string.Empty));
						}
						manager.BatchProvider.Execute(manager.BatchProvider.CreateSqlStatement(q)[0], null);
					}
				}
				return rowCount;
			}
		}

		internal override List<CampaignResult> Execute(ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                Step.UILastStatus = "Truncating temporary table...";
                manager.UpdateStep(Step);

                manager.BatchProvider.DropTable(manager.GetCampaignTable((long)Step.OutputTableId).Name);

                int rowCount = 0;
                var statements = GetSqlStatement(overrideParameters);
                for (int i = 0; i < statements.Count; i++)
                {
                    var sql = statements[i];
                    Step.UILastStatus = string.Format("Executing database query {0} of {1}...", i + 1, statements.Count);
                    manager.UpdateStep(Step);

                    manager.BatchProvider.Execute(sql, sql.Parameters);
                    if (sql.ApplyToResults)
                    {
                        //output step uses "create table as" statement. Oracle does not return a row count 
                        //from this statement, so we'll count the rows in the new table:
                        int statementCount = manager.BatchProvider.RowCountExact(Step.OutputTableName);
                        rowCount += statementCount;
                    }
                }

                return new List<CampaignResult>() { new CampaignResult(rowCount) };
            }
		}

		protected string GetNextCertificateNumber(ContentService content, ContentObjType type, string typeCode)
		{
			var cert = content.RetrieveFirstAvailablePromoCertificate(type, typeCode, null, null);
			if (cert == null)
			{
				throw new LWException(string.Format("Cannot retrieve certificates of type {0} for typecode {1}. No more certificates available.", type.ToString(), typeCode));
			}
			return cert.CertNmbr;
		}

		protected override void ApplyCampaignParameters(SqlStatement statement, Dictionary<string, string> overrideParameters = null)
		{
			using(CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
			if (manager.BatchProvider.DatabaseType == SupportedDataSourceType.Oracle10g)
			{
				//Oracle uses "CREATE TABLE <OutputTableName> AS", which does not allow parameters. We'll convert them to literals in order to use them in the output step.
				if (statement == null)
				{
					throw new ArgumentNullException("statement");
				}
				if (Step.CampaignId.HasValue)
				{
					var attributes = manager.GetAllAttributes();
					if (attributes != null && attributes.Count > 0)
					{
						var campaignAttributes = manager.GetAllCampaignAttributes(Step.CampaignId.Value);
						foreach (var attribute in attributes)
						{
							string dbSyntax = manager.BatchProvider.ParameterPrefix + attribute.ToDatabaseName();
							if (statement.Statement.IndexOf(dbSyntax, StringComparison.OrdinalIgnoreCase) > -1)
							{
								string value = null;
								if (overrideParameters != null && overrideParameters.ContainsKey(attribute.Name))
								{
									value = overrideParameters[attribute.Name];
								}
								else
								{
									value = campaignAttributes.Where(o => o.AttributeId == attribute.Id).Select(o => o.AttributeValue).FirstOrDefault();
								}

								switch (attribute.DataType)
								{
									case AttributeDataType.DateTime:
										DateTime dateVal = DateTimeUtil.MinValue;
										DateTime.TryParse(value, out dateVal);
										value = string.Format("to_date('{0}', 'MM/DD/YYYY')", dateVal.ToShortDateString());
										break;
									case AttributeDataType.Float:
									case AttributeDataType.Integer:
										break;
									case AttributeDataType.String:
										value = string.Format("'{0}'", value.Replace("'", "''"));
										break;
								}
								statement.Statement = statement.Statement.Replace(dbSyntax, value);
							}
						}
					}
				}
			}
			else
			{
				base.ApplyCampaignParameters(statement, overrideParameters);
			}
		}

        protected DateTime GetExpressionDate(string expression, DateTime defaultDate, ContextObject context = null)
        {
            return GetExpressionDate(expression, (DateTime?)defaultDate, context).Value;
        }

        protected DateTime? GetExpressionDate(string expression, DateTime? defaultDate, ContextObject context = null)
		{
			if (!string.IsNullOrEmpty(expression))
			{
				Expression e = new ExpressionFactory().Create(expression);
				object result = e.evaluate(context);
				if (result != null)
				{
					if (result is DateTime)
					{
						return (DateTime)result;
					}
					else if (result is IConvertible)
					{
						return Convert.ToDateTime(result);
					}
					else
					{
						return DateTime.Parse(result.ToString());
					}
				}
			}
			return defaultDate;
		}

		protected int? GetDisplayOrder(string expression, int? defaultOrder, ContextObject context = null)
		{
			if (!string.IsNullOrEmpty(expression))
			{
				Expression e = new ExpressionFactory().Create(expression);
				object result = e.evaluate(context);
				if (result != null)
				{
					if (result is int)
					{
						return (int)result;
					}
					else if (result is IConvertible)
					{
						return Convert.ToInt32(result);
					}
					else
					{
						return int.Parse(result.ToString());
					}
				}
			}
			return defaultOrder;
		}

		private string GetOutputAsFieldName(QueryColumn column, bool asToken = false)
		{
			string ret = null;

			if (!string.IsNullOrWhiteSpace(column.OutputAs))
			{
				ret = asToken ? column.GetOutputAsToken(Columns) : column.OutputAs;
			}
			else if (!string.IsNullOrWhiteSpace(column.FieldName))
			{
				ret = column.FieldName;
			}
			else
			{
				ret = column.ColumnExpression;
				if (!string.IsNullOrWhiteSpace(column.ColumnExpression))
				{
					if (ret.Contains("."))
					{
						ret = ret.Substring(ret.LastIndexOf(".") + 1);
					}

					if (ret != string.Empty)
					{
						ret = Regex.Replace(ret, "[^0-9a-zA-Z]", string.Empty);
					}
				}
			}
			if (string.IsNullOrWhiteSpace(ret))
			{
				throw new Exception(string.Format("A field name must be given to column {0}.", column.ColumnExpression));
			}
			return ret;
		}

        private void SendAssociatedPush(LoyaltyDataService loyalty, long ipCode, long pushNotificationId)
        {
            string methodName = "SendAssociatedPush";
            Member member = loyalty.LoadMemberFromIPCode(ipCode);
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

        private AttributeEncryptionType GetAttributeEncryptionType(QueryColumn column, List<CampaignTable> tables)
        {
            if (column.ColumnExpression.Contains("."))
            {
                string tableName = column.ColumnExpression.Substring(0, column.ColumnExpression.IndexOf('.'));
                string columnName = column.ColumnExpression.Substring(column.ColumnExpression.IndexOf('.') + 1);
                CampaignTable table = GetTableName(tableName, tables);
                if (table != null)
                {
                    TableField field = GetColumnName(columnName, table.Fields);
                    return field != null ? field.EncryptionType : AttributeEncryptionType.None;
                }
            }
            return AttributeEncryptionType.None;
        }
	}
}