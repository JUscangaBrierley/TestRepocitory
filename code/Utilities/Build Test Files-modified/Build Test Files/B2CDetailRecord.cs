using System;

namespace BuildTestFiles
{
	/// <summary>
	/// Summary description for TenderTypeDetail.
	/// </summary>
	public struct B2CDetailRecord
    {
		public int StoreNumber;
		public string TransactionNumber;
        public DateTime TransactionDate;
        public string RegisterNumber;
        public int LineNumber;
        public string SKUNumber;
        public string ClassCode;
        
        public decimal FinalSellingPrice;
		public int ItemType;
        public int Quantity;
        public string ClearanceFlag;
        public string CertificateNumber;
        public string ExcludeFromQPA;
        public string CertificateTypeCode;
        public string OrderNumber;

        public string FullFieldStore;
        public string orgTxnNum;
        public string orgTxnStore;
        public string orgtxnDate;
        public string orgOrdNum;
	}
}
