using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class TestingDataService : ServiceBase
	{
		private PromoTestSetDao _promoTestSetDao;
		private PromoTestMemberDao _promoTestMemberDao;
		private PromoDataFileDao _promoDataFileDao;
		private PromoTemplateDao _promoTemplateDao;
		private PromoMappingFileDao _promoMappingFileDao;

		private PromoTestSetDao PromoTestSetDao
		{
			get
			{
				if (_promoTestSetDao == null)
				{
					_promoTestSetDao = new PromoTestSetDao(Database, Config);
				}
				return _promoTestSetDao;
			}
		}

		private PromoTestMemberDao PromoTestMemberDao
		{
			get
			{
				if (_promoTestMemberDao == null)
				{
					_promoTestMemberDao = new PromoTestMemberDao(Database, Config);
				}
				return _promoTestMemberDao;
			}
		}

		private PromoTemplateDao PromoTemplateDao
		{
			get
			{
				if (_promoTemplateDao == null)
				{
					_promoTemplateDao = new PromoTemplateDao(Database, Config);
				}
				return _promoTemplateDao;
			}
		}

		private PromoDataFileDao PromoDataFileDao
		{
			get
			{
				if (_promoDataFileDao == null)
				{
					_promoDataFileDao = new PromoDataFileDao(Database, Config);
				}
				return _promoDataFileDao;
			}
		}

		private PromoMappingFileDao PromoMappingFileDao
		{
			get
			{
				if (_promoMappingFileDao == null)
				{
					_promoMappingFileDao = new PromoMappingFileDao(Database, Config);
				}
				return _promoMappingFileDao;
			}
		}

		public TestingDataService(ServiceConfig config)
			: base(config)
		{
		}

		public void CreatePromoTestSet(PromoTestSet set)
		{
			PromoTestSetDao.Create(set);
		}

		public void UpdatePromoTestSet(PromoTestSet set)
		{
			PromoTestSetDao.Update(set);
		}

		public PromoTestSet GetPromoTestSet(long id)
		{
			return PromoTestSetDao.Retrieve(id);
		}

		public PromoTestSet GetPromoTestSet(string name)
		{
			return PromoTestSetDao.Retrieve(name);
		}

		public IList<PromoTestSet> GetPopulatedPromoTestSets()
		{
			return PromoTestSetDao.RetrievePopulated();
		}

		public IList<PromoTestSet> GetAllPromoTestSets()
		{
			return PromoTestSetDao.RetrieveAll();
		}

		public IList<PromoTestSet> GetAllPromoTestSets(DateTime changedSince)
		{
			return PromoTestSetDao.RetrieveAll(changedSince);
		}

		public IList<PromoTestSet> GetPromoTestSetsByFolder(long folderId)
		{
			return PromoTestSetDao.RetrieveByFolder(folderId);
		}

		public void DeletePromoTestSet(long id)
		{
			using (var txn = Database.GetTransaction())
			{
				var set = GetPromoTestSet(id);
				if (set != null)
				{
					var members = GetPromoTestMembers(set.Id);
					if (members != null && members.Count > 0)
					{
						foreach (var member in members)
						{
							PromoTestMemberDao.Delete(member.Id);
						}
					}
					PromoTestSetDao.Delete(id);
				}
				txn.Complete();
			}
		}

		public void CreatePromoTestMember(PromoTestMember member)
		{
			PromoTestMemberDao.Create(member);
		}

		public void CreatePromoTestMembers(IEnumerable<PromoTestMember> members)
		{
			PromoTestMemberDao.Create(members);
		}

		public PromoTestMember GetPromoTestMember(long id)
		{
			return PromoTestMemberDao.Retrieve(id);
		}

		public IList<PromoTestMember> GetPromoTestMembers(long setId)
		{
			return PromoTestMemberDao.RetrieveBySet(setId);
		}

		public void DeletePromoTestMember(long id)
		{
			PromoTestMemberDao.Delete(id);
		}

		public void DeletePromoTestMembersBySetId(long setId)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance(Organization, Environment))
			{
				IList<PromoTestMember> testMembers = GetPromoTestMembers(setId);
				var ipcodes = (from x in testMembers select x.IpCode).Distinct<long>();
				foreach (var ipcode in ipcodes)
				{
					loyalty.DeleteMember(ipcode, true);
				}
				PromoTestMemberDao.DeleteBySetId(setId);
			}
		}

		public void CreatePromoTemplate(PromoTemplate template)
		{
			PromoTemplateDao.Create(template);
		}

		public void UpdatePromoTemplate(PromoTemplate template)
		{
			PromoTemplateDao.Update(template);
		}

		public PromoTemplate GetPromoTemplate(long id)
		{
			return PromoTemplateDao.Retrieve(id);
		}

		public PromoTemplate GetPromoTemplate(string name)
		{
			return PromoTemplateDao.Retrieve(name);
		}

		public IList<PromoTemplate> GetAllPromoTemplates()
		{
			return PromoTemplateDao.RetrieveAll();
		}

		public IList<string> GetPromoTemplateTypes()
		{
			return PromoTemplateDao.RetrievePromoTemplateTypes();
		}

		public void DeletePromoTemplate(long id)
		{
			var template = GetPromoTemplate(id);
			if (template != null)
			{
				PromoTemplateDao.Delete(id);
			}
		}

		public void CreatePromoDataFile(PromoDataFile dataFile)
		{
			PromoDataFileDao.Create(dataFile);
			CacheManager.Add(CacheRegions.PromoDataFileByName, dataFile.Name, dataFile);
		}

		public void UpdatePromoDataFile(PromoDataFile dataFile)
		{
			PromoDataFileDao.Update(dataFile);
			CacheManager.Update(CacheRegions.PromoDataFileByName, dataFile.Name, dataFile);
		}

		public PromoDataFile GetPromoDataFile(long id)
		{
			PromoDataFile dataFile = PromoDataFileDao.Retrieve(id);
			if (dataFile != null)
			{
				CacheManager.Update(CacheRegions.PromoDataFileByName, dataFile.Name, dataFile);
			}
			return dataFile;
		}

		public PromoDataFile GetPromoDataFile(string name)
		{
			PromoDataFile dataFile = (PromoDataFile)CacheManager.Get(CacheRegions.PromoDataFileByName, name);
			if (dataFile == null)
			{
				dataFile = PromoDataFileDao.Retrieve(name);
				if (dataFile != null)
				{
					CacheManager.Update(CacheRegions.PromoDataFileByName, dataFile.Name, dataFile);
				}
			}
			return dataFile;
		}

		public IList<PromoDataFile> GetAllPromoDataFiles()
		{
			IList<PromoDataFile> fileList = PromoDataFileDao.RetrieveAll() ?? new List<PromoDataFile>();
			foreach (PromoDataFile dataFile in fileList)
			{
				CacheManager.Update(CacheRegions.PromoDataFileByName, dataFile.Name, dataFile);
			}
			return fileList;
		}

		public void DeletePromoDataFile(long id)
		{
			var dataFile = GetPromoDataFile(id);
			if (dataFile != null)
			{
				PromoDataFileDao.Delete(id);
				CacheManager.Remove(CacheRegions.PromoDataFileByName, dataFile.Name);
			}

		}

		public void CreatePromoMappingFile(PromoMappingFile mappingFile)
		{
			PromoMappingFileDao.Create(mappingFile);
			CacheManager.Add(CacheRegions.PromoMappingFileByName, mappingFile.Name, mappingFile);
		}

		public void UpdatePromoMappingFile(PromoMappingFile mappingFile)
		{
			PromoMappingFileDao.Update(mappingFile);
			CacheManager.Update(CacheRegions.PromoMappingFileByName, mappingFile.Name, mappingFile);
		}

		public PromoMappingFile GetPromoMappingFile(long id)
		{
			PromoMappingFile mappingFile = PromoMappingFileDao.Retrieve(id);
			if (mappingFile != null)
			{
				CacheManager.Update(CacheRegions.PromoMappingFileByName, mappingFile.Name, mappingFile);
			}
			return mappingFile;
		}

		public PromoMappingFile GetPromoMappingFile(string name)
		{
			PromoMappingFile mappingFile = (PromoMappingFile)CacheManager.Get(CacheRegions.PromoMappingFileByName, name);
			if (mappingFile == null)
			{
				mappingFile = PromoMappingFileDao.Retrieve(name);
				if (mappingFile != null)
				{
					CacheManager.Update(CacheRegions.PromoMappingFileByName, mappingFile.Name, mappingFile);
				}
			}
			return mappingFile;
		}

		public IList<PromoMappingFile> GetAllPromoMappingFiles()
		{
			IList<PromoMappingFile> fileList = PromoMappingFileDao.RetrieveAll() ?? new List<PromoMappingFile>();
			foreach (PromoMappingFile mappingFile in fileList)
			{
				CacheManager.Update(CacheRegions.PromoMappingFileByName, mappingFile.Name, mappingFile);
			}
			return fileList;
		}

		public void DeletePromoMappingFile(long id)
		{
			var mappingFile = GetPromoMappingFile(id);
			if (mappingFile != null)
			{
				PromoMappingFileDao.Delete(id);
				CacheManager.Remove(CacheRegions.PromoMappingFileByName, mappingFile.Name);
			}
		}
	}
}
