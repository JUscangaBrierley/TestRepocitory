using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TemplateDao : DaoBase<Template>
	{
		public TemplateDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
		
		public Template Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Template Retrieve(string name)
		{
			return Database.FirstOrDefault<Template>("select * from LW_Template where name = @0", name);
		}

		public List<Template> Retrieve(TemplateType templateType)
		{
			return Database.Fetch<Template>("select * from LW_Template where TemplateType = @0", templateType);
		}

		public List<Template> Retrieve(TemplateType templateType, DateTime changedSince)
		{
			return Database.Fetch<Template>("select * from LW_Template where TemplateType = @0 and UpdateDate >= @1", templateType, changedSince);
		}

		public List<Template> RetrieveByFolder(long folderId)
		{
			return Database.Fetch<Template>("select * from LW_Template where FolderId = @0", folderId);
		}

		public List<Template> RetrieveSurveyRunnerTemplates()
		{
			return Database.Fetch<Template>("select * from LW_Template where HtmlContent like '%<contentarea name=\"SURVEYRUNNER\"%' and TemplateType = @0", TemplateType.Webpage);
		}

		public List<Template> RetrieveAll()
		{
			return Database.Fetch<Template>("select * from LW_Template");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
