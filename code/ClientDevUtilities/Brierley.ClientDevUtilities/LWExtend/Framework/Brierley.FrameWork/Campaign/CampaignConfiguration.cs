using System;
using System.Xml.Linq;

using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.Data;


namespace Brierley.FrameWork.CampaignManagement
{
	public class CampaignConfiguration
	{
		public bool ConfigExists { get; set; }

		public bool UseFramework { get; set; }

		public string DatabaseType { get; set; }

		public string ConnectionString { get; set; }

		public string DataSchema { get; set; }

		public bool SendExecutionEmail { get; set; }

		public bool IndexTempTables { get; set; }

		public bool UseArrayBinding { get; set; }

		private int? _maxValueList;


		private ExecutionTypes _executionType = ExecutionTypes.Schedule;

		public ExecutionTypes ExecutionType
		{
			get
			{
				return _executionType;
			}
			set
			{
				_executionType = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of distinct values that will appear in a list in campaign builder.
		/// </summary>
		public int MaxValueList
		{
			get { return _maxValueList.GetValueOrDefault(200); }
			set { _maxValueList = value; }
		}


		#region Conversion Methods

		/*
		public XElement ConvertToXml()
		{
			XElement element = new XElement("ConfigEntries",
				new XAttribute("UseFramework", UseFramework), 
				new XAttribute("ExecutionType", ExecutionType.ToString())
				);

			if (!string.IsNullOrEmpty(DatabaseType))
			{
				element.Add(new XAttribute("DatabaseType", DatabaseType));
			}            
			if (!string.IsNullOrEmpty(ConnectionString))
			{
				element.Add(new XAttribute("ConnectionString", ConnectionString));
			}
			if (!string.IsNullOrEmpty(DataSchema))
			{
				element.Add(new XAttribute("DataSchema", DataSchema));
			}
			if (SendExecutionEmail)
			{
				element.Add(new XAttribute("SendExecutionEmail", true));
			}
			element.Add(new XAttribute("IndexTempTables", true));
			return element;
		}
		*/

		public static CampaignConfiguration LoadFromXml(string xml)
		{
			CampaignConfiguration config = new CampaignConfiguration();

			if (!string.IsNullOrEmpty(xml))
			{
				XElement element = XElement.Parse(xml);

				if (element.Attribute("UseFramework") != null)
				{
					config.UseFramework = bool.Parse(element.AttributeValue("UseFramework"));
				}
				if (!config.UseFramework)
				{
					config.DatabaseType = element.AttributeValue("DatabaseType");
					config.ConnectionString = element.AttributeValue("ConnectionString");
				}
				config.DataSchema = element.AttributeValue("DataSchema");
				if (element.Attribute("ExecutionType") != null)
				{
					config.ExecutionType = (ExecutionTypes)Enum.Parse(typeof(ExecutionTypes), element.AttributeValue("ExecutionType"));
				}
				config.IndexTempTables = bool.Parse(element.AttributeValue("IndexTempTables", "true"));
				config.SendExecutionEmail = bool.Parse(element.AttributeValue("SendExecutionEmail", "false"));
				config.UseArrayBinding = bool.Parse(element.AttributeValue("UseArrayBinding", "false"));
			}
			return config;
		}


		#endregion


		#region Public Helpers


		public static CampaignConfiguration GetConfiguration()
		{
			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
				CampaignConfiguration config = new CampaignConfiguration();
				LWConfiguration lwConfig = LWConfigurationUtil.GetCurrentConfiguration();
				string key = lwConfig.Organization + ":" + lwConfig.Environment + ":CampaignManagementConfig";
				ClientConfiguration cfg = svc.GetClientConfiguration(key);
				if (cfg != null)
				{
					config = CampaignConfiguration.LoadFromXml(cfg.Value);
					config.ConfigExists = true;
				}
				return config;
			}
		}


		#endregion


	}
}
