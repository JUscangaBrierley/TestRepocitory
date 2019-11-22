using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;

namespace BuildTestFiles
{
    public partial class fMain : Form
    {
     
        private List<TlogRecordClass> tlogList;
        private List<B2CTlogRecordClass> B2CtlogList;
        private List<B2CDetailRecord> DetailList2 ;
        private List<B2CTenderRecord> TenderList2 ;
        private List<POSEnrollmentRecordClass> posenrollmentList;
        private List<GEEnrollmentRecordClass> geenrollmentList;
        private List<ADLOYRecordClass> adloyList;
        private List<String> excelList;
        Tlog tlog = new Tlog();
        POSEnrollment posEnrollment = new POSEnrollment();
        GEEnrollment geEnrollment = new GEEnrollment();
        ADLOY Adloy = new ADLOY();
        private string tlogPrefix = "BuildTlogConfig";
        private string B2CtlogPrefix = "B2CBuildTlogConfig";
        private string GEPrefix = "BuildGEEnrollmentConfig_";
        private string POSPrefix = "BuildPOSEnrollmentConfig_";
        private string ADLoyPrefix = "BuildADLOYConfig_";
        private string fileSuffix = "xls";
        private string fileSuffix2010 = "xlsx";
        private string filePrefix = string.Empty;
        private long defaultLoyaltyNumber = 8000000000;
        private int defaultTransactionNumber = 50000;
        private DateTime defaultTxnDate = DateTime.Now.AddYears(-1);
        private int numberofTxnsPerLoyaltyNumber = 10;

