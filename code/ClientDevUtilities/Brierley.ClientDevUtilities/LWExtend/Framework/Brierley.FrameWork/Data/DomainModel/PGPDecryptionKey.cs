using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Bcpg;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class PGPDecryptionKey
	{
		#region properties
		public string Name { get; set; }
		public string Description { get; set; }
		public bool RequireIntegrityCheckForDecryption { get; set; }
		public string EncodedPrivateKey { get; set; }
		#endregion

		#region constructors
		public PGPDecryptionKey()
		{
			RequireIntegrityCheckForDecryption = false;
		}
		#endregion
	}

	public class PGPDecryptionKeyCollection : Dictionary<string, PGPDecryptionKey>
	{
		public PGPDecryptionKeyCollection()
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

		public void Add(PGPDecryptionKey key)
		{
			this.Add(key.Name, key);
		}
	}
}
