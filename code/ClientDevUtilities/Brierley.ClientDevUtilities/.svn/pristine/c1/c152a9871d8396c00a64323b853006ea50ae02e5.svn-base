using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using static Brierley.FrameWork.Data.DataAccess.EmailFeedbackDao;

namespace Brierley.FrameWork.Data
{
	public class EmailService : ServiceBase
	{
		public EmailService(ServiceConfig config)
			: base(config)
		{
		}

		private LangChanContentDao _langChanContentDao;
		private ContentAttributeDao _contentAttributeDao;
		private EmailDao _emailDao;
		private EmailQueueDao _emailQueueDao;
		private EmailPersonalizationDao _emailPersonalizationDao;
		private EmailAssociationDao _emailAssociationDao;
		private MailingDao _mailingDao;
		private EmailLinkDao _linkDao;
		private TestListDao _testListDao;
		private EmailFeedbackDao _emailFeedbackDao;

		public LangChanContentDao LangChanContentDao
		{
			get
			{
				if (_langChanContentDao == null)
				{
					_langChanContentDao = new LangChanContentDao(Database, Config);
				}
				return _langChanContentDao;
			}
		}

		public ContentAttributeDao ContentAttributeDao
		{
			get
			{
				if (_contentAttributeDao == null)
				{
					_contentAttributeDao = new ContentAttributeDao(Database, Config);
				}
				return _contentAttributeDao;
			}
		}

		public EmailDao EmailDao
		{
			get
			{
				if (_emailDao == null)
				{
					_emailDao = new EmailDao(Database, Config);
				}
				return _emailDao;
			}
		}

		public EmailQueueDao EmailQueueDao
		{
			get
			{
				if (_emailQueueDao == null)
				{
					_emailQueueDao = new EmailQueueDao(Database, Config);
				}
				return _emailQueueDao;
			}
		}

		public EmailPersonalizationDao EmailPersonalizationDao
		{
			get
			{
				if (_emailPersonalizationDao == null)
				{
					_emailPersonalizationDao = new EmailPersonalizationDao(Database, Config);
				}
				return _emailPersonalizationDao;
			}
		}

		public EmailAssociationDao EmailAssociationDao
		{
			get
			{
				if (_emailAssociationDao == null)
				{
					_emailAssociationDao = new EmailAssociationDao(Database, Config);
				}
				return _emailAssociationDao;
			}
		}

		public MailingDao MailingDao
		{
			get
			{
				if (_mailingDao == null)
				{
					_mailingDao = new MailingDao(Database, Config);
				}
				return _mailingDao;
			}
		}

		public EmailLinkDao LinkDao
		{
			get
			{
				if (_linkDao == null)
				{
					_linkDao = new EmailLinkDao(Database, Config);
				}
				return _linkDao;
			}
		}

		public TestListDao TestListDao
		{
			get
			{
				if (_testListDao == null)
				{
					_testListDao = new TestListDao(Database, Config);
				}
				return _testListDao;
			}
		}

		public EmailFeedbackDao EmailFeedbackDao
		{
			get
			{
				if (_emailFeedbackDao == null)
				{
					_emailFeedbackDao = new EmailFeedbackDao(Database, Config);
				}
				return _emailFeedbackDao;
			}
		}

		public void CreateEmail(EmailDocument email)
		{
			CreateEmail(email, null);
		}

		public void CreateEmail(EmailDocument email, IEnumerable<EmailPersonalization> personalizations)
		{
			EmailDao.Create(email);
			if (personalizations != null)
			{
				foreach (var p in personalizations)
				{
					p.EmailId = email.Id;
					EmailPersonalizationDao.Create(p);
				}
			}
		}

		public void CreateEmail(EmailDocument email, long templateId)
		{
			Template template = LWDataServiceUtil.ContentServiceInstance().GetTemplate(templateId);
			if (template == null)
			{
				throw new LWException("Invalid template id provided.");
			}
			using (var transaction = Database.GetTransaction())
			{
				Document doc = new Document();
				doc.Name = email.Name;
				doc.DocumentType = DocumentType.EmailDocument;
				doc.TemplateID = templateId;
				using (var content = new ContentService(this.Config))
				{
					content.CreateDocument(doc);
					email.DocumentId = doc.ID;
					EmailDao.Create(email);
				}
				transaction.Complete();
			}
		}

		public EmailDocument GetEmail(long id)
		{
			return EmailDao.Retrieve(id);
		}

		public EmailDocument GetEmail(string name)
		{
			return EmailDao.Retrieve(name);
		}

		public List<EmailDocument> GetEmails()
		{
			return EmailDao.RetrieveAll() ?? new List<EmailDocument>();
		}

		public List<EmailDocument> GetEmailsByFolder(long folderId)
		{
			return EmailDao.RetrieveByFolder(folderId) ?? new List<EmailDocument>();
		}

