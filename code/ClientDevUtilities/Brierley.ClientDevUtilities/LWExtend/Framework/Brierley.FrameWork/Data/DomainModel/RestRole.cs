using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestRole.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestRole")]
    [AuditLog(true)]
    public class RestRole : RestObjectBase
    {
        /// <summary>
        /// Gets or Sets the Name of the RestRole
        /// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the Description of the RestRole
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }
    }
}
