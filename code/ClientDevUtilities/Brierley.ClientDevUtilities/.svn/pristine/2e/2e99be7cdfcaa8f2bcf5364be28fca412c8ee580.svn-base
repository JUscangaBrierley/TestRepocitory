//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.LoyaltyWare.LWIntegrationSvc;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling
{
    public class MemberResponseHelperUtil
    {
        #region Fields
        private const string _className = "ResponseHelperUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private static Dictionary<string, IMemberResponseHelper> _intMap = new Dictionary<string, IMemberResponseHelper>();
        #endregion

        #region Helpers

        #region Interceptor Helpers
        private static IMemberResponseHelper LookupInterceptor(string key)
        {
            lock (_intMap)
            {
                return _intMap.ContainsKey(key) ? _intMap[key] : null;
            }
        }

        private static void AddInterceptor(string key, IMemberResponseHelper interceptor)
        {
            lock (_intMap)
            {
                _intMap.Add(key, interceptor);
            }
        }

        public static IMemberResponseHelper GetMemberResponseHelper(string opName, LWIntegrationConfig.InterceptorDirective directive)
        {
            string methodName = "GetMemberResponseHelper";

            IMemberResponseHelper interceptor = null;
            if (directive == null)
            {
                _logger.Debug(_className, methodName, "No member response helper directive provided.");
                return null;
            }
            
            if (string.IsNullOrEmpty(directive.InterceptorAssemlyName))
            {
                _logger.Debug(_className, methodName, "No member response helper assembly name provided.");
                return null;
            }
            if (string.IsNullOrEmpty(directive.InterceptorTypeName))
            {
                _logger.Debug(_className, methodName, "No member response helper type provided.");
                return null;
            }
            try
            {               
                if (directive.ReuseForFile )
                {
                    interceptor = LookupInterceptor(opName);
                    if (interceptor != null)
                    {
                        _logger.Debug(_className, methodName, "Reusing cached member response helper.");
                        return interceptor;
                    }
                }
                _logger.Debug(_className, methodName, "Creating instance of member response helper " + directive.InterceptorTypeName);
                interceptor = (IMemberResponseHelper)ClassLoaderUtil.CreateInstance(directive.InterceptorAssemlyName, directive.InterceptorTypeName);
                if (interceptor != null)
                {
                    _logger.Debug(_className, methodName, "Initializing member response helper " + directive.InterceptorTypeName);
                    interceptor.Initialize(directive.InterceptorParms);
                }
                else
                {
                    _logger.Error(_className, methodName, "Unable to load member response helper.");
                    throw new LWIntegrationException("Unable to load member response helper") { ErrorCode = 9995 };
                }
                if (directive.ReuseForFile)
                {
                    AddInterceptor(opName, interceptor);
                }                
            }
            catch (LWIntegrationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error loading member response helper,", ex);
                Console.Out.WriteLine("Error loading member response helper: " + ex.Message);
                throw new LWIntegrationException("Error loading member response helper", ex) { ErrorCode = 9995 };
            }
            return interceptor;
        }
        #endregion

        #region Attribute Set Helpers
        private static IAttributeSetContainer LoadAttributeSet(IAttributeSetContainer thisContainer, string attributeSetToLoad)
        {
            string[] attributeSets = attributeSetToLoad.Split('/');
            string attributeSetName = attributeSets[0];
            if (attributeSetName == "VirtualCard")
            {
                throw new LWIntegrationException(
                    string.Format("Invalid LoadAttributeSet directive {0} found.", attributeSetToLoad)) { ErrorCode = 3000 };
            }
            //AttributeSetMetaData asDef = ds.GetAttributeSetMetaData(attributeSetName);
            if (!thisContainer.IsLoaded(attributeSetName))
            {
                using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
                {
                    service.LoadAttributeSetList(thisContainer, attributeSetName, false);
                }
            }
            IList<IClientDataObject> aSet = thisContainer.GetChildAttributeSets(attributeSetName);
            foreach (IClientDataObject row in aSet)
            {
                // now call recursively to process the next
                if (attributeSets.Length > 1)
                {
                    string attLoadStr = attributeSetToLoad.Substring(attributeSetToLoad.IndexOf("/") + 1);
                    LoadAttributeSet(row, attLoadStr);
                }
            }
            return thisContainer;
        }

        private static Member LoadMemberAttributeSets(Member member, IList<string> attributeSetsToLoad/*, int index*/)
        {
            bool virtualCardPresent = false;
            foreach (string attributeSetToLoad in attributeSetsToLoad)
            {
                if (!attributeSetToLoad.StartsWith("VirtualCard"))
                {
                    member = (Member)LoadAttributeSet(member, attributeSetToLoad);
                }
                else
                {
                    virtualCardPresent = true;
                }
            }
            // now process virtual cards
            if (virtualCardPresent)
            {
                foreach (string attributeSetToLoad in attributeSetsToLoad)
                {
                    if (attributeSetToLoad.StartsWith("VirtualCard"))
                    {
                        foreach (VirtualCard vc in member.LoyaltyCards)
                        {
                            // strip off the VirtualCard part.
                            int idx = attributeSetToLoad.IndexOf("VirtualCard/");
                            if (idx == 0)
                            {
                                string astl = attributeSetToLoad.Substring(("VirtualCard/").Length);
                                LoadAttributeSet(vc, astl);
                            }
                        }
                    }
                }
            }
            return member;
        }
        #endregion

        #endregion

        public static Member LoadMemberAttributeSets(LWIntegrationDirectives.OperationDirective opDirective, Member member)
        {
            string methodName = "LoadMemberAttributeSets";

            if (opDirective.ResponseDirective != null)
            {
                if (opDirective.ResponseDirective.ReloadMember && member.IpCode > 0)
                {
                    _logger.Trace(_className, methodName, "Reloading member with ipcode " + member.IpCode);
                    long ipcode = member.IpCode;                    
                    // we do not want to loose any transient properties that the member has
                    ICollection propNames = member.GetTransientPropertyNames();
                    Hashtable props = new Hashtable();
                    foreach (string propName in propNames)
                    {
                        props.Add(propName, member.GetTransientProperty(propName));
                    }
                    member = null;
                    using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
                    {
                        member = service.LoadMemberFromIPCode(ipcode);
                    }
                    if (props.Count > 0)
                    {
                        foreach (DictionaryEntry entry in props)
                        {
                            member.UpdateTransientProperty((string)entry.Key, props[entry.Key]);
                        }
                    }
                }
                IList<string> attributeSetsToLoad = opDirective.ResponseDirective.AttributeSetsToLoad;
                if (attributeSetsToLoad != null && attributeSetsToLoad.Count > 0)
                {                                        
                    // load the requested attribute sets                        
                    member = (Member)LoadMemberAttributeSets(member, attributeSetsToLoad/*, 0*/);
                    // If the member response helper is defined then process it.
                    IMemberResponseHelper helper = null;
                    if (opDirective.ResponseDirective.ResponseHelperDirective != null)
                    {
                        helper = GetMemberResponseHelper(opDirective.Name, opDirective.ResponseDirective.ResponseHelperDirective);
                    }
                    if (helper != null)
                    {
                        try
                        {
                            _logger.Debug(_className, methodName, "Invoking ProcessMemberAfterAttributeSetLoad method on response helper.");
                            member = helper.ProcessMemberAfterAttributeSetLoad(opDirective.Name, opDirective.ResponseDirective, member);
                            _logger.Debug(_className, methodName, "Done invoking ProcessMemberAfterAttributeSetLoad method on response helper.");
                        }
                        catch (LWException ex)
                        {
                            _logger.Error(_className, methodName, "Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(_className, methodName, "Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex);
                            throw new LWException("Error invoking ProcessMemberBeforeAttributeSetLoad method of response helper.", ex) { ErrorCode = 3111 }; 
                        }
                    }
                    else
                    {
                        _logger.Debug(_className, methodName, "No member response helper available to invoke for " + opDirective.Name);
                    }
                }                
            }
            return member;
        }
    }
}
