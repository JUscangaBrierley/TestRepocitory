using Brierley.FrameWork.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using System.Collections.Generic;
using System.IO;
using System;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.CampaignManagement
{
	public class TableMappingImporter
	{
		private ServiceConfig _config;
				
		public TableMappingImporter()
		{
		}

		public TableMappingImporter(ServiceConfig config)
		{
			_config = config;
		}

		public long ImportMapping(string xml, bool overwrite)
		{
			XElement mappingElement = XElement.Parse(xml);
			return ImportMapping(mappingElement, overwrite);
		}

		public long ImportMapping(Stream stream, bool overwrite)
		{
			var reader = new StreamReader(stream, true);
			string xml = reader.ReadToEnd();
			XElement element = XElement.Parse(xml);
			return ImportMapping(element, overwrite);
		}

		public long ImportMapping(XElement element, bool overwrite)
		{
			if (_config == null)
			{
				_config = LWDataServiceUtil.GetServiceConfiguration();
			}

			if (element.Name != "mapping")
			{
				throw new ArgumentException("The argument element is not a valid exported mapping.");
			}

			using (var manager = new CampaignManager(_config))
			{

				string tableName = element.AttributeValue("name");

				CampaignTable existing = manager.GetCampaignTable(tableName);
				if (existing != null)
				{
					if (overwrite)
					{
						//_manager.DeleteCampaignTable(existing.Id);

						foreach (TableField field in manager.GetTableFields(existing.Id))
						{
							manager.DeleteTableField(field.Id);
						}
                        foreach(TableKey key in manager.GetTableKeyByTable(existing.Id))
                        {
                            manager.DeleteTableKey(key.Id);
                        }
					}
					else
					{
						throw new Exception(string.Format("A campaign table named '{0}' already exists.", tableName));
					}
				}


				CampaignTable table = null;
				if (existing != null)
				{
					table = existing.Clone(existing);
				}
				else
				{
					table = new CampaignTable(tableName, TableType.Framework);
				}
				if (element.Attribute("alias") != null)
				{
					table.Alias = element.AttributeValue("alias");
				}
				table.IsCampaignSchema = Convert.ToBoolean(element.AttributeValue("iscampaignschema"));
				table.IsFrameworkSchema = Convert.ToBoolean(element.AttributeValue("isframeworkschema"));

				if (existing != null)
				{
					manager.UpdateCampaignTable(table);
				}
				else
				{
					manager.CreateCampaignTable(table);
				}

				foreach (var keyElement in element.Element("keys").Descendants("key"))
				{
					var audience = manager.GetAudience(keyElement.AttributeValue("audience"));

					TableKey key = new TableKey();
					key.TableId = table.Id;
					key.AudienceId = audience.Id;
					key.FieldName = keyElement.AttributeValue("fieldname");
					key.FieldType = keyElement.AttributeValue("fieldtype");
					key.IsAudienceLevel = Convert.ToBoolean(keyElement.AttributeValue("isaudiencelevel"));

					manager.CreateTableKey(key);
				}

				foreach (var fieldElement in element.Element("fields").Descendants("field"))
				{
					TableField field = new TableField();
					field.TableId = table.Id;
					field.Alias = fieldElement.AttributeValue("alias", null);
					field.DataType = fieldElement.AttributeValue("datatype", null);
                    field.EncryptionType = (AttributeEncryptionType)Enum.Parse(typeof(AttributeEncryptionType), fieldElement.AttributeValue("encryptiontype", AttributeEncryptionType.None.ToString()));
					field.Name = fieldElement.AttributeValue("name");
					field.ValueGenerationType = (ValueGenerationType)Enum.Parse(typeof(ValueGenerationType), fieldElement.AttributeValue("valuegenerationtype"));
					field.ValueList = fieldElement.AttributeValue("valuelist", null);
					field.Visible = Convert.ToBoolean(fieldElement.AttributeValue("visible"));

					manager.CreateTableField(field);
				}

				return table.Id;
			}
		}

	}
}
