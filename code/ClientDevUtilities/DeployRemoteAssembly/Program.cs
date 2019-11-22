using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace DeployRemoteAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception("Invalid number of commmand line parameters. Expected three (OrganizationName, EnvironmentName, AssemblyPath.)");
            }

            string orgName = args[0];
            string envName = args[1];
            string argumentPath = args[2];
            var fileInfo = new FileInfo(argumentPath);

            if (!fileInfo.Exists)
            {
                throw new Exception(string.Format("The following file path does not exist: {0}", fileInfo.FullName));
            }

            // the following gets the correctly cased filename.
            string fullPath = fileInfo.Directory.GetFileSystemInfos(fileInfo.Name)[0].FullName;

            AssemblyName name = AssemblyName.GetAssemblyName(fullPath);
            RemoteAssembly remoteAssembly = new RemoteAssembly();
            remoteAssembly.AssemblyName = name.FullName;
            remoteAssembly.AssemblyFileName = Path.GetFileName(fullPath);
            remoteAssembly.Assembly = IOUtils.BytesFromFile(fullPath);

            using (var service = LWDataServiceUtil.DataServiceInstance(orgName, envName))
            {
                service.SaveRemoteAssembly(remoteAssembly);
            }
        }
    }
}
