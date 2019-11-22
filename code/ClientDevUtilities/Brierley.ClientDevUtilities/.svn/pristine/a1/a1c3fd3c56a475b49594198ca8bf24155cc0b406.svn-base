using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestGroup
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestGroup")]
    [AuditLog(true)]
    public class RestGroup : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the Name of the Rest Group
        /// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description of the Rest Group
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }
    }
}