        private enum FileType
        {
            Tlog = 1,
            POSEnrollment = 2,
            GEEnrollment = 3,
            ADLoy = 4,
            B2CTlog = 5
        }
        public fMain(string FileName)
        {
            InitializeComponent();

            if (FileName.Length > 0)
            {
                txtFileName.Text = FileName;
                switch (FileName.Substring(1, 3))
                {
                    case "IFC":
                        LoadFile(FileType.Tlog);
                        break;
                    case "ddb":
                        LoadFile(FileType.POSEnrollment);
                        break;
                    case "AE_":
                        LoadFile(FileType.GEEnrollment);
                        break;
                    case "AD_LOY_":
                        LoadFile(FileType.ADLoy);
                        break;
                    case "B2C":
                        LoadFile(FileType.B2CTlog);
                        break;
                    default:
                        break;
                }
            }

            long nextLoyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(defaultLoyaltyNumber);
            txtStartingLoyaltyNumber.Text = nextLoyaltyNumber.ToString();
            txtStartingTxnDate.Text = defaultTxnDate.ToShortDateString();
            txtStartingTxnNumber.Text = defaultTransactionNumber.ToString();
            txtNumberOfTxns.Text = "1000";
            txtNumberOfTxnsPerLoyaltyNumber.Text = numberofTxnsPerLoyaltyNumber.ToString();
        }
        private FileType DetermineFileType()
        {
            FileType fileType = FileType.Tlog;
            string fileName = System.IO.Path.GetFileName(txtFileName.Text);

            if (fileName.ToUpper().Contains("GE"))
            {
                fileType = FileType.GEEnrollment;
            }
            if (fileName.ToUpper().Contains("POS"))
            {
                fileType = FileType.POSEnrollment;
            }
            if (fileName.ToUpper().Contains("TLOG"))
            {
                fileType = FileType.Tlog;
            }
            if (fileName.ToUpper().Contains("ADLOY"))
            {
                fileType = FileType.ADLoy;
            }
            if (fileName.ToUpper().Contains("B2C"))
            {
                fileType = FileType.B2CTlog;
            }

            return fileType;
        }
        private void LoadFile(FileType fileType)
        {
            string fileName = System.IO.Path.GetFileName(txtFileName.Text);
            lblStatus.Text = "Loading File: " + fileName;
            Thread.Sleep(0);

            try
            {

                switch (fileType)
                {
                    case FileType.GEEnrollment:
                        GetFileType(fileName, GEPrefix);
                        OpenTlogFile2(txtFileName.Text, fileType);
                        break;

                    case FileType.POSEnrollment:
                        GetFileType(fileName, POSPrefix);
                        OpenTlogFile2(txtFileName.Text, fileType);
                        break;

                    case FileType.Tlog:
                        GetFileType(fileName, tlogPrefix);
                        OpenTlogFile2(txtFileName.Text, fileType);
                        //OpenTlogFile(txtFileName.Text);
                        break;

                    case FileType.ADLoy:
                        GetFileType(fileName, ADLoyPrefix);
                        OpenTlogFile2(txtFileName.Text, fileType);                        
                        break;

                    case FileType.B2CTlog:
                        GetFileType(fileName, B2CtlogPrefix);
                        OpenTlogFileB2C(txtFileName.Text, fileType);
                        break;

                    default:
                        break;
                }

                lblStatus.Text = "File: " + fileName + " Uploaded ";

            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }
        private void GetFileType(string fileName, string fileType)
        {
            int position = 0;

            if (fileName.StartsWith(fileType))
            {
                filePrefix = fileName.Remove(0, fileType.Length - 1);
                if (filePrefix.EndsWith(fileSuffix2010))
                {
                    position = filePrefix.IndexOf(fileSuffix2010);
                }
                else
                {
                    position = filePrefix.IndexOf(fileSuffix);
                }
                filePrefix = filePrefix.Substring(1, position - 2);
            }
        }
        private void OpenTlogFile2(string fileName, FileType fileType)
        {
            int recordCount = 0;
            Microsoft.Office.Interop.Excel.Application xlApp = null;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
            object misValue = System.Reflection.Missing.Value;

            try
            {

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(fileName, 0, true, 5, string.Empty, string.Empty, true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Range xlRange = xlWorkSheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                string message = string.Empty;

                tlogList = new List<TlogRecordClass>();
                geenrollmentList = new List<GEEnrollmentRecordClass>();
                posenrollmentList = new List<POSEnrollmentRecordClass>();
                adloyList = new List<ADLOYRecordClass>();

                for (int rowNumber = 2; rowNumber <= rowCount; rowNumber++)
                {
                    ++recordCount;
                    excelList = new List<string>(colCount);
                    for (int j = 1; j <= colCount; j++)
                    {
                        message = string.Empty;
                        if (xlRange.Cells[rowNumber, j].Value2 != null)
                        {
                            message = xlRange.Cells[rowNumber, j].Value2.ToString();
                        }
                        excelList.Add(message);
                    }
                    if (fileType == FileType.Tlog)
                    {
                        TlogRecordClass AERec = null;
                        SKUItemDetail itemDtl;
                        if (tlog.ProcessTlogExcelRecord(excelList, rowNumber, out AERec, out itemDtl, ref recordCount))
                        {
                            tlogList.Add(AERec);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (fileType == FileType.GEEnrollment)
                    {
                        GEEnrollmentRecordClass rec = null;
                        if (geEnrollment.ProcessExcelRecord(excelList, rowNumber, out rec, ref recordCount))
                        {
                            geenrollmentList.Add(rec);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (fileType == FileType.POSEnrollment)
                    {
                        POSEnrollmentRecordClass rec = null;
                        if (posEnrollment.ProcessExcelRecord(excelList, rowNumber, out rec, ref recordCount))
                        {
                            posenrollmentList.Add(rec);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (fileType == FileType.ADLoy)
                    {
                        ADLOYRecordClass rec = null;

                        if (rowNumber > 4) // Skip 4 headers
                        {
                            if (Adloy.ProcessExcelRecord(excelList, rowNumber, out rec, ref recordCount))                            
                            {
                                adloyList.Add(rec);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    lblStatus.Text = "Processed " + recordCount.ToString() + " of " + rowCount.ToString();
                    System.Windows.Forms.Application.DoEvents();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }
        private void OpenTlogFileB2C(string fileName, FileType fileType)
        {
            int recordCount = 0;
            Microsoft.Office.Interop.Excel.Application xlApp = null;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
            object misValue = System.Reflection.Missing.Value;

            try
            {

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(fileName, 0, true, 5, string.Empty, string.Empty, true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Range xlRange = xlWorkSheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                string message = string.Empty;

                B2CtlogList = new List<B2CTlogRecordClass>();
                DetailList2 = new List<B2CDetailRecord>();
                TenderList2 = new List<B2CTenderRecord>();
                for (int rowNumber = 2; rowNumber <= rowCount; rowNumber++)
                {
                    ++recordCount;
                    excelList = new List<string>(colCount);
                    for (int j = 1; j <= colCount; j++)
                    {
                        message = string.Empty;
                        if (xlRange.Cells[rowNumber, j].Value2 != null)
                        {
                            message = xlRange.Cells[rowNumber, j].Value2.ToString();
                        }
                        excelList.Add(message);
                    }
                    if (fileType == FileType.B2CTlog)
                    {
                        B2CTlogRecordClass AERec = null;
                        List<B2CDetailRecord> DetailList = new List<B2CDetailRecord>();
                       
                        List<B2CTenderRecord> TenderList = new List<B2CTenderRecord>();
                        
                        if (tlog.ProcessB2CTlogExcelRecord(excelList, rowNumber, out AERec, out DetailList,out TenderList, ref recordCount))
                        {
                            B2CtlogList.Add(AERec);
                            foreach (B2CDetailRecord detail in DetailList)
                            {
                                DetailList2.Add(detail);
                            }
                            foreach (B2CTenderRecord tender in TenderList)
                            {
                                TenderList2.Add(tender);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }                                            
                    lblStatus.Text = "Processed " + recordCount.ToString() + " of " + rowCount.ToString();
                    System.Windows.Forms.Application.DoEvents();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        } 

        private String[] GetExcelSheetNames(string excelFile)
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            //DisplayMessage("GetExcelSheetNames - Begin</br>");

            try
            {
                //DisplayMessage("GetExcelSheetNames - OPen File</br>");
                // Connection String. Change the excel file to the file you
                // will search.
                String connString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                //DisplayMessage("GetExcelSheetNames - dt row count" + dt.Rows.Count.ToString());
                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    string rowName = row["TABLE_NAME"].ToString();
                    //DisplayMessage("GetExcelSheetNames - Check Row " + rowName);
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                return excelSheets;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                txtFileName.Text = openFileDialog1.FileName;
            }
        }
        private void fMain_Load(object sender, EventArgs e)
        {

        }

        private void btnGenerateTlog_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            LoadFile(FileType.Tlog);
            lblStatus.Text = tlog.WriteTlogFile(tlogList, filePrefix, out path);
            System.Diagnostics.Process.Start("explorer.exe", path); 
        }

        private void btnGenerateGEEnrollment_Click(object sender, EventArgs e)
        {
            LoadFile(FileType.GEEnrollment);
        }

        private void btnBuildPOSEnrollment_Click(object sender, EventArgs e)
        {
            LoadFile(FileType.POSEnrollment);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void btnGenerateFile_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            this.Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Loading File";
            System.Windows.Forms.Application.DoEvents();

            FileType fileType = DetermineFileType();
            //LoadFile(fileType);
            if (fileType == FileType.Tlog)
            {
                LoadFile(fileType);
                lblStatus.Text = tlog.WriteTlogFile(tlogList, filePrefix, out path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            if (fileType == FileType.GEEnrollment)
            {
                LoadFile(fileType);
                lblStatus.Text = geEnrollment.WriteFile(geenrollmentList, filePrefix, out path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            if (fileType == FileType.POSEnrollment)
            {
                LoadFile(fileType);
                lblStatus.Text = posEnrollment.WriteFile(posenrollmentList, filePrefix, out path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            if (fileType == FileType.ADLoy)
            {
                LoadFile(fileType);
                lblStatus.Text = Adloy.WriteFile(adloyList, filePrefix, out path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            if (fileType == FileType.B2CTlog)
            {
                LoadFile(fileType);
                lblStatus.Text = tlog.WriteB2CTlogFile(B2CtlogList, DetailList2,TenderList2, filePrefix, out path);
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            this.Cursor = Cursors.Default;

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            Dispose();

        }

        private void btnGenerateBulkTlog_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            int numberOfTxns = int.Parse(txtNumberOfTxns.Text);
            int numberOfTxnsPerLoyaltyNumber = int.Parse(txtNumberOfTxnsPerLoyaltyNumber.Text);
            int numberOfLoyaltyNumbers = numberOfTxns / numberofTxnsPerLoyaltyNumber;
            long loyaltyNumber = 0;
            long loyaltyNumberBase = defaultLoyaltyNumber;
            int txnNumber = int.Parse(txtStartingTxnNumber.Text);
            DateTime txnDate = DateTime.Parse(txtStartingTxnDate.Text);
            int recordCount = 0;

            loyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(loyaltyNumberBase);
            tlogList = new List<TlogRecordClass>();

            while (numberOfTxns > 0)
            {
                txnDate = DateTime.Parse(txtStartingTxnDate.Text);
                while (numberOfTxnsPerLoyaltyNumber > 0)
                {
                    TlogRecordClass AERec = null;
                    SKUItemDetail itemDtl;

                    if (tlog.ProcessBulkTlogRecord(loyaltyNumber.ToString(), txnDate, txnNumber, out AERec, out itemDtl, ref recordCount))
                    {
                        tlogList.Add(AERec);
                    }
                    else
                    {
                        break;
                    }
                    lblStatus.Text = "Processed " + recordCount.ToString() + " of " + numberOfTxns.ToString();
                    System.Windows.Forms.Application.DoEvents();
                    ++txnNumber;
                    txnDate = txnDate.AddDays(1);
                    --numberOfTxnsPerLoyaltyNumber;
                    --numberOfTxns;
                }
                ++loyaltyNumberBase;
                loyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(loyaltyNumberBase);
                numberOfTxnsPerLoyaltyNumber = int.Parse(txtNumberOfTxnsPerLoyaltyNumber.Text);

            }
            lblStatus.Text = tlog.WriteTlogFile(tlogList, filePrefix, out path);
            System.Diagnostics.Process.Start("explorer.exe", path);

        }
    }
}
