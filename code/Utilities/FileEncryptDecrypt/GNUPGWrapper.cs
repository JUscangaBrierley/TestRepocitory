using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace AmericanEagle.Utilities.FileEncryptDecryptBulk
{
    public enum Commands
    {

        // Make a signature
        Sign,

        // Encrypt  data
        Encrypt,

        // Sign and encrypt data
        SignAndEncrypt,

        // Decrypt data
        Decrypt

    }


    public class GnuPGWrapper
    {
        // Variables used to store property values (prefix: underscore "_")
        private Commands _command = Commands.SignAndEncrypt;
        private bool _armor = false;
        private bool _yes = false;
        private bool _output = true;
        private bool _outputDoc = true;
        private string _pwdFile = "";
        private string _recipient = "";
        public string _workdirectory = "";
        public string _workFile = "";
        private string _passphrase = "";
        private string _passphrasefd = "";
        private bool _batch = false;
        private string _originator = "";
        private int _ProcessTimeOutMilliseconds = 10000; // 10 seconds
        private int _exitcode = 0;

        // Variables used for monitoring external process and threads
        private Process _processObject;
        //private string _outputString;
        private string _errorString;

        //Variables used for files
        string strGnuPGInputFileName = "";

        public GnuPGWrapper()
        {
        }


        // Constructor
        //Home directory for GnuPG (where keyrings AND gpg.exe are located)
        public GnuPGWrapper(string workDirectory)
        {
            workdirectory = workDirectory;
        }

        public Commands command
        {
            set
            {
                _command = value;
            }
        }

        public string PWDFile
        {
            set
            {
                _pwdFile = value;
            }
        }

        // Boolean flag: if true, GnuPG creates ASCII armored output (text output). 
        public bool armor
        {
            set
            {
                _armor = value;
            }
        }

        //Recipient email address - mandatory when command is Encrypt 
        //GnuPG uses this parameter to find the associated public key. You must have imported 
        //this public key in your keyring before.
        public string recipient
        {
            set
            {
                _recipient = value;
            }
        }


        // Originator email address - recommended when SignAndEncrypt
        //GnuPG uses this parameter to find the associated secret key. You must have imported 
        //this secret key in your keyring before. Otherwise, GnuPG uses the first secret key 
        //in your keyring to sign messages. This property is mapped to the "--default-key" option.
        public string originator
        {
            set
            {
                _originator = value;
            }
        }

        // Boolean flag; if true, GnuPG assumes "yes" on most questions.
        //Defaults to true.
        public bool yes
        {
            set
            {
                _yes = value;
            }
        }

        // Boolean flag; if true, GnuPG uses batch mode Defaults to true.
        public bool batch
        {
            set
            {
                _batch = value;
            }
        }


        // Passphrase for using your private key - mandatory when SignAndEncrypt.
        public string passphrase
        {
            set
            {
                _passphrase = value;
                if (_passphrase != "")
                {
                    _passphrasefd = "0"; // stdin
                }
                else
                {
                    _passphrasefd = "";
                }
            }
        }


        // name of the home directory (where keyrings AND gpg.exe are located)
        public string workdirectory
        {
            set
            {
                _workdirectory = value;

            }
        }


        // File descriptor for entering passphrase - defaults to 0 (standard input).
        public string passphrasefd
        {
            set
            {
                _passphrasefd = value;
            }
        }


        // Exit code from GnuPG process (0 = success; otherwise an error occured)
        public int exitcode
        {
            get
            {
                return (_exitcode);
            }
        }



        // Timeout for GnuPG process, in milliseconds.
        //If the process doesn't exit before the end of the timeout period, the process is terminated (killed).
        //Defaults to 10000 (10 seconds).
        public int ProcessTimeOutMilliseconds
        {
            get
            {
                return (_ProcessTimeOutMilliseconds);
            }
            set
            {
                _ProcessTimeOutMilliseconds = value;
            }
        }
        protected string BuildOptions()
        {
            StringBuilder optionsBuilder = new StringBuilder("", 255);
            bool recipientNeeded = false;
            bool passphraseNeeded = false;

            if (_output)
            {
                optionsBuilder.Append("--output ");
                optionsBuilder.Append("\"");
                optionsBuilder.Append(_workFile);
                optionsBuilder.Append("\" ");
            }

            // Answer yes to all questions?
            if (_yes)
            {
                optionsBuilder.Append("--yes ");
            }

            // batch mode?
            if (_batch)
            {
                optionsBuilder.Append("--batch ");
            }


            // Passphrase file descriptor?
            if (_passphrasefd != null && _passphrasefd != "")
            {
                optionsBuilder.Append(" --passphrase-fd ");
                optionsBuilder.Append(_passphrasefd);
                optionsBuilder.Append(" ");
            }
            else
            {
                if (passphraseNeeded && (_passphrase == null || _passphrase == ""))
                {
                    throw new Exception("GPGNET: Missing 'passphrase' parameter: cannot sign without a passphrase");
                }
            }

            // ASCII output?
            if (_armor)
            {
                optionsBuilder.Append("--armor ");
            }

            // Recipient?
            if (_recipient != null && _recipient != "")
            {
                optionsBuilder.Append("--recipient ");
                optionsBuilder.Append(_recipient);
                optionsBuilder.Append(" ");
            }

            // Originator?
            if (_originator != null && _originator != "")
            {
                optionsBuilder.Append("--default-key ");
                optionsBuilder.Append(_originator);
                optionsBuilder.Append(" ");
            }

            // Command
            switch (_command)
            {
                case Commands.Sign:
                    optionsBuilder.Append("--sign ");
                    passphraseNeeded = true;
                    break;
                case Commands.Encrypt:
                    optionsBuilder.Append("--encrypt ");
                    recipientNeeded = true;
                    break;
                case Commands.SignAndEncrypt:
                    optionsBuilder.Append("--sign ");
                    optionsBuilder.Append("--encrypt ");
                    recipientNeeded = true;
                    passphraseNeeded = true;
                    break;
                case Commands.Decrypt:
                    //_encryptedFile = inputFileName;
                    //				string[] arrFileName = inputFileName.Split(new char[] {'.'});
                    //				_decryptedFile = _decryptedFilePath + arrFileName[0] + ".txt"; 
                    //				outputFileName = _decryptedFile;

                    optionsBuilder.Append(" --decrypt ");
                    if (_outputDoc)
                    {
                        optionsBuilder.Append("\"");
                        optionsBuilder.Append(strGnuPGInputFileName);
                        optionsBuilder.Append("\"");
                    }

                    break;
            }

            // Recipient?
            if (_recipient == null || _recipient == "")
            {
                // If you encrypt, you NEED a recipient!
                if (recipientNeeded)
                {
                    throw new Exception("GPGNET: Missing 'recipient' parameter: cannot encrypt without a recipient");
                }
            }

            // Passphrase?
            if (_passphrase == null || _passphrase == "")
            {
                if (passphraseNeeded)
                {
                    throw new Exception("GPGNET: Missing 'passphrase' parameter: cannot sign without a passphrase");
                }
            }


            if (Commands.Encrypt == _command || _command == Commands.SignAndEncrypt)
            {
                optionsBuilder.Append("\"");
                optionsBuilder.Append(strGnuPGInputFileName);
                optionsBuilder.Append("\"");
            }

            return (optionsBuilder.ToString());
        }

        public void ExecuteCommand(string inputFileName, string outputFileName, bool SendYes)
        {
            strGnuPGInputFileName = inputFileName;
            string[] arrFileName = inputFileName.Split(new char[] { '.' });
            _workFile = outputFileName;
            if (_command == Commands.Decrypt)
            {
                //					_passphrase = GetPW();
                _passphrasefd = "0";
            }

            string gpgOptions = BuildOptions();
            string gpgExecutable = _workdirectory + (_workdirectory.EndsWith("\\") ? "" : "\\") + "gpg.exe";

            // TODO check existence of _bindirectory and gpgExecutable

            // Create startinfo object
            ProcessStartInfo pInfo = new ProcessStartInfo(gpgExecutable, gpgOptions);
            //pInfo.WorkingDirectory = _workdirectory;
            pInfo.CreateNoWindow = true;
            pInfo.UseShellExecute = false;
            // Redirect everything: 
            // stdin to send the passphrase, stdout to get encrypted message, stderr in case of errors...
            pInfo.RedirectStandardInput = true;
            pInfo.RedirectStandardOutput = true;
            pInfo.RedirectStandardError = true;
            _processObject = Process.Start(pInfo);

            // Send pass phrase, if any
            if (_passphrase != null && _passphrase != "")
            {
                _processObject.StandardInput.WriteLine(_passphrase);
                _processObject.StandardInput.Flush();
            }

            // Send "y" if prompted
            if (SendYes)
            {
                _processObject.StandardInput.WriteLine("y");
                _processObject.StandardInput.Flush();
            }



            //_outputString = "";
            _errorString = "";

            // Create a thread to read both error streams 
            ThreadStart errorEntry = new ThreadStart(StandardErrorReader);
            Thread errorThread = new Thread(errorEntry);
            errorThread.Start();

            // Send "y" if prompted
            if (SendYes)
            {
                _processObject.StandardInput.WriteLine("y");
                _processObject.StandardInput.Flush();
            }

            if (_processObject.WaitForExit(ProcessTimeOutMilliseconds))
            {
                // Process exited before timeout...

                if (!errorThread.Join(ProcessTimeOutMilliseconds / 2))
                {
                    errorThread.Abort();
                }
            }
            else
            {
                // Process timeout: PGP hung somewhere... kill it
                _errorString = "Timed out after " + ProcessTimeOutMilliseconds.ToString() + " milliseconds";
                _processObject.Kill();
                if (errorThread.IsAlive)
                {
                    errorThread.Abort();
                }
            }

            // Check results and prepare output
            _exitcode = _processObject.ExitCode;
            if (_exitcode != 0)
            {
                if (_errorString == "")
                {
                    _errorString = "GPGNET: [" + _processObject.ExitCode.ToString() + "]: Unknown error";
                }
                throw new Exception(_errorString);
            }
        }

        public void StandardErrorReader()
        {
            string error = _processObject.StandardError.ReadToEnd();
            lock (this)
            {
                _errorString = error;
            }
        }


    }
}
