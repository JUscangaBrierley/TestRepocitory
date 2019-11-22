using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Messaging.Messages
{
    public class EmailFeedbackMessage
    {
        public string Type { get; set; }

        public string MessageId { get; set; }

        public string TopicArn { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public string SignatureVersion { get; set; }

        public string Signature { get; set; }

        public string SigningCertURL { get; set; }

        public string UnsubscribeURL { get; set; }

        public AmazonSesBounceNotification Notification { get; set; }
    }

    /// <summary>Represents an Amazon SES bounce notification.</summary>
    public class AmazonSesBounceNotification
    {
        public string NotificationType { get; set; }
        public AmazonSesBounce Bounce { get; set; }
        public AmazonSesComplaint Complaint { get; set; }
    }

    /// <summary>Represents meta data for the bounce notification from Amazon SES.</summary>
    public class AmazonSesBounce
    {
        public string BounceType { get; set; }
        public string BounceSubType { get; set; }
        public DateTime Timestamp { get; set; }
        public List<AmazonSesBouncedRecipient> BouncedRecipients { get; set; }
    }

    public class AmazonSesComplaint
    {
        public List<AmazonSesBouncedRecipient> ComplainedRecipients { get; set; }
        public DateTime Timestamp { get; set; }
        public string FeedbackId { get; set; }
        public string UserAgent { get; set; }
        public string ComplaintFeedbackType { get; set; }
    }

    /// <summary>Represents the email address of recipients that bounced
    /// when sending from Amazon SES.</summary>
    public class AmazonSesBouncedRecipient
    {
        public string EmailAddress { get; set; }
    }
}
