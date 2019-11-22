//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_CSRole")]
    public class CSRole : LWCoreObjectBase
	{
		/// <summary>
		/// Initializes a new instance of the CSRole class
		/// </summary>
		public CSRole()
		{
			Functions = new List<CSFunction>();
		}

		/// <summary>
		/// Gets or sets the ID for the current CSRole
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current CSRole
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current CSRole
		/// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }

		public List<CSFunction> Functions { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of points the role is allowed to award to a member.
		/// </summary>
        [PetaPoco.Column]
        public int? PointAwardLimit { get; set; }


		public bool HasFunction(string funcName)
		{
			bool hasIt = false;
			if (Functions != null && Functions.Count > 0)
			{
				foreach (CSFunction func in Functions)
				{
					if (func.Name == funcName)
					{
						hasIt = true;
						break;
					}
				}
			}
			return hasIt;
		}
	}
}
