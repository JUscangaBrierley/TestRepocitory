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
	public class Sftp : IDisposable
	{
		private const string _className = "Sftp";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public const int DEFAULT_ConnectTimeoutMs = 0;
		public const int DEFAULT_IdleTimeoutMs = 0;

		private SFtp _sftp = null;
		private bool _isConnected = false;

		public string Login { get; set; }

		public string Password { get; set; }

		/// <summary>
		/// The name or IP address of the remote sftp host
		/// </summary>
		public string Hostname{get;set;}

		/// <summary>
		/// The sftp port number on the remote sftp host
		/// </summary>
		public int Portnumber { get; set; }

		/// <summary>
		/// The private key as returned from Brierley.FrameWork.Common.Security.KeyManager.
		/// </summary>
		public LWPrivateKey PrivateKey { get; set; }

		/// <summary>
		/// Path name on the remote SFTP server.
		/// </summary>
		public string RemotePath { get; set; }

		/// <summary>
		/// Currently not used.
		/// </summary>
		public bool Blocking
		{
			get
			{
				return true;
			}
			set { }
		}

		/// <summary>
		/// Timeout value for connection.  Set to 0 for infinite timeout.
		/// </summary>
		public short Timeout
		{
			get
			{
				return (short)_sftp.IdleTimeoutMs;
			}
			set
			{
				_sftp.ConnectTimeoutMs = _sftp.IdleTimeoutMs = value;
				_logger.Debug(_className, "Timeout", string.Format("Set Timeouts: connect={0} ms, idle={1} ms.", _sftp.ConnectTimeoutMs, _sftp.IdleTimeoutMs));
			}
		}

		public bool IsConnected
		{
			get
			{
				if (!_sftp.IsConnected) _isConnected = false;
				return _isConnected;
			}
		}

        private bool _createTriggerFile = false;
        public bool CreateTriggerFile
        {
            get { return _createTriggerFile; }
            set { _createTriggerFile = value; }
        }

        public string TriggerFileFolder { get; set; }
        public string TriggerFileName { get; set; }


		public Sftp()
		{
			const string methodName = "Sftp";

			Portnumber = 22;
			RemotePath = ".";

			_sftp = new SFtp();
			if (!_sftp.UnlockComponent("RWENTXSSH_zOLvCr5U3JnB"))
			{
				_logger.Error(_className, methodName, "Unable to obtain license for Chilkat SFTP");
			}

			_sftp.ConnectTimeoutMs = DEFAULT_ConnectTimeoutMs;
			_sftp.IdleTimeoutMs = DEFAULT_IdleTimeoutMs;

			_logger.Debug(_className, methodName, string.Format("Initialized Chilkat.SFTP v{0}.", _sftp.Version));
		}

		/// <summary>
		/// Connect to remote SFTP server
		/// </summary>
		public void Connect()
		{
			const string methodName = "Connect";

			// Connect
			if (!_sftp.IsConnected)
			{
				if (!_sftp.Connect(Hostname, Portnumber))
				{
					string message = string.Format("SFTP connect failure: {0}", _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
				else
				{
					_logger.Debug(_className, methodName, string.Format("Connected to {0}:{1}", Hostname, Portnumber));
					_logger.Debug(_className, methodName, string.Format("Remote host key fingerprint: {0}", _sftp.HostKeyFingerprint));
				}
			}
			else
			{
				_logger.Debug(_className, methodName, "Already connected.");
			}

			// Ensure that private key has been specified
			//if (PrivateKey == null)
			//{
			//    string message = "No private key has been specified for SFTP.";
			//    _logger.Error(_className, methodName, message);
			//    throw new LWException(message);
			//}

			// Authenticate
			if (PrivateKey != null && !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
			{
				if (!_sftp.AuthenticatePwPk(Login, Password, (SshKey)PrivateKey))
				{
					string message = string.Format("SFTP authenticate PwPk failure: {0}", _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
			}
			else if (PrivateKey != null && !string.IsNullOrEmpty(Login))
			{
				if (!_sftp.AuthenticatePk(Login, (SshKey)PrivateKey))
				{
					string message = string.Format("SFTP authenticate Pk failure: {0}", _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
			}
			else if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
			{
				if (!_sftp.AuthenticatePw(Login, Password))
				{
					string message = string.Format("SFTP authenticate Pw failure: {0}", _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
			}
			else
			{
				throw new LWException("Insufficient login credentials supplied for SFTP.");
			}

			_logger.Debug(_className, methodName, "Authenticated.");

			//  Initialize
			if (!_sftp.InitializeSftp())
			{
				string message = string.Format("SFTP initialize failure: {0}", _sftp.LastErrorText);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Initialized, using SSLv{0}.", _sftp.ProtocolVersion));

			_isConnected = true;
		}

		/// <summary>
		/// Get a list of the file names for the current remote directory.
		/// </summary>
		/// <returns>list of file names</returns>
		public List<string> ListNames()
		{
			const string methodName = "ListNames";
			EnsureConnected(methodName);
			string handle = OpenRemoteDirectory(RemotePath);

			List<string> result = new List<string>();
			SFtpDir dirListing = null;
			dirListing = _sftp.ReadDir(handle);
			if (dirListing != null)
			{
				//  Iterate over the files.
				int n = dirListing.NumFilesAndDirs;
				if (n > 0)
				{
					for (int i = 0; i <= n - 1; i++)
					{
						SFtpFile fileObj = dirListing.GetFileObject(i);
						_logger.Debug(string.Format("Found item '{0}', type {1}, size {2}", fileObj.Filename, fileObj.FileType, fileObj.SizeStr));

						if (fileObj.FileType != "directory")
						{
							result.Add(fileObj.Filename);
						}
					}
				}
			}

			//  Close the directory
			if (!string.IsNullOrEmpty(handle) && !_sftp.CloseHandle(handle))
			{
				_logger.Debug(string.Format("Failed to close handle '{0}': {1}", handle, _sftp.LastErrorText));
			}

			return result;
		}

		/// <summary>
		/// Stream get remote file.
		/// </summary>
		/// <param name="toStream">stream to which the file is written</param>
		/// <param name="remoteFile">remote file to get</param>
		public void GetFile(Stream toStream, string remoteFile)
		{
			const string methodName = "GetFile1";
			EnsureConnected(methodName);

			// Open file
			string handle = _sftp.OpenFile(remoteFile, "readOnly", "openExisting");
			if (string.IsNullOrEmpty(handle))
			{
				string message = string.Format("Unable to open remote file '{0}'.", remoteFile);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Opened remote file '{0}'.", remoteFile));

			// read file
			byte[] pData = null;
			bool bEof = false;
			int chunkSize = 10000;
			while (bEof == false)
			{
				pData = _sftp.ReadFileBytes(handle, chunkSize);
				if (_sftp.LastReadFailed(handle) == true)
				{
					string message = string.Format("Error reading remote file '{0}': {1}", remoteFile, _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
				else
				{
					toStream.Write(pData, 0, pData.Length);
				}
				bEof = _sftp.Eof(handle);
			}
			_sftp.CloseHandle(handle);
			_logger.Debug(_className, methodName, string.Format("Finished getting remote file '{0}'.", remoteFile));
		}

		/// <summary>
		/// Stream get current RemotePath file.
		/// </summary>
		/// <param name="toStream"></param>
		public void GetFile(Stream toStream)
		{
			GetFile(toStream, RemotePath);
		}

		/// <summary>
		/// Stream put current RemotePath file.
		/// </summary>
		/// <param name="fromStream"></param>
		public void PutFile(Stream fromStream)
		{
			const string methodName = "PutFile";
			EnsureConnected(methodName);

			// Open file
			string handle = _sftp.OpenFile(RemotePath, "writeOnly", "createTruncate");
			if (string.IsNullOrEmpty(handle))
			{
				string message = string.Format("Unable to open remote file '{0}'. {1}", RemotePath, _sftp.LastErrorText);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Opened remote file '{0}'.", RemotePath));

			// write file
			int chunkSize = 10000;
			byte[] buffer = new byte[chunkSize];
			while (true)
			{
				int numBytesRead = fromStream.Read(buffer, 0, buffer.Length);
				if (numBytesRead == 0)
				{
					// The end of the file is reached.
					break;
				}
				byte[] bytes = new byte[numBytesRead];
				Array.Copy(buffer, bytes, numBytesRead);
				if (!_sftp.WriteFileBytes(handle, bytes))
				{
					string message = string.Format("Error writing remote file '{0}': {1}", RemotePath, _sftp.LastErrorText);
					_logger.Error(_className, methodName, message);
					throw new LWException(message);
				}
			}
			_sftp.CloseHandle(handle);
			_logger.Debug(_className, methodName, string.Format("Finished writing remote file '{0}'.", RemotePath));

            if (_createTriggerFile)
            {
                CreateTrigger();
            }
		}

		/// <summary>
		/// Delete the specified remote file.
		/// </summary>
		/// <param name="remotePath"></param>
		public void DeleteFile(string remotePath)
		{
			const string methodName = "DeleteFile1";
			EnsureConnected(methodName);

			if (!_sftp.RemoveFile(remotePath))
			{
				string message = string.Format("Unable to delete remote file '{0}'.", remotePath);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Deleted remote file '{0}'.", remotePath));
		}

		/// <summary>
		/// Delete the file represented by the current RemotePath.
		/// </summary>
		public void DeleteFile()
		{
			const string methodName = "DeleteFile2";
			EnsureConnected(methodName);

			if (!_sftp.RemoveFile(RemotePath))
			{
				string message = string.Format("Unable to delete remote file '{0}'.", RemotePath);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Deleted remote file '{0}'.", RemotePath));
		}

		/// <summary>
		/// Create a directory on the remote SFTP server.
		/// </summary>
		/// <param name="remotePath"></param>
		public void MakeDir(string remotePath)
		{
			const string methodName = "MakeDir";
			EnsureConnected(methodName);

			if (!_sftp.CreateDir(remotePath))
			{
				string message = string.Format("Unable to create remote path '{0}'.", remotePath);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Created remote directory '{0}'.", remotePath));
		}

		/// <summary>
		/// Set the file or directory permissions on the current RemotePath.
		/// </summary>
		/// <param name="permissions"></param>
		public void SetAttributes(long permissions)
		{
			const string methodName = "SetAttributes";
			EnsureConnected(methodName);

			if (!_sftp.SetPermissions(RemotePath, false, (int)permissions))
			{
				string message = string.Format("Unable to set permissions '{0}' on remote path '{1}'.", permissions, RemotePath);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
			_logger.Debug(_className, methodName, string.Format("Set permissions '{0}' on remote path '{1}'.", permissions, RemotePath));
		}

		/// <summary>
		/// Disconnect from the remote SFTP server.
		/// </summary>
		public void Disconnect()
		{
			const string methodName = "Disconnect";
			if (_sftp.IsConnected)
			{
				_sftp.Disconnect();
				_isConnected = false;
				_logger.Debug(_className, methodName, "Disconnected.");
			}
		}


        public void CreateTrigger()
        {
            string methodName = "CreateTriggerFile";

            if (_createTriggerFile)
            {
                string remoteFolder = !string.IsNullOrWhiteSpace(TriggerFileFolder) ? TriggerFileFolder : Path.GetDirectoryName(RemotePath);
                remoteFolder = remoteFolder.Replace('\\', '/');
                if (!remoteFolder.EndsWith("/"))
                {
                    remoteFolder += "/";
                }

                string triggerFile = string.Empty;

                if (!string.IsNullOrWhiteSpace(TriggerFileName))
                {
                    triggerFile = TriggerFileName;
                }
                else
                {
                    triggerFile = string.Format("{0}.trg", Path.GetFileNameWithoutExtension(RemotePath));
                }

                string triggerFilePath = string.Format("{0}{1}", remoteFolder, triggerFile);

                string handle = _sftp.OpenFile(triggerFilePath, "writeOnly", "createTruncate");
                if (string.IsNullOrEmpty(handle))
                {
                    string message = string.Format("Unable to open remote file '{0}'. {1}", triggerFilePath, _sftp.LastErrorText);
                    _logger.Error(_className, methodName, message);
                    throw new LWException(message);
                }
                _logger.Debug(_className, methodName, string.Format("Created remote trigger file '{0}'.", triggerFilePath));
                _sftp.CloseHandle(handle);
            }
        }

		private void EnsureConnected(string methodName)
		{
			if (!IsConnected)
			{
				string message = "Not connected to SFTP server.";
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}
		}

		private string OpenRemoteDirectory(string remotePath)
		{
			const string methodName = "OpenRemoteDirectory";
			string handle = string.Empty;

			handle = _sftp.OpenDir(remotePath);
			if (string.IsNullOrEmpty(handle))
			{
				string message = string.Format("Unable to open remote path '{0}'.", remotePath);
				_logger.Error(_className, methodName, message);
				throw new LWException(message);
			}

			return handle;
		}

		void IDisposable.Dispose()
		{
			if (_sftp != null)
			{
				_sftp.Dispose();
			}
		}
	}
}
