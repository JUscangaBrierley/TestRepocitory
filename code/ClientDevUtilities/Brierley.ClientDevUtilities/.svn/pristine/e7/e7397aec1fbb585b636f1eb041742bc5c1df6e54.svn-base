using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement
{
	public class CampaignExporter
	{
		private ServiceConfig _config;

		public CampaignExporter()
		{
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public CampaignExporter(ServiceConfig config)
		{
			_config = config;
		}

		public XElement ExportCampaign(long campaignId)
		{
			using (var manager = LWDataServiceUtil.CampaignManagerInstance(_config.Organization, _config.Environment))
			using (var dataService = LWDataServiceUtil.DataServiceInstance(_config.Organization, _config.Environment))
			using (var contentService = LWDataServiceUtil.ContentServiceInstance(_config.Organization, _config.Environment))
			using (var emailService = LWDataServiceUtil.EmailServiceInstance(_config.Organization, _config.Environment))
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance(_config.Organization, _config.Environment))
			{
				Campaign campaign = manager.GetCampaign(campaignId);
				if (campaign == null)
				{
					return null;
				}

				var element = new XElement("campaign");

				var steps = manager.GetStepsByCampaignID(campaignId);

				var mappings = new XElement("mappings");
				List<long> exportedKeys = new List<long>();
				element.Add(mappings);
				foreach (var step in steps.Where(o => o.KeyId.HasValue))
				{
					if (exportedKeys.Contains(step.KeyId.Value))
					{
						continue;
					}

					TableKey key = manager.GetTableKey(step.KeyId.Value);
					if (key == null || !key.IsAudienceLevel)
					{
						continue;
					}

					Audience audience = manager.GetAudience(key.AudienceId);
					CampaignTable table = manager.GetCampaignTable(key.TableId);

					XElement mapping = new XElement(
						"mapping",
						new XAttribute("keyid", key.Id),
						new XAttribute("audience", audience.Name),
						new XAttribute("table", table.Name),
						new XAttribute("isframeworkschema", table.IsFrameworkSchema),
						new XAttribute("iscampaignschema", table.IsCampaignSchema),
						new XAttribute("residesinalternateschema", table.ResidesInAlternateSchema),
						new XAttribute("fieldname", key.FieldName));

					mappings.Add(mapping);
					exportedKeys.Add(key.Id);
				}

				//add output references so that we can switch the ids to the correct destination id during migration
				var references = new XElement("references");
				var exportedReferences = new Dictionary<OutputType, List<long>>();
				element.Add(references);
				OutputType[] referencedTypes = new OutputType[] { OutputType.Coupon, OutputType.Message, OutputType.Offer, OutputType.Promotion, OutputType.Survey, OutputType.Email, OutputType.SocialMedia };
				foreach (var step in steps.Where(
						o =>
						o.Query != null &&
						o.Query is OutputQuery &&
						((OutputQuery)o.Query).OutputOption != null &&
						referencedTypes.Contains(((OutputQuery)o.Query).OutputOption.OutputType) &&
						((OutputQuery)o.Query).OutputOption.RefId != null))
				{
					var query = (OutputQuery)step.Query;
					OutputType outputType = query.OutputOption.OutputType;
					if (!exportedReferences.ContainsKey(outputType))
					{
						exportedReferences.Add(outputType, new List<long>());
					}

					foreach (var id in query.OutputOption.RefId)
					{
						if (!exportedReferences[outputType].Contains(id))
						{
							exportedReferences[outputType].Add(id);
						}
					}
				}

				foreach (var step in steps.Where(
						o =>
						o.Query != null &&
						o.Query is OutputQuery &&
						((OutputQuery)o.Query).OutputOption != null &&
						((OutputQuery)o.Query).OutputOption.OutputType == OutputType.SocialMedia &&
						((OutputQuery)o.Query).OutputOption.TextBlockId > 0))
				{
					var query = (OutputQuery)step.Query;
					OutputType outputType = query.OutputOption.OutputType;
					if (!exportedReferences.ContainsKey(outputType))
					{
						exportedReferences.Add(outputType, new List<long>());
					}

					if (!exportedReferences[outputType].Contains(query.OutputOption.TextBlockId))
					{
						exportedReferences[outputType].Add(query.OutputOption.TextBlockId);
					}
				}

				foreach (var exportedType in
							from d in exportedReferences
							from v in d.Value
							select new { OutputType = d.Key, ReferenceId = v })
				{
					string name = string.Empty;
					switch (exportedType.OutputType)
					{
						case OutputType.Coupon:
							var coupon = contentService.GetCouponDef(exportedType.ReferenceId);
							if (coupon != null)
							{
								name = coupon.Name;
							}
							break;
						case OutputType.Email:
							var email = emailService.GetEmail(exportedType.ReferenceId);
							if (email != null)
							{
								name = email.Name;
							}
							break;
						case OutputType.Message:
							var message = contentService.GetMessageDef(exportedType.ReferenceId);
							if (message != null)
							{
								name = message.Name;
							}
							break;
						case OutputType.Offer:
							var bonus = contentService.GetBonusDef(exportedType.ReferenceId);
							if (bonus != null)
							{
								name = bonus.Name;
							}
							break;
						case OutputType.Promotion:
							var promotion = contentService.GetPromotion(exportedType.ReferenceId);
							if (promotion != null)
							{
								name = promotion.Name;
							}
							break;
						case OutputType.Survey:
							var survey = surveyManager.RetrieveSurvey(exportedType.ReferenceId);
							if (survey != null)
							{
								name = survey.Name;
							}
							break;
						case OutputType.SocialMedia:
							var textBlock = contentService.GetTextBlock(exportedType.ReferenceId);
							if (textBlock != null)
							{
								name = textBlock.Name;
							}
							break;
					}

					var reference = new XElement(
						"reference",
						new XAttribute("type", exportedType.OutputType),
						new XAttribute("id", exportedType.ReferenceId),
						new XAttribute("name", name));
					references.Add(reference);
				}

				var tables = new XElement("tables");
				List<long> exportedTables = new List<long>();
				element.Add(tables);
				foreach (var step in steps.Where(o => o.Query != null))
				{
					foreach (var qc in step.Query.Columns.Where(o => o.TableId > 0))
					{
						if (exportedTables.Contains(qc.TableId))
						{
							continue;
						}

						CampaignTable t = manager.GetCampaignTable(qc.TableId);
						if (t == null)
						{
							continue;
						}

						XElement tableElement = new XElement(
							"table",
							new XAttribute("tableid", t.Id),
							new XAttribute("name", t.Name));

						tables.Add(tableElement);
						exportedTables.Add(t.Id);
					}
				}

				//serialize campaign:
				var serializer = new XmlSerializer(typeof(Campaign));
				var campaignString = new System.IO.StringWriter();
				serializer.Serialize(campaignString, campaign);

				XElement campaignElement = new XElement("campaignobject");
				campaignElement.Add(XElement.Parse(campaignString.ToString()));
				element.Add(campaignElement);

				//serialize steps and IO:
				var stepsElement = new XElement("steps");
				element.Add(stepsElement);

				var stepIOElement = new XElement("stepinputs");
				element.Add(stepIOElement);

				serializer = new XmlSerializer(typeof(Step));

				foreach (var step in steps)
				{
					var stepString = new System.IO.StringWriter();
					serializer.Serialize(stepString, step);

					XElement stepElement = new XElement("step");
					stepsElement.Add(XElement.Parse(stepString.ToString()));

					var inputs = manager.RetrieveInputs(step.Id);
					foreach (var input in inputs)
					{
						var inputElement = new XElement(
							"input",
							new XAttribute("fromstep", input.InputStepId),
							new XAttribute("tostep", input.OutputStepId));
						if (input.MergeType.HasValue)
						{
							inputElement.Add(new XAttribute("mergetype", input.MergeType.Value));
						}
						if (input.MergeOrder.HasValue)
						{
							inputElement.Add(new XAttribute("mergeorder", input.MergeOrder));
						}
						stepIOElement.Add(inputElement);
					}
				}
				return element;
			}
		}
	}
}
