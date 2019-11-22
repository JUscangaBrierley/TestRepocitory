using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class QueryFactory
	{
		public static Query GetQuery(StepType stepType)
		{
			Query query = null;
			switch (stepType)
			{
				case StepType.Assignment:
					query = new AssignmentQuery();
					break;
				case StepType.bScript:
					query = new bScriptQuery();
					break;
				case StepType.ChangeAudience:
					query = new ChangeAudienceQuery();
					break;
				case StepType.ControlGroup:
					query = new ControlGroupQuery();
					break;
				case StepType.DeDupe:
					query = new DedupeQuery();
					break;
				case StepType.Merge:
					query = new MergeQuery();
					break;
				case StepType.Output:
					query = new OutputQuery();
					break;
				case StepType.Select:
					query = new SelectQuery();
					break;
				case StepType.SplitProcess:
					query = new SplitProcessQuery();
					break;
				case StepType.Sql:
					query = new SqlTextQuery();
					break;
				case StepType.RealTimeSelect:
					query = new RealTimeSelectQuery();
					break;
				case StepType.RealTimebScript:
					query = new RealTimebScriptQuery();
					break;
				case StepType.RealTimeSurvey:
					query = new SurveyQuery();
					break;
				case StepType.RealTimeInputProcessing:
					query = new RealTimeInputQuery();
					break;
				case StepType.RealTimeAssignment:
					query = new RealTimeAssignmentQuery();
					break;
				case StepType.RealTimeOutput:
					query = new RealTimeOutputQuery();
					break;
				case StepType.Campaign:
					query = new CampaignQuery();
					break;
				case StepType.Wait:
					query = new WaitQuery();
					break;
				case StepType.Pivot:
					query = new PivotQuery();
					break;
			}
			return query;
		}
	}
}