using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace FileCountUtility
{
    public class Processor
    {
        private string m_ToEmailAddress;
        private string m_FromEmailAddress;
        private string m_EmailSubject;
        private string m_EmailBody;
        private string m_EmailClient;
        private string m_FilePath;
        private string m_fileName;
        private string m_fileName2;
        private string m_newFileName;
        private string m_backupFileName;
        private string m_filetype;
        private string m_countInFile;
        // AEO-767 Start -------------- AH
        private string m_braCountInFile;
        private string m_jeanCountInFile;
        private string m_channelType;
        // AEO-767 End ---------------- AH
        private string m_filePrefix;
        private string m_fileSuffix;
        private string m_filePrefix2;
        private string m_fileSuffix2;
        // AEO-889 Start -------------- AH
        private string m_jeanCampaignID;
        private string m_braCampaignID;
        private string m_fiveRewardCampaignID;
        // AEO-889 End ---------------- AH

        // AEO-926 Start -------------- AH
        private bool m_birthday_flag = false;
        private bool m_aecc_flag = false;
        private string m_fileCount1;
        private string m_fileCount2;
        private string m_formattedText;
        // AEO-926 End ---------------- AH

        // AEO-1719 Start ------------- AH
        private Dictionary<string, int> rewardCounts = new Dictionary<string,int>();
        private string[] RewardTypes =
        {
            "",                 // skip index 0
            "$10 Off Rewards",  // Reward Type 1
            "$15 Off Rewards",  // Reward Type 2
            "$20 Off Rewards",  // Reward Type 3
            "$30 Off Rewards",  // Reward Type 4
            "$40 Off Rewards",  // Reward Type 5
            "$45 Off Rewards",  // Reward Type 6
            "$50 Off Rewards",  // Reward Type 7
            "$60 Off Rewards",  // Reward Type 8
            "B5G1 Bra Rewards", // Reward Type 9 NOTE: The reason there's a space after is to make it look nice in the email.
            "B5G1 Jean Rewards" // Reward Type 10
        };
        // AEO-1719 End --------------- AH

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("usage: -count -fileprefix <fileprefix> -filesuffix <filesuffix>");
                Console.WriteLine("or: -countemail -filetype <filetype> -countinfile <yes or no> -fileprefix <fileprefix> -filesuffix <filesuffix> -fileprefix2 <fileprefix> -filesuffix2 <filesuffix> -jeanID <jean_campaign_id> -braID <bra_campaign_id> -5RewardID <5Reward_CampaignID>");
                Console.WriteLine("or: -rewardbackup -fileprefix <fileprefix> -filesuffix <filesuffix>");
                return;
            }
            Processor process = new Processor();


            switch (args[0])
            {
                case "-count":
                    //process.m_fileName = process.m_FilePath + @"\" + args[2];
                    process.m_filePrefix = args[2];
                    process.m_fileSuffix = args[4];
                    process.CountAndRename(process.m_filePrefix, process.m_fileSuffix);
                    break;
                case "-countemail":
                    //process.m_fileName = process.m_FilePath + @"\" + args[6];
                    process.m_filetype = args[2];
                    process.m_countInFile = args[4].ToUpper();
                    process.m_filePrefix = args[6];

                    // AEO 767 File passed into job requires stripping the suffix off of the prefix. 
                    if (process.m_filePrefix.Contains("." + process.m_fileSuffix))
                    {
                        process.m_filePrefix = process.m_filePrefix.Remove(process.m_filePrefix.IndexOf("." + process.m_fileSuffix));
                    }
                    // AEO 767 - AH

                    process.m_fileSuffix = args[8];
                    if (args.Length > 9)
                    {
                        //process.m_fileName2 = process.m_FilePath + @"\" + args[8];
                        for (int i = 8; i < args.Length; ++i)
                        {
                            switch (args[i])
                            {
                                case "-jeanID":
                                    process.m_jeanCampaignID = args[i + 1];
                                    break;
                                case "-braID":
                                    process.m_braCampaignID = args[i + 1];
                                    break;
                                case "-5RewardID":
                                    process.m_fiveRewardCampaignID = args[i + 1];
                                    break;
                                case "-filePrefix2":
                                    process.m_filePrefix2 = args[i + 1];
                                    break;
                                case "-fileSuffix2":
                                    process.m_fileSuffix2 = args[i + 1];
                                    break;
                                default:
                                    break;
                            }
                        }
                        //process.m_filePrefix2 = args[10];
                        //process.m_fileSuffix2 = args[12];
                    }
                    process.CountAndEmail();
                    break;
                case "-rewardbackup":
                    process.m_fileName = process.m_FilePath + @"\" + args[2];
                    process.RewardBackup();
                    break;
                default:
                    Console.WriteLine("Command not recognized");
                    break;
            }
        }
        public Processor()
        {
            if (null == ConfigurationManager.AppSettings["FilePath"])
            {
                throw new Exception("FilePath key is not found in app.config.");
            }
            m_FilePath = ConfigurationManager.AppSettings["FilePath"];

        }
        private void CountAndRename(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = CountFile(filePrefix, fileSuffix);

            CopyFile(numberOfRecords);

        }
        private void CountAndEmail()
        {
            Console.WriteLine("Count and Email");
            if (null == ConfigurationManager.AppSettings[m_filetype + "Email"])
            {
                throw new Exception(m_filetype + "Email key is not found in app.config.");
            }
            if (null == ConfigurationManager.AppSettings[m_filetype + "Subject"])
            {
                throw new Exception(m_filetype + "Subject key is not found in app.config.");
            }
            if (null == ConfigurationManager.AppSettings[m_filetype + "Body"])
            {
                throw new Exception(m_filetype + "Body key is not found in app.config.");
            }
            if (null == ConfigurationManager.AppSettings["EmailClient"])
            {
                throw new Exception("EmailClient key is not found in app.config.");
            }
            if (null == ConfigurationManager.AppSettings["FromEmail"])
            {
                throw new Exception("FromEmail key is not found in app.config.");
            }
            m_ToEmailAddress = ConfigurationManager.AppSettings[m_filetype + "Email"];
            m_EmailSubject = ConfigurationManager.AppSettings[m_filetype + "Subject"];
            m_EmailBody = ConfigurationManager.AppSettings[m_filetype + "Body"];
            m_EmailClient = ConfigurationManager.AppSettings["EmailClient"];
            m_FromEmailAddress = ConfigurationManager.AppSettings["FromEmail"];

            GetFileName(m_filePrefix, m_fileSuffix);
            Console.WriteLine("File1: " + m_fileName);

            // AEO-767 Start --------------------------------------------------------- AH
            int numberOfRecords = 0;

            switch(m_filetype)
            {
                case "B5G1":
                    numberOfRecords = B5G1_CountFile(m_filePrefix, m_fileSuffix);
                    break;
                case "AECC":                                                                /* AEO-931 AH */
                case "5Reward":
                    numberOfRecords = FiveRewardCount(m_filePrefix, m_fileSuffix);
                    break;
                case "Reminder":
                    numberOfRecords = ReminderCount(m_filePrefix, m_fileSuffix);            /* AEO-926 AH */
                    break;
                case "Birthday":
                    numberOfRecords = BirthdayCount(m_filePrefix, m_fileSuffix);            /* AEO-926 AH */
                    break;
                case "Reward":
                    rewardCounts = RewardCount(m_filePrefix, m_fileSuffix);
                    break;
                default:
                    numberOfRecords = CountFile(m_filePrefix, m_fileSuffix);
                    break;
            }
            // AEO-767 End ----------------------------------------------------------- AH

            if (m_countInFile == "YES")
            {
                Console.WriteLine("Add Counts to filename: " + m_fileName);
                CopyFile(numberOfRecords);
                m_fileName = m_newFileName;
                Console.WriteLine("new filename: " + m_fileName);
            }

            m_EmailBody = m_EmailBody.Replace("@filename1", m_fileName.Replace(m_FilePath+@"\", string.Empty));
            m_EmailBody = m_EmailBody.Replace("@filecount1", numberOfRecords.ToString());

            // AEO-1719 Start -- AH
            if (m_filetype.CompareTo("Reward") == 0)
            {
                int b5g1_total = 0, dollar_total = 0, dollar_idx = 0;
                String reward_data = "";
                foreach(KeyValuePair<string, int> entry in rewardCounts.Skip(1)) 
                {
                    // Format string -> <RewardName> - <RewardCount>
                    reward_data += String.Format("{0} - {1}\n", entry.Key, entry.Value);
                    if ( RewardTypes.Skip(1).Take(8).Contains(entry.Key) )
                    {
                        dollar_total += entry.Value;
                        dollar_idx++;
                    }
                    else
                    {
                        b5g1_total += entry.Value;
                    }

                    if( dollar_idx == 8 )
                    {
                        reward_data += String.Format("\n$ Off Totals - {0}\n\n", dollar_total);
                        dollar_idx++;
                    }
                }
                m_EmailBody = m_EmailBody.Replace("@rewardTotal", (dollar_total + b5g1_total).ToString());
                m_EmailBody = m_EmailBody.Replace("@reward_data", reward_data);
            }
            // AEO-1719 End -- AH
            
            // AEO-767 Start --------------------------------------------------------- AH
            if (m_filetype.CompareTo("B5G1") == 0)
            {
                Console.WriteLine("Changing Jean and Bra Counts...");
                m_EmailBody = m_EmailBody.Replace("@bra_count", m_braCountInFile);
                m_EmailBody = m_EmailBody.Replace("@jean_count", m_jeanCountInFile);
            }
            else if (m_filetype.CompareTo("Reminder") == 0)                                 /* AEO-926 AH */
            {
                m_EmailBody = m_EmailBody.Replace("@count_breakdown", m_formattedText);
            }
            else if (m_filetype.CompareTo("Birthday") == 0)
            {
                m_EmailBody = m_EmailBody.Replace("@bd_count1", m_fileCount1);
                m_EmailBody = m_EmailBody.Replace("@bd_count2", m_fileCount2);
            }                                                                               /* AEO-926 AH */


            if (m_filePrefix.Contains("_EM_"))
            {
                m_EmailBody = m_EmailBody.Replace("@channel_type", "EM");
                m_ToEmailAddress = ConfigurationManager.AppSettings["EM_"+m_filetype + "Email"];
                m_EmailSubject = ConfigurationManager.AppSettings["EM_" + m_filetype + "Subject"];  // For testing purposes only.
            }
            else if (m_filePrefix.Contains("_SM_"))
            {
                m_EmailBody = m_EmailBody.Replace("@channel_type", "SMS");
                m_ToEmailAddress = ConfigurationManager.AppSettings["SM_" + m_filetype + "Email"];
                m_EmailSubject = ConfigurationManager.AppSettings["SM_" + m_filetype + "Subject"];  // For testing purposes only.
            }
            else if (m_filePrefix.Contains("_DM_"))
            {
                m_EmailBody = m_EmailBody.Replace("@channel_type", "DM");
                m_ToEmailAddress = ConfigurationManager.AppSettings["DM_" + m_filetype + "Email"];
                m_EmailSubject = ConfigurationManager.AppSettings["DM_" + m_filetype + "Subject"];  // For testing purposes only.
            }
            // AEO-767 End ----------------------------------------------------------- AH

            if (m_filePrefix2 != null && m_filePrefix2.Length > 0)
            {
                GetFileName(m_filePrefix2, m_fileSuffix2);
                Console.WriteLine("File2: " + m_fileName);
                numberOfRecords = CountFile(m_filePrefix2, m_fileSuffix2);
                m_EmailBody = m_EmailBody.Replace("@filename2", m_fileName.Replace(m_FilePath + @"\", string.Empty));
                m_EmailBody = m_EmailBody.Replace("@filecount2", numberOfRecords.ToString());
            }

            Console.WriteLine("Send Email to : " + m_ToEmailAddress);
            SendEmail_SMTP(m_FromEmailAddress, m_ToEmailAddress, m_EmailSubject, m_EmailBody);

            Console.WriteLine("Process Complete");

        }
        private void RewardBackup()
        {
            DateTime prevQuarterDate = DateTime.Now.AddMonths(-1);
            string quarter = string.Empty;
            string year = string.Empty;

            switch(prevQuarterDate.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarter = "1";
                    break;
                case 4:
                case 5:
                case 6:
                    quarter = "2";
                    break;
                case 7:
                case 8:
                case 9:
                    quarter = "3";
                    break;
                case 10:
                case 11:
                case 12:
                    quarter = "4";
                    break;
                default:
                    quarter = "1";
                    break;
            }
            m_newFileName =  m_fileName.Replace("Qxx_yyyy",  "Q" + quarter + "_" + prevQuarterDate.Year.ToString());
            m_backupFileName = m_fileName.Replace(".txt",".bak");

            CopyFile(0);
        }
        private void GetFileName(string filePrefix, string fileSuffix)
        {
            string[] inputfiles = null;

            inputfiles = System.IO.Directory.GetFiles(m_FilePath, filePrefix + "*." + fileSuffix);

            if (inputfiles.Length > 0)
            {
                m_fileName = m_FilePath + @"\" + inputfiles[0].Remove(0, m_FilePath.Length);
            }
            else
            {
                throw new Exception("File does not exist.\nFile Path: " + m_FilePath + "\nFileName: " + filePrefix + "*." + fileSuffix);
            }
            
        }
        private void CopyFile(int numberOfRecords)
        {

            if (m_fileName.EndsWith(".txt"))
            {
                if (numberOfRecords > 0)
                {
                    m_newFileName = m_fileName.Replace(".txt", "_" + numberOfRecords.ToString() + ".txt");
                }
                m_backupFileName = m_fileName.Replace(".txt", ".bak");
            }
            if (m_fileName.EndsWith(".csv"))
            {
                if (numberOfRecords > 0)
                {
                    m_newFileName = m_fileName.Replace(".csv", "_" + numberOfRecords.ToString() + ".csv");
                }
                m_backupFileName = m_fileName.Replace(".csv", ".bak");
            }

            if (File.Exists(m_backupFileName))
            {
                File.Delete(m_backupFileName);
            }

            File.Copy(m_fileName, m_newFileName);
            File.Copy(m_fileName, m_backupFileName);
            File.Delete(m_fileName);
        }
        private int CountFile(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = 0;
            string recordIn = string.Empty;
            GetFileName(filePrefix, fileSuffix);

            StreamReader sr = new StreamReader(m_fileName);
            recordIn = sr.ReadLine();

            while (true)
            {
                if (recordIn == null)
                {
                    sr.Close();
                    break;
                }
                numberOfRecords++;
                recordIn = sr.ReadLine();
            }


            return numberOfRecords;
        }

        // AEO-1719 Start -- AH
        private Dictionary<string, int> RewardCount(string filePrefix, string fileSuffix)
        {
            Dictionary<string, int> rewardCounts = new Dictionary<string, int>();
            int numberOfRecords = 0;
            int rewardType;
            string recordIn = string.Empty;
            string rewardName;
            GetFileName(filePrefix, fileSuffix);

            // Initialize dictionary values
            foreach(string key in RewardTypes)
            {
                rewardCounts[key] = 0;
            }

            StreamReader sr = new StreamReader(m_fileName);
            string[] tokens;
            recordIn = sr.ReadLine();

            while ((recordIn = sr.ReadLine()) != null)
            {
                tokens = recordIn.Split('|');
                int.TryParse(tokens[3], out rewardType);
                rewardCounts[RewardTypes[rewardType]] += 1;
                numberOfRecords++;
            }

            return rewardCounts;
        }

        // AEO-767 Start --------------------------------------------------------- AH
        // Count Number of records in the 5Reward file.
        private int FiveRewardCount(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = 0;
            string recordIn = string.Empty;
            GetFileName(filePrefix, fileSuffix);

            StreamReader sr = new StreamReader(m_fileName);
            string[] tokens;
            recordIn = sr.ReadLine();

            while ((recordIn = sr.ReadLine()) != null)
            {
                
                tokens = recordIn.Split('|');

                // If the row is a header, skip it.
                if (tokens[0].CompareTo("DELIVERY_CHANNEL") == 0)
                    continue;
                // Get the channel type of the records to determine message delivery method.
                else if (numberOfRecords == 0)
                    m_channelType = tokens[0];

                numberOfRecords++;
            }

            return numberOfRecords; 
        }

        /* ------------------------- AEO - 926 Start ----------------------------------- */
        // Oddly enough this doesn't go by credit vs non-credit
        private int ReminderCount(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = 0;
            string recordIn = string.Empty;
            // key = AUTH_CD, value = count
            Dictionary<string, int> codes = new Dictionary<string,int>();

            int auth_index = -1;

            GetFileName(filePrefix, fileSuffix);

            StreamReader sr = new StreamReader(m_fileName);
            string[] tokens = sr.ReadLine().Split('|');

            for (int x = 0; x < tokens.Length; ++x)
            {
                if (tokens[x].CompareTo("AUTH_CD") == 0)
                {
                    auth_index = x;
                    break;
                }
                else if (x == tokens.Length - 1)
                {
                    Console.WriteLine(m_fileName + ": missing AUTH_CD in file header");
                    return numberOfRecords;
                }

            }
            
            // helper variables
            int v = 0;
            string t;
            
            // Read the file and get a count for each auth_code
            while ((recordIn = sr.ReadLine()) != null)
            {
                tokens = recordIn.Split('|');

                if (numberOfRecords == 0)
                {
                    m_channelType = tokens[0];
                }
                
                t = tokens[auth_index];
                
                /* 
                 * If the key doesn't exist then v = 0. In which case we set
                 * the auth_code's count to be 1. If it does exists then
                 * we incrament the value.
                 */
                codes.TryGetValue(t, out v);
                v = v == 0 ? codes[t] = 1 : codes[t]++;

                numberOfRecords++;
            }

            // Create the formatted text that gets inserted into the email.
            foreach( string k in codes.Keys )
            {
                m_formattedText += k + ": " + codes[k] + "\n";
            }

            return numberOfRecords;
        } // End ReminderCount

        private int BirthdayCount(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = 0;
            string recordIn = string.Empty;
            int credit = 0, non_credit = 0;
            int card_type_index = -1;

            GetFileName(filePrefix, fileSuffix);

            StreamReader sr = new StreamReader(m_fileName);
            string[] tokens = sr.ReadLine().Split('|');

            for (int x = 0; x < tokens.Length; ++x)
            {
                if (tokens[x].CompareTo("CARD_TYPE") == 0)
                {
                    card_type_index = x;
                    break;
                }
                else if (x == tokens.Length - 1)
                {
                    Console.WriteLine(m_fileName + ": missing CARD_TYPE in file header");
                    return numberOfRecords;
                }
            }

            while ((recordIn = sr.ReadLine()) != null)
            {
                tokens = recordIn.Split('|');

                if (numberOfRecords == 0)
                {
                    m_channelType = tokens[0];
                }

                switch(tokens[card_type_index])
                {
                    case "":            //if it's empty or 0 then it's a non-credit member
                    case "0":
                        non_credit++;
                        break;
                    default:
                        credit++;
                        break;
                }
                numberOfRecords++;
            }
            
            m_fileCount1 = non_credit.ToString();
            m_fileCount2 = credit.ToString();

            return numberOfRecords;
        } //End BirthdayCount

        /* ------------------------- AEO - 926 End ------------------------------------- */

        // Record breakdown of B5G1
        private int B5G1_CountFile(string filePrefix, string fileSuffix)
        {
            int numberOfRecords = 0;
            string recordIn = string.Empty;
            int bra_count = 0, jean_count = 0;
            int campaign_id = -1;
            GetFileName(filePrefix, fileSuffix);

            StreamReader sr = new StreamReader(m_fileName);
            string[] tokens = sr.ReadLine().Split('|');
            
            // Get the column index of CAMPAIGN_ID to better identify Class Codes
            for( int x = 0; x < tokens.Length ; ++x )
            {
                if (tokens[x].CompareTo("CAMPAIGN_ID") == 0)
                {
                    campaign_id = x;
                    break;
                }
            }
            // If the column isn't in the file then we'll have a problem.
            if (campaign_id == -1)
            {
                Console.WriteLine(m_fileName + ": missing index for CAMPAIGN_ID");
                m_braCountInFile = bra_count.ToString();
                m_jeanCountInFile = jean_count.ToString();
                return numberOfRecords;
            }

            while ((recordIn = sr.ReadLine()) != null)
            {
                tokens = recordIn.Split('|');

                // Gets the Delivery Channel type. Either a 1,2,3 (EM,SM,DM)
                if (numberOfRecords == 0)
                    m_channelType = tokens[0];

                // Check for bra codes based on campaign id. BRAS003 -> DM files only                                   AEO-889 AH
                if (tokens[campaign_id].CompareTo("BRAS003") == 0 || tokens[campaign_id].CompareTo(m_braCampaignID) == 0)
                {
                    bra_count++;
                    numberOfRecords++;
                }
                // Check for jean codes based on campaign id. JEANS004 -> DM files only.                                AEO-889 AH
                else if (tokens[campaign_id].CompareTo("JEANS004") == 0 || tokens[campaign_id].CompareTo(m_jeanCampaignID) == 0)
                {
                    jean_count++;
                    numberOfRecords++;
                }
            }
            
            // Set bra and jean counts
            m_braCountInFile = bra_count.ToString();
            m_jeanCountInFile = jean_count.ToString();

            return numberOfRecords;
        }
        // AEO-767 End ----------------------------------------------------------- AH

        private void SendEmail_SMTP(string from, string to, string subject, string body)
        {
            string[] toEmailAddresses;

            try
            {
                if(to.Contains(";"))
                {
                    toEmailAddresses = to.Split(';');
                }
                else
                {
                    toEmailAddresses = new string[1];
                    toEmailAddresses[0] = to;
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                foreach (string email in toEmailAddresses)
                {
                    mail.To.Add(email);
                }
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;

                mail.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient(m_EmailClient);
                client.Send(mail);
            }
            catch (Exception e)
            {
                throw new Exception("SendEmail_SMTP: " + e.Message);
            }

        }

    }
}
