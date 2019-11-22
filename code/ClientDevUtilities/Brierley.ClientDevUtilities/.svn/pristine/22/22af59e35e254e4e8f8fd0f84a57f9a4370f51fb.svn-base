using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement
{
	public class CampaignImporter
	{
		
		private ServiceConfig _config;

		public CampaignImporter()
		{
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public CampaignImporter(ServiceConfig config)
		{
			_config = config;
		}

		public long ImportCampaign(string xml, bool overwrite)
		{
			XElement campaignXml = XElement.Parse(xml);
			return ImportCampaign(campaignXml, null, overwrite);
		}

		public long ImportCampaign(Stream stream, string campaignName, bool overwrite)
		{
			var reader = new StreamReader(stream, true);
			string xml = reader.ReadToEnd();
			XElement element = XElement.Parse(xml);
			return ImportCampaign(element, campaignName, overwrite);
		}

		public long ImportCampaign(XElement element, string campaignName, bool overwrite)
		{
			if (element.Name != "campaign")
			{
				throw new ArgumentException("The argument element is not a valid exported campaign.");
			}

			XmlSerializer serializer = new XmlSerializer(typeof(Campaign));
			Campaign campaign = (Campaign)serializer.Deserialize(new System.IO.StringReader(element.Element("campaignobject").Element("Campaign").ToString()));

			campaign.Id = 0;
			if (!string.IsNullOrEmpty(campaignName))
			{
				campaign.Name = campaignName;
			}
			else
			{
				campaignName = campaign.Name;
			}

            using (var manager = LWDataServiceUtil.CampaignManagerInstance(_config.Organization, _config.Environment))
            {

                Campaign existing = manager.GetCampaign(campaign.Name);
                if (existing != null)
                {
                    if (overwrite)
                    {
                        campaign.Id = existing.Id;
                        campaign.CreateDate = existing.CreateDate;
                        manager.ClearCampaign(existing);
                    }
                    else
                    {
                        throw new Exception(string.Format("A campaign named '{0}' already exists.", campaignName));
                    }
                }


                //map 
                Dictionary<long, long> convertedKeys = new Dictionary<long, long>();
                foreach (var mapping in element.Element("mappings").Descendants("mapping"))
                {
                    //long stepId = Convert.ToInt64(mapping.Attribute("stepid").Value);
                    long keyId = Convert.ToInt64(mapping.Attribute("keyid").Value);
                    string audienceName = mapping.AttributeValue("audience");
                    string tableName = mapping.AttributeValue("table");
                    bool isFrameworkSchema = Convert.ToBoolean(mapping.Attribute("isframeworkschema").Value);
                    bool isCampaignSchema = Convert.ToBoolean(mapping.Attribute("iscampaignschema").Value);
                    bool residesInAlternateSchema = Convert.ToBoolean(mapping.Attribute("residesinalternateschema").Value);
                    string fieldName = mapping.AttributeValue("fieldname");

                    Audience audience = manager.GetAudience(audienceName);
                    if (audience == null)
                    {
                        throw new Exception(string.Format("Failed to import campaign {0} because the audience {1} does not exist.", campaignName, audienceName));
                    }

                    TableKey key = null;
                    IList<TableKey> keys = manager.GetTableKeyByAudience(audience.Id);
                    foreach (var k in keys.Where(o => o.IsAudienceLevel && o.FieldName.ToLower() == fieldName.ToLower()))
                    {
                        CampaignTable t = manager.GetCampaignTable(k.TableId);
                        if (t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        {
                            key = k;
                            break;
                        }
                    }
                    if (key == null)
                    {
                        throw new Exception(string.Format("Failed to import campaign {0} because the required table mappings do not exist.", campaignName));
                    }
                    convertedKeys.Add(keyId, key.Id);
                }

                //reference map
                OutputType[] referencedTypes = new OutputType[] { OutputType.Coupon, OutputType.Message, OutputType.Offer, OutputType.Promotion, OutputType.Survey, OutputType.Email, OutputType.SocialMedia };
                var referenceMap = new Dictionary<OutputType, Dictionary<long, string>>();
                if (element.Element("references") != null)
                {
                    foreach (var mapping in element.Element("references").Descendants("reference"))
                    {
                        OutputType outputType = (OutputType)Enum.Parse(typeof(OutputType), mapping.AttributeValue("type"));
                        long sourceId = long.Parse(mapping.AttributeValue("id"));
                        string sourceName = mapping.AttributeValue("name");
                        if (!referenceMap.ContainsKey(outputType))
                        {
                            referenceMap.Add(outputType, new Dictionary<long, string>());
                        }
                        if (!referenceMap[outputType].ContainsKey(sourceId))
                        {
                            referenceMap[outputType].Add(sourceId, sourceName);
                        }
                    }
                }

                if (existing == null)
                    manager.CreateCampaign(campaign);
                else
                    manager.UpdateCampaign(campaign);

                Dictionary<long, long> convertedSteps = new Dictionary<long, long>();
                Dictionary<long, long> convertedOutputTables = new Dictionary<long, long>();
                foreach (var stepElement in element.Element("steps").Descendants("Step"))
                {
                    serializer = new XmlSerializer(typeof(Step));
                    Step step = (Step)serializer.Deserialize(new System.IO.StringReader(stepElement.ToString()));

                    long oldOutputTableId = step.OutputTableId.GetValueOrDefault(0);
                    long oldStepId = step.Id;
                    step.Id = 0;
                    step.CampaignId = campaign.Id;
                    step.OutputTableId = null;
                    step.ExecutionStart = null;
                    step.IsExecuting = null;
                    step.LastError = null;
                    step.LastRunDate = null;
                    step.UILastRecordCount = null;
                    step.VerificationState = null;

                    if (step.KeyId.HasValue)
                    {
                        if (convertedKeys.ContainsKey(step.KeyId.Value))
                        {
                            step.KeyId = convertedKeys[step.KeyId.Value];
                            step.Key = null;
                        }
                        else
                        {
                            step.KeyId = null;
                            step.Key = null;
                        }
                    }
                    manager.CreateStep(step);
                    campaign.Steps.Add(step);
                    convertedSteps.Add(oldStepId, step.Id);

                    if (oldOutputTableId > 0)
                    {
                        convertedOutputTables.Add(oldOutputTableId, step.OutputTableId.GetValueOrDefault(0));
                    }
                }

                foreach (var step in campaign.Steps)
                {
                    if (step.Query != null)
                    {
                        bool converted = false;
                        foreach (var qc in step.Query.Columns)
                        {
                            if (convertedOutputTables.ContainsKey(qc.TableId))
                            {
                                qc.TableId = convertedOutputTables[qc.TableId];
                                converted = true;
                            }
                        }

                        foreach (var table in element.Element("tables").Descendants("table"))
                        {
                            long tableId = Convert.ToInt64(table.AttributeValue("tableid"));
                            string name = table.AttributeValue("name");
                            var t = manager.GetCampaignTable(name);
                            if (t != null)
                            {
                                foreach (var qc in step.Query.Columns.Where(o => o.TableId == tableId))
                                {
                                    qc.TableId = t.Id;
                                    converted = true;
                                }
                            }
                        }


                        if ((step.StepType == StepType.Output || step.StepType == StepType.RealTimeOutput) && step.Query is OutputQuery)
                        {
                            OutputQuery query = (OutputQuery)step.Query;

                            using (var dataService = new DataService(_config))
                            using (var emailService = new EmailService(_config))
                            using (var surveyManager = new SurveyManager(_config))
                            using (var contentService = new ContentService(_config))
                            {

                                if (
                                    query.OutputOption != null &&
                                    referencedTypes.Contains(query.OutputOption.OutputType) &&
                                    //query uses either RedId or TextBlockId to hold its reference(s)
                                    (query.OutputOption.RefId != null || (query.OutputOption.OutputType == OutputType.SocialMedia && query.OutputOption.TextBlockId > 0))
                                    )
                                {
                                    var referenceIds =
                                        query.OutputOption.OutputType == OutputType.SocialMedia ?
                                        new List<long>() { query.OutputOption.TextBlockId } :
                                        query.OutputOption.RefId.ToList();

                                    query.OutputOption.RefId.Clear();

                                    //check reference map against each reference type
                                    foreach (var sourceId in referenceIds)
                                    {
                                        if (
                                            !referenceMap.ContainsKey(query.OutputOption.OutputType) ||
                                            !referenceMap[query.OutputOption.OutputType].ContainsKey(sourceId))
                                        {
                                            //no match in the map, just add it back (mainly for backward compatibility)
                                            if (query.OutputOption.OutputType != OutputType.SocialMedia)
                                            {
                                                query.OutputOption.RefId.Add(sourceId);
                                            }
                                            continue;
                                        }

                                        string sourceName = referenceMap[query.OutputOption.OutputType][sourceId];

                                        switch (query.OutputOption.OutputType)
                                        {
                                            case OutputType.Coupon:
                                                var coupon = contentService.GetCouponDef(sourceName);
                                                query.OutputOption.RefId.Add(coupon != null ? coupon.Id : sourceId);
                                                break;
                                            case OutputType.Email:
                                                var email = emailService.GetEmail(sourceName);
                                                query.OutputOption.RefId.Add(email != null ? email.Id : sourceId);
                                                break;
                                            case OutputType.Message:
                                                var message = contentService.GetMessageDef(sourceName);
                                                query.OutputOption.RefId.Add(message != null ? message.Id : sourceId);
                                                break;
                                            case OutputType.Offer:
                                                var bonus = contentService.GetBonusDef(sourceName);
                                                query.OutputOption.RefId.Add(bonus != null ? bonus.Id : sourceId);
                                                break;
                                            case OutputType.Promotion:
                                                var promotion = contentService.GetPromotionByName(sourceName);
                                                query.OutputOption.RefId.Add(promotion != null ? promotion.Id : sourceId);
                                                break;
                                            case OutputType.Survey:
                                                var survey = surveyManager.RetrieveSurvey(sourceName);
                                                query.OutputOption.RefId.Add(survey != null ? survey.ID : sourceId);
                                                break;
                                            case OutputType.SocialMedia:
                                                var block = contentService.GetTextBlock(sourceName);
                                                query.OutputOption.TextBlockId = block != null ? block.Id : sourceId;
                                                break;
                                        }
                                        converted = true;
                                    }
                                }
                            }
                        }

                        if (converted)
                        {
                            manager.UpdateStep(step);
                        }
                    }
                }

                foreach (var stepElement in element.Element("stepinputs").Descendants("input"))
                {
                    long fromStep = Convert.ToInt64(stepElement.AttributeValue("fromstep"));
                    long toStep = Convert.ToInt64(stepElement.AttributeValue("tostep"));
                    MergeType? mergeType = null;
                    if (stepElement.Attribute("mergetype") != null)
                    {
                        mergeType = (MergeType)Enum.Parse(typeof(MergeType), stepElement.AttributeValue("mergetype"));
                    }
                    int? mergeOrder = null;
                    if (stepElement.Attribute("mergeorder") != null)
                    {
                        mergeOrder = Convert.ToInt32(stepElement.AttributeValue("mergeorder"));
                    }

                    campaign.Steps[convertedSteps[toStep]].Inputs.Add(convertedSteps[fromStep], mergeType, mergeOrder, true);
                }
                return campaign.Id;
            }
		}
	}
}
