using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Messaging.Consumers
{
    public class EmailFeedbackQueueConsumer : IConsumer
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_QueueProcessor);
        private static string _className = "EmailFeedbackQueueConsumer";

        public void Consume(object msg)
        {
            const string methodName = "Consume";

            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }

            var message = msg as EmailFeedbackMessage;

            if (message == null)
            {
                throw new Exception(string.Format("Failed to cast message type {0} to EmailFeedbackMessage", message.GetType().ToString()));
            }

            _logger.Debug("EmailFeedbackQueueConsumer", "Consume", string.Format("consuming message {0}", message.MessageId.ToString()));

            if (message.Notification == null)
            {
                throw new Exception("Cannot consume email feedback message. The notification is null");
            }

            if (message.Notification.Bounce == null && message.Notification.Complaint == null)
            {
                //notification is for email feedback, but it is not a bounce. Nothing to do here
                _logger.Debug(_className, methodName, string.Format("Notification message {0}, type {1}, is not a bounce or complaint notification. Exiting.", message.MessageId, message.Notification.NotificationType));
                return;
            }

            List<AmazonSesBouncedRecipient> recipients = null;
            DateTime timestamp = DateTime.Now;
            EmailFeedbackType feedbackType = EmailFeedbackType.Unknown;
            EmailFeedbackSubtype subType = EmailFeedbackSubtype.Unknown;

            if (message.Notification.Bounce != null)
            {
                //bounce
                var bounce = message.Notification.Bounce;

                recipients = bounce.BouncedRecipients;

                if (!Enum.TryParse<EmailFeedbackType>(bounce.BounceType, true, out feedbackType))
                {
                    throw new Exception(string.Format("Cannot consume email feedback message. The bounce notification is of an unknown type", bounce.BounceType));
                }

                if (!Enum.TryParse<EmailFeedbackSubtype>(bounce.BounceSubType, true, out subType))
                {
                    throw new Exception(string.Format("Cannot consume email feedback message. The bounce notification is of an unknown subtype", bounce.BounceSubType));
                }

                timestamp = bounce.Timestamp;
            }
            else
            {
                //complaint
                var complaint = message.Notification.Complaint;

                recipients = complaint.ComplainedRecipients;

                feedbackType = EmailFeedbackType.Complaint;

                if (!Enum.TryParse<EmailFeedbackSubtype>(complaint.ComplaintFeedbackType, true, out subType))
                {
                    throw new Exception(string.Format("Cannot consume email feedback message. The complaint notification is of an unknown subtype", complaint.ComplaintFeedbackType));
                }

                timestamp = complaint.Timestamp;
            }


            if (recipients == null || recipients.Count == 0)
            {
                //highly unlikely to happen. Just in case, though...
                _logger.Debug(_className, methodName, string.Format("Notification message {0}, has no recipients. Exiting.", message.MessageId));
                return;
            }


            if (feedbackType == EmailFeedbackType.Complaint && subType == EmailFeedbackSubtype.NotSpam)
            {
                //this particular type does not need to be logged. The email went into the recipient's spam folder, the recipient marked it as "not spam" and a notification was sent back
                _logger.Debug(_className, methodName, string.Format("Notification message {0}, is a \"not-spam\" notification. Exiting.", message.MessageId));
                return;
            }

            using (var svc = LWDataServiceUtil.EmailServiceInstance())
            {
                foreach (var recipient in recipients)
                {
                    if (string.IsNullOrEmpty(recipient.EmailAddress))
                    {
                        continue;
                    }

                    //check first for existing, matching feedback record. SQS does not have immediate consistency; messages may be delivered more than once
                    var history = svc.GetFeedbackHistory(recipient.EmailAddress, 1, 10);
                    if (history.TotalItems > 0)
                    {
                        if (history.Items.Where(o => o.FeedbackDate == timestamp && o.FeedbackType == feedbackType && o.FeedbackSubtype == subType).Count() > 0)
                        {
                            //duplicate. skip
                            _logger.Debug(_className, methodName, string.Format("Notification message {0}, has already been processed. Exiting.", message.MessageId));
                            continue;
                        }
                    }

                    var feedback = new EmailFeedback()
                    {
                        EmailAddress = recipient.EmailAddress,
                        FeedbackDate = timestamp,
                        FeedbackType = feedbackType,
                        FeedbackSubtype = subType
                    };

                    svc.CreateEmailFeedback(feedback);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
