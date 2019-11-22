using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Bcpg;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class PGPEncryptionKey
	{
		#region properties
		public string Name { get; set; }
		public string Description { get; set; }
		public bool UseArmor { get; set; }
		public bool UseCompression { get; set; }
		public CompressionAlgorithmTag CompressionType { get; set; }
		public bool UseIntegrityPacket { get; set; }
		public SymmetricKeyAlgorithmTag SymmetricKeyAlgorithm { get; set; }
		public string EncodedPublicKey { get; set; }
		#endregion

		#region constructors
		public PGPEncryptionKey()
		{
			UseArmor = true;
			UseCompression = true;
			CompressionType = CompressionAlgorithmTag.Zip;
			UseIntegrityPacket = true;
			SymmetricKeyAlgorithm = SymmetricKeyAlgorithmTag.Cast5;
		}
		#endregion
	}

	public class PGPEncryptionKeyCollection : Dictionary<string,PGPEncryptionKey>
	{
		public PGPEncryptionKeyCollection() : base()
		{
		}

		public List<string> KeyList
		{
			get
			{
				return new List<string>(this.Keys);
			}
		}

		public void Add(PGPEncryptionKey key)
		{
			this.Add(key.Name, key);
		}
	}
}
