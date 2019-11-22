using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Configuration;

//using Brierley.FrameWork.Common.Config;
//using Brierley.FrameWork.Common.Logging;
//using Brierley.FrameWork.Data;
//using Brierley.FrameWork.Data.DomainModel;

namespace AmericanEagle.Utilities.FileEncryptDecryptBulk
{
    public class Processor
    {
        /// <summary>
        /// Data service instance
        /// </summary>
        //private ILWDataService dataService;

        /// <summary>
        /// Logger instance
        /// </summary>
        //private LWLogger logger;

        private string m_Recipient;
        private string m_GnuPGPath;
        private string m_GnuPWD;
        private string m_DecryptPath;
        private string m_EncryptPath;
        private string brierleyKey = string.Empty;

        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("usage: -encrypt -key <pgp key> -path <pathname>");
                Console.WriteLine("or: -decrypt -key <pgp key> -path <pathname>");
                return;
            }
            Processor process = new Processor();

            switch (args[0])
            {
                case "-encrypt":
                    process.EncryptFile(args[2], args[4]);
                    break;
                case "-decrypt":
                    process.DecryptFiles(args[2], args[4]);
                    break;
                default:
                    Console.WriteLine("Command not recognized");
                    break;
            }

        }
        public Processor()
        {
            // Checking Output path key
            //if (null == ConfigurationManager.AppSettings["OrganizationName"])
            //{
            //    throw new Exception("OrganizationName key is not found in app.config.");
            //}

            //if (null == ConfigurationManager.AppSettings["Environment"] )
            //{
            //    throw new Exception("Environment key is not found in app.config.");
            //}
            if (null == ConfigurationManager.AppSettings["GnuPGPath"])
            {
                throw new Exception("GnuPGPath key is not found in in app.config.");
            }
            m_GnuPGPath = ConfigurationManager.AppSettings["GnuPGPath"];
            string encryptedPassword = Encrypt("AE Production", true);
            string decryptedPassword = Decrypt(encryptedPassword, true);
            //"BTatungXlt1hWUXj7Hx1nw=="
            //"ZIzLWyoqRrW1P36P7EWR6w=="


            //Console.WriteLine("Initializing Framework...");
            //LWConfigurationUtil.SetCurrentEnvironmentContext(ConfigurationManager.AppSettings["OrganizationName"], ConfigurationManager.AppSettings["Environment"]);
            //LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            //dataService = LWDataServiceUtil.DataServiceInstance(true);
            //logger = LWLoggerManager.GetLogger("FileEncryptDecryptBulk");
            //brierleyKey = Brierley.FrameWork.Common.CryptoUtil.Encode("bri3rl3y");

        }

        public void EncryptFile(string encryptKey, string path)
        {
            string recipientKey = encryptKey.ToLower() + "_rec";
            string fileName = string.Empty;


            if (null == ConfigurationManager.AppSettings[recipientKey])
            {
                throw new Exception(recipientKey + " key is not found in app.config.");
            }
            string recipient = ConfigurationManager.AppSettings[recipientKey];

            string[] filenames = System.IO.Directory.GetFiles(path, "*.*");

            m_EncryptPath = path;

            foreach (string file in filenames)
            {
                fileName = file.Replace(path + @"\", string.Empty);
                Console.WriteLine("Encrypting File: " + fileName);
                GnuPGEncryptDecrypt(Commands.Encrypt, recipient, string.Empty, fileName, fileName + ".pgp");
            }

        }
        private bool IsPrivate(string key)
        {
            bool returnValue = false;

            switch (key)
            {
                case "aetobrierley":
                    returnValue = true;
                    break;
                case "vendortobrierley":
                    returnValue = true;
                    break;
                default:
                    returnValue = false;
                    break;
            }
            return returnValue;
        }
        public void DecryptFiles(string decryptKey, string path)
        {
            decryptKey = decryptKey.ToLower();
            string passphraseKey = decryptKey + "_pwd";
            string passPhrase = string.Empty;
            string recipientKey = decryptKey + "_rec";
            string fileName = string.Empty;

            if (null == ConfigurationManager.AppSettings[recipientKey])
            {
                throw new Exception(recipientKey + " key is not found in app.config.");
            }
            string recipient = ConfigurationManager.AppSettings[recipientKey];

            if (IsPrivate(decryptKey))
            {
                if (null == ConfigurationManager.AppSettings[passphraseKey])
                {
                    throw new Exception(passphraseKey + " key is not found in app.config.");
                }
                passPhrase = ConfigurationManager.AppSettings[passphraseKey];
            }
            string password = decryptString(passPhrase);


            string[] filenames = System.IO.Directory.GetFiles(path, "*.*");

            m_DecryptPath = path;

            foreach (string file in filenames)
            {
                fileName = file.Replace(path+@"\", string.Empty);
                Console.WriteLine("Decrypting File: " + fileName);
                GnuPGEncryptDecrypt(Commands.Decrypt, recipient, password, fileName, fileName.Replace(".pgp", string.Empty));
            }
        }
        public void GnuPGEncryptDecrypt(Commands cmd, string recipient, string passPhrase, string strInFileName, string strOutFileName)
        {

            try
            {
                //Console.WriteLine("GnuPGEncryptDecrypt: Begin" );
                // Create GnuPG wrapping class
                GnuPGWrapper gpg = new GnuPGWrapper();

                // Set parameters from BrierleyConfig file
                if (Commands.Encrypt == cmd)
                    gpg.recipient = recipient;

                if (cmd == Commands.Decrypt)
                {
                    strInFileName = m_DecryptPath + @"\" + strInFileName;
                    strOutFileName = m_DecryptPath + @"\Decrypted\" + strOutFileName;
                }
                else
                {
                    strInFileName = m_EncryptPath + @"\" + strInFileName;
                    strOutFileName = m_EncryptPath + @"\Encrypted\" + strOutFileName;
                }
                //Console.WriteLine("GnuPGEncryptDecrypt: Set Parms");
                //Console.WriteLine("GnuPGEncryptDecrypt: working directory - " + m_GnuPGPath);
                gpg.workdirectory = m_GnuPGPath;
                gpg.command = cmd;
                gpg.passphrase = passPhrase;
                gpg.ProcessTimeOutMilliseconds = 240000; // 4 minutes
                gpg.yes = true;
                //Console.WriteLine("GnuPGEncryptDecrypt: Execute Command");
                gpg.ExecuteCommand(strInFileName, strOutFileName, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Can't check signature") > 0)
                {
                    return;
                }
                else if (ex.Message.IndexOf("invalid packet") > 0)
                {
                    //"gpg: key 82900964: secret key without public key - skipped\r\ngpg: encrypted with 1024-bit ELG-E key, ID 09ACF997, created 2004-07-08\r\n      \"American Eagle Program (Public key generated for American Eagle / Brierley secure communications) <rgepfert@brierley.com>\"\r\ngpg: Error creating `C:\\devroot\\AmericanEagle\\files\\Decrypted\\ddbp_data_201404110600.txt': No such file or directory\r\n"
                    //StreamWriter sw = new StreamWriter("FileEncryptDescrypt.log", true);
                    //sw.WriteLine(DateTime.Now.ToString("mm/dd/yyyy hh:mm:ss") + " - File is Corrupt");
                    //sw.Close();
                //    EventLog ev = null;
                //    ev = new EventLog("Application", ".", m_SourceCode);
                //    ev.WriteEntry("GNUPGencrypt - File is Corrupt", EventLogEntryType.Error, 1);
                //    LogError(strInFileName, "GNUPGencrypt - File is Corrupt");
                    Console.WriteLine("GnuPGEncryptDecrypt: Error: " + ex.Message);
                    //logger.Error("FileEncryptDecryptBulk", "GnuPGEncryptDecrypt", "FileName: " + strInFileName); 
                    //logger.Error("FileEncryptDecryptBulk", "GnuPGEncryptDecrypt", ex.Message);
                }
                else
                {
                    //StreamWriter sw = new StreamWriter("FileEncryptDescrypt.log", true);
                    //sw.WriteLine(DateTime.Now.ToString("mm/dd/yyyy hh:mm:ss") + " - " + ex.Message);
                    //sw.Close();
                //    EventLog ev = null;
                //    ev = new EventLog("Application", ".", m_SourceCode);
                //    ev.WriteEntry(ex.Message, EventLogEntryType.Error, 1);
                //    LogError(strInFileName, "GNUPGencrypt - " + ex.Message);
                    Console.WriteLine("GnuPGEncryptDecrypt: Error: " + ex.Message);
                    //logger.Error("FileEncryptDecryptBulk", "GnuPGEncryptDecrypt", "FileName: " + strInFileName);
                    //logger.Error("FileEncryptDecryptBulk", "GnuPGEncryptDecrypt", ex.Message);
                }
            }
        }

        public string encryptString(String dataString)
        {

            //string encryptedString = Brierley.FrameWork.Common.CryptoUtil.Encrypt(key, dataString);
            string encryptedString = Encrypt(dataString, true);
            return encryptedString;
        }

        public string decryptString(String dataString)
        {
            //string decryptedString = Brierley.FrameWork.Common.CryptoUtil.Decrypt(key, dataString);
            string decryptedString = Decrypt(dataString, true);
            return decryptedString;
        }
        public string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey",
                                                         typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            string key = (string)settingsReader.GetValue("SecurityKey",
                                                             typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}
