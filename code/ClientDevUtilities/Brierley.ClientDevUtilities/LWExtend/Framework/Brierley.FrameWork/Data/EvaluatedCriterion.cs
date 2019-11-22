//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data
{
	public class EvaluatedCriterion
	{
		private IDictionary<string, bool> _paramList = new Dictionary<string, bool>();

		public string Where { get; set; }

		public IDictionary<string, object> Parameters { get; private set; }

		public void AddParameter(string parmName, object parmValue)
		{
			Parameters.Add(parmName, parmValue);
			_paramList.Add(parmName, false);
		}

		public void AddParameterList(string parmName, object parmValue)
		{
			Parameters.Add(parmName, parmValue);
			_paramList.Add(parmName, true);
		}

		public bool IsParmAList(string parmName)
		{
			return _paramList[parmName];
		}

		public EvaluatedCriterion()
		{
			Where = string.Empty;
			Parameters = new Dictionary<string, object>();
		}
	}
}
