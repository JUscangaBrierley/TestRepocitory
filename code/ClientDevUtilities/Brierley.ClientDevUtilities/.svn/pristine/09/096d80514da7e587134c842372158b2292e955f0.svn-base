using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PromoTemplate")]
	public class PromoTemplate : LWCoreObjectBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string TemplateType { get; set; }

        [PetaPoco.Column(Length = 500)]
		public string Description { get; set; }

        [PetaPoco.Column]
		public string Content { get; set; }

		public virtual PromoTemplate Clone()
		{
			return Clone(new PromoTemplate());
		}

		public virtual PromoTemplate Clone(PromoTemplate dest)
		{
			dest.Name = Name;
			dest.TemplateType = TemplateType;
			dest.Description = Description;
			dest.Content = Content;
			return (PromoTemplate)base.Clone(dest);
		}
	}
}
