using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using LWDataServiceUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil;

namespace AmericanEagle.SDK.Global
{
    public class AEPointsUtilies : IAEPointsUtilies
    {
        #region [Private Attributes]
        private ILWDataServiceUtil _dataUtil;
        
        private LWLogger _logger = LWLoggerManager.GetLogger("AEPointsUtility");
        private String ClassName { get { return "AEPointsUtility"; } }

        private string Key_CurrentPointTypesForDollar { get { return "PointTypeNamesForDollarReward"; } }
        private string Key_CurrentPointTypesForBra { get { return "PointTypeNamesForBraReward"; } }
        private string Key_CurrentPointTypesForJean { get { return "PointTypeNamesForJeanReward"; } }
        private string Key_CurrentPointTypesForDollarBasicPoints { get { return "PointTypeNamesForBasicPoints"; } }
        private string Key_CurrentPointTypesForDollarBonusPoints { get { return "PointTypeNamesForBonusPoints"; } }

        #endregion

        #region [Constructors]
        public AEPointsUtilies()
        {
            this._dataUtil = LWDataServiceUtil.Instance;
        }

        public AEPointsUtilies(ILWDataServiceUtil dataUtil)
        {
            this._dataUtil = dataUtil;
        }

        #endregion

        #region [IAEPointsUtility implementation]
        /// <summary>
        /// Returns the current Dollar Member Points (where the Point type don't content "BRA", "JEAN" or "NETSPEND")  
        /// This points will include the Points on Hold
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetTotalDollarPoints(Member member, DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    IList<long> pointTypeIDs = new List<long>();
                    pointTypeIDs = this.GetPointTypeIdsFilteredByType(TypeOfPoint.Dollar);
                    
                    retValue = GetTotalPoints(member, startDate, endDate, pointTypeIDs);
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the current Basic Dollar Member Points (where the Point type is on the defined PointTypes for Basic Dollar Points)  
        /// This points will include the Points on Hold
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetTotalBasicDollarPoints(Member member, DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    IList<long> pointTypeIDs = new List<long>();
                    pointTypeIDs = this.GetPointTypeIdsFilteredByType(TypeOfPoint.BasicDollar);
                    retValue = GetTotalPoints(member, startDate, endDate, pointTypeIDs);
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the current Bonus Dollar Member Points (where the Point type is on the defined PointTypes for Bonus Dollar Points)  
        /// This points will include the Points on Hold
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetTotalBonusDollarPoints(Member member, DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    IList<long> pointTypeIDs = new List<long>();
                    pointTypeIDs = this.GetPointTypeIdsFilteredByType(TypeOfPoint.BonusDollar);
                    retValue = GetTotalPoints(member, startDate, endDate, pointTypeIDs);
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the current Dollar Member Points Eligible for Rewards (where the Point type don't content "BRA", "JEAN" or "NETSPEND")  
        /// This points will not include the Points on Hold
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetDollarPointsEllegibleForRewards(Member member, DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            decimal retValue = 0;
            decimal pointsOnHold = 0;
            if (member != null)
            {
                try
                {
                    IList<long> pointTypeIDs = this.GetPointTypeIdsFilteredByType(TypeOfPoint.Dollar);

                    pointsOnHold = this.GetPointsOnHold(member, startDate, endDate, pointTypeIDs);
                    retValue = GetTotalPoints(member, startDate, endDate, pointTypeIDs);
                    retValue -= pointsOnHold;
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the current Dollar Member Points on Hold (where the Point type don't content "BRA", "JEAN" or "NETSPEND")  
        /// This points will include only the Points on Hold
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetDollarPointsOnHold(Member member, DateTime startDate, DateTime endDate)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    IList<long> pointTypeIDs = this.GetPointTypeIdsFilteredByType(TypeOfPoint.Dollar);                    
                    retValue = this.GetPointsOnHold(member,startDate,endDate,pointTypeIDs);
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method "+ methodName +": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the summarization of Member Points based on a pointTypesId List provided  
        /// This points will include the Points on Hold
        /// Note:On his use, you must be sure to filter by the same type of Points.
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <param name="pointTypeIDs">List of PointTypeId's as filter</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetTotalPoints(Member member, DateTime startDate, DateTime endDate, IList<long> pointTypeIDs)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            Decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    retValue = member.GetPoints(pointTypeIDs.ToArray(), startDate, endDate);
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method GetTotalDollarPoints: " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Returns the summarization of Member Points on Hold based on a pointTypesId List provided  
        /// This points will only include the Points on Hold
        /// Note:On his use, you must be sure to filter by the same type of Points.
        /// </summary>
        /// <param name="member">Instance of a member, if Null then method will return 0</param>
        /// <param name="startDate">Start Date filter for Points</param>
        /// <param name="endDate">End date filter for Points</param>
        /// <param name="pointTypeIDs">List of PointTypeId's as filter</param>
        /// <returns>Decimal which represent the Member Points based on the filters</returns>
        public decimal GetPointsOnHold(Member member, DateTime startDate, DateTime endDate, IList<long> pointTypeIDs)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            Decimal retValue = 0;
            if (member != null)
            {
                try
                {
                    using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                    {
                        IList<PointType> types = this.GetPointTypesFromIdsList(pointTypeIDs);
                        IList<PointEvent> events = dataService.GetAllPointEvents();
                        retValue = dataService.GetPointsOnHold(member.LoyaltyCards, types, events, startDate, endDate);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method "+ methodName + ": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided Member object is Null");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return retValue;
        }
        /// <summary>
        /// Extract object of the type PointType based on a list of Id's provided
        /// </summary>
        /// <param name="pointTypeIDs">List of Point Type Id's</param>
        /// <returns>List of PointType Objects</returns>
        public IList<PointType> GetPointTypesFromIdsList(IList<long> pointTypeIDs)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string msg;
            List<PointType> types = new List<PointType>();
            if (pointTypeIDs != null && pointTypeIDs.Count > 0)
            {
                try
                {
                    using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                    {
                        foreach (var type in pointTypeIDs)
                        {
                            PointType pType = dataService.GetPointType(type);
                            if (pType != null)
                                types.Add(pType);
                        }
                    }
                        
                }
                catch (Exception ex)
                {
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method "+ methodName +": " + ex.Message, ex);
                    throw;
                }
            }
            else
            {
                msg = String.Format("Provided pointTypeIDs List object is Null or doesn't content Items");
                this._logger.Debug(nameof(AEPointsUtilies), methodName, msg);
            }
            return types;
        }
        /// <summary>
        /// Get Point Type Id's based on the Type of Point provided
        /// </summary>
        /// <param name="type">Type of Point</param>
        /// <returns>List of Point Type Id's</returns>
        public IList<long> GetPointTypeIdsFilteredByType(TypeOfPoint type)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            List<PointType> types = new List<PointType>();
            IList<long> pointTypeIDs = new List<long>();
            try
            {
                IList<PointType> pointTypes = new List<PointType>();
                using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                {
                    pointTypes = dataService.GetAllPointTypes();
                }
                switch (type)
                {
                    case TypeOfPoint.Dollar:
                    case TypeOfPoint.BasicDollar:
                    case TypeOfPoint.BonusDollar:
                    case TypeOfPoint.Jean:
                    case TypeOfPoint.Bra:
                        List<string> validPointTypeNames = this.GetValidPointTypeNamesByType(type);
                        validPointTypeNames = validPointTypeNames.ConvertAll(item => item.ToUpper());
                        for (int i = 0; i < pointTypes.Count; i++)
                        {
                            if (validPointTypeNames.Contains(pointTypes[i].Name.ToUpper()))
                                pointTypeIDs.Add(pointTypes[i].ID);
                        }
                        break;
                    case TypeOfPoint.Netspend:
                        foreach (PointType pt in pointTypes)
                        {
                            if (pt.Name.ToUpper().Equals("NETSPEND"))
                                pointTypeIDs.Add(pt.ID);
                        }
                        break;
                }              
            }
            catch (Exception ex) {
                this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                throw;
            }
            return pointTypeIDs;
        }

        /// <summary>
        /// Get Valid Point Type Names based on the Type of Point provided (Dollar, Bra and Jean becomes from a GlobalValue Configuration)
        /// </summary>
        /// <param name="type">Type of Point</param>
        /// <returns>List of the Valid PointType Names for the specified Type</returns>
        public List<string> GetValidPointTypeNamesByType(TypeOfPoint type)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string key="";
            List<string> validPointTypeNames = null;
            switch (type)
            {
                case TypeOfPoint.Dollar:
                    key = this.Key_CurrentPointTypesForDollar;
                    break;
                case TypeOfPoint.Bra:
                    key = this.Key_CurrentPointTypesForBra;
                    break;
                case TypeOfPoint.Jean:
                    key = this.Key_CurrentPointTypesForJean;
                    break;
                case TypeOfPoint.BasicDollar:
                    key = this.Key_CurrentPointTypesForDollarBasicPoints;
                    break;
                case TypeOfPoint.BonusDollar:
                    key = this.Key_CurrentPointTypesForDollarBonusPoints;
                    break;
                default:
                    this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": Code not implemented for Option [TypeOfPoint."+ type.ToString()+ "]");
                    throw new Exception("Code not implemented for Option [TypeOfPoint." + type.ToString() + "]");
            }
            try
            {
                using (IDataService contentService = this._dataUtil.DataServiceInstance())
                {
                    string batchConfig = contentService.GetClientConfigProp(key);
                    if (String.IsNullOrEmpty(batchConfig))
                    {
                        throw new Exception("Global Value Configuration [" + key + "] is missing.");
                    }
                    validPointTypeNames = new List<string>(batchConfig.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            catch (Exception ex) {
                this._logger.Error(nameof(AEPointsUtilies), methodName, "Error executing method " + methodName + ": " + ex.Message, ex);
                throw;
            }
            return validPointTypeNames;
        }
        #endregion
    }
}
