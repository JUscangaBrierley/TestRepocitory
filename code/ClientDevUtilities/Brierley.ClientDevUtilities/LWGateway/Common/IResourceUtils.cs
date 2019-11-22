using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Brierley.ClientDevUtilities.LWGateway.Common
{
    public interface IResourceUtils
    {
        List<ListItem> FillWithISO4217CurrencySymbols(List<ListItem> vals);
        string GetGlobalWebResource(string classKey, string resourceKey);
        string GetLocalWebResource(string virtualPath, string resourceKey);
        string GetLocalWebResource(string virtualPath, string resourceKey, string defaultValue);
        string GetManifestResourceString(string name);
        string GetManifestResourceString(string name, Assembly assembly);
        ResourceManager GetResourceManager(string resourceName, Assembly assembly);
        ResourceManager GetResourceManagerFromFile(string resourceFilePath, string resourceName);
        void LoadResourceClientScript(string name, ClientScriptManager clientScriptManager, Type type, string key, bool addScriptTags);
    }
}
