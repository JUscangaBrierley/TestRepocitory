//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.CampaignManagement.DataProvider
{
    public static class CampaignDataProviderUtil
    {
        public static CampaignDataProvider GetInstance(SupportedDataSourceType databaseType, string connectionString, string dataSchema, bool indexTempTables, bool useArrayBinding)
        {
            if (databaseType != SupportedDataSourceType.MsSQL2005 && databaseType != SupportedDataSourceType.Oracle10g)
            {
                throw new ArgumentException(string.Format("Unknown or unsupported database type {0} provided.", databaseType));
            }

            CampaignDataProvider provider = new CampaignDataProvider(databaseType, connectionString);

            provider.DataSchema = dataSchema;
            provider.IndexTempTables = indexTempTables;
            provider.UseArrayBinding = useArrayBinding;
            return provider;
        }
    }
}
