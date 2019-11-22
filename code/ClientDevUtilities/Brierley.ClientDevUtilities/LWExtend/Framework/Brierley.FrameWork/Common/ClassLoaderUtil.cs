//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common
{
    public static class ClassLoaderUtil
    {

        #region Fields

        private const string _className = "ClassLoaderUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private static Dictionary<string, Assembly> assemblyMap = new Dictionary<string, Assembly>();

        private static string[] ignoreList =
        {
            "Microsoft.Practices.EnterpriseLibrary.Validation",
            "Brierley.WebFrameWork.XmlSerializers",
            "Telerik.Web.UI.Skins",
            "Microsoft.Practices.EnterpriseLibrary.Data",
        };

        #endregion

        public delegate Assembly AssemblyLoadingEventHandler(string assemblyName);
        public static event AssemblyLoadingEventHandler OnAssemblyLoading;

        #region Helpers


        public static string GetCustomAssemblyPath()
        {
            return GetCustomAssemblyPath(false);
        }

        public static string GetCustomAssemblyPath(bool create)
        {
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            return GetCustomAssemblyPath(ctx.Organization, ctx.Environment, create);
        }

        public static string GetCustomAssemblyPath(string org, string env)
        {
            return GetCustomAssemblyPath(org, env, false);
        }

        public static string GetCustomAssemblyPath(string org, string env, bool create)
        {
            const string defaultPath = "LWCustomAssemblyRoot";
            string path = System.Configuration.ConfigurationManager.AppSettings["LWCustomAssemblyRoot"];
            if (string.IsNullOrEmpty(path))
            {
                path = System.Configuration.ConfigurationManager.AppSettings["LWAssemblyPath"];
                if (!string.IsNullOrEmpty(path) && path.Contains(";"))
                {
                    path = path.Split(';')[0];
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                if (System.Web.HttpContext.Current != null)
                {
                    path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        path += Path.DirectorySeparatorChar;
                    }
                    path += defaultPath;
                }
                else
                {
                    path = defaultPath;
                }
            }

            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }
            path = Path.Combine(path, string.Format("{0}_{1}", org, env));

            if (create && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private static Assembly ResolveAssemblyFromPath(string path, string aName, string[] asmFileNameTokens)
        {
            string methodName = "ResolveAssemblyFromPath";
            Assembly assembly = null;
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                string fileToLoad = string.Empty;
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file);
                    if (!string.IsNullOrEmpty(ext) && ext.ToLower() == ".dll")
                    {
                        string assemblyName = Path.GetFileNameWithoutExtension(file);
                        if (assemblyName.ToLower() == asmFileNameTokens[0].Trim().ToLower())
                        {
                            // found the assembly.
                            AssemblyName fileAssemblyName = AssemblyName.GetAssemblyName(file);
                            if (fileAssemblyName.Name.ToLower() == asmFileNameTokens[0].Trim().ToLower())
                            {
                                // now compare version numbers.
                                if (asmFileNameTokens.Length == 1)
                                {
                                    // no version available.
                                    fileToLoad = file;
                                    _logger.Trace(_className, methodName, string.Format("{0} has been resolved from {1}.  Version number not considered.", asmFileNameTokens[0], path));
                                    break;
                                }
                                else
                                {
                                    string vstr = asmFileNameTokens[1].Trim().Substring(8);
                                    string[] versions = vstr.Split('.');
                                    if (fileAssemblyName.Version.Major == int.Parse(versions[0]) &&
                                        int.Parse(versions[1]) <= fileAssemblyName.Version.Minor)
                                    {
                                        fileToLoad = file;
                                        _logger.Trace(_className, methodName, string.Format("{0} has been resolved from {1}", aName, path));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(fileToLoad))
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(fileToLoad);
                        _logger.Trace(_className, methodName,
                            string.Format("Assembly file {0} has been successfully loaded", fileToLoad));
                        assemblyMap.Add(asmFileNameTokens[0], assembly);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName,
                            string.Format("Unable to load assembly {0}.  ", fileToLoad, ex.Message), ex);
                        throw;
                    }
                }
            }
            else
            {
                _logger.Error(_className, methodName, string.Format("{0} provided in the search path is not a valid directory.", path));
            }
            return assembly;
        }

        private static Assembly ResolveAssemblyFromCustomRoot(string aName, string[] asmFileNameTokens)
        {
            string methodName = "ResolveAssemblyFromCustomRoot";

            Assembly assembly = null;

            string searchRoot = System.Configuration.ConfigurationManager.AppSettings["LWCustomAssemblyRoot"];
            if (string.IsNullOrEmpty(searchRoot))
            {
                _logger.Debug(_className, methodName, "No custom root assembly path defined.");
                return null;
            }
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            if (ctx == null)
            {
                _logger.Debug(_className, methodName, "No current environment context found.");
                return null;
            }
            string searchPath = string.Format("{0}{1}_{2}", Brierley.FrameWork.Common.IO.IOUtils.AppendSeparatorToFolderPath(searchRoot), ctx.Organization, ctx.Environment);
            _logger.Debug(_className, methodName, string.Format("Search for {0} in path {1}", aName, searchPath));
            if (!string.IsNullOrEmpty(searchPath))
            {
                assembly = ResolveAssemblyFromPath(searchPath, aName, asmFileNameTokens);
            }

            return assembly;
        }
        #endregion



        #region Assembly Management


        public static Assembly LoadAssemblyFromName(string assemblyName, bool useFullName)
        {
            string methodName = "LoadAssemblyFromName";
            Assembly assembly = null;

            _logger.Debug(_className, methodName,
                string.Format("Loading assembly {0}.  Use full name option = {1}.", assemblyName, useFullName.ToString()));

            if (!string.IsNullOrEmpty(assemblyName))
            {
                if (OnAssemblyLoading != null)
                {
                    assembly = OnAssemblyLoading(assemblyName);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }

                AppDomain domain = System.AppDomain.CurrentDomain;
                try
                {
                    Assembly[] loaded = domain.GetAssemblies();
                    foreach (Assembly ass in loaded)
                    {
                        string name = useFullName ? ass.GetName().FullName : ass.GetName().Name;
                        if (name.Equals(assemblyName) == true)
                        {
                            assembly = ass;
                            break;
                        }
                    }
                    if (assembly == null)
                    {
                        assembly = Assembly.Load(assemblyName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, "LoadAssembly", "Ignoring exception", ex);
                }
            }
            return assembly;
        }


        public static Assembly LoadAssembly(string assemblyPath)
        {
            Assembly assembly = null;
            if (OnAssemblyLoading != null)
            {
                assembly = OnAssemblyLoading(assemblyPath);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                string name = assemblyPath;
                string ext = Path.GetExtension(assemblyPath);
                if (!string.IsNullOrEmpty(ext) && ext == ".dll")
                {
                    name = Path.GetFileNameWithoutExtension(assemblyPath);
                }
                AppDomain domain = System.AppDomain.CurrentDomain;
                try
                {
                    Assembly[] loaded = domain.GetAssemblies();
                    foreach (Assembly ass in loaded)
                    {
                        if (ass.GetName().Name.Equals(name) == true)
                        {
                            assembly = ass;
                            break;
                        }
                    }
                    if (assembly == null)
                    {
                        // now try to load it.
                        //assembly = Assembly.Load(assemblyPath);
                        assembly = Assembly.Load(name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, "LoadAssembly", "Ignoring exception", ex);
                }
            }
            return assembly;
        }


        public static Assembly LoadAssembly(string assemblyName, byte[] rawAssembly)
        {
            Assembly assembly = null;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                AppDomain domain = System.AppDomain.CurrentDomain;
                try
                {
                    Assembly[] loaded = domain.GetAssemblies();
                    foreach (Assembly ass in loaded)
                    {
                        if (ass.GetName().Name.Equals(assemblyName) == true)
                        {
                            assembly = ass;
                            break;
                        }
                    }
                    if (assembly == null)
                    {
                        // now try to load it.
                        assembly = Assembly.Load(rawAssembly);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, "LoadAssembly", "Ignoring exception", ex);
                }
            }
            else
            {
                // just load the assembly.
                assembly = Assembly.Load(rawAssembly);
            }

            if (!string.IsNullOrEmpty(assemblyName) && assembly != null)
            {
                lock (assemblyMap)
                {
                    if (assemblyMap.ContainsKey(assemblyName))
                        assemblyMap[assemblyName] = assembly;
                    else
                        assemblyMap.Add(assemblyName, assembly);
                }
            }

            return assembly;
        }


        public static Assembly LoadAssembly(byte[] rawAssembly)
        {
            Assembly assembly = null;
            if (rawAssembly != null && rawAssembly.Length > 0)
            {
                assembly = Assembly.Load(rawAssembly);
            }
            return assembly;
        }


        public static string GetAssemblyName(string fullName)
        {
            string[] asmFileNameTokens = fullName.Split(',');
            return asmFileNameTokens[0];
        }


        private static bool CompareNames(AssemblyName assemblyName, string[] asmFileNameTokens)
        {
            bool sameName = false;
            if (assemblyName.Name == asmFileNameTokens[0].Trim())
            {
                // now compare version numbers.
                if (asmFileNameTokens.Length == 1)
                {
                    // no version available.
                    sameName = true;
                }
                else
                {
                    string vstr = asmFileNameTokens[1].Trim().Substring(8);
                    string[] versions = vstr.Split('.');
                    if (assemblyName.Version.Major == int.Parse(versions[0]) &&
                        int.Parse(versions[1]) <= assemblyName.Version.Minor)
                    {
                        sameName = true;
                    }
                }
            }
            return sameName;
        }


        public static Assembly MyAssemblyResolver(object sender, ResolveEventArgs args)
        {
            string methodName = "MyAssemblyResolver";

            if (AssemblyNameIsSatelliteAssembly(args.Name))
            {
                _logger.Debug(_className, methodName, string.Format("assembly {0} is satellite, ignoring", args.Name));
                return null;
            }

            Assembly assembly = null;
            lock (assemblyMap)
            {
                string[] asmFileNameTokens = args.Name.Split(',');

                if (InIgnoreList(asmFileNameTokens[0]))
                {
                    _logger.Debug(_className, methodName, string.Format("assembly {0} is in ignore list. Ignoring", args.Name));
                    return null;
                }

                if (asmFileNameTokens[0].ToLower().EndsWith(".dll"))
                {
                    asmFileNameTokens[0] = asmFileNameTokens[0].Substring(0, asmFileNameTokens[0].Length - 4);
                }

                if (assemblyMap.ContainsKey(asmFileNameTokens[0]))
                {
                    assembly = assemblyMap[asmFileNameTokens[0]];
                }
                else
                {
                    // first look for search path in the configuration file.
                    string searchPath = System.Configuration.ConfigurationManager.AppSettings["LWAssemblyPath"];
                    if (string.IsNullOrEmpty(searchPath))
                    {
                        // look for search path in the environment.
                        searchPath = System.Environment.GetEnvironmentVariable("LWAssemblyPath");
                    }
                    _logger.Debug(_className, methodName, string.Format("Search for {0} in path {1}", args.Name, searchPath));
                    if (!string.IsNullOrEmpty(searchPath))
                    {
                        string[] pathList = searchPath.Split(';');
                        foreach (string path in pathList)
                        {
                            assembly = ResolveAssemblyFromPath(path, args.Name, asmFileNameTokens);
                            if (assembly != null)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        _logger.Warning(string.Format("Could not get a search path to locally resolve {0}", args.Name));
                    }

                    if (assembly == null)
                    {
                        // make one last ditch attempt to find it in the already loaded assemblies
                        Assembly[] loaded = System.AppDomain.CurrentDomain.GetAssemblies();
                        foreach (Assembly ass in loaded)
                        {
                            if (CompareNames(ass.GetName(), asmFileNameTokens))
                            {
                                return ass;
                            }
                        }
                        _logger.Error(_className, methodName, "Unable to resolve assembly: " + args.Name);
                    }

                    if (assembly == null)
                    {
                        // try to search through custom root assembly
                        assembly = ResolveAssemblyFromCustomRoot(args.Name, asmFileNameTokens);
                    }
                }
            }
            return assembly;
        }


        public static bool AssemblyNameIsSatelliteAssembly(string assemblyName)
        {
            int comma = assemblyName.IndexOf(',');
            if (comma > 10 && string.Equals(assemblyName.Substring(comma - 10, 10), ".resources"))
            {
                return true;
            }
            return false;
        }

        public static bool InIgnoreList(string assemblyName)
        {
            return (from x in ignoreList where x == assemblyName select 1).Count() > 0;
        }

        #endregion


        #region Instance Management


        public static object CreateInstance(string assemblyName, string typeName, Type[] argTypes, object[] args)
        {
            Assembly assembly = null;
            if (OnAssemblyLoading != null)
            {
                assembly = OnAssemblyLoading(assemblyName);
            }

            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
            }

            if (assembly != null)
            {
                return CreateInstance(assembly, typeName, argTypes, args);
            }
            else
            {
                throw new ArgumentException(string.Format("Unable to load assembly {0}.", assemblyName));
            }
        }


        public static object CreateInstance(Assembly assembly, string typeName, Type[] argTypes, object[] args)
        {
            Type type = assembly.GetType(typeName);
            if (type == null)
                throw new ArgumentException("Could not resolve type '" + typeName + "'");

            ConstructorInfo cInfo = type.GetConstructor(argTypes);
            if (cInfo == null)
            {
                throw new ArgumentException("Unable to get constructor information through reflection.");
            }
            return cInfo.Invoke(args);
        }


        public static object CreateInstance(string assemblyName, string typeName)
        {
            object instance = null;
            Assembly assembly = null;
            if (OnAssemblyLoading != null)
            {
                assembly = OnAssemblyLoading(assemblyName);
            }

            if (assembly == null)
            {
                if (assemblyName.ToLower().EndsWith(".dll"))
                {
                    assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
                }
                assembly = Assembly.Load(assemblyName);
            }
            if (assembly != null)
            {
                instance = CreateInstance(assembly, typeName);
            }
            return instance;
        }


        public static object CreateInstance(Assembly assembly, string typeName)
        {
            object obj = null;
            try
            {
                obj = assembly.CreateInstance(typeName);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, "CreateInstance", "Ignoring exception", ex);
            }
            return obj;
        }

        #endregion


        public static object InvokeMethod(object instance, string methodName, object[] parms)
        {
            object result = null;
            Type[] argTypes = null;
            MethodInfo mi = null;
            if (parms != null && parms.Length > 0)
            {
                argTypes = new Type[parms.Length];
                for (int i = 0; i < parms.Length; i++)
                {
                    argTypes[i] = parms[i].GetType();
                }
                mi = instance.GetType().GetMethod(methodName, argTypes);
            }
            else
            {
                mi = instance.GetType().GetMethod(methodName);
            }
            result = mi.Invoke(instance, parms);
            return result;
        }
    }

}
