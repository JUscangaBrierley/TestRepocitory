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
	public class SurveyExporter
	{
		private ServiceConfig _config;

		public SurveyExporter()
		{
            _config = LWDataServiceUtil.GetServiceConfiguration();
        }

		public SurveyExporter(ServiceConfig config)
		{
			_config = config;
		}

		public void ExportSurveyToXMLFile(long surveyId, string fileName)
		{
			XElement surveyXml = ExportSurveyToXElement(surveyId);
			XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
			surveyXml.WriteTo(writer);
		}

		public void ExportSurveyToStream(long surveyId, Stream writer)
		{
			XElement surveyXml = ExportSurveyToXElement(surveyId);

			string xml = surveyXml.ToString();
			byte[] data = UTF8Encoding.UTF8.GetBytes(xml);
			writer.Write(data, 0, data.Length);
		}

		public string ExportSurveyToXMLString(long surveyId)
		{
			XElement surveyXml = ExportSurveyToXElement(surveyId);

			string xml = surveyXml.ToString();

			return xml;
		}

		public byte[] ExportSurveyToUTF8Bytes(long surveyId)
		{
			XElement surveyXml = ExportSurveyToXElement(surveyId);

			string xml = surveyXml.ToString();

			byte[] data = UTF8Encoding.UTF8.GetBytes(xml);

			return data;
		}

		public XElement ExportSurveyToXElement(long surveyId)
		{
			using (var surveyService = new SurveyManager(_config))
			using (var contentService = new ContentService(_config))
			using (var loyaltyService = new LoyaltyDataService(_config))
			{
				SMSurvey survey = surveyService.RetrieveSurvey(surveyId);
				if (survey == null)
					throw new ArgumentException(string.Format("ExportSurvey: invalid survey ID {0}.", surveyId));

				string templateName = string.Empty;
				if (survey.TemplateID != -1)
				{
					Template template = contentService.GetTemplate(survey.TemplateID);
					if (template != null) templateName = template.Name;
				}
				string documentName = string.Empty;
				if (survey.DocumentID != -1)
				{
					Document document = contentService.GetDocument(survey.DocumentID);
					if (document != null) documentName = document.Name;
				}

				string surveyCompletedRule = string.Empty;
				if (survey.SurveyCompleteRuleId > 0)
				{
					RuleTrigger rt = loyaltyService.GetRuleById(survey.SurveyCompleteRuleId);
					if (rt != null)
					{
						surveyCompletedRule = rt.RuleName;
					}
				}
				string surveyTerminateAndTallyRule = string.Empty;
				if (survey.SurveyTerminateAndTallyRuleId > 0)
				{
					RuleTrigger rt = loyaltyService.GetRuleById(survey.SurveyTerminateAndTallyRuleId);
					if (rt != null)
					{
						surveyTerminateAndTallyRule = rt.RuleName;
					}
				}
				// Survey

				XElement surveyXml = new XElement("survey");
				surveyXml.Add(new XAttribute("id", survey.ID));
				surveyXml.Add(new XAttribute("name", survey.Name));
				surveyXml.Add(new XAttribute("description", StringUtils.FriendlyString(survey.Description)));
				surveyXml.Add(new XAttribute("effectivedate", survey.EffectiveDate));
				surveyXml.Add(new XAttribute("expirationdate", survey.ExpirationDate));
				surveyXml.Add(new XAttribute("constraintsxml", StringUtils.FriendlyString(survey.ConstraintsXML)));
				surveyXml.Add(new XAttribute("emailid", survey.EmailID));
				surveyXml.Add(new XAttribute("surveytype", survey.SurveyType));
				surveyXml.Add(new XAttribute("surveyaudience", survey.SurveyAudience));
				surveyXml.Add(new XAttribute("displayorder", survey.DisplayOrder));
				surveyXml.Add(new XAttribute("surveycompleterule", surveyCompletedRule));
				surveyXml.Add(new XAttribute("surveyterminateandtallyrule", surveyTerminateAndTallyRule));
				surveyXml.Add(new XAttribute("templatename", templateName));
				surveyXml.Add(new XAttribute("documentname", documentName));
				surveyXml.Add(new XAttribute("folderid", survey.FolderId.HasValue ? survey.FolderId.Value : 0));
				// ignore: SurveyStatus
				// ignore: CreateDate
				// ignore: UpdateDate

				// Languages
				List<SMLanguage> languages = surveyService.RetrieveLanguages();
				Dictionary<long, string> languageID2Name = new Dictionary<long, string>();
				foreach (SMLanguage language in languages)
				{
					if (!languageID2Name.ContainsKey(language.ID))
					{
						languageID2Name.Add(language.ID, language.Description);
					}
				}

				// Concepts
				XElement conceptsXml = new XElement("concepts");
				foreach (SMLanguage language in languages)
				{
					List<SMConcept> concepts = surveyService.RetrieveConcepts(survey.ID, language.ID);
					foreach (SMConcept concept in concepts)
					{
						XElement conceptXml = new XElement("concept");
						conceptXml.Add(new XAttribute("id", concept.ID));
						conceptXml.Add(new XAttribute("surveyid", concept.SurveyID));
						conceptXml.Add(new XAttribute("language", languageID2Name[concept.LanguageID]));
						conceptXml.Add(new XAttribute("name", concept.Name));
						conceptXml.Add(new XAttribute("groupname", StringUtils.FriendlyString(concept.GroupName)));
						conceptXml.Add(new XAttribute("constraintsxml", StringUtils.FriendlyString(concept.ConstraintsXML, "<constraints />")));
						// ignore: CreateDate
						// ignore: UpdateDate
						conceptXml.Value = StringUtils.FriendlyString(concept.Content);
						conceptsXml.Add(conceptXml);
					}
				}
				surveyXml.Add(conceptsXml);

				// State model
				List<SMState> states = surveyService.RetrieveStatesBySurveyID(survey.ID);
				XElement statesXml = new XElement("states");
				foreach (SMState state in states)
				{
					// State
					XElement stateXml = new XElement("state");
					stateXml.Add(new XAttribute("id", state.ID));
					stateXml.Add(new XAttribute("surveyid", state.SurveyID));
					stateXml.Add(new XAttribute("statetype", state.StateType));
					stateXml.Add(new XAttribute("uipositionx", state.UIPositionX));
					stateXml.Add(new XAttribute("uipositiony", state.UIPositionY));
					stateXml.Add(new XAttribute("page", state.Page));
					stateXml.Add(new XAttribute("uiname", state.UIName));
					stateXml.Add(new XAttribute("uidescription", state.UIDescription));
					// ignore: CreateDate
					// ignore: UpdateDate

					// Input transitions
					XElement inputsXml = new XElement("inputs");
					SMTransitionCollection inputs = state.GetInputs(_config);
					foreach (SMTransition transition in inputs)
					{
						XElement inputXml = new XElement("input");
						inputXml.Add(new XAttribute("srcstateid", transition.SrcStateID));
						inputXml.Add(new XAttribute("srcconnectorindex", transition.SrcConnectorIndex));
						inputXml.Add(new XAttribute("dststateid", transition.DstStateID));
						inputXml.Add(new XAttribute("dstconnectorindex", transition.DstConnectorIndex));
						inputXml.Add(new XAttribute("page", transition.Page));
						// ignore: CreateDate
						// ignore: UpdateDate
						inputsXml.Add(inputXml);
					}
					stateXml.Add(inputsXml);

					// Output transitions
					XElement outputsXml = new XElement("outputs");
					SMTransitionCollection outputs = state.GetOutputs(_config);
					foreach (SMTransition transition in outputs)
					{
						XElement outputXml = new XElement("output");
						outputXml.Add(new XAttribute("srcstateid", transition.SrcStateID));
						outputXml.Add(new XAttribute("srcconnectorindex", transition.SrcConnectorIndex));
						outputXml.Add(new XAttribute("dststateid", transition.DstStateID));
						outputXml.Add(new XAttribute("dstconnectorindex", transition.DstConnectorIndex));
						outputXml.Add(new XAttribute("page", transition.Page));
						// ignore: CreateDate
						// ignore: UpdateDate
						outputsXml.Add(outputXml);
					}
					stateXml.Add(outputsXml);

					// Content based on state type
					switch (state.StateType)
					{
						case StateType.Start:
							{
								stateXml.Add(new XElement("start"));
							}
							break;

						case StateType.Message:
							{
								XElement messageXml = new XElement("message");
								SMMessage message = surveyService.RetrieveMessageByStateID(state.ID);
								if (message != null)
								{
									messageXml.Add(new XAttribute("id", message.ID));
									messageXml.Add(new XAttribute("stateid", message.StateID));
									messageXml.Value = StringUtils.FriendlyString(message.Content);
									// ignore: CreateDate
									// ignore: UpdateDate
								}
								stateXml.Add(messageXml);
							}
							break;

						case StateType.Decision:
							{
								XElement decisionXml = new XElement("decision");
								SMDecision decision = surveyService.RetrieveDecisionByStateID(state.ID);
								if (decision != null)
								{
									decisionXml.Add(new XAttribute("id", decision.ID));
									decisionXml.Add(new XAttribute("stateid", decision.StateID));
									decisionXml.Value = StringUtils.FriendlyString(decision.Expression);
									// ignore: CreateDate
									// ignore: UpdateDate
								}
								stateXml.Add(decisionXml);
							}
							break;

						case StateType.Question:
						case StateType.MatrixQuestion:
							{
								XElement questionXml = new XElement((state.StateType == StateType.Question ? "simplequestion" : "matrixquestion"));
								SMQuestion question = surveyService.RetrieveQuestionByStateID(state.ID);
								if (question != null)
								{
									// Question
									questionXml.Add(new XAttribute("id", question.ID));
									questionXml.Add(new XAttribute("stateid", question.StateID));
									questionXml.Add(new XAttribute("effectivedate", question.EffectiveDate));
									questionXml.Add(new XAttribute("expirationdate", question.ExpirationDate));
									questionXml.Add(new XAttribute("ismatrix", question.IsMatrix));
									questionXml.Add(new XAttribute("hasotherspecify", question.HasOtherSpecify));
									questionXml.Add(new XAttribute("matrixpointscale", question.MatrixPointScale));
									questionXml.Add(new XAttribute("answercontroltype", question.AnswerControlType));
									questionXml.Add(new XAttribute("answerorientation", question.AnswerOrientation));
									questionXml.Add(new XAttribute("responseminval", StringUtils.FriendlyString(question.ResponseMinVal)));
									questionXml.Add(new XAttribute("responsemaxval", StringUtils.FriendlyString(question.ResponseMaxVal)));
									questionXml.Add(new XAttribute("responseoptional", question.ResponseOptional));
									questionXml.Add(new XAttribute("ispiped", question.IsPiped));
									questionXml.Add(new XAttribute("pipedstateid", question.PipedStateID));
									questionXml.Add(new XAttribute("rowlimit", question.RowLimit));
									// ignore: CreateDate
									// ignore: UpdateDate

									// Question headers
									XElement questionHeadersXml = new XElement("questionheaders");
									foreach (SMLanguage language in languages)
									{
										List<SMQuestionContent> questionHeaders = surveyService.RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.HEADER_TEXT);
										foreach (SMQuestionContent questionHeader in questionHeaders)
										{
											XElement questionHeaderXml = new XElement("questionheader");
											questionHeaderXml.Add(new XAttribute("id", questionHeader.ID));
											questionHeaderXml.Add(new XAttribute("questionid", questionHeader.QuestionID));
											questionHeaderXml.Add(new XAttribute("language", languageID2Name[questionHeader.LanguageID]));
											questionHeaderXml.Add(new XAttribute("matrixindex", questionHeader.MatrixIndex));
											questionHeaderXml.Add(new XAttribute("contenttype", questionHeader.ContentType));
											questionHeaderXml.Add(new XAttribute("randomize", questionHeader.Randomize));
											questionHeaderXml.Add(new XAttribute("rowsum", questionHeader.RowSum));
											questionHeaderXml.Value = StringUtils.FriendlyString(questionHeader.Content);
											// ignore: CreateDate
											// ignore: UpdateDate
											questionHeadersXml.Add(questionHeaderXml);
										}
									}
									questionXml.Add(questionHeadersXml);

									// Question bodys
									XElement questionBodysXml = new XElement("questionbodys");
									foreach (SMLanguage language in languages)
									{
										List<SMQuestionContent> questionBodys = surveyService.RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.BODY_TEXT);
										foreach (SMQuestionContent questionBody in questionBodys)
										{
											XElement questionBodyXml = new XElement("questionbody");
											questionBodyXml.Add(new XAttribute("id", questionBody.ID));
											questionBodyXml.Add(new XAttribute("questionid", questionBody.QuestionID));
											questionBodyXml.Add(new XAttribute("language", languageID2Name[questionBody.LanguageID]));
											questionBodyXml.Add(new XAttribute("matrixindex", questionBody.MatrixIndex));
											questionBodyXml.Add(new XAttribute("contenttype", questionBody.ContentType));
											questionBodyXml.Add(new XAttribute("randomize", questionBody.Randomize));
											questionBodyXml.Add(new XAttribute("rowsum", questionBody.RowSum));
											// ignore: CreateDate
											// ignore: UpdateDate

											if (state.StateType == StateType.Question)
											{
												// content for simple question is the value
												questionBodyXml.Value = StringUtils.FriendlyString(questionBody.Content);
											}

											if (state.StateType == StateType.MatrixQuestion)
											{
												if (!string.IsNullOrWhiteSpace(questionBody.VisibilityExpression))
												{
													questionBodyXml.Add(new XAttribute("visibilityexpression", CryptoUtil.EncodeUTF8(questionBody.VisibilityExpression)));
												}

												// content for matrix question is wrapped in an element
												XElement questionBodyContentXml = new XElement("matrixquestionbodycontent");
												questionBodyContentXml.Value = StringUtils.FriendlyString(questionBody.Content);
												questionBodyXml.Add(questionBodyContentXml);

												// Matrix answer bodys
												XElement matrixanswerBodysXml = new XElement("matrixanswerbodys");
												List<SMMatrixAnswer> matrixanswerBodys = surveyService.RetrieveMatrixAnswerByQuestionContentID(questionBody.ID);
												foreach (SMMatrixAnswer matrixanswerBody in matrixanswerBodys)
												{
													XElement matrixanswerBodyXml = new XElement("matrixanswerbody");
													matrixanswerBodyXml.Add(new XAttribute("id", matrixanswerBody.ID));
													matrixanswerBodyXml.Add(new XAttribute("questioncontentid", matrixanswerBody.QuestionContentID));
													matrixanswerBodyXml.Add(new XAttribute("language", languageID2Name[matrixanswerBody.LanguageID]));
													matrixanswerBodyXml.Add(new XAttribute("columnindex", matrixanswerBody.ColumnIndex));
													matrixanswerBodyXml.Add(new XAttribute("answercontroltype", matrixanswerBody.AnswerControlType));
													matrixanswerBodyXml.Add(new XAttribute("responseminval", StringUtils.FriendlyString(matrixanswerBody.ResponseMinVal)));
													matrixanswerBodyXml.Add(new XAttribute("responsemaxval", StringUtils.FriendlyString(matrixanswerBody.ResponseMaxVal)));
													matrixanswerBodyXml.Add(new XAttribute("colsum", matrixanswerBody.ColSum));
													// ignore: CreateDate
													// ignore: UpdateDate
													matrixanswerBodysXml.Add(matrixanswerBodyXml);
												}
												questionBodyXml.Add(matrixanswerBodysXml);
											}

											questionBodysXml.Add(questionBodyXml);
										}
									}
									questionXml.Add(questionBodysXml);

									// Question anchors
									XElement questionAnchorsXml = new XElement("questionanchors");
									foreach (SMLanguage language in languages)
									{
										List<SMQuestionContent> questionAnchors = surveyService.RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.ANCHOR_TEXT);
										foreach (SMQuestionContent questionAnchor in questionAnchors)
										{
											XElement questionAnchorXml = new XElement("questionanchor");
											questionAnchorXml.Add(new XAttribute("id", questionAnchor.ID));
											questionAnchorXml.Add(new XAttribute("questionid", questionAnchor.QuestionID));
											questionAnchorXml.Add(new XAttribute("language", languageID2Name[questionAnchor.LanguageID]));
											questionAnchorXml.Add(new XAttribute("matrixindex", questionAnchor.MatrixIndex));
											questionAnchorXml.Add(new XAttribute("contenttype", questionAnchor.ContentType));
											questionAnchorXml.Add(new XAttribute("randomize", questionAnchor.Randomize));
											questionAnchorXml.Add(new XAttribute("rowsum", questionAnchor.RowSum));
											// ignore: CreateDate
											// ignore: UpdateDate
											questionAnchorXml.Value = StringUtils.FriendlyString(questionAnchor.Content);
											questionAnchorsXml.Add(questionAnchorXml);
										}
									}
									questionXml.Add(questionAnchorsXml);

									// Question "other specify" bodys
									XElement otherSpecifysXml = new XElement("otherspecifys");
									foreach (SMLanguage language in languages)
									{
										List<SMQuestionContent> otherSpecifys = surveyService.RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.OTHER_SPECIFY_TEXT);
										foreach (SMQuestionContent otherSpecify in otherSpecifys)
										{
											XElement otherSpecifyXml = new XElement("otherspecify");
											otherSpecifyXml.Add(new XAttribute("id", otherSpecify.ID));
											otherSpecifyXml.Add(new XAttribute("questionid", otherSpecify.QuestionID));
											otherSpecifyXml.Add(new XAttribute("language", languageID2Name[otherSpecify.LanguageID]));
											otherSpecifyXml.Add(new XAttribute("matrixindex", otherSpecify.MatrixIndex));
											otherSpecifyXml.Add(new XAttribute("contenttype", otherSpecify.ContentType));
											otherSpecifyXml.Add(new XAttribute("randomize", otherSpecify.Randomize));
											otherSpecifyXml.Add(new XAttribute("rowsum", otherSpecify.RowSum));
											// ignore: CreateDate
											// ignore: UpdateDate
											otherSpecifyXml.Value = StringUtils.FriendlyString(otherSpecify.Content);
											otherSpecifysXml.Add(otherSpecifyXml);
										}
									}
									questionXml.Add(otherSpecifysXml);

									if (state.StateType == StateType.Question)
									{
										// Answer bodys
										XElement answerBodysXml = new XElement("answerbodys");
										foreach (SMLanguage language in languages)
										{
											List<SMAnswerContent> answerBodys = surveyService.RetrieveAnswerContents(question.ID, language.ID);
											foreach (SMAnswerContent answerBody in answerBodys)
											{
												XElement answerBodyXml = new XElement("answerbody");
												answerBodyXml.Add(new XAttribute("id", answerBody.ID));
												answerBodyXml.Add(new XAttribute("questionid", answerBody.QuestionID));
												answerBodyXml.Add(new XAttribute("language", languageID2Name[answerBody.LanguageID]));
												answerBodyXml.Add(new XAttribute("displayindex", answerBody.DisplayIndex));
												answerBodyXml.Add(new XAttribute("randomize", answerBody.Randomize));
												if (!string.IsNullOrWhiteSpace(answerBody.VisibilityExpression))
												{
													answerBodyXml.Add(new XAttribute("visibilityexpression", CryptoUtil.EncodeUTF8(answerBody.VisibilityExpression)));
												}
												// ignore: CreateDate
												// ignore: UpdateDate
												answerBodyXml.Value = StringUtils.FriendlyString(answerBody.Content);
												answerBodysXml.Add(answerBodyXml);
											}
										}
										questionXml.Add(answerBodysXml);
									}
								}
								stateXml.Add(questionXml);
							}
							break;

						case StateType.Terminate:
							{
								stateXml.Add(new XElement("terminate"));
							}
							break;

						case StateType.Skip:
							{
								stateXml.Add(new XElement("skip"));
							}
							break;
					}
					statesXml.Add(stateXml);
				}
				surveyXml.Add(statesXml);

				return surveyXml;
			}
		}
	}
}
