//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_CSRoleFunction")]
	public class CSRoleFunction : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current CSRoleFunction
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the RoleId for the current CSRoleFunction
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(CSRole), "Id")]
        public long RoleId { get; set; }

		/// <summary>
		/// Gets or sets the FunctionId for the current CSRoleFunction
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(CSFunction), "Id")]
        public long FunctionId { get; set; }


		/// <summary>
		/// Initializes a new instance of the CSRoleFunction class
		/// </summary>
		public CSRoleFunction()
		{
		}
	}
}
