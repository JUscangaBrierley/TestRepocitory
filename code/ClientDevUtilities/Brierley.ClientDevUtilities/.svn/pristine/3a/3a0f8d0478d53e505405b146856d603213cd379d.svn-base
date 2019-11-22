using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for IDGenerator.
	/// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ObjectName", autoIncrement = false, sequenceName = "SEQ_IDGENERATOR")]
    [PetaPoco.TableName("LW_IDGenerator")]
    public class IDGenerator
    {
        /// <summary>
        /// Gets or sets the ObjectName for the current IDGenerator
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the SeedValue for the current IDGenerator
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SeedValue { get; set; }

        /// <summary>
        /// Gets or sets the IncrValue for the current IDGenerator
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long IncrValue { get; set; }

        /// <summary>
        /// Gets or sets the PrevValue for the current IDGenerator
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long PrevValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the IDGenerator class
        /// </summary>
        public IDGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the IDGenerator class
        /// </summary>
        /// <param name="objectName">Initial <see cref="IDGenerator.ObjectName" /> value</param>
        /// <param name="seedValue">Initial <see cref="IDGenerator.SeedValue" /> value</param>
        /// <param name="incrValue">Initial <see cref="IDGenerator.IncrValue" /> value</param>
        /// <param name="prevValue">Initial <see cref="IDGenerator.PrevValue" /> value</param>
        public IDGenerator(string objectName, Int64 seedValue, Int64 incrValue, Int64 prevValue)
        {
            ObjectName = objectName;
            SeedValue = seedValue;
            IncrValue = incrValue;
            PrevValue = prevValue;
        }
    }
}