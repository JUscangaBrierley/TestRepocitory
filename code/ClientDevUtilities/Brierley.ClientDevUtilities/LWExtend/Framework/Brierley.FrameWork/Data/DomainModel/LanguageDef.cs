using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Indicates a LanguageMap used to define different langauges.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Culture", autoIncrement = false)]
	[PetaPoco.TableName("LW_LanguageDef")]
	public class LanguageDef
	{
		/// <summary>
		/// Internet standard culture. For example, 'en-US' indicates English language ('en') and 
		/// United States culture ('US').
		/// </summary>
        [PetaPoco.Column(Length = 8, IsNullable = false)]
		public string Culture { get; set; }

		/// <summary>
		/// The name of the language for display purpose.
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Language { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public LanguageDef()
		{
		}
	}
}
