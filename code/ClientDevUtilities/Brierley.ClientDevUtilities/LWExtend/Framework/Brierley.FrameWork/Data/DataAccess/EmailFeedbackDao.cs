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
	public class EmailFeedbackDao : DaoBase<EmailFeedback>
	{
		public class FeedbackSummary
		{
			public EmailFeedbackType FeedbackType { get; set; }

			public int Count { get; set; }
		}

		public EmailFeedbackDao(Database database, ServiceConfig config)
            : base(database, config)
        {
		}

		public List<EmailFeedback> RetrieveAll(string email)
		{
			return Database.Fetch<EmailFeedback>("select * from LW_EmailFeedback where lower(EmailAddress) = @0 order by FeedbackDate desc", email);
		}

		public PetaPoco.Page<EmailFeedback> Retrieve(string email, long page, long resultsPerPage)
		{
			return Database.Page<EmailFeedback>(page, resultsPerPage, "select f.* from LW_EmailFeedback f where lower(f.EmailAddress) = @0 order by f.FeedbackDate desc", email.ToLower());
		}

		public List<FeedbackSummary> RetrieveSummary(string email, DateTime minDate, bool activeOnly)
		{
			return Database.Fetch<FeedbackSummary>("select FeedbackType, count(*) as Count from LW_EmailFeedback where lower(EmailAddress) = @0 and (@1 = 0 or ClearedBy is null) and FeedbackDate >= @2 group by FeedbackType", email.ToLower(), activeOnly ? 1 : 0, minDate);
		}

		public List<FeedbackSummary> RetrieveSummary(string email, EmailFeedbackType feedbackType, DateTime minDate, bool activeOnly)
		{
			return Database.Fetch<FeedbackSummary>("select FeedbackType, count(*) as Count from LW_EmailFeedback where lower(EmailAddress) = @0 and (@1 = 0 or ClearedBy is null) and FeedbackDate >= @2 and FeedbackType = @3 group by FeedbackType", email.ToLower(), activeOnly ? 1 : 0, minDate, feedbackType);
		}

		public void ClearEmailFeedback(long id, long clearedBy)
		{
			Database.Execute("update LW_EmailFeedback set ClearedBy = @0, UpdateDate = @1 where Id = @2", clearedBy, DateTime.Now, id);
		}

		public void ClearEmailFeedbacks(string email, long clearedBy)
		{
			Database.Execute("update LW_EmailFeedback set ClearedBy = @0, UpdateDate = @1 where lower(EmailAddress) = @2", clearedBy, DateTime.Now, email);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
