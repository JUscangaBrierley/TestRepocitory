using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Data.ModelAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ColumnIndexAttribute : Attribute
    {
        public ColumnIndexAttribute()
        {

        }

        /// <summary>
        /// The table's name in the database. 
        /// Part of the generated index name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The column's name in the database. 
        /// Part of the generated index name.
        /// </summary>
        public string ColumnName { get; set; }

        public bool RequiresLowerFunction { get; set; }
    }
}
