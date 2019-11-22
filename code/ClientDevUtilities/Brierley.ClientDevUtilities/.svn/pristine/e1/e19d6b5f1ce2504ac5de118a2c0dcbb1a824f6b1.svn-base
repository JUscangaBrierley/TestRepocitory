using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestConsumerRole
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestConsumerRole")]
    [UniqueIndex(ColumnName = "RestConsumerId,RestRoleId", RequiresLowerFunction = false)]
    [AuditLog(true)]
    public class RestConsumerRole : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the RestConsumerId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestConsumer), "Id")]
        public long RestConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the RestRoleId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestRole), "Id")]
        public long RestRoleId { get; set; }
    }
}
