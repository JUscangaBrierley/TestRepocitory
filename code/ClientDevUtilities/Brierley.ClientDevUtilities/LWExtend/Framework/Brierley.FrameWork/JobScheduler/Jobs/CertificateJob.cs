//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.JobScheduler;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.JobScheduler.Jobs
{
    public class CertificateJob : IJob
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_JOBSCHEDULER);
        private const string _className = "CertificateJob";

        private StringBuilder _report = null;

        public void SetRunID(long runID)
        {
        }

        public FrameWork.Common.ScheduleJobStatus Run(string parms)
        {
            return GenerateCertificates(parms);
        }

        public FrameWork.Common.ScheduleJobStatus Resume(string parms)
        {
            throw new NotSupportedException();
        }

        public FrameWork.Common.ScheduleJobStatus GenerateCertificates(string parms)
        {
            const string methodName = "GenerateCertificates";
            _report = new StringBuilder();
            try
            {
                using (var content = LWDataServiceUtil.ContentServiceInstance())
                {
                    XElement element = XElement.Parse(parms);

                    int quantity = 0;
                    string format = string.Empty;
                    string typeCode = string.Empty;
                    Common.ContentObjType objectType = ContentObjType.Promotion;
                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    ParseCertificateJobParms(parms, ref quantity, ref format, ref typeCode, ref objectType, ref startDate, ref endDate);

                    content.GeneratePromoCertificates(quantity, format, typeCode, objectType, startDate, endDate);
                }

                _logger.Trace(_className, methodName, "Finished generating certificates");
                _report.AppendLine("Finished generating certificates");
                _logger.Trace(_className, methodName, "end");
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Failed to generate certificates.", ex);
                throw;
            }
            return FrameWork.Common.ScheduleJobStatus.Success;
        }

        public static void ParseCertificateJobParms(string parms, ref int quantity, ref string format, ref string typeCode, ref Common.ContentObjType objectType, ref DateTime? startDate, ref DateTime? endDate)
        {
            const string methodName = "ParseCertificateJobParams";

            if (string.IsNullOrEmpty(parms))
            {
                string msg = "No parameters provided for job.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            XDocument doc = XDocument.Parse(parms);
            if (doc.Root.Name.LocalName != "CertificateJobParms")
            {
                string msg = "Malformed parameters provided for job.  Invalid root node.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            XElement xQuantity = doc.Root.Element("quantity");
            if (xQuantity == null)
            {
                string msg = "Malformed parameters provided for job.  No Quantity node found.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            quantity = int.Parse(xQuantity.Value);

            XElement xFormat = doc.Root.Element("format");
            if (xFormat == null)
            {
                string msg = "Malformed parameters provided for job.  No Format node found.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            format = xFormat.Value;

            XElement xTypeCode = doc.Root.Element("typeCode");
            if (xTypeCode == null)
            {
                string msg = "Malformed parameters provided for job.  No TypeCode node found.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            typeCode = xTypeCode.Value;

            XElement xObjectType = doc.Root.Element("objectType");
            if (xObjectType == null)
            {
                string msg = "Malformed parameters provided for job.  No ObjectType node found.";
                _logger.Error(_className, methodName, msg);
                throw new LWSchedulerException(msg) { ErrorCode = 1 };
            }
            objectType = (Common.ContentObjType)Enum.Parse(typeof(Common.ContentObjType), xObjectType.Value);

            XElement xStartDate = doc.Root.Element("startDate");
            if (xStartDate != null)
            {
                startDate = Convert.ToDateTime(xStartDate.Value);
            }

            XElement xEndDate = doc.Root.Element("endDate");
            if (xEndDate != null)
            {
                endDate = Convert.ToDateTime(xEndDate.Value);
            }
        }


        public string GetReport()
        {
            return _report == null ? string.Empty : _report.ToString();
        }

        public void RequestAbort()
        {
        }

        public void FinalizeJob(FrameWork.Common.ScheduleJobStatus jobStatus) { }
    }

    #region Job Factory
    public class CertificateJobFactory : IJobFactory
    {
        public IJob GetJob()
        {
            return new CertificateJob();
        }
    }
    #endregion
}
