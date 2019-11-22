using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class ContentProperties
	{
		private const string CONTENT_PROPS_PREFIX = "ContentProperties_";
		private ServiceConfig _config;

		internal ContentProperties(string orgName, /*string appName,*/ string envName)
		{
			if (String.IsNullOrEmpty(orgName)) throw new ArgumentNullException("orgName");
			//if (String.IsNullOrEmpty(appName)) throw new ArgumentNullException("appName");
			if (String.IsNullOrEmpty(envName)) throw new ArgumentNullException("envName");
			_config = LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
		}

		/// <summary>
		/// Get the named property's value
		/// </summary>
		/// <param name="name">property's name</param>
		/// <returns>property's value, or "" if property not found</returns>
		public string GetProperty(string name)
		{
			return GetProperty(name, "");
		}

		/// <summary>
		/// Get the named property's value
		/// </summary>
		/// <param name="name">property's name</param>
		/// <param name="defaultValue">value to return if the property is not found</param>
		/// <returns>property's value, or defaultValue if property not found</returns>
		public string GetProperty(string name, string defaultValue)
		{
			string result = SelectProperty(name);
			if (String.IsNullOrEmpty(result)) result = defaultValue;
			return result;
		}

		/// <summary>
		/// Set the value for the named property
		/// </summary>
		/// <param name="name">property's name</param>
		/// <param name="value">value to be set for the property</param>
		public void SetProperty(string name, string value)
		{
			SaveProperty(name, value);
		}

		/// <summary>
		/// Delete the named property
		/// </summary>
		/// <param name="name">property's name</param>
		public void DeleteProperty(string name)
		{
			string key = CONTENT_PROPS_PREFIX + name;
			using (var svc = new DataService(_config))
			{
				svc.DeleteClientConfiguration(key);
			}
		}

		private string SelectProperty(string name)
		{
			using (var svc = new DataService(_config))
			{
				string key = CONTENT_PROPS_PREFIX + name;
				ClientConfiguration prop = svc.GetClientConfiguration(key);
				string result = string.Empty;
				if (prop != null && !string.IsNullOrEmpty(prop.Value))
					result = prop.Value;
				return result;
			}
		}

		private void SaveProperty(string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				DeleteProperty(name);
				return;
			}

			using (var svc = new DataService(_config))
			{
				string key = CONTENT_PROPS_PREFIX + name;
				ClientConfiguration prop = svc.GetClientConfiguration(key);
				if (prop == null)
				{
					prop = new ClientConfiguration();
					prop.Key = key;
					prop.Value = value;
					prop.ExternalValue = false;
					svc.CreateClientConfiguration(prop);
				}
				else
				{
					prop.Value = value;
					prop.ExternalValue = false;
					svc.UpdateClientConfiguration(prop);
				}
			}
		}
	}
}

