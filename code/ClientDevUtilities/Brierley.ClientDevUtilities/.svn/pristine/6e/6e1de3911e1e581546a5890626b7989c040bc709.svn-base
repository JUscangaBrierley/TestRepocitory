using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Indicates a ChannelDef used to define different channels.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_ChannelDef")]
	public class ChannelDef
	{
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(Length = 255)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string MimeType { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ChannelDef()
		{
		}

		public ChannelDef Clone()
		{
			return Clone(new ChannelDef());
		}

		public ChannelDef Clone(ChannelDef dest)
		{
			dest.Name = Name;
			dest.Description = Description;
			dest.MimeType = MimeType;
			return dest;
		}
	}
}
