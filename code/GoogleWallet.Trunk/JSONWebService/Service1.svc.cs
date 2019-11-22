using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Configuration;
//using AmericanEagle.SDK.Global;
//using AmericanEagle.SDK.Interceptors;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.ClientLib;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Client;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;
using TestPOSTWebService;
using Google.Apis.Walletobjects.v1.Data;
using System.Security.Cryptography.X509Certificates;
using log4net;
using System.Web;
using System.Xml;
using System.Net;
namespace JSONWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Service1));            
        private Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member member;       
        private string _info = string.Empty;
        private string _url = ConfigurationManager.AppSettings["url"].ToString();
        private string _seracct = ConfigurationManager.AppSettings["seracct"].ToString();
        private string _keypath = ConfigurationManager.AppSettings["keypath"].ToString();
        private string _classId = ConfigurationManager.AppSettings["classId"].ToString();
        private string _objectId = ConfigurationManager.AppSettings["objectId"].ToString();
        public LWIntegrationSvcClientManager svcMgr;
       
       static Service1()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "log4net.xml"));
        }

        public Stream WalletRequest(Stream JSONdataStream)
        {
            
            X509Certificate2 certificate = new X509Certificate2(_keypath,"notasecret",X509KeyStorageFlags.Exportable);
              try
                    {
                        _info = "Wallet Batch Process has begun ";
                        logger.Info(_info);
                        svcMgr = new LWIntegrationSvcClientManager(_url, "Walletwebservice", true, string.Empty);
                        StreamReader reader = new StreamReader(JSONdataStream); // Read in our Stream into a string...
                        string JSONdata = reader.ReadToEnd();
                        JavaScriptSerializer jss = new JavaScriptSerializer(); // ..then convert the string into a single "RootObject" record.         
                        WalletRootObject ro = jss.Deserialize<WalletRootObject>(JSONdata);
                        if (ro == null)
                        {
                            IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                            Utils utils = new Utils(_seracct, objects, certificate);
                            TestPOSTWebService.JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Sorry we can't complete this activity", Result = "rejected" };
                            return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                        }
                        #region: link
                        if (ro.method == "link")
                        { // call validate member  
                            _info = "Calling link function";
                            logger.Info(_info);
                          //   _info = "linking id =  " +ro.@params.linkingId + " First Name=  " + ro.@params.walletUser.firstName + " LastName= " + ro.@params.walletUser.lastName + "address1= " + ro.@params.walletUser.addressLine1 + "zip= " + ro.@params.walletUser.zipcode +"others = " + ro.method + " " + ro.id;
                          //    logger.Info(_info);
                            
                            AEValidateMemberOut aevalidate = svcMgr.AEValidateMember(ro.@params.linkingId,ro.@params.walletUser.firstName, ro.@params.walletUser.lastName, ro.@params.walletUser.addressLine1, ro.@params.walletUser.zipcode);
                          //  _info = "aevalidate.IPcode = " + aevalidate.IPcode + "aevalidate.LoyaltyNumber = " + aevalidate.LoyaltyNumber + "aevalidate.Msg =" + aevalidate.Msg + " aevalidate.TotalPoints=" + aevalidate.TotalPoints;
                         //   logger.Info(_info);
                            if (aevalidate.LoyaltyNumber.Equals(ro.@params.linkingId) || aevalidate.LoyaltyNumber != string.Empty)
                            {
                                //Google.Apis.Walletobjects.v1.Data.LoyaltyObject obj = Loyalty.generateLoyaltyObject(_classId, _objectId + aevalidate.IPcode, ro.@params.walletUser.firstName + " " + ro.@params.walletUser.lastName, aevalidate.LoyaltyNumber, aevalidate.TotalPoints, Convert.ToInt64(aevalidate.GWversion));
                                Google.Apis.Walletobjects.v1.Data.LoyaltyObject obj = Loyalty.generateLoyaltyObject(_classId, _objectId + ro.@params.linkingId, ro.@params.walletUser.firstName + " " + ro.@params.walletUser.lastName, aevalidate.LoyaltyNumber, aevalidate.TotalPoints, Convert.ToInt64(aevalidate.GWversion));
                                IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                                objects.Add(obj);
                                Utils utils = new Utils(_seracct, objects, certificate);
                                JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Thanks for linking your AEREWARDS card through Google Wallet", Result = "approved" };
                                return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                            }
                            else
                            //if ((aevalidate.LoyaltyNumber == string.Empty) || !aevalidate.LoyaltyNumber.Equals(ro.@params.linkingId))
                            {
                                //return aevalidate.LoyaltyNumber + "no member found,please signup";                  
                                IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                                Utils utils = new Utils(_seracct, objects, certificate);
                                JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Uh oh… the information you entered doesn't seem to match an existing AEREWARDS account. Please check your membership details and try again.", Result = "rejected" };
                                return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                                
                            }
                            
                        }
                        #endregion : link

                        #region: signup
                        if (ro.method == "signup")
                        {  // CALL VALIDATE MEMBER API  
                            string linkingid = "SIGNUP";
                            //AEValidateMemberOut aevalidate = svcMgr.AEValidateMember(ro.@params.linkingId,ro.@params.walletUser.firstName, ro.@params.walletUser.lastName, ro.@params.walletUser.addressLine1, ro.@params.walletUser.zipcode);
                            AEValidateMemberOut aevalidate = svcMgr.AEValidateMember(linkingid, ro.@params.walletUser.firstName, ro.@params.walletUser.lastName, ro.@params.walletUser.addressLine1, ro.@params.walletUser.zipcode);
                            string strLoyaltyNumber = string.Empty;
                            int strIPcode = 0;
                            if (aevalidate.LoyaltyNumber == string.Empty) // MEMBER DOES NOT EXIST OR INVALID
                            {    
                                
                                int ErrorCode = 0;
                                if (aevalidate.Msg == Convert.ToString(ResponseCode.LoyaltyNumberRequired)) //MEMBER DOES NOT EXIST,So ADD MEMBER
                                {
                                    #region : ADD MEMBER
                                   
                                    //validate other signup inputs before add member
                                    //validate state
                                    if ((ro.@params.walletUser.state == null) || (ro.@params.walletUser.state.Trim().Length == 0))
                                    {
                                        ErrorCode = (int)ResponseCode.StateRequired;
                                    }
                                    //validate city
                                    if ((ro.@params.walletUser.city == null) || (ro.@params.walletUser.city.Trim().Length == 0))
                                    {
                                          ErrorCode = (int)ResponseCode.CityRequired ;
                                    }
                                    else if (!Utilities.IsCityValid(ro.@params.walletUser.city))
                                    {
                                         ErrorCode = (int)ResponseCode.InvalidCity ;
                                    }

                                    //// Validate Address1
                                    //if ((ro.@params.walletUser.addressLine1 == null) || (ro.@params.walletUser.addressLine1.Trim().Length == 0))
                                    //{
                                    //    ErrorCode = (int)ResponseCode.EmailAddressRequired;
                                    //}

                                    //else  if (!Utilities.IsAddressValid(ro.@params.walletUser.addressLine1))
                                    //{
                                    //        ErrorCode = (int)ResponseCode.InvalidAddressLine1;
                                    //}
                                    
                                    // Validate Address2
                                    if ((ro.@params.walletUser.addressLine2 != null) && (ro.@params.walletUser.addressLine2.Trim().Length > 0))
                                    {
                                        if (!Utilities.IsAddressValid(ro.@params.walletUser.addressLine2))
                                        {
                                             ErrorCode = (int)ResponseCode.InvalidAddressLine2 ;
                                        }
                                    }
                                   
                                     // Validate EmailAddress
                                   if ((ro.@params.walletUser.email == null) || (ro.@params.walletUser.email.Trim().Length == 0))
                                        {
                                            ErrorCode = (int)ResponseCode.EmailAddressRequired ;
                                        }

                                    else if (!Utilities.IsEmailValid(ro.@params.walletUser.email))
                                        {
                                            ErrorCode = (int)ResponseCode.InvalidEmailAddress ;
                                        }
                
                                    if (ErrorCode == 0)
                                    {
                                      
                                    member = new Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member();
                                    member.FirstName = ro.@params.walletUser.firstName;
                                    member.LastName = ro.@params.walletUser.lastName;
                                    member.PrimaryEmailAddress = ro.@params.walletUser.email;
                                    member.PrimaryPhoneNumber = ro.@params.walletUser.phone;
                                    member.MemberCreateDate = System.DateTime.Now;
                                    member.MemberStatus = Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member.MemberStatusEnum.Active;
                                    member.BirthDate = Convert.ToDateTime("1/1/1990"); //default DOB to 1/1/1990 as value is not passed from App
                                    MemberDetails details = new MemberDetails();
                                    details.EmailAddress = ro.@params.walletUser.email;
                                    details.AddressLineOne = ro.@params.walletUser.addressLine1;
                                    details.AddressLineTwo = ro.@params.walletUser.addressLine2;
                                    details.StateOrProvince = ro.@params.walletUser.state;
                                    details.City = ro.@params.walletUser.city;
                                    if (ro.@params.walletUser.country == "US")
                                         { details.Country = "USA"; }
                                    else
                                         { details.Country = ro.@params.walletUser.country; }
                                    details.ZipOrPostalCode = ro.@params.walletUser.zipcode;
                                    details.MemberSource = 22;
                                    details.GwObjVer = 1;
                                    details.GwLinked = 1;
                                    details.BaseBrandID = 0;
                                  //  
                                    
                                    details.SMSOptIn = false;
                                    member.Add(details);
                                  ////  //Send Welcome email//
                                  ////  //  Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member m1 = new Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member();
                                  ////  //  m1 = member;
                                  ////  Brierley.FrameWork.Data.DomainModel.Member m2 = new Brierley.FrameWork.Data.DomainModel.Member();
                                  ////  m2.FirstName = ro.@params.walletUser.firstName;
                                  ////  m2.LastName = ro.@params.walletUser.lastName;
                                  ////  m2.PrimaryEmailAddress = ro.@params.walletUser.email;
                                  ////  m2.PrimaryPhoneNumber = ro.@params.walletUser.phone;
                                  ////  m2.MemberCreateDate = System.DateTime.Now;
                                  ////  m2.BirthDate = Convert.ToDateTime("1/1/1990"); //default DOB to 1/1/1990 as value is not passed from App
                                  //////  m2.AddChildAttributeSet((Brierley.FrameWork.Data.IClientDataObject)details);
                                  ////  Dictionary<string, string> additionalFields = new Dictionary<string, string>();
                                  ////  additionalFields.Add("firstname", ro.@params.walletUser.firstName);
                                  ////  if (!String.IsNullOrEmpty(ro.@params.walletUser.email))
                                  ////  {
                                  ////      //  Convert.ChangeType(m1, typeof(Brierley.FrameWork.Data.DomainModel.Member));
                                  ////      //  Brierley.FrameWork.Data.DomainModel.Member m2 = (Brierley.FrameWork.Data.DomainModel.Member)m1;
                                  ////      AEEmail.SendEmail(m2, EmailType.EnrollActivate, additionalFields, ro.@params.walletUser.email);
                                  ////  }

                                   Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member returnMember = svcMgr.AddMember(member);   //Member added to Db, now response
                                   strLoyaltyNumber = returnMember.GetLoyaltyCardByType(Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member.VirtualCardRetrieveType.PrimaryCard).LoyaltyIdNumber;
                                   strIPcode = (int)returnMember.GetLoyaltyCardByType(Brierley.LoyaltyWare.ClientLib.DomainModel.Framework.Member.VirtualCardRetrieveType.PrimaryCard).IpCode;
                                    string strTotalPoints = svcMgr.GetAccountSummary(strLoyaltyNumber).CurrencyBalance.ToString();
                                    long strGWVersion = 1;
                                    //Google.Apis.Walletobjects.v1.Data.LoyaltyObject obj = Loyalty.generateLoyaltyObject(_classId, _objectId + strIPcode, ro.@params.walletUser.firstName + " " + ro.@params.walletUser.lastName, strLoyaltyNumber, strTotalPoints, strGWVersion);
                                    Google.Apis.Walletobjects.v1.Data.LoyaltyObject obj = Loyalty.generateLoyaltyObject(_classId, _objectId + strLoyaltyNumber, ro.@params.walletUser.firstName + " " + ro.@params.walletUser.lastName, strLoyaltyNumber, strTotalPoints, strGWVersion);
                                    IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                                    objects.Add(obj);
                                    Utils utils = new Utils(_seracct, objects, certificate);
                                    JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Thanks for signing up! Your member ID is: " + strLoyaltyNumber, Result = "approved" };
                                    return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                                    }
                                    else
                                    {
                                     IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();   
                                    Utils utils = new Utils(_seracct, objects, certificate);
                                    ResponseCode EnumVal =   (ResponseCode)ErrorCode ;
                                    JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Sorry we can't complete this signup - " + Definitions.GetResponseMessage(EnumVal), Result = "rejected" };
                                    return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                                    }
                                     #endregion
                                }              
                                else //if (aevalidate.Msg.Substring(0, 7) == "Invalid")
                                {  
                                    #region: Invalidinput
                                    IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();   
                                    Utils utils = new Utils(_seracct, objects, certificate);
                                    ResponseCode EnumVal = (ResponseCode)Enum.Parse(typeof(ResponseCode), aevalidate.Msg);
                                   JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Sorry we can't complete this Signup - " + Definitions.GetResponseMessage(EnumVal), Result = "rejected" };
                                    return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                                     #endregion
                                }
                               
                            }
                            else // (aevalidate.LoyaltyNumber != string.Empty) loyalty number found but incorret member input
                            {
                                IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                                Utils utils = new Utils(_seracct, objects, certificate);
                                JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Account exists. Please link using your AEREWARDS number.", Result = "rejected" };
                                return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                            }

                        }
                        #endregion
                        else
                        {
                            IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                            Utils utils = new Utils(_seracct, objects, certificate);
                            JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Sorry we can't complete this activity", Result = "rejected" };
                            return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                        }

                    }
                    catch (Exception ex)
                    {
                        IList<Google.Apis.Walletobjects.v1.Data.LoyaltyObject> objects = new List<Google.Apis.Walletobjects.v1.Data.LoyaltyObject>();
                        Utils utils = new Utils(_seracct, objects, certificate);
                      //  string[] words = ex.Message.Split(new string[] { "Message" }, StringSplitOptions.RemoveEmptyEntries);
                      //  string errormsg = words[0];// ex.Message.Substring(0, 21);
                        JsonWebToken.Payload.WebserviceResponse response = new JsonWebToken.Payload.WebserviceResponse() { Message = "Sorry we can't complete this activity", Result = "rejected" };
                        _info = "Error has occured : " + ex.Message;
                        logger.Error(_info);
                        _info = "Error has occured : " + ex.InnerException;
                        logger.Error(_info);
                        return new MemoryStream(Encoding.UTF8.GetBytes(utils.GenerateWsJwt(response)));
                    }
                    finally
                    {
                        _info = "Wallet Batch Process has completed ";
                        logger.Info(_info);

                    }
                    
                
        }
            
    }
}