		public List<EmailDocument> GetEmailsByTemplate(long templateId)
		{
			return EmailDao.RetrieveByTemplate(templateId) ?? new List<EmailDocument>();
		}

		public List<EmailDocument> GetEmails(DateTime changedSince)
		{
			return EmailDao.RetrieveAll() ?? new List<EmailDocument>();
		}

		public void UpdateEmail(EmailDocument email)
		{
			UpdateEmail(email, null);
		}

		public void UpdateEmail(EmailDocument email, IEnumerable<EmailPersonalization> personalizations)
		{
			EmailDao.Update(email);

			var current = EmailPersonalizationDao.Retrieve(email.Id);
			if (personalizations == null)
			{
				personalizations = new List<EmailPersonalization>();
			}

			//make sure email id is in sync for all personalizations
			foreach (var p in personalizations.Where(o => o.EmailId != email.Id))
			{
				p.EmailId = email.Id;
			}

			//delete any that aren't in the new collection
			foreach (var c in current)
			{
				if (personalizations.Where(o => o.Name.Equals(c.Name)).Count() == 0)
				{
					EmailPersonalizationDao.Delete(email.Id, c.Name);
				}
			}

			//add any that are new
			foreach (var p in personalizations)
			{
				if (current.Where(o => o.Name.Equals(p.Name)).Count() == 0)
				{
					//new personalization doesn't exist. create
					EmailPersonalizationDao.Create(p);
				}
			}

			//update any that have changed
			foreach (var p in personalizations)
			{
				var existing = current.Where(o => o.Name.Equals(p.Name)).FirstOrDefault();
				if (existing != null && existing.Expression != p.Expression)
				{
					EmailPersonalizationDao.Update(p);
				}
			}
		}

