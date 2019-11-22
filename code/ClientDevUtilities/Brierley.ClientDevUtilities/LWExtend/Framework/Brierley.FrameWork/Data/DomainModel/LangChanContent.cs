using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_LangChanContent")]
    [AuditLog(true)]
    [UniqueIndex(ColumnName = "LangChanType,RefId,LanguageCulture,Channel,Name")]
    public class LangChanContent : LWCoreObjectBase
	{
        [PetaPoco.Column("Id", IsNullable = false)]
		public long PKey { get; set; }

        [PetaPoco.Column(Length = 25, IsNullable = false, PersistEnumAsString = true)]
        public ContentObjType LangChanType { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long RefId { get; set; }

		[PetaPoco.Column(Length = 25, IsNullable = false)]
		public string LanguageCulture { get; set; }

		[PetaPoco.Column(Length = 25, IsNullable = false)]
		public string Channel { get; set; }

		[PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }

		[PetaPoco.Column]
		public string Content { get; set; }

		public override bool Equals(object obj)
		{
			var otherInstance = obj as LangChanContent;
			if (otherInstance != null)
			{
				return (otherInstance.LangChanType == LangChanType &&
					otherInstance.RefId == RefId &&
					otherInstance.LanguageCulture == LanguageCulture &&
					otherInstance.Channel == Channel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public LangChanContent Clone()
		{
			return Clone(new LangChanContent());
		}

		public LangChanContent Clone(LangChanContent dest)
		{
			dest.LangChanType = LangChanType;
			dest.RefId = RefId;
			dest.LanguageCulture = LanguageCulture;
			dest.Channel = Channel;
			dest.Name = Name;
			dest.Content = Content;
			base.Clone(dest);
			return dest;
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			LangChanContent_AL ar = new LangChanContent_AL()
			{
				ObjectId = this.PKey,
				LangChanType = this.LangChanType,
				RefId = this.RefId,
				LanguageCulture = this.LanguageCulture,
				Channel = this.Channel,
				Name = this.Name,
				Content = this.Content,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
