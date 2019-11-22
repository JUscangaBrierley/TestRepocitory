//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_CSFunction")]
	public class CSFunction : LWCoreObjectBase
	{
		public const string LW_CSFUNCTION_USERADMIN = "UserAdministration";
		public const string LW_CSFUNCTION_CHANGEACCOUNTSTATUS = "ChangeAccountStatus";
		public const string LW_CSFUNCTION_UPDATEACCOUNT = "UpdateAccount";
		public const string LW_CSFUNCTION_ALLOWPOINTAWARD = "AllowPointAward";
		public const string LW_CSFUNCTION_CREATECSNOTES = "CreateCSNotes";

		/// <summary>
		/// Gets or sets the ID for the current CSFunction
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current CSFunction
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current CSFunction
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Description { get; set; }


		/// <summary>
		/// Initializes a new instance of the CSFunction class
		/// </summary>
		public CSFunction()
		{
		}

	}
}
