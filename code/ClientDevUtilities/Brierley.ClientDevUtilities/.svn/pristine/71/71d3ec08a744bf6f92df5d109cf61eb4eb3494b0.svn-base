using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.Sql
{
    public struct LWDatabaseFieldType
    {
        public string Name;
        public DataType Type;
        public bool Required;
        public string DefaultValue;
        public string SpecialType;
        public object Value;
        public string Regex;

        public LWDatabaseFieldType Clone()
        {
            LWDatabaseFieldType newField = new LWDatabaseFieldType()
            {
                Name = this.Name,
                Type = this.Type,
                Required = this.Required,
                DefaultValue = this.DefaultValue,
                SpecialType = this.SpecialType,
                Value = this.Value,
                Regex = this.Regex
            };
            return newField;
        }
    }
}
