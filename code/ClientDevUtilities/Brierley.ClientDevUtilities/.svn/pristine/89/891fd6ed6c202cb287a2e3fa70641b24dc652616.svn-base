using System;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Represents an email personalization (field) in Teradata's DMC system.
	/// </summary>
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("SmsId, name", autoIncrement = false)]
    [PetaPoco.TableName("LW_SmsPersonalization")]
    public class SmsPersonalization
	{
		/// <summary>
		/// Gets or sets the Id of the LoyaltyWare email
		/// </summary>
		[JsonIgnore]
        [PetaPoco.Column(IsNullable = false)]
        public long SmsId { get; set; }

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

		public SmsPersonalization()
		{
		}

        public SmsPersonalization(long smsId, string name, string expression = null)
		{
			SmsId = smsId;
			Name = name;
			Expression = expression;
		}

		public override bool Equals(object obj)
		{
			EmailPersonalization otherInstance = obj as EmailPersonalization;
			if (otherInstance != null)
			{
				return 
					otherInstance.EmailId == SmsId && 
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
