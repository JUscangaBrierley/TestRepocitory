using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

using Newtonsoft.Json;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.Sql;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	#region Promo Template Configuration
	public class PromoTemplateProperty
	{
		[JsonProperty(PropertyName = "propName")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "propType")]
		public string PropType { get; set; }

		[JsonProperty(PropertyName = "propVal")]
		public string PropValue { get; set; }

		[JsonProperty(PropertyName = "propShowInReport")]
		public bool PropShowInReport { get; set; }

		[JsonProperty(PropertyName = "templates")]
		public PromoTestSetConfigTemplate[] Templates { get; set; }
	}

	public class PromoTemplateAttSet
	{
		[JsonProperty(PropertyName = "howmany")]
		public int HowMany { get; set; }

		[JsonProperty(PropertyName = "attSetName")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "props")]
		public PromoTemplateProperty[] Properties { get; set; }

		[JsonProperty(PropertyName = "attSets")]
		public PromoTemplateAttSet[] ChildAttSets { get; set; }
	}

	public class PromoTemplateVirtualCard
	{
		[JsonProperty(PropertyName = "howmany")]
		public int HowMany { get; set; }

		[JsonProperty(PropertyName = "props")]
		public PromoTemplateProperty[] Properties { get; set; }

		[JsonProperty(PropertyName = "attSets")]
		public PromoTemplateAttSet[] AttSets { get; set; }
	}

	public class PromoTemplateMember
	{
		[JsonProperty(PropertyName = "props")]
		public PromoTemplateProperty[] Properties { get; set; }

		[JsonProperty(PropertyName = "cards")]
		public PromoTemplateVirtualCard[] VirtualCards { get; set; }

		[JsonProperty(PropertyName = "attSets")]
		public PromoTemplateAttSet[] AttSets { get; set; }
	}

	public class MappedColumn
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public DataType DataType { get; set; }
		public int? Length { get; set; }
		public bool Required { get; set; }
		public string DefaultValue { get; set; }
		public string Format { get; set; }
	}

	public class ExportBatchMapping
	{
		public IList<ExportTarget> ExportTargets { get; set; }
	}

	public class ExportTarget
	{
		public PromoTestOutputTarget OutputTarget { get; set; }
		public string Name { get; set; }
		public string TableName { get; set; }
		public string Delimiter { get; set; }
		public IList<MappedColumn> MappedColumns { get; set; }
	}

	public enum PromoTestType { Enrollment, TLog };
	public enum PromoTestOutputTarget { Framework, Staging, File };

	public class PromoOptions
	{
		public bool BatchType { get; set; } // false if this test set is being used for online
		public PromoTestType TestType { get; set; }
		public PromoTestOutputTarget Target { get; set; }
		public RuleExecutionMode ExecutionMode { get; set; }
		public IList<ContextObject.RuleResult> Results { get; set; }
		public string ExportedFolder { get; set; }
		public long PromotionId { get; set; }
	}

	#endregion

	public class PromoTestDataGenerator
	{
		#region fields
		private const string _className = "PromoTestDataGenerator";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		PromoTestSet _testSet = null;
		Dictionary<string, int> patternedSequence = new Dictionary<string, int>();
		Dictionary<string, int> fileRecordSequence = new Dictionary<string, int>();
		ExportBatchMapping _exportBatchMapping = null;
		Dictionary<Member, PromoTestSetConfigTemplate> _memberMap = new Dictionary<Member, PromoTestSetConfigTemplate>();
		Random _random = new Random();
		#endregion

		#region Construction/Initialization
		public PromoTestDataGenerator(long testSetId)
		{
			const string methodName = "PromoTestDataGenerator";
			using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{
				_testSet = test.GetPromoTestSet(testSetId);
				if (_testSet == null)
				{
					string errMsg = string.Format("Unable to find promotion test set with id = {0}.", testSetId);
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
			}
		}
		#endregion

		#region Private Helpers

		#region Field Value Helpers

		#region Pattern Helpers
		public int GetRandomNumber(int min, int max)
		{
			//Random random = new Random();
			//return random.Next(min, max);
			return _random.Next(min, max);
		}

		private void ValidatePattern(string pattern)
		{
			string methodName = "ValidatePattern";
			if (string.IsNullOrWhiteSpace(pattern) || !pattern.Contains('#'))
			{
				string errMsg = string.Format("Invalid format {0} specified.", pattern);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
		}

		internal class PatternFormat
		{
			public int StartIndex = -1;
			public int EndIndex = -1;
		}

		private List<PatternFormat> ParsePattern(string pattern)
		{
			List<PatternFormat> list = new List<PatternFormat>();

			bool patternStarted = false;
			PatternFormat current = null;
			for (int i = 0; i < pattern.Length; i++)
			{
				char c = pattern[i];
				if (c == '#')
				{
					if (!patternStarted)
					{
						// start the pattern
						current = new PatternFormat() { StartIndex = i };
						patternStarted = true;
						list.Add(current);
					}
				}
				else if (patternStarted)
				{
					// there is a previous pattern that just ended.
					patternStarted = false;
					current.EndIndex = i - 1;
				}
			}
			if (patternStarted && current != null)
			{
				current.EndIndex = pattern.Length - 1;
			}
			return list;
		}

		private int GetUpperLimit(int length)
		{
			List<char> str = new List<char>();
			for (int i = 0; i < length; i++)
			{
				str.Add('9');
			}
			return int.Parse(new String(str.ToArray<char>()));
		}

		private string GetFormattedString(int number, int length)
		{
			string fmt = "";
			for (int i = 0; i < length; i++)
			{
				fmt += "0";
			}
			string str = number.ToString(fmt);
			return str;
		}

		private string GeneratePatternedString(string objectKey, string pattern)
		{
			ValidatePattern(pattern);
			List<PatternFormat> formats = ParsePattern(pattern);
			StringBuilder newValue = new StringBuilder(pattern);
			int index = 0;
			foreach (PatternFormat format in formats)
			{
				int length = format.EndIndex - format.StartIndex + 1;
				int upperLimit = GetUpperLimit(length);
				string valueKey = string.Format("{0}::{1}", objectKey, index);
				int number = -1;
				if (patternedSequence.ContainsKey(valueKey))
				{
					number = patternedSequence[valueKey] + 1;
					if (number > upperLimit)
					{
						number = 1;
					}
					patternedSequence[valueKey] = number;
				}
				else
				{
					number = 1;
					patternedSequence.Add(valueKey, number);
				}
				string str = GetFormattedString(number, length);
				newValue.Remove(format.StartIndex, length);
				newValue.Insert(format.StartIndex, str);
				index++;
			}
			return newValue.ToString();
		}

		private bool GeneratePatternedBoolean(string pattern)
		{
			ValidatePattern(pattern);
			return true;
		}

		#region Patterned Date Helpers
		private DateTime GenerateSinglePatternedDate(string pattern)
		{
			string methodName = "GenerateSinglePatternedDate";

			string[] tokens = pattern.Split('/');
			if (tokens == null || tokens.Length != 3)
			{
				string errMsg = string.Format("Invalid date format {0} specified.", pattern);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			string month = string.Empty;
			string day = string.Empty;
			string year = string.Empty;

			if (tokens[2] == "Y")
			{
				year = GetRandomNumber(DateTimeUtil.MinValue.Year, DateTimeUtil.MaxValue.Year).ToString();
			}
			else
			{
				year = tokens[2];
			}

			if (tokens[0] == "M")
			{
				month = GetRandomNumber(1, 12).ToString();
			}
			else
			{
				month = tokens[0];
			}
			if (tokens[1] == "D")
			{
				int iYear = int.Parse(year);
				int iMonth = int.Parse(month);
				int max = DateTime.DaysInMonth(iYear, iMonth);
				day = GetRandomNumber(1, max).ToString();
			}
			else
			{
				day = tokens[1];
			}

			string strDate = string.Format("{0}/{1}/{2}", month, day, year);

			return DateTime.Parse(strDate);
		}

		private DateTime GenerateTopPatternedDate(string pattern)
		{
			string methodName = "GenerateTopPatternedDate";

			string[] tokens = pattern.Split('/');
			if (tokens == null || tokens.Length != 3)
			{
				string errMsg = string.Format("Invalid date format {0} specified.", pattern);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			string month = string.Empty;
			string day = string.Empty;
			string year = string.Empty;

			if (tokens[2] == "Y")
			{
				year = DateTimeUtil.MaxValue.Year.ToString();
			}
			else
			{
				year = tokens[2];
			}

			if (tokens[0] == "M")
			{
				month = "12";
			}
			else
			{
				month = tokens[0];
			}
			if (tokens[1] == "D")
			{
				int iYear = int.Parse(year);
				int iMonth = int.Parse(month);
				day = DateTime.DaysInMonth(iYear, iMonth).ToString();
			}
			else
			{
				day = tokens[1];
			}

			string strDate = string.Format("{0}/{1}/{2}", month, day, year);

			return DateTime.Parse(strDate);
		}

		private DateTime GenerateBottomPatternedDate(string pattern)
		{
			string methodName = "GenerateBottomPatternedDate";

			string[] tokens = pattern.Split('/');
			if (tokens == null || tokens.Length != 3)
			{
				string errMsg = string.Format("Invalid date format {0} specified.", pattern);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			string month = string.Empty;
			string day = string.Empty;
			string year = string.Empty;

			if (tokens[2] == "Y")
			{
				year = DateTimeUtil.MinValue.Year.ToString();
			}
			else
			{
				year = tokens[2];
			}

			if (tokens[0] == "M")
			{
				month = "1";
			}
			else
			{
				month = tokens[0];
			}
			if (tokens[1] == "D")
			{
				day = "1";
			}
			else
			{
				day = tokens[1];
			}

			string strDate = string.Format("{0}/{1}/{2}", month, day, year);

			return DateTime.Parse(strDate);
		}

		private DateTime GeneratePatternedDate(string pattern)
		{
			string methodName = "GeneratePatternedDate";

			string[] rangeTokens = pattern.Split('-');
			if (rangeTokens == null || (rangeTokens.Length != 1 && rangeTokens.Length != 2))
			{
				string errMsg = string.Format("Invalid date format {0} specified.", pattern);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			if (rangeTokens.Length == 1)
			{
				return GenerateSinglePatternedDate(rangeTokens[0]);
			}
			else
			{
				// this is a range
				// get left and right limits
				DateTime left = GenerateBottomPatternedDate(rangeTokens[0]);
				DateTime right = GenerateTopPatternedDate(rangeTokens[1]);
				// now generate a random date in between the two

				string month = GetRandomNumber(left.Month, right.Month).ToString();
				string day = GetRandomNumber(left.Day, right.Day).ToString();
				string year = GetRandomNumber(left.Year, right.Year).ToString();

				// ensure that the day is not greater than the max
				int iMonth = int.Parse(month);
				int iYear = int.Parse(year);
				int iDay = int.Parse(day);
				if (iDay > DateTime.DaysInMonth(iYear, iMonth))
				{
					day = DateTime.DaysInMonth(iYear, iMonth).ToString();
				}

				string strDate = string.Format("{0}/{1}/{2}", month, day, year);

				try
				{
					return DateTime.Parse(strDate);
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, string.Format("Error creating date from {0}.  Range token was {1}.", strDate, rangeTokens[1]), ex);
					throw;
				}
			}
		}
		#endregion

		private object GeneratePatternValue(string objectKey, Type type, string pattern)
		{
			object value = null;
			if (type == typeof(int) ||
				type == typeof(int?))
			{
				string str = GeneratePatternedString(objectKey, pattern);
				value = int.Parse(str);
			}
			else if (type == typeof(string))
			{
				value = GeneratePatternedString(objectKey, pattern);
			}
			else if (type == typeof(long) ||
				type == typeof(long?))
			{
				string str = GeneratePatternedString(objectKey, pattern);
				value = long.Parse(str);
			}
			//else if (type == typeof(double) ||
			//    type == typeof(double?))
			//{
			//    string str = GeneratePatternedString(objectKey, pattern);
			//    value = double.Parse(str);
			//}
			else if (type == typeof(decimal) ||
			type == typeof(decimal?))
			{
				string str = GeneratePatternedString(objectKey, pattern);
				value = decimal.Parse(str);
			}
			else if (type == typeof(bool) ||
				type == typeof(bool?))
			{
				value = GeneratePatternedBoolean(pattern);
			}
			else if (type == typeof(DateTime) ||
				type == typeof(DateTime?))
			{
				value = GeneratePatternedDate(pattern);
			}
			return value;
		}
		#endregion

		private DataType ConvertToDatabaseType(Type type)
		{
			DataType dbType = DataType.String;
			if (type == typeof(int) ||
				type == typeof(int?))
			{
				dbType = DataType.Integer;
			}
			else if (type == typeof(string))
			{
				dbType = DataType.String;
			}
			else if (type == typeof(long) ||
				type == typeof(long?))
			{
				dbType = DataType.Long;
			}
			else if (type == typeof(decimal) ||
			type == typeof(decimal?))
			{
				dbType = DataType.Number;
			}
			else if (type == typeof(bool) ||
				type == typeof(bool?))
			{
				dbType = DataType.Boolean;
			}
			else if (type == typeof(DateTime) ||
				type == typeof(DateTime?))
			{
				dbType = DataType.Date;
			}
			else if (type.IsEnum)
			{
				dbType = DataType.Integer;
			}
			return dbType;
		}

		private object GenerateStaticValue(Type type, string staticValue)
		{
			object value = null;
			if (type == typeof(int) ||
				type == typeof(int?))
			{
				value = int.Parse(staticValue);
			}
			else if (type == typeof(string))
			{
				value = staticValue;
			}
			else if (type == typeof(long) ||
				type == typeof(long?))
			{
				value = long.Parse(staticValue);
			}
			else if (type == typeof(decimal) ||
			type == typeof(decimal?))
			{
				value = decimal.Parse(staticValue);
			}
			else if (type == typeof(bool) ||
				type == typeof(bool?))
			{
				value = bool.Parse(staticValue);
			}
			else if (type == typeof(DateTime) ||
				type == typeof(DateTime?))
			{
				value = DateTime.Parse(staticValue);
			}
			else if (type.IsEnum)
			{
				Type ut = Enum.GetUnderlyingType(type);
				value = Convert.ChangeType(Enum.Parse(type, staticValue), ut);
			}
			return value;
		}

		private object GenerateFileValue(string objectName, bool newObject, Type type, string fileSpec)
		{
			string methodName = "GenerateFileValue";

			using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{
				object value = null;
				string[] tokens = fileSpec.Split('|');
				if (tokens == null || tokens.Length != 2)
				{
					string errMsg = string.Format("Invalid date format {0} specified.", fileSpec);
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
				PromoDataFile dataFile = test.GetPromoDataFile(tokens[0]);
				if (dataFile != null)
				{
					List<string> columnValues = dataFile.GetColumnValues(tokens[1]);
					int recordNmbr = 0;
					if (dataFile.IsTakenInOrder)
					{
						// sequential
						string key = objectName + "::" + dataFile.Name;
						if (fileRecordSequence.ContainsKey(key))
						{
							recordNmbr = fileRecordSequence[key];
							if (newObject)
							{
								// the object index has changed
								recordNmbr++;
								if (recordNmbr >= columnValues.Count)
								{
									recordNmbr = 0;
								}
								fileRecordSequence[key] = recordNmbr;
							}
						}
						else
						{
							recordNmbr = 0;
							fileRecordSequence.Add(key, recordNmbr);
						}
					}
					else
					{
						// random                    
						recordNmbr = GetRandomNumber(0, columnValues.Count);
					}
					string columnValue = columnValues[recordNmbr];
					value = GenerateStaticValue(type, columnValue);
				}
				return value;
			}
		}

		private object GetInheritedValue(IAttributeSetContainer parent, string propValue)
		{
			object value = null;
			PropertyInfo ppInfo = parent.GetType().GetProperty(propValue);
			if (ppInfo != null)
			{
				value = ppInfo.GetValue(parent, null);
			}
			return value;
		}

		private object GeneratePropValue(IAttributeSetContainer parent, string objectName, bool newObject, PropertyInfo pInfo, PromoTemplateProperty config)
		{
			object value = null;
			switch (config.PropType)
			{
				case "Static":
					value = GenerateStaticValue(pInfo.PropertyType, config.PropValue);
					break;
				case "Pattern":
					value = GeneratePatternValue(objectName + "::" + pInfo.Name, pInfo.PropertyType, config.PropValue);
					break;
				case "File":
					value = GenerateFileValue(objectName, newObject, pInfo.PropertyType, config.PropValue);
					break;
				case "Inherit":
					value = GetInheritedValue(parent, config.PropValue);
					break;
			}
			return value;
		}
		#endregion

		#region Formulate Output Fields

		private void InitializeColumnMappings(string mappingFile)
		{
			string methodName = "InitializeColumnMappings";

			using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{

				_logger.Trace(_className, methodName, string.Format("Initializing column mappings using mapping file {0}.", mappingFile));

				PromoMappingFile map = test.GetPromoMappingFile(mappingFile);
				if (map == null)
				{
					string errMsg = string.Format("Unable to retrieve column mapping file {0}.", mappingFile);
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}

				_exportBatchMapping = new ExportBatchMapping()
				{
					ExportTargets = new List<ExportTarget>()
				};

				MemoryStream mem = new MemoryStream(UTF8Encoding.UTF8.GetBytes(map.Content));
				using (StreamReader reader = new StreamReader(mem))
				{
					XDocument doc = XDocument.Load(reader);
					XElement rootNode = doc.Root;
					foreach (XElement expNode in rootNode.Nodes().OfType<XElement>())
					{
						ExportTarget target = new ExportTarget();
						_exportBatchMapping.ExportTargets.Add(target);

						XAttribute att = expNode.Attribute("Name");
						if (att != null && !string.IsNullOrWhiteSpace(att.Value))
						{
							target.Name = att.Value;
						}
						att = expNode.Attribute("Type");
						target.OutputTarget = (PromoTestOutputTarget)Enum.Parse(typeof(PromoTestOutputTarget), att.Value);
						att = expNode.Attribute("TableName");
						if (att != null && !string.IsNullOrWhiteSpace(att.Value))
						{
							target.TableName = att.Value;
						}
						target.Delimiter = "|";
						att = expNode.Attribute("Delimiter");
						if (att != null && !string.IsNullOrWhiteSpace(att.Value))
						{
							target.Delimiter = att.Value;
						}
						target.MappedColumns = new List<MappedColumn>();
						foreach (XElement colNode in expNode.Nodes().OfType<XElement>())
						{
							if (colNode.Name.LocalName == "Columns")
							{
								foreach (XElement node in colNode.Nodes().OfType<XElement>())
								{
									MappedColumn col = new MappedColumn() { Required = false };
									XAttribute nAtt = node.Attribute("Name");
									col.Name = nAtt.Value;
									nAtt = node.Attribute("Path");
									col.Path = nAtt.Value;
									nAtt = node.Attribute("Type");
									if (nAtt.Value == "Double")
									{
										// we replaced Double with Decimal but there may be some old templates that still may have the old datatype.
										col.DataType = DataType.Decimal;
									}
									else
									{
										col.DataType = (DataType)Enum.Parse(typeof(DataType), nAtt.Value);
									}
									nAtt = node.Attribute("Length");
									if (nAtt != null && !string.IsNullOrWhiteSpace(nAtt.Value))
									{
										col.Length = int.Parse(nAtt.Value);
									}
									nAtt = node.Attribute("Required");
									if (nAtt != null && !string.IsNullOrWhiteSpace(nAtt.Value))
									{
										col.Required = bool.Parse(nAtt.Value);
									}
									nAtt = node.Attribute("DefaultValue");
									if (nAtt != null && !string.IsNullOrWhiteSpace(nAtt.Value))
									{
										col.DefaultValue = nAtt.Value;
									}
									nAtt = node.Attribute("Format");
									if (nAtt != null && !string.IsNullOrWhiteSpace(nAtt.Value))
									{
										col.Format = nAtt.Value;
									}
									target.MappedColumns.Add(col);
								}
							}
						}
					}
				}
			}
		}

		private object GenerateOutputValue(DataType type, string defaultValue)
		{
			object value = null;
			if (type == DataType.Integer)
			{
				value = int.Parse(defaultValue);
			}
			else if (type == DataType.String)
			{
				value = defaultValue;
			}
			else if (type == DataType.Long)
			{
				value = long.Parse(defaultValue);
			}
			//else if (type == DataType.Double)
			//{
			//    value = double.Parse(defaultValue);
			//}
			else if (type == DataType.Decimal)
			{
				value = decimal.Parse(defaultValue);
			}
			else if (type == DataType.Boolean)
			{
				value = bool.Parse(defaultValue);
			}
			else if (type == DataType.Date)
			{
				value = DateTime.Parse(defaultValue);
			}
			return value;
		}

		private string GenerateFormattedValue(MappedColumn column, object itsValue)
		{
			string value = null;
			if (!string.IsNullOrWhiteSpace(column.Format))
			{
				if (column.DataType == DataType.Integer)
				{
					value = int.Parse(itsValue.ToString()).ToString(column.Format);
				}
				else if (column.DataType == DataType.String)
				{
					value = itsValue.ToString();
				}
				else if (column.DataType == DataType.Long)
				{
					value = long.Parse(itsValue.ToString()).ToString(column.Format);
				}
				//else if (column.DataType == DataType.Double)
				//{
				//    value = double.Parse(itsValue.ToString()).ToString(column.Format);
				//}
				else if (column.DataType == DataType.Decimal)
				{
					value = decimal.Parse(itsValue.ToString()).ToString(column.Format);
				}
				else if (column.DataType == DataType.Boolean)
				{
					value = itsValue.ToString();
				}
				else if (column.DataType == DataType.Date)
				{
					value = DateTime.Parse(itsValue.ToString()).ToString(column.Format);
				}
			}
			else
			{
				value = itsValue.ToString();
			}
			return value;
		}

		private LWDatabaseFieldType? GetDefaultField(MappedColumn column)
		{
			string methodName = "GetDefaultField";

			if (string.IsNullOrWhiteSpace(column.DefaultValue))
			{
				if (column.Required)
				{
					string errorMessage = string.Format("Unable to get default value for field {0}.", column.Name);
					_logger.Error(_className, methodName, errorMessage);
					throw new LWException(errorMessage);
				}
				else
				{
					return null;
				}
			}
			else
			{
				LWDatabaseFieldType field = new LWDatabaseFieldType()
				{
					Name = column.Name,
					Type = column.DataType,
					Value = GenerateOutputValue(column.DataType, column.DefaultValue)
				};
				return field;
			}
		}

		private List<LWDatabaseFieldType> GetOutputFields(Dictionary<string, LWDatabaseFieldType> availableFields, ExportTarget target, Member member)
		{


			List<LWDatabaseFieldType> fields = new List<LWDatabaseFieldType>();

			/*
			 * WS 1/8/2014 Jira issue LW-205
			 * RowKwy should not be added in anycase, regardless of the database.  RowKey always represents the records unique
			 * key and will always be either assigned by the database itself (SQL Server) or assigned by nHiberNate
			 * or in case of staging tables by the InsertRecord method of LWQueryUtil.
			 * */
			//long rowKey = 0;
			//LWDatabaseFieldType field = new LWDatabaseFieldType()
			//{
			//    Name = "RowKey",
			//    Value = ++rowKey,
			//    Type = DataType.Long
			//};
			///*
			// * For SQL server, RowKey is an identity field and hence must not be part of the insert statement.
			// * */
			//LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			//if (config.DBConfig.DBType != SupportedDataSourceType.MsSQL2005)
			//{
			//    fields.Add(field);
			//}

			foreach (MappedColumn column in target.MappedColumns)
			{
				if (column.Name == "RowKey")
				{
					// skip it.  It has already been added.
					continue;
				}
				//else if (column.Name == "IpCode")
				//{
				//    field = new LWDatabaseFieldType()
				//    {
				//        Name = column.Name,
				//        Type = DataType.Long,
				//        Value = member.IpCode
				//    };
				//    fields.Add(field);
				//}
				else if (!string.IsNullOrWhiteSpace(column.Path))
				{
					// a path has been specified for the column
					if (availableFields.ContainsKey(column.Path))
					{
						// add this field
						LWDatabaseFieldType newField = availableFields[column.Path].Clone();
						newField.Name = column.Name;
						fields.Add(newField);
					}
					else
					{
						// field not found in the available fields
						LWDatabaseFieldType? f = GetDefaultField(column);
						if (f != null)
						{
							fields.Add(f.Value.Clone());
						}
					}
				}
				else
				{
					// no path is specified for this column
					LWDatabaseFieldType? f = GetDefaultField(column);
					if (f != null)
					{
						fields.Add(f.Value.Clone());
					}
				}
			}
			return fields;
		}

		private Dictionary<string, LWDatabaseFieldType> GetAttributeSetContainersAvialbaleFields(
			string keyPrefix,
			IAttributeSetContainer thisContainer,
			Dictionary<string, LWDatabaseFieldType> availableFields)
		{
			PropertyInfo[] properties = thisContainer.GetType().GetProperties();

			// First log the properties of the container itself
			foreach (PropertyInfo pInfo in properties)
			{
				try
				{
					LWDatabaseFieldType field = new LWDatabaseFieldType()
					{
						Name = pInfo.Name,
						Value = pInfo.GetValue(thisContainer, null),
						Type = ConvertToDatabaseType(pInfo.PropertyType)
					};
					if (field.Value != null)
					{
						//fields.Add(field);
						string key = string.Format("{0}/{1}", keyPrefix, field.Name);
						availableFields.Add(key, field);
					}
				}
				catch (Exception)
				{
					// this property does not define get method.                        
				}
			}
			// now get attribut sets for the container
			if (thisContainer.GetMetaData() != null)
			{
				foreach (AttributeSetMetaData attSet in thisContainer.GetMetaData().ChildAttributeSets)
				{
					IList<IClientDataObject> rows = thisContainer.GetChildAttributeSets(attSet.Name, false);
					if (rows != null && rows.Count > 0)
					{
						// for right now just write the first row.
						IClientDataObject row = rows[0];
						string newPrefix = string.Format("{0}/{1}", keyPrefix, attSet.Name);
						availableFields = GetAttributeSetContainersAvialbaleFields(newPrefix, row, availableFields);
					}
				}
			}

			return availableFields;
		}
		/*
		 * This operation will create the available field map that is required both by
		 * staging and File output types.
		 * */
		private Dictionary<string, LWDatabaseFieldType> GetAvailableFields(Member member, Type memberType)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				Dictionary<string, LWDatabaseFieldType> availableFields = new Dictionary<string, LWDatabaseFieldType>();

				availableFields = GetAttributeSetContainersAvialbaleFields("Member", member, availableFields);

				IList<AttributeSetMetaData> attSets = loyalty.GetAttributeSetsByType(AttributeSetType.Member);
				foreach (AttributeSetMetaData attSet in attSets)
				{
					IList<IClientDataObject> rows = member.GetChildAttributeSets(attSet.Name, false);
					if (rows != null && rows.Count > 0)
					{
						// for right now just write the first row.
						IClientDataObject row = rows[0];
						string newPrefix = string.Format("Member/{0}", attSet.Name);
						availableFields = GetAttributeSetContainersAvialbaleFields(newPrefix, row, availableFields);
					}
				}
				// now get member's virtual cards
				VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
				if (vc != null)
				{
					string newPrefix = string.Format("Member/VirtualCard");
					availableFields = GetAttributeSetContainersAvialbaleFields(newPrefix, vc, availableFields);

					// now get virtual card's attribute sets
					attSets = loyalty.GetAttributeSetsByType(AttributeSetType.VirtualCard);
					foreach (AttributeSetMetaData attSet in attSets)
					{
						IList<IClientDataObject> rows = vc.GetChildAttributeSets(attSet.Name, false);
						if (rows != null && rows.Count > 0)
						{
							// for right now just write the first row.
							IClientDataObject row = rows[0];
							newPrefix = string.Format("Member/VirtualCard/{0}", attSet.Name);
							availableFields = GetAttributeSetContainersAvialbaleFields(newPrefix, row, availableFields);
						}
					}
				}

				return availableFields;
			}
		}

		#endregion

		#region Data Generation Helpers
		private IClientDataObject GenerateAttributeSet(IAttributeSetContainer parent, PromoTemplateAttSet attSetConfig)
		{
			string methodName = "GenerateAttributeSet";
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData meta = loyalty.GetAttributeSetMetaData(attSetConfig.Name);
				if (meta == null)
				{
					string errMsg = string.Format("Unable to retrieve meta data for attribute set {0}.", attSetConfig.Name);
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
				IClientDataObject attSetRow = DataServiceUtil.GetNewClientDataObject(meta.Name);
				Type attSetType = attSetRow.GetType();
				bool newAttSet = true;
				foreach (PromoTemplateProperty prop in attSetConfig.Properties)
				{
					if (string.IsNullOrWhiteSpace(prop.PropValue))
					{
						continue;
					}
					PropertyInfo pInfo = attSetType.GetProperty(prop.Name);
					if (pInfo != null)
					{
						object value = GeneratePropValue(parent, meta.Name, newAttSet, pInfo, prop);
						if (prop.PropType == "File")
						{
							newAttSet = false;
						}
						attSetRow.SetAttributeValue(prop.Name, value);
					}
				}

				foreach (PromoTemplateAttSet childAttSetConfig in attSetConfig.ChildAttSets)
				{
					int howmany = childAttSetConfig.HowMany <= 0 ? 1 : childAttSetConfig.HowMany;
					for (int i = 0; i < howmany; i++)
					{
						IClientDataObject childRow = GenerateAttributeSet(attSetRow, childAttSetConfig);
						attSetRow.AddChildAttributeSet(childRow);
					}
				}

				return attSetRow;
			}
		}

		private VirtualCard GenerateVirtualCard(Member member, PromoTemplateVirtualCard config)
		{
			VirtualCard vc = member.CreateNewVirtualCard();
			Type vType = vc.GetType();
			//first populate its properties
			bool newCard = true;
			foreach (PromoTemplateProperty prop in config.Properties)
			{
				if (string.IsNullOrWhiteSpace(prop.PropValue))
				{
					continue;
				}
				PropertyInfo pInfo = vType.GetProperty(prop.Name);
				if (pInfo != null)
				{
					object value = GeneratePropValue(null, "VirtualCard", newCard, pInfo, prop);
					if (prop.PropType == "File")
					{
						newCard = false;
					}
					if (prop.Name != "Status")
					{
						pInfo.SetValue(vc, value, null);
					}
					else
					{
						VirtualCardStatusType status = (VirtualCardStatusType)value;
						if (status != VirtualCardStatusType.Active)
						{
							vc.NewStatus = status;
						}
					}
				}
			}
			return vc;
		}
		private Member GenerateMember(PromoTemplateMember memberConfig)
		{
			Member m = new Member();
			Type mType = m.GetType();
			//first populate member properties
			bool newMember = true;
			foreach (PromoTemplateProperty prop in memberConfig.Properties)
			{
				if (string.IsNullOrWhiteSpace(prop.PropValue))
				{
					continue;
				}
				PropertyInfo pInfo = mType.GetProperty(prop.Name);
				if (pInfo != null)
				{
					object value = GeneratePropValue(null, "Member", newMember, pInfo, prop);
					if (prop.PropType == "File")
					{
						newMember = false;
					}
					if (prop.Name == "Password" && LWPasswordUtil.IsHashingEnabled())
					{
						m.Salt = CryptoUtil.GenerateSalt();
						m.Password = LWPasswordUtil.HashPassword(m.Salt, value.ToString());
					}
					else
					{
						pInfo.SetValue(m, value, null);
					}
				}
			}
			// populate all its virtual cards            
			foreach (PromoTemplateVirtualCard config in memberConfig.VirtualCards)
			{
				int howmany = config.HowMany <= 0 ? 1 : config.HowMany;
				for (int i = 0; i < howmany; i++)
				{
					GenerateVirtualCard(m, config);
				}
			}
			//now populate its attribute sets            
			foreach (PromoTemplateAttSet attSetConfig in memberConfig.AttSets)
			{
				int howmany = attSetConfig.HowMany <= 0 ? 1 : attSetConfig.HowMany;
				for (int i = 0; i < howmany; i++)
				{
					IClientDataObject attSet = GenerateAttributeSet(m, attSetConfig);
					m.AddChildAttributeSet(attSet);
				}
			}
			return m;
		}

		private Member GenerateTransactions(PromoTemplateMember memberConfig, Member member)
		{
			Member m = new Member();
			Type mType = m.GetType();

			VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
			//now populate its attribute sets
			foreach (PromoTemplateVirtualCard config in memberConfig.VirtualCards)
			{
				foreach (PromoTemplateAttSet attSetConfig in config.AttSets)
				{
					int howmany = attSetConfig.HowMany <= 0 ? 1 : attSetConfig.HowMany;
					for (int i = 0; i < howmany; i++)
					{
						IClientDataObject attSet = GenerateAttributeSet(vc, attSetConfig);
						vc.AddChildAttributeSet(attSet);
					}
				}
			}
			return m;
		}

		#endregion

		public void SavePromoTestMembers(IList<Member> members)
		{
			string methodName = "SavePromoTestMembers";

            using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{
				_logger.Debug(_className, methodName, "Saving promotional test members.");
				List<PromoTestMember> _testMembers = new List<PromoTestMember>();
				foreach (Member member in members)
				{
					if (member.IpCode > 0)
					{
						PromoTestMember pm = new PromoTestMember()
						{
							IpCode = member.IpCode,
							SetId = _testSet.Id
						};
						if (_memberMap.ContainsKey(member))
						{
							PromoTestSetConfigTemplate t = _memberMap[member];
							pm.TemplateName = t.Name;
						}
						_testMembers.Add(pm);
					}
				}
				test.CreatePromoTestMembers(_testMembers);
			}
		}

		public void AddMemberToPromotion(Member member, Promotion promo)
		{
			MemberPromotion p = new MemberPromotion();
			p.MemberId = member.IpCode;
			p.Code = promo.Code;
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				loyalty.CreateMemberPromotion(p);
			}
		}

		#region Enrollment Processing Helper
		/*
         * This method generates all the members and their virtual cards.  It does not generate
         * any attribute sets under the virtual card.
         */
		public IList<Member> GenerateMembers(PromoTestSetConfig config)
		{
			string methodName = "GenerateMembers";

            using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{
				_logger.Debug(_className, methodName, "Generating members...");

				IList<Member> members = new List<Member>();
				foreach (PromoTestSetConfigTemplate t in config.Templates)
				{
					PromoTemplate template = test.GetPromoTemplate(t.Name);
					if (template == null)
					{
						_logger.Error(_className, methodName,
							string.Format("Unable to retrieve template {0}.  Most probably, the template has been deleted.", t.Name));
						continue;
					}
					PromoTemplateMember memberConfig = JsonConvert.DeserializeObject<PromoTemplateMember>(template.Content);
					for (int i = 0; i < t.NumMembers; i++)
					{
						Member m = GenerateMember(memberConfig);
						members.Add(m);
						_memberMap.Add(m, t);
					}
				}
				return members;
			}
		}

		/*
		 * This operation will export member data to a file
		 * */
		public PromoOptions ExportMembersToFile(IList<Member> members, PromoOptions options)
		{
			string methodName = "ExportMembersToFile";

			string randomName = Path.GetRandomFileName();
			string dname = Path.Combine(IOUtils.GetTempDirectory(), Path.GetFileNameWithoutExtension(randomName));
			if (!Directory.Exists(dname))
			{
				Directory.CreateDirectory(dname);
			}
			options.ExportedFolder = dname;

			string tempFile = Path.Combine(dname, _exportBatchMapping.ExportTargets[0].Name + ".dat");

			FileWriter writer = null;

			try
			{
				writer = new FileWriter(tempFile, ASCIIEncoding.ASCII, true);

				_logger.Trace(_className, methodName, string.Format("Writing Enrollment data to file {0}.", tempFile));

				/*
				 * Now write out the column delimiters
				 * */

				bool first = true;
				foreach (MappedColumn column in _exportBatchMapping.ExportTargets[0].MappedColumns)
				{
					if (column.Name == "RowKey")
					{
						continue;
					}
					if (first)
					{
						first = false;
					}
					else
					{
						writer.write(_exportBatchMapping.ExportTargets[0].Delimiter);
					}
					writer.write(column.Name);
				}
				writer.newLine();

				Dictionary<string, LWDatabaseFieldType> availableFields = new Dictionary<string, LWDatabaseFieldType>();

				Type memberType = typeof(Member);
				foreach (Member member in members)
				{
					availableFields = GetAvailableFields(member, memberType);

					IList<LWDatabaseFieldType> fields = GetOutputFields(availableFields, _exportBatchMapping.ExportTargets[0], member);
					Dictionary<string, LWDatabaseFieldType> fieldmap = new Dictionary<string, LWDatabaseFieldType>();
					foreach (LWDatabaseFieldType field in fields)
					{
						fieldmap.Add(field.Name, field);
					}
					first = true;
					foreach (MappedColumn column in _exportBatchMapping.ExportTargets[0].MappedColumns)
					{
						if (column.Name == "RowKey")
						{
							continue;
						}
						if (first)
						{
							first = false;
						}
						else
						{
							writer.write(_exportBatchMapping.ExportTargets[0].Delimiter);
						}
						// find this column in the available fields
						if (fieldmap.ContainsKey(column.Name))
						{
							LWDatabaseFieldType field = fieldmap[column.Name];
							writer.write(GenerateFormattedValue(column, field.Value));
						}
					}
					writer.newLine();
				}
			}
			finally
			{
				if (writer != null)
				{
					writer.dispose();
				}
			}
			return options;
		}


		/*
		 * This operation will stage member data in LW_TXNMEMBER_STAGE table
		 * */
		public void StageMembers(IList<Member> members)
		{
			string methodName = "StageMembers";
			string stagingTable = "LW_TXNMEMBER_STAGE";

			if (!string.IsNullOrWhiteSpace(_exportBatchMapping.ExportTargets[0].TableName))
			{
				stagingTable = _exportBatchMapping.ExportTargets[0].TableName;
			}

			_logger.Trace(_className, methodName, string.Format("Staging member data to table {0}.", stagingTable));

			Dictionary<string, LWDatabaseFieldType> availableFields = new Dictionary<string, LWDatabaseFieldType>();

			LWQueryUtil util = new LWQueryUtil();

			/*
			 * Truncate staging table.
			 * */
			util.TruncateTable(stagingTable);

			Type memberType = typeof(Member);
			foreach (Member member in members)
			{
				availableFields = GetAvailableFields(member, memberType);

				List<LWDatabaseFieldType> fields = GetOutputFields(availableFields, _exportBatchMapping.ExportTargets[0], member);
				util.InsertRecord(stagingTable, "RowKey", fields);
			}
		}

		public List<ContextObject.RuleResult> SaveMembers(IList<Member> members, RuleExecutionMode mode, PromoTestType testType)
		{
			string methodName = "SaveMembers";

			_logger.Trace(_className, methodName, "Executing  enrollment test.");
			var resultList = new List<ContextObject.RuleResult>();

			try
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					if (testType == PromoTestType.Enrollment)
					{
						foreach (Member m in members)
						{
							loyalty.SaveMember(m, resultList, mode);
						}
					}
					else
					{
						loyalty.SaveMembers(members);
					}
				}
			}
			finally
			{
				SavePromoTestMembers(members);
			}

			return resultList;
		}
		#endregion

		#region TLog processing helpers

		/*
         * This operation will export transaction data to a file
         * */
		public PromoOptions ExportTransactionsToFile(IList<Member> members, PromoOptions options)
		{
			string methodName = "ExportTransactionsToFile";

			string randomName = Path.GetRandomFileName();
			string dname = Path.Combine(IOUtils.GetTempDirectory(), Path.GetFileNameWithoutExtension(randomName));
			if (!Directory.Exists(dname))
			{
				Directory.CreateDirectory(dname);
			}
			options.ExportedFolder = dname;

			foreach (ExportTarget target in _exportBatchMapping.ExportTargets)
			{
				string tempFile = Path.Combine(dname, target.Name + ".dat");

				FileWriter writer = null;

				try
				{
					writer = new FileWriter(tempFile, ASCIIEncoding.ASCII, true);

					_logger.Trace(_className, methodName, string.Format("Writing Enrollment data to file {0}.", tempFile));

					/*
					 * Now write out the column delimiters
					 * */

					bool first = true;
					foreach (MappedColumn column in target.MappedColumns)
					{
						if (column.Name == "RowKey")
						{
							continue;
						}
						if (first)
						{
							first = false;
						}
						else
						{
							writer.write(target.Delimiter);
						}
						writer.write(column.Name);
					}
					writer.newLine();

					Dictionary<string, LWDatabaseFieldType> availableFields = new Dictionary<string, LWDatabaseFieldType>();

					Type memberType = typeof(Member);
					foreach (Member member in members)
					{
						availableFields = GetAvailableFields(member, memberType);

						IList<LWDatabaseFieldType> fields = GetOutputFields(availableFields, target, member);
						Dictionary<string, LWDatabaseFieldType> fieldmap = new Dictionary<string, LWDatabaseFieldType>();
						foreach (LWDatabaseFieldType field in fields)
						{
							fieldmap.Add(field.Name, field);
						}
						first = true;
						foreach (MappedColumn column in target.MappedColumns)
						{
							if (column.Name == "RowKey")
							{
								continue;
							}
							if (first)
							{
								first = false;
							}
							else
							{
								writer.write(target.Delimiter);
							}
							// find this column in the available fields
							if (fieldmap.ContainsKey(column.Name))
							{
								LWDatabaseFieldType field = fieldmap[column.Name];
								writer.write(GenerateFormattedValue(column, field.Value));
							}
						}
						writer.newLine();
					}
				}
				finally
				{
					if (writer != null)
					{
						writer.dispose();
					}
				}
			}
			return options;
		}

		/*
		 * This operation will stage member data in LW_TXNDETAIL_STAGE table
		 * */
		public void StageTransactions(IList<Member> members)
		{
			string methodName = "StageTransactions";

			foreach (ExportTarget target in _exportBatchMapping.ExportTargets)
			{
				string stagingTable = "LW_TXNDETAIL_STAGE";

				if (!string.IsNullOrWhiteSpace(target.TableName))
				{
					stagingTable = target.TableName;
				}

				_logger.Trace(_className, methodName, string.Format("Staging member data to table {0}.", stagingTable));

				Dictionary<string, LWDatabaseFieldType> availableFields = new Dictionary<string, LWDatabaseFieldType>();

				LWQueryUtil util = new LWQueryUtil();

				/*
				 * Truncate staging table.
				 * */
				util.TruncateTable(stagingTable);

				Type memberType = typeof(Member);
				foreach (Member member in members)
				{
					availableFields = GetAvailableFields(member, memberType);

					List<LWDatabaseFieldType> fields = GetOutputFields(availableFields, target, member);
					util.InsertRecord(stagingTable, "RowKey", fields);
				}
			}
		}

		/// <summary>
		/// This operation applies the transaction part of the template to the members provided.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="members"></param>
		/// <returns></returns>
		public IList<Member> GenerateTransactions(PromoTestSetConfig config, IList<Member> members)
		{
			string methodName = "GenerateTransactions";

			_logger.Debug(_className, methodName, "Generating transactions...");

			//foreach (PromoTestSetConfigTemplate t in config.Templates)
			//{
			//    PromoTemplate template = _service.GetPromoTemplate(t.Name);                
			//    PromoTemplateMember memberConfig = JsonConvert.DeserializeObject<PromoTemplateMember>(template.Content);
			//    foreach (Member member in members)
			//    {
			//        GenerateTransactions(memberConfig, member);
			//    }                
			//}

			Dictionary<string, PromoTemplateMember> tmpMap = new Dictionary<string, PromoTemplateMember>();
			foreach (Member member in members)
			{
				PromoTestSetConfigTemplate t = _memberMap[member];
				PromoTemplateMember memberConfig = null;
				if (!tmpMap.ContainsKey(t.Name))
				{
                    using (var test = LWDataServiceUtil.TestingDataServiceInstance())
					{
						PromoTemplate template = test.GetPromoTemplate(t.Name);
						memberConfig = JsonConvert.DeserializeObject<PromoTemplateMember>(template.Content);
						tmpMap.Add(t.Name, memberConfig);
					}
				}
				else
				{
					memberConfig = tmpMap[t.Name];
				}
				GenerateTransactions(memberConfig, member);
			}

			return members;
		}
		#endregion

		#endregion

		#region Public Methods
		public PromoOptions Generate(PromoOptions options)
		{
			string methodName = "Generate";
			List<ContextObject.RuleResult> resultList = new List<ContextObject.RuleResult>();
			if (_testSet == null)
			{
				string errMsg = string.Format("No promotion test set has been initialized for data generation.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			_logger.Trace(_className, methodName, string.Format("Generating data for promotion test set {0}.", _testSet.Name));
			PromoTestSetConfig config = _testSet.GetConfig();
			if (config == null)
			{
				string errMsg = string.Format("Promotion test set {0} has not yet been configured.", _testSet.Name);
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			/*
			 * let us make sure that the total number of records for an online set do not exceed
			 * the configured limit.
			 * */
			if (/*config.TestType == "Online"*/!options.BatchType)
			{
				long size = 0;
				foreach (PromoTestSetConfigTemplate t in config.Templates)
				{
					size += t.NumMembers;
				}
				string strSize = LWConfigurationUtil.GetConfigurationValue("PromoTestOnlineSetSize") ?? "100";
				long limit = long.Parse(strSize);
				if (size > limit)
				{
					string errMsg = string.Format("The configured limit for online test size is {0}.  This test set's size is {1}.", limit, size);
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
			}

			if (options.Target != PromoTestOutputTarget.Framework)
			{
				// initialize the column mappings
				InitializeColumnMappings(config.MappingFile);
			}

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())


			using (var content = LWDataServiceUtil.ContentServiceInstance())
            using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{

				Promotion promo = null;
				if (options.PromotionId > 0)
				{
					promo = content.GetPromotion(options.PromotionId);
					if (promo != null)
					{
						if (!promo.IsValid())
						{
							string errMsg = string.Format("Promotion {0} is not valid anymore.", promo.Name);
							_logger.Error(_className, methodName, errMsg);
							throw new LWException(errMsg);
						}
					}
					else
					{
						string errMsg = string.Format("Unable to retrieve promotion with id {0}.", options.PromotionId);
						_logger.Error(_className, methodName, errMsg);
						throw new LWException(errMsg);
					}
				}

				IList<Member> members = null;
				var existingMembers = test.GetPromoTestMembers(_testSet.Id);
				if (options.TestType == PromoTestType.Enrollment)
				{
					members = GenerateMembers(config);
					if (options.Target == PromoTestOutputTarget.Framework)
					{
						resultList = SaveMembers(members, options.ExecutionMode, options.TestType);
						if (promo != null)
						{
							// add the members to the specified promotion
							foreach (Member member in members)
							{
								AddMemberToPromotion(member, promo);
							}
						}
					}
					else if (options.Target == PromoTestOutputTarget.Staging)
					{
						StageMembers(members);
					}
					else
					{
						options = ExportMembersToFile(members, options);
					}
				}
				else if (options.TestType == PromoTestType.TLog)
				{
					/*
					 * If TLog then we need to apply the tlog portion of the template and process the members.
					 * */
					if (existingMembers.Count == 0)
					{
						/*
						 * No members exist yet.  We need to create the members in th edatabase, regardless of the 
						 * output target.  The output target directive only applies for the transactions part.
						 */
						members = GenerateMembers(config);
						resultList = SaveMembers(members, options.ExecutionMode, options.TestType);
						if (promo != null)
						{
							// add the members to the specified promotion
							foreach (Member member in members)
							{
								AddMemberToPromotion(member, promo);
							}
						}
					}
					else
					{
						// use existing
						var ipcodes = (from x in existingMembers select x.IpCode).Distinct<long>();
						members = loyalty.GetAllMembers(ipcodes.ToArray<long>(), true);
						// populate the member/template map
						_memberMap.Clear();
						Dictionary<long, PromoTestMember> tmp = new Dictionary<long, PromoTestMember>();
						foreach (PromoTestMember pm in existingMembers)
						{
							if (!tmp.ContainsKey(pm.IpCode))
							{
								tmp.Add(pm.IpCode, pm);
							}
						}
						// now iterate over members and populate the memberMap
						foreach (Member m in members)
						{
							PromoTestMember pm = tmp[m.IpCode];
							var tms = (from x in config.Templates where x.Name == pm.TemplateName select x);
							if (tms.Count() > 0)
							{
								_memberMap.Add(m, tms.ElementAt(0));
							}
						}
					}
					members = GenerateTransactions(config, members);
					if (options.Target == PromoTestOutputTarget.Framework)
					{
						foreach (Member m in members)
						{
							loyalty.SaveMember(m, resultList, options.ExecutionMode);
							if (promo != null)
							{
								// add the member to the specified promotion
								AddMemberToPromotion(m, promo);
							}
						}
					}
					else if (options.Target == PromoTestOutputTarget.Staging)
					{
						StageTransactions(members);
					}
					else
					{
						options = ExportTransactionsToFile(members, options);
					}

				}

				options.Results = resultList;
				return options;
			}
		}

		public void Clear()
		{
			string methodName = "Clear";
			if (_testSet == null)
			{
				string errMsg = string.Format("No promotion test set has been initialized for clearing test data.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWException(errMsg);
			}
			_logger.Trace(_className, methodName, string.Format("Clearing test data for promotion test set {0}.", _testSet.Name));
            using (var test = LWDataServiceUtil.TestingDataServiceInstance())
			{
				test.DeletePromoTestMembersBySetId(_testSet.Id);
			}
		}
		#endregion
	}
}
