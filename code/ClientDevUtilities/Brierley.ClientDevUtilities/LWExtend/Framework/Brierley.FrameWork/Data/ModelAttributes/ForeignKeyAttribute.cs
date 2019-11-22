using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Brierley.FrameWork.Data.ModelAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute()
        {
        }

        public ForeignKeyAttribute(Type foreignKeyClass, string foreignKeyPropertyName)
        {
            object[] classAttributes = foreignKeyClass.GetCustomAttributes(typeof(PetaPoco.TableNameAttribute), true);
            if (classAttributes == null || classAttributes.Length == 0)
                throw new Exception(string.Format("Invalid class '{0}' given, missing TableNameAttribute", foreignKeyClass.Name));

            var tableNameAttribute = (PetaPoco.TableNameAttribute)classAttributes[0];
            ForeignKeyTable = tableNameAttribute.Value;

            PropertyInfo pi = foreignKeyClass.GetProperty(foreignKeyPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if(pi == null)
                throw new Exception(string.Format("Invalid property name '{0}' for class '{1}'", foreignKeyPropertyName, foreignKeyClass.Name));
            var attr = pi.GetCustomAttribute(typeof(PetaPoco.ColumnAttribute));
            if (attr == null)
                throw new Exception(string.Format("Invalid property '{0}' for class '{1}', missing Column attribute", foreignKeyPropertyName, foreignKeyClass.Name));
            var columnAttribute = (PetaPoco.ColumnAttribute)attr;
            ForeignKeyColumn = string.IsNullOrEmpty(columnAttribute.Name) ? pi.Name : columnAttribute.Name;
        }

        public string PrimaryKeyTable { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public string ForeignKeyTable { get; set; }
        public string ForeignKeyColumn { get; set; }

        private string _foreignKeyName;
        public string ForeignKeyName
        {
            get
            {
                if (string.IsNullOrEmpty(_foreignKeyName))
                {
                    if (string.IsNullOrEmpty(PrimaryKeyTable) || string.IsNullOrEmpty(PrimaryKeyColumn))
                        return string.Empty;

                    string name = "FK_{0}_{1}";
                    string tableName = PrimaryKeyTable.ToUpper().Replace("LW_", "").Replace("_", "");
                    string columnName = PrimaryKeyColumn.ToUpper();

                    if (tableName.Length + columnName.Length + 4 > 30)
                    {
                        try
                        {
                            int totalToRemove = tableName.Length + columnName.Length + 4 - 30;
                            int removeTable = (int)Math.Floor((totalToRemove + tableName.Length - columnName.Length) / 2.0d);
                            int removeColumn = (int)Math.Ceiling((totalToRemove + columnName.Length - tableName.Length) / 2.0d);

                            if (removeTable > 0)
                                tableName = tableName.Substring(0, tableName.Length - removeTable);
                            if (removeColumn > 0)
                                columnName = columnName.Substring(0, columnName.Length - removeColumn);
                        }
                        catch
                        {
                            Brierley.FrameWork.Common.Logging.LWLoggerManager.GetLogger(Brierley.FrameWork.Common.LWConstants.LW_FRAMEWORK).Error("ForeignKeyAttribute", "ForeignKeyName",
                                string.Format("Table: {0}, Column: {1}", PrimaryKeyTable, PrimaryKeyColumn));
                            throw;
                        }
                    }
                    _foreignKeyName = string.Format(name, tableName, columnName);
                }
                return _foreignKeyName;
            }
            set
            {
                _foreignKeyName = value;
            }
        }
    }
}
