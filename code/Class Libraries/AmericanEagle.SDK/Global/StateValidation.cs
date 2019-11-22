using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public class StateValidation
    {
        private string region = "AmericanEagleCache";
        private string stateAttributeSet = "RefStates";
        private string usaStateAttributeSet = "usaRefStates";
        private string canStateAttributeSet = "canRefStates";
        private IList<RefStates> refUSAStates;
        private IList<RefStates> refCANStates;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private LWLogger _logger = null;

        public StateValidation()
        {
            // Initialize logging
            _logger = LWLoggerManager.GetLogger("StateValidation");
        }
        public void LoadStateList()
        {
            using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

                refUSAStates = (IList<RefStates>)ldService.CacheManager.Get(region, usaStateAttributeSet);
                refCANStates = (IList<RefStates>)ldService.CacheManager.Get(region, canStateAttributeSet);
                if (refUSAStates == null)
                {
                    LWCriterion crit = new LWCriterion(stateAttributeSet);
                    crit.Add(LWCriterion.OperatorType.AND, "CountryCode", "USA", LWCriterion.Predicate.Eq);
                    // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    //  IList<IClientDataObject> objStates = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, stateAttributeSet, crit, new LWQueryBatchInfo(), false);
                    IList<IClientDataObject> objStates = ldService.GetAttributeSetObjects(null, stateAttributeSet, crit, null, false);

                    refUSAStates = new List<RefStates>();
                    foreach (IClientDataObject obj in objStates)
                    {
                        RefStates state = (RefStates)obj;
                        refUSAStates.Add(state);
                    }
                    var newStateList = refUSAStates.OrderBy(x => x.FullName);
                    refUSAStates = newStateList.ToList();

                    //dataService.CacheManager.Update(usaRegion, stateAttributeSet, refUSAStates);
                    ldService.CacheManager.Add(region, usaStateAttributeSet, refUSAStates);

                }
                if (refCANStates == null)
                {
                    LWCriterion crit = new LWCriterion(stateAttributeSet);
                    crit.Add(LWCriterion.OperatorType.AND, "CountryCode", "CAN", LWCriterion.Predicate.Eq);
                    // AEO-74 Upgrade 4.5 changes here -----------SCJ
                    //IList<IClientDataObject> objStates = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, stateAttributeSet, crit, new LWQueryBatchInfo(), false);
                    IList<IClientDataObject> objStates = ldService.GetAttributeSetObjects(null, stateAttributeSet, crit, null, false);

                    refCANStates = new List<RefStates>();
                    foreach (IClientDataObject obj in objStates)
                    {
                        RefStates state = (RefStates)obj;
                        refCANStates.Add(state);
                    }
                    var newCANStateList = refCANStates.OrderBy(x => x.FullName);
                    refCANStates = newCANStateList.ToList();

                    ldService.CacheManager.Add(region, canStateAttributeSet, refCANStates);
                }
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
        }
        public IList<RefStates> LoadStates(string countryCode)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            IList<RefStates> returnValue = null;
            LoadStateList();

            if (countryCode == "USA")
            {
                returnValue = refUSAStates;
            }
            else
            {
                returnValue = refCANStates;
            }
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return returnValue;
        }
        public string BuildLoadStatesJavaScript()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<script language='javascript'>\n\r");

                sb.Append("function Country(id,name){\n\r");
                sb.Append(" this.id=id;\n\r");
                sb.Append(" this.name=name;\n\r");
                sb.Append(" return true;\n\r");
                sb.Append("}\n\r");

                sb.Append("function State(countryid,id,name){\n\r");
                sb.Append(" this.id=id;\n\r");
                sb.Append(" this.countryid=countryid;\n\r");
                sb.Append(" this.name=name;\n\r");
                sb.Append(" return true;\n\r");
                sb.Append("}\n\r");

                sb.Append("var _countries=[\n\r");
                sb.Append(" new Country('USA','United States'),\n\r");
                sb.Append(" new Country('CAN','Canada')\n\r");
                sb.Append("];\n\r");

                sb.Append("var _states=[\n\r");
                foreach (RefStates state in LoadStates("USA"))
                {
                    sb.Append(" new State('USA','" + state.StateCode + "','" + state.FullName + "'),\n\r");
                }
                foreach (RefStates state in LoadStates("CAN"))
                {
                    sb.Append(" new State('CAN','" + state.StateCode + "','" + state.FullName + "'),\n\r");
                }
                sb.Remove(sb.Length - 3, 3);
                sb.Append("];\n\r");
                sb.Append("</script>\n\r");
                return sb.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("BuildLoadStatesJavaScript: " + e.Message);
            }
        }
        /// <summary>
        /// Method definition for StateIsValid
        /// </summary>
        /// <param name="stateCode">string stateCode</param>
        /// <returns>return returnValue;</returns>
        public bool StateIsValid(string stateCode, string countryCode)
        {
            bool returnValue = false;
            if ((countryCode == null) || (countryCode.Length == 0))
            {
                if (refUSAStates == null)
                {
                    LoadStateList();
                }
                foreach (RefStates state in refUSAStates)
                {
                    if (state.StateCode == stateCode)
                    {
                        returnValue = true;
                    }
                }
            }
            else if (countryCode.ToUpper() == "USA")
            {
                if (refUSAStates == null)
                {
                    LoadStateList();
                }
                foreach (RefStates state in refUSAStates)
                {
                    if (state.StateCode == stateCode)
                    {
                        returnValue = true;
                    }
                }
            }
            else
            {
                if (refCANStates == null)
                {
                    LoadStateList();
                }
                foreach (RefStates state in refCANStates)
                {
                    if (state.StateCode == stateCode)
                    {
                        returnValue = true;
                    }
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Method definition for GetCountryByState
        /// </summary>
        /// <param name="stateCode">string stateCode</param>
        /// <returns>return returnValue</returns>
        public string GetCountryByState(string stateCode)
        {
            string returnValue = string.Empty;

            foreach (RefStates state in refUSAStates)
            {
                if (state.StateCode == stateCode)
                {
                    returnValue = state.CountryCode;
                }
            }

            return returnValue;
        }
    }
}
