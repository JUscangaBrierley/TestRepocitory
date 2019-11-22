using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class ContactStatusMap : Dictionary<long, string>
	{
		public string GetDescriptionForID(long ID)
		{
			if (this.ContainsKey(ID))
			{
				return this[ID];
			}
			else
			{
				return ID.ToString();
			}
		}
	}
}
