using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetMembers : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetMembers";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetMembers() : base("GetMembers") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            //string methodName = "Invoke";

            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("parameters specified for member serach.") { ErrorCode = 3300 };
                }

                IList<Member> members = null;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                string[] searchOptionTypes = (string[])args["MemberSearchType"];
                string[] searchValues = (string[])args["SearchValue"];
                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
                
                if (searchOptionTypes.Length != searchValues.Length)
                {
                    throw new LWOperationInvocationException("The number of search values provided do not match the number of search options.") { ErrorCode = 3348 };
                }

                for (int i = 0; i < searchOptionTypes.Length; i++)
                {
                    if (members != null && members.Count > 0)
                    {
                        break;
                    }
                    string searchType = searchOptionTypes[i];                   
                    Member member = null;
                    if (string.IsNullOrEmpty(searchType))
                    {
                        throw new LWOperationInvocationException("No search type provided for member search.") { ErrorCode = 3317 };
                    }
                    switch (searchType)
                    {
                        case "MemberID":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No member id provided for member search.") { ErrorCode = 3301 };
                            }
                            member = LoyaltyDataService.LoadMemberFromIPCode(long.Parse(searchValues[i]));
                            if (member != null)
                            {
                                members = new List<Member>();
                                members.Add(member);
                            }
                            break;
                        case "CardID":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No card id provided for member search.") { ErrorCode = 3304 };
                            }
                            member = LoyaltyDataService.LoadMemberFromLoyaltyID(searchValues[i]);
                            if (member != null)
                            {
                                members = new List<Member>();
                                members.Add(member);
                            }
                            break;
                        case "EmailAddress":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No email address provided for member search.") { ErrorCode = 3318 };
                            }
                            member = LoyaltyDataService.LoadMemberFromEmailAddress(searchValues[i]);
                            if (member != null)
                            {
                                members = new List<Member>();
                                members.Add(member);
                            }
                            break;
                        case "PhoneNumber":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No phone number provided for member search.") { ErrorCode = 3319 };
                            }
                            members = LoyaltyDataService.GetMembersByPhoneNumber(searchValues[i], batchInfo);
                            break;
                        case "AlternateID":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No alternate id provided for member search.") { ErrorCode = 3320 };
                            }
                            member = LoyaltyDataService.LoadMemberFromAlternateID(searchValues[i]);
                            if (member != null)
                            {
                                members = new List<Member>();
                                members.Add(member);
                            }
                            break;
                        case "LastName":
                            members = LoyaltyDataService.GetMembersByName(string.Empty, searchValues[i], string.Empty, batchInfo);
                            break;
                        case "Username":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No username provided for member search.") { ErrorCode = 3321 };
                            }
                            member = LoyaltyDataService.LoadMemberFromUserName(searchValues[i]);
                            if (member != null)
                            {
                                members = new List<Member>();
                                members.Add(member);
                            }
                            break;
                        case "PostalCode":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No postal code provided for member search.") { ErrorCode = 3322 };
                            }
                            members = LoyaltyDataService.GetMembersByPostalCode(searchValues[i], batchInfo);
                            break;
                        default:
                            throw new LWOperationInvocationException(string.Format("Invalid search type {0} provided. " + 
                                "Valid search values are: MemberID, CardID, EmailAddress, PhoneNumber, AlternateID, LastName, Username, PostalCode", searchType)) { ErrorCode = 3375 };
                    }
                }
                if (members == null || members.Count == 0)
                {
                    throw new LWOperationInvocationException(string.Format("No members found.")) { ErrorCode = 3323 };
                }
                else
                {
                    APIArguments responseArgs = new APIArguments();
                    responseArgs.Add("member", members);
                    response = SerializationUtils.SerializeResult(Name, Config, responseArgs);                    
                }                             
                return response;
            }
            //catch (LWOperationInvocationException)
            //{
            //    throw;
            //}
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
