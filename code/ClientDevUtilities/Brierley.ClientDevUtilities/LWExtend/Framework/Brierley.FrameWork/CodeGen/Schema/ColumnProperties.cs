using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CodeGen.Schema
{
	public class ColumnProperties
	{
		public string ColumnName { get; set; }
		public int ColumnLength { get; set; }
		public bool isNullable { get; set; }
		public bool isEncrypted { get; set; }
		public bool PersistEnumAsString { get; set; }
        public Type PropertyType { get; set; }
	}
}
