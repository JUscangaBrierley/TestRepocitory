using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class SMSurveyReportResponseRow
	{
		public long QuestionContentID { get; set; }
		public long AnswerContentID { get; set; }
		public long MatrixAnswerID { get; set; }
		public long ColumnIndex { get; set; }
		public string Content { get; set; }
		public string PipedContent { get; set; }
		public long ResponseCount { get; set; }
	}

	#region SMSurveyReportData class
	public class SMSurveyReportData
	{
		public long NumCompletes = 0;
		public long NumSkips = 0;
		public long NumStarts = 0;
		public Dictionary<long, List<SMSurveyReportResponseRow>> Responses = new Dictionary<long, List<SMSurveyReportResponseRow>>();
		public Dictionary<long, long> ConceptViewSummary = new Dictionary<long, long>();

		internal void AddFromRawRows(List<dynamic> rawRows)
		{
			foreach (dynamic rawRow in rawRows)
			{
				// Marshal the query results
                long questionContentID = StringUtils.FriendlyInt64(rawRow.QUESTIONCONTENT_ID, -1);
                long answerContentID = StringUtils.FriendlyInt64(rawRow.ANSWERCONTENT_ID, -1);
                long matrixAnswerID = StringUtils.FriendlyInt64(rawRow.MATRIXANSWER_ID, -1);
                long columnIndex = StringUtils.FriendlyInt64(rawRow.COLUMN_INDEX, -1);
                string content = StringUtils.FriendlyString(rawRow.CONTENT);
                string pipedContent = StringUtils.FriendlyString(rawRow.PIPEDCONTENT);
                long responseCount = StringUtils.FriendlyInt64(rawRow.COUNT, -1);

				// new question content ID?
				if (!Responses.ContainsKey(questionContentID))
					Responses.Add(questionContentID, new List<SMSurveyReportResponseRow>());

				Responses[questionContentID].Add(new SMSurveyReportResponseRow()
				{
					QuestionContentID = questionContentID,
					AnswerContentID = answerContentID,
					MatrixAnswerID = matrixAnswerID,
					ColumnIndex = columnIndex,
					Content = content,
					PipedContent = pipedContent,
					ResponseCount = responseCount
				});
			}
		}
	}
	#endregion
}
