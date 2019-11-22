using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public class LoyaltyCard
    {
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        public enum LoyaltyCardType
        {
            Temporary = 1,
            AE = 2,
            aerie = 3,
            AEOnline = 4,
            AerieOnline = 5,
            AeriePearl = 6,
            AECredit = 7,
            AerieCredit = 8
        }
        /// <summary>
        /// Determines the validity of an AE loyalty card number.
        /// </summary>
        /// <param name="loyaltyNumber">Loyalty number to validate.</param>
        /// <returns>A valid card returns true, otherwise false.</returns>
        public static bool IsLoyaltyNumberValid(long loyaltyNumber)
        {
            bool bIsValid = false;
            /* AEO-2327 begin
            if (70000000000000 <= loyaltyNumber && 89999999999999 >= loyaltyNumber)
            {
               AEO-2327 end */
                // Convert long to array of shorts.
                short[] digits = NumberToArray(loyaltyNumber);

                // 5 == ([1]*7+[2]*3+[3]*7) % 7
                short check1 = (short)((digits[0] * 7 + digits[1] * 3 + digits[2] * 7) % 7);

                if (digits[3] == check1)
                {
                    // 9 == ([4]+[5]+[6]+[7]) % 11
                    short check2 = (short)(((digits[3] + digits[4] + digits[5] + digits[6]) % 11) % 10);

                    if (digits[7] == check2)
                    {
                        // 4 == ([1]*1+[2]*7+[3]*3+[5]*3+[6]*7+[7]*1+[8]*1+[9]*7+[10]*3+[11]*3)/13
                        short check3 = (short)(((digits[0] + digits[1] * 7 + digits[2] * 3 + digits[4] * 3 + digits[5] * 7
                            + digits[6] + digits[7] + digits[8] * 7 + digits[9] * 3 + digits[10] * 3) % 13) % 10);

                        if (digits[11] == check3)
                        {
                            // 5 == ([1]*3+[2]*1+[3]*3+[4]*1+[5]*3+[6]*1+[7]*3+[8]*1+[9]*3+[10]*1+[11]*3+[12]*1+[13]*3)/10
                            short check4 = (short)((digits[0] * 3 + digits[1] + digits[2] * 3 + digits[3] +
                                digits[4] * 3 + digits[5] + digits[6] * 3 + digits[7] + digits[8] * 3 +
                                digits[9] + digits[10] * 3 + digits[11] + digits[12] * 3) % 10);

                            if (digits[13] == check4)
                            {
                                // Number is valid.
                                bIsValid = true;
                            }
                        }
                    }
                }
            // AEO-2327 }

            return bIsValid;
        }

        /// <summary>
        /// Converts a long integer to an array of shorts.
        /// </summary>
        /// <param name="Input">Long integer to convert.</param>
        /// <returns>Array of shorts.</returns>
        public static short[] NumberToArray(long Input)
        {
            // Input 1440 and return {1, 4, 4, 0}.
            int n = 0;
            do
                n++;
            while (Input >= Math.Pow(10, n));

            short[] output = new short[n];

            while (0 < Input)
            {
                output[--n] = (short)(Input % 10);
                Input /= 10;
            }

            return output;
        }
        public static long GetNextHHKey()
        {
            using (var ldService = _dataUtil.DataServiceInstance())
            {
                //Get LastTempLoyaltyNumber from Global Variables
                //Call GetNextNumber(LoyaltyNumber) to increment to the next number
                //return number
                const string key = "LastHHKey";
                //long LastHHkey = 0;
                long NewHHkey = 0;

                NewHHkey = ldService.GetNextID(key);
                //ClientConfiguration config = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration(key);

                //LastHHkey = Convert.ToInt64(config.Value);
                //LastHHkey++;
                //config.Value = LastHHkey.ToString();

                //LWDataServiceUtil.DataServiceInstance(true).UpdateClientConfiguration(config);
                return NewHHkey;
            }

        }
        /// <summary>
        /// Get the next LoyaltyNumber from the database and add the 
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public static long GetNextLoyaltyNumber(LoyaltyCardType cardType)
        {
            using (var ldService = _dataUtil.DataServiceInstance())
            {
                //Get LastTempLoyaltyNumber from Global Variables
                //Call GetNextNumber(LoyaltyNumber) to increment to the next number
                //return number
                string key = "LastTempLoyaltyNumber";
                long lastLoyaltyNumber = 0;
                long NewLoyaltyNumber = 0;

                switch (cardType)
                {
                    case LoyaltyCardType.AE:
                        key = "LastAELoyatlyNumber";
                        break;
                    case LoyaltyCardType.aerie:
                        key = "LastAerieLoyaltyNumber";
                        break;
                    case LoyaltyCardType.AEOnline:
                        key = "LastAEOnlineLoyaltyNumber";
                        break;

                    case LoyaltyCardType.AerieOnline:
                        key = "LastAerieOnlineLoyaltyNumber";
                        break;
                    case LoyaltyCardType.AeriePearl:
                        key = "LastAeriePearlLoyaltyNumber";
                        break;
                    case LoyaltyCardType.AECredit:
                        key = "LastAECreditLoyaltyNumber";
                        break;
                    case LoyaltyCardType.AerieCredit:
                        key = "LastAerieCreditLoyaltyNumber";
                        break;
                    default:
                        key = "LastTempLoyaltyNumber";
                        break;
                }

                lastLoyaltyNumber = ldService.GetNextID(key);

                //ClientConfiguration config = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration(key);

                //LastTempLoyaltyNumber = Convert.ToInt64(config.Value);
                NewLoyaltyNumber = AddCheckDigits(lastLoyaltyNumber);
                //config.Value = NewTempLoyaltyNumber.ToString();

                //LWDataServiceUtil.DataServiceInstance(true).UpdateClientConfiguration(config);
                return NewLoyaltyNumber;
            }
        }
        /// <summary>
        /// Take the raw number from the database and add the necessary check digits 
        /// </summary>
        public static long AddCheckDigits(long lastLoyaltyNumber)
        {

            long workingNumber = lastLoyaltyNumber;

            do
            {
                //long lngRawNumber = CheckedToRaw(workingNumber);
                long lngRawNumber = workingNumber;

                // Increment and convert to 14-elem array with 3, 7, 11 and 13
                //	indices set to 0 (NumberToArrayRaw()).

                //lngRawNumber++;
                short[] a = NumberToArrayRaw(lngRawNumber);

                // Calculate check digits.

                a[3] = (short)((a[0] * 7 + a[1] * 3 + a[2] * 7) % 7);

                a[7] = (short)(((a[3] + a[4] + a[5] + a[6]) % 11) % 10);

                a[11] = (short)(((a[0] + a[1] * 7 + a[2] * 3 + a[4] * 3 + a[5] * 7
                    + a[6] + a[7] + a[8] * 7 + a[9] * 3 + a[10] * 3) % 13) % 10);

                a[13] = (short)((a[0] * 3 + a[1] + a[2] * 3 + a[3] + a[4] * 3 + a[5]
                    + a[6] * 3 + a[7] + a[8] * 3 + a[9] + a[10] * 3 + a[11] + a[12] * 3) % 10);

                // Convert from array back to long.

                long mult = 10000000000000L;
                workingNumber = 0;

                for (int i = 0; i < a.Length; i++)
                {
                    workingNumber += a[i] * mult;
                    mult /= 10;
                }
            } // Keep going until number is valid.  Should be first try.
            while (!IsLoyaltyNumberValid(workingNumber));

            return workingNumber;
        }
        /// <summary>
        /// Converts an AE loyalty number with check digits to a raw number
        /// without check digits.
        /// </summary>
        /// <param name="CheckedNumber">AE loyalty number with check digits.</param>
        /// <returns>Raw number without the check digits.</returns>
        public static long CheckedToRaw(long CheckedNumber)
        {
            // Convert loyalty number to 14-elem array.

            short[] a = NumberToArray(CheckedNumber);

            // Now convert back to long... but skip the 3, 7, 11 and 13
            //	indices.  Those are the check digits.

            long lngRawNumber = 0;
            long mult = 1000000000L;
            for (int i = 0; i < a.Length; i++)
            {
                // Skip over check digits.

                if (3 != i && 7 != i && 11 != i && 13 != i)
                {
                    lngRawNumber += a[i] * mult;
                    mult /= 10;
                }
            }

            return lngRawNumber;
        }
        /// <summary>
        /// Converts the ten-digit raw number to a 14-element array of shorts.
        /// output[3,7,11,13] will be left blank for check digits.
        /// </summary>
        /// <param name="Input">10-digit long number to create array from.</param>
        /// <returns>14-element array of shorts.  output[3,7,11,13] will be left blank for check digits.</returns>
        private static short[] NumberToArrayRaw(long Input)
        {
            // Input 10-digit number lacking the check digits.
            // Return 14-element array, with blank check digits.

            int n = 14;
            short[] output = new short[14];

            while (0 < Input)
            {
                n--;

                // Leave check digits blank.

                if (3 != n && 7 != n && 11 != n && 13 != n)
                {
                    output[n] = (short)(Input % 10);
                    Input /= 10;
                }
            }

            return output;
        }

    }
}
