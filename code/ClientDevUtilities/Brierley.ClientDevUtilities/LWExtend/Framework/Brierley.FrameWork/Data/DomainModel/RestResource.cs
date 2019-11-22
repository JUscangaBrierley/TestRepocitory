using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestResource.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestResource")]
    [AuditLog(true)]
    public class RestResource : RestObjectBase
    {
        /// <summary>
        /// Gets or Sets the RestResourceType
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex(RequiresLowerFunction = false)]
        public RestResourceType RestResourceType { get; set; }

        /// <summary>
        /// Gets or Sets the Details of the RestResource
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Details { get; set; }

        /// <summary>
        /// Result column for Permission.  If this is set by user, it will not be written.
        /// </summary>
        [PetaPoco.ResultColumn]
        public RestPermissionType RestPermissionType { get; set; }
    }
}
