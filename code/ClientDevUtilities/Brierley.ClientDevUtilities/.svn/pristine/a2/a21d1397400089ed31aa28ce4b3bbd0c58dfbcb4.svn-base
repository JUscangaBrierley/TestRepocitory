using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// Holds the convertion rate between two currencies
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_ExchangeRate")]
    public class ExchangeRate : LWCoreObjectBase
    {

        /// <summary>
        /// Primary key
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// ISO 4217 Code of the from currency 
        /// </summary>
        [PetaPoco.Column(Length = 3, IsNullable = false)]
        [ColumnIndex]
        public string FromCurrency { get; set; }

        /// <summary>
        /// Display name of the from currency
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string FromCurrencyName { get; set; }

        /// <summary>
        /// ISO 4217 code fo the to currency
        /// </summary>
        [PetaPoco.Column(Length = 3, IsNullable = false)]
        [ColumnIndex]
        public string ToCurrency { get; set; }

        /// <summary>
        /// Display name of the the to currency
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string ToCurrencyName { get; set; }

        /// <summary>
        /// Convertion rate between the two currencies
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal Rate { get; set; }

        /// <summary>
        /// Create the Audit log object
        /// </summary>
        /// <returns></returns>
        public override LWObjectAuditLogBase GetArchiveObject()
        {
            ExchangeRate_AL ar = new ExchangeRate_AL()
            {

                ObjectId = this.Id,
                FromCurrencyName = this.FromCurrencyName,
                FromCurrency = this.FromCurrency,
                ToCurrencyName = this.ToCurrencyName,
                ToCurrency = this.ToCurrency,
                Rate = this.Rate,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
    }
}
