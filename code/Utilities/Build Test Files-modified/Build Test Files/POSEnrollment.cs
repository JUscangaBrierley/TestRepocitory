using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Configuration;


namespace BuildTestFiles
{
    public class POSEnrollment
    {
        private ArrayList lstItems;

        private string GetFieldValue(List<string> row, ref int columnNumber)
        {
            string returnValue;

            returnValue = row[columnNumber].Trim();
            ++columnNumber;

            return returnValue;
        }
        public bool ProcessExcelRecord(List<string> row, int rowNumber, out POSEnrollmentRecordClass singleRec, ref int numberOfRecords)
        {
            bool returnVal = false;
            int columnNumber = 0;
            singleRec = null;


            try
            {
                returnVal = true;
                singleRec = new POSEnrollmentRecordClass();

                try
                {
                    singleRec.LoyaltyNumber = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.LoyaltyNumber = string.Empty;
                }

                if (singleRec.LoyaltyNumber == "70304665182138")
                {
                    string stop = "Stop";
                }

                try
                {
                    singleRec.FirstName = GetFieldValue(row, ref columnNumber);
                    if (singleRec.FirstName.Length == 0)
                    {
                        singleRec.FirstName = "John" + numberOfRecords.ToString();
                    }
                }
                catch
                {
                    singleRec.FirstName = "John" + numberOfRecords.ToString();
                }
                try
                {
                    singleRec.LastName = GetFieldValue(row, ref columnNumber);
                    if (singleRec.LastName.Length == 0)
                    {
                        singleRec.LastName = "Smith" + numberOfRecords.ToString();
                    }
                }
                catch
                {
                    singleRec.LastName = "Smith" + numberOfRecords.ToString();
                }
                try
                {
                    singleRec.Address1 = GetFieldValue(row, ref columnNumber);
                    if (singleRec.Address1.Length == 0)
                    {
                        singleRec.Address1 = "333 main";
                    }
                }
                catch
                {
                    singleRec.Address1 = "333 main";
                }
                try
                {
                    singleRec.Address2 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Address2 = string.Empty;
                }
                try
                {
                    singleRec.City = GetFieldValue(row, ref columnNumber);
                    if (singleRec.City.Length == 0)
                    {
                        singleRec.City = "Dallas";
                    }
                }
                catch
                {
                    singleRec.City = "Dallas";
                }
                try
                {
                    singleRec.State = GetFieldValue(row, ref columnNumber);
                    if (singleRec.State.Length == 0)
                    {
                        singleRec.State = "TX";
                    }
                }
                catch
                {
                    singleRec.State = "TX";
                }
                try
                {
                    singleRec.PostalCode = GetFieldValue(row, ref columnNumber);
                    if (singleRec.PostalCode.Length == 0)
                    {
                        singleRec.PostalCode = "75024";
                    }
                }
                catch
                {
                    singleRec.PostalCode = "75024";
                }
                try
                {
                    singleRec.Country = GetFieldValue(row, ref columnNumber);
                    if (singleRec.Country.Length == 0)
                    {
                        singleRec.Country = "USA";
                    }
                }
                catch
                {
                    singleRec.Country = "USA";
                }
                try
                {
                    singleRec.PhoneNumber = GetFieldValue(row, ref columnNumber);
                    if (singleRec.PhoneNumber.Length == 0)
                    {
                        singleRec.PhoneNumber = "5555551212";
                    }
                }
                catch
                {
                    singleRec.PhoneNumber = "5555551212";
                }
                try
                {
                    singleRec.BirthDate = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.BirthDate);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.BirthDate = date.ToShortDateString();

                    if (singleRec.BirthDate.Length == 0)
                    {
                        singleRec.BirthDate = "01/01/1980";
                    }
                }
                catch
                {
                    singleRec.BirthDate = "01/01/1980";
                }
                try
                {
                    singleRec.Gender = GetFieldValue(row, ref columnNumber);
                    if (singleRec.Gender.Length == 0)
                    {
                        singleRec.Gender = "1";
                    }
                }
                catch
                {
                    singleRec.Gender = "1";
                }
                try
                {
                    singleRec.EmailAddress = GetFieldValue(row, ref columnNumber);
                    if (singleRec.EmailAddress.Length == 0)
                    {
                        singleRec.EmailAddress = "support@aeallaccess.com";
                    }
                }
                catch
                {
                    singleRec.EmailAddress = "support@aeallaccess.com";
                }
                try
                {
                    singleRec.Question1 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Question1 = string.Empty;
                }
                try
                {
                    singleRec.Question2 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Question2 = string.Empty;
                }
                try
                {
                    singleRec.Question3 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Question3 = string.Empty;
                }
                try
                {
                    singleRec.StoreNumber = GetFieldValue(row, ref columnNumber);
                    if (singleRec.StoreNumber.Length == 0)
                    {
                        singleRec.StoreNumber = "657";
                    }
                }
                catch
                {
                    singleRec.StoreNumber = "657";
                }
                try
                {
                    singleRec.ApplicationType = GetFieldValue(row, ref columnNumber);
                    if (singleRec.ApplicationType.Length == 0)
                    {
                        singleRec.ApplicationType = "1";
                    }
                }
                catch
                {
                    singleRec.ApplicationType = "1";
                }
                try
                {
                    singleRec.MailableStatus = GetFieldValue(row, ref columnNumber);
                    if (singleRec.MailableStatus.Length == 0)
                    {
                        singleRec.MailableStatus = "0";
                    }
                }
                catch
                {
                    singleRec.MailableStatus = "0";
                }
                try
                {
                    singleRec.CellPhone = GetFieldValue(row, ref columnNumber);
                    if (singleRec.CellPhone.Length == 0)
                    {
                        singleRec.CellPhone = "3332224444";
                    }
                }
                catch
                {
                    singleRec.CellPhone = "3332224444";
                }
                try
                {
                    singleRec.LanguagePref = GetFieldValue(row, ref columnNumber);
                    if (singleRec.LanguagePref.Length == 0)
                    {
                        singleRec.LanguagePref = "0";
                    }
                }
                catch
                {
                    singleRec.LanguagePref = "0";
                }
                try
                {
                    singleRec.SMSMessaging = GetFieldValue(row, ref columnNumber);
                    if (singleRec.SMSMessaging.Length == 0)
                    {
                        singleRec.SMSMessaging = "0";
                    }
                }
                catch
                {
                    singleRec.SMSMessaging = "0";
                }
                try
                {
                    singleRec.Primarycontact = GetFieldValue(row, ref columnNumber);
                    if (singleRec.Primarycontact.Length == 0)
                    {
                        singleRec.Primarycontact = "1"; /*email address */
                    }
                }
                catch
                {
                    singleRec.Primarycontact = "1";
                }

               try
               {
                   singleRec.Enrollmentdate = GetFieldValue(row, ref columnNumber);
                   double d = double.Parse(singleRec.Enrollmentdate);
                   DateTime date = DateTime.FromOADate(d);
                   singleRec.Enrollmentdate = date.ToShortDateString();

                   if (singleRec.Enrollmentdate.Length == 0)
                   {
                       singleRec.Enrollmentdate = "01/01/1980";
                   }
               }
               catch
               {
                   singleRec.Enrollmentdate = "08/01/2015";
               }
               
                
                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return returnVal;
            }

        }

