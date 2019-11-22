using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ObjectName", autoIncrement = false)]
	[PetaPoco.TableName("LW_ATSLock")]
	public class ATSLock
	{
		/// <summary>
		/// Gets or sets the ObjectName for the current ATSLock
		/// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string ObjectName { get; set; }

		/// <summary>
		/// Gets or sets the times locked for the current ATSLock.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public Int64 TimesLocked { get; set; }
	}
}