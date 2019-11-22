using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using PetaPoco;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class DataServiceUtil : IDataServiceUtil
    {
        public static DataServiceUtil Instance { get; private set; }

        static DataServiceUtil()
        {
            Instance = new DataServiceUtil();
        }

        public void ExecuteRawSqlCommand(Database database, string cmdline)
        {
            Brierley.FrameWork.Data.DataServiceUtil.ExecuteRawSqlCommand(database, cmdline);
        }

        public void ExecuteRawSqlCommand(ServiceConfig config, string cmdline)
        {
            Brierley.FrameWork.Data.DataServiceUtil.ExecuteRawSqlCommand(config, cmdline);
        }

        public Type GetClientDataObjectType(string attributeSetName)
        {
            return Brierley.FrameWork.Data.DataServiceUtil.GetClientDataObjectType(attributeSetName);
        }

        public string GetKey(string org, string env)
        {
            return Brierley.FrameWork.Data.DataServiceUtil.GetKey(org, env);
        }

        public List<string> GetKnownModelNamespaces()
        {
            return Brierley.FrameWork.Data.DataServiceUtil.GetKnownModelNamespaces();
        }

        public IClientDataObject GetNewClientDataObject(string attSetName)
        {
            return Brierley.FrameWork.Data.DataServiceUtil.GetNewClientDataObject(attSetName);
        }

        public void InitializeTargetDatabase(LWConfiguration lwConfig)
        {
            Brierley.FrameWork.Data.DataServiceUtil.InitializeTargetDatabase(lwConfig);
        }

        public void PurgeLoadedPetaPocoTypes()
        {
            Brierley.FrameWork.Data.DataServiceUtil.PurgeLoadedPetaPocoTypes();
        }

        public string RunDBScript(string scriptName, bool skipSemiColon = true)
        {
            return Brierley.FrameWork.Data.DataServiceUtil.RunDBScript(scriptName, skipSemiColon);
        }

        public void SetupStandardAttributeSets(string orgName, string envName)
        {
            Brierley.FrameWork.Data.DataServiceUtil.SetupStandardAttributeSets(orgName, envName);
        }

        public void SetupStandardProgram(string orgName, string envName, bool hasAttributeSets)
        {
            Brierley.FrameWork.Data.DataServiceUtil.SetupStandardProgram(orgName, envName, hasAttributeSets);
        }
    }
}
