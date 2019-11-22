using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A message state within a survey's state diagram.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Message_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Message")]
    public class SMMessage : LWCoreObjectBase
    {
        /// <summary>
        /// The unique identifier for this message.
        /// </summary>
        [PetaPoco.Column("Message_ID", IsNullable = false)]
        public long ID { get; set; }

        /// <summary>
        /// The unique identifier for the associated state in the state diagram.
        /// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        [ForeignKey(typeof(SMState), "ID")]
        public long StateID { get; set; }

        /// <summary>
        /// The content to be displayed for the message at run time.
        /// </summary>
        [PetaPoco.Column]
        public string Content { get; set; }

		public SMMessage()
		{
			ID = -1;
			StateID = -1;
			Content = string.Empty;
		}
    }
}
