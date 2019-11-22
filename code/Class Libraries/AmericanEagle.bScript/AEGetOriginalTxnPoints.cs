using System;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.WebFrameWork.Portal;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork;
using System.Linq;

namespace AmericanEagle.bScript
{
    /// <summary>
    /// Returns the points from the original header transaction for a particular pointtype and pointevent
    /// <para>
    /// This method is case insensitive i.e. will return same for 'name'/'Name' or 'NAME'
    /// </para>
    /// </summary>    
    /// <example>
    /// 1. AEGetOriginalTxnPoints('PointType','PointEvent','OrgTxnDate','OrgTxnStoreID','OrgTxnNumber','OrgTxnOrderNumber')
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Get original transaction points.",
        DisplayName = "AEGetOriginalTxnPoints",
        ExpressionType = ExpressionTypes.Function,
        ExpressionReturns = ExpressionApplications.Numbers,
        WizardDescription = "Get original transaction Promotion points.",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Behavior,
        EvalRequiresMember = true)]
    [ExpressionParameter(Name = "PointTypeName", WizardDescription = "Point Type Name", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 0)]
    [ExpressionParameter(Name = "PointEventName", WizardDescription = "Point Event Name", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 1)]
    [ExpressionParameter(Name = "OrgTxnDate", WizardDescription = "Original Transaction Date", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 2)]
    [ExpressionParameter(Name = "OrgTxnStoreID", WizardDescription = "Original transaction Store Id", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 3)]
    [ExpressionParameter(Name = "OrgTxnNumber", WizardDescription = "Original Transaction Number", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 4)]
    [ExpressionParameter(Name = "OrgTxnOrderNumber", WizardDescription = "Original Transaction OrderNumber", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 5)]

    public class AEGetOriginalTxnPoints : UnaryOperation
    {
        #region Fields
        const string CLASS_NAME = "AEGetOriginalTxnPoints";
        const string EVALUATE_METHOD = "evaluate";

        public Member Member { get; set; }
        public string PointTypeName { get; set; }
        public string PointEventName { get; set; }
        public string OrgTxnDate { get; set; }
        public string OrgTxnStoreID { get; set; }
        public string OrgTxnNumber { get; set; }
        public string OrgTxnOrderNumber { get; set; }

        public AEGetOriginalTxnPoints()
        {
        }
        /// <summary>
        /// Holds the service data API
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();
        /// <summary>
        /// Holds a reference to the LWLogger class
        /// </summary>
        LWLogger _logger = LWLoggerManager.GetLogger("CustomBScript-AEGetOriginalTxnPoints");
        #endregion

        public AEGetOriginalTxnPoints(Expression rhs) : base("AEGetOriginalTxnPoints", rhs)
        {

            if (!(rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 6))
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to AEGetOriginalTxnPoints");
        }

        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                decimal points = 0.0m;
                AssignParameters(contextObject);
                ValidateParameters();

                using (var lds = _dataUtil.LoyaltyDataServiceInstance())
                {
                    _logger.Trace(CLASS_NAME, EVALUATE_METHOD, "Loyalty data service initialized.");

                    long rowkey = GetRowKey(lds);
                    if (rowkey > 0) // txnheader was found and a valid rowkey was passed ,If not then skip up Point Txn table look up
                    {
                        PointType pointType = lds.GetPointType(PointTypeName);
                        PointEvent pointEvent = lds.GetPointEvent(PointEventName);
                        if (pointType == null)
                            throw new LWBScriptException("Could not get PointType");

                        if (pointEvent == null)
                            throw new LWBScriptException("Could not get PointEvent");
                        AttributeSetMetaData atsMetadata = lds.GetAttributeSetMetaData("TxnHeader");
                        if (atsMetadata == null)
                            throw new LWBScriptException("Could not get AttributeSetMetaData");

                        _logger.Trace(CLASS_NAME, EVALUATE_METHOD, "Getting Points Balance.");
                        points = lds.GetPointBalance(
                            vcKeys: Member.GetLoyaltyCardIds(),
                            pointTypeIds: new long[] { pointType.ID },
                            pointEventIds: new long[] { pointEvent.ID },
                            txnTypes: new PointBankTransactionType[] { PointBankTransactionType.Credit },
                            from: null,
                            to: null,
                            awardDateFrom: null,
                            awardDateTo: null,
                            changedBy: null,
                            locationId: null,
                            ownerType: PointTransactionOwnerType.AttributeSet,
                            ownerId: atsMetadata.ID,
                            rowkeys: new long[] { rowkey });
                        
                    }
                    else points = 0;// send in a 0, when transaction is not found
                }
                return points;
            }
            catch (Exception e)
            {
                _logger.Error(CLASS_NAME, EVALUATE_METHOD, string.Format("Failed executing custom bScript. Error Message: {0}", e.Message));
                throw new LWBScriptException(string.Format("Failed executing custom bScript. Error Message: {0}", e.Message));
            }
        }

        private void AssignParameters(ContextObject contextObject)
        {
            Member = ResolveMember(contextObject.Owner);

            ParameterList parameterList = GetRight() as ParameterList;
            PointTypeName = parameterList.Expressions[0].evaluate(contextObject)?.ToString();
            PointEventName = parameterList.Expressions[1].evaluate(contextObject)?.ToString();
            OrgTxnDate = parameterList.Expressions[2].evaluate(contextObject)?.ToString();
            OrgTxnStoreID = parameterList.Expressions[3].evaluate(contextObject)?.ToString();
            OrgTxnNumber = parameterList.Expressions[4].evaluate(contextObject)?.ToString();
            OrgTxnOrderNumber = parameterList.Expressions[5].evaluate(contextObject)?.ToString();
        }

        private void ValidateParameters()
        {
            // Checking parameters, must provide All Parms, OrgTxnDate needs to be a valid date,OrgTxnStoreID and OrgTxnNumber needs to be numeric

            if (string.IsNullOrWhiteSpace(PointTypeName) || string.IsNullOrWhiteSpace(PointEventName) || string.IsNullOrWhiteSpace(OrgTxnDate) || string.IsNullOrWhiteSpace(OrgTxnStoreID) || string.IsNullOrWhiteSpace(OrgTxnNumber) || string.IsNullOrWhiteSpace(OrgTxnOrderNumber))
                throw new LWBScriptException("All parameters must have a value.");

            if (!DateTime.TryParse(OrgTxnDate, out DateTime txndate))
            {
                throw new LWBScriptException("Could not parse OrgTxnDate as date.");
            }

            if (!long.TryParse(OrgTxnStoreID, out long storeId))
            {
                throw new LWBScriptException("Could not parse OrgTxnStoreID as a long.");
            }
            if (!long.TryParse(OrgTxnNumber, out long txnnum))
            {
                throw new LWBScriptException("Could not parse OrgTxnNumber as a long.");
            }

        }

        private long GetRowKey(ILoyaltyDataService lds)
        {
            long rowkey;
            DateTime.TryParse(OrgTxnDate, out DateTime txndate);
            // First, get the TxnHeader using the parms
            var lwCriteria = new LWCriterion("TxnHeader");
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetBeginningOfDay(txndate), LWCriterion.Predicate.Ge);
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetEndOfDay(txndate), LWCriterion.Predicate.Le);
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnStoreId", OrgTxnStoreID, LWCriterion.Predicate.Eq);
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnNumber", OrgTxnNumber, LWCriterion.Predicate.Eq);
            lwCriteria.Add(LWCriterion.OperatorType.AND, "OrderNumber", OrgTxnOrderNumber, LWCriterion.Predicate.Eq);

            var clientDataObjects = lds.GetAttributeSetObjects(null, "TxnHeader", lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
            var txnHeader = clientDataObjects?.Cast<TxnHeader>().FirstOrDefault();

            if (txnHeader == null)
                rowkey = 0;    // Dont error if a txnheader is not found   throw new LWBScriptException("Could not load TxnHeader");
            else
                rowkey = txnHeader.RowKey;

            return rowkey;
        }
    }
}

