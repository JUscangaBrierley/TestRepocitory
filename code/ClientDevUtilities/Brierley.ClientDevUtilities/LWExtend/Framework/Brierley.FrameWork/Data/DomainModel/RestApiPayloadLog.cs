using System;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for RestApiPayloadLog
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "log_sequence")]
    [PetaPoco.TableName("LW_RestApiPayloadLog")]
    public class RestApiPayloadLog
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
        /// Gets or sets the RequestContentType
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string RequestContentType { get; set; }

        /// <summary>
        /// Gets or sets the RequestProtocol
        /// </summary>
        [PetaPoco.Column(Length = 25, IsNullable = true)]
        public string RequestProtocol { get; set; }

        /// <summary>
        /// Gets or sets the RequestScheme
        /// </summary>
        [PetaPoco.Column(Length = 25, IsNullable = true)]
        public string RequestScheme { get; set; }

        /// <summary>
        /// Gets or sets the RequestContentLength
        /// </summary>
        [PetaPoco.Column(IsNullable = true)]
        public long? RequestContentLength { get; set; }

        /// <summary>
        /// Gets or sets the RequestQueryString
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = true)]
        public string RequestQueryString { get; set; }

        /// <summary>
        /// Gets or sets the RequestHeaders
        /// </summary>
        [PetaPoco.Column(IsNullable = true)]
        public string RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the RequestBody
        /// </summary>
        [PetaPoco.Column(IsNullable = true)]
        public string RequestBody { get; set; }

        /// <summary>
        /// Gets or sets the ResponseContentType
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string ResponseContentType { get; set; }

        /// <summary>
        /// Gets or sets the ResponseHeaders
        /// </summary>
        [PetaPoco.Column(IsNullable = true)]
        public string ResponseHeaders { get; set; }

        /// <summary>
        /// Gets or sets the ResponseBody
        /// </summary>
        [PetaPoco.Column(IsNullable = true)]
        public string ResponseBody { get; set; }
    }
}
