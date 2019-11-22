using System;
using System.Collections;
using System.Collections.Generic;

namespace BuildTestFiles
{
    /// <summary>
    /// Summary description for B2CTlogRecordClass.
    /// </summary>
    public class B2CTlogRecordClass
    {
       
        private string _loyaltynumber;
        private string m_PurchaseDate = string.Empty;
        private string _transactionDate;
        private int _registerNumber;
        private decimal _taxAmount = 0;
        private string _webOrderNumber;
        //private int _transactionType;
        private string _tenderInfo;
        private string _employeeId;
        private int _itemType;
        private int _storeNumber;
        private string _detailitems;
        private decimal _actualAmount;
        private string _country;
        private int _transactionNumber;
        private string _flagstoredoor;
        private string _shipDate;
        private string _foundOrderNum;
        /*
        private int _orgTxnNumber;
        private int _orgStoreNumber;
        private string _orgTransactionDate;
        private int _orgOrderNumber;
        private int _foundOrderAmount;_foundOrderAmount
        */

        public string Loyaltynumber {
            get { return _loyaltynumber; }
            set { _loyaltynumber = value; }
        }
        public string TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; }
        }
        public int RegisterNumber
        {
            get { return _registerNumber; }
            set { _registerNumber = value; }
        }
        public decimal TaxAmount
        {
            get { return _taxAmount; }
            set { _taxAmount = value; }
        }
        public string WebOrderNumber
        {
            get { return _webOrderNumber; }
            set { _webOrderNumber = value; }
        }

        /*
        public int TransactionType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }
        */

        public string TenderInfo
        {
            get { return _tenderInfo; }
            set { _tenderInfo = value; }
        }
        public string EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }
        public int ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
        public int StoreNumber
        {
            get { return _storeNumber; }
            set { _storeNumber = value; }
        }
        public string Detailitems
        {
            get { return _detailitems; }
            set { _detailitems = value; }
        }
        public decimal ActualAmount
        {
            get { return _actualAmount; }
            set { _actualAmount = value; }
        }
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }
        public int TransactionNumber
        {
            get { return _transactionNumber; }
            set { _transactionNumber = value; }
        }/*
        public int OrgTxnNumber
        {
            get { return _orgTxnNumber; }
            set { _orgTxnNumber = value; }
        }
        public int OrgStoreNumber
        {
            get { return _orgStoreNumber; }
            set { _orgStoreNumber = value; }
        }
        public string OrgTransactionDate
        {
            get { return _orgTransactionDate; }
            set { _orgTransactionDate = value; }
        }
        public int OrgOrderNumber
        {
            get { return _orgOrderNumber; }
            set { _orgOrderNumber = value; }
        }
        public int FoundOrderAmount
        {
            get { return _foundOrderAmount; }
            set { _foundOrderAmount = value; }
        }
        */

        public string FoundOrderNum
        {
            get { return _foundOrderNum; }
            set { _foundOrderNum = value; }
        }

        public string FlagStoreDoor
        {
            get { return _flagstoredoor; }
            set { _flagstoredoor = value; }
        }

        public string ShipDate
        {
            get { return _shipDate; }
            set { _shipDate = value; }
        }

        public string PurchaseDate
        {
            get { return m_PurchaseDate; }
            set { m_PurchaseDate = value; }
        }



        public B2CTlogRecordClass()
        {
        }

       
        
    }
}
