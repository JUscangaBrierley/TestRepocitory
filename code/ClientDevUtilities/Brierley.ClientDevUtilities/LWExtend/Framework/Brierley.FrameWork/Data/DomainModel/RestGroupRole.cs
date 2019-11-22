using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestGroupRole
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestGroupRole")]
    [UniqueIndex(ColumnName = "RestGroupId,RestRoleId", RequiresLowerFunction = false)]
    [AuditLog(true)]
    public class RestGroupRole : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the RestGroupId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestGroup), "Id")]
        public long RestGroupId { get; set; }

        /// <summary>
        /// Gets or sets the RestRoleId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestRole), "Id")]
        public long RestRoleId { get; set; }
    }
}
