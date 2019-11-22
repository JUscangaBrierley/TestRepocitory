using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    public class SMTransitionDao : DaoBase<SMTransition>
    {
        public SMTransitionDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }

        public void SetForceQueryCacheReset(bool value)
        {
            //ForceQueryCacheReset = value;
        }

        /// <summary>
        /// Update a transition.
        /// </summary>
        /// <param name="transition">the transition to update</param>
        /// <returns>the updated transition</returns>
        public void Update(SMTransition transition)
        {
            transition.UpdateDate = DateTime.Now;
            Database.Execute(@"update LW_SM_Transition set Page = @4, CreateDate = @5, UpdateDate = @6 
                               where Src_State_ID = @0 and Src_Connector_Index = @1 and Dst_State_ID = @2 and Dst_Connector_Index = @3",
                transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex,
                transition.Page, transition.CreateDate, transition.UpdateDate);
        }

        /// <summary>
        /// Get a specific transition.
        /// </summary>
        /// <param name="srcStateID">the ID of the source state</param>
        /// <param name="srcConnectorIndex">the index of the output connector on the source state</param>
        /// <param name="dstStateID">the ID of the destination state</param>
        /// <param name="dstConnectorIndex">the index of the input connector on the source state</param>
        /// <returns>the specified transition, or null if it doesn't exist</returns>
        public SMTransition Retrieve(long srcStateID, long srcConnectorIndex, long dstStateID, long dstConnectorIndex)
        {
            return Database.FirstOrDefault<SMTransition>("select * from LW_SM_Transition where Src_State_ID = @0 and Src_Connector_Index = @1 and Dst_State_ID = @2 and Dst_Connector_Index = @3",
                srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
        }

        /// <summary>
        /// Get all transitions
        /// </summary>
        /// <returns>list of all transitions</returns>
        public List<SMTransition> RetrieveAll()
        {
            return Database.Fetch<SMTransition>("select * from LW_SM_Transition");
        }

        /// <summary>
        /// Get the list of transitions that terminate at the given state.
        /// </summary>
        /// <param name="stateID">a specific state</param>
        /// <returns>list of input transitions for the state</returns>
        public List<SMTransition> RetrieveInputs(long stateID)
        {
            return Database.Fetch<SMTransition>("select * from LW_SM_Transition where Dst_State_ID = @0", stateID);
        }

        /// <summary>
        /// Get the list of transitions that originate at the given state.
        /// </summary>
        /// <param name="stateID">a specific state</param>
        /// <returns>list of output transitions for the state</returns>
        public List<SMTransition> RetrieveOutputs(long stateID)
        {
            return Database.Fetch<SMTransition>("select * from LW_SM_Transition where Src_State_ID = @0", stateID);
        }

        /// <summary>
        /// Delete a transition.
        /// </summary>
        /// <param name="srcStateID">the ID of the source state</param>
        /// <param name="srcConnectorIndex">the index of the output connector on the source state</param>
        /// <param name="dstStateID">the ID of the destination state</param>
        /// <param name="dstConnectorIndex">the index of the input connector on the source state</param>
        public void Delete(long srcStateID, long srcConnectorIndex, long dstStateID, long dstConnectorIndex)
        {
            Database.Execute("delete from LW_SM_Transition where Src_State_ID = @0 and Src_Connector_Index = @1 and Dst_State_ID = @2 and Dst_Connector_Index = @3",
                srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
        }

        /// <summary>
        /// Delete all transitions for a particular survey
        /// </summary>
        /// <param name="surveyID">ID of the survey</param>
        /// <returns>number of rows deleted</returns>
        public int DeleteAllForSurvey(long surveyID)
        {
            if (surveyID < 0)
                throw new ArgumentNullException("surveyID");

            string queryStr = @"
               delete from LW_SM_Transition where Src_State_ID in (
                  select distinct State_ID from LW_SM_State 
                  where Survey_ID=@0)";

            int rows = Database.Execute(queryStr, surveyID);

            queryStr = @"
               delete from LW_SM_Transition where Dst_State_ID in (
                  select distinct State_ID from LW_SM_State 
                  where Survey_ID=@0)";

            rows += Database.Execute(queryStr, surveyID);

            return rows;
        }
    }
}
