using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PromoMappingFile")]
	public class PromoMappingFile : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        [PetaPoco.Column(Length = 500)]
		public string Description { get; set; }

        [PetaPoco.Column]
		public string Content { get; set; }

        public PromoMappingFile Clone()
		{
			return Clone(new PromoMappingFile());
		}

		public PromoMappingFile Clone(PromoMappingFile dest)
		{
			dest.Name = Name;
			dest.Description = Description;
			dest.Content = Content;
			return (PromoMappingFile)base.Clone(dest);
		}
	}
}