        private void ProcessConfigFile(string FileName)
        {
            StreamReader sr = new StreamReader(FileName, Encoding.ASCII);
            //First record is headers
            lstItems = new ArrayList();
            int numberOfRecords = 0;

            string strRawRecord = sr.ReadLine();
            while (true)
            {
                try
                {

                    POSEnrollmentRecordClass AERec = null;
                    // Read Record
                    if (!ReadConfigRecord(sr, out AERec, ref numberOfRecords))
                    {
                        break;
                    }
                    lstItems.Add(AERec);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

        }
        private bool ReadConfigRecord(StreamReader sr, out POSEnrollmentRecordClass singleRec, ref int numberOfRecords)
        {
            bool returnVal = false;
            string strRawRecord;
            singleRec = null;

            string[] parsedStrings = new string[0];

            char[] param = new char[1];
            param[0] = ',';




            try
            {
                strRawRecord = sr.ReadLine();

                while (true)
                {
                    ++numberOfRecords;
                    //lblStatus.Text = "Processing record #" + numberOfRecords.ToString();
                    Application.DoEvents();
                    if (strRawRecord == null)
                    {
                        break;
                    }
                    if (strRawRecord.Length == 0)
                    {
                        strRawRecord = sr.ReadLine();
                    }
                    else if (strRawRecord.Substring(0, 1) == "-")
                    {
                        strRawRecord = sr.ReadLine();
                    }
                    else if (strRawRecord.Substring(0, 2) == ",,")
                    {
                        strRawRecord = null;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                if (strRawRecord != null)
                {
                    parsedStrings = strRawRecord.Split(param);
                    returnVal = true;
                    singleRec = new POSEnrollmentRecordClass();

                    try
                    {
                        singleRec.LoyaltyNumber = parsedStrings[0].Trim();
                    }
                    catch
                    {
                        singleRec.LoyaltyNumber = string.Empty;
                    }

                    if (singleRec.LoyaltyNumber == "70304665182138")
                    {
                        string stop = "Stop";
                    }

                    try
                    {
                        singleRec.FirstName = parsedStrings[1].Trim();
                        if (singleRec.FirstName.Length == 0)
                        {
                            singleRec.FirstName = "John" + numberOfRecords.ToString();
                        }
                    }
                    catch
                    {
                        singleRec.FirstName = "John" + numberOfRecords.ToString();
                    }
                    try
                    {
                        singleRec.LastName = parsedStrings[2].Trim();
                        if (singleRec.LastName.Length == 0)
                        {
                            singleRec.LastName = "Smith" + numberOfRecords.ToString();
                        }
                    }
                    catch
                    {
                        singleRec.LastName = "Smith" + numberOfRecords.ToString();
                    }
                    try
                    {
                        singleRec.Address1 = parsedStrings[3].Trim();
                        if (singleRec.Address1.Length == 0)
                        {
                            singleRec.Address1 = "333 main";
                        }
                    }
                    catch
                    {
                        singleRec.Address1 = "333 main";
                    }
                    try
                    {
                        singleRec.Address2 = parsedStrings[4].Trim();
                    }
                    catch
                    {
                        singleRec.Address2 = string.Empty;
                    }
                    try
                    {
                        singleRec.City = parsedStrings[5].Trim();
                        if (singleRec.City.Length == 0)
                        {
                            singleRec.City = "Dallas";
                        }
                    }
                    catch
                    {
                        singleRec.City = "Dallas";
                    }
                    try
                    {
                        singleRec.State = parsedStrings[6].Trim();
                        if (singleRec.State.Length == 0)
                        {
                            singleRec.State = "TX";
                        }
                    }
                    catch
                    {
                        singleRec.State = "TX";
                    }
                    try
                    {
                        singleRec.PostalCode = parsedStrings[7].Trim();
                        if (singleRec.PostalCode.Length == 0)
                        {
                            singleRec.PostalCode = "75024";
                        }
                    }
                    catch
                    {
                        singleRec.PostalCode = "75024";
                    }
                    try
                    {
                        singleRec.Country = parsedStrings[8].Trim();
                        if (singleRec.Country.Length == 0)
                        {
                            singleRec.Country = "USA";
                        }
                    }
                    catch
                    {
                        singleRec.Country = "USA";
                    }
                    try
                    {
                        singleRec.PhoneNumber = parsedStrings[9].Trim();
                        if (singleRec.PhoneNumber.Length == 0)
                        {
                            singleRec.PhoneNumber = "5555551212";
                        }
                    }
                    catch
                    {
                        singleRec.PhoneNumber = "5555551212";
                    }
                    try
                    {
                        singleRec.BirthDate = parsedStrings[10].Trim();
                        if (singleRec.BirthDate.Length == 0)
                        {
                            singleRec.BirthDate = "01/01/1980";
                        }
                    }
                    catch
                    {
                        singleRec.BirthDate = "01/01/1980";
                    }
                    try
                    {
                        singleRec.Gender = parsedStrings[11].Trim();
                        if (singleRec.Gender.Length == 0)
                        {
                            singleRec.Gender = "1";
                        }
                    }
                    catch
                    {
                        singleRec.Gender = "1";
                    }
                    try
                    {
                        singleRec.EmailAddress = parsedStrings[12].Trim();
                        if (singleRec.EmailAddress.Length == 0)
                        {
                            singleRec.EmailAddress = "support@aeallaccess.com";
                        }
                    }
                    catch
                    {
                        singleRec.EmailAddress = "support@aeallaccess.com";
                    }
                    try
                    {
                        singleRec.Question1 = parsedStrings[13].Trim();
                    }
                    catch
                    {
                        singleRec.Question1 = string.Empty;
                    }
                    try
                    {
                        singleRec.Question2 = parsedStrings[14].Trim();
                    }
                    catch
                    {
                        singleRec.Question2 = string.Empty;
                    }
                    try
                    {
                        singleRec.Question3 = parsedStrings[15].Trim();
                    }
                    catch
                    {
                        singleRec.Question3 = string.Empty;
                    }
                    try
                    {
                        singleRec.StoreNumber = parsedStrings[16].Trim();
                        if (singleRec.StoreNumber.Length == 0)
                        {
                            singleRec.StoreNumber = "657";
                        }
                    }
                    catch
                    {
                        singleRec.StoreNumber = "657";
                    }
                    try
                    {
                        singleRec.ApplicationType = parsedStrings[17].Trim();
                        if (singleRec.ApplicationType.Length == 0)
                        {
                            singleRec.ApplicationType = "1";
                        }
                    }
                    catch
                    {
                        singleRec.ApplicationType = "1";
                    }
                    try
                    {
                        singleRec.MailableStatus = parsedStrings[18].Trim();
                        if (singleRec.MailableStatus.Length == 0)
                        {
                            singleRec.MailableStatus = "0";
                        }
                    }
                    catch
                    {
                        singleRec.MailableStatus = "0";
                    }
                    try
                    {
                        singleRec.CellPhone = parsedStrings[19].Trim();
                        if (singleRec.CellPhone.Length == 0)
                        {
                            singleRec.CellPhone = "3332224444";
                        }
                    }
                    catch
                    {
                        singleRec.CellPhone = "3332224444";
                    }
                    try
                    {
                        singleRec.LanguagePref = parsedStrings[20].Trim();
                        if (singleRec.LanguagePref.Length == 0)
                        {
                            singleRec.LanguagePref = "0";
                        }
                    }
                    catch
                    {
                        singleRec.LanguagePref = "0";
                    }
                    try
                    {
                        singleRec.SMSMessaging = parsedStrings[21].Trim();
                        if (singleRec.SMSMessaging.Length == 0)
                        {
                            singleRec.SMSMessaging = "0";
                        }
                    }
                    catch
                    {
                        singleRec.SMSMessaging = "0";
                    }

                }

                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return returnVal;
            }

        }
        private string BuildRecord(POSEnrollmentRecordClass AERec)
        {
            string returnValue = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(Global.MakeFixedLength(AERec.LoyaltyNumber, 14, "0"));
            sb.Append(Global.MakeFixedLength(AERec.FirstName, 40, " "));
            sb.Append(Global.MakeFixedLength(AERec.LastName, 40, " "));
            sb.Append(Global.MakeFixedLength(AERec.Address1, 30, " "));
            sb.Append(Global.MakeFixedLength(AERec.Address2, 30, " "));
            sb.Append(Global.MakeFixedLength(AERec.City, 50, " "));
            sb.Append(Global.MakeFixedLength(AERec.State, 2, " "));
            sb.Append(Global.MakeFixedLength(AERec.PostalCode, 9, " "));
            sb.Append(Global.MakeFixedLength(AERec.Country, 3, " "));
            sb.Append(Global.MakeFixedLength(AERec.PhoneNumber, 10, " "));
            sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.BirthDate).ToString("MMddyyyy"), 8, " "));
            sb.Append(Global.MakeFixedLength(AERec.Gender, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.EmailAddress, 150, " "));
            sb.Append(Global.MakeFixedLength(AERec.Question1, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.Question2, 2, " "));
            sb.Append(Global.MakeFixedLength(AERec.Question3, 2, " "));
            sb.Append(Global.MakeFixedLength(AERec.StoreNumber, 5, "0"));
            sb.Append(Global.MakeFixedLength(AERec.ApplicationType, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.MailableStatus, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.CellPhone, 10, " "));
            sb.Append(Global.MakeFixedLength(AERec.LanguagePref, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.SMSMessaging, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.Primarycontact, 1, " "));
            sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.Enrollmentdate).ToString("MMddyyyy"), 8, " "));
            returnValue = sb.ToString();
            return returnValue;
        }

        public string WriteFile(List<POSEnrollmentRecordClass> enrollmentList, string fileType, out string path)
        {
            TextWriter output = null;
            string FileName;
            int NumberOfRecords = 0;
            POSEnrollmentRecordClass AERec = null;

            path = string.Empty;

            if (null == ConfigurationManager.AppSettings["OutputPath"])
            {
                MessageBox.Show("No path defined in app.config");
                return string.Empty;
            }
            else
            {
                path = ConfigurationManager.AppSettings["OutputPath"] + @"\POSEnrollment\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            FileName = path + "ddbp_data_" + DateTime.Now.ToString("yyMMdd") + "_052332.txt";
            if (!File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            output = File.CreateText(FileName);


            try
            {
                foreach (POSEnrollmentRecordClass rec in enrollmentList)
                {

                    AERec = rec;
                    string BirthDate = string.Empty;
                    string LoyaltyNumber = AERec.LoyaltyNumber;
                    NumberOfRecords++;

                    output.WriteLine(BuildRecord(AERec));
                }
                output.Close();
                return "POS Enrollment file ( " + FileName + ") Written";

            }
            catch (Exception e)
            {
                throw new Exception("WritePOSEnrollmentFile: LoyaltyNumber - " + AERec.LoyaltyNumber + " - " + e.Message);
            }


        }

    }
}
