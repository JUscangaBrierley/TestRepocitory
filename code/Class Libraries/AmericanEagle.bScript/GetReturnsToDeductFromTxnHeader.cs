using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.bScript;
using System;
using Brierley.FrameWork;
using System.Reflection;
using Brierley.FrameWork.bScript.Functions;
using Brierley.ClientDevUtilities.LWGateway;
using LWDataServiceUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil;
using System.Collections.Generic;
using System.Text;
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.BScriptHelpers;

namespace AmericanEagle.bScript
{
    [Serializable]
    [ExpressionContext(Description = "Get how many transactions that granted points for a Point Event and Point Type qualify for a return due the returned items linked to the transaction header.",
       DisplayName = "GetReturnsToDeductFromTxnHeader",
       ExcludeContext = ExpressionContexts.Survey | ExpressionContexts.Email,
       ExpressionReturns = ExpressionApplications.Numbers,
       ExpressionType = ExpressionTypes.Function,
        WizardDescription = "Get how many transactions that granted points for a Point Event and Point Type qualify for a return due the returned items linked to the transaction header.",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Behavior,
        EvalRequiresMember = true)]

    [ExpressionParameter(Name = "PointTypeName", WizardDescription = "Point Type Name", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 0)]
    [ExpressionParameter(Name = "PointEventName", WizardDescription = "Point Event Name", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 1)]
    [ExpressionParameter(Name = "bsExpressionQAPurchasedItems", WizardDescription = "Expression to be applied on the Detail Items of the Original Transaction", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 2)]
    [ExpressionParameter(Name = "bsExpressionQAReturnedItems", WizardDescription = "Expression to be applied on the Detail Items of the Original Transaction", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 3)]
    [ExpressionParameter(Name = "PartialReturns", WizardDescription = "Define if partial returns are admitted ", Type = ExpressionApplications.Booleans, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 4)]
    public class GetReturnsToDeductFromTxnHeader : UnaryOperation
    {
        #region [Properties]
        private ILWDataServiceUtil _dataUtil;
        private static LWLogger _logger = null;
        private const string _className = "GetReturnsToDeductFromTxnHeader";
        private const string _appName = "CustomBscript-GetReturnsToDeductFromTxnHeader";
        private const string _ownerAttributSetNameTxnHeader = "TxnHeader";
        private const string _attributSetNameTxnDetailItem = "TxnDetailItem";

        private string TxnHeaderIdForOwner { get; set; }
        public Member Member { get; set; }
        public string PointTypeName { get; set; }
        public string PointEventName { get; set; }
        public Expression MatchingExpressionForPurchaseItems { get; set; }
        public Expression MatchingExpressionForReturnItems { get; set; }
        public bool IsPartialReturns { get; set; }
        #endregion

        public GetReturnsToDeductFromTxnHeader()
        {
            _dataUtil = LWDataServiceUtil.Instance;
        }

        public GetReturnsToDeductFromTxnHeader(Expression arg)
            : base(nameof(GetReturnsToDeductFromTxnHeader), arg)
        {
            _dataUtil = LWDataServiceUtil.Instance;
            _logger = LWLoggerManager.GetLogger(_appName);

            ContextObject cObj = new ContextObject();

            if ((arg as ParameterList).Expressions.Length != 5)
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to " + _className + ".");
            //Validate parameter types
        }

        public GetReturnsToDeductFromTxnHeader(Expression arg, ILWDataServiceUtil dataUtil)
            : base(nameof(GetReturnsToDeductFromTxnHeader), arg)
        {
            this._dataUtil = dataUtil;
            _logger = LWLoggerManager.GetLogger(_appName);

            ContextObject cObj = new ContextObject();


            if ((arg as ParameterList).Expressions.Length != 5)
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to " + _className + ".");
        }

        public new String Syntax
        {
            get
            {
                return _className + "('PointTypeName','PointEventName','bsExpressionQAPurchasedItems','bsExpressionQAReturnedItems',PartialReturns)";
            }
        }


        public override object evaluate(ContextObject contextObject)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            TxnHeaderHelper txnHelper = new TxnHeaderHelper(_dataUtil);
            _logger.Trace(_appName, methodName, "Begin");
            PointType pointType;
            PointEvent pointEvent;
            int returnsToDeduct = 0;
            try {
                AssignParameters(contextObject);
                ValidateParameters();
                this.Member = this.ResolveMember(contextObject.Owner);
                _logger.Trace(_appName, methodName, "Member Loaded");
                if (this.Member == null)
                    throw new LWBScriptException("GetReturnsToDeductFromTxnHeader must be evaluated in the context of a loyalty member.");
                
                if (contextObject.InvokingRow.GetType() != typeof(TxnHeader))
                    throw new LWBScriptException("GetReturnsToDeductFromTxnHeader must be evaluated in the context of TxnHeader as Owner.");
                else
                    this.TxnHeaderIdForOwner = ((TxnHeader)contextObject.InvokingRow).TxnHeaderId;
                
                _logger.Trace(_appName, methodName, String.Format("TxnHeaderId of InvokingRow:{0}", this.TxnHeaderIdForOwner));

                //Verifying existence of Point Type and Point Event
                using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance()) {
                    pointType = dataService.GetPointType(PointTypeName);
                    pointEvent = dataService.GetPointEvent(PointEventName);
                }
                if (pointType == null)
                    throw new LWBScriptException("Could not get PointType");

