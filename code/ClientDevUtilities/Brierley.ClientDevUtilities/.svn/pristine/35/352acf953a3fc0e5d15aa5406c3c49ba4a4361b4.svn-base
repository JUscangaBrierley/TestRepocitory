using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	public class StructuredDataCell
	{
		public StructuredContentData Datum;
		public StructuredContentAttribute Attribute;

		public string Name
		{
			get { return Attribute.Name; }
		}
		public string Value
		{
			get { return Datum.Data; }
		}
	}

	[Serializable]
	public class StructuredDataRow : List<StructuredDataCell>
	{
	}

	[Serializable]
	public class StructuredDataRowList : List<StructuredDataRow>
	{
	}
}
