using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    public class MemberLoyaltyCardPass : LWCoreObjectBase
	{
		#region properties
		public virtual long ID { get; set; }
		public virtual long PassDefID { get; set; }
		public virtual string SerialNumber { get; set; }
		public virtual string AuthToken { get; set; }
		public virtual long IPCode { get; set; }
		public virtual long VCKey { get; set; }		
		#endregion

		public MemberLoyaltyCardPass() { }
	}
}
