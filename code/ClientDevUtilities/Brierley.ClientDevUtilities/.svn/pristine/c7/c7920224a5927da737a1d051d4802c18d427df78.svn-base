using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class SFTPKey
	{
		public string EncodedKeyPassword { get; set; }
		public string EncodedKey { get; set; }
	}

	public class SFTPKeyInfo
	{
		#region properties
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Blocking { get; set; }
		public ushort TimeoutMilliseconds { get; set; }
		public string InitialRemotePath { get; set; }
		public string HostName { get; set; }
		public ushort PortNumber { get; set; }
		public string UserName { get; set; }
		public string EncodedPassword { get; set; }
		public SFTPKey Key { get; set; }
		#endregion

		#region constructors
		public SFTPKeyInfo()
		{
			Blocking = true;
			TimeoutMilliseconds = 0;
			PortNumber = 22;
		}
		#endregion
	}

	public class SFTPKeyInfoCollection : Dictionary<string,SFTPKeyInfo>
	{
		public SFTPKeyInfoCollection()
			: base()
		{
		}

		public List<string> KeyList
		{
			get
			{
				return new List<string>(this.Keys);
			}
		}

		public void Add(SFTPKeyInfo keyInfo)
		{
			this.Add(keyInfo.Name, keyInfo);
		}
	}
}
