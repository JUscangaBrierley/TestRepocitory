using Brierley.FrameWork.Data.ModelAttributes;
using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestRoleResource
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestRoleResource")]
    [UniqueIndex(ColumnName = "RestRoleId,RestResourceId,RestPermissionType", RequiresLowerFunction = false)]
    [AuditLog(true)]
    public class RestRoleResource : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the RestRoleId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestRole), "Id")]
        public long RestRoleId { get; set; }

        /// <summary>
        /// Gets or sets the RestResourceId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestResource), "Id")]
        public long RestResourceId { get; set; }

        /// <summary>
        /// Gets or sets the Permission
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public RestPermissionType RestPermissionType { get; set; }
    }
}
