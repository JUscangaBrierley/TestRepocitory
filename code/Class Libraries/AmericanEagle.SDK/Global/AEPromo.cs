using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public static class AEPromo
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("AEPromo");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static string[] braClassCodes;
        private static string[] jeansClassCodes;

        static AEPromo()
        {
            using (var ldService = _dataUtil.DataServiceInstance())
            {
                braClassCodes = ldService.GetClientConfiguration("BraPromoClassCodes").Value.Split(';');

                if (braClassCodes.Length == 0)
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bra Class Codes not defined");

                jeansClassCodes = ldService.GetClientConfiguration("JeansPromoClassCodes").Value.Split(';');

                if (jeansClassCodes.Length == 0)
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Jeans Class Codes not defined");
            }
        }
        /// <summary>
        /// Pass in the txn header and list of txn detail items and determine if they are part of a promo.  If so then 
        /// add them to the promo queue so the promotion process can pick them up.
        /// </summary>
        /// <param name="dtlItems"></param>
        /// <param name="txnHeader"></param>
        /// <param name="member"></param>
        public static void CheckForPromo(IList<TxnDetailItem> dtlItems, TxnHeader txnHeader, Member member, string source, out bool IsBraPurchase, out bool IsJeanPurchase)
        {
            int[] promoTypes = new int[2];
            int index = 0;
            int promoType = 0;

            IsBraPurchase = false;
            IsJeanPurchase = false;

            foreach (int promotype in promoTypes)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Before promotype: " + promotype.ToString());
            }

            foreach (TxnDetailItem item in dtlItems)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "class: " + item.DtlClassCode);
                promoType = DeterminPromoType(item.DtlClassCode, (int)item.DtlProductId);

                if (promoType == (int)PromoType.Bra)
                {
                    IsBraPurchase = true;
                }
                if (promoType == (int)PromoType.Jeans)
                {
                    IsJeanPurchase = true;
                }
                if (promoType != 0)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "promo: " + promoType.ToString());
                    if (!promoTypes.Contains(promoType))
                    {
                        promoTypes[index] = promoType;
                        index++;
                    }
                }
            }
            foreach (int promotype in promoTypes)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "After promotype: " + promotype.ToString());
            }
            //CreatePromoQueue(txnHeader, member, promoTypes, source);
        }
        /// <summary>
        /// Write all the promo records to the queue
        /// </summary>
        /// <param name="header"></param>
        /// <param name="member"></param>
        /// <param name="promoTypes"></param>
        private static void CreatePromoQueue(TxnHeader header, Member member, int[] promoTypes, string source)
        {
            foreach (int promoType in promoTypes)
            {
                if (promoType > 0)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "create promo: " + promoType.ToString());
                    MemberPromoQueue queue = new MemberPromoQueue();
                    queue.PromoType = promoType;
                    queue.TxnDate = header.TxnDate;
                    queue.TxnNumber = header.TxnNumber;
                    queue.TxnRegisterNumber = header.TxnRegisterNumber;
                    queue.StoreNumber = header.StoreNumber;
                    queue.Source = source;
                    member.AddChildAttributeSet(queue);
                }
            }
        }
        /// <summary>
        /// use the class code to determine if a txn item qualifies for a promo
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private static int DeterminPromoType(string classCode, int productId)
        {
            int promoType = 0;

            try
            {
                if (classCode.Length == 0)
                {
                    classCode = GetProductClass(productId);
                }
                //foreach (string cls in braClassCodes)
                //{
                //    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "braClassCodes: " + cls);
                //}
                if (braClassCodes.Contains(classCode))
                {
                    promoType = (int)PromoType.Bra;
                }
                else if (jeansClassCodes.Contains(classCode))
                {
                    promoType = (int)PromoType.Jeans;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return promoType;
        }
        /// <summary>
        /// the txn item didn't have a class so use the product id to go get the class from the product ref table
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private static string GetProductClass(int productId)
        {
            string classCode = string.Empty;

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "product: " + productId.ToString());
            try
            {
                Product product = Utilities.GetProduct((long)productId);

                if (product != null)
                {
                    classCode = product.ClassCode;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return classCode;
        }
    }
}
