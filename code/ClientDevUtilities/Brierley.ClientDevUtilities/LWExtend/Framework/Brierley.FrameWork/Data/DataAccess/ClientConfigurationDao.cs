using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ClientConfigurationDao : DaoBase<ClientConfiguration>
	{
		public ClientConfigurationDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public ClientConfiguration Retrieve(string key)
		{
			return Database.FirstOrDefault<ClientConfiguration>("select * from LW_ClientConfiguration where ckey = @0", key);
		}

		public List<ClientConfiguration> RetrieveAllExternal()
		{
			return Database.Fetch<ClientConfiguration>("select * from LW_ClientConfiguration where ExternalValue = 1");
		}

		public List<ClientConfiguration> RetrieveAllExternalByFolder(long folderId)
		{
			return Database.Fetch<ClientConfiguration>("select * from LW_ClientConfiguration where ExternalValue = 1 and FolderId = @0", folderId);
		}

		public List<ClientConfiguration> RetrieveChangedObjects(DateTime since, bool onlyExternal)
		{
			string sql = string.Format("select * from LW_ClientConfiguration where UpdateDate >= @0{0}", onlyExternal ? " and ExternalValue = 1" : string.Empty);
			return Database.Fetch<ClientConfiguration>(sql, since);
		}

		public List<ClientConfiguration> RetrieveAll()
		{
			return Database.Fetch<ClientConfiguration>("select * from LW_ClientConfiguration");
		}

		public List<ClientConfiguration> RetrieveAllLike(string keyPattern)
		{
			if (!keyPattern.Contains("%"))
			{
				keyPattern = string.Format("%{0}%", keyPattern);
			}
			return Database.Fetch<ClientConfiguration>("select * from LW_ClientConfiguration where ckey like @0", keyPattern);
		}

		public List<string> RetrieveAllKeys()
		{
			return Database.Fetch<string>("select ckey from LW_ClientConfiguration");
		}

		public void Delete(string key)
		{
			var entity = Retrieve(key);
			if (entity != null)
			{
				DeleteEntity(entity);
			}
		}
	}
}
