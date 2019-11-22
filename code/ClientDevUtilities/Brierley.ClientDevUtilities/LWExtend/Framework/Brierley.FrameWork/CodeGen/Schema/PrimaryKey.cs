using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CodeGen.Schema
{
	public class PrimaryKey
	{
		public string ColumnName { get; set; }
		public string SequenceName { get; set; }
		public bool AutoIncrement { get; set; }
	}
}
