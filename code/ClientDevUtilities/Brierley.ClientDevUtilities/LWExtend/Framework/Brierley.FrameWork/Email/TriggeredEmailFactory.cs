using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Email
{
	public static class TriggeredEmailFactory
	{
		public static ITriggeredEmail Create(string name, ICommunicationLogger logger = null)
		{
			ITriggeredEmail ret = GetTriggeredEmailInstance(logger);
			ret.Init(name);
			return ret;
		}

		public static ITriggeredEmail Create(long id, ICommunicationLogger logger = null)
		{
			ITriggeredEmail ret = GetTriggeredEmailInstance(logger);
			ret.Init(id);
			return ret;
		}

		private static ITriggeredEmail GetTriggeredEmailInstance(ICommunicationLogger logger = null)
		{
			ITriggeredEmail ret = null;
			var provider = LWConfigurationUtil.GetConfigurationValue("LWEmailProvider");
			switch (provider)
			{
				case "aws":
				case "AWS":
					ret = new AwsTriggeredEmail(logger);
					break;
				case "dmc":
				case "DMC":
					ret = new DmcTriggeredEmail(logger);
					break;
				case "custom":
					ret = GetCustomEmailInstance();
					break;
				case "":
				default:
					throw new Exception("Could not determine the email provider to use. Please ensure configuration value \"LWEmailProvider\" is set to a valid value (aws, dmc or custom).");
			}
			return ret;
		}

		private static ITriggeredEmail GetCustomEmailInstance()
		{
			var name = LWConfigurationUtil.GetConfigurationValue("LWEmailProviderAssembly");
			if (string.IsNullOrEmpty(name))
			{
				throw new Exception("LWEmailProvider has been set to custom, but the assembly name has not been provided in LWEmailProviderAssembly");
			}
			var assembly = ClassLoaderUtil.LoadAssembly(name);
			if (assembly == null)
			{
				throw new Exception(string.Format("Email provider assembly {0} not found ", name));
			}
			string typeName = LWConfigurationUtil.GetConfigurationValue("LWEmailProviderType");
			if (string.IsNullOrEmpty(typeName))
			{
				throw new Exception("LWEmailProvider has been set to custom, but the type name has not been provided in LWEmailProviderType");
			}
			ITriggeredEmail ret = (ITriggeredEmail)ClassLoaderUtil.CreateInstance(assembly, typeName);
			if (ret == null)
			{
				throw new Exception(string.Format("Failed to load triggered email type {0}", typeName));
			}
			return ret;
		}
	}
}
