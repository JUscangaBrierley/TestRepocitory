using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.LoyaltyWare.DataAcquisition.Core.Exceptions;
using Brierley.Clients.AmericanEagle.DataModel;
using System.Collections.Specialized;
using Brierley.ClientDevUtilities.LWGateway;
using LWDataServiceUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil;
using System.Xml.Linq;

namespace AmericanEagle.SDK.OutputProviders
{


    public class RunTransactionRulesDAP : IDAPOutputProvider
    {
        #region [Global Variables]

        private ILWDataServiceUtil _dataUtil;

        private LWLogger _logger = LWLoggerManager.GetLogger("RunTransactionRulesDAP");
        private string _className = "RunTransactionRulesDAP";
        private int _threads;
        private int _errors;
        private List<string> _rules;
        object lockErrors = new object();

        #endregion

        #region [Constructors]

        public RunTransactionRulesDAP(ILWDataServiceUtil dataUtil, string ruleNames, int threads)
        {
            _dataUtil = dataUtil;
            _rules = ruleNames.Split(';').ToList();
            _threads = threads;
        }

        public RunTransactionRulesDAP()
        {
            _dataUtil = LWDataServiceUtil.Instance;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IDAPOutputProvider

        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            _errors = 0;
            //Set rules from parameter in config file
            string ruleNames = parameters["Rules"];
            _rules = ruleNames.Split(';').ToList();

            //Set thread count from config file
            _threads = Convert.ToInt32(parameters["Threads"]);
        }

        public virtual void ProcessMessageBatch(List<string> messageBatch)
        {
            string methodName = "ProcessMessageBatch";
            IList<LoyaltyTxn> txns = new List<LoyaltyTxn>();
            try
            {
                foreach (string str in messageBatch)
                {
                    // Create the XmlDocument.
                    XDocument doc = XDocument.Parse(str);
                    // Loding XML
                    XElement transactionInfos = (XElement)doc.Element("TransactionInfos");
                    if (transactionInfos == null)
                    {
                        throw new LWDAPException("Malformatted message batch, no TransactionInfos found, message batch raw: " + str);
                    }

                    IEnumerable<XElement> transactionsToProcess = transactionInfos.Elements("TransactionInfo");
                    if (transactionsToProcess == null || transactionsToProcess.Count() == 0)
                    {
                        throw new LWDAPException("No TransactionInfo Elements found in batch, message batch raw: " + str);
                    }

                    // Get XML Node
                    foreach (XElement txnElement in transactionsToProcess)
                    {

                        string rowKey = txnElement.Element("RowKey")?.Value;
                        if (string.IsNullOrEmpty(rowKey))
                        {
                            throw new LWDAPException(String.Format("Messages in unexpected format no RowKey {0}", txnElement.ToString()));
                        }

                        string loyaltyIdNumber = txnElement.Element("LoyaltyIdNumber")?.Value;
                        if (string.IsNullOrEmpty(loyaltyIdNumber))
                        {
                            throw new LWDAPException(String.Format("Messages in unexpected format no LoyaltyIdNumber {0}", txnElement.ToString()));
                        }
                        txns.Add(new LoyaltyTxn() { LoyaltyIDNumber = loyaltyIdNumber, RowKey = rowKey });
                    }
                }

                //Run ProcessMember method for each thread
                Parallel.ForEach(txns, new ParallelOptions { MaxDegreeOfParallelism = _threads }, txn => { ProcessRules(txn); });
            }
            catch (Exception ex)
            {
                if (ex is LWDAPException)
                {
                    throw;
                }
                else
                {
                    _logger.Error(_className, methodName, "Unexpected Error: " + ex.ToString());
                    throw ex;
                }
            }

        }
        #endregion

        /// <summary>
        /// This method takes a TxnHeader and Loyalty ID and runs the appropriate rules based on what was sent in the parameters;
        /// </summary>
        /// <param name="header"></param>
        /// <param name="loyaltyIdNumber"></param>
        public virtual void ProcessRules(LoyaltyTxn txn)
        {
            string methodName = "ProcessRules";

            Member member = null;
            VirtualCard vc = null;

            //create rule results for configuring context object soon
            List<ContextObject.RuleResult> results = new List<ContextObject.RuleResult>();
            ContextObject.RuleResult ruleResult = new ContextObject.RuleResult();

            results.Add(ruleResult);

            try
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    //get the member via LID from query using the MemberDao directly here to avoid threading issues where two threads can get the same member from the split second its in cache before the scavenger gets it
                    member = service.MemberDao.RetrieveByLoyaltyIDNumber(txn.LoyaltyIDNumber, true);

                    if (member == null)
                    {
                        throw new LWException("Member not found. LID: " + txn.LoyaltyIDNumber);
                    }

                    //get the virtual card from via the member
                    vc = member.GetLoyaltyCard(txn.LoyaltyIDNumber);

                    if (vc == null)
                    {
                        throw new LWException("Virtual Card not found. LID: " + txn.LoyaltyIDNumber);
                    }

                    TxnHeader header = (TxnHeader)service.GetAttributeSetObject("TxnHeader", Int64.Parse(txn.RowKey), false, false);
                    if (header == null)
                    {
                        throw new LWDAPException("No header found with rowkey: " + txn.RowKey);
                    }
                    header.Parent = vc;

                    Dictionary<string, object> environment = new Dictionary<string, object>();

                    //Create a new context object with the header as the invoking row
                    ContextObject contextObject = new ContextObject()
                    {
                        Owner = vc,
                        Results = results,
                        InvokingRow = header,
                        Mode = RuleExecutionMode.Real,
                        Environment = environment
                    };
                    bool isError = false;
                    //For each rule in the rules list
                    foreach (string ruleName in _rules)
                    {
                        if (!environment.ContainsKey("RuleName"))
                        {
                            environment.Add("RuleName", ruleName);
                        }
                        else
                        {
                            environment["RuleName"] = ruleName;
                        }

                        //Get the rule
                        RuleTrigger rule = service.GetRuleByName(ruleName);
                        if (rule == null)
                        {
                            string error = String.Format("Rule Trigger Name '{0}' is not found", ruleName);
                            _logger.Error(error);
                            isError = true;
                            continue;
                        }
                        try
                        {
                            //Run the rule from the context object
                            service.Execute(rule, contextObject);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.ToString());
                            isError = true;
                            continue;
                        }
                    }
                    if (isError)
                    {
                        _errors++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.ToString());
                lock (lockErrors)
                {
                    _errors++;
                }
            }
        }

        public int Shutdown()
        {
            return _errors;
        }
    }

    public class LoyaltyTxn
    {
        public string LoyaltyIDNumber { get; set; }
        public string RowKey { get; set; }
    }

}
