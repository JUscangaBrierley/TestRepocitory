using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Indicates a cultureMap used for taking surveys.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("CultureMap_Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_CultureMap")]
    public class SMCultureMap
    {
        #region fields
        private long _ID;
        private long _languageID;
        private string _culture;
        private string _description;
        #endregion

        #region constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public SMCultureMap()
        {
        }
        #endregion

        #region attributes
        /// <summary>
        /// The unique identifier.
        /// </summary>
        [PetaPoco.Column("CultureMap_Id", IsNullable = false)]
        public long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// The unique identifier for the associated language.
        /// </summary>
        [PetaPoco.Column("Language_Id", IsNullable = false)]
        [ForeignKey(typeof(SMLanguage), "ID")]
        public long LanguageID
        {
            get { return _languageID; }
            set { _languageID = value; }
        }

        /// <summary>
        /// Internet standard culture.  For example, 'en-US' indicates English language ('en') and 
        /// United States culture ('US').
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        /// <summary>
        /// A description.
        /// </summary>
        [PetaPoco.Column("CultureMap_Description", Length = 255)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        #endregion
    }
}
