using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildTestFiles
{
    public class DiscountRecord
    {

        /*
            STRING TYPE(27)                  2              001-002
            VENDOR ID(999)                  3              003-005
            BEFORE/AFTER INDICATOR(0)        1              006-006
            USER IDENTIFICATION(51)          2              007-008
            DISCOUNT TYPE                    1              009-009
            DISCOUNT REASON CODE            2              010-011
            BARCODE #1                      13            012-024
            BARCODE #2                      16            025-040
            3-DIGIT DISCOUNT REASON CODE  3               041-043
            20-DIGIT bARCODE               20             044-064
            DISCOUNT_AMOUNT                 10           076-085
         * 
         * DISCOUNT TYPE - 1 = ITEM LEVEL; 2 = TRANSACTION LEVEL
         * */
        private string m_DiscountType = string.Empty;
        private string m_Code_3Digit = string.Empty;
        private string m_Code_20Digit = string.Empty;
        private int m_DiscountAmt = 0;

        public DiscountRecord(string DiscountType, string Code_3Digit, string Code_20Digit, int DiscountAmt)
        {
            m_DiscountType = DiscountType;
            m_Code_3Digit = Code_3Digit;
            m_Code_20Digit = Code_20Digit;
            m_DiscountAmt  = DiscountAmt;
        }

        public string DiscountType
        {
            get { return m_DiscountType; }
            set { m_DiscountType = value; }
        }

        public string Code_3Digit
        {
            get { return m_Code_3Digit; }
            set { m_Code_3Digit = value; }
        }
        public string Code_20Digit
        {
            get { return m_Code_20Digit; }
            set { m_Code_20Digit = value; }
        }

        public int DiscountAmt
        {
            get { return m_DiscountAmt; }
            set { m_DiscountAmt = value; }
        }


    }

}
