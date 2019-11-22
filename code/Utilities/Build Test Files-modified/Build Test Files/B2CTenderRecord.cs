using System;

namespace BuildTestFiles
{
	/// <summary>
	/// Summary description for TenderTypeDetail.
	/// </summary>
	public struct B2CTenderRecord
    {
		public int StoreNumber;
		public string TransactionNumber;
        public DateTime TransactionDate;
        public string RegisterNumber;
        public int LineNumber;
        public string TenderType;
        public decimal TenderAmount;
		public string OrderNumber;

		public B2CTenderRecord(string TenderType, int StoreNumber, string TransactionNumber,DateTime TransactionDate,string RegisterNumber,int LineNumber,decimal TenderAmount,string OrderNumber)
		{
			this.TenderType = TenderType;
            this.StoreNumber = StoreNumber;
            this.TransactionNumber = TransactionNumber;
            this.TransactionDate = TransactionDate;
            this.RegisterNumber = RegisterNumber;
            this.LineNumber = LineNumber;            
            this.TenderAmount = TenderAmount;
            this.OrderNumber = OrderNumber;
    }
	}
}
