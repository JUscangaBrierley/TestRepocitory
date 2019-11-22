using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class SmsService : ServiceBase
	{
        public SmsService(ServiceConfig config)
			: base(config)
		{
		}

        private LangChanContentDao _langChanContentDao;
        private ContentAttributeDao _contentAttributeDao;
        private SmsDao _smsDao;
        private SmsQueueDao _smsQueueDao;
        private SmsPersonalizationDao _smsPersonalizationDao;

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

        public SmsDao SmsDao
        {
            get
            {
                if (_smsDao == null)
                {
                    _smsDao = new SmsDao(Database, Config);
                }
                return _smsDao;
            }
        }

        public SmsQueueDao SmsQueueDao
        {
            get
            {
                if (_smsQueueDao == null)
                {
                    _smsQueueDao = new SmsQueueDao(Database, Config);
                }
                return _smsQueueDao;
            }
        }

        public SmsPersonalizationDao SmsPersonalizationDao
        {
            get
            {
                if (_smsPersonalizationDao == null)
                {
                    _smsPersonalizationDao = new SmsPersonalizationDao(Database, Config);
                }
                return _smsPersonalizationDao;
            }
        }

		public void CreateSmsMessage(SmsDocument sms)
		{
            CreateSmsMessage(sms, null);
		}

        public void CreateSmsMessage(SmsDocument sms, IEnumerable<SmsPersonalization> personalizations)
		{
            SmsDao.Create(sms);
            if (personalizations != null)
            {
                foreach (var p in personalizations)
                {
                    p.SmsId = sms.Id;
                    SmsPersonalizationDao.Create(p);
                }
            }
		}

		public SmsDocument GetSmsMessage(long id)
		{
			return SmsDao.Retrieve(id);
		}

		public SmsDocument GetSmsMessage(string name)
		{
			return SmsDao.Retrieve(name);
		}

		public List<SmsDocument> GetSmsMessages()
		{
            List<SmsDocument> smsMessages = SmsDao.RetrieveAll();
            if (smsMessages == null)
            {
                smsMessages = new List<SmsDocument>();
            }
            return smsMessages;
		}

		public List<SmsDocument> GetSmsByFolder(long folderId)
		{
            List<SmsDocument> smsMessages = SmsDao.RetrieveByFolder(folderId);
            if (smsMessages == null)
            {
                smsMessages = new List<SmsDocument>();
            }
            return smsMessages;
		}

		public List<SmsDocument> GetSmsMessages(DateTime changedSince)
		{
            List<SmsDocument> smsMessages = SmsDao.RetrieveAll();
            if (smsMessages == null)
            {
                smsMessages = new List<SmsDocument>();
            }
            return smsMessages;
		}

		public void UpdateSmsMessage(SmsDocument sms)
		{
			UpdateSmsMessage(sms, null);
		}

		public void UpdateSmsMessage(SmsDocument sms, IEnumerable<SmsPersonalization> personalizations)
		{
            SmsDao.Update(sms);

            var current = SmsPersonalizationDao.Retrieve(sms.Id);
            if (personalizations == null)
            {
                personalizations = new List<SmsPersonalization>();
            }

            //make sure email id is in sync for all personalizations
            foreach (var p in personalizations.Where(o => o.SmsId != sms.Id))
            {
                p.SmsId = sms.Id;
            }

            //delete any that aren't in the new collection
            foreach (var c in current)
            {
                if (personalizations.Where(o => o.Name.Equals(c.Name)).Count() == 0)
                {
                    SmsPersonalizationDao.Delete(sms.Id, c.Name);
                }
            }

            //add any that are new
            foreach (var p in personalizations)
            {
                if (current.Where(o => o.Name.Equals(p.Name)).Count() == 0)
                {
                    //new personalization doesn't exist. create
                    SmsPersonalizationDao.Create(p);
                }
            }

            //update any that have changed
            foreach (var p in personalizations)
            {
                var existing = current.Where(o => o.Name.Equals(p.Name)).FirstOrDefault();
                if (existing != null && existing.Expression != p.Expression)
                {
                    SmsPersonalizationDao.Update(p);
                }
            }
		}

		public void DeleteSmsMessage(long smsID)
		{
            if (smsID > 0)
            {
                SmsDocument sms = GetSmsMessage(smsID);
                if (sms != null)
                {
                    SmsPersonalizationDao.Delete(smsID);

                    // Prefetch the document before we delete the Email
                    using (ContentService cms = LWDataServiceUtil.ContentServiceInstance(Organization, Environment))
                    {
                        Document document = sms.ExternalId.HasValue && sms.ExternalId.Value > 0 ? cms.GetDocument(sms.ExternalId.Value) : null;

                        // Email refers to Document table
                        SmsDao.Delete(smsID);

                        // Now we can delete the document
                        if (document != null)
                            cms.DeleteDocument(document.ID);
                    }
                }
            }
		}

		public void CreateSmsQueue(SmsQueue smsQueue)
		{
			SmsQueueDao.Create(smsQueue);
		}

		public SmsQueue GetSmsQueue(long ID)
		{
            return SmsQueueDao.Retrieve(ID);
		}

        public List<SmsQueue> GetSmsQueueBySmsID(long smsID)
		{
            return SmsQueueDao.RetrieveBySmsID(smsID);
		}

        public List<SmsQueue> GetSmsQueueBySmsIDAndFailureType(long smsID, SmsFailureType smsFailureType)
		{
            return (List<SmsQueue>)SmsQueueDao.RetrieveBySmsIDAndFailureType(smsID, smsFailureType);
		}

		public List<SmsQueue> GetSmsQueue(CommunicationType messageType, SmsFailureType smsFailureType)
		{
			return (List<SmsQueue>)SmsQueueDao.Retrieve(messageType, smsFailureType);
		}

		public List<SmsQueueSummaryItem> GetSmsQueueSummary()
		{
            return SmsQueueDao.RetrieveSummary();
		}

        public void UpdateSmsQueue(SmsQueue smsQueue)
		{
            SmsQueueDao.Update(smsQueue);
		}

		public void DeleteSmsQueue(long ID)
		{
            SmsQueueDao.Delete(ID);
		}

		public void CreatePersonalization(SmsPersonalization personalization)
		{
			SmsPersonalizationDao.Create(personalization);
		}

		public void CreatePersonalization(long smsId, string name, string expression)
		{
            SmsPersonalizationDao.Create(smsId, name, expression);
		}

		public SmsPersonalization GetPersonalizaion(long smsId, string name)
		{
            return SmsPersonalizationDao.Retrieve(smsId, name);
		}

        public IEnumerable<SmsPersonalization> GetPersonalizations(long smsId)
		{
            return SmsPersonalizationDao.Retrieve(smsId) ?? new List<SmsPersonalization>();
		}

        public void UpdatePersonalization(SmsPersonalization personalization)
		{
            SmsPersonalizationDao.Update(personalization);
		}

		public void DeletePersonalization(long emailId, string name)
		{
            SmsPersonalizationDao.Delete(emailId, name);
		}

		public void DeletePersonalization(long emailId)
		{
			SmsPersonalizationDao.Delete(emailId);
		}

        public void DeletePersonalization(SmsPersonalization personalization)
		{
            SmsPersonalizationDao.Delete(personalization.SmsId, personalization.Name);
		}
	}
}
