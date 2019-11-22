using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Represents a directed transition from a source state to a destination state.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Src_State_ID, Src_Connector_Index, Dst_State_ID, Dst_Connector_Index", sequenceName = "hibernate_sequence", autoIncrement = false)]
    [PetaPoco.TableName("LW_SM_Transition")]
    public class SMTransition : LWCoreObjectBase
    {
        #region fields
        private long _srcStateID = -1;
        private long _srcConnectorIndex = 0;
        private long _dstStateID = -1;
        private long _dstConnectorIndex = 0;
        private long _page = 0;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
		public SMTransition()
		{
        }

        #region properties
        /// <summary>
        /// The unique ID of the source state for the transition.
        /// </summary>
        [PetaPoco.Column("Src_State_ID", IsNullable = false)]
        [ForeignKey(typeof(SMState), "ID")]
        public long SrcStateID
		{
            get { return _srcStateID; }
            set { _srcStateID = value; }
		}

        /// <summary>
        /// The index of the output connector on the source state to which the transition is originating.
        /// </summary>
        [PetaPoco.Column("Src_Connector_Index", IsNullable = false)]
        public long SrcConnectorIndex
		{
            get { return _srcConnectorIndex; }
            set { _srcConnectorIndex = value; }
		}

        /// <summary>
        /// The unique ID of the destination state for the transition.
        /// </summary>
        [PetaPoco.Column("Dst_State_ID", IsNullable = false)]
        [ForeignKey(typeof(SMState), "ID")]
        public long DstStateID
        {
            get { return _dstStateID; }
            set { _dstStateID = value; }
        }

        /// <summary>
        /// The index of the input connector on the destination state to which the transition is terminating.
        /// </summary>
        [PetaPoco.Column("Dst_Connector_Index", IsNullable = false)]
        public long DstConnectorIndex
		{
            get { return _dstConnectorIndex; }
            set { _dstConnectorIndex = value; }
        }

        /// <summary>
        /// The page for this transition.
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Page
        {
            get { return _page; }
            set { _page = value; }
        }
        #endregion

        /// <summary>
        /// Equality operator for two transitions
        /// </summary>
        /// <param name="obj">transition used for equality test</param>
        /// <returns>true if the transitions are equal, or false if they are not equal, or the argument is not a transition</returns>
        public override bool Equals(object obj)
        {
            SMTransition otherInstance = obj as SMTransition;
         
            if (otherInstance == null) return false;

            if (otherInstance.SrcStateID != _srcStateID 
                || otherInstance.SrcConnectorIndex != _srcConnectorIndex
                || otherInstance.DstStateID != _dstStateID 
                || otherInstance.DstConnectorIndex != _dstConnectorIndex
                || otherInstance.Page != _page
             )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the hash code value for this instance.
        /// </summary>
        /// <returns>hash code</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
