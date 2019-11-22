using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Brierley.FrameWork.Common.Exceptions;
using Chilkat;

namespace Brierley.FrameWork.Common.Config
{
    public class LWKeystore
    {
        private string _encodedKeystorePass = string.Empty;
        private string _keystore = string.Empty;
        private string _symmetricIV = null;
        private string _symmetrickeystore = null;
        private long _keySize = 0;

        private PrivateKey _privateKey = null;
        private PublicKey _publicKey = null;
        private RSACryptoServiceProvider _privateCryptoProvider = null;
        private RSACryptoServiceProvider _publicCryptoProvider = null;

        /// <summary>
        /// Adding a new constructor to be able to 
        /// </summary>
        /// <param name="encodedKeystorePass"></param>
        /// <param name="keystore"></param>
        /// <param name="symmetricIV"></param>
        /// <param name="SymmetricKeyStore"></param>
        public LWKeystore(string encodedKeystorePass, string keystore, string symmetricIV, string SymmetricKeyStore, long keySize)
        {
            _encodedKeystorePass = encodedKeystorePass;
            _keystore = keystore;
            _symmetricIV = symmetricIV;
            _symmetrickeystore = SymmetricKeyStore;
            _keySize = keySize;
        }

        public string EncodedKeystorePass
        {
            get { return _encodedKeystorePass; }
        }

        public string Keystore
        {
            get { return _keystore; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SymmetricIV
        {
            get { return _symmetricIV; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SymmectricKey
        {
            get { return _symmetrickeystore; }
        }

        public long KeySize
        {
            get { return _keySize; }
        }

        public PrivateKey PrivateKey
        {
            get
            {
                if(_privateKey == null)
                {
                    string keystorePass = CryptoUtil.DecodeUTF8(EncodedKeystorePass);
                    PrivateKey privKey = new PrivateKey();
                    if (!privKey.LoadEncryptedPem(Keystore, keystorePass))
                        throw new LWException("Error loading private key: " + privKey.LastErrorText);
                    _privateKey = privKey;
                }
                return _privateKey;
            }
        }

        public PublicKey PublicKey
        {
            get
            {
                if(_publicKey == null)
                {
                    string pubKeyXml = PrivateCryptoProvider.ToXmlString(false);
                    PublicKey pubKey = new PublicKey();
                    if (!pubKey.LoadXml(pubKeyXml))
                        throw new LWException("Error loading public key: " + pubKey.LastErrorText);
                    _publicKey = pubKey;
                }
                return _publicKey;
            }
        }

        public RSACryptoServiceProvider PrivateCryptoProvider
        {
            get
            {
                if(_privateCryptoProvider == null)
                {
                    string privKeyXml = PrivateKey.GetXml();
                    RSACryptoServiceProvider provider = new RSACryptoServiceProvider((int)KeySize);
                    provider.FromXmlString(privKeyXml);
                    _privateCryptoProvider = provider;
                }
                return _privateCryptoProvider;
            }
        }

        public RSACryptoServiceProvider PublicCryptoProvider
        {
            get
            {
                if(_publicCryptoProvider == null)
                {
                    string pubKeyXml = PublicKey.GetXml();
                    RSACryptoServiceProvider encryptor = new RSACryptoServiceProvider((int)KeySize);
                    encryptor.FromXmlString(pubKeyXml);
                    _publicCryptoProvider = encryptor;
                }
                return _publicCryptoProvider;
            }
        }
    }
}
