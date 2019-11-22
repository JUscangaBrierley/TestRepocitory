using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	public class LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the CreateDate for the current object
		/// </summary>
		//[System.Xml.Serialization.XmlIgnore]
        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// Gets or sets the UpdateDate for the current object
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }

		public virtual LWObjectAuditLogBase GetArchiveObject()
		{
			return null;
		}

		public virtual LWCoreObjectBase Clone(LWCoreObjectBase dest)
		{
			dest.CreateDate = CreateDate;
			dest.UpdateDate = UpdateDate;
			return dest;
		}
	}
}