                if (pointEvent == null)
                    throw new LWBScriptException("Could not get PointEvent");
                
                //Getting DetailItems from Current transaction
                IList<IClientDataObject> atsListTxnDtlItems = this.GetChildAttributesFromContextObject(contextObject, _attributSetNameTxnDetailItem);
                if (atsListTxnDtlItems != null) {
                    //Get Different Original transactions on the TxnDetailItems
                    IList<TxnFourPartKey> involvedOriginalTxns = txnHelper.GetOriginalTxnHeaderInfoForReturnedItems(atsListTxnDtlItems);
                    _logger.Trace(_appName, methodName, String.Format("Count of Original TxnHeader Involved:{0}", involvedOriginalTxns.Count));

                    for (int i = 0; i < involvedOriginalTxns.Count; i++)
                    {
                        if (ShouldDeductPoints(contextObject, this.Member, involvedOriginalTxns[i], pointType, pointEvent, atsListTxnDtlItems))
                            returnsToDeduct++;
                    }
                }
            }
            catch (Exception ex)
            {
                GetReturnsToDeductFromTxnHeader._logger.Error("Custom bscript " + nameof(GetReturnsToDeductFromTxnHeader), methodName, "Error evaluating criteria:", ex);
                if (ex is LWBScriptException)
                    throw;
                else
                    throw new LWBScriptException("Error evaluating criteria:", ex);
            }
            _logger.Trace(_appName, methodName, String.Format("Returned value: {0}", returnsToDeduct));
            return returnsToDeduct;
        }

        private bool ShouldDeductPoints(ContextObject contextObject, Member member, TxnFourPartKey txnEvaluated, PointType pointType, PointEvent pointEvent, IList<IClientDataObject> txnDtlItemsInCurrent) {
            string methodName = MethodBase.GetCurrentMethod().Name;
            bool deductPoints = false;
            long qaItemsInOriginal = 0;
            long qaItemsReturned = 0;

            TxnHeaderHelper txnHelper = new TxnHeaderHelper(_dataUtil);
            //Getting TxnHeader
            TxnHeader originalTxnHeader = txnHelper.GetTxnHeader(txnEvaluated);

            //Verify if Original granted points
            if (txnHelper.GetTxnPointsOnOriginal(member, originalTxnHeader, pointType, pointEvent) > 0)
            {
                IList<IClientDataObject> txnDtlItemsInOriginal = txnHelper.GetDetailItemsByTxnHeader(originalTxnHeader);
                _logger.Trace(_appName, methodName, String.Format("Items in Original TH with rowkey {0} are: {1}", originalTxnHeader.RowKey, txnDtlItemsInOriginal.Count));
                //Getting QAItems from original
                qaItemsInOriginal = GetCountOfQualifiedItems(contextObject, txnDtlItemsInOriginal, this.MatchingExpressionForPurchaseItems);
                _logger.Trace(_appName, methodName, String.Format("QA Items in Original TH with rowkey {0} are: {1}", originalTxnHeader.RowKey, qaItemsInOriginal));

                IList<IClientDataObject> dtlItemsInCurrentWithForthPartKey = new List<IClientDataObject>();
                for (int i = 0; i < txnDtlItemsInCurrent.Count; i++)
                {
                    if (typeof(TxnDetailItem) == txnDtlItemsInCurrent[i].GetType())
                    {
                        TxnDetailItem dtlItem = (TxnDetailItem)txnDtlItemsInCurrent[i];
                        if (dtlItem.TxnOriginalOrderNumber == txnEvaluated.OrderNumber &&
                            dtlItem.TxnOriginalTxnNumber == txnEvaluated.TxnNumber &&
                            dtlItem.TxnOriginalStoreId == txnEvaluated.TxnStoreId &&
                            dtlItem.TxnOriginalTxnDate == txnEvaluated.TxnDate )
                        {
                            dtlItemsInCurrentWithForthPartKey.Add(dtlItem);
                        }
                    }
                }
                _logger.Trace(_appName, methodName, String.Format("Returned Items Purchased in TH with rowkey {0} but returned in current TH are: {1}", originalTxnHeader.RowKey, dtlItemsInCurrentWithForthPartKey.Count));

                //Comparing QA Items in original against returned Items
                qaItemsReturned = GetCountOfQualifiedItems(contextObject, dtlItemsInCurrentWithForthPartKey, this.MatchingExpressionForReturnItems);
                _logger.Trace(_appName, methodName, String.Format("Returned QA Items Purchased in TH with rowkey {0} but returned in current TH are: {1}", originalTxnHeader.RowKey, qaItemsReturned));

                if (this.IsPartialReturns)
                {
                    _logger.Trace(_appName, methodName, String.Format("Apply for Parial Returns"));
                    //Getting Returned
                    IList<IClientDataObject> returnedDtlItemsInOtherTxn = txnHelper.GetAllReturnedDetailItemsLinkedToTxnHeader(member, originalTxnHeader);
                    if(returnedDtlItemsInOtherTxn != null)
                        qaItemsReturned += GetCountOfQualifiedItems(contextObject, returnedDtlItemsInOtherTxn, this.MatchingExpressionForReturnItems);

                    _logger.Trace(_appName, methodName, String.Format("Returned Items Purchased in TH with rowkey {0} across time: {1}", originalTxnHeader.RowKey, dtlItemsInCurrentWithForthPartKey.Count));
                }
                //Defining if points should been deducted
                if (qaItemsInOriginal == qaItemsReturned)
                    deductPoints = true;

            }
            return deductPoints;
        }        

        private IList<IClientDataObject> GetChildAttributesFromContextObject(ContextObject contextObject, string attributeSetName)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            
            IList<IClientDataObject> atsList;

            using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
            {
                AttributeSetMetaData attributeSetMetaData = dataService.GetAttributeSetMetaData(attributeSetName);
                //IAttributeSetContainer root = contextObject.InvokingRow != null ? contextObject.InvokingRow : contextObject.Owner;

                if (attributeSetMetaData == null)
                    throw new LWBScriptException("Unable to retrieve metadata for " + attributeSetName);

                IAttributeSetContainer parentContainer = null;
                if (contextObject.InvokingRow.GetType() == typeof(Member))
                    parentContainer = (Member)contextObject.Owner;
                else if (contextObject.InvokingRow.GetType() == typeof(VirtualCard))
                    parentContainer = (VirtualCard)contextObject.Owner;
                else
                {
                    if (contextObject.InvokingRow != null)
                        parentContainer = contextObject.InvokingRow;
                    else
                        parentContainer = contextObject.Owner;
                }

                if (!parentContainer.IsLoaded(attributeSetName))
                {
                    _logger.Trace(_appName, methodName, String.Format("Attribute Childs not loaded"));
                    dataService.LoadAttributeSetList(parentContainer, attributeSetName, false);                    
                }
                atsList = parentContainer.GetChildAttributeSets(attributeSetName);
                _logger.Trace(_appName, methodName, String.Format("Attribute Childs on current:{0}", atsList.Count));
            }
            return atsList;
        }

        private long GetCountOfQualifiedItems(ContextObject contextObject, IList<IClientDataObject> atsList, Expression matchingExpression)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            long num = 0;
            try
            {
                if (atsList != null)
                {
                    for (int i = 0; i < atsList.Count; i++)
                    {
                        if (atsList[i] != null && atsList[i].GetType()==typeof(TxnDetailItem))
                        {
                            if (Boolean.TryParse(matchingExpression.evaluate(new ContextObject() {
                                    InvokingRow = atsList[i],
                                    Owner = contextObject.Owner
                                }).ToString(), out bool evaluationIsTrue)
                            )
                            {
                                var txnDetailItem = (TxnDetailItem)atsList[i];
                                if (evaluationIsTrue)
                                {
                                    if (txnDetailItem.DtlQuantity != null)
                                        num += (long)txnDetailItem.DtlQuantity;
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Function unable to evaluate TxnDetailItems rows " + contextObject.InvokingRow.GetMetaData().Name, ex);
            }
            return num;
        }

        private void AssignParameters(ContextObject contextObject)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            _logger.Trace(_appName, methodName, String.Format("Assign of parameters"));
            
            Member = ResolveMember(contextObject.Owner);

            ParameterList parameterList = GetRight() as ParameterList;
            this.PointTypeName = parameterList.Expressions[0].evaluate(contextObject)?.ToString();
            this.PointEventName = parameterList.Expressions[1].evaluate(contextObject)?.ToString();
            this.MatchingExpressionForPurchaseItems = parameterList.Expressions[2];
            this.MatchingExpressionForReturnItems = parameterList.Expressions[3];
            this.IsPartialReturns = Convert.ToBoolean(parameterList.Expressions[4].evaluate(contextObject)?.ToString());
        }

        private void ValidateParameters()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            _logger.Trace(_appName, methodName, String.Format("Validation of parameters"));
            if (string.IsNullOrWhiteSpace(PointTypeName) || string.IsNullOrWhiteSpace(PointEventName))
                throw new LWBScriptException("All parameters must have a value.");
        }
        
        
    }
}
