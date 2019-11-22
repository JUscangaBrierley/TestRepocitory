//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Brierley.FrameWork.Common.Exceptions;
//using Brierley.FrameWork.Data;
//using Brierley.FrameWork.Data.DomainModel;
//using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;

//namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Auctions
//{
//    public class CreateAuctionListing : AuctionFunctionsBase
//    {
//        #region Fields
//        #endregion

//        #region Construction
//        public CreateAuctionListing() : base("CreateAuctionListing") { }
//        #endregion

//        #region Overriden Methods
//        public override string Invoke(string source, string parms)
//        {
//            try
//            {
//                string response = string.Empty;
//                if (string.IsNullOrEmpty(parms))
//                {
//                    throw new LWOperationInvocationException("No ipcode specified") { ErrorCode = 1 };
//                }

//                AuctionListing listing = null/*DeserializeAuctionListing(Name, parms)*/;

//                ILWAuctionService auctionSvc = LWDataServiceUtil.AuctionServiceInstance();
//                listing = auctionSvc.CreateListing(listing);

//                //response = ResponseUtil.GetXmlResponse(Name, config, listing);
//                //response = GetXmlResponse(config, listing);

//                return response;
//            }
//            catch (LWOperationInvocationException ex)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw new LWOperationInvocationException("Error creating auction listing.") { ErrorCode = 2, Reason = ex.Message };
//            }
//        }

//        protected override void Cleanup()
//        {
//        }
//        #endregion
//    }
//}
