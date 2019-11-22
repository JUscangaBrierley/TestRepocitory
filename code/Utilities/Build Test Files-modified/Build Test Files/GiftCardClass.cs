using System;

namespace BuildTestFiles
{
	/// <summary>
	/// Summary description for GiftCardClass.
	/// </summary>
	public class GiftCardClass
	{
		private decimal m_giftCardAmount;
		private int m_giftCardQuntity;

		public GiftCardClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public decimal GiftCardAmount
		{
			get{ return m_giftCardAmount; }
			set{ m_giftCardAmount = value; }
		}

		public int GiftCardQuntity
		{
			get{ return m_giftCardQuntity; }
			set{ m_giftCardQuntity = value; }
		}

	}
}
