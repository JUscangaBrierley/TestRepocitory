using Brierley.FrameWork.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using System.Collections.Generic;

namespace Brierley.FrameWork.CampaignManagement
{
	public class TableMappingExporter
	{
		private ServiceConfig _config;
		
		public TableMappingExporter()
		{
		}
		
		public TableMappingExporter(ServiceConfig config)
		{
			_config = config;
		}

		public XElement ExportMapping(long tableId)
		{
			if (_config == null)
			{
				_config = LWDataServiceUtil.GetServiceConfiguration();
			}

			using (var manager = new CampaignManager(_config))
			{

				var element = new XElement("mapping");

				var table = manager.GetCampaignTable(tableId);
				if (table == null)
				{
					return null;
				}

				element.Add(
					new XAttribute("name", table.Name),
					new XAttribute("iscampaignschema", table.IsCampaignSchema),
					new XAttribute("isframeworkschema", table.IsFrameworkSchema)
				);
				if (!string.IsNullOrEmpty(table.Alias))
				{
					element.Add(new XAttribute("alias", table.Alias));
				}

				var keysElement = new XElement("keys");
				element.Add(keysElement);

				var keys = manager.GetTableKeyByTable(tableId);
				foreach (var key in keys)
				{
					var audience = manager.GetAudience(key.AudienceId);

					keysElement.Add(
						new XElement("key",
							new XAttribute("audience", audience.Name),
							new XAttribute("fieldname", key.FieldName),
							new XAttribute("fieldtype", key.FieldType),
							new XAttribute("isaudiencelevel", key.IsAudienceLevel)
						)
					);
				}

				var fieldsElement = new XElement("fields");
				element.Add(fieldsElement);

				var fields = manager.GetTableFields(tableId);
				foreach (var field in fields)
				{
					XElement fieldElement = new XElement("field",
							new XAttribute("name", field.Name),
							new XAttribute("valuegenerationtype", field.ValueGenerationType),
							new XAttribute("visible", field.Visible.GetValueOrDefault())
						);
					if (!string.IsNullOrEmpty(field.Alias))
					{
						fieldElement.Add(new XAttribute("alias", field.Alias));
					}
					if (!string.IsNullOrEmpty(field.ValueList))
					{
						fieldElement.Add(new XAttribute("valuelist", field.ValueList));
					}
					if (!string.IsNullOrEmpty(field.DataType))
					{
						fieldElement.Add(new XAttribute("datatype", field.DataType));
					}
                    fieldElement.Add(new XAttribute("encryptiontype", field.EncryptionType));

					fieldsElement.Add(fieldElement);
				}

				return element;
			}
		}


	}
}
