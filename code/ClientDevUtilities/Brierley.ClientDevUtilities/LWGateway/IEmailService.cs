using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public interface IEmailService : IServiceBase
    {
        void ClearEmailFeedback(long id, long clearedBy);
        void ClearEmailFeedbacks(string email, long clearedBy);
        void CreateEmail(EmailDocument email);
        void CreateEmail(EmailDocument email, IEnumerable<EmailPersonalization> personalizations);
        void CreateEmail(EmailDocument email, long templateId);
        void CreateEmailAssociation(EmailAssociation email);
        void CreateEmailFeedback(EmailFeedback feedback);
        void CreateEmailLink(EmailLink emailLink);
        void CreateEmailQueue(EmailQueue emailQueue);
        void CreateMailing(Mailing mailing);
        void CreatePersonalization(EmailPersonalization personalization);
        void CreatePersonalization(long emailId, string name, string expression);
        void CreateTestList(TestList testList);
        void DeleteEmail(long emailID);
        void DeleteEmailAssociation(long Id);
        void DeleteEmailFeedback(long id);
        void DeleteEmailLink(long id);
        void DeleteEmailQueue(long ID);
        void DeletePersonalization(long emailId, string name);
        void DeletePersonalization(long emailId);
        void DeletePersonalization(EmailPersonalization personalization);
        void DeleteTestList(long id);
        List<TestList> GetAllTestLists(DateTime changedSince);
        List<TestList> GetAllTestLists();
        IList<TestList> GetAllTestListsByTemplate(long templateID);
        EmailDocument GetEmail(string name);
        EmailDocument GetEmail(long id);
        EmailAssociation GetEmailAssociation(long Id);
        List<EmailAssociation> GetEmailAssociations(PointTransactionOwnerType ownerType, long ownerId, long? rowKey);
        EmailLink GetEmailLink(long id);
        List<EmailLink> GetEmailLinks(long emailId, bool excludeInactiveLinks);
        List<EmailLink> GetEmailLinks(long emailId);
        List<EmailQueue> GetEmailQueue(CommunicationType messageType, EmailFailureType failureType);
        EmailQueue GetEmailQueue(long ID);
        List<EmailQueue> GetEmailQueueByEmailID(long emailID);
        List<EmailQueue> GetEmailQueueByEmailIDAndFailureType(long emailID, EmailFailureType emailFailureType);
        List<EmailQueueSummaryItem> GetEmailQueueSummary();
        List<EmailDocument> GetEmails();
        List<EmailDocument> GetEmails(DateTime changedSince);
        List<EmailDocument> GetEmailsByFolder(long folderId);
        List<EmailDocument> GetEmailsByTemplate(long templateId);
        Page<EmailFeedback> GetFeedbackHistory(string email, long page, long resultsPerPage);
        List<EmailFeedbackDao.FeedbackSummary> GetFeedbackSummary(string email, DateTime? minDate, bool activeOnly = true);
        Mailing GetMailing(long id);
        List<Mailing> GetMailings(long emailId);
        EmailPersonalization GetPersonalization(long emailId, string name);
        IEnumerable<EmailPersonalization> GetPersonalizations(long emailId);
        TestList GetTestList(long id);
        TestList GetTestList(string name);
        bool IsEmailSuppressed(string email);
        void UpdateEmail(EmailDocument email);
        void UpdateEmail(EmailDocument email, IEnumerable<EmailPersonalization> personalizations);
        void UpdateEmailAssociation(EmailAssociation email);
        void UpdateEmailFeedback(EmailFeedback feedback);
        void UpdateEmailLink(EmailLink emailLink);
        void UpdateEmailQueue(EmailQueue emailQueue);
        void UpdatePersonalization(EmailPersonalization personalization);
        void UpdateTestList(TestList testList);
    }
}
