using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A collection of states for a given survey.
    /// </summary>
	public class SMStateCollection : List<SMState>
	{
        private ServiceConfig _config;
        private long _surveyID;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="surveyID">the ID of the associated survey</param>
		public SMStateCollection(ServiceConfig config, long surveyID)
        {
			_config = config;
            _surveyID = surveyID;
			using (var svc = new SurveyManager(config))
			{
				List<SMState> states = svc.RetrieveStatesBySurveyID(_surveyID);
				if (states != null && states.Count > 0)
				{
					base.AddRange(states);
				}
			}
        }

		public SMStateCollection(ServiceConfig config, long surveyID, IList<SMState> states)
		{
			_config = config;
			_surveyID = surveyID;
			if (states != null && states.Count > 0)
			{
				base.AddRange(states);
			}
		}

        /// <summary>
        /// Add a state to the collection
        /// </summary>
        /// <param name="state">state to add</param>
        /// <returns>the unique ID assigned to the state</returns>
        public new long Add(SMState state)
        {
            // add to the database (ID is assigned here)
			using (var svc = new SurveyManager(_config))
			{
				svc.CreateState(state);
			}
            // add to the collection
            SMState existingState = this[state.ID];
            if (existingState != null)
            {
                base.Remove(existingState);
            }
            base.Add(state);

            return state.ID;
        }

        /// <summary>
        /// Update the specified state.
        /// </summary>
        /// <param name="state">changed state</param>
        public void Update(SMState state)
		{
            // update the collection
            SMState existingState = this[state.ID];
            if (existingState != null)
            {
                base.Remove(existingState);
            }
            base.Add(state);

            // update in the database  
			using (var svc = new SurveyManager(_config))
			{
				svc.UpdateState(state);
			}
		}

        /// <summary>
        /// Remove the specified state.
        /// </summary>
        /// <param name="state">state to remove</param>
        public new void Remove(SMState state)
		{
            Remove(state.ID);
		}

        /// <summary>
        /// Remove the specified state.
        /// </summary>
        /// <param name="stateID">unique ID of the state to remove</param>
        public void Remove(long stateID)
        {
            // remove from database
			using (var svc = new SurveyManager(_config))
			{
				svc.DeleteState(stateID);
			}

            // remove from the collection
            SMState existingState = this[stateID];
            if (existingState != null)
            {
                base.Remove(existingState);
            }
        }

        /// <summary>
        /// Get a state from the collection based on it's unique ID
        /// </summary>
        /// <param name="stateID">unique ID of the state</param>
        /// <returns>the specified state, or null if it does not exist in the collection</returns>
		public SMState this[long stateID]
        {
            get
            {
                foreach (SMState state in this)
                {
                    if (state.ID == stateID) return state;
                }
				return null;
            }
        }
	}
}
