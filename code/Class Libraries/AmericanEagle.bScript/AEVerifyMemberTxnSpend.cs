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
using System.Collections.Generic;
using AmericanEagle.SDK.BScriptHelpers;
using System.Reflection;

namespace AmericanEagle.bScript
{
    /// <summary>
    /// Returns True or False if the member has met a certain qualify spent within a minimum Txn limit in a date range
    /// <para>
    /// This method is case insensitive i.e. will return same for 'name'/'Name' or 'NAME'
    /// </para>
    /// </summary>    
    /// <example>
    /// 1. AEVerifyMemberTxnSpend('StartDate','EndDate','MinTxnCount','HeaderExclusionParms','DetailItemExclusionParms','MinQualifiedSpend')
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns true or false if  a member has met Qualify Spent within a date range and a Minimum Txm limit",
        DisplayName = "AEVerifyMemberTxnSpend",
        ExpressionType = ExpressionTypes.Function,
        ExpressionReturns = ExpressionApplications.Numbers,
        WizardDescription = "Check if Member has met accumulated  transaction spend ",
        AdvancedWizard = false,
        WizardCategory = WizardCategories.Behavior,
        EvalRequiresMember = true)]
    [ExpressionParameter(Name = "StartDate", WizardDescription = "Txn Start Date", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 0)]
    [ExpressionParameter(Name = "EndDate", WizardDescription = "Txn End Date", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 1)]
    [ExpressionParameter(Name = "MinTxnCount", WizardDescription = "Minimum count of eligible transactions ", Type = ExpressionApplications.Numbers, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 2)]
    [ExpressionParameter(Name = "HeaderInclusionParms", WizardDescription = "Expression for transactions to include", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 3)]
    [ExpressionParameter(Name = "DetailItemInclusionParms", WizardDescription = "Expression for detail items to include", Type = ExpressionApplications.Strings, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 4)]
    [ExpressionParameter(Name = "MinQualifiedSpend", WizardDescription = "Minimum Qualified Spend", Type = ExpressionApplications.Numbers, Optional = false, Helpers = ParameterHelpers.ContentType, Order = 5)]
    [ExpressionParameter(Name = "DetailInclusionParamsForTxnCount", WizardDescription = "Expression for details to consider Transactions", Type = ExpressionApplications.Strings, Optional = true, Helpers = ParameterHelpers.ContentType, Order = 6)]
    public class AEVerifyMemberTxnSpend : UnaryOperation
    {
        #region Fields
        const string CLASS_NAME = "AEVerifyMemberTxnSpend";
        const string EVALUATE_METHOD = "evaluate";
        private const string _attributSetNameTxnDetailItem = "TxnDetailItem";
        public Member Member { get; set; }
        public Expression HeaderTxnInclusionParms { get; set; }
        public Expression DetailTxnItemInclusionParms { get; set; }
        public Expression MatchExpressionDetailMakesTxnValid { get; set; }
        public bool ThereIsMatchExpressionForTxnValid { get; set; }

        //public string DetailTxnItemInclusionParmsString { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MinTxnCount { get; set; }
        public string MinQualifiedSpend { get; set; }

        public AEVerifyMemberTxnSpend()
        {
            _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
            txnHelper = new TxnHeaderHelper(_dataUtil);
        }
        /// <summary>
        /// Holds the service data API
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();
        TxnHeaderHelper txnHelper;
        /// <summary>
        /// Holds a reference to the LWLogger class
        /// </summary>
        LWLogger _logger = LWLoggerManager.GetLogger("CustomBScript-AEVerifyMemberTxnSpend");
        #endregion

        public AEVerifyMemberTxnSpend(Expression rhs) : base("AEVerifyMemberTxnSpend", rhs)
        {
            txnHelper = new TxnHeaderHelper(_dataUtil);
            if(!(rhs is ParameterList))
                throw new LWBScriptException("Invalid Function Call: Wrong type for arguments object: type ParameterList is expected");
            if (((ParameterList)rhs).Expressions.Length != 6 && ((ParameterList)rhs).Expressions.Length != 7)
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to AEVerifyMemberTxnSpend");
        }
        public AEVerifyMemberTxnSpend(Expression rhs, ILWDataServiceUtil lwDataServiceUtil) : base("AEVerifyMemberTxnSpend", rhs)
        {
            _dataUtil = lwDataServiceUtil;
            txnHelper = new TxnHeaderHelper(_dataUtil);
            if (!(rhs is ParameterList))
                throw new LWBScriptException("Invalid Function Call: Wrong type for arguments object: type ParameterList is expected");
            if (((ParameterList)rhs).Expressions.Length != 6 && ((ParameterList)rhs).Expressions.Length != 7)
                throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to AEVerifyMemberTxnSpend");
        }

        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                _logger.Debug(CLASS_NAME, EVALUATE_METHOD, "bScript Start.");
                Member member = this.ResolveMember(contextObject.Owner);
                AssignParameters(contextObject);
                _logger.Debug(CLASS_NAME, EVALUATE_METHOD, "AssignParammeter Complete.");
                ValidateParameters(contextObject);
                _logger.Debug(CLASS_NAME, EVALUATE_METHOD, "ValidateParameter Complete.");

                using (var lds = _dataUtil.LoyaltyDataServiceInstance())
                {
                    _logger.Trace(CLASS_NAME, EVALUATE_METHOD, String.Format("TxnHeaderId of InvokingRow:{0}", ((TxnHeader)contextObject.InvokingRow).TxnHeaderId));
                    _logger.Trace(CLASS_NAME, EVALUATE_METHOD, "Loyalty data service initialized.");
                    decimal totalSpend = 0;
                    long pMinTxnCount=Convert.ToInt16(MinTxnCount);
                    decimal pMinQualifiedSpend=Convert.ToDecimal(MinQualifiedSpend);
                    int validTransactions = 0;
                    _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("MinTxnCount:{0}  MinQualifiedSpend:{1}", pMinTxnCount.ToString(), pMinQualifiedSpend.ToString()));
                    //Find the txn headers that met the date range criteria and exclude any that are specified
                    //Finding TxnHeaders on period of time
                    List<TxnHeader> Txnhdrs = FindTxnheaders(lds, StartDate, EndDate, HeaderTxnInclusionParms, member);

                    //Evaluate Per TxnHeader the spend Amount
                    bool amountReached = false;
                    bool minTxnReached = false;
                    int i = 0;
                    do
                    {
                        //Getting Items of the transaction
                        IList<IClientDataObject> txnDtlItems = txnHelper.GetDetailItemsByTxnHeader(Txnhdrs[i]);

                        //Evaluating Items of the transaction => out Values totalSpend, validTransactions
                        if(ThereIsMatchExpressionForTxnValid)
                            this.EvaluateDetailItemsBatch(contextObject, txnDtlItems, this.DetailTxnItemInclusionParms, this.MatchExpressionDetailMakesTxnValid, ref totalSpend, ref validTransactions);
                        else
                            this.EvaluateDetailItemsBatch(contextObject, txnDtlItems, this.DetailTxnItemInclusionParms, ref totalSpend, ref validTransactions);

                        _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("Evaluated TxnHeaderId:{0}", ((TxnHeader)Txnhdrs[i]).TxnHeaderId));
                        _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("validTransactions:{0}  totalSpend:{1}", validTransactions.ToString(), totalSpend.ToString()));

                        //Verify if value has been reachedout
                        if (validTransactions >= pMinTxnCount)
                            minTxnReached = true;
                        if (totalSpend >= pMinQualifiedSpend)
                            amountReached = true;
                        i++;

                    } while ((!amountReached || !minTxnReached) && i< Txnhdrs.Count);

                    //Getting Details(which only exist in memory) from contextObject
                    IList < IClientDataObject > atsListTxnDtlItemsOnMemory = this.txnHelper.GetChildAttributesFromContextObject(contextObject, _attributSetNameTxnDetailItem);
                    //Evaluating for Items on Memory
                    if(this.ThereIsMatchExpressionForTxnValid)
                        this.EvaluateDetailItemsBatch(contextObject, atsListTxnDtlItemsOnMemory, this.DetailTxnItemInclusionParms, this.MatchExpressionDetailMakesTxnValid, ref totalSpend, ref validTransactions);
                    else
                        this.EvaluateDetailItemsBatch(contextObject, atsListTxnDtlItemsOnMemory, this.DetailTxnItemInclusionParms, ref totalSpend, ref validTransactions);

                    _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("Evaluated TxnHeaderId (Current):{0}", ((TxnHeader)contextObject.InvokingRow).TxnHeaderId));
                    _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("validTransactions:{0}  totalSpend:{1}", validTransactions.ToString(), totalSpend.ToString()));

                    _logger.Debug(CLASS_NAME, EVALUATE_METHOD, String.Format("return:{0}", (totalSpend >= pMinQualifiedSpend && validTransactions >= pMinTxnCount).ToString()));

                    if (totalSpend >= pMinQualifiedSpend && validTransactions >= pMinTxnCount)
                        return true;
                    else
                        return false;
                }

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
            StartDate = parameterList.Expressions[0].evaluate(contextObject)?.ToString();
            EndDate = parameterList.Expressions[1].evaluate(contextObject)?.ToString();
            MinTxnCount = parameterList.Expressions[2].evaluate(contextObject)?.ToString();
            HeaderTxnInclusionParms = parameterList.Expressions[3];
            DetailTxnItemInclusionParms = parameterList.Expressions[4];
            MinQualifiedSpend = parameterList.Expressions[5].evaluate(contextObject)?.ToString();
            _logger.Debug(CLASS_NAME, "AssignParameters", String.Format("Num of provided Parameters:{0}", parameterList.Expressions.Length.ToString()));
            if (parameterList.Expressions.Length > 6)
            {
                this.MatchExpressionDetailMakesTxnValid = parameterList.Expressions[6];
                this.ThereIsMatchExpressionForTxnValid = true;
            }
            else
                this.ThereIsMatchExpressionForTxnValid = false;

            _logger.Debug(CLASS_NAME, "AssignParameters", String.Format("ThereIsMatchExpressionForTxnValid:{0}", ThereIsMatchExpressionForTxnValid.ToString()));
        }

        private void ValidateParameters(ContextObject contextObject)
        {
            // Checking parameters, must provide All Parms, OrgTxnDate needs to be a valid date,OrgTxnStoreID and OrgTxnNumber needs to be numeric

            if (string.IsNullOrWhiteSpace(StartDate) || string.IsNullOrWhiteSpace(EndDate) || string.IsNullOrWhiteSpace(MinTxnCount) || string.IsNullOrWhiteSpace(MinQualifiedSpend)
               || string.IsNullOrEmpty(HeaderTxnInclusionParms.evaluate(contextObject).ToString())
               //    || string.IsNullOrEmpty(DetailTxnItemInclusionParms.evaluate(contextObject).ToString())
               )
                throw new LWBScriptException("All parameters must have a value.");

            if (!DateTime.TryParse(StartDate, out DateTime starttxndate))
            {
                throw new LWBScriptException("Could not parse StartDate as date.");
            }
            if (!DateTime.TryParse(EndDate, out DateTime endtxndate))
            {
                throw new LWBScriptException("Could not parse EndDate as date.");
            }
            if (!long.TryParse(MinTxnCount, out long txncount))
            {
                throw new LWBScriptException("Could not parse MinTxnCount as a long.");
            }
            if (!decimal.TryParse(MinQualifiedSpend, out decimal Minspend))
            {
                throw new LWBScriptException("Could not parse MinQualifiedSpend as a decimal.");
            }


        }

        private void EvaluateDetailItemsBatch(ContextObject contextObject, IList<IClientDataObject> atsList, Expression matchingExpression, ref decimal totalSpend, ref int validBatches)
        {
            int qualifiedItems = 0;
            try
            {
                if (atsList != null)
                {
                    for (int i = 0; i < atsList.Count; i++)
                    {
                        if (atsList[i] != null && atsList[i].GetType() == typeof(TxnDetailItem))
                        {
                            //Validating Matching Expression to includ in TotalSpend and validBatch
                            if (Boolean.TryParse(matchingExpression.evaluate(new ContextObject()
                            {
                                InvokingRow = atsList[i],
                                Owner = contextObject.Owner
                            }).ToString(), out bool evaluationIsTrue)
                            )
                            {
                                var txnDetailItem = (TxnDetailItem)atsList[i];
                                if (evaluationIsTrue)
                                {
                                    totalSpend += txnDetailItem.DtlSaleAmount;
                                    qualifiedItems++;
                                }
                            }
                        }
                    }
                    if (qualifiedItems > 0)
                        validBatches++;
                }
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Function unable to evaluate TxnDetailItems rows " + contextObject.InvokingRow.GetMetaData().Name, ex);
            }
        }

        private void EvaluateDetailItemsBatch(ContextObject contextObject, IList<IClientDataObject> atsList, Expression matchingExpression, Expression matchingExpressionForTxnCount, ref decimal totalSpend, ref int validBatches)
        {
            int qualifiedItems = 0;
            try
            {
                if (atsList != null)
                {
                    for (int i = 0; i < atsList.Count; i++)
                    {
                        if (atsList[i] != null && atsList[i].GetType() == typeof(TxnDetailItem))
                        {
                            //Validating Matching Expression to includ in TotalSpend and validBatch
                            ContextObject detailItemContextObject = new ContextObject()
                            {
                                InvokingRow = atsList[i],
                                Owner = contextObject.Owner
                            };
                            //Validation for Amount
                            if (Boolean.TryParse(matchingExpression.evaluate(detailItemContextObject).ToString(), out bool evaluationIsTrue))
                            {
                                var txnDetailItem = (TxnDetailItem)atsList[i];
                                if (evaluationIsTrue)
                                    totalSpend += txnDetailItem.DtlSaleAmount;
                            }
                            //Validation for Txn Count
                            if (Boolean.TryParse(matchingExpressionForTxnCount.evaluate(detailItemContextObject).ToString(), out bool evaluationForTxnCountIsTrue))
                            {
                                if (evaluationForTxnCountIsTrue)
                                    qualifiedItems++;
                            }
                            
                        }
                    }
                    if (qualifiedItems > 0)
                        validBatches++;
                }
            }
            catch (Exception ex)
            {
                throw new LWBScriptException("Function unable to evaluate TxnDetailItems rows " + contextObject.InvokingRow.GetMetaData().Name, ex);
            }
        }

        private List<TxnHeader> FindTxnheaders(ILoyaltyDataService lds, string Startdate, string EndDate, Expression Inclusioncritera, Member mem)
        {
            AttributeSetMetaData attributeSetMetaData = lds.GetAttributeSetMetaData("TxnHeader");
            List<TxnHeader> Txnheaders = new List<TxnHeader>();
            DateTime.TryParse(StartDate, out DateTime TxnStartdate);
            DateTime.TryParse(EndDate, out DateTime TxnEndtdate);

            // First, get the TxnHeader using the parms
            var lwCriteria = new LWCriterion("TxnHeader");
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetBeginningOfDay(TxnStartdate), LWCriterion.Predicate.Ge);
            lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetEndOfDay(TxnEndtdate), LWCriterion.Predicate.Le);
            var clientDataObjects = lds.GetAttributeSetObjects(mem.LoyaltyCards.ToArray(), attributeSetMetaData, lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
            if (clientDataObjects != null)
            {
                foreach (ClientDataObject cDataObj in clientDataObjects)
                {// Check the excludiong expression to remove unwanted txnheader
                    if (Boolean.TryParse(Inclusioncritera.evaluate(new ContextObject() { InvokingRow = cDataObj, Owner = mem }).ToString(), out bool evaluationIsTrue))
                    {
                        var txnHeader = (TxnHeader)cDataObj;
                        if (evaluationIsTrue)
                        {
                            if (txnHeader != null)
                            { Txnheaders.Add(txnHeader); }
                        }
                    }
                }
            }
            return Txnheaders;
        }

    }
}

