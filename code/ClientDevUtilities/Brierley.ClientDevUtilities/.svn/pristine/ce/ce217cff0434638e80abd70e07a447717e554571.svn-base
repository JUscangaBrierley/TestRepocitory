using System;
using Newtonsoft.Json;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Represents an email personalization (field) in Teradata's DMC system.
	/// </summary>
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("EmailId, name", autoIncrement = false)]
    [PetaPoco.TableName("LW_EmailPersonalization")]
    public class EmailPersonalization
	{
		/// <summary>
		/// Gets or sets the Id of the LoyaltyWare email
		/// </summary>
		[JsonIgnore]
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(EmailDocument), "Id")]
        public long EmailId { get; set; }

		/// <summary>
		/// Gets or sets the name of the personalization.
		/// </summary>
		[JsonProperty(PropertyName="name")]
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the bScrpt expression used to produce the value of the personalization at send time.
		/// </summary>
		[JsonProperty(PropertyName = "expression")]
        [PetaPoco.Column]
        public string Expression { get; set; }

		public EmailPersonalization()
		{
		}

		public EmailPersonalization(long emailId, string name, string expression = null)
		{
			EmailId = emailId;
			Name = name;
			Expression = expression;
		}

		public override bool Equals(object obj)
		{
			EmailPersonalization otherInstance = obj as EmailPersonalization;
			if (otherInstance != null)
			{
				return 
					otherInstance.EmailId == EmailId && 
					otherInstance.Name == Name && 
					otherInstance.Expression == Expression;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
