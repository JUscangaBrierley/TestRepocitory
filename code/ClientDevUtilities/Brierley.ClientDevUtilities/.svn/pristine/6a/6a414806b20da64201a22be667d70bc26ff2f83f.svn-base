//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace Brierley.FrameWork.Rules.UIDesign
{
	public class SortableMemberInfo : IComparable
	{
		#region Fields
		MemberInfo _minfo = null;
		#endregion

		#region Constructor
		public SortableMemberInfo(MemberInfo minfo)
		{
			_minfo = minfo;
		}
		#endregion

		#region Properties
		public MemberInfo Info
		{
			get { return _minfo; }
		}
		#endregion

		#region IComparable Methods
		public int CompareTo(object obj)
		{
			object[] attributes1 = _minfo.GetCustomAttributes(typeof(RulePropertyOrderAttribute), true);
			object[] attributes2 = ((SortableMemberInfo)obj).Info.GetCustomAttributes(typeof(RulePropertyOrderAttribute), true);
			RulePropertyOrderAttribute myOrder = attributes1 != null && attributes1.Length > 0 ? (RulePropertyOrderAttribute)attributes1[0] : null;
			RulePropertyOrderAttribute otherOrder = attributes2 != null && attributes2.Length > 0 ? (RulePropertyOrderAttribute)attributes2[0] : null;
			if (myOrder == null)
			{
				//No order specified;
				return 1;
			}
			else if (otherOrder == null)
			{
				return -1;
			}
			else
			{
				return myOrder.Order.CompareTo(otherOrder.Order);
			}
		}
		#endregion
	}
}
