using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public class FileUtils
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        public enum OfferType
        {
            Email = 1,
            SMS = 2,
            DM = 3,
            // AEO-509 begin
            Email_20 = 4,
            SMS_20 = 5,
            DM_20 = 6
            // AEO-509 end
        }

        public string GetOffer(string typeCode, int month, OfferType offerType)
        {
            string returnType = string.Empty;
            string sequenceType = string.Empty;

            if (offerType == OfferType.Email)
            {
                sequenceType = "EmailBirthdaySequenceNumber";
            }
            else if(offerType == OfferType.SMS)
            {
                sequenceType = "SMSBirthdaySequenceNumber";
            }
            else if ( offerType == OfferType.DM) // AEO=509 begin
            {
                sequenceType = "DMBirthdaySequenceNumber";
            }
            else  if (offerType == OfferType.Email_20) {
                sequenceType ="Email20SequenceNumber";
            }
            else  if (offerType == OfferType.SMS_20) {
                sequenceType ="SMS20SequenceNumber";
               
            } 
            else {
                 sequenceType ="DM20SequenceNumber";
                 
            }// AEO-509 end



            using (var ldService = _dataUtil.DataServiceInstance())
            {
                ClientConfiguration clientConfiguration = ldService.GetClientConfiguration(sequenceType);
                if (clientConfiguration == null)
                {
                    // AEO-509 Begin
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " Sequence not found: " + sequenceType);
                    throw new Exception("Sequence not found" + sequenceType);
                    // AEO-509 end
                }

                string RewardSequenceNumber = clientConfiguration.Value;

                returnType = string.Concat(typeCode, "_", month, "_", RewardSequenceNumber);

            }

            return returnType;
        }

        public void GetNextRewardCertificateNumber(long ipCode, string typeCode, out string certificateNumber, out string offerCode, out DateTime expirationDate)
        {
            using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
            {
                LWCriterion crit = new LWCriterion("RewardShortCode");

                crit.Add(LWCriterion.OperatorType.AND, "TypeCode", typeCode, LWCriterion.Predicate.Eq);


                var rewardShortCode = ldService.GetAttributeSetObjects(null, "RewardShortCode", crit, null, true);

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bar codes found = " + rewardShortCode == null ? "nothing" : rewardShortCode.Count.ToString());

                if (rewardShortCode == null || rewardShortCode.Count == 0)    // AEO-258 BEgin & End
                {
                    throw new Exception("No reward bar code valid for " + typeCode);
                }
                RewardShortCode shortCode = (RewardShortCode)rewardShortCode[0];

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "short code found = " + shortCode.ShortCode);

                //
                // now look for a valid single use code for this short code that is not used
                //
                crit = new LWCriterion("RewardBarCodes");

                crit.Add(LWCriterion.OperatorType.AND, "TypeCode", shortCode.ShortCode, LWCriterion.Predicate.Eq);
                crit.Add(LWCriterion.OperatorType.AND, "Status", 0, LWCriterion.Predicate.Eq);

                var rewardBarCodes = ldService.GetAttributeSetObjects(null, "RewardBarCodes", crit, null, true);

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bar codes found = " + rewardBarCodes == null ? "nothing" : rewardBarCodes.Count.ToString());

                if (rewardBarCodes != null && rewardBarCodes.Count > 0)
                {
                    RewardBarCodes barcodes = (RewardBarCodes)rewardBarCodes[0];

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bar codes found = " + barcodes.RowKey);

                    //mark the barcode as assigned
                    //
                    barcodes.Status = 1;
                    barcodes.IpCode = ipCode;

                    barcodes = (RewardBarCodes)ldService.SaveAttributeSetObject(barcodes, null, RuleExecutionMode.Real, false);

                    // assign the code and type to the reward
                    //
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bar codes updated = " + barcodes.RowKey);

                    certificateNumber = barcodes.BarCode;
                    offerCode = barcodes.TypeCode;
                    expirationDate = (DateTime)shortCode.EffectiveEndDate;
                }
                else
                {

                    throw new Exception("No reward bar code valid for " + typeCode);
                }
            }
        }

    }
}
