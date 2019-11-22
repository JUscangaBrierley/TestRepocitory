using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class SurveyImporter
	{
		private ServiceConfig _config;

		public SurveyImporter()
		{
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public SurveyImporter(ServiceConfig config)
		{
			_config = config;
		}

		public long ImportSurveyFromXMLFile(string fileName, string surveyName, bool overwrite)
		{
			string xml = File.ReadAllText(fileName, Encoding.UTF8);
			if (string.IsNullOrEmpty(xml))
				throw new Exception(string.Format("The file '{0}' is not a valid survey.", fileName));

			XElement surveyXml = XElement.Parse(xml);
			if (surveyXml == null || surveyXml.Name != "survey")
				throw new Exception(string.Format("The file '{0}' is not a valid survey.", fileName));

			return ImportSurveyFromXElement(surveyXml, surveyName, overwrite);
		}

		public long ImportSurveyFromXMLString(string xml, string surveyName, bool overwrite)
		{
			XElement surveyXml = XElement.Parse(xml);

			return ImportSurveyFromXElement(surveyXml, surveyName, overwrite);
		}

		public long ImportSurveyFromXMLString(string xml, bool overwrite)
		{
			XElement surveyXml = XElement.Parse(xml);
			return ImportSurveyFromXElement(surveyXml, null, overwrite);
		}

		public long ImportSurveyFromStream(Stream stream, string surveyName, bool overwrite)
		{
			StreamReader reader = new StreamReader(stream, true);
			string xml = reader.ReadToEnd();
			XElement surveyXml = XElement.Parse(xml);

			return ImportSurveyFromXElement(surveyXml, surveyName, overwrite);
		}

		public long ImportSurveyFromXElement(XElement surveyXml, string surveyName, bool overwrite)
		{
			using (var surveyService = new SurveyManager(_config))
			using (var contentService = new ContentService(_config))
			using (var loyaltyService = new LoyaltyDataService(_config))
			{
				if (surveyXml.Name != "survey")
					throw new Exception("The argument 'surveyXml' is not a valid exported survey.");

				if (string.IsNullOrEmpty(surveyName))
					surveyName = StringUtils.FriendlyXAttribute(surveyXml.Attribute("name"));
				if (string.IsNullOrEmpty(surveyName))
					throw new Exception("No survey name has been provided or can be inferred.");

				SMSurvey survey = surveyService.RetrieveSurvey(surveyName);
				if (survey != null)
				{
					if (overwrite)
					{
						surveyService.DeleteSurvey(survey.ID);
					}
					else
					{
						throw new Exception(string.Format("A survey named '{0}' already exists.", surveyName));
					}
				}

				long templateID = -1;
				string templateName = StringUtils.FriendlyString(StringUtils.FriendlyXAttribute(surveyXml.Attribute("templatename")));
				if (!string.IsNullOrEmpty(templateName))
				{
					Template template = contentService.GetTemplate(templateName);
					if (template != null)
					{
						templateID = template.ID;
					}
				}

				long documentID = -1;
				string documentName = StringUtils.FriendlyString(StringUtils.FriendlyXAttribute(surveyXml.Attribute("documentname")));
				if (!string.IsNullOrEmpty(documentName))
				{
					Document document = contentService.GetDocument(documentName);
					if (document != null)
					{
						documentID = document.ID;
					}
				}

				DateTime now = DateTime.Now;

				// Survey
				survey = new SMSurvey();
				survey.Name = surveyName;
				survey.Description = StringUtils.FriendlyXAttribute(surveyXml.Attribute("description"));
				survey.EffectiveDate = StringUtils.FriendlyDateTime(StringUtils.FriendlyXAttribute(surveyXml.Attribute("effectivedate"), null), DateTimeUtil.MinValue);
				survey.ExpirationDate = StringUtils.FriendlyDateTime(StringUtils.FriendlyXAttribute(surveyXml.Attribute("expirationdate"), null), DateTimeUtil.MaxValue);
				survey.ConstraintsXML = StringUtils.FriendlyString(StringUtils.FriendlyXAttribute(surveyXml.Attribute("constraintsxml"), "<constraints><absolute-quota enabled=\"false\" quota=\"0\" /></constraints>"));
				survey.EmailID = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(surveyXml.Attribute("emailid"), "-1"));
				survey.SurveyType = (SurveyType)Enum.Parse(typeof(SurveyType),
					StringUtils.FriendlyXAttribute(surveyXml.Attribute("surveytype"), Enum.GetName(typeof(SurveyType), SurveyType.General)),
					true);
				survey.SurveyAudience = (SurveyAudience)Enum.Parse(typeof(SurveyAudience),
					StringUtils.FriendlyXAttribute(surveyXml.Attribute("surveyaudience"), Enum.GetName(typeof(SurveyAudience), SurveyAudience.PreSelected)),
					true);
				survey.DisplayOrder = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(surveyXml.Attribute("displayorder"), "-1"));

				XAttribute att = surveyXml.Attribute("surveycompleterule");
				if (att != null && !string.IsNullOrEmpty(att.Value))
				{
					RuleTrigger rt = loyaltyService.GetRuleByName(att.Value);
					if (rt != null)
					{
						survey.SurveyCompleteRuleId = rt.Id;
					}
				}
				att = surveyXml.Attribute("surveyterminateandtallyrule");
				if (att != null && !string.IsNullOrEmpty(att.Value))
				{
					RuleTrigger rt = loyaltyService.GetRuleByName(att.Value);
					if (rt != null)
					{
						survey.SurveyTerminateAndTallyRuleId = rt.Id;
					}
				}
				att = surveyXml.Attribute("folderid");
				if (att != null && !string.IsNullOrEmpty(att.Value))
				{
					survey.FolderId = long.Parse(att.Value);
				}
				//survey.SurveyCompleteRuleId = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(surveyXml.Attribute("surveycompleteruleid"), "-1"));
				//survey.SurveyTerminateAndTallyRuleId = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(surveyXml.Attribute("surveyterminateandtallyruleid"), "-1"));

				survey.TemplateID = templateID;
				survey.DocumentID = documentID;
				survey.SurveyStatus = SurveyStatus.Design;
				survey.CreateDate = now;
				survey.UpdateDate = now;
				surveyService.CreateSurvey(survey);

				// Languages
				List<SMLanguage> languages = surveyService.RetrieveLanguages();
				Dictionary<string, long> languageName2ID = new Dictionary<string, long>();
				foreach (SMLanguage language in languages)
				{
					if (!languageName2ID.ContainsKey(language.Description))
					{
						languageName2ID.Add(language.Description, language.ID);
					}
				}

				// Concepts
				XElement conceptsXml = surveyXml.Element("concepts");
				if (conceptsXml != null && conceptsXml.HasElements)
				{
					foreach (XElement conceptXml in conceptsXml.Elements("concept"))
					{
						SMConcept concept = new SMConcept();
						concept.SurveyID = survey.ID;
						concept.LanguageID = ExtractLanguageID(conceptXml.Attribute("language"), languageName2ID);
						concept.Name = StringUtils.FriendlyXAttribute(conceptXml.Attribute("name"));
						concept.GroupName = StringUtils.FriendlyXAttribute(conceptXml.Attribute("groupname"));
						concept.ConstraintsXML = StringUtils.FriendlyXAttribute(conceptXml.Attribute("constraintsxml"));
						now = DateTime.Now;
						concept.CreateDate = now;
						concept.UpdateDate = now;
						concept.Content = StringUtils.FriendlyString(conceptXml.Value);
						surveyService.CreateConcept(concept);
					}
				}

				// State model
				XElement statesXml = surveyXml.Element("states");
				if (statesXml != null && statesXml.HasElements)
				{
					// Import the states and create a map for the old state ID to the new state
					Dictionary<long, SMState> oldStateID2NewState = new Dictionary<long, SMState>();
					foreach (XElement stateXml in statesXml.Elements("state"))
					{
						StateType stateType = (StateType)Enum.Parse(typeof(StateType),
							StringUtils.FriendlyXAttribute(stateXml.Attribute("statetype"), Enum.GetName(typeof(StateType), StateType.InvalidStateType)),
							true);

						if (stateType != StateType.Page)
							continue;

						// Page State
						SMState state = new SMState();
						state.SurveyID = survey.ID;
						state.StateType = StateType.Page;
						state.UIPositionX = StringUtils.FriendlyInt32(StringUtils.FriendlyXAttribute(stateXml.Attribute("uipositionx"), "-1"));
						state.UIPositionY = StringUtils.FriendlyInt32(StringUtils.FriendlyXAttribute(stateXml.Attribute("uipositiony"), "-1"));
						state.UIName = StringUtils.FriendlyXAttribute(stateXml.Attribute("uiname"));
						state.UIDescription = StringUtils.FriendlyXAttribute(stateXml.Attribute("uidescription"));
						state.Page = 0;
						now = DateTime.Now;
						state.CreateDate = now;
						state.UpdateDate = now;
						surveyService.CreateState(state);

						long oldStateID = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(stateXml.Attribute("id"), "-1"));
						oldStateID2NewState.Add(oldStateID, state);
					}
					foreach (XElement stateXml in statesXml.Elements("state"))
					{
						StateType stateType = (StateType)Enum.Parse(typeof(StateType),
							StringUtils.FriendlyXAttribute(stateXml.Attribute("statetype"), Enum.GetName(typeof(StateType), StateType.InvalidStateType)),
							true);

						if (stateType == StateType.Page)
							continue;

						long page = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(stateXml.Attribute("page")), 0);
						if (page != 0) page = oldStateID2NewState[page].ID;

						// Non-page State
						SMState state = new SMState();
						state.SurveyID = survey.ID;
						state.StateType = stateType;
						state.UIPositionX = StringUtils.FriendlyInt32(StringUtils.FriendlyXAttribute(stateXml.Attribute("uipositionx"), "-1"));
						state.UIPositionY = StringUtils.FriendlyInt32(StringUtils.FriendlyXAttribute(stateXml.Attribute("uipositiony"), "-1"));
						state.UIName = StringUtils.FriendlyXAttribute(stateXml.Attribute("uiname"));
						state.UIDescription = StringUtils.FriendlyXAttribute(stateXml.Attribute("uidescription"));
						state.Page = page;
						now = DateTime.Now;
						state.CreateDate = now;
						state.UpdateDate = now;
						surveyService.CreateState(state);

						long oldStateID = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(stateXml.Attribute("id"), "-1"));
						oldStateID2NewState.Add(oldStateID, state);
					}

					// Now import transitions etc. that depend on the old stateID
					foreach (XElement stateXml in statesXml.Elements("state"))
					{
						SMState state = oldStateID2NewState[StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(stateXml.Attribute("id"), "-1"))];

						// Input transitions
						XElement inputsXml = stateXml.Element("inputs");
						if (inputsXml != null && inputsXml.HasElements)
						{
							foreach (XElement inputXml in inputsXml.Elements("input"))
							{
								long srcStateID = oldStateID2NewState[StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(inputXml.Attribute("srcstateid")))].ID;
								long srcConnectorIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(inputXml.Attribute("srcconnectorindex")));
								long dstStateID = oldStateID2NewState[StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(inputXml.Attribute("dststateid")))].ID;
								long dstConnectorIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(inputXml.Attribute("dstconnectorindex")));
								long page = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(inputXml.Attribute("page")), 0);
								if (page != 0) page = oldStateID2NewState[page].ID;

								// Transition may already exist as output from the dst state
								SMTransition transition = surveyService.RetrieveTransition(srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
								if (transition == null)
								{
									transition = new SMTransition();
									transition.SrcStateID = srcStateID;
									transition.SrcConnectorIndex = srcConnectorIndex;
									transition.DstStateID = dstStateID;
									transition.DstConnectorIndex = dstConnectorIndex;
									transition.Page = page;
									now = DateTime.Now;
									transition.CreateDate = now;
									transition.UpdateDate = now;
									surveyService.CreateTransition(transition);
								}
							}
						}

						// Output transitions
						XElement outputsXml = stateXml.Element("outputs");
						if (outputsXml != null && outputsXml.HasElements)
						{
							foreach (XElement outputXml in outputsXml.Elements("output"))
							{
								long srcStateID = oldStateID2NewState[StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(outputXml.Attribute("srcstateid")))].ID;
								long srcConnectorIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(outputXml.Attribute("srcconnectorindex")));
								long dstStateID = oldStateID2NewState[StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(outputXml.Attribute("dststateid")))].ID;
								long dstConnectorIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(outputXml.Attribute("dstconnectorindex")));
								long page = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(outputXml.Attribute("page")), 0);
								if (page != 0) page = oldStateID2NewState[page].ID;

								// Transition may already exist as input from the src state
								SMTransition transition = surveyService.RetrieveTransition(srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
								if (transition == null)
								{
									transition = new SMTransition();
									transition.SrcStateID = srcStateID;
									transition.SrcConnectorIndex = srcConnectorIndex;
									transition.DstStateID = dstStateID;
									transition.DstConnectorIndex = dstConnectorIndex;
									transition.Page = page;
									now = DateTime.Now;
									transition.CreateDate = now;
									transition.UpdateDate = now;
									surveyService.CreateTransition(transition);
								}
							}
						}

						// Content based on state type
						switch (state.StateType)
						{
							case StateType.Start:
								break;

							case StateType.Message:
								{
									XElement messageXml = stateXml.Element("message");
									if (messageXml != null)
									{
										SMMessage message = new SMMessage();
										message.StateID = state.ID;
										now = DateTime.Now;
										message.CreateDate = now;
										message.UpdateDate = now;
										message.Content = StringUtils.FriendlyString(messageXml.Value);
										surveyService.CreateMessage(message);
									}
								}
								break;

							case StateType.Decision:
								{
									XElement decisionXml = stateXml.Element("decision");
									if (decisionXml != null)
									{
										SMDecision decision = new SMDecision();
										decision.StateID = state.ID;
										now = DateTime.Now;
										decision.CreateDate = now;
										decision.UpdateDate = now;
										decision.Expression = StringUtils.FriendlyString(decisionXml.Value);
										surveyService.CreateDecision(decision);
									}
								}
								break;

							case StateType.Question:
							case StateType.MatrixQuestion:
								{
									XElement questionXml = stateXml.Element((state.StateType == StateType.Question ? "simplequestion" : "matrixquestion"));
									if (questionXml != null)
									{
										// Question
										SMQuestion question = new SMQuestion();
										question.StateID = state.ID;
										question.EffectiveDate = StringUtils.FriendlyDateTime(StringUtils.FriendlyXAttribute(questionXml.Attribute("effectivedate"), null), DateTimeUtil.MinValue);
										question.ExpirationDate = StringUtils.FriendlyDateTime(StringUtils.FriendlyXAttribute(questionXml.Attribute("expirationdate"), null), DateTimeUtil.MaxValue);
										question.IsMatrix = (state.StateType == StateType.Question ? false : true);
										question.HasOtherSpecify = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionXml.Attribute("hasotherspecify")), false);
										question.MatrixPointScale = (QA_PointScale)Enum.Parse(typeof(QA_PointScale),
											StringUtils.FriendlyXAttribute(questionXml.Attribute("matrixpointscale"), Enum.GetName(typeof(QA_PointScale), QA_PointScale.Four)),
											true);
										question.AnswerControlType = (QA_AnswerControlType)Enum.Parse(typeof(QA_AnswerControlType),
											StringUtils.FriendlyXAttribute(questionXml.Attribute("answercontroltype"), Enum.GetName(typeof(QA_AnswerControlType), QA_AnswerControlType.RADIO)),
											true);
										question.AnswerOrientation = (QA_OrientationType)Enum.Parse(typeof(QA_OrientationType),
											StringUtils.FriendlyXAttribute(questionXml.Attribute("answerorientation"), Enum.GetName(typeof(QA_OrientationType), QA_OrientationType.HORIZONTAL)),
											true);
										question.ResponseMinVal = StringUtils.FriendlyXAttribute(questionXml.Attribute("responseminval"));
										question.ResponseMaxVal = StringUtils.FriendlyXAttribute(questionXml.Attribute("responsemaxval"));
										question.ResponseOptional = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionXml.Attribute("responseoptional")), false);
										question.IsPiped = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionXml.Attribute("ispiped")), false);
										question.PipedStateID = 0;
										if (question.IsPiped)
										{
											long oldPipedStateID = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(questionXml.Attribute("pipedstateid")));
											if (oldPipedStateID > 0)
											{
												question.PipedStateID = oldStateID2NewState[oldPipedStateID].ID;
											}
										}
										question.RowLimit = StringUtils.FriendlyInt32(StringUtils.FriendlyXAttribute(questionXml.Attribute("rowlimit")), 0);
										question.ValidationTotal = StringUtils.FriendlyString(StringUtils.FriendlyXAttribute(questionXml.Attribute("validationtotal"))); // CS-73
										now = DateTime.Now;
										question.CreateDate = now;
										question.UpdateDate = now;
										surveyService.CreateQuestion(question);

										// Question headers
										XElement questionHeadersXml = questionXml.Element("questionheaders");
										if (questionHeadersXml != null && questionHeadersXml.HasElements)
										{
											foreach (XElement questionHeaderXml in questionHeadersXml.Elements("questionheader"))
											{
												SMQuestionContent questionHeader = new SMQuestionContent();
												questionHeader.QuestionID = question.ID;
												questionHeader.LanguageID = ExtractLanguageID(questionHeaderXml.Attribute("language"), languageName2ID);
												questionHeader.MatrixIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(questionHeaderXml.Attribute("matrixindex")));
												questionHeader.ContentType = QuestionContentType.HEADER_TEXT;
												questionHeader.Randomize = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionHeaderXml.Attribute("randomize")), false);
												questionHeader.RowSum = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionHeaderXml.Attribute("rowsum")), false);
												now = DateTime.Now;
												questionHeader.CreateDate = now;
												questionHeader.UpdateDate = now;
												questionHeader.Content = StringUtils.FriendlyString(questionHeaderXml.Value);
												surveyService.CreateQuestionContent(questionHeader);
											}
										}

										// Question bodys
										XElement questionBodysXml = questionXml.Element("questionbodys");
										if (questionHeadersXml != null && questionBodysXml.HasElements)
										{
											foreach (XElement questionBodyXml in questionBodysXml.Elements("questionbody"))
											{
												SMQuestionContent questionBody = new SMQuestionContent();
												questionBody.QuestionID = question.ID;
												questionBody.LanguageID = ExtractLanguageID(questionBodyXml.Attribute("language"), languageName2ID);
												questionBody.MatrixIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(questionBodyXml.Attribute("matrixindex")));
												questionBody.ContentType = QuestionContentType.BODY_TEXT;
												questionBody.Randomize = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionBodyXml.Attribute("randomize")), false);
												questionBody.RowSum = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionBodyXml.Attribute("rowsum")), false);
												now = DateTime.Now;
												questionBody.CreateDate = now;
												questionBody.UpdateDate = now;
												if (state.StateType == StateType.Question)
												{
													questionBody.Content = StringUtils.FriendlyString(questionBodyXml.Value);
												}
												if (state.StateType == StateType.MatrixQuestion)
												{
													string visibilityExpression = StringUtils.FriendlyXAttribute(questionBodyXml.Attribute("visibilityexpression"));
													if (!string.IsNullOrWhiteSpace(visibilityExpression))
													{
														questionBody.VisibilityExpression = CryptoUtil.DecodeUTF8(visibilityExpression);
													}

													XElement questionBodyContent = questionBodyXml.Element("matrixquestionbodycontent");
													if (questionBodyContent != null)
													{
														questionBody.Content = StringUtils.FriendlyString(questionBodyContent.Value);
													}
												}
												surveyService.CreateQuestionContent(questionBody);

												if (state.StateType == StateType.MatrixQuestion)
												{
													// Matrix answer bodys
													XElement matrixanswerBodysXml = questionBodyXml.Element("matrixanswerbodys");
													if (matrixanswerBodysXml != null && matrixanswerBodysXml.HasElements)
													{
														foreach (XElement matrixanswerBodyXml in matrixanswerBodysXml.Elements("matrixanswerbody"))
														{
															SMMatrixAnswer matrixanswerBody = new SMMatrixAnswer();
															matrixanswerBody.QuestionContentID = questionBody.ID;
															matrixanswerBody.LanguageID = questionBody.LanguageID;
															matrixanswerBody.ColumnIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(matrixanswerBodyXml.Attribute("columnindex")));
															matrixanswerBody.AnswerControlType = (QA_AnswerControlType)Enum.Parse(typeof(QA_AnswerControlType),
																StringUtils.FriendlyXAttribute(
																	matrixanswerBodyXml.Attribute("answercontroltype"),
																	Enum.GetName(typeof(QA_AnswerControlType), QA_AnswerControlType.RADIO)),
																true);
															matrixanswerBody.ResponseMinVal = StringUtils.FriendlyXAttribute(matrixanswerBodyXml.Attribute("responseminval"));
															matrixanswerBody.ResponseMaxVal = StringUtils.FriendlyXAttribute(matrixanswerBodyXml.Attribute("responsemaxval"));
															matrixanswerBody.ColSum = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(matrixanswerBodyXml.Attribute("colsum")), false);
															now = DateTime.Now;
															matrixanswerBody.CreateDate = now;
															matrixanswerBody.UpdateDate = now;
															surveyService.CreateMatrixAnswer(matrixanswerBody);
														}
													}
												}
											}
										}

										// Question anchors
										XElement questionAnchorsXml = questionXml.Element("questionanchors");
										if (questionAnchorsXml != null && questionAnchorsXml.HasElements)
										{
											foreach (XElement questionAnchorXml in questionAnchorsXml.Elements("questionanchor"))
											{
												SMQuestionContent questionAnchor = new SMQuestionContent();
												questionAnchor.QuestionID = question.ID;
												questionAnchor.LanguageID = ExtractLanguageID(questionAnchorXml.Attribute("language"), languageName2ID);
												questionAnchor.MatrixIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(questionAnchorXml.Attribute("matrixindex")));
												questionAnchor.ContentType = QuestionContentType.ANCHOR_TEXT;
												questionAnchor.Randomize = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionAnchorXml.Attribute("randomize")), false);
												questionAnchor.RowSum = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(questionAnchorXml.Attribute("rowsum")), false);
												now = DateTime.Now;
												questionAnchor.CreateDate = now;
												questionAnchor.UpdateDate = now;
												questionAnchor.Content = StringUtils.FriendlyString(questionAnchorXml.Value);
												surveyService.CreateQuestionContent(questionAnchor);
											}
										}

										// Question "other specify" bodys
										XElement otherSpecifysXml = questionXml.Element("otherspecifys");
										if (otherSpecifysXml != null && otherSpecifysXml.HasElements)
										{
											foreach (XElement otherSpecifyXml in otherSpecifysXml.Elements("otherspecify"))
											{
												SMQuestionContent otherSpecify = new SMQuestionContent();
												otherSpecify.QuestionID = question.ID;
												otherSpecify.LanguageID = ExtractLanguageID(otherSpecifyXml.Attribute("language"), languageName2ID);
												otherSpecify.MatrixIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(otherSpecifyXml.Attribute("matrixindex")));
												otherSpecify.ContentType = QuestionContentType.OTHER_SPECIFY_TEXT;
												otherSpecify.Randomize = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(otherSpecifyXml.Attribute("randomize")), false);
												otherSpecify.RowSum = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(otherSpecifyXml.Attribute("rowsum")), false);
												now = DateTime.Now;
												otherSpecify.CreateDate = now;
												otherSpecify.UpdateDate = now;
												otherSpecify.Content = StringUtils.FriendlyString(otherSpecifyXml.Value);
												surveyService.CreateQuestionContent(otherSpecify);
											}
										}

										if (state.StateType == StateType.Question)
										{
											// Answer bodys
											XElement answerBodysXml = questionXml.Element("answerbodys");
											if (answerBodysXml != null && answerBodysXml.HasElements)
											{
												foreach (XElement answerBodyXml in answerBodysXml.Elements("answerbody"))
												{
													SMAnswerContent answerBody = new SMAnswerContent();
													answerBody.QuestionID = question.ID;
													answerBody.LanguageID = ExtractLanguageID(answerBodyXml.Attribute("language"), languageName2ID);
													answerBody.DisplayIndex = StringUtils.FriendlyInt64(StringUtils.FriendlyXAttribute(answerBodyXml.Attribute("displayindex")));
													answerBody.Randomize = StringUtils.FriendlyBool(StringUtils.FriendlyXAttribute(answerBodyXml.Attribute("randomize")), false);
													now = DateTime.Now;
													answerBody.CreateDate = now;
													answerBody.UpdateDate = now;

													string visibilityExpression = StringUtils.FriendlyXAttribute(answerBodyXml.Attribute("visibilityexpression"));
													if (!string.IsNullOrWhiteSpace(visibilityExpression))
													{
														answerBody.VisibilityExpression = CryptoUtil.DecodeUTF8(visibilityExpression);
													}

													answerBody.Content = StringUtils.FriendlyString(answerBodyXml.Value);
													surveyService.CreateAnswerContent(answerBody);
												}
											}
										}
									}
								}
								break;

							case StateType.PageStart:
							case StateType.PageEnd:
							case StateType.Page:
								break;

							case StateType.Terminate:
								break;

							case StateType.Skip:
								break;
						}
					}
				}
				return survey.ID;
			}			
		}

		private long ExtractLanguageID(XAttribute attribute, Dictionary<string, long> languageName2ID)
		{
			string languageName = StringUtils.FriendlyXAttribute(attribute, "English");
			long languageID = languageName2ID[languageName];
			return languageID;
		}
	}
}
