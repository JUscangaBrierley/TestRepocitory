using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AL_ExchangeRate")]
    public class ExchangeRate_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }

        /// <summary>
        /// Display name of the from currency
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string FromCurrencyName { get; set; }

        /// <summary>
        /// ISO 4217 Code of the from currency 
        /// </summary>
        [PetaPoco.Column(Length = 3, IsNullable = false)]
        public string FromCurrency { get; set; }

        /// <summary>
        /// Display name of the the to currency
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = true)]
        public string ToCurrencyName { get; set; }

        /// <summary>
        /// ISO 4217 code fo the to currency
        /// </summary>
        [PetaPoco.Column(Length = 3, IsNullable = false)]
        public string ToCurrency { get; set; }

        /// <summary>
        /// Convertion rate between the two currencies
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal Rate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
    }
}
