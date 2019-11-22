using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Interfaces
{
    public enum StepType { Workflow, Process, Both };

	/// <summary>
	/// base class for both structure and attributes
	/// </summary>
    [XmlInclude(typeof(MpaProperty))]
    [KnownType(typeof(MpaProperty))]
    //[XmlInclude(typeof(List<MpaProperty>))]
    //[KnownType(typeof(List<MpaProperty>))]
    //[DataContract]
    //public abstract class MpaAttributeBase
    //{
    //}

	/// <summary>
	/// defines a structure of attributes, to be collected as rows of data
	/// </summary>
    //[DataContract]
    //public class List<MpaProperty> : MpaAttributeBase
    //{
    //    #region properties
    //    [DataMember]
    //    public string StructureName { get; set; }

    //    //Structure can define things like min and max rows required, depending on how far we want to take it
    //    //public int MinRows { get; set; }
    //    //public int MaxRows { get; set; }

    //    [DataMember]
    //    public List<MpaAttributeBase> Structure { get; set; }

    //    [DataMember]
    //    public MpaAttributeData Data { get; set; }
    //    #endregion

    //    #region constructors
    //    public List<MpaProperty>()
    //    {
    //        Structure = new List<MpaAttributeBase>();
    //    }

    //    public List<MpaProperty>(string structureName, params MpaAttributeBase[] structure)
    //    {
    //        StructureName = structureName;

    //        Structure = new List<MpaAttributeBase>();

    //        ////set Rows with default structure
    //        if (structure != null && structure.Length > 0)
    //        {
    //            foreach (var s in structure)
    //            {
    //                Structure.Add(s);
    //            }
    //        } 
    //    }
    //    #endregion

    //    #region public methods
    //    public static bool PropertyExists(List<MpaProperty> list, string keyName, bool recursive = false)
    //    {
    //        if (list.Structure == null)
    //            return false;
    //        bool exists = list.Structure.Where(x => (x is MpaAttribute && ((MpaAttribute)x).Name == keyName) ||
    //                                                (recursive && x is List<MpaProperty> && PropertyExists((List<MpaProperty>)x, keyName, true))
    //                ).Count() > 0;
    //        return exists;
    //    }
		
    //    public static MpaAttributeBase GetProperty(List<MpaProperty> structure, string keyName)
    //    {
    //        var prop = (from x in structure.Structure where x is MpaAttribute && ((MpaAttribute)x).Name == keyName select x);
    //        if (prop.Count() > 0)
    //        {
    //            return prop.ElementAt<MpaAttributeBase>(0);
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }

    //    public static object GetData(List<MpaProperty> structure, string keyName, int rowIndex = 0)
    //    {
    //        if(structure.Data == null || structure.Data.Count <= rowIndex)
    //        {
    //            return null;
    //        }
    //        var row = structure.Data[rowIndex];
    //        var col = row[keyName];
    //        return col != null ? col.Value : null;
    //    }

    //    public static void SetData(List<MpaProperty> structure, string keyName, object value, int rowIndex = 0, bool create = false)
    //    {
    //        if (structure.Data == null || structure.Data.Count <= rowIndex)
    //        {
    //            if (!create)
    //            {
    //                return;
    //            }
    //            else
    //            {
    //                if (structure.Data == null)
    //                {
    //                    structure.Data = new MpaAttributeData();
    //                }
    //                while (structure.Data.Count <= rowIndex)
    //                {
    //                    structure.Data.Add(new MpaAttributeDataRow());
    //                }
    //            }
    //        }
    //        var row = structure.Data[rowIndex];
    //        var col = row[keyName];
    //        if (col == null)
    //        {
    //            col = new MpaAttributeDataColumn(keyName, value);
    //            row.Add(col);
    //        }
    //        else
    //        {
    //            col.Value = value;
    //        }
    //    }

    //    public static List<MpaProperty> Deserialize(string structure)
    //    {
    //        if (string.IsNullOrWhiteSpace(structure))
    //        {
    //            return null;
    //        }
    //        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    //        return JsonConvert.DeserializeObject<List<MpaProperty>>(structure, settings);
    //    }

    //    public static string Serialize(List<MpaProperty> structure)
    //    {
    //        if (structure == null)
    //        {
    //            throw new ArgumentNullException("structure");
    //        }
    //        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    //        return JsonConvert.SerializeObject(structure, settings);
    //    }
    //    #endregion
    //}

	[DataContract]
	public class MpaProperty /*: MpaAttributeBase*/
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DataType DataType { get; set; }

		[DataMember]
		public bool Required { get; set; }

        [DataMember]
        public bool Output { get; set; }

        [DataMember]
        public object Value { get; set; }

		[DataMember]
		public List<object> DefaultValues { get; set; }
		
		public MpaProperty(string name)
			: this(name, false, DataType.String, null)
		{
		}

		public MpaProperty(string name, bool required)
			: this(name, required, DataType.String, null)
		{
		}

		public MpaProperty(string name, DataType dataType)
			: this(name, false, dataType, null)
		{
		}

		public MpaProperty(string name, bool required, DataType dataType)
			: this(name, required, dataType, null)
		{
		}

		public MpaProperty(string name, bool required, DataType dataType, List<object> defaultValues)
		{
			Name = name;
			Required = required;
			DataType = dataType;
			DefaultValues = defaultValues;
		}

        public MpaProperty()
		{
		}

        #region Helpers
        public static bool PropertyExists(List<MpaProperty> list, string keyName)
        {
            bool exists = (from x in list where x.Name == keyName select x).Count() > 0;
            return exists;
        }

        public static MpaProperty GetProperty(List<MpaProperty> list, string keyName)
        {
            var prop = (from x in list where x.Name == keyName select x);
            if (prop.Count() > 0)
            {
                return prop.ElementAt<MpaProperty>(0);
            }
            else
            {
                return null;
            }
        }

		public static string GetStringProperty(List<MpaProperty> list, string keyName)
		{
			var prop = (from x in list where x.Name == keyName select x);
			if (prop.Count() > 0)
			{
				MpaProperty mpaProperty = prop.ElementAt<MpaProperty>(0);
				return StringUtils.FriendlyString(mpaProperty.Value);
			}
			else
			{
				return string.Empty;
			}
		}

        public static void SetProperty(List<MpaProperty> list, string keyName, object value)
        {
            var prop = (from x in list where x.Name == keyName select x);
            if (prop.Count() > 0)
            {
                prop.ElementAt<MpaProperty>(0).Value = value;
            }            
        }

        public static string Serialize(List<MpaProperty> properties)
        {            
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.SerializeObject(properties, settings);
        }

        public static List<MpaProperty> Deserialize(string propStr)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<List<MpaProperty>>(propStr, settings);
        }
        #endregion
	}

    //[CollectionDataContract]
    //public class MpaAttributeData : List<MpaAttributeDataRow>
    //{
    //    public MpaAttributeData()
    //    {
    //    }

    //    public MpaAttributeData(params MpaAttributeDataRow[] rows)
    //    {
    //        if (rows != null)
    //        {
    //            foreach (var row in rows)
    //            {
    //                this.Add(row);
    //            }
    //        }
    //    }
    //}

    //[CollectionDataContract]
    //public class MpaAttributeDataRow : List<MpaAttributeDataColumn>
    //{
    //    public MpaAttributeDataRow()
    //    {
    //    }

    //    public MpaAttributeDataRow(params MpaAttributeDataColumn[] columns)
    //        : base()
    //    {
    //        if (columns != null)
    //        {
    //            foreach (var column in columns)
    //            {
    //                this.Add(column);
    //            }
    //        }
    //    }

    //    public MpaAttributeDataColumn this[string name]
    //    {
    //        get
    //        {
    //            foreach(var c in this)
    //            {
    //                if(c.AttributeName == name)
    //                {
    //                    return c;
    //                }
    //            }
    //            return null;
    //        }
    //    }
    //}

    //[DataContract]
    //public class MpaAttributeDataColumn
    //{
    //    [DataMember]
    //    public string AttributeName { get; set; }

    //    [DataMember]
    //    public object Value { get; set; }

    //    [DataMember]
    //    public DataType DataType { get; set; }

    //    [DataMember]
    //    public bool Output { get; set; }

    //    [DataMember]
    //    public bool Required { get; set; }

    //    [DataMember]
    //    public List<string> DefaultValues { get; set; }

    //    public MpaAttributeDataColumn()
    //    {
    //    }

    //    public MpaAttributeDataColumn(string attributeName, object value)
    //    {
    //        AttributeName = attributeName;
    //        Value = value;
    //    }
    //}


    public interface IMPAStepConfiguration
    {
        #region Properties
        /// <summary>
        /// Step's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Display Text
        /// </summary>
        string DisplayText { get; }

        /// <summary>
        /// Name of the icon for step pallet
        /// </summary>
        string IconPath { get; }
        /// <summary>
        /// Step's category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// This determines what type of process does this adapter support.
        /// </summary>
        StepType SupportedStepType { get; }        
        #endregion

        #region Display Related
        /// <summary>
        /// This method will be called by the loyalty navigator to display the tool tip for this adapter
        /// when the mouse is hovered over it.
        /// </summary>
        /// <returns></returns>
        string GetToolTip();
        #endregion

        #region Designtime Related
        /// <summary>
        /// This method will be called by the loyalty navigator to get step's properties.  Step specific
        /// properties will then be displayed in a property grid so that the user can make the correct
        /// selection.  These properties might for example include the name of the campaign or
        /// email.
        /// </summary>
        /// <returns></returns>
        List<MpaProperty> GetStepProperties();

        /// <summary>
        /// This method will be called by the loyalty navigator to populate the data sheet for the 
        /// step so that its data can be mapped to process's data for input and output during design time.
        /// The key in the emap is the name of the category and the value is the list of all properties
        /// in that category.
        /// </summary>
        /// <returns></returns>
        List<MpaProperty> GetStepDataDefinition(List<MpaProperty> properties);
        #endregion

        #region Runtime Related                        
        /// <summary>
        /// Create a runtime instance for this step.
        /// </summary>
        /// <returns></returns>
        IMPAStepRunTime GetRunTimeInstance();
        #endregion
    }

    public class MPAContext : Brierley.FrameWork.ContextObject
    {
        public Member CurrentMember
        {
            get { return this.Owner as Member; }
            set { this.Owner = value; }
        }
        public List<MpaProperty> Properties { get; set; }
    }

    public interface IMPAStepRunTime : IDisposable
    {
        void Initialize(string stepName, MPAContext context, List<MpaProperty> stepData);
        void Start(string stepName, MPAContext context);
        List<MpaProperty> GetStepData();
    }
}
