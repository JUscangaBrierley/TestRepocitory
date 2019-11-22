using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

using Chilkat;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;

using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;
using Newtonsoft.Json;
using Brierley.FrameWork.Common.Security;
using System.Xml.Linq;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Brierley.FrameWork.Common
{
    public class CryptoUtil
    {
        #region password hashing
		/// <summary>
		/// Generate salt to be used for password hashing.  Salt should be generated per-user 
		/// so that two different users with the same password will have different hashes.
		/// Salt should also be isolated from the hashed password so that it cannot be used
		/// to generate rainbow tables.  If the per-user salt changes, the password must be
		/// re-hashed.
		/// </summary>
		/// <returns>Base64 encoded salt bytes</returns>
		public static string GenerateSalt()
		{
			byte[] salt = new byte[32];
			RNGCryptoServiceProvider.Create().GetBytes(salt);
			return Convert.ToBase64String(salt);
		}
		
		/// <summary>
		/// Hash a password.  The result is Base64 encoded and will always be 44 characters long.  
		/// Password validation should already have been applied.
		/// </summary>
		/// <param name="salt">salt created by GenerateSalt()</param>
		/// <param name="password">clear text password</param>
		/// <returns>Base64 encoded hash of the password</returns>
		public static string PasswordHash(string salt, string password)
		{
			if (string.IsNullOrEmpty(salt))
				throw new LWException("salt is null or empty");
			if (string.IsNullOrEmpty(password))
				throw new LWException("password is null or empty");
			
			// convert salt
			byte[] saltBytes = Convert.FromBase64String(salt);
			if (saltBytes.Length != 32)
				throw new LWException("salt is invalid");

			// salt the password. 
			byte[] passwordBytes = UnicodeEncoding.Unicode.GetBytes(password);
			byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
			Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
			Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

			// iterate the hash
			HashAlgorithm hashAlgorithm = new SHA256Managed();
			for (int i = 0; i < 10000; i++)
			{
				// hash salted password				
				combinedBytes = hashAlgorithm.ComputeHash(combinedBytes);
			}

			return Convert.ToBase64String(combinedBytes);
		}

		/// <summary>
		/// Computes a SHA256 hash for the provided byte[] and returns hashed value as byte[].
		/// </summary>
		/// <param name="unhashedData">data to hash</param>
		/// <returns>hashed value as byte[]</returns>
		public static byte[] HashToBytes(byte[] unhashedData)
		{
			if (unhashedData == null || unhashedData.Length < 1)
				throw new ArgumentException("CryptoUtil.HashToBytes(unhashedData): unhashedData is null or empty");

			SHA256 shaM = new SHA256Managed();
			byte[] result = shaM.ComputeHash(unhashedData);
			return result;
		}

        /// <summary>
        /// Computes a SHA256 hash for the provided string and returns hashed value as byte[].
        /// </summary>
        /// <param name="unhashedValue">value to hash</param>
        /// <returns>hashed value as byte[]</returns>
        public static byte[] HashToBytes(String unhashedValue)
        {
            if (string.IsNullOrEmpty(unhashedValue))
                throw new ArgumentException("CryptoUtil.HashToBytes(unhashedValue): unhashedValue is null or empty");

            byte[] unhashedData = Encoding.UTF8.GetBytes(unhashedValue);
			byte[] result = HashToBytes(unhashedData);
            return result;
        }

		/// <summary>
		/// Computes a SHA256 hash for the provided byte[] and return string representation of the hashed value.
		/// </summary>
		/// <param name="unhashedData">data to hash</param>
		/// <returns>string representation of the hashed value</returns>
		public static String HashToString(byte[] unhashedData)
		{
			if (unhashedData == null || unhashedData.Length < 1)
				throw new ArgumentException("CryptoUtil.HashToString(unhashedData): unhashedData is null or empty");

			byte[] hashedData = HashToBytes(unhashedData);
			String result = BitConverter.ToString(hashedData).ToLower().Replace("-", "");
			return result;
		}

        /// <summary>
        /// Computes a SHA256 hash for the provided string and return string representation of the hashed value.
        /// </summary>
        /// <param name="unhashedValue">value to hash</param>
        /// <returns>string representation of the hashed value</returns>
        public static String HashToString(String unhashedValue)
        {
            if (string.IsNullOrEmpty(unhashedValue))
                throw new ArgumentException("CryptoUtil.HashToString(unhashedValue): unhashedValue is null or empty");

            byte[] hashedData = HashToBytes(unhashedValue);
            String result = BitConverter.ToString(hashedData).ToLower().Replace("-", "");
            return result;
        }
		#endregion

		#region base64 encoding/decoding
		/// <summary>
		/// Encodes the provided byte array into a base64-encoded string.
		/// </summary>
		/// <param name="bytes">byte array to encode</param>
		/// <returns>base64-encoded string</returns>
		public static string Encode(byte[] bytes)
		{
			if (bytes == null || bytes.Length < 1)
				throw new ArgumentNullException("bytes");

			string result = Convert.ToBase64String(bytes);
			return result;
		}

		/// <summary>
		/// Encodes the provided string.  The string is first encoded into UTF8 bytes, 
		/// which are then encoded into a base64-encoded string.  
		/// </summary>
		/// <param name="data">string to encode</param>
		/// <returns>UTF8-encoded then base64-encoded string</returns>
		public static string EncodeUTF8(string data)
		{
			if (string.IsNullOrEmpty(data))
				throw new ArgumentNullException("data");

			byte[] bytes = Encoding.UTF8.GetBytes(data);
			string result = Encode(bytes);
			return result;
		}

		/// <summary>
		/// Decodes the provided base64-encoded string into a byte array.
		/// </summary>
		/// <param name="data">base64-encoded string to decode</param>
		/// <returns>base64-decoded byte array</returns>
		public static byte[] Decode(string data)
		{
			if (string.IsNullOrEmpty(data))
				throw new ArgumentNullException("data");

			byte[] result = Convert.FromBase64String(data);
			return result;
		}

		/// <summary>
		/// Decodes the provided base64-encoded string.  The base64-decoded bytes are 
		/// converted to a string using UTF8-decoding.
		/// </summary>
		/// <param name="data">base64-encoded string to decode</param>
		/// <returns>base64-decoded then UTF8-decoded string</returns>
		public static string DecodeUTF8(string data)
		{
			if (string.IsNullOrEmpty(data))
				throw new ArgumentNullException("data");

			byte[] bytes = Decode(data);
			string result = Encoding.UTF8.GetString(bytes);
			return result;
		}
		#endregion

		#region PKI-based encryption/decryption
		private const int RSA_ENCRYPT_BLOCKSIZE = 53;

		/// <summary>
		/// Validates the provided keystore with no password for the keystore.
		/// </summary>
		/// <param name="keystore">keystore</param>
		public static void ValidateKeystore(string keystore)
		{
			ValidateKeystore(null, keystore);
		}

		/// <summary>
		/// Validates the provided keystore.
		/// </summary>
		/// <param name="encodedKeystorePassword">base64-encoded keystore password</param>
		/// <param name="keystore">keystore</param>
		public static void ValidateKeystore(string encodedKeystorePassword, string keystore)
		{
			if (string.IsNullOrEmpty(encodedKeystorePassword))
			{
				LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
				LWKeystore lwkeystore = config.LWKeystore;
				encodedKeystorePassword = lwkeystore.EncodedKeystorePass;
			}

            string keystorePass = DecodeUTF8(encodedKeystorePassword);
            PrivateKey privKey = new PrivateKey();
            if (!privKey.LoadEncryptedPem(keystore, keystorePass))
                throw new LWException("Error loading private key: " + privKey.LastErrorText);
        }

        /// <summary>
        /// Generate a keypair to be used for public-key (aka. asymmetric) encryption.  The 
        /// resulting keypair is encrypted using the provided password, and returned as a 
        /// PKCS#8 PEM-formatted string.  The provided password must have been encoded using 
        /// the CryptoUtil.EncodeUTF8() method.
        /// </summary>
        /// <param name="encodedKeystorePassword">base64-encoded password used to encrypt the key</param>
        /// <returns>base64-encoded encrypted key</returns>
        public static string GenerateKey(string encodedKeystorePassword, long keySize)
        {
            if (string.IsNullOrEmpty(encodedKeystorePassword))
                throw new ArgumentNullException("encodedKeystorePassword");

            RSACryptoServiceProvider crypto = new RSACryptoServiceProvider((int)keySize);
            if (crypto == null)
                throw new LWException("Error generating private key");

            string privKeyXML = crypto.ToXmlString(true);
            if (string.IsNullOrEmpty(privKeyXML))
                throw new LWException("Error getting private key as xml");

            PrivateKey privKey = new PrivateKey();
            if (!privKey.LoadXml(privKeyXML))
                throw new LWException("Error loading private key xml");

            string keystorePassword = DecodeUTF8(encodedKeystorePassword);
            string result = privKey.GetPkcs8EncryptedPem(keystorePassword);
            return result;
        }

        /// <summary>
        /// Re-encrypt an encrypted keypair.  The provided key is decrypted using the old
        /// password and encrypted using the new password.  The provided passwords must 
        /// have been encoded using the CryptoUtil.EncodeUTF8() method.  The provided key must
        /// have ben generated using the CryptoUtil.GenerateKey() method.
        /// </summary>
        /// <param name="encodedOldPassword">base64-encoded current key encryption password</param>
		/// <param name="encodedNewPassword">base64-encoded new key encryption password</param>
		/// <param name="key">base64-encoded encrypted key</param>
		/// <returns>base64-encoded re-encrypted key</returns>
        public static string ReEncryptKey(string encodedOldPassword, string encodedNewPassword, string key)
        {
            if (string.IsNullOrEmpty(encodedOldPassword))
                throw new ArgumentNullException("encodedOldPassword");
            if (string.IsNullOrEmpty(encodedNewPassword))
                throw new ArgumentNullException("encodedNewPassword");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            // Decrypt key using old password
            string keystorePass = DecodeUTF8(encodedOldPassword);

            PrivateKey privKey = new PrivateKey();
            if (!privKey.LoadEncryptedPem(key, keystorePass))
                throw new LWException("Error loading private key: " + privKey.LastErrorText);

            // Re-encrypt key using new password
            string newPassword = DecodeUTF8(encodedNewPassword);
            string result = privKey.GetPkcs8EncryptedPem(newPassword);
            return result;
        }

		/// <summary>
		/// Performs PKI-based encryption of the provided string encoded using UTF8.
		/// </summary>
		/// <param name="data">string to UTF8-encode then encrypt</param>
		/// <returns>base64-encoded encrypted data</returns>
		public static string EncryptAsymmetricUTF8(string data)
		{
			LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			LWKeystore lwkeystore = config.LWKeystore;
            return EncryptAsymmetricUTF8(lwkeystore, data);
		}

        /// <summary>
        /// Performs PKI-based encryption of the provided string encoded using UTF8.
        /// </summary>
        /// <param name="lwkeystore">lwkeystore</param>
        /// <param name="data">string to UTF8-encode then encrypt</param>
        /// <returns>base64-encoded encrypted data</returns>
        public static string EncryptAsymmetricUTF8(LWKeystore lwkeystore, string data)
        {
            byte[] unencryptedBytes = Encoding.UTF8.GetBytes(data);
            return EncryptAsymmetric(lwkeystore, unencryptedBytes);
        }

		/// <summary>
		/// Performs PKI-based encryption of the provided byte array.
		/// </summary>
		/// <param name="unencryptedBytes">bytes to encrypt</param>
		/// <returns>base64-encoded encrypted data</returns>
		public static string EncryptAsymmetric(byte[] unencryptedBytes)
		{
			LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			LWKeystore lwkeystore = config.LWKeystore;
            return EncryptAsymmetric(lwkeystore, unencryptedBytes);
		}

		/// <summary>
		/// Performs PKI-based encryption of the provided byte array.
		/// </summary>
		/// <param name="lwkeystore">lwkeystore</param>
		/// <param name="unencryptedBytes">bytes to encrypt</param>
		/// <returns>base64-encoded encrypted data</returns>
		public static string EncryptAsymmetric(LWKeystore lwkeystore, byte[] unencryptedBytes)
		{
            RSACryptoServiceProvider encryptor = lwkeystore.PublicCryptoProvider;

            byte[] encryptedBytes = new Byte[0];
            for (int idx = 0; idx < unencryptedBytes.Length; idx += RSA_ENCRYPT_BLOCKSIZE)
            {
                int blockLength = RSA_ENCRYPT_BLOCKSIZE;
                if ((unencryptedBytes.Length - idx) < RSA_ENCRYPT_BLOCKSIZE)
                    blockLength = unencryptedBytes.Length - idx;

                byte[] unencryptedBlock = new byte[blockLength];
                Array.Copy(unencryptedBytes, idx, unencryptedBlock, 0, blockLength);

                byte[] encryptedBlock = encryptor.Encrypt(unencryptedBlock, false);
                int len = encryptedBytes.Length;
                Array.Resize<byte>(ref encryptedBytes, len + encryptedBlock.Length);
                encryptedBlock.CopyTo(encryptedBytes, len);
            }

            string encodedEncryptedString = Encode(encryptedBytes);
            return encodedEncryptedString;
        }

		/// <summary>
		/// Performs PKI-based decryption of the provided base64-encoded encrypted data which is a UTF8-encoded string.
		/// </summary>
		/// <param name="encryptedData">base64-encoded encrypted data which is a UTF8-encoded string</param>
		/// <returns>decrypted UTF8 string</returns>
		public static string DecryptAsymmetricUTF8(string encryptedData)
		{
			LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			LWKeystore lwkeystore = config.LWKeystore;
            return DecryptAsymmetricUTF8(lwkeystore, encryptedData);
		}

        /// <summary>
        /// Performs PKI-based decryption of the provided base64-encoded encrypted data which is a UTF8-encoded string.
        /// </summary>
        /// <param name="lwkeystore">lwkeystore</param>
        /// <param name="encryptedData">base64-encoded encrypted data which is a UTF8-encoded string</param>
        /// <returns>decrypted UTF8 string</returns>
        public static string DecryptAsymmetricUTF8(LWKeystore lwkeystore, string encryptedData)
        {
            byte[] unencryptedBytes = DecryptAsymmetric(lwkeystore, encryptedData);
            return Encoding.UTF8.GetString(unencryptedBytes);
        }
        
        /// <summary>
        /// Performs PKI-based decryption of the provided base64-encoded encrypted binary data.
        /// </summary>
        /// <param name="encryptedData">base64-encoded encrypted binary data</param>
        /// <returns>decrypted binary data as byte array</returns>
        public static byte[] DecryptAsymmetric(string encryptedData)
		{
			LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			LWKeystore lwkeystore = config.LWKeystore;
			return DecryptAsymmetric(lwkeystore, encryptedData);
		}

		/// <summary>
		/// Performs PKI-based decryption of the provided base64-encoded encrypted binary data.
		/// </summary>
		/// <param name="lwkeystore">lwkeystore</param>
		/// <param name="encryptedData">base64-encoded encrypted binary data</param>
		/// <returns>decrypted binary data as byte array</returns>
		public static byte[] DecryptAsymmetric(LWKeystore lwkeystore, string encryptedData)
		{
            RSACryptoServiceProvider decryptor = lwkeystore.PrivateCryptoProvider;

            byte[] encryptedBytes = Decode(encryptedData);
            byte[] unencryptedBytes = new Byte[0];

            int blocksize = (int)(lwkeystore.KeySize / 8);
            if ((encryptedBytes.Length % blocksize) != 0)
                throw new Exception("Invalid encrypted data");

            for (int idx = 0; idx < encryptedBytes.Length; idx += blocksize)
            {
                byte[] encryptedBlock = new byte[blocksize];
                Array.Copy(encryptedBytes, idx, encryptedBlock, 0, blocksize);

                byte[] unencryptedBlock = decryptor.Decrypt(encryptedBlock, false);
                int len = unencryptedBytes.Length;
                Array.Resize<byte>(ref unencryptedBytes, len + unencryptedBlock.Length);
                unencryptedBlock.CopyTo(unencryptedBytes, len);
            }

            return unencryptedBytes;
        }
        #endregion

        #region symmetric encryption/decryption
        private static Dictionary<string, HttpClient> _clients = new Dictionary<string, HttpClient>();
        private static HttpClient EncryptDecryptClient
        {
            get
            {
                LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
                if (!_clients.ContainsKey(DataServiceUtil.GetKey(config.Organization, config.Environment)))
                {
                    string baseURL = config.FWConfig.GetFWConfigProperty("EncryptionBaseURL");
                    if (baseURL.EndsWith("/"))
                        baseURL = baseURL.Substring(0, baseURL.Length - 1);

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(baseURL);
                    _clients.Add(DataServiceUtil.GetKey(config.Organization, config.Environment), client);
                }
                return _clients[DataServiceUtil.GetKey(config.Organization, config.Environment)];
            }
        }

		/// <summary>
		/// Symmetric encryption of the provided data using the provided LW keystore.  The data must be UTF8.
		/// 
		/// Note, if the same initialization vector is used for the same data, the resulting encrypted data
		/// will be the same.  This is convenient for exact matching the encrypted data, but undermines the
		/// strength of the encryption.
		/// </summary>
		/// <param name="lwkeystore">keystore</param>
		/// <param name="data">data to encrypt</param>
		/// <param name="vector">initialization vector</param>
		/// <returns>encrypted data</returns>
		public static string Encrypt(LWKeystore lwkeystore, string data, string vector = "InitializationVector")
		{
			if (lwkeystore == null || string.IsNullOrWhiteSpace(lwkeystore.EncodedKeystorePass) || string.IsNullOrWhiteSpace(lwkeystore.Keystore))
				throw new ArgumentNullException("lwkeystore");
			if (string.IsNullOrWhiteSpace(data))
				throw new ArgumentNullException("data");

            // Use the "P" prime number part of the PKI key as the symmetric key
            PrivateKey privKey = lwkeystore.PrivateKey;
			string privKeyXml = privKey.GetXml();
			XElement element = XElement.Parse(privKeyXml);
			string encodedKey = element.Element("P").Value;

			return Encrypt(encodedKey, data, vector);
		}

        /// <summary>
        /// This updated version of the Symmetric encryption method uses key and IV values that are generated specifically
        /// for symmetric encryption and not hardcoded IV values and extractiving a piece of the original keystore.
        /// </summary>
        /// <param name="lwkeystore">Keystore object</param>
        /// <param name="data">Data to be encrypted</param>
        /// <returns>The encrypted value of the data that was passed in</returns>
        public static string Encryptv2(LWKeystore lwkeystore, string data)
        {
            if (lwkeystore == null || string.IsNullOrWhiteSpace(lwkeystore.SymmectricKey) || string.IsNullOrWhiteSpace(lwkeystore.SymmetricIV))
                throw new ArgumentNullException("lwkeystore");
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException("data");

			//check for service. if none exists, go to old method
			LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
			string baseURL = config.FWConfig.GetFWConfigProperty("EncryptionBaseURL");
			if (string.IsNullOrEmpty(baseURL))
			{
				return Encrypt(lwkeystore, data);
			}


            return EncryptDecryptWebService(data, "Encrypt");
        }

		/// <summary>
        /// Symmetric encryption of the provided data using the provided key.  The key must have been 
        /// encoded using the CryptoUtil.EncodeUTF8() method.  The data must be UTF8.
		/// 
		/// Note, if the same initialization vector is used for the same data, the resulting encrypted data
		/// will be the same.  This is convenient for exact matching the encrypted data, but undermines the
		/// strength of the encryption.
        /// </summary>
        /// <param name="encodedKey">base64-encoded key</param>
        /// <param name="data">data to be encrypted</param>
		/// <param name="vector">initialization vector</param>
        /// <returns>encrypted string</returns>
		public static string Encrypt(string encodedKey, string data, string vector = "InitializationVector")
        {
            string key = DecodeUTF8(encodedKey);

            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Missing config entry: EncryptionKey");
            if (string.IsNullOrEmpty(vector)) throw new ArgumentException("Missing config entry: InitializationVector");

            // Setup the encryptor
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.BlockSize = 256;
            RMCrypto.KeySize = 256;
            RMCrypto.Mode = CipherMode.CBC;
            RMCrypto.Padding = PaddingMode.Zeros;
            ICryptoTransform encryptor = RMCrypto.CreateEncryptor(GetHash(key), GetHash(vector));

            // get the data in the correct format.
            byte[] rawData = System.Text.Encoding.UTF8.GetBytes(data);
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream();

            // Perform the encryption and return the results in base64 string.
            CryptoStream CryptStream = new CryptoStream(inputStream, encryptor, CryptoStreamMode.Write);
            CryptStream.Write(rawData, 0, rawData.Length);
            CryptStream.FlushFinalBlock();
            byte[] encryptedBytes = inputStream.ToArray();
            string result = Encode(encryptedBytes);
            return result;
        }


		/// <summary>
		/// Symmetric decryption of the provided encrypted data using the provided LW keystore.
		/// 
		/// Note, if the same initialization vector is used for the same data, the resulting encrypted data
		/// will be the same.  This is convenient for exact matching the encrypted data, but undermines the
		/// strength of the encryption.
		/// </summary>
		/// <param name="lwkeystore">keystore</param>
		/// <param name="data">encrypted data</param>
		/// <param name="vector">initialization vector</param>
		/// <returns>unencrypted data</returns>
		public static string Decrypt(LWKeystore lwkeystore, string data, string vector = "InitializationVector")
		{
			if (lwkeystore == null || string.IsNullOrWhiteSpace(lwkeystore.EncodedKeystorePass) || string.IsNullOrWhiteSpace(lwkeystore.Keystore))
				throw new ArgumentNullException("lwkeystore");
			if (string.IsNullOrWhiteSpace(data))
				throw new ArgumentNullException("data");

            // Use the "P" prime number part of the PKI key as the symmetric key
            PrivateKey privKey = lwkeystore.PrivateKey;
			string privKeyXml = privKey.GetXml();
			XElement element = XElement.Parse(privKeyXml);
			string encodedKey = element.Element("P").Value;

			return Decrypt(encodedKey, data, vector);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lwkeystore"></param>
        /// <param name="data"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static string Decryptv2(LWKeystore lwkeystore, string data)
        {
            if (lwkeystore == null || string.IsNullOrWhiteSpace(lwkeystore.SymmectricKey) || string.IsNullOrWhiteSpace(lwkeystore.SymmetricIV))
                throw new ArgumentNullException("lwkeystore");
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException("data");

            //check for service. if none exists, go to old method
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            string baseURL = config.FWConfig.GetFWConfigProperty("EncryptionBaseURL");
            if (string.IsNullOrEmpty(baseURL))
            {
                return Decrypt(lwkeystore, data);
            }

            return EncryptDecryptWebService(data, "Decrypt");
        }

        /// <summary>
        /// Symmetric decryption of the provided data using the provided key.  The key must have been 
        /// encoded using the CryptoUtil.EncodeUTF8() method.
		/// 
		/// Note, if the same initialization vector is used for the same data, the resulting encrypted data
		/// will be the same.  This is convenient for exact matching the encrypted data, but undermines the
		/// strength of the encryption.
        /// </summary>
        /// <param name="encodedKey">base64-encoded key</param>
        /// <param name="data">data to be decrypted</param>
		/// <param name="vector">initialization vector</param>
        /// <returns>decrypted UTF8 string</returns>
		public static string Decrypt(string encodedKey, string data, string vector = "InitializationVector")
        {
            string key = DecodeUTF8(encodedKey);

            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Missing config entry: EncryptionKey");
            if (string.IsNullOrEmpty(vector)) throw new ArgumentException("Missing config entry: InitializationVector");

            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.BlockSize = 256;
            RMCrypto.KeySize = 256;
            RMCrypto.Mode = CipherMode.CBC;
            RMCrypto.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = RMCrypto.CreateDecryptor(GetHash(key), GetHash(vector));
            byte[] rawData = Convert.FromBase64String(data);
            MemoryStream inputStream = new MemoryStream(rawData);
            try
            {
                CryptoStream CryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
                byte[] decrypted = new byte[rawData.Length];
                CryptStream.Read(decrypted, 0, decrypted.Length);
                char[] chars = Encoding.UTF8.GetChars(decrypted);
                StringBuilder sb = new StringBuilder();
                foreach (char c in chars)
                {
                    if (c == 0) // padding mode of zeros appended to the end by block cypher
                    {
                        break;
                    }
                    sb.Append(c);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Decrypt the message: The error was - " + ex.Message);
            }
        }

        public static string GenerateSymmetricIV()
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.BlockSize = 256;
            rm.KeySize = 256;
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.Zeros;
            rm.GenerateIV();

            return BitConverter.ToString(rm.IV);
        }

        public static string GenerateSymmetricKey()
        {
            RijndaelManaged rm = new RijndaelManaged();
            rm.BlockSize = 256;
            rm.KeySize = 256;
            rm.Mode = CipherMode.CBC;
            rm.Padding = PaddingMode.Zeros;
            rm.GenerateKey();

            return BitConverter.ToString(rm.Key);
        }

		private const string MTOUCH_ENCRYPTION_KEY = "surveymagic";

		/// <summary>
		/// Decrypt an MTouch that has been encrypted with CryptoUtil.EncryptMTouch().
		/// </summary>
		/// <param name="mtouch">encrypted MTouch</param>
		/// <returns>decrypted MTouch, or null if encrypted MTouch is invalid</returns>
		public static string DecryptMTouch(string mtouch)
		{
			if (string.IsNullOrEmpty(mtouch)) return null;

			string encodedKey = CryptoUtil.EncodeUTF8(MTOUCH_ENCRYPTION_KEY);
			string result = null;
			try
			{
				result = CryptoUtil.Decrypt(encodedKey, mtouch);
			}
			catch
			{
				// invalid mtouch
			}
			return result;
		}

		/// <summary>
		/// Encrypt an MTouch.
		/// </summary>
		/// <param name="mtouch">MTouch to encrypt</param>
		/// <returns>encrypted MTouch</returns>
		public static string EncryptMTouch(string mtouch)
		{
			if (string.IsNullOrEmpty(mtouch)) return null;

			string encodedKey = CryptoUtil.EncodeUTF8(MTOUCH_ENCRYPTION_KEY);
			string result = null;
			try
			{
				result = CryptoUtil.Encrypt(encodedKey, mtouch);
			}
			catch
			{
				// invalid mtouch
			}
			return result;
		}
        
        private static byte[] GetHash(string data)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
            System.Security.Cryptography.SHA256Managed sha256 = new SHA256Managed();
            byte[] retVal = sha256.ComputeHash(buffer);
            buffer = null;
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="methodType"></param>
        /// <returns></returns>
        private static string EncryptDecryptWebService(string data, string methodType)
        {
            try
            {
                string returnValue = String.Empty;

                LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();

                string urlParameters = String.Empty;

                if (methodType == "Encrypt")
                    urlParameters = "/api/v3/SymmetricEncryption/" + config.Organization + "/" + config.Environment;
                else
                    urlParameters = "/api/v3/SymmetricDecrypt/" + config.Organization + "/" + config.Environment;

                HttpClient client = EncryptDecryptClient;

                var encodedData = WebUtility.UrlEncode(data);

                //HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                HttpResponseMessage response = client.PostAsJsonAsync(urlParameters, encodedData).Result;

                if (response.IsSuccessStatusCode)
                {
                    returnValue = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    throw new LWException(String.Format("Error in {0} process: StatusCode: {1} - {2}", methodType, (int)response.StatusCode, response.ReasonPhrase));
                }

                return methodType == "Encrypt" ? WebUtility.UrlDecode(returnValue) : returnValue;
            }
            catch (Exception ex)
            {
                throw new LWException(String.Format("Error {0}ing the data provided", methodType), ex);
            }
        }

        #endregion

		#region PGP encryption/decryption
		/// <summary>
		/// Used by LoyaltyNavigator to validate uploaded PGP private key files.
		/// </summary>
		/// <param name="privateKeyBytes"></param>
		public static void ValidatePGPPrivateKeyFile(byte[] privateKeyBytes)
		{
			Stream privFis = new MemoryStream(privateKeyBytes);

			// This doesn't throw an exception, but if invalid data, there will be no keys
			PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privFis));
			if (pgpSec.Count < 1)
			{
				throw new LWException("No keys are present");
			}

			// Looks like valid private key file, but go a bit deeper to see if there is a private key there.
			System.Collections.IEnumerator ringEnumerator = pgpSec.GetKeyRings().GetEnumerator();
			bool foundKey = false;
			while (ringEnumerator.MoveNext())
			{
				if (ringEnumerator.Current is PgpSecretKeyRing)
				{
					PgpSecretKeyRing ring = (ringEnumerator.Current as PgpSecretKeyRing);
					if (ring != null)
					{
						System.Collections.IEnumerator keyEnumerator = ring.GetSecretKeys().GetEnumerator();
						while (keyEnumerator.MoveNext())
						{
							PgpSecretKey key = (keyEnumerator.Current as PgpSecretKey);
							if (key != null)
							{
								foundKey = true;
								break;
							}
						}
					}
				}
			}
			if (!foundKey)
			{
				throw new LWException("Can't find any keys in key ring");
			}
			// OK, by now this is definitely valid...
		}

		/// <summary>
		/// Used by LoyaltyNavigator to validate uploaded PGP public key files.
		/// </summary>
		/// <param name="publicKeyBytes"></param>
		public static void ValidatePGPPublicKeyFile(byte[] publicKeyBytes)
		{
			Stream pubFis = new MemoryStream(publicKeyBytes);

			// This throws an exception if the key is invalid
			PgpPublicKeyRing ring = new PgpPublicKeyRing(PgpUtilities.GetDecoderStream(pubFis));

			// go a little deeper, just in case...
			if (ring == null)
			{
				throw new LWException("No key rings are present");
			}
			PgpPublicKey key = ring.GetPublicKey();
			if (key == null)
			{
				throw new LWException("No keys are present");
			}
		}

		/// <summary>
		/// PGP encryption of provided clearData.  Requires PGP properties to be configured for the client.
		/// </summary>
		/// <param name="keyName">pgp encryption key name</param>
		/// <param name="clearData">data to be encrypted</param>
		/// <returns>encrypted data</returns>
		public static byte[] PGPEncrypt(string keyName, byte[] clearData)
		{
			// throws exception if keyName not found
			PGPEncryptionKey props = GetPGPEncryptionKey(keyName);
			if (string.IsNullOrEmpty(props.EncodedPublicKey))
				throw new ArgumentException(string.Format("The PGP public key is missing from the encryption key '{0}'.", keyName));

			// read the public key
			byte[] publicKeyBytes = Decode(props.EncodedPublicKey);
			Stream pubFis = new MemoryStream(publicKeyBytes);
			PgpPublicKeyRing ring = new PgpPublicKeyRing(PgpUtilities.GetDecoderStream(pubFis));
			PgpPublicKey key = null;
			foreach (PgpPublicKey tmpKey in ring.GetPublicKeys())
			{
				if (tmpKey.IsEncryptionKey)
				{
					key = tmpKey;
					break;
				}
			}

			// compress the data
			byte[] compressedData = null;
			if (props.UseCompression)
			{
				compressedData = PGPCompress(clearData, "unused", props.CompressionType);
			}
			else
			{
				compressedData = clearData;
			}

			// encrypt
			MemoryStream bOut = new MemoryStream();
			Stream output = bOut;
			if (props.UseArmor)
			{
				output = new ArmoredOutputStream(output);
			}
			PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(props.SymmetricKeyAlgorithm, props.UseIntegrityPacket, new SecureRandom());
			encGen.AddMethod(key);
			Stream encOut = encGen.Open(output, compressedData.Length);
			encOut.Write(compressedData, 0, compressedData.Length);
			encOut.Close();
			if (props.UseArmor)
			{
				output.Close();
			}

			return bOut.ToArray();
		}

		/// <summary>
		/// PGP file encryption. Requires PGP properties to be configured for the client.
		/// </summary>
		/// <param name="keyName">pgp encryption key name</param>
		/// <param name="fileName">full path to the file to be encrypted</param>
		public static void PGPEncrypt(string keyName, string fileName)
		{
			// throws exception if keyName not found
			PGPEncryptionKey props = GetPGPEncryptionKey(keyName);
			if (string.IsNullOrEmpty(props.EncodedPublicKey))
				throw new ArgumentException(string.Format("The PGP public key is missing from the encryption key '{0}'.", keyName));

			if (!File.Exists(fileName))
			{
				throw new ArgumentException(string.Format("File {0} does not exist.", fileName));
			}

			// read the public key
			byte[] publicKeyBytes = Decode(props.EncodedPublicKey);
			Stream pubFis = new MemoryStream(publicKeyBytes);
			PgpPublicKeyRing ring = new PgpPublicKeyRing(PgpUtilities.GetDecoderStream(pubFis));
			PgpPublicKey key = null;
			foreach (PgpPublicKey tmpKey in ring.GetPublicKeys())
			{
				if (tmpKey.IsEncryptionKey)
				{
					key = tmpKey;
					break;
				}
			}

			// compress the data
			if (props.UseCompression)
			{
				PGPCompress(fileName, props.CompressionType);
			}

			string tempFileName = fileName + "_pgp_tmp";
			int count = 0;
			while (File.Exists(tempFileName))
			{
				tempFileName = fileName + "_pgp_tmp" + count++.ToString();
				if (count > 10000)
					throw new LWException("failed to find unique temporary file name");
			}

			try
			{
				//move unencrypted file to "_pgp_tmp" and create new empty file
				File.Move(fileName, tempFileName);
				
				using (FileStream inFileStream = new FileStream(tempFileName, FileMode.Open))
				{
					using (FileStream outFileStream = new FileStream(fileName, FileMode.OpenOrCreate))
					{
						Stream input = inFileStream;
						Stream output = outFileStream;

						if (props.UseArmor)
						{
							output = new ArmoredOutputStream(output);
						}

						PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(props.SymmetricKeyAlgorithm, props.UseIntegrityPacket, new SecureRandom());
						encGen.AddMethod(key);

						Stream encOut = encGen.Open(output, inFileStream.Length);
						
						input.CopyTo(encOut);

						encOut.Close();
						if (props.UseArmor)
						{
							output.Close();
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}
			}
		}

		/// <summary>
		/// Decrypt PGP message.  Requires PGP properties to be configured for the client.
		/// </summary>
		/// <param name="encryptedData">data to be decrypted</param>
		/// <param name="privateKeyPassword">password for the configured PGP private key</param>
		/// <returns>decrypted data</returns>
		public static byte[] PGPDecrypt(string keyName, byte[] encryptedData, string privateKeyPassword)
		{
			// throws exception if keyName not found
			PGPDecryptionKey props = GetPGPDecryptionKey(keyName);
			if (string.IsNullOrEmpty(props.EncodedPrivateKey))
				throw new ArgumentException(string.Format("The PGP private key is missing from the decryption key '{0}'.", keyName));

			Stream inputStream = new MemoryStream(encryptedData);
			inputStream = PgpUtilities.GetDecoderStream(inputStream);
			PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
			PgpEncryptedDataList enc;
			PgpObject o = pgpF.NextPgpObject();
			//
			// the first object might be a PGP marker packet.
			//
			if (o is PgpEncryptedDataList)
			{
				enc = (PgpEncryptedDataList)o;
			}
			else
			{
				enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
			}

			// find the private key
			byte[] privateKeyBytes = Decode(props.EncodedPrivateKey);
			Stream privFis = new MemoryStream(privateKeyBytes);
			PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privFis));
			PgpPrivateKey privateKey = null;
			PgpPublicKeyEncryptedData pbe = null;
			foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
			{
				PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(pked.KeyId);
				if (pgpSecKey != null)
				{
					privateKey = pgpSecKey.ExtractPrivateKey(privateKeyPassword.ToCharArray());
				}
				if (privateKey != null)
				{
					pbe = pked;
					break;
				}
			}
			if (privateKey == null)
			{
				throw new ArgumentException("private key for message not found.");
			}

			// decrypt
			byte[] result = null;
			Stream clear = pbe.GetDataStream(privateKey);
			PgpObjectFactory plainFact = new PgpObjectFactory(clear);
			PgpObject message = plainFact.NextPgpObject();
			if (message is PgpCompressedData)
			{
				PgpCompressedData cData = (PgpCompressedData)message;
				PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());
				message = pgpFact.NextPgpObject();
			}
			if (message is PgpLiteralData)
			{
				PgpLiteralData ld = (PgpLiteralData)message;
				Stream unc = ld.GetInputStream();
				MemoryStream decrypted = new MemoryStream();
				Streams.PipeAll(unc, decrypted);
				decrypted.Close();
				result = decrypted.ToArray();
			}
			else if (message is PgpOnePassSignatureList)
			{
				// if we supported decrypting signed messages, we'd do that here
				throw new LWException("encrypted message contains a signed message - not literal data.");
			}
			else
			{
				// uunknown/invalid message
				throw new LWException("message is not a simple encrypted file - type unknown.");
			}

			// integrity check
			if (pbe.IsIntegrityProtected())
			{
				if (!pbe.Verify() && props.RequireIntegrityCheckForDecryption)
				{
					throw new LWException("message failed integrity check");
				}
			}
			else if (props.RequireIntegrityCheckForDecryption)
			{
				throw new LWException("message is not integrity protected");
			}

			return result;
		}

		/// <summary>
		/// Get a list of the names of the PGP encryption keys.
		/// </summary>
		/// <returns>list of key names</returns>
		public static List<string> GetPGPEncryptionKeyNames()
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("PGPEncryptionKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no PGP encryption keys configured.");
				}
				PGPEncryptionKeyCollection keys = JsonConvert.DeserializeObject<PGPEncryptionKeyCollection>(keysJSON);
				return keys.KeyList;
			}
		}

		/// <summary>
		/// Get a list of the names of the PGP decryption keys.
		/// </summary>
		/// <returns>list of key names</returns>
		public static List<string> GetPGPDecryptionKeyNames()
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("PGPDecryptionKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no PGP decryption keys configured.");
				}
				PGPDecryptionKeyCollection keys = JsonConvert.DeserializeObject<PGPDecryptionKeyCollection>(keysJSON);
				return keys.KeyList;
			}
		}

		private static PGPEncryptionKey GetPGPEncryptionKey(string keyName)
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("PGPEncryptionKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no PGP encryption keys configured.");
				}
				PGPEncryptionKeyCollection keys = JsonConvert.DeserializeObject<PGPEncryptionKeyCollection>(keysJSON);
				if (!keys.ContainsKey(keyName))
				{
					throw new LWException(string.Format("PGP encryption key '{0}' not found.", keyName));
				}
				return keys[keyName];
			}
		}

		private static PGPDecryptionKey GetPGPDecryptionKey(string keyName)
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("PGPDecryptionKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no PGP decryption keys configured.");
				}
				PGPDecryptionKeyCollection keys = JsonConvert.DeserializeObject<PGPDecryptionKeyCollection>(keysJSON);
				if (!keys.ContainsKey(keyName))
				{
					throw new LWException(string.Format("PGP decryption key '{0}' not found.", keyName));
				}
				return keys[keyName];
			}
		}

		private static byte[] PGPCompress(byte[] clearData, string fileName, CompressionAlgorithmTag algorithm)
		{
			MemoryStream bOut = new MemoryStream();

			PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(algorithm);
			Stream cos = comData.Open(bOut); // open it with the final destination
			PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();

			// we want to Generate compressed data. This might be a user option later,
			// in which case we would pass in bOut.
			Stream pOut = lData.Open(
				cos,					// the compressed output stream
				PgpLiteralData.Binary,
				fileName,				// "filename" to store
				clearData.Length,		// length of clear data
				DateTime.UtcNow			// current time
			);

			pOut.Write(clearData, 0, clearData.Length);
			pOut.Close();

			comData.Close();

			return bOut.ToArray();
		}

		private static void PGPCompress(string fileName, CompressionAlgorithmTag algorithm)
		{
			if (!System.IO.File.Exists(fileName))
			{
				throw new ArgumentException(string.Format("File {0} does not exist.", fileName), "fileName");
			}

			string tempFileName = fileName + "_zip_tmp";
			int count = 0;
			while (System.IO.File.Exists(tempFileName))
			{
				tempFileName = fileName + "_zip_tmp" + count++.ToString();
			}


			try
			{
				System.IO.File.Move(fileName, tempFileName);

				using (FileStream inFileStream = new FileStream(tempFileName, FileMode.Open))
				{
					using (FileStream outFileStream = new FileStream(fileName, FileMode.OpenOrCreate))
					{

						PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(algorithm);
						Stream cos = comData.Open(outFileStream); // open it with the final destination
						PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();

						Stream pOut = lData.Open(
							cos,					// the compressed output stream
							PgpLiteralData.Binary,
							fileName,				// "filename" to store
							inFileStream.Length,	// length of clear data
							DateTime.UtcNow			// current time
						);

						inFileStream.CopyTo(pOut);
						pOut.Close();
						comData.Close();
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (System.IO.File.Exists(tempFileName))
				{
					if (!System.IO.File.Exists(fileName))
					{
						System.IO.File.Move(tempFileName, fileName);
					}
					else
					{
						System.IO.File.Delete(tempFileName);
					}
				}
			}

		}
		#endregion

		#region SFTP keys
		public static void ValidateSFTPKeyFile(byte[] keyBytes, string keyPassword)
		{
			string keyString = Encoding.ASCII.GetString(keyBytes);
			LWPrivateKey key = new LWPrivateKey();
			if (!string.IsNullOrWhiteSpace(keyPassword)) {
				key.Password = keyPassword;
			}
			if (!key.FromPuttyPrivateKey(keyString))
			{
				throw new InvalidSFTPKeyException("Error reading SFTP private key.", key.LastErrorHtml, key.LastErrorText, key.LastErrorXml);
			}
		}

		public static List<string> GetSFTPKeyNames()
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("SFTPKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no SFTP keys configured.");
				}
				SFTPKeyInfoCollection keyInfos = JsonConvert.DeserializeObject<SFTPKeyInfoCollection>(keysJSON);
				return keyInfos.KeyList;
			}
		}

		public static SFTPKeyInfo GetSFTPKeyInfo(string keyName)
		{
			using (var service = LWDataServiceUtil.DataServiceInstance())
			{
				string keysJSON = service.GetClientConfigProp("SFTPKeys");
				if (string.IsNullOrWhiteSpace(keysJSON))
				{
					throw new LWException("There are no SFTP keys configured.");
				}
				SFTPKeyInfoCollection keyInfos = JsonConvert.DeserializeObject<SFTPKeyInfoCollection>(keysJSON);
				if (!keyInfos.ContainsKey(keyName))
				{
					throw new LWException(string.Format("SFTP key '{0}' not found.", keyName));
				}
				return keyInfos[keyName];
			}
		}
		#endregion
	}
}
