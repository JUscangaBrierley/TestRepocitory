using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Indicates a language used for taking surveys.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Language_Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Language")]
    public class SMLanguage
    {
        private long _ID;
        private string _description;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SMLanguage()
        {
        }

        /// <summary>
        /// A unique identifier.
        /// </summary>
        [PetaPoco.Column("Language_Id", IsNullable = false)]
        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// A description.
        /// </summary>
        [PetaPoco.Column("Language_Description", Length = 255, IsNullable = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
