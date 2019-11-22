using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Event")]
    public class LWEvent : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string DisplayText { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool UserDefined { get; set; }

		public LWEvent Clone()
        {
            return Clone(new LWEvent());
        }

        public LWEvent Clone(LWEvent dest)
        {
            dest.Name = Name;
            dest.DisplayText = DisplayText;
            dest.Description = Description;
            dest.UserDefined = UserDefined;
            return (LWEvent)base.Clone(dest);
        }
    }
}
