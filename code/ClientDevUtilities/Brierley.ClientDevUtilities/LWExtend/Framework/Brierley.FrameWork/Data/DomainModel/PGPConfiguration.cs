using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class PGPConfiguration
	{
		public string Version { get; set; }
		public PGPEncryptionKeyCollection EncryptionKeys { get; set; }
		public PGPDecryptionKeyCollection DecryptionKeys { get; set; }

		public PGPConfiguration()
		{
			Version = "1.0";
			EncryptionKeys = new PGPEncryptionKeyCollection();
			DecryptionKeys = new PGPDecryptionKeyCollection();
		}
	}
}
