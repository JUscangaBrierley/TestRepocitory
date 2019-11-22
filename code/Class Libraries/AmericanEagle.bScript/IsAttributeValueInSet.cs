using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.DomainModel;
//LW 4.1.14 change
//using Brierley.DNNModules.PortalModuleSDK;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.bScript
{
    [ExpressionContext(Description = "This function will return true/false if the source value(s) are in the set supplied",
        DisplayName = "IsAttributeValueInSet",
        ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member | ExpressionApplications.Strings | ExpressionApplications.Objects,
        ExpressionReturns = ExpressionApplications.Booleans)]
    public class IsAttributeValueInSet : UnaryOperation
    {

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private IClientDataObject invokingRow = null;
        /// <summary>
        /// 
        /// </summary>
        private string invokingRowChildAttributeSet = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string invokingRowAttribute = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string searchAttributeSet = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string searchAttribute = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string inSetAttribute = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private string inSetValues = string.Empty;
        /// <summary>
        /// Holds the service data API
        /// </summary>
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        /// <summary>
        /// Holds a reference to the LWLogger class
        /// </summary>
        LWLogger _logger = LWLoggerManager.GetLogger("CustomBScript-IsAttributeValueInSet");
        #endregion

        #region Constructor
        public IsAttributeValueInSet()
        {

        }
        public IsAttributeValueInSet(Expression rhs) :
            base("IsAttributeValueInSet", rhs)
        {
            ParameterList pList = rhs as ParameterList;
            ContextObject cObj = new ContextObject();            

            if (pList.Expressions.Length == 6)
            {
                invokingRowChildAttributeSet = pList.Expressions[0].ToString();
                invokingRowAttribute = pList.Expressions[1].ToString();
                searchAttributeSet = pList.Expressions[2].ToString();
                searchAttribute = pList.Expressions[3].ToString();
                inSetAttribute = pList.Expressions[4].ToString();
                inSetValues = pList.Expressions[5].ToString();
            }
            else
            {
                throw new LWException("You did not supply all the values required by IsAttributeValueInSet!");
            }

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            IList<IClientDataObject> childAttributeSet = null;
            long[] rowKeyArray = null;
            bool returnValue = false;
            invokingRow = contextObject.InvokingRow;

            if (invokingRowChildAttributeSet != string.Empty)
            {
                childAttributeSet = invokingRow.GetChildAttributeSets(invokingRowChildAttributeSet);
            }

            if (childAttributeSet != null && childAttributeSet.Count > 0)
            {
                rowKeyArray = new long[childAttributeSet.Count];

                int i = 0;

                foreach (IClientDataObject obj in childAttributeSet)
                {
                    object rowKey = obj.GetAttributeValue(invokingRowAttribute);

                    rowKeyArray[i] = long.Parse(rowKey.ToString());

                    i++;
                }
            }
            else
            {
                object rowKey = invokingRow.GetAttributeValue(invokingRowAttribute);

                rowKeyArray = new long[1];

                rowKeyArray[0] = long.Parse(rowKey.ToString());
            }

            returnValue = IsProductInSearchList(rowKeyArray);

            _logger.Debug(string.Format("The return value was {0} for TxnID - {1}", returnValue.ToString(), invokingRow.GetAttributeValue("TxnHeaderId").ToString()));

            return returnValue;
        }

        #endregion

        #region Private Methods

        private bool IsProductInSearchList(long[] rowKeys)
        {
            IDataService _DataService = _dataUtil.DataServiceInstance();
            IContentService _ContentService = _dataUtil.ContentServiceInstance();
            List<Product> products = _ContentService.GetAllProducts(rowKeys,true).ToList();
            string[] searchAttributes = inSetAttribute.Split(new char[] { '|' });
            string[] searchValues = inSetValues.Split(new char[] { '|' });
            bool[] searchResults = new bool[searchAttributes.Length];

            //PI15655 - Get ClassCodes thru global variable; namely, the inSetValues  
            if (inSetAttribute.ToUpper() == "CLASSCODE")
            {
                try
                {
                    searchValues = _DataService.GetClientConfiguration(inSetValues.ToString()).Value.Split(new char[] { '|' });
                }
                catch (Exception ex)
                {
                    _logger.Error("Custom bScript", "IsProductInSearchList", "InSetValues (Class Codes) not defined: " + ex.Message);

                }
            }


            List<Product> foundProduct = products.FindAll(p =>
            {
                string[] searchValuesArray = null;
                bool productFound = true;

                for (int i = 0; i < searchAttributes.Length; i++)
                {

                    if (searchValues[i].Contains(","))
                    {
                        searchValuesArray = searchValues[i].Split(new char[] { ',' });
                    }
                    else
                    {
                        searchValuesArray = new string[1];
                        searchValuesArray[0] = searchValues[i];
                    }

                    switch (searchAttributes[i].ToLower())
                    {
                        case "brandname":
                            if (p.BrandName.ContainsAny(searchValuesArray))
                                searchResults[i] = true;

                            else
                                searchResults[i] = false;
                            break;
                        case "classcode":
                            if (p.ClassCode.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "companycode":
                            if (p.CompanyCode.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "deptcode":
                            if (p.DeptCode.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "divisioncode":
                            if (p.DivisionCode.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "longuserfield":
                            if (p.LongUserField.ToString().ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "partnumber":
                            if (p.PartNumber.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                        case "stylecode":
                            if (p.StyleCode.ContainsAny(searchValuesArray))
                                searchResults[i] = true;
                            else
                                searchResults[i] = false;
                            break;
                    }

                }

                for (int x = 0; x < searchResults.Length; x++)
                {
                    productFound &= searchResults[x];
                }

                return productFound;
            });

            return foundProduct.Count > 0;

        }

        #endregion
    }

    public static class StringExtensions
    {
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length > 0)
            {
                foreach (string value in values)
                {
                    if (str.Equals(value.Trim()))
                        return true;
                }
            }

            return false;
        }
    }
}
