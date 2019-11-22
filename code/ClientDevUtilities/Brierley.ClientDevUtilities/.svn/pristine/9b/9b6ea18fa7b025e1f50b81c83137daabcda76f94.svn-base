//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.JobScheduler
{
	/// <summary>
	/// 
	/// </summary>
	public interface IJob
	{
		/// <summary>
		/// Sets the run ID for this job
		/// </summary>
		/// <param name="runID">run ID</param>
		void SetRunID(long runID);

		/// <summary>
		/// Executes the job.
		/// </summary>
		ScheduleJobStatus Run(string parms);

		/// <summary>
		/// Resumes an interrupted job.
		/// </summary>
		/// <param name="parms"></param>
		/// <returns></returns>
		ScheduleJobStatus Resume(string parms);

		/// <summary>
		/// Sends an abort request to the job.
		/// </summary>
		void RequestAbort();

		/// <summary>
		/// Retrieves the text of the report for the job.
		/// </summary>
		/// <returns></returns>
		string GetReport();

        /// <summary>
        /// Perform any final activities.
        /// </summary>
        void FinalizeJob(ScheduleJobStatus jobStatus);
	}
}
