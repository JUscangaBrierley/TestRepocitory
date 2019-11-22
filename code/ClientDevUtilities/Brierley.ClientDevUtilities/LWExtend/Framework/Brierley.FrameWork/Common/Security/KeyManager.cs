using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Chilkat;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Security
{
	public class LWPrivateKey : SshKey
	{
		public LWPrivateKey()
			: base()
		{
		}
	}

	public class KeyManager : IDisposable
	{
		private const string _className = "KeyManager";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private LWPrivateKey _key = new LWPrivateKey();

		public KeyManager()
		{
		}

		public void Load(string keyPath)
		{
			const string methodName = "Load(string)";

			if (!File.Exists(keyPath))
			{
				string message = string.Format("Missing SFTP private key file: '{0}'", keyPath);
				_logger.Error(_className, methodName, message);
				throw new Exception(message);
			}

			if (!_key.FromPuttyPrivateKey(File.ReadAllText(keyPath)))
			{
				string message = string.Format("Error reading SFTP private key file: '{0}': {1}", keyPath, _key.LastErrorText);
				_logger.Error(_className, methodName, message);
				throw new Exception(message);
			}
		}

		public void Load(Stream stream)
		{
			const string methodName = "Load(Stream)";

			StreamReader reader = new StreamReader(stream, true);
			string key = reader.ReadToEnd();

			if (!_key.FromPuttyPrivateKey(key))
			{
				string message = string.Format("Error reading SFTP private key stream: {0}", _key.LastErrorText);
				_logger.Error(_className, methodName, message);
				throw new Exception(message);
			}
		}

		public LWPrivateKey PrivateKey()
		{
			return _key;
		}

		void IDisposable.Dispose()
		{
			if (_key != null)
			{
				_key.Dispose();
			}
		}
	}
}
