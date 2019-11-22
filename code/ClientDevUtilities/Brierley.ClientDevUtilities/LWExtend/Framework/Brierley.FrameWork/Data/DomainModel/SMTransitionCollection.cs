using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A collection of transitions for a particular state.
    /// </summary>
    public class SMTransitionCollection : List<SMTransition>
    {
		private ServiceConfig _config;
        private long _stateID = -1;
        private TransitionCollectionType _transitionCollectionType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stateID">the unique ID for the associated state</param>
        /// <param name="transitionCollectionType">the type of transitions in the collection: "Input" for those entering the state, or "Output" for those leaving</param>
        public SMTransitionCollection(ServiceConfig config, long stateID, TransitionCollectionType transitionCollectionType)
        {
			_config = config;
            _stateID = stateID;
            _transitionCollectionType = transitionCollectionType;

			using (var svc = new SurveyManager(config))
			{
				List<SMTransition> transitionList = null;
				switch ((int)_transitionCollectionType)
				{
					case (int)TransitionCollectionType.Input:
						transitionList = svc.RetrieveInputTransitions(_stateID);
						break;
					case (int)TransitionCollectionType.Output:
						transitionList = svc.RetrieveOutputTransitions(_stateID);
						break;
				}
				if (transitionList == null)
					transitionList = new List<SMTransition>();
				base.AddRange(transitionList);
			}
        }

        /// <summary>
        /// Add a transition to the collection.
        /// </summary>
        /// <param name="transition">transition to add</param>
        public new void Add(SMTransition transition)
        {
            // add to the collection
            if ((_transitionCollectionType == TransitionCollectionType.Output && transition.SrcStateID == _stateID)
                || (_transitionCollectionType == TransitionCollectionType.Input && transition.DstStateID == _stateID))
            {
                if (!base.Contains(transition))
                    base.Add(transition);
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        "Transition not for this state.  State = {0}, transition is from state {1} to {2}",
                        _stateID, transition.SrcStateID, transition.DstStateID
                    )
                );
            }

            // add to the database
			using (var svc = new SurveyManager(_config))
			{
				SMTransition tmp = svc.RetrieveTransition(transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
				if (tmp == null)
				{
					svc.CreateTransition(transition);
				}
			}
        }

        public new void Remove(SMTransition transition)
        {
            // remove from the collection
            if ((_transitionCollectionType == TransitionCollectionType.Output && transition.SrcStateID == _stateID)
                || (_transitionCollectionType == TransitionCollectionType.Input && transition.DstStateID == _stateID))
            {
                if (base.Contains(transition))
                    base.Remove(transition);
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        "Transition not for this state.  State = {0}, transition is from state {1} to {2}",
                        _stateID, transition.SrcStateID, transition.DstStateID
                    )
                );
            }

            // remove from the database
			using (var svc = new SurveyManager(_config))
			{
				SMTransition tmp = svc.RetrieveTransition(transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
				if (tmp != null)
				{
					svc.DeleteTransition(transition);
				}
			}
        }

        /// <summary>
        /// Remove all transitions from the collection.
        /// </summary>
		public new void Clear()
		{
			using (var svc = new SurveyManager(_config))
			{
				foreach (SMTransition transition in this)
				{
					SMTransition tmp = svc.RetrieveTransition(transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
					if (tmp != null)
					{
						svc.DeleteTransition(transition);
					}
				}
				base.Clear();
			}
		}
    }
}
