using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CodeGen.Schema
{
    public class IndexingProperties
    {
        public string TableName { get; set; }

        public List<string> Columns { get; set; }

        public IndexType IndexType { get; set; }

        public bool RequiresLowerFunction { get; set; }

        public bool RequiresSubstrFunction { get; set; }

        public string IndexName
        {
            get
            {
                string name = "IDX_{0}_{1}";
                if (IndexType == IndexType.Unique || IndexType == IndexType.UniqueConstraint)
                    name = "UQ_{0}_{1}";

                string tableName = TableName.ToUpper().Replace("LW_", "").Replace("_", "");
                string columnName = string.Join("", Columns).ToUpper();

                if (string.Format(name, tableName, columnName).Length > 30)
                {
                    int totalToRemove = string.Format(name, tableName, columnName).Length - 30;
                    int removeTable = (int)Math.Floor((totalToRemove + tableName.Length - columnName.Length) / 2.0d);
                    int removeColumn = (int)Math.Ceiling((totalToRemove + columnName.Length - tableName.Length) / 2.0d);

                    if (removeTable > 0)
                        tableName = tableName.Substring(0, tableName.Length - removeTable);
                    if (removeColumn > 0)
                    {
                        int oldLength = columnName.Length;
                        columnName = string.Empty;
                        int totalRemoved = 0;
                        foreach (string column in Columns)
                        {
                            int numRemoved = (int)Math.Floor(column.Length * removeColumn / (oldLength * 1.0d));
                            columnName += column.Substring(0, column.Length - numRemoved);
                            totalRemoved += numRemoved;
                        }
                        if (totalRemoved < removeColumn)
                            columnName = columnName.Substring(0, columnName.Length - (removeColumn - totalRemoved));
                    }
                }

                return string.Format(name, tableName, columnName);
            }
        }
    }

    public enum IndexType
    {
        Column,
        Unique,
        UniqueConstraint
    }
}
