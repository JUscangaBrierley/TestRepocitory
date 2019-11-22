using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Brierley.FrameWork.CampaignManagement
{
	public class Utils
	{
		public static string GetStepTypeFriendlyName(StepType stepType)
		{
			string friendlyName = string.Empty;
			switch(stepType)
			{
				case StepType.Assignment:
					friendlyName = "Assign";
					break;
				case StepType.ChangeAudience:
					friendlyName = "Change Audience";
					break;
				case StepType.Merge:
					friendlyName = "Merge";
					break;
				case StepType.Output:
					friendlyName = "Output";
					break;
				case StepType.SplitProcess:
					friendlyName = "Split Process";
					break;
				case StepType.Select:
					friendlyName = "Select";
					break;
				case StepType.ControlGroup:
					friendlyName = "Control Group";
					break;
				case StepType.DeDupe:
					friendlyName = "De-Dupe";
					break;
				case StepType.RealTimeAssignment:
					friendlyName = "Real-time Assign";
					break;
				case StepType.RealTimebScript:
					friendlyName = "Real-time bScript";
					break;
				case StepType.RealTimeOutput:
					friendlyName = "Real-time Output";
					break;
				case StepType.RealTimeSelect:
					friendlyName = "Real-time Select";
					break;
				case StepType.RealTimeInputProcessing:
					friendlyName = "Real-time Input Processing";
					break;
				case StepType.Sql:
					friendlyName = "SQL";
					break;
			}
			return friendlyName;
		}



		public static bool DataTypeRequiresLength(string dataType)
		{
			bool required = false;
			switch (dataType.ToLower())
			{
				case "varchar":
				case "nvarchar":
				case "char":
				case "raw":
				case "nchar":
				case "nvarchar2":
				case "varchar2":
					required = true;
					break;
				default:
					break;
			}
			return required;
		}

		public static DbType DbTypeFromString(string type)
		{
			DbType ret = DbType.String;
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}

			switch (type.ToLower())
			{
				case "datetime":
				case "smalldatetime":
				case "timestamp":
				case "timestamp(4)":
				case "timestamp(6)":
					ret = DbType.DateTime;
					break;
				case "date":
					ret = DbType.Date;
					break;
				case "float":
					ret = DbType.Double;
					break;
				case "real":
					ret = DbType.Single;
					break;
				case "decimal":				
				case "numeric":
				case "smallmoney":
				case "number":
					ret = DbType.Decimal;
					break;
				case "int":
				case "bigint":
				case "bit":
				case "smallint":
				case "tinyint":
					ret = DbType.Int64;
					break;
				case "text":
				case "ntext":
				case "varchar":
				case "nvarchar":
				case "uniqueidentifier":
				case "xml":
				case "char":
				case "clob":
				case "nchar":
				case "nclob":
				case "nvarchar2":
				case "varchar2":
				default:
					ret = DbType.String;
					break;
			}

			return ret;
		}
	


	}
}
