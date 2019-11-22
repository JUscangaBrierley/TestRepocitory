using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Brierley.ClientDevUtilities.LWGateway.Common
{
    public class ResourceUtils : IResourceUtils
    {
        public static ResourceUtils Instance { get; private set; }

        static ResourceUtils()
        {
            Instance = new ResourceUtils();
        }

        public List<ListItem> FillWithISO4217CurrencySymbols(List<ListItem> vals)
        {
            return Brierley.FrameWork.Common.ResourceUtils.FillWithISO4217CurrencySymbols(vals);
        }

        public string GetGlobalWebResource(string classKey, string resourceKey)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetGlobalWebResource(classKey, resourceKey);
        }

        public string GetLocalWebResource(string virtualPath, string resourceKey)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetLocalWebResource(virtualPath, resourceKey);
        }

        public string GetLocalWebResource(string virtualPath, string resourceKey, string defaultValue)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetLocalWebResource(virtualPath, resourceKey, defaultValue);
        }

        public string GetManifestResourceString(string name)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetManifestResourceString(name);
        }

        public string GetManifestResourceString(string name, Assembly assembly)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetManifestResourceString(name, assembly);
        }

        public ResourceManager GetResourceManager(string resourceName, Assembly assembly)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetResourceManager(resourceName, assembly);
        }

        public ResourceManager GetResourceManagerFromFile(string resourceFilePath, string resourceName)
        {
            return Brierley.FrameWork.Common.ResourceUtils.GetResourceManagerFromFile(resourceFilePath, resourceName);
        }

        public void LoadResourceClientScript(string name, ClientScriptManager clientScriptManager, Type type, string key, bool addScriptTags)
        {
            Brierley.FrameWork.Common.ResourceUtils.LoadResourceClientScript(name, clientScriptManager, type, key, addScriptTags);
        }
    }
}
