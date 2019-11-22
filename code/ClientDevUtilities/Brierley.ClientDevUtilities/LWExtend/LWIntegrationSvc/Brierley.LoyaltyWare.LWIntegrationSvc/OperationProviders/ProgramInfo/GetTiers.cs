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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.ProgramInfo
{
    public class GetTiers : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetTiers";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetTiers() : base("GetTiers") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;

                IList<TierDef> tiers = LoyaltyDataService.GetAllTierDefs();
                if (tiers == null || tiers.Count == 0)
                {
                    throw new LWOperationInvocationException("No tiers found.") { ErrorCode = 3362 };
                }                                
                APIArguments resultParams = new APIArguments();
                APIStruct[] pes = new APIStruct[tiers.Count];
                int idx = 0;
                foreach (TierDef tier in tiers)
                {
                    APIArguments tparms = new APIArguments();
                    tparms.Add("Name", tier.Name);
                    if (!string.IsNullOrEmpty(tier.DisplayText))
                    {
                        tparms.Add("DisplayText", tier.DisplayText);
                    }
                    if (!string.IsNullOrEmpty(tier.Description))
                    {
                        tparms.Add("Description", tier.Description);
                    }
                    tparms.Add("EntryPoints", tier.EntryPoints);
                    tparms.Add("ExitPoints", tier.ExitPoints);
                    if (!string.IsNullOrEmpty(tier.PointTypeNames))
                    {
                        tparms.Add("PointTypeNames", tier.PointTypeNames);
                    }
                    if (!string.IsNullOrEmpty(tier.PointEventNames))
                    {
                        tparms.Add("PointEventNames", tier.PointEventNames);
                    }
                    if(tier.Attributes.Count > 0)
                    {
                        APIStruct[] atts = new APIStruct[tier.Attributes.Count];
                        int index = 0;
                        foreach (ContentAttribute ra in tier.Attributes)
                        {
                            ContentAttributeDef def = ContentService.GetContentAttributeDef(ra.ContentAttributeDefId);
                            APIArguments attparms = new APIArguments();
                            attparms.Add("AttributeName", def.Name);
                            attparms.Add("AttributeValue", ra.Value);
                            APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                            atts[index++] = v;
                        }
                        tparms.Add("ContentAttributes", atts);
                    }
                    APIStruct pevent = new APIStruct() { Name = "TierDef", IsRequired = false, Parms = tparms };
                    pes[idx++] = pevent;
                }
                resultParams.Add("TierDef", pes);
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                return response;
            }
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
