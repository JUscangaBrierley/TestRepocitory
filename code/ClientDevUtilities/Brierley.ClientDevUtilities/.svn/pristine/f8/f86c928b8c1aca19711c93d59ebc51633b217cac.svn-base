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
	public class DocumentDao : DaoBase<Document>
	{
		public DocumentDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Document Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Document Retrieve(string name)
		{
			return Database.FirstOrDefault<Document>("select * from LW_Document where name = @0", name);
		}

		public List<Document> RetrieveAll()
		{
			return Database.Fetch<Document>("select * from LW_Document");
		}

		public List<Document> RetrieveAll(DocumentType documentType)
		{
			return Database.Fetch<Document>("select * from LW_Document where DocumentType = @0", documentType);
		}

		public List<Document> RetrieveAll(DocumentType documentType, DateTime changedSince)
		{
			return Database.Fetch<Document>("select * from LW_Document where DocumentType = @0 and UpdateDate >= @1", documentType, changedSince);
		}

		public List<Document> RetrieveSurveyRunnerDocuments()
		{
			return Database.Fetch<Document>(
				"select * from LW_Document where TemplateId in (select Id from LW_Template where HtmlContent like '%<contentarea name=\"SURVEYRUNNER\"%' and TemplateType = @0)", 
				TemplateType.Webpage);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
