using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;

namespace Brierley.FrameWork.Common.Globalization
{
	public class DBResourceProviderFactory : ResourceProviderFactory
	{
		public override IResourceProvider CreateGlobalResourceProvider(string classKey)
		{
			if (classKey == "GlobalResources")
			{
				return new DBResourceProvider(classKey);
			}
			else
			{
				IResourceProvider resxProvider;
				string typeName = "System.Web.Compilation.ResXResourceProviderFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				ResourceProviderFactory factory = (ResourceProviderFactory)Activator.CreateInstance(Type.GetType(typeName));
				resxProvider = factory.CreateGlobalResourceProvider(classKey);
				return resxProvider;
			}
		}

		public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
		{
			IResourceProvider resxProvider;
			string typeName = "System.Web.Compilation.ResXResourceProviderFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
			ResourceProviderFactory factory = (ResourceProviderFactory)Activator.CreateInstance(Type.GetType(typeName));
			resxProvider = factory.CreateLocalResourceProvider(virtualPath);
			return resxProvider;

			//string classKey = virtualPath;
			//if (!string.IsNullOrEmpty(virtualPath))
			//{
			//	virtualPath = virtualPath.Remove(0, 1);
			//	classKey = virtualPath.Remove(0, virtualPath.IndexOf('/') + 1);
			//}
			//return new DBResourceProvider(classKey);
		}
	}
}
