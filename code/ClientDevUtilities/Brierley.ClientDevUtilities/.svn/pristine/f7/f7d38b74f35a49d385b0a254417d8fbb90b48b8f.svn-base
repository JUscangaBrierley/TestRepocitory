using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestConsumerGroup
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestConsumerGroup")]
    [UniqueIndex(ColumnName = "RestConsumerId,RestGroupId", RequiresLowerFunction = false)]
    [AuditLog(true)]
    public class RestConsumerGroup : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the RestConsumerId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestConsumer), "Id")]
        public long RestConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the RestGroupId
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RestGroup), "Id")]
        public long RestGroupId { get; set; }
    }
}
