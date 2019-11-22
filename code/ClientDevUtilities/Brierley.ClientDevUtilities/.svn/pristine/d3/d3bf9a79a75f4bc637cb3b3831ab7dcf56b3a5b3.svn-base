using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.WorkflowService.Client;

namespace Brierley.FrameWork.LWIntegration.Util
{
    public static class EmailDesignBriefUtil
    {
        #region Fields
        private const string _className = "EmailDesignBriefUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        #endregion

        private static WorkflowAttributeDef LookupAttribute(IList<WorkflowAttributeDef> atts, string attName)
        {
            WorkflowAttributeDef att = null;
            foreach (WorkflowAttributeDef a in atts)
            {
                if (a.Name == attName)
                {
                    att = a;
                    break;
                }
            }
            return att;
        }

        public static long Start(EmailDesignBrief designBrief, string notificationEmail, List<MpaProperty> stepData = null)
        {
            string methodName = "Start";
           
                #region Create Email
                ILWEmailService svc = LWDataServiceUtil.EmailServiceInstance();
                Template template = LWDataServiceUtil.ContentServiceInstance().GetTemplate(designBrief.TemplateName);
                EmailDocument email = new EmailDocument()
                {
                    Name = designBrief.Name,
                    Description = designBrief.EmailDescription,
                    EmailType = MailingTypes.Batch,
                    Subject = "TODO: Change subject",
                    FromName = "TODO: Change from name",
                    FromEmail = "todo@changeit.com",
                    ReplyTo = "todo@changeit.com",
                    BounceEmail = "todo@changeit.com",
                    ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable,

                };
                if (svc.GetEmail(email.Name) == null)
                {
                    email = svc.CreateEmail(template.ID, email);
                }
                #endregion

                #region Start Workflow
                ILWDataService ds = LWDataServiceUtil.DataServiceInstance();
                WorkflowDef workflow = ds.GetWorkflowDef(designBrief.WorkflowName, false);
                if (workflow == null)
                {
                    string errMsg = string.Format("Unable to retrieve workflow {0}.", designBrief.WorkflowName);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWException(errMsg);
                }

                IList<WorkflowAttributeDef> atts = ds.GetAllWorkflowAttributeDefsByWorkflowID(workflow.ID);
                List<WorkflowServiceParm> parms = new List<WorkflowServiceParm>();

                // add the email name
                WorkflowAttributeDef att = LookupAttribute(atts, "Email Name");
                if (att != null)
                {
                    parms.Add(new WorkflowServiceParm(att.Name, designBrief.Name, WorkflowAttributeDataType.String));
                }

                // check all the content attributes
                foreach (ContentAttribute ca in designBrief.Attributes)
                {                    
                    ContentAttributeDef aDef = ds.GetContentAttributeDef(ca.ContentAttributeDefId);
                    MpaProperty prop = null;
                    if (MpaProperty.PropertyExists(stepData, aDef.Name))
                    {
                        prop = MpaProperty.GetProperty(stepData, aDef.Name);
                    }
                    //if (stepData != null && stepData.Count > 0)
                    //{
                    //    var props = (from x in stepData where x.Name == aDef.Name select x);
                    //    if (props != null && props.Count() > 0)
                    //    {
                    //        prop = props.ElementAt(0);
                    //    }
                    //}
                    object caValue = prop != null ? prop.Value : ca.Value;
                    att = LookupAttribute(atts, aDef.Name);
                    if (att != null && !string.IsNullOrWhiteSpace(caValue.ToString()))
                    {
                        object value = null;
                        switch (att.DataType)
                        {
                            case WorkflowAttributeDataType.String:
                                value = caValue as string;
                                break;
                            case WorkflowAttributeDataType.Long:
                                value = long.Parse(caValue.ToString());
                                break;
                            case WorkflowAttributeDataType.Decimal:
                                value = decimal.Parse(caValue.ToString());
                                break;
                            case WorkflowAttributeDataType.DateTime:
                                value = DateTime.Parse(caValue.ToString());
                                break;
                            case WorkflowAttributeDataType.Boolean:
                                value = bool.Parse(caValue.ToString());
                                break;

                        }
                        parms.Add(new WorkflowServiceParm(att.Name, value, att.DataType));
                    }
                }

                string serviceUrl = LWConfigurationUtil.GetConfigurationValue("WorkflowServiceURL");
                if (string.IsNullOrWhiteSpace(serviceUrl))
                {
                    string errMsg = string.Format("Unable to retrieve workflow service Url.  Please define property WorkflowServiceURL.");
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWException(errMsg);
                }
                WorkflowServiceClient client = new WorkflowServiceClient(serviceUrl);

                try
                {
                    long wflwid = client.ScheduleImmediateWorkflow(designBrief.WorkflowName, notificationEmail, parms);
                    string msg = string.Format("Workflow {0} has been started successfully.  The workflow id is {1}.", workflow.Name, wflwid);
                    _logger.Trace(_className, methodName, msg);
                    return wflwid;
                }
                catch (Exception ex)
                {
                    string errMsg = string.Format("Unable to start workflow {0} successfully.  Error message: {1}", workflow.Name, ex.Message);
                    _logger.Error(_className, methodName, errMsg, ex);
                    throw new LWException(errMsg, ex);                    
                }
                #endregion
                                       
        }
    }
}
