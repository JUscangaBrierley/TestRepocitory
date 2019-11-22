using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.CodeGen.Schema
{
	public class TablePropeties
	{
		public TablePropeties()
		{
			ExplicitColumns = false;
			PrimaryKey = new PrimaryKey();
		}

		public string TableName { get; set; }
		public bool ExplicitColumns { get; set; }
		public PrimaryKey PrimaryKey { get; set; }
	}
}
