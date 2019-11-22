using System;

namespace BuildTestFiles
{
	/// <summary>
	/// Summary description for SKUItemDetail.
	/// </summary>
	public struct SKUItemDetail
	{
		public int Quantity;
		public int RetailAmount;
		public decimal ActualAmount;
		public decimal ReturnAmount;
		public int ReturnAmountRounded;
		public string SKUNumber;
		public int Credits;
		public int CreditType;
		public int ItemType;
		public int ClassCode;
		public bool IsReturn;
		public string ClearanceDumpIndicator;
		public int itemHistoryID;
		public int BrandID;
        public int ReasonCode;
        public string ReasonCodeDetail; //PI3412

		public SKUItemDetail
							(
							int Quantity,
							int RetailAmount,
							decimal ActualAmount,
							string SKUNumber,
							int Credits,
							int CreditType,
							int ItemType,
							decimal ReturnAmount,
							int ReturnAmountRounded,
							int ClassCode,
							int BrandID
							)
		{
			this.Quantity = Quantity;
			this.RetailAmount = RetailAmount;
			this.ActualAmount = ActualAmount;
			this.SKUNumber = SKUNumber;
			this.Credits = Credits;
			this.CreditType = CreditType;
			this.ItemType = ItemType;
			this.ReturnAmount = ReturnAmount;
			this.ReturnAmountRounded = ReturnAmountRounded;
			this.ClassCode = ClassCode;
			this.IsReturn = false;
			this.ClearanceDumpIndicator = "";
			this.itemHistoryID = 0;
			this.BrandID = BrandID;
            this.ReasonCode = 0;
            this.ReasonCodeDetail = "";
		}

		public SKUItemDetail
			(
			int Quantity,
			int RetailAmount,
			decimal ActualAmount,
			string SKUNumber,
			int Credits,
			int CreditType,
			int ItemType,
			decimal ReturnAmount,
			int ReturnAmountRounded,
			int ClassCode,
			string ClearanceDumpIndicator,
			int itemHistoryID,
			int BrandID,
            int ReasonCode
			)
		{
			this.Quantity = Quantity;
			this.RetailAmount = RetailAmount;
			this.ActualAmount = ActualAmount;
			this.SKUNumber = SKUNumber;
			this.Credits = Credits;
			this.CreditType = CreditType;
			this.ItemType = ItemType;
			this.ReturnAmount = ReturnAmount;
			this.ReturnAmountRounded = ReturnAmountRounded;
			this.ClassCode = ClassCode;
			this.IsReturn = false;
			this.ClearanceDumpIndicator = ClearanceDumpIndicator;
			this.itemHistoryID = itemHistoryID;
			this.BrandID = BrandID;
            this.ReasonCode = ReasonCode;
            this.ReasonCodeDetail = "";
		}

        		public SKUItemDetail
			(
			int Quantity,
			int RetailAmount,
			decimal ActualAmount,
			string SKUNumber,
			int Credits,
			int CreditType,
			int ItemType,
			decimal ReturnAmount,
			int ReturnAmountRounded,
			int ClassCode,
			string ClearanceDumpIndicator,
			int itemHistoryID,
			int BrandID,
            int ReasonCode,
            string ReasonCodeDetail
			)
		{
			this.Quantity = Quantity;
			this.RetailAmount = RetailAmount;
			this.ActualAmount = ActualAmount;
			this.SKUNumber = SKUNumber;
			this.Credits = Credits;
			this.CreditType = CreditType;
			this.ItemType = ItemType;
			this.ReturnAmount = ReturnAmount;
			this.ReturnAmountRounded = ReturnAmountRounded;
			this.ClassCode = ClassCode;
			this.IsReturn = false;
			this.ClearanceDumpIndicator = ClearanceDumpIndicator;
			this.itemHistoryID = itemHistoryID;
			this.BrandID = BrandID;
            this.ReasonCode = ReasonCode;
            this.ReasonCodeDetail = ReasonCodeDetail;
		}

	}
	public struct PromotionItemDetail
	{
		public string SKUNumber;

		public PromotionItemDetail (string SKUNumber)
		{
			this.SKUNumber = SKUNumber;
		}
	}
	public struct Promotion
	{
		private string m_PromotionCode;

		public Promotion(string PromotionCode)
		{
			this.m_PromotionCode = PromotionCode;
		}
	}
}
