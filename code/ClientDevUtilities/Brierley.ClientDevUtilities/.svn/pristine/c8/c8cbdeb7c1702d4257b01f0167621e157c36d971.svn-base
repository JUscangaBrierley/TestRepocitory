//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace Brierley.FrameWork.Common.XML
{
    /// <summary>
    /// 
    /// </summary>
    public static class RuleInstanceSerializationUtil
	{
		#region Fields
		private static Dictionary<string, object> serializationInProgressMap = new Dictionary<string, object>();
		#endregion

		#region Serialization Map methods
		private static object GetSerializationInstance(string instanceId)
		{
			object inst = null;
			lock (serializationInProgressMap)
			{
				if (serializationInProgressMap.ContainsKey(instanceId))
				{
					inst = serializationInProgressMap[instanceId];
				}
			}
			return inst;
		}

		private static void AddSerialzationInstance(string instanceId, object instance)
		{
			lock (serializationInProgressMap)
			{
				if (!serializationInProgressMap.ContainsKey(instanceId))
				{
					serializationInProgressMap.Add(instanceId, instance);
				}
			}
		}

		private static void RemoveSerialzationInstance(string instanceId)
		{
			lock (serializationInProgressMap)
			{
				if (serializationInProgressMap.ContainsKey(instanceId))
				{
					serializationInProgressMap.Remove(instanceId);
				}
			}
		}
		#endregion

		#region Helper methods
		/// <summary>
        /// 
        /// </summary>
        /// <param name="sobj"></param>
        /// <returns></returns>
        public static MemberInfo[] GetConfigurableMembers(Object sobj)
        {
            if (sobj == null)
            {
                throw new ArgumentNullException("sobj", "Specify a non-null argument.");
            }
            ArrayList list = new ArrayList();
            foreach (MemberInfo memberInfo in sobj.GetType().FindMembers(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, null, null))
            {
                object[] attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), true);
				if (attributes.Length > 0)
				{
					foreach (object attribute in attributes)
					{
						if (attribute.GetType() == typeof(System.ComponentModel.BrowsableAttribute))
						{
							bool browsable = ((System.ComponentModel.BrowsableAttribute)attribute).Browsable;
							if (browsable)
							{
								list.Add(memberInfo);
							}
						}
					}
				}
            }
            return list.ToArray(typeof(System.Reflection.MemberInfo)) as System.Reflection.MemberInfo[];
        }

        //public static Assembly LoadAssembly(string assemblyPath)
        //{
        //    Assembly assembly = null;
        //    string dllPath = assemblyPath;
        //    if (!string.IsNullOrEmpty(dllPath))
        //    {
        //        string name = Path.GetFileNameWithoutExtension(dllPath);
        //        AppDomain domain = System.AppDomain.CurrentDomain;
        //        try
        //        {
        //            Assembly[] loaded = domain.GetAssemblies();
        //            foreach (Assembly ass in loaded)
        //            {
        //                if (ass.GetName().Name.Equals(name) == true)
        //                {
        //                    assembly = ass;
        //                    break;
        //                }
        //            }
        //            if (assembly == null)
        //            {
        //                // now try to load it.
        //                assembly = Assembly.LoadFrom(dllPath);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //    return assembly;
        //}

        public static void SetPropertyValue(object obj, string propertyType, string propertyName, bool isEnum, string propertyValue)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Specify a non-null argument.");
            }
            try
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    return;
                }
                PropertyInfo pi = obj.GetType().GetProperty(propertyName);
				if (pi == null)
				{
					string msg = string.Format("The rule {0} may have been previously configured with {1} property in it.  We cannot find this property any more.  Try dropping the rule and then reconfiguring it.",
						obj.GetType().Name, propertyName);					
					return;
				}
                MethodInfo mi = pi.GetSetMethod();
                if (mi == null)
                {
                    return;
                }
                if (pi.PropertyType.FullName != propertyType)
                {
                    string msg = string.Format("Was expecting the type of {0} to be {1} but found it to be {2}",
                        propertyName, propertyType, pi.PropertyType.FullName);
                    throw new Exception(msg);
                }
                if (!isEnum)
                {
                    Type type = Type.GetType(propertyType);
                    if (type == typeof(System.String))
                    {                        
                        pi.SetValue(obj, propertyValue, null);
                    }
                    else
                    {                        
                        object typeObj = System.Activator.CreateInstance(type);
                        Type[] args = new Type[1];
                        args[0] = propertyValue.GetType();
                        MethodInfo parseMethod = typeObj.GetType().GetMethod("Parse", args);
                        if (parseMethod != null)
                        {
                            string[] vals = new string[1];
                            vals[0] = propertyValue;
                            object valueObj = parseMethod.Invoke(typeObj, vals);                            
                            pi.SetValue(obj, valueObj, null);
                        }
                    }                    
                }
                else
                {
                    Type enumType = pi.PropertyType;
                    if (enumType == null && pi.PropertyType.Assembly != null)
                    {
                        enumType = pi.PropertyType.Assembly.GetType(propertyType);
                    }
                    if (enumType != null)
                    {                        
                        FieldInfo info = enumType.GetField(propertyValue);
                        object enumValue = info.GetValue(obj);
                        pi.SetValue(obj, enumValue, null);
                    }
                    else
                    {
                        throw new Exceptions.LWException("Unable to create an instance of type " + propertyType);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error setting value for " + propertyName + ": " + ex.Message);
                throw;
            }
        }

        #endregion

        /// <summary>
		/// This method will perform shallow serialization of an object to xml.
        /// </summary>
        /// <param name="sobj"></param>
        /// <param name="fwkVersion"></param>
        /// <returns></returns>
        public static string SerializeRuleInstance(Object sobj, string fwkVersion)
        {
            if (sobj == null)
            {
                throw new ArgumentNullException("sobj", "Specify a non-null argument.");
            }

            Type type = sobj.GetType();

            XDocument doc = new XDocument();
            XElement root = new XElement(type.Name);
            doc.Add(root);
            XAttribute att = new XAttribute("InstanceId", System.Guid.NewGuid().ToString());
            root.Add(att);
            att = new XAttribute("Type", type.FullName);
            root.Add(att);
            att = new XAttribute("Assembly", type.Assembly.FullName);
            root.Add(att);
            att = new XAttribute("FwkVersion", fwkVersion);
            root.Add(att);
            
			string location = type.Assembly.Location;
			if (string.IsNullOrEmpty(location))
			{
				location = type.Assembly.ManifestModule.ScopeName;
			}

            att = new XAttribute("Location", location);
            root.Add(att);
			
            MemberInfo[] minfoList = GetConfigurableMembers(sobj);
            foreach (MemberInfo mInfo in minfoList)
            {
                PropertyInfo pi = sobj.GetType().GetProperty(mInfo.Name);
                object value = pi.GetValue(sobj, null);
                if (value != null)
                {
                    XElement node = new XElement("Property");
                    root.Add(node);

                    XAttribute natt = new XAttribute("Type", pi.PropertyType.FullName);
                    node.Add(natt);
                    natt = new XAttribute("Enum", pi.PropertyType.IsEnum.ToString());
                    node.Add(natt);
                    natt = new XAttribute("Name", mInfo.Name);
                    node.Add(natt);
                    node.Value = value is DateTime ? ((DateTime)value).ToString("u") : value.ToString();
                }
            }

            return doc.ToString();
        }

		private static Assembly GetAssembly(string assemblyName, string location)
		{
			Assembly assembly = null;
			if (!string.IsNullOrEmpty(assemblyName))
			{
                // Use only the short name because the rules may have been created in a previous version
                // of framework.
                AssemblyName an = new AssemblyName(assemblyName);
                //assembly = ClassLoaderUtil.LoadAssemblyFromName(assemblyName, true);
                assembly = ClassLoaderUtil.LoadAssemblyFromName(an.Name, false);				
			}
			return assembly;			
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object DeSerializeRuleInstance(string xml)
        {
            object obj = null;
            if (!string.IsNullOrEmpty(xml))
            {
                XDocument doc = XDocument.Parse(xml);
                XElement root = doc.Root;
                string instanceId = root.Attribute("InstanceId").Value;
				object inst = GetSerializationInstance(instanceId);
				if (inst != null)
				{
					return inst;
				}				
                else
                {
                    string type = root.Attribute("Type").Value;
					string assemblyName = root.Attribute("Assembly").Value;
                    string location = root.Attribute("Location").Value;

					Assembly assembly = GetAssembly(assemblyName, location);
                    if (assembly != null)
                    {
                        try
                        {
                            obj = assembly.CreateInstance(type);
                            if (obj != null)
                            {
                                AddSerialzationInstance(instanceId, obj);
                                foreach (XElement node in root.Elements())
                                {
                                    if (node.Name.LocalName == "Property")
                                    {
                                        string propertyType = node.Attribute("Type").Value;
                                        string propertyName = node.Attribute("Name").Value;
                                        bool isEnum = bool.Parse(node.Attribute("Enum").Value);
                                        string value = node.Value;
                                        SetPropertyValue(obj, propertyType, propertyName, isEnum, value);
                                    }
                                    else
                                    {
                                        string msg = string.Format("Unexpected node {0} found while expecting Property", node.Name.LocalName);
                                        throw new Exception(msg);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Unable to create an object of type: " + type);
                            }
                        }
                        finally
                        {
                            RemoveSerialzationInstance(instanceId);
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Unable to load assembly {0} to de-serialize  rule.",
                            assemblyName));
                    }
                }
            }
            return obj;
        }
    }
}
