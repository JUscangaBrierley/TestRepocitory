using System;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestApiLog
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "log_sequence")]
    [PetaPoco.TableName("LW_RestApiLog")]
    public class RestApiLog
    {
        /// <summary>
        /// Gets or sets the Id 
        /// </summary>
        /// <remarks>Required identity column for SQL Server support</remarks>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Unique RequestId
        /// </summary>
        [PetaPoco.Column(Length = 36, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerId
        /// </summary>
        [PetaPoco.Column(Length = 36, IsNullable = true)]
        public string ConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the RequestHost
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string RequestHost { get; set; }

        /// <summary>
        /// Gets or sets the RequestMethod
        /// </summary>
        [PetaPoco.Column(Length = 10, IsNullable = false)]
        public string RequestMethod { get; set; }

        /// <summary>
        /// Gets or sets the RequestEndpoint
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string RequestEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the RequestPath
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string RequestPath { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public int HttpStatusCode { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public int? ResponseCode { get; set; }

        [PetaPoco.Column(IsNullable = true)]
        public long? ThreadId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTimeOffset StartTime { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTimeOffset EndTime { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public int ElapsedTime { get; set; }
    }
}
