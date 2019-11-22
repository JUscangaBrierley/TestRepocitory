using System;

namespace BuildTestFiles
{
	/// <summary>
	/// Summary description for TenderTypeDetail.
	/// </summary>
	public struct TenderTypeDetail
	{
		public int RoundedAmount;
		public decimal ActualAmount;
		public int TenderType;

		public TenderTypeDetail(int TenderType, int RoundedAmount, decimal ActualAmount)
		{
			this.TenderType = TenderType;
			this.RoundedAmount = RoundedAmount;
			this.ActualAmount = ActualAmount;
		}
	}
}
