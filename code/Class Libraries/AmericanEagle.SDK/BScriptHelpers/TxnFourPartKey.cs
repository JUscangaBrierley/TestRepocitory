using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanEagle.SDK.BScriptHelpers
{
    public struct TxnFourPartKey
    {
        public string TxnNumber;
        public string OrderNumber;
        public DateTime? TxnDate;
        public long? TxnStoreId;
    }
}
