using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestConsumer
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RestConsumer")]
    [AuditLog(true)]
    public class RestConsumer : RestObjectBase
    {
        /// <summary>
        /// Gets or sets the Consumer Id
        /// </summary>
        /// <remarks>For Kong Integration</remarks>
        [PetaPoco.Column(Length = 255)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string ConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the Custom Id
        /// </summary>
        /// <remarks>For Kong Integration</remarks>
        [PetaPoco.Column(Length = 255)]
        [ColumnIndex(RequiresLowerFunction = false)]
        public string CustomId { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        /// <remarks>For Kong Integration</remarks>
        [PetaPoco.Column(Length = 50)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Username { get; set; }
    }
}
