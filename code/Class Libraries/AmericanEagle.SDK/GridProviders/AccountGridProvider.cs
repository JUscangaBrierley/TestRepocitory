using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Text;// AEO Redesign 2015 Begin  & End

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.GridProvider
{
    public class TransactionHeader
    {
        public long RowKey { get; set; }
        public string HeaderId { get; set; }
        public string TransactionNumber { get; set; }
        public string StoreNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public decimal Points { get; set; }
        public string PointType { get; set; }
        public decimal AECCBonusPoints { get; set; }
        public decimal TotalPoints { get; set; }
        public long PointId { get; set; }


        // AEO-Redesing 2015 
        public string BraJeanPrefix { get; set; }        
        public bool isPointConsumption { get; set; }
        public bool loadDetails { get; set; }
        // AEO-Redesign 2015
    }

    public class TransactionDetail
    {
        public string Class { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string SKUNumber { get; set; }
        public decimal Price { get; set; }

        // AEO-Redesing 2015 
        public string BraJeanPrefix { get; set; }
        // AEO-448
        public decimal Saleamount { get; set; }
        //AEO-752
        public string txnDetailId { get; set; }
        public string txnHeaderId { get; set; }
        public int? txnTypeId { get; set; }
    }


    public struct TxnItemStruct
    {
        public string ClassCode;
        //AEO-752 Begin
        public string txnDetailId;
        public string txnHeaderId;
        public int? TxnTypeId;
        //AEO-752 End
        public string ItemDescription;
        public string SKUNumber;   //AEO-1337
        public string TxnType;
        // AEO-448
        public decimal Saleamount { get; set; }
    }

    public class AccountGridProvider : AspGridProviderBase, INestedGridProvider
    {
        /// <summary>
        /// Enumeration to indicate start and end date of quarter 
        /// </summary>
        private enum QuarterDate
        {
            StartDate,
            EndDate
        }

        private string getTxnDescription ( string eventName){

            string retVal = eventName;

            List<string> specialInputVal = new List<string>((new string[] { "AEO or Aerie Credit Card Bonus",    // AEO-384 : Update AECC Purchase
                    "AEO or Aerie Credit Card Bonus Return",
                    "AE SilverTier Purchase",
                    "AE SilverTier Return" }));

            List<string> specialReturnValues = new List<string>((new string[] { "AEO or Aerie Credit Card Bonus",  // AEO-384 : Update AECC Purchase
                    "AEO or Aerie Credit Card Bonus Return",
                    "Silver Purchase",
                    "Silver Return" }));

                if ( specialInputVal.Contains(eventName) )
                {
                    int liPosition = specialInputVal.IndexOf(eventName);

                    retVal= specialReturnValues[liPosition];
                }
               

                return retVal;            
        }

        private List<string> NonDetailsEvents {

            get { 

                List<string> retVal = new List<string>((new string[] { "AEO or Aerie Credit Card Bonus",  // AEO-384 : Update AECC Purchase
                    "AEO or Aerie Credit Card Bonus Return",
                    "AE SilverTier Purchase",
                    "AE SilverTier Return" }));

              
                return retVal;
            }
        }
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        private static LWLogger _logger = LWLoggerManager.GetLogger("AccountActivity");
        private DateTime _startDate = DateTimeUtil.MinValue;
        private DateTime _endDate = DateTimeUtil.MaxValue;
        private List<TransactionHeader> _headers = null;
        
        private string Key_ExcludedPTypesForAccountGrid { get { return "AccountGridProviderPTypesExcluded"; } }
        private string Key_NotExcludedPEventsForAccountGrid { get { return "AccountGridProviderPEventsNotExcluded"; } }

        public IDynamicGridProvider GetChildGridProvider(object keyVal)
        {
            return new AccountChildGridProvider(keyVal);
        }

        public NestingTypes NestingType
        {
            get { return NestingTypes.DataBound; }
        }

        public bool HasChildren(object keyVal)
        {
            return keyVal is string;
        }

        public override int NumberOfRowsPerPage
        {
            get
            {
                return 10;
            }
            set
            {
                base.NumberOfRowsPerPage = value;
            }
        }

        public static String PointsConsumptionFilter {
            get
            {
                return "4";
            }
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            _logger.Trace("AccountGridProvider", "GetColumnSpecs", "Begin");

            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[10];

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "a_txnheaderid";
            c.DisplayText = "Id";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.IsSortable = false;
            columns[0] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "TransactionNumber";
            c.DisplayText = "Trans # / Order #";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[1] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "StoreNumber";
            c.DisplayText = "Store Number";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[2] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "PurchaseDate";
            c.DisplayText = "Purchase Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[3] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "PostDate";
            c.DisplayText = "Post Date";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[4] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Description";
            c.DisplayText = "Description";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.IsVisible = true;
            c.IsSortable = false;            
            columns[5] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Points";
            c.DisplayText = "Points";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.Int64";
            c.DataType = typeof(System.String); //AEO Redesign 2015
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
           
            c.IsEditable = false;
            c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[6] = c;

            /* AEO-1337 Begin */
            c = new DynamicGridColumnSpec();
            c.Name = "AECCBonusPoints";
            c.DisplayText = "AECC Bonus";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.Int64";
            c.DataType = typeof(System.Decimal); //AEO Redesign 2015
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ

            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[7] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "TotalPoints";
            c.DisplayText = "Total Points Earned";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.Int64";
            c.DataType = typeof(System.Decimal); //AEO Redesign 2015
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ

            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[8] = c;
            /* AEO-1337 End */

            c = new DynamicGridColumnSpec();
            c.Name = "PointType";
            c.DisplayText = "Point Type";
            
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
           c.DataType = typeof(System.String); 
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[9] = c;

            return columns;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Method to get Grid Name.
        /// </summary>
        /// <returns>empty</returns>
        protected override string GetGridName()
        {
            return string.Empty;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        public override bool IsGridEditable()
        {
            return false;
        }

        /// <summary>
        /// Grab selected member account date(start and end date of selected quarter)
        /// </summary>
        /// <param name="enmQuarterDate">Enum QuarterDate</param>
        /// <returns>Start date/End date</returns>
        private DateTime GetQuarterStartEndDate(QuarterDate enmQuarterDate)
        {
            DateTime retDate = DateTime.MinValue;

            try
            {
                String DateRange = PortalState.GetFromCache("MemberAccountDate") as String;
                _logger.Trace("AccountGridProvidert", "GetQuarterStartEndDate", "DateRange: " + DateRange);
                if (DateRange.StartsWith("Init"))
                {
                    //The dates from the dropdown are coming from the Page Init so check to see if the grid already has dates.
                    //The reason this code was put in was when you have more than 1 page of data, if you click the 2nd page, the
                    //dropdown was resetting and getting the initial dates.  So, I put code in the LoadGridData to save the dates
                    //it had and if it has dates and the dropdown resets then we just use those dates.
                    DateRange = DateRange.Substring(5);
                    String accountActivityDates = PortalState.GetFromCache("AccountActivityDate") as String;
                    _logger.Trace("AccountGridProvidert", "GetQuarterStartEndDate", "AccountActivityDate: " + accountActivityDates);
                    if ((accountActivityDates!=null) && (accountActivityDates.Length > 0))
                    {
                        _logger.Trace("AccountGridProvidert", "GetQuarterStartEndDate", "get dates from account activity: ");
                        switch (enmQuarterDate)
                        {
                            case QuarterDate.StartDate:
                                String strStartDate = accountActivityDates.Substring(0, accountActivityDates.IndexOf("to"));
                                DateTime dtStartDate = DateTime.Now;
                                DateTime.TryParse(strStartDate, out dtStartDate);
                                retDate = dtStartDate;
                                break;
                            case QuarterDate.EndDate:
                                String strEndDate = accountActivityDates.Substring(accountActivityDates.IndexOf("to") + 3);
                                DateTime dtEndDate = DateTime.Now;
                                DateTime.TryParse(strEndDate, out dtEndDate);
                                retDate = dtEndDate;
                                break;
                            default:
                                break;
                        }
                        return retDate;
                    }
                }
                _logger.Trace("AccountGridProvidert", "GetQuarterStartEndDate", "DateRange After: " + DateRange);
                switch (enmQuarterDate)
                {
                    case QuarterDate.StartDate:
                        String strStartDate = DateRange.Substring(0, DateRange.IndexOf("to"));
                        DateTime dtStartDate = DateTime.Now;
                        DateTime.TryParse(strStartDate, out dtStartDate);
                        retDate = dtStartDate;
                        break;
                    case QuarterDate.EndDate:
                        String strEndDate = DateRange.Substring(DateRange.IndexOf("to") + 3);
                        DateTime dtEndDate = DateTime.Now;
                        DateTime.TryParse(strEndDate, out dtEndDate);
                        retDate = dtEndDate;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Trace("AccountGridProvidert", "GetQuarterStartEndDate", "Error: " + ex.Message);
            }

            return retDate;
        }

        public override void SetSearchParm(string parmName, object parmValue)
        {
            _logger.Trace("AccountGridProvider", "SetSearchParm", "Begin");
            if (string.IsNullOrWhiteSpace(parmName))
            {
                _startDate = DateTimeUtil.MinValue;
                _endDate = DateTimeUtil.MaxValue;
            }
            else if (parmName == "FromDate")
            {
                _startDate = (DateTime)parmValue;
                _startDate = _startDate.AddSeconds(-1);
            }
            else if (parmName == "ToDate")
            {
                _endDate = (DateTime)parmValue;
                _endDate = _endDate.AddDays(1).AddTicks(-1);
            }
        }

        public override void LoadGridData()
        {
            try
            {

                _logger.Trace("AccountGridProvider", "LoadGridData", "Begin");                             
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
                IDataService _DataService = _dataUtil.DataServiceInstance();
                IContentService _ContentService = _dataUtil.ContentServiceInstance();


                _headers = new List<TransactionHeader>();

                Member member = PortalState.GetFromCache("SelectedMember") as Member;

                IList<IClientDataObject> mbrDeta = member.GetChildAttributeSets("MemberDetails", true);
                MemberDetails mbrDetails = mbrDeta == null || mbrDeta.Count == 0 ?
                    null : (MemberDetails)mbrDeta[0];

                bool isPilot = mbrDetails != null ? Utilities.isInPilot(mbrDetails.ExtendedPlayCode) : false; // pilot conversion

                _logger.Trace("AccountGridProvider", "LoadGridData", "Start and End Date Before: " + _startDate.ToShortDateString() + " - " + _endDate.ToShortDateString());
                _startDate = GetQuarterStartEndDate(QuarterDate.StartDate);
                _endDate = GetQuarterStartEndDate(QuarterDate.EndDate);
                _endDate = _endDate.AddDays(1);
                _endDate = _endDate.AddMinutes(-1);

                IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, _startDate, _endDate, true, true, null);
                IList<PointType> bonuspointtype = _LoyaltyData.GetPointTypes(new string[] { "AEO Visa Card Points" }); // MMV-Fix
                IList<PointEvent> bonuspointevent = _LoyaltyData.GetPointEvents(new string[] { "AEO Visa Card Adjustment Points" });  // MMV fix

                if (bonuspointtype != null && bonuspointtype.Count > 0 && bonuspointevent != null && bonuspointevent.Count > 0)
                {

                    PointType aeccpt = null;

                    foreach (PointType r in bonuspointtype)
                    {
                        if (r.Name == "AEO Visa Card Points")
                        {
                            aeccpt = r;
                            break;
                        }
                    }

                    IList<long> bonuspointtypeid = new List<long>();

                    foreach (PointType i in bonuspointtype)
                    {
                        bonuspointtypeid.Add(i.ID);
                    }

                    IList<long> bonuspointeventid = new List<long>();

                    foreach (PointEvent i in bonuspointevent)
                    {
                        bonuspointeventid.Add(i.ID);
                    }
                    IList<PointTransaction> bonusPoints = AccountActivityUtil.GetOtherPointsHistory(member, _startDate, _endDate,
                        null, bonuspointeventid.ToArray<long>(), bonuspointtypeid.ToArray<long>(), true, new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });


                    foreach (PointTransaction bonusPointTxn in bonusPoints)
                    {
                        if (bonusPointTxn.ParentTransactionId <= 0)
                        {
                            var bonus_th = new TransactionHeader();
                            bonus_th.TransactionNumber = null;
                            bonus_th.PurchaseDate = bonusPointTxn.TransactionDate;
                            bonus_th.Points = bonusPointTxn.Points;
                            bonus_th.PostDate = bonusPointTxn.PointAwardDate;
                            bonus_th.PointType = _LoyaltyData.GetPointType(bonusPointTxn.PointTypeId).Name;
                            bonus_th.Description = _LoyaltyData.GetPointEvent(bonusPointTxn.PointEventId).Name;
                            bonus_th.loadDetails = false;
                            bonus_th.AECCBonusPoints = 0;
                            bonus_th.TotalPoints = bonusPointTxn.Points;
                            bonus_th.BraJeanPrefix = null;
                            bonus_th.PointId = bonusPointTxn.Id;

                            if (_headers.Where(p => p.PointId == bonus_th.PointId).Count() == 0)
                                _headers.Add(bonus_th);

                        }
                    }
                }

                /* AEO-2269 end */
                IList<PointTransaction> pointsConsumed = AccountActivityUtil.GetOtherPointsHistory(member, _startDate, _endDate,
                                                                            AccountGridProvider.PointsConsumptionFilter, null, null,
                                                                            true, false,
                                                                            new LWQueryBatchInfo()
                                                                            {
                                                                                BatchSize = 1000,
                                                                                StartIndex = 0
                                                                            });

                foreach (PointTransaction Row in pointsConsumed)
                {
                    if (Row.OwnerType == PointTransactionOwnerType.Reward)
                    {
                        RewardDef rd = null;

                        rd = _ContentService.GetRewardDef(Row.OwnerId);
                        if (rd != null)
                        {

                            PointType ptype = _LoyaltyData.GetPointType(Row.PointTypeId);

                            TransactionHeader txnHdr = new TransactionHeader();
                            txnHdr.Description = "Points Toward " + rd.Name;
                            txnHdr.TransactionNumber = string.Empty;
                            txnHdr.StoreNumber = string.Empty;
                            txnHdr.Points = Row.Points;
                            txnHdr.PointType = ptype != null && ptype.Name != null ? ptype.Name : "No point type defined";
                            txnHdr.isPointConsumption = true;
                            txnHdr.PostDate = Row.PointAwardDate;
                            txnHdr.PurchaseDate = Row.TransactionDate;
                            txnHdr.TotalPoints = txnHdr.Points;
                            txnHdr.PointId = Row.Id;

                            if (_headers.Where(p => p.PointId == txnHdr.PointId).Count() == 0)
                                _headers.Add(txnHdr);
                        }
                    }
                }

                _logger.Trace("AccountGridProvider", "LoadGridData", "Start and End Date: " + _startDate.ToShortDateString() + " - " + _endDate.ToShortDateString());
                _logger.Trace("AccountGridProvider", "LoadGridData", "txnHeaders.Count: " + txnHeaders.Count.ToString());
                if (txnHeaders != null && txnHeaders.Count > 0)
                {
                    string accountActivityDates = string.Concat(_startDate.ToShortDateString(), " to ", _endDate.ToShortDateString());
                    PortalState.PutInCache("AccountActivityDate", accountActivityDates);

                    foreach (IClientDataObject cdo in txnHeaders)
                    {
                        TxnHeader txnHeader = (TxnHeader)cdo;

                        //AEO-629 - BEGIN
                        if (txnHeader.TxnTypeId == 8)
                        {
                            continue;
                        }

                        //AEO-629 - END
                        bool isEmployeeId = !string.IsNullOrEmpty(txnHeader.TxnEmployeeId);
                        string employeeTransMessage = @"<b> - E</b>";
                        decimal totalAECCPoints = 0;
                        var th = new TransactionHeader();
                        th.RowKey = txnHeader.RowKey;
                        _logger.Trace("AccountGridProvider", "LoadGridData", "txnHeader.RowKey: " + txnHeader.RowKey.ToString());

                        // AEO-2015-Redesgin Begin
                        bool hasJean = false;
                        bool hasBra = false;

                        //AEO-448 Begin
                        bool isRedemption = false;
                        //AEO-448 End

                        ClientConfiguration loConfiguration = _DataService.GetClientConfiguration("JeansPromoClassCodes");
                        string lsJeansClassCodes = loConfiguration.Value;

                        loConfiguration = _DataService.GetClientConfiguration("BraPromoClassCodes");
                        string lsBraClassCodes = loConfiguration.Value;

                        lsBraClassCodes = lsBraClassCodes == null ? String.Empty : lsBraClassCodes;
                        lsJeansClassCodes = lsJeansClassCodes == null ? String.Empty : lsJeansClassCodes;

                        // AEO-324 Begin
                        List<string> laBraPromoCodes = new List<string>(lsBraClassCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> laJeanPromoCodes = new List<string>(lsJeansClassCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                        // AEO-324 End

                        _logger.Trace("AccountGridProvider", "LoadGridData", "BraPromoClassCodes: " + lsBraClassCodes);
                        _logger.Trace("AccountGridProvider", "LoadGridData", "JeansPromoClassCodes: " + lsJeansClassCodes);

                        foreach (IClientDataObject txnDetail in txnHeader.GetChildAttributeSets("TxnDetailItem", true))
                        {
                            TxnDetailItem detail = (TxnDetailItem)txnDetail;

                            th.Points += 10 * (((int)detail.DtlQuantity) * Math.Round(detail.DtlSaleAmount, MidpointRounding.AwayFromZero));   // AEO-1337 AH

                            if (detail != null && detail.DtlClassCode != null && !detail.DtlClassCode.Equals(string.Empty))
                            {
                                // AEO-324 Begin
                                hasBra = hasBra ? hasBra : laBraPromoCodes.IndexOf(detail.DtlClassCode) >= 0;
                                hasJean = hasJean ? hasJean : laJeanPromoCodes.IndexOf(detail.DtlClassCode) >= 0;
                                // AEO-324 End
                            }

                            //AEO-448
                            if (detail.DtlSaleAmount.Equals(0.01M) || detail.DtlSaleAmount.Equals(0.0M))
                            {
                                isRedemption = true;
                            }
                            //AEO-448 
                        }

                        // AEO-1337 AH - Begin
                        foreach (IClientDataObject txnTender in txnHeader.GetChildAttributeSets("TxnTender", true))
                        {
                            TxnTender tender = (TxnTender)txnTender;

                            if (tender != null && tender.ParentRowKey == txnHeader.RowKey && (tender.TenderType == 75 || tender.TenderType == 78))
                            {
                                totalAECCPoints += Math.Round(tender.TenderAmount, MidpointRounding.AwayFromZero);
                            }
                        }

                        th.AECCBonusPoints = totalAECCPoints * 5;
                        th.TotalPoints = th.AECCBonusPoints + th.Points;
                        // AEO-1337 AH - End

                        _logger.Trace("AccountGridProvider", "LoadGridData", "hasBra: " + hasBra.ToString());
                        _logger.Trace("AccountGridProvider", "LoadGridData", "hasJeans: " + hasJean.ToString());
                        _logger.Trace("AccountGridProvider", "LoadGridData", "isPilot: " + isPilot.ToString());
                        // AEO-2015-Redesgin End

                        th.HeaderId = txnHeader.TxnHeaderId;
                        if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber.Length > 0) && (txnHeader.ShipDate != null))
                        {
                            th.TransactionNumber = txnHeader.OrderNumber;
                            th.StoreNumber = "Web Order";
                        }
                        else if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber.Length > 0) && (txnHeader.TxnNumber == "0"))
                        {
                            th.TransactionNumber = txnHeader.OrderNumber;
                            var store = _dataUtil.ContentServiceInstance().GetStoreDef(txnHeader.TxnStoreId);
                            if (store != null)
                            {
                                th.StoreNumber = store.StoreNumber;
                            }
                        }
                        else
                        {
                            th.TransactionNumber = txnHeader.TxnNumber;
                            if (cdo.HasTransientProperty("Store"))
                            {
                                _logger.Trace("AccountGridProvider", "LoadGridData", "Store Exists: ");
                                var store = (StoreDef)cdo.GetTransientProperty("Store");
                                if (store != null)
                                {
                                    th.StoreNumber = string.Format("{0}", store.StoreNumber);
                                }
                            }
                        }
                        th.PurchaseDate = txnHeader.TxnDate;

                        _logger.Trace("AccountGridProvider", "LoadGridData", "th.TransactionNumber: " + th.TransactionNumber);

                        //points
                        if (cdo.HasTransientProperty("PointsHistory"))
                        {
                            IList<PointTransaction> points = (IList<PointTransaction>)cdo.GetTransientProperty("PointsHistory");
                            _logger.Trace("AccountGridProvider", "LoadGridData", "PointsHistory: " + points.Count.ToString());
                            foreach (var point in points)
                            {
                                //AEO-1749 --We shouldn't process any Point On Hold
                                if (point.TransactionType != PointBankTransactionType.Hold)
                                {
                                    string pointEvent = _LoyaltyData.GetPointEvent(point.PointEventId).Name;

                                    _logger.Trace("AccountGridProvider", "LoadGridData", "points: " + point.Points.ToString());
                                    string pointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;

                                    bool condition = (pointType == "Bonus Points") || (pointType == "Adjustment Bonus Points") || pointType == "Adjustment Points" ||
                                                        (pointType == "AEO Connected Bonus Points") || (pointType == "Bonus Points") || (pointType == "Adjustment Bonus Points") || (pointType == "Adjustment Points") || (pointType == "AEO Visa Card Points");
                                    //  MMMV - FIX 17Feb2018 end
                                    if (condition)
                                    // AEO Redesign 2015 end
                                    {
                                        var bonus_th = new TransactionHeader();
                                        bonus_th.TransactionNumber = th.TransactionNumber;
                                        bonus_th.PurchaseDate = point.TransactionDate.AddSeconds(1);
                                        bonus_th.Points = point.Points;
                                        bonus_th.PostDate = point.PointAwardDate;
                                        bonus_th.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                                        bonus_th.Description = _LoyaltyData.GetPointEvent(point.PointEventId).Name;
                                        bonus_th.loadDetails = false;
                                        bonus_th.AECCBonusPoints = 0;
                                        bonus_th.TotalPoints = point.Points;
                                        bonus_th.PointId = point.Id;

                                        if (isEmployeeId)
                                        {
                                            bonus_th.PointType = bonus_th.PointType + employeeTransMessage;
                                        }

                                        // AEO Redesign 2015 Begin
                                        if (bonus_th.PointType == "Bonus Points")/*AEO-253 changes here -----------------SCJ*/
                                        {
                                            bonus_th.BraJeanPrefix = null;
                                        }
                                        else if (bonus_th.PointType == "AEO Visa Card Points")
                                        {
                                            bonus_th.BraJeanPrefix = null;
                                        }
                                        else
                                        {
                                            // AEO-431 begin
                                            ClientConfiguration tmp = _DataService.GetClientConfiguration("JeansProgramBeginDate");
                                            String lsJeanStartDate = tmp == null ? null : tmp.Value;

                                            bool jeanMustBeLabeled = true;
                                            if (lsJeanStartDate != null)
                                            {
                                                DateTime tmpDate = new DateTime(1900, 1, 1);
                                                if (DateTime.TryParse(lsJeanStartDate, out tmpDate))
                                                {
                                                    jeanMustBeLabeled = DateTime.Compare(bonus_th.PurchaseDate, tmpDate) >= 0;
                                                }

                                            }

                                            bonus_th.BraJeanPrefix = Utilities.getBraJeanPrefix(hasBra, jeanMustBeLabeled ? hasJean : false,
                                                        isPilot,
                                                        bonus_th.BraJeanPrefix, isRedemption);//AEO-448:isRedemption Flag added
                                                                                                // AEO-431 end 
                                        }
                                        // AEO Redesign 2015 End
                                        if (_headers.Where(p => p.PointId == bonus_th.PointId).Count() == 0)
                                            _headers.Add(bonus_th);
                                    }
                                    else
                                    {
                                        if (pointType != "NetSpend")
                                        {
                                            Boolean exist = false;
                                            // AEO-214 Begin
                                            foreach (TransactionHeader tmpHeader in this._headers)
                                            {
                                                // AEO-2401 if(tmpHeader.TransactionNumber == th.TransactionNumber)
                                                if (tmpHeader.HeaderId == th.HeaderId)
                                                {
                                                    th = tmpHeader;
                                                    exist = true;
                                                    break;
                                                }
                                            }
                                            // AEO-214 End

                                            if (!exist)
                                            {
                                                th.loadDetails = !this.NonDetailsEvents.Contains(_LoyaltyData.GetPointEvent(point.PointEventId).Name);

                                            }
                                            else
                                            {
                                                if (!this.NonDetailsEvents.Contains(_LoyaltyData.GetPointEvent(point.PointEventId).Name))
                                                {
                                                    th.loadDetails = true;
                                                }
                                            }

                                            if (!this.NonDetailsEvents.Contains(_LoyaltyData.GetPointEvent(point.PointEventId).Name))
                                            {
                                                th.PostDate = point.PointAwardDate;
                                                th.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                                                th.Description = this.getTxnDescription(_LoyaltyData.GetPointEvent(point.PointEventId).Name);
                                            }

                                            if (isEmployeeId)
                                            {
                                                th.PointType = th.PointType + employeeTransMessage;
                                            }

                                            // AEO Redesign 2015 Begin
                                            if (th.PointType == "Bonus Points") /*AEO-253 changes here -----------------SCJ*/
                                            {
                                                th.BraJeanPrefix = null;
                                            }
                                            else if (th.PointType == "AEO Visa Card Points")
                                            {
                                                th.BraJeanPrefix = null;
                                            }
                                            else
                                            {
                                                // AEO-431 begin
                                                ClientConfiguration tmp = _DataService.GetClientConfiguration("JeansProgramBeginDate");
                                                String lsJeanStartDate = tmp == null ? null : tmp.Value;
                                                bool jeanMustBeLabeled = true;
                                                if (lsJeanStartDate != null)
                                                {
                                                    DateTime tmpDate = new DateTime(1900, 1, 1);
                                                    if (DateTime.TryParse(lsJeanStartDate, out tmpDate))
                                                    {
                                                        jeanMustBeLabeled = DateTime.Compare(th.PurchaseDate, tmpDate) >= 0;
                                                    }
                                                }

                                                th.BraJeanPrefix = Utilities.getBraJeanPrefix(hasBra, jeanMustBeLabeled ? hasJean : false,
                                                            isPilot, th.BraJeanPrefix, isRedemption);//AEO-448:isRedemption Flag added
                                                                                                     // AEO-431 end 
                                                if (!exist)
                                                {
                                                    _headers.Add(th);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _headers.Add(th);
                        }
                        // AEO Redesign 2015 End

                        // PI 27532, Akbar, Gift Card bonus points - start
                        IList<PointTransaction> giftCardPoints = Utilities.GetGiftCardBonusPoints(txnHeader);
                        if (giftCardPoints != null && giftCardPoints.Count > 0)
                        {
                            foreach (var point in giftCardPoints)
                            {
                                TransactionHeader giftHeader = new TransactionHeader();
                                giftHeader.PurchaseDate = point.TransactionDate.AddSeconds(1);
                                giftHeader.Points = point.Points;
                                giftHeader.PostDate = point.PointAwardDate;
                                giftHeader.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                                giftHeader.Description = _LoyaltyData.GetPointEvent(point.PointEventId).Name;
                                giftHeader.PointId = point.Id;

                                // AEO-1337 AH
                                giftHeader.TotalPoints = point.Points;
                                if (_headers.Where(p => p.PointId == giftHeader.PointId).Count() == 0)
                                    _headers.Add(giftHeader);
                            }
                        }
                        // PI 27532, Akbar, Gift Card bonus points - end
                    }

                }

                //-------------------------------------------

                // finally all pointtrasanction whose owner is unknown are
                // included as txn wthout details

                IList<PointType> pointTypes = _LoyaltyData.GetAllPointTypes();

                long[] pointTypeIDs = new long[pointTypes.Count];
                PointBankTransactionType[] txnTypestoShow = { PointBankTransactionType.Consumed,
                                                        PointBankTransactionType.Credit,
                                                        PointBankTransactionType.Debit};
                PointTransactionOwnerType owner = PointTransactionOwnerType.Unknown;

                //List<PointTransaction> GetPointTransactionsByPointTypePointEvent(Member member, DateTime? from, DateTime? to,
                //PointBankTransactionType[] txnTypes, long[] pointTypeIds, long[] pointEventIds, PointTransactionOwnerType?
                //ownerType, long? ownerId, long[] rowkeys, bool includeExpired, LWQueryBatchInfo batchInfo);

                IList<PointTransaction> filteredList = _LoyaltyData.GetPointTransactionsByPointTypePointEvent(
                        member, _startDate, _endDate,
                //      old code : txnTypestoShow, pointTypeIDs, null, owner, -1, null,
                // AEO-2820 BEGIN
                        txnTypestoShow, null, null, null, -1, null,
                // AEO-2820 END
                        true, null);



                //Start Changes [AEO-5227]
                string batchExcludedPT = _DataService.GetClientConfigProp(this.Key_ExcludedPTypesForAccountGrid);
                if (String.IsNullOrEmpty(batchExcludedPT))
                    throw new Exception("Global Value Configuration [" + this.Key_ExcludedPTypesForAccountGrid + "] is missing.");
                
                string batchNotExcludedPE = _DataService.GetClientConfigProp(this.Key_NotExcludedPEventsForAccountGrid);
                if (String.IsNullOrEmpty(batchNotExcludedPE))
                    throw new Exception("Global Value Configuration [" + this.Key_NotExcludedPEventsForAccountGrid + "] is missing.");

                List<string> lstExcludedPointTypes = new List<string>(batchExcludedPT.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                List<string> lstNotExcludedPointEvents = new List<string>(batchNotExcludedPE.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                //Ends Changes [AEO-5227]

                if (filteredList != null && filteredList.Count > 0)
                {
                    _logger.Trace("AccountGridProvider", "LoadGridData", "Load Point Txns: " + filteredList.Count.ToString());
                    foreach (PointTransaction txn in filteredList)
                    {

                        var th = new TransactionHeader();
                        th.PurchaseDate = txn.TransactionDate;
                        th.Points = txn.Points;
                        th.PostDate = txn.PointAwardDate;
                        th.PointType = _LoyaltyData.GetPointType(txn.PointTypeId).Name;
                        th.Description = _LoyaltyData.GetPointEvent(txn.PointEventId).Name;
                        th.AECCBonusPoints = 0;
                        th.TotalPoints = txn.Points;
                        th.PointId = txn.Id;

                        //Start Changes [AEO-5227]
                        bool validTxnPointToDisplay_v2 = false;
                        if (!lstExcludedPointTypes.Contains(th.PointType) || lstNotExcludedPointEvents.Contains(th.Description))
                            validTxnPointToDisplay_v2 = true;

                        if (validTxnPointToDisplay_v2 & _headers.Where(p => p.PointId == th.PointId).Count() == 0)
                            _headers.Add(th);
                        //Ends Changes [AEO-5227]
                    }
                }

                _logger.Trace("AccountGridProvider", "LoadGridData", "Sort the list");
                var newHeaders = _headers.OrderBy(x => x.PurchaseDate).ThenBy(x => x.TransactionNumber).ThenBy(x => x.StoreNumber);

                _headers = newHeaders.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error("AccountGridProvider", "LoadGridData", ex.Message);
            }

            _logger.Trace("AccountGridProvider", "LoadGridData", "End");
        }

        public override int GetNumberOfRows()
        {
            _logger.Trace("AccountGridProvider", "GetNumberOfRows", "Begin");
            return _headers.Count;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            object val = null;
            var header = _headers[rowIndex];
            _logger.Trace("AccountChildGridProvider", "GetColumnData", "header.RowKey: " + header.RowKey.ToString());

            switch (column.Name)
            {
                case "a_txnheaderid":
                    if ( header.HeaderId != null &&  header.HeaderId.Length > 0 )
                    {
                        if ( header.loadDetails )
                        {
                            val = header.HeaderId + "-" + header.loadDetails.ToString();
                        }
                        else {
                            val = null;
                        }
                        
                    }
                    else
                    {
                        val = header.HeaderId;
                    }
                    break;

                case "TransactionNumber":
                    val = header.TransactionNumber;
                    break;

                case "StoreNumber":
                    val = header.StoreNumber;
                    break;

                case "PurchaseDate":
                    val = header.PurchaseDate.ToShortDateString();
                    break;

                case "PostDate":                
                    val =   header.PostDate.ToShortDateString() ;
					break;					

                case "Description":
                    if ( header.isPointConsumption )
                    { 
                        val =  "<STRONG>"+ header.Description+ "</STRONG>";
                    }
                    else 
                    {
                        val = header.Description;
                    }
                    break;

                case "Points":
                    val = header.Points.ToString()  +"<STRONG>"+ header.BraJeanPrefix+ "</STRONG>";
                    break;

                case "AECCBonusPoints":
                    val = header.AECCBonusPoints.ToString();
                    break;

                case "TotalPoints":
                    val = header.TotalPoints.ToString();
                    break;

                case "PointType":
                    val = header.PointType;
                    break;
            }
            return val;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            throw new NotImplementedException();
        }
    }

    public class AccountChildGridProvider : AspGridProviderBase
    {
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        private static LWLogger _logger = LWLoggerManager.GetLogger("AccountActivity");
        private string _keyVal = string.Empty;
        private List<TransactionDetail> _detail = null;

        public AccountChildGridProvider(object keyVal)
        {
            _logger.Trace("AccountChildGridProvider", "AccountChildGridProvider", "keyVal: " + keyVal.ToString());
            //if (keyVal != null && keyVal is long)
            //{
            //    _keyVal = (long)keyVal;
            //}
            if (keyVal != null && keyVal is string)
            {
                _keyVal = keyVal.ToString();
            }

            _logger.Trace("AccountChildGridProvider", "AccountChildGridProvider", "_keyVal: " + _keyVal.ToString());
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {
            DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[7];

            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
            c.Name = "HeaderId";
            c.DisplayText = "Id";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = false;
            c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.IsSortable = false;
            columns[0] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Class";
            c.DisplayText = "Class-Style Number";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[1] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "SKUNumber";
            c.DisplayText = "SKU Number";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[2] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "ItemDescription";
            c.DisplayText = "Item Description";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[3] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Quantity";
            c.DisplayText = "Quantity";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            // c.DataType = "System.Int32";
            c.DataType = typeof(System.Int32);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[4] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Price";
            c.DisplayText = "Price";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.Decimal);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[5] = c;

            c = new DynamicGridColumnSpec();
            c.Name = "Description";
            c.DisplayText = "Transaction Description";
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //c.DataType = "System.String";
            c.DataType = typeof(System.String);
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            columns[6] = c;

            return columns;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Method to get Grid Name.
        /// </summary>
        /// <returns>empty</returns>
        protected override string GetGridName()
        {
            return string.Empty;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        public override bool IsGridEditable()
        {
            return false;
        }
        public override void LoadGridData()
        {
            // AEO-2015-Redesgin Begin
            Member member = PortalState.GetFromCache("SelectedMember") as Member;
            IList<IClientDataObject> mbrDeta = member.GetChildAttributeSets("MemberDetails", true);
            MemberDetails mbrDetails = mbrDeta == null || mbrDeta.Count == 0 ? null : (MemberDetails)mbrDeta[0];
            // AEO-2015-Redesgin End
            TxnHeader loHeader = null; // AEO-431
            ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
            IDataService _DataService = _dataUtil.DataServiceInstance();

            _logger.Trace("AccountChildGridProvider", "LoadGridData", "Begin ");
            try
            {
                _detail = new List<TransactionDetail>();

                if (_keyVal.Length > 0)
                {
                    _logger.Trace("AccountChildGridProvider", "LoadGridData", "GetAccountActivityDetail Key: " + _keyVal);

                    int specialCharPos = _keyVal.IndexOf("-");

                    IList<IClientDataObject> txnDetails = new List<IClientDataObject>();

                    if (specialCharPos < 0)
                    {

                        LWCriterion crit = new LWCriterion("TxnDetailItem");
                        crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", _keyVal.ToString().Trim(), LWCriterion.Predicate.Eq);
                        txnDetails = _LoyaltyData.GetAttributeSetObjects(null, "TxnDetailItem", crit, null, false);

                        // AEO-431

                        crit = new LWCriterion("TxnHeader");
                        crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", _keyVal.ToString().Trim(), LWCriterion.Predicate.Eq);
                        IList<IClientDataObject> tmp = _LoyaltyData.GetAttributeSetObjects(null, "TxnHeader", crit, null, false);

                        if (tmp != null && tmp.Count > 0)
                        {
                            loHeader = (TxnHeader)tmp[0];
                        }
                        // AEO-431
                    }
                    else
                    {
                        List<IClientDataObject> filteredDetails = new List<IClientDataObject>();

                        String headerIdPortion = _keyVal.Substring(0, specialCharPos);
                        String flagPortion = _keyVal.Substring(specialCharPos + 1, (_keyVal.Length - 1) - specialCharPos);

                        Boolean loadDetails = false;

                        LWCriterion crit = new LWCriterion("TxnDetailItem");
                        crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", headerIdPortion.Trim(), LWCriterion.Predicate.Eq);

                        if (Boolean.TryParse(flagPortion, out loadDetails) && loadDetails)
                        {
                            txnDetails = _LoyaltyData.GetAttributeSetObjects(null, "TxnDetailItem", crit, null, false);
                        }
                        else
                        {
                            txnDetails = null;
                        }

                        // AEO-431
                        crit = new LWCriterion("TxnHeader");
                        crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", headerIdPortion.Trim(), LWCriterion.Predicate.Eq);

                        IList<IClientDataObject> tmp = _LoyaltyData.GetAttributeSetObjects(null, "TxnHeader", crit, null, false);

                        if (tmp != null && tmp.Count > 0)
                        {
                            loHeader = (TxnHeader)tmp[0];
                        }
                        // AEO-431                                            
                    }

                    if (txnDetails == null)
                    {
                        return;
                    }

                    //Only load items into the system if they are
                    //1 - Normal Sale
                    //2 - Exchange within a normal sale
                    //6 - Normal Return
                    if (txnDetails != null && txnDetails.Count > 0)
                    {
                        _logger.Trace("AccountChildGridProvider", "LoadGridData", "txnDetails count: " + txnDetails.Count.ToString());

                        IList<TxnDetailItem> txnDetailItems = new List<TxnDetailItem>();
                        foreach (IClientDataObject obj in txnDetails)
                        {
                            TxnDetailItem txn = (TxnDetailItem)obj;
                            txnDetailItems.Add(txn);
                        }

                        IList<TxnItemStruct> lstTxnItemStruct = new List<TxnItemStruct>();
                        foreach (TxnDetailItem txnDtlItem in txnDetailItems)
                        {
                            Product product = Utilities.GetProduct((long)txnDtlItem.DtlProductId);  // AEO-1337
                            TxnItemStruct txnItemRec = new TxnItemStruct();

                            txnItemRec.txnDetailId = txnDtlItem.TxnDetailId;//AEO-752
                            txnItemRec.txnHeaderId = txnDtlItem.TxnHeaderId;//AEO-752

                            if (product != null)
                            {
                                txnItemRec.ItemDescription = product.StyleDescription;
                                //AEO-1793 Begin
                                txnItemRec.ClassCode = txnDtlItem.DtlClassCode + '-' + product.StyleCode;// AEO-1337
                                txnItemRec.SKUNumber = product.PartNumber;    // AEO-1337
                                                                                //AEO-1793 End
                            }

                            //AEO-752 Begins
                            txnItemRec.TxnTypeId = txnDtlItem.DtlTypeId;
                            //AEO-752 End

                            if (txnDtlItem.DtlTypeId == 1)
                            {
                                txnItemRec.TxnType = "Purchase";
                            }
                            else if (txnDtlItem.DtlTypeId == 2 || txnDtlItem.DtlTypeId == 6)
                            {
                                txnItemRec.TxnType = "Return";
                            }

                            //AEO-448
                            txnItemRec.Saleamount = txnDtlItem.DtlSaleAmount;

                            if (txnItemRec.ClassCode != "0" && !String.IsNullOrEmpty(txnItemRec.ClassCode) && txnDtlItem.DtlTypeId != null && !String.IsNullOrEmpty(txnItemRec.ItemDescription))
                            {
                                lstTxnItemStruct.Add(txnItemRec);
                            }
                        }
                        _logger.Trace("AccountChildGridProvider", "LoadGridData", "lstTxnItemStruct.Count: " + lstTxnItemStruct.Count);
                        //AEO-752 added txnDetalId & txnHeaderId Begin
                        var qry = lstTxnItemStruct.GroupBy(data => new { data.ClassCode, data.ItemDescription, data.TxnType, data.Saleamount, data.txnDetailId, data.txnHeaderId, data.TxnTypeId, data.SKUNumber },
                                                (key, group) => new
                                                {
                                                    Key1 = key.ClassCode,
                                                    Key2 = key.ItemDescription,
                                                    key3 = key.TxnType,
                                                    key4 = key.Saleamount,
                                                    key5 = key.txnDetailId,
                                                    key6 = key.txnHeaderId,
                                                    key7 = key.TxnTypeId,
                                                    key8 = key.SKUNumber,
                                                    Count = group.Count()
                                                });
                        //AEO-752 End
                        //  bool hasBarcode = false; AEO-799
                        foreach (var item in qry)
                        {
                            // hasBarcode = false; AEO-799
                            TransactionDetail txnDtlItemDistinct = new TransactionDetail();
                            _logger.Trace("AccountChildGridProvider", "LoadGridData", "Group item: " + item.Key1.ToString() + " " + item.Key2 + " " + item.key3 + " " + item.Count);

                            txnDtlItemDistinct.Class = item.Key1;
                            txnDtlItemDistinct.ItemDescription = item.Key2;
                            txnDtlItemDistinct.Quantity = item.Count;
                            txnDtlItemDistinct.Description = item.key3;
                            txnDtlItemDistinct.Saleamount = item.key4;
                            //AEO-752 Begin
                            txnDtlItemDistinct.txnDetailId = item.key5;
                            txnDtlItemDistinct.txnHeaderId = item.key6;
                            txnDtlItemDistinct.txnTypeId = item.key7;
                            txnDtlItemDistinct.SKUNumber = item.key8;
                            txnDtlItemDistinct.Price = item.key4;

                            bool hasJean = false;
                            bool hasBra = false;
                            bool isPilot = true; //AEO-2206 -  404 on CSPortal Account Activity - Solution

                            ClientConfiguration loConfiguration = _DataService.GetClientConfiguration("JeansPromoClassCodes");
                            string lsJeansClassCodes = loConfiguration.Value;

                            loConfiguration = _DataService.GetClientConfiguration("BraPromoClassCodes");
                            string lsBraClassCodes = loConfiguration.Value;

                            lsBraClassCodes = lsBraClassCodes == null ? String.Empty : lsBraClassCodes;
                            lsJeansClassCodes = lsJeansClassCodes == null ? String.Empty : lsJeansClassCodes;

                            // AEO-324 Begin
                            List<string> laBraPromoCodes = new List<string>(lsBraClassCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                            List<string> laJeanPromoCodes = new List<string>(lsJeansClassCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                            // AEO-324 End

                            if (txnDtlItemDistinct != null && !txnDtlItemDistinct.Class.Equals(string.Empty))
                            {
                                // AEO-324 Begin
                                if (!hasBra)
                                    hasBra = laBraPromoCodes.IndexOf(txnDtlItemDistinct.Class.ToString()) >= 0;

                                if (!hasJean)
                                    hasJean = laJeanPromoCodes.IndexOf(txnDtlItemDistinct.Class.ToString()) >= 0;
                                // AEO-324 End
                            }

                            //AEO-448 Begin
                            bool isRedemption = false;
                            //bool hasBarcode = false;//AEO-752 YET
                            if ((txnDtlItemDistinct.Saleamount == 0.0m || txnDtlItemDistinct.Saleamount == 0.01m) && (txnDtlItemDistinct.txnTypeId != 2)) isRedemption = true;
                            //AEO-448 End

                            // AEO-431 begin
                            bool jeanMustBeLabeled = true;
                            ClientConfiguration tmp = _DataService.GetClientConfiguration("JeansProgramBeginDate");
                            String lsJeanStartDate = tmp == null ? null : tmp.Value;

                            if (lsJeanStartDate != null)
                            {
                                DateTime tmpDate = new DateTime(1900, 1, 1);
                                if (DateTime.TryParse(lsJeanStartDate, out tmpDate))
                                {
                                    jeanMustBeLabeled = DateTime.Compare(loHeader.TxnDate, tmpDate) >= 0;
                                }
                            }

                            txnDtlItemDistinct.BraJeanPrefix = Utilities.getBraJeanPrefix(hasBra, jeanMustBeLabeled ? hasJean : false,
                                            isPilot, txnDtlItemDistinct.BraJeanPrefix, isRedemption); //AEO-799 begin & end

                            // AEO-431 end 
                            // AEO-2015-Redesgin End
                            _detail.Add(txnDtlItemDistinct);
                        }
                    }
                    var newDetails = _detail.OrderBy(x => x.Class);
                    _detail = newDetails.ToList();
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("No Account details found"))
                {
                    throw new Exception(ex.Message);
                }
            }
            _logger.Trace("AccountChildGridProvider", "LoadGridData", "End");
        }

        public override int GetNumberOfRows()
        {
            return _detail.Count;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            object val = null;
            TransactionDetail d = _detail[rowIndex];

            switch (column.Name)
            {
                case "HeaderId":
                    val = string.Empty;
                    break;
                case "Class":
                    val = d.Class.ToString() +"<STRONG>"+ d.BraJeanPrefix+ "</STRONG>";// AEO Redesign 2015
                    break;
                case "SKUNumber":
                    val = d.SKUNumber.ToString();
                    break;
                case "ItemDescription":
                    val = d.ItemDescription;
                    break;
                case "Quantity":
                    val = d.Quantity;
                    break;
                case "Price":
                    val = d.Price.ToString("0.00");
                    break;
                case "Description":
                    val = d.Description;
                    break;
            }
            return val;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ 
        //public override void SaveGridData(DynamicGridColumnSpec[] columns, bool update)
        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction update)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            throw new NotImplementedException();
        }

        public override string GetEmptyGridMessage()
        {
            return "No results to display";
        }
    }
}
