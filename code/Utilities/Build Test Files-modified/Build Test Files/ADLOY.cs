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
    class ADLOY
    {
        private string GetFieldValue(List<string> row, ref int columnNumber)
        {
            string returnValue;

            returnValue = row[columnNumber].Trim();
            ++columnNumber;

            return returnValue;
        }
        public bool ProcessExcelRecord(List<string> row, int rowNumber, out ADLOYRecordClass singleRec, ref int numberOfRecords)
        {
            bool returnVal = false;
            int columnNumber = 0;
            singleRec = null;

            try
            {
                returnVal = true;
                singleRec = new ADLOYRecordClass();

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
                    singleRec.First_Name = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.First_Name = string.Empty;
                }
                try
                {
                    singleRec.Last_Name = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Last_Name = string.Empty;
                }
                try
                {
                    singleRec.Line1 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Line1 = string.Empty;
                }
                try
                {
                    singleRec.Line2 = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Line2 = string.Empty;
                }
                try
                {
                    singleRec.City = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.City = string.Empty;
                }
                try
                {
                    singleRec.State = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.State = string.Empty;
                }
                try
                {
                    singleRec.Zip = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Zip = string.Empty;
                }
                try
                {
                    singleRec.Country = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Country = string.Empty;
                }
                try
                {
                    singleRec.Birth_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.Birth_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.Birth_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.Birth_Date = string.Empty;
                }
                try
                {
                    singleRec.Mobile_Number = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Mobile_Number = string.Empty;
                }
                try
                {
                    singleRec.Gender = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Gender = string.Empty;
                }
                try
                {
                    singleRec.Loyalty_Number = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Loyalty_Number = string.Empty;
                }
                try
                {
                    singleRec.Email_Address = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Email_Address = string.Empty;
                }
                try
                {
                    singleRec.Valid_Address = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Valid_Address = string.Empty;
                }
                try
                {
                    singleRec.Record_Type = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Record_Type = string.Empty;
                }
                try
                {
                    singleRec.Valid_Email_Address = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Valid_Email_Address = string.Empty;
                }
                try
                {
                    singleRec.Cell_Double_Opt_In = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Cell_Double_Opt_In = string.Empty;
                }
                try
                {
                    singleRec.Employee_Status = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Employee_Status = string.Empty;
                }
                try
                {
                    singleRec.Store_Number = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Store_Number = string.Empty;
                }
                try
                {
                    singleRec.Language_Preference = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Language_Preference = string.Empty;
                }
                try
                {
                    singleRec.Enrollment_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.Enrollment_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.Enrollment_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.Enrollment_Date = string.Empty;
                }
                try
                {
                    singleRec.Enrollment_Source = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Enrollment_Source = string.Empty;
                }
                try
                {
                    singleRec.Enrollment_ID = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.Enrollment_ID = string.Empty;
                }
                try
                {
                    singleRec.AECC_Status = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.AECC_Status = string.Empty;
                }
                try
                {
                    singleRec.AECC_Open_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.AECC_Open_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.AECC_Open_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.AECC_Open_Date = string.Empty;
                }
                try
                {
                    singleRec.AECC_Close_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.AECC_Close_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.AECC_Close_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.AECC_Close_Date = string.Empty;
                }
                try
                {
                    singleRec.AEVisa_Status = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.AEVisa_Status = string.Empty;
                }
                try
                {
                    singleRec.AEVisa_Account_Key = GetFieldValue(row, ref columnNumber);
                }
                catch
                {
                    singleRec.AEVisa_Account_Key = string.Empty;
                }
                try
                {
                    singleRec.AEVisa_Open_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.AEVisa_Open_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.AEVisa_Open_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.AEVisa_Open_Date = string.Empty;
                }
                try
                {
                    singleRec.AEVisa_Close_Date = GetFieldValue(row, ref columnNumber);
                    double d = double.Parse(singleRec.AEVisa_Close_Date);
                    DateTime date = DateTime.FromOADate(d);
                    singleRec.AEVisa_Close_Date = date.ToShortDateString();
                }
                catch
                {
                    singleRec.AEVisa_Close_Date = string.Empty;
                }
                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return returnVal;
            }
        }
        private string BuildRecord(ADLOYRecordClass AERec)
        {
            string returnValue = "";

            StringBuilder sb = new StringBuilder();
            sb.Append(AERec.CID);
            sb.Append("|");
            sb.Append(AERec.First_Name);
            sb.Append("|");
            sb.Append(AERec.Last_Name);
            sb.Append("|");
            sb.Append(AERec.Line1);
            sb.Append("|");
            sb.Append(AERec.Line2);
            sb.Append("|");
            sb.Append(AERec.City);
            sb.Append("|");
            sb.Append(AERec.State);
            sb.Append("|");
            sb.Append(AERec.Zip);
            sb.Append("|");
            sb.Append(AERec.Country);
            sb.Append("|");
            if (AERec.Birth_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.Birth_Date).ToString("MMddyyyy"), 8, " ")); }            
            sb.Append("|");
            sb.Append(AERec.Mobile_Number);
            sb.Append("|");
            sb.Append(AERec.Gender);
            sb.Append("|");
            sb.Append(AERec.Loyalty_Number);
            sb.Append("|");
            sb.Append(AERec.Email_Address);
            sb.Append("|");
            sb.Append(AERec.Valid_Address);
            sb.Append("|");
            sb.Append(AERec.Record_Type);
            sb.Append("|");
            sb.Append(AERec.Valid_Email_Address);
            sb.Append("|");
            sb.Append(AERec.Cell_Double_Opt_In);
            sb.Append("|");
            sb.Append(AERec.Employee_Status);
            sb.Append("|");
            sb.Append(AERec.Store_Number);
            sb.Append("|");
            sb.Append(AERec.Language_Preference);
            sb.Append("|");
            if (AERec.Enrollment_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.Enrollment_Date).ToString("MMddyyyy"), 8, " ")); }            
            sb.Append("|");
            sb.Append(AERec.Enrollment_Source);
            sb.Append("|");
            sb.Append(AERec.Enrollment_ID);
            sb.Append("|");
            sb.Append(AERec.AECC_Status);
            sb.Append("|");
            if (AERec.AECC_Open_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.AECC_Open_Date).ToString("MMddyyyy"), 8, " ")); }            
            sb.Append("|");            
            if (AERec.AECC_Close_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.AECC_Close_Date).ToString("MMddyyyy"), 8, " ")); }            
            sb.Append("|");
            sb.Append(AERec.AEVisa_Status);
            sb.Append("|");
            sb.Append(AERec.AEVisa_Account_Key);
            sb.Append("|");
            if (AERec.AEVisa_Open_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.AEVisa_Open_Date).ToString("MMddyyyy"), 8, " ")); }
            sb.Append("|");
            if(AERec.AEVisa_Close_Date != string.Empty)
            { sb.Append(Global.MakeFixedLength(DateTime.Parse(AERec.AEVisa_Close_Date).ToString("MMddyyyy"), 8, " ")); }            
            sb.Append("|");
            returnValue = sb.ToString();
            return returnValue;
        }
        public string WriteFile(List<ADLOYRecordClass> adloyList, string fileType, out string path)
        {
            TextWriter output = null;
            string FileName;
            int NumberOfRecords = 0;
            ADLOYRecordClass AERec = null;

            path = string.Empty;

            if (null == ConfigurationManager.AppSettings["OutputPath"])
            {
                MessageBox.Show("No path defined in app.config");
                return string.Empty;
            }
            else
            {
                path = ConfigurationManager.AppSettings["OutputPath"] + @"\ProfileIN\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            FileName = path + "AD_LOY_" + DateTime.Now.ToString("yyMMdd") + "_052332.txt";
            if (!File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            output = File.CreateText(FileName);


            try
            {
                foreach (ADLOYRecordClass rec in adloyList)
                {

                    AERec = rec;
                    string BirthDate = string.Empty;
                    string LoyaltyNumber = AERec.Loyalty_Number;
                    NumberOfRecords++;

                    output.WriteLine(BuildRecord(AERec));
                }
                output.Close();
                return "Profile IN file ( " + FileName + ") Written";

            }
            catch (Exception e)
            {
                throw new Exception("WriteProfileINFile: LoyaltyNumber - " + AERec.Loyalty_Number + " - " + e.Message);
            }
        }

    }
}