		public void DeleteEmail(long emailID)
		{
			if (emailID > 0)
			{
				EmailDocument email = GetEmail(emailID);
				if (email != null)
				{
					// Mailing refers to Email table
					List<Mailing> mailings = MailingDao.Retrieve(email.Id);
					if (mailings != null && mailings.Count > 0)
					{
						foreach (Mailing mailing in mailings)
						{
							MailingDao.Delete(mailing.ID);
						}
					}

					EmailPersonalizationDao.Delete(emailID);

					// Prefetch the document before we delete the Email
					using (ContentService cms = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
					{
						Document document;
						if (email.ExternalId.HasValue && email.ExternalId.Value > 0)
							document = cms.GetDocument(email.ExternalId.Value);
						else if (email.DocumentId.GetValueOrDefault() > 0)
							document = cms.GetDocument(email.DocumentId.GetValueOrDefault());
						else
							document = null;

						// Email refers to Document table
						EmailDao.Delete(emailID);

						// Now we can delete the document
						if (document != null)
							cms.DeleteDocument(document.ID);
					}
				}
			}
		}

		public void CreateEmailQueue(EmailQueue emailQueue)
		{
			EmailQueueDao.Create(emailQueue);
		}

		public EmailQueue GetEmailQueue(long ID)
		{
			return EmailQueueDao.Retrieve(ID);
		}

		public List<EmailQueue> GetEmailQueueByEmailID(long emailID)
		{
			return EmailQueueDao.RetrieveByEmailID(emailID);
		}

		public List<EmailQueue> GetEmailQueueByEmailIDAndFailureType(long emailID, EmailFailureType emailFailureType)
		{
			return (List<EmailQueue>)EmailQueueDao.RetrieveByEmailIDAndFailureType(emailID, emailFailureType);
		}

		public List<EmailQueue> GetEmailQueue(CommunicationType messageType, EmailFailureType failureType)
		{
			return (List<EmailQueue>)EmailQueueDao.Retrieve(messageType, failureType);
		}

		public List<EmailQueueSummaryItem> GetEmailQueueSummary()
		{
			return EmailQueueDao.RetrieveSummary();
		}

		public void UpdateEmailQueue(EmailQueue emailQueue)
		{
			EmailQueueDao.Update(emailQueue);
		}

		public void DeleteEmailQueue(long ID)
		{
			EmailQueueDao.Delete(ID);
		}

		public void CreateMailing(Mailing mailing)
		{
			MailingDao.Create(mailing);
		}

		public Mailing GetMailing(long id)
		{
			return MailingDao.RetrieveByMailingId(id);
		}

		public List<Mailing> GetMailings(long emailId)
		{
			return MailingDao.Retrieve(emailId);
		}

		public void CreatePersonalization(EmailPersonalization personalization)
		{
			EmailPersonalizationDao.Create(personalization);
		}

		public void CreatePersonalization(long emailId, string name, string expression)
		{
			EmailPersonalizationDao.Create(emailId, name, expression);
		}

		public EmailPersonalization GetPersonalization(long emailId, string name)
		{
			return EmailPersonalizationDao.Retrieve(emailId, name);
		}

		public IEnumerable<EmailPersonalization> GetPersonalizations(long emailId)
		{
			return EmailPersonalizationDao.Retrieve(emailId) ?? new List<EmailPersonalization>();
		}

		public void UpdatePersonalization(EmailPersonalization personalization)
		{
			EmailPersonalizationDao.Update(personalization);
		}

		public void DeletePersonalization(long emailId, string name)
		{
			EmailPersonalizationDao.Delete(emailId, name);
		}

		public void DeletePersonalization(long emailId)
		{
			EmailPersonalizationDao.Delete(emailId);
		}

		public void DeletePersonalization(EmailPersonalization personalization)
		{
			EmailPersonalizationDao.Delete(personalization.EmailId, personalization.Name);
		}

		public void CreateEmailAssociation(EmailAssociation email)
		{
			EmailAssociationDao.Create(email);
		}

		public void UpdateEmailAssociation(EmailAssociation email)
		{
			EmailAssociationDao.Update(email);
		}

		public EmailAssociation GetEmailAssociation(long Id)
		{
			return EmailAssociationDao.Retrieve(Id);
		}

		public List<EmailAssociation> GetEmailAssociations(PointTransactionOwnerType ownerType, long ownerId, long? rowKey)
		{
			List<EmailAssociation> list = EmailAssociationDao.Retrieve(ownerType, ownerId, rowKey);
			return list ?? new List<EmailAssociation>();
		}

		public void DeleteEmailAssociation(long Id)
		{
			EmailAssociationDao.Delete(Id);
		}


		public void CreateEmailLink(EmailLink emailLink)
		{
			LinkDao.Create(emailLink);
		}

		public List<EmailLink> GetEmailLinks(long emailId)
		{
			return GetEmailLinks(emailId, true);
		}

		public List<EmailLink> GetEmailLinks(long emailId, bool excludeInactiveLinks)
		{
			return LinkDao.RetrieveByEmailId(emailId, excludeInactiveLinks);
		}

		public EmailLink GetEmailLink(long id)
		{
			return LinkDao.Retrieve(id);
		}

		public void UpdateEmailLink(EmailLink emailLink)
		{
			LinkDao.Update(emailLink);
		}

		public void DeleteEmailLink(long id)
		{
			LinkDao.Delete(id);
		}

		public void CreateTestList(TestList testList)
		{
			TestListDao.Create(testList);
		}

		public TestList GetTestList(long id)
		{
			return TestListDao.Retrieve(id);
		}

		public TestList GetTestList(string name)
		{
			return TestListDao.Retrieve(name);
		}

		public void UpdateTestList(TestList testList)
		{
			TestListDao.Update(testList);
		}

		public void DeleteTestList(long id)
		{
			TestListDao.Delete(id);
		}

		public List<TestList> GetAllTestLists()
		{
			return TestListDao.RetrieveAll() ?? new List<TestList>();
		}

		public List<TestList> GetAllTestLists(DateTime changedSince)
		{
			return TestListDao.RetrieveAll(changedSince) ?? new List<TestList>();
		}

		public IList<TestList> GetAllTestListsByTemplate(long templateID)
		{
			return TestListDao.RetrieveAllByTemplate(templateID) ?? new List<TestList>();
		}

		public bool IsEmailSuppressed(string email)
		{
			DateTime minDate = DateTime.Now;

            var config = LWDataServiceUtil.GetServiceConfiguration();
			if (
				(config.EmailSuppressionSettings.UndeterminedBounceRule == null || config.EmailSuppressionSettings.UndeterminedBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.PermanentBounceRule == null || config.EmailSuppressionSettings.PermanentBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.TransientBounceRule == null || config.EmailSuppressionSettings.TransientBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.ComplaintBounceRule == null || config.EmailSuppressionSettings.ComplaintBounceRule.Type == EmailBounceRuleType.None)
				)
			{
				// all are null or otherwise explicitly set to not suppress. The email address is not suppressed
				return false;
			}

			var feedback = EmailFeedbackDao.RetrieveAll(email);
			if(feedback == null || feedback.Count ==0)
			{
				return false;
			}

			Func<Email.SuppressionSettings.SuppressionRule, EmailFeedbackType, bool> isSuppressedForType = (Email.SuppressionSettings.SuppressionRule rule, EmailFeedbackType type) =>
			{
				if (rule != null)
				{
					switch (rule.Type)
					{
						case EmailBounceRuleType.Strict:
							{
								var summary = feedback.Where(o => o.FeedbackType == type && !o.ClearedBy.HasValue);
								if (summary.Count() > 0)
								{
									return true;
								}
							}
							break;
						case EmailBounceRuleType.Sliding:
							{
								var summary = feedback.Where(o => o.FeedbackType == type && o.FeedbackDate >= DateTime.Now.AddMinutes(rule.Interval * -1) && !o.ClearedBy.HasValue);
								if (summary.Count() > 0)
								{
									return true;
								}
							}
							break;
						default:
							break;
					}
				}
				return false;
			};

			//in order of most likely to give a suppression (permanent, complaint, transient, undetermined - why not?)
			return
				isSuppressedForType(config.EmailSuppressionSettings.PermanentBounceRule, EmailFeedbackType.Permanent) ||
				isSuppressedForType(config.EmailSuppressionSettings.ComplaintBounceRule, EmailFeedbackType.Complaint) ||
				isSuppressedForType(config.EmailSuppressionSettings.TransientBounceRule, EmailFeedbackType.Transient) ||
				isSuppressedForType(config.EmailSuppressionSettings.UndeterminedBounceRule, EmailFeedbackType.Undetermined);
		}

		//this version will hit the database once for each feedback type. Assuming a typical email address receives very few feedback records, this is probably the least
		//efficient way to check for suppression.
		/*
		public bool IsEmailSuppressed(string email)
		{
			DateTime minDate = DateTime.Now;

			Func<Email.SuppressionSettings.SuppressionRule, EmailFeedbackType, bool> isSuppressedForType = (Email.SuppressionSettings.SuppressionRule rule, EmailFeedbackType type) =>
			{
				if (rule != null)
				{
					switch (rule.Type)
					{
						case EmailBounceRuleType.Strict:
							{
								var summary = EmailFeedbackDao.RetrieveSummary(email, type, DateTimeUtil.MinValue, true);
								if (summary.Count > 0)
								{
									return true;
								}
							}
							break;
						case EmailBounceRuleType.Sliding:
							{
								var summary = EmailFeedbackDao.RetrieveSummary(email, type, DateTime.Now.AddMinutes(rule.Interval * -1), true);
								if (summary.Count > 0)
								{
									return true;
								}
							}
							break;
						default:
							break;
					}
				}
				return false;
			};

			//1. Get each rule setting. 
			//		any "Strict" rules means we need to consider all feedback history. 
			//		Email address is not suppressed if all rules are set to "None"
			//		use the minimum sliding date when all rules are set to "Sliding"

			var config = LWDataServiceUtil.GetServiceConfiguration();
			if (
				(config.EmailSuppressionSettings.UndeterminedBounceRule == null || config.EmailSuppressionSettings.UndeterminedBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.PermanentBounceRule == null || config.EmailSuppressionSettings.PermanentBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.TransientBounceRule == null || config.EmailSuppressionSettings.TransientBounceRule.Type == EmailBounceRuleType.None) &&
				(config.EmailSuppressionSettings.ComplaintBounceRule == null || config.EmailSuppressionSettings.ComplaintBounceRule.Type == EmailBounceRuleType.None)
				)
			{
				// all are null or otherwise explicitly set to not suppress. The email address is not suppressed
				return false;
			}

			//in order of most likely to give a suppression (permanent, complaint, transient, undetermined - why not?)
			return
				isSuppressedForType(config.EmailSuppressionSettings.PermanentBounceRule, EmailFeedbackType.Permanent) ||
				isSuppressedForType(config.EmailSuppressionSettings.ComplaintBounceRule, EmailFeedbackType.Complaint) ||
				isSuppressedForType(config.EmailSuppressionSettings.TransientBounceRule, EmailFeedbackType.Transient) ||
				isSuppressedForType(config.EmailSuppressionSettings.UndeterminedBounceRule, EmailFeedbackType.Undetermined);
		}
		*/

		public PetaPoco.Page<EmailFeedback> GetFeedbackHistory(string email, long page, long resultsPerPage)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}

			return EmailFeedbackDao.Retrieve(email, page, resultsPerPage);
		}

		public List<FeedbackSummary> GetFeedbackSummary(string email, DateTime? minDate, bool activeOnly = true)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}

			return EmailFeedbackDao.RetrieveSummary(email, minDate.GetValueOrDefault(DateTimeUtil.MinValue), activeOnly);
		}

		public void CreateEmailFeedback(EmailFeedback feedback)
		{
			EmailFeedbackDao.Create(feedback);
		}

		public void UpdateEmailFeedback(EmailFeedback feedback)
		{
			EmailFeedbackDao.Update(feedback);
		}

		public void DeleteEmailFeedback(long id)
		{
			EmailFeedbackDao.Delete(id);
		}

		public void ClearEmailFeedback(long id, long clearedBy)
		{
			EmailFeedbackDao.ClearEmailFeedback(id, clearedBy);
		}

		public void ClearEmailFeedbacks(string email, long clearedBy)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException("email");
			}
			EmailFeedbackDao.ClearEmailFeedbacks(email, clearedBy);
		}
	}
}
