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
    public class GEEnrollment
    {
        private ArrayList lstItems;

        private string GetFieldValue(List<string> row, ref int columnNumber)
        {
            string returnValue;

            returnValue = row[columnNumber].Trim();
            ++columnNumber;

            return returnValue;
        }
        public bool ProcessExcelRecord(List<string> row, int rowNumber, out GEEnrollmentRecordClass singleRec, ref int numberOfRecords)
        {
            bool returnVal = false;
            singleRec = null;
            int columnNumber = 0;

            try
            {

                returnVal = true;
                singleRec = new GEEnrollmentRecordClass();


                try
                {
                    singleRec.LoyaltyNumber = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.LoyaltyNumber = string.Empty;
                }
                if (singleRec.LoyaltyNumber.Length == 0)
                {
                    return false;
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
                    singleRec.MiddleName = GetFieldValue(row, ref columnNumber);
                    if (singleRec.MiddleName.Length == 0)
                    {
                        singleRec.MiddleName = "k";
                    }
                }
                catch
                {
                    singleRec.LastName = "k";
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
                    singleRec.PostalPlusFour = GetFieldValue(row, ref columnNumber);
                    if (singleRec.PostalPlusFour.Length == 0)
                    {
                        singleRec.PostalPlusFour = string.Empty;
                    }
                }
                catch
                {
                    singleRec.PostalPlusFour = string.Empty;
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

                if (singleRec.LoyaltyNumber == "70304665182138")
                {
                    string stop = "Stop";
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
                    singleRec.CardType = GetFieldValue(row, ref columnNumber);
                    if (singleRec.CardType.Length == 0)
                    {
                        singleRec.CardType = "D";
                    }
                }
                catch
                {
                    singleRec.CardType = "D";
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
                    singleRec.ProductId = GetFieldValue(row, ref columnNumber);
                    if (singleRec.ProductId.Length == 0)
                    {
                        singleRec.ProductId = "020";
                    }
                }
                catch
                {
                    singleRec.ProductId = "020";
                }

                try
                {
                    singleRec.Closedate = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.Closedate);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.Closedate = date.ToShortDateString();

                    if (singleRec.Closedate.Length == 0)
                    {
                        singleRec.Closedate = string.Empty;
                    }
                }
                catch
                {
                    singleRec.Closedate = string.Empty;
                }
                try
                {
                    singleRec.OpenDate = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.OpenDate);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.OpenDate = date.ToShortDateString();

                    if (singleRec.OpenDate.Length == 0)
                    {
                        singleRec.OpenDate = string.Empty;
                    }
                }
                catch
                {
                    singleRec.OpenDate = string.Empty;
                }
                try
                {
                    singleRec.CID = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.CID = string.Empty;
                }
                try
                {
                    singleRec.AccountKey = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.AccountKey = string.Empty;
                }

                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return returnVal;
            }

        }

        private bool ReadConfigRecord(StreamReader sr, out GEEnrollmentRecordClass singleRec, ref int numberOfRecords)
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
                    singleRec = new GEEnrollmentRecordClass();


                    try
                    {
                        singleRec.LoyaltyNumber = parsedStrings[0].Trim();
                    }
                    catch
                    {
                        singleRec.LoyaltyNumber = string.Empty;
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
                        singleRec.MiddleName = parsedStrings[2].Trim();
                        if (singleRec.MiddleName.Length == 0)
                        {
                            singleRec.MiddleName = "k" ;
                        }
                    }
                    catch
                    {
                        singleRec.LastName = "k" ;
                    }
                    try
                    {
                        singleRec.LastName = parsedStrings[3].Trim();
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
                        singleRec.Address1 = parsedStrings[4].Trim();
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
                        singleRec.Address2 = parsedStrings[5].Trim();
                    }
                    catch
                    {
                        singleRec.Address2 = string.Empty;
                    }
                    try
                    {
                        singleRec.City = parsedStrings[6].Trim();
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
                        singleRec.State = parsedStrings[7].Trim();
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
                        singleRec.PostalCode = parsedStrings[8].Trim();
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
                        singleRec.PostalPlusFour = parsedStrings[9].Trim();
                        if (singleRec.PostalPlusFour.Length == 0)
                        {
                            singleRec.PostalPlusFour = "1234";
                        }
                    }
                    catch
                    {
                        singleRec.PostalPlusFour = "1234";
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

                    if (singleRec.LoyaltyNumber == "70304665182138")
                    {
                        string stop = "Stop";
                    }
                    try
                    {
                        singleRec.EmailAddress = parsedStrings[11].Trim();
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
                        singleRec.CardType = parsedStrings[12].Trim();
                        if (singleRec.CardType.Length == 0)
                        {
                            singleRec.CardType = "D";
                        }
                    }
                    catch
                    {
                        singleRec.CardType = "D";
                    }
                    try
                    {
                        singleRec.StoreNumber = parsedStrings[13].Trim();
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
                        singleRec.ProductId = parsedStrings[14].Trim();
                        if (singleRec.ProductId.Length == 0)
                        {
                            singleRec.ProductId = "020";
                        }
                    }
                    catch
                    {
                        singleRec.ProductId = "020";
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
        private string BuildRecord(GEEnrollmentRecordClass AERec)
        {
            string returnValue = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(Global.MakeFixedLength(AERec.FirstName, 40, " "));
            sb.Append(Global.MakeFixedLength(AERec.MiddleName, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.LastName, 40, " "));
            sb.Append(Global.MakeFixedLength(AERec.Address1, 50, " "));
            sb.Append(Global.MakeFixedLength(AERec.Address2, 50, " "));
            sb.Append(Global.MakeFixedLength(AERec.City, 50, " "));
            sb.Append(Global.MakeFixedLength(AERec.State, 2, " "));
            sb.Append(Global.MakeFixedLength(AERec.PostalCode, 5, " "));
            sb.Append(Global.MakeFixedLength(AERec.PostalPlusFour, 4, " "));
            sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.BirthDate).ToString("yyyyMMdd"), 8, " "));
            sb.Append(Global.MakeFixedLength(string.Empty, 2, " "));
            sb.Append(Global.MakeFixedLength(AERec.LoyaltyNumber, 20, " "));
            sb.Append(Global.MakeFixedLength(AERec.EmailAddress, 150, " "));
            sb.Append(Global.MakeFixedLength(AERec.CardType, 1, " "));
            sb.Append(Global.MakeFixedLength(AERec.StoreNumber, 12, " "));
            sb.Append(Global.MakeFixedLength(AERec.ProductId, 3, " "));
            if (AERec.Closedate.Length > 0)
            {
                sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.Closedate).ToString("MMddyyyy"), 8, " "));
            }
            else
            {
                sb.Append(Global.MakeFixedLength(string.Empty, 8, " "));
            }
            if (AERec.OpenDate.Length > 0)
            {
                sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.OpenDate).ToString("MMddyyyy"), 8, " "));
            }
            else
            {
                sb.Append(Global.MakeFixedLength(string.Empty, 8, " "));
            }
            sb.Append(Global.MakeFixedLength(AERec.CID, 13, " "));
            sb.Append(Global.MakeFixedLength(AERec.AccountKey, 20, " "));
            returnValue = sb.ToString();
            return returnValue;
        }

        public string WriteFile(List<GEEnrollmentRecordClass> enrollmentList, string fileType, out string path)
        {
            TextWriter output = null;
            string FileName;
            int NumberOfRecords = 0;
            GEEnrollmentRecordClass AERec = null;
            string outMessage = string.Empty;

            path = string.Empty;

            if (null == ConfigurationManager.AppSettings["OutputPath"])
            {
                MessageBox.Show("No path defined in app.config");
                return outMessage;
            }
            else
            {
                path = ConfigurationManager.AppSettings["OutputPath"] + @"\GEEnrollment\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            FileName = path + "AE_BPCardholder_" + DateTime.Now.ToString("MMddyyyy") + "_052332.dat";
            if (!File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            output = File.CreateText(FileName);


            try
            {
                foreach (GEEnrollmentRecordClass rec in enrollmentList)
                {

                    AERec = rec;
                    string BirthDate = string.Empty;
                    string LoyaltyNumber = AERec.LoyaltyNumber;
                    NumberOfRecords++;

                    output.WriteLine(BuildRecord(AERec));
                }
                output.Close();
                return "GE Enrollment file ( " + FileName + ") Written";

            }
            catch (Exception e)
            {
                throw new Exception("WriteGEEnrollmentFile: LoyaltyNumber - " + AERec.LoyaltyNumber + " - " + e.Message);
            }


        }

    }
}
