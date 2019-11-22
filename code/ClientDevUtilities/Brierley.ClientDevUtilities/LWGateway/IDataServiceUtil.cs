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
    public interface IDataServiceUtil
    {
        void ExecuteRawSqlCommand(Database database, string cmdline);
        void ExecuteRawSqlCommand(ServiceConfig config, string cmdline);
        Type GetClientDataObjectType(string attributeSetName);
        string GetKey(string org, string env);
        List<string> GetKnownModelNamespaces();
        IClientDataObject GetNewClientDataObject(string attSetName);
        void InitializeTargetDatabase(LWConfiguration lwConfig);
        void PurgeLoadedPetaPocoTypes();
        string RunDBScript(string scriptName, bool skipSemiColon = true);
        void SetupStandardAttributeSets(string orgName, string envName);
        void SetupStandardProgram(string orgName, string envName, bool hasAttributeSets);
    }
}
