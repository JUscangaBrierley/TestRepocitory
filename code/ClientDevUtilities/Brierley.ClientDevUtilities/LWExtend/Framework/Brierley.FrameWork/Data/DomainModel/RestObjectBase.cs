using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Object base for Rest object types
    /// </summary>
    [Serializable]
    public class RestObjectBase : LWCoreObjectBase
    {
        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Soft Delete Y/N flag for the current object
        /// </summary>
        [PetaPoco.Column(Length = 1, IsNullable = false)]
        public string IsDeleted { get; set; }

        public RestObjectBase()
        {
            IsDeleted = "N";
        }
    }
}
