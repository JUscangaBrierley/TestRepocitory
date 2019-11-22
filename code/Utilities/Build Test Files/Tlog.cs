using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;


namespace BuildTestFiles
{
    class Tlog
    {
        const string acceptableStrings = "01,02,03,04,05,06,07,08,10,11,14,15,97,27";
        const string acceptable27Strings = "33,51,62,92,93,99";
        const string PositiveTenderTypes = "60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,78";
        const string NegativeTenderTypes = "80,81,82,83,84,85,86,87,88,89,90,91,92,73,94,95,";
        public int TransactionNumber = 0;
        public string RegisterNumber = "001";
        public int StoreNumber = 0;
        public int TransactionType = 0;
        public int RetailAmount = 0;
        public int TenderType = 71;
        public int ItemType = 1;
        public string sTenderType = string.Empty;
        public ArrayList lstTenderTypes;
        private ArrayList lstItems;
        private decimal CentsCutOff = (decimal).49;
        private bool autoGenerateTxnNumber = false;
        public bool foundorderfl = false;

        public void BuildTlog()
        {
            lstItems = new ArrayList();
            BuildTlogFromConfig();
            //WriteTlogFile();
        }
        private void BuildTlogFromConfig()
        {
            string Path = System.Windows.Forms.Application.StartupPath.Replace(@"bin\Debug", string.Empty);
            string FileName = "BuildTlogConfig.csv";

            if (!Directory.Exists(Path + @"Config"))
            {
                Directory.CreateDirectory(Path + "Config");
            }
            Path += @"Config\";
            ProcessConfigFile(Path + FileName);

            //if(GetTransactionNumberDefault(Path))
            //{
            //    ProcessConfigFile(Path + FileName);
            //}

        }
        private void ProcessConfigFile(string FileName)
        {
            StreamReader sr = new StreamReader(FileName, Encoding.ASCII);
            //First record is headers
            lstItems = new ArrayList();
            int numberOfRecords = 0;

            System.Windows.Forms.Application.DoEvents();
            TransactionNumber = 50000;

            string strRawRecord = sr.ReadLine();
            while (true)
            {
                try
                {

                    TlogRecordClass AERec = null;
                    SKUItemDetail itemDtl;
                    // Read Record
                    if (!ReadConfigRecord(sr, out AERec, out itemDtl, ref numberOfRecords))
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
        public bool ProcessTlogExcelRecord(List<string> row, int rowNumber, out TlogRecordClass singleRec, out SKUItemDetail itemDtl, ref int numberOfRecords)
        {
            bool returnVal = false;
            string strRawRecord;
            singleRec = null;
            lstTenderTypes = new ArrayList();

            string[] parsedSKUs = new string[100];
            string[] parsedTenderTypes = new string[3];

            char[] param = new char[1];
            param[0] = ',';

            char[] paramTenderType = new char[1];
            paramTenderType[0] = '|';

            char[] paramSKU = new char[1];
            paramSKU[0] = '|';

            int ClassCode = 0;

            string sSKUNumber = string.Empty;
            string sItemReasonCode = string.Empty;
            int itemAmount = 2500;
            decimal itemDecimalAmount = 2500;
            const decimal oneCent = .01M;
            int itemType = 1;
            int columnNumber = 0;

            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, string.Empty, 0, 0, 0, 0, 0, 0, 0);

            try
            {

                returnVal = true;
                singleRec = new TlogRecordClass();
                singleRec.AltCardNumber = row[columnNumber].Trim();
                ++columnNumber;

                singleRec.PurchaseDate = row[columnNumber].Trim();
                if (singleRec.PurchaseDate.Length == 0)
                {
                    return false;
                }
                double d = double.Parse(singleRec.PurchaseDate);
                DateTime date = DateTime.FromOADate(d);
                singleRec.PurchaseDate = date.ToShortDateString();

                ++columnNumber;
                if (singleRec.PurchaseDate.Length == 0)
                {
                    return false;
                }

                if (singleRec.AltCardNumber == "79369004740261")
                {
                    string stop = "Stop";
                }
                try
                {
                    singleRec.WebOrderNumber = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.WebOrderNumber = string.Empty;
                }
                ++columnNumber;
                try
                {
                    singleRec.NumberOfItems = int.Parse(row[columnNumber].Trim());
                }
                catch (Exception ex)
                {
                    singleRec.NumberOfItems = 1;
                }
                ++columnNumber;
                try
                {
                    singleRec.TransType = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.TransType = 0;
                }
                ++columnNumber;
                try
                {
                    sTenderType = row[columnNumber].Trim();
                    if (sTenderType.IndexOf("|") > 0)
                    {
                        lstTenderTypes.AddRange(sTenderType.Split(paramTenderType, 3));
                        foreach (string s in lstTenderTypes)
                        {
                            int TenderType = int.Parse(s);
                            singleRec.TenderDetail.Add(
                                new TenderTypeDetail(
                                TenderType,
                                0,
                                10));
                        }
                    }
                    else if (sTenderType.Length > 0)
                    {
                        singleRec.TenderDetail.Add(
                            new TenderTypeDetail(
                            int.Parse(sTenderType),
                            0,
                            10));
                    }
                    else
                    {
                        singleRec.TenderDetail.Add(
                            new TenderTypeDetail(
                            71,
                            0,
                            10));
                    }
                }
                catch (Exception e)
                {
                    singleRec.TenderType = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.EmployeeNumber = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.EmployeeNumber = string.Empty;
                }
                ++columnNumber;
                try
                {
                    singleRec.ReasonCode = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.ReasonCode = string.Empty;
                }
                ++columnNumber;
                try
                {
                    ItemType = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    ItemType = 1;
                }
                ++columnNumber;
                try
                {
                    singleRec.StoreCode = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.StoreCode = 0;
                }
                ++columnNumber;
                try
                {
                    ClassCode = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    ClassCode = 0;
                }
                ++columnNumber;
                try
                {
                    sSKUNumber = row[columnNumber].Trim();
                    if (sSKUNumber.Contains("|"))
                    {
                        parsedSKUs = sSKUNumber.Split(paramSKU);
                    }
                    else
                    {
                        parsedSKUs = null;
                    }
                }
                catch (Exception ex1)
                {
                    sSKUNumber = string.Empty;
                }
                ++columnNumber;
                try
                {
                    string amount = row[columnNumber].Trim();
                    singleRec.ActualAmount = decimal.Parse(row[columnNumber].Trim());
                    if (!amount.Contains("."))
                    {
                        singleRec.ActualAmount = singleRec.ActualAmount * 100;
                    }
                }
                catch
                {
                    singleRec.ActualAmount = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.PurchaseTime = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.PurchaseTime = string.Empty;
                }
                ++columnNumber;
                try
                {
                    singleRec.Credits = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.Credits = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.Country = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.Country = string.Empty;
                }
                ++columnNumber;
                try
                {
                    singleRec.GiftCardMode = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.GiftCardMode = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.String77 = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.String77 = string.Empty;
                }
                ++columnNumber;
                try
                {
                    if (autoGenerateTxnNumber)
                    {
                        ++TransactionNumber;
                        singleRec.TransactionNumber = TransactionNumber.ToString();
                    }
                    else
                    {
                        if (row[columnNumber].Trim().Length > 0)
                        {
                            string txnNumber = row[columnNumber].Trim();
                            if (txnNumber.Contains("generate"))
                            {
                                txnNumber = txnNumber.Substring(9);
                                TransactionNumber = int.Parse(txnNumber);
                                autoGenerateTxnNumber = true;
                                singleRec.TransactionNumber = TransactionNumber.ToString();
                            }
                            else
                            {
                                singleRec.TransactionNumber = row[columnNumber].Trim();
                            }

                        }
                    }
                }
                catch { }
                ++columnNumber;
                if (singleRec.TransactionNumber == "089013")
                {
                    string stop = "stop";
                }
                try
                {
                    singleRec.OriginalTransactionNumber = row[columnNumber].Trim();
                }
                catch { }
                ++columnNumber;
                try
                {
                    singleRec.OriginalStoreCode = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.OriginalStoreCode = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.OriginalPurchaseDate = row[columnNumber].Trim();
                    double date1 = double.Parse(singleRec.OriginalPurchaseDate);
                    DateTime date2 = DateTime.FromOADate(date1);
                    singleRec.OriginalPurchaseDate = date2.ToShortDateString();

                }
                catch { }
                ++columnNumber;
                try
                {
                    singleRec.OriginalTenderType = int.Parse(row[columnNumber].Trim());
                }
                catch { }
                ++columnNumber;
                try
                {
                    singleRec.OriginalWebOrderNumber = row[columnNumber].Trim();
                }
                catch { }
                ++columnNumber;
                try
                {
                    singleRec.AltTransactionNumber = row[columnNumber].Trim();
                }
                catch { }
                ++columnNumber;
                try
                {
                    singleRec.AltStoreCode = int.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.AltStoreCode = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.AltTransactionDate = row[columnNumber].Trim();
                }
                catch { }

                ++columnNumber;
                if (singleRec.WebOrderNumber == "W8901")
                {
                    string stop = "stop";
                }
                try
                {
                    singleRec.String181Amount = decimal.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.String181Amount = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.String182Amount = decimal.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.String182Amount = 0;
                }
                ++columnNumber;
                try
                {
                    singleRec.GiftCertificateAmount = decimal.Parse(row[columnNumber].Trim());
                }
                catch
                {
                    singleRec.GiftCertificateAmount = 0;
                }
                ++columnNumber;


                try
                {
                    sItemReasonCode = row[columnNumber].Trim();
                }
                catch
                {
                    sItemReasonCode = string.Empty;
                }

                ++columnNumber;

                // PI 26487, Akbar, Assiging value to single use coupon fields starts 
                try
                {
                    singleRec.SingleUseCoupon = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.SingleUseCoupon = string.Empty;
                }

                ++columnNumber;

                try
                {
                    singleRec.SUCNotValid = row[columnNumber].Trim();
                }
                catch
                {
                    singleRec.SUCNotValid = string.Empty;
                }

                ++columnNumber;
                // PI 26487, Akbar, Assiging value to single use coupon fields ends

                if (singleRec.TransactionNumber == "927164")
                {
                    string stop = "stop";
                }
                if ((parsedSKUs != null) && (parsedSKUs.Length > 0))
                {
                    int remainder = parsedSKUs.Length % 4;
                    if (remainder != 0)
                    {
                        throw new Exception("Odd Number of items in SKU ");
                    }

                    singleRec.NumberOfItems = 0;
                    for (int i = 0; i <= parsedSKUs.Length - 1; i = i + 4)
                    {
                        string sClass = parsedSKUs[i].ToString().Trim();
                        string ssku = parsedSKUs[i + 1].ToString().Trim();
                        string sAmt = parsedSKUs[i + 2].ToString().Trim();
                        string sTxnType = parsedSKUs[i + 3].ToString().Trim();
                        ClassCode = int.Parse(parsedSKUs[i].ToString().Trim());
                        sSKUNumber = parsedSKUs[i + 1].ToString().Trim();
                        itemAmount = decimal.ToInt32(decimal.Parse(sAmt));
                        itemDecimalAmount = decimal.Parse(sAmt);
                        itemType = int.Parse(sTxnType);
                        if (sItemReasonCode.Length > 0)
                        {
                            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, itemType, 0, 0, ClassCode, string.Empty, 0, 0, int.Parse(sItemReasonCode));
                        }
                        else
                        {
                            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, itemType, 0, 0, ClassCode, 0);
                        }
                        singleRec.ItemDetail.Add(itemDtl);
                    }
                }
                else
                {
                    for (int i = 1; i <= singleRec.NumberOfItems; i++)
                    {
                        if (singleRec.ActualAmount > 0)
                        {
                            if (singleRec.ActualAmount == oneCent)
                            {
                                itemAmount = 1;
                            }
                            else
                            {
                                itemAmount = decimal.ToInt32(singleRec.ActualAmount);
                            }
                            itemDecimalAmount = singleRec.ActualAmount;
                        }
                        if (sItemReasonCode.Length > 0)
                        {
                            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, ItemType, 0, 0, ClassCode, string.Empty, 0, 0, int.Parse(sItemReasonCode));
                        }
                        else
                        {
                            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, ItemType, 0, 0, ClassCode, 0);
                        }
                        singleRec.ItemDetail.Add(itemDtl);
                    }
                    singleRec.NumberOfItems = 0;
                }

                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error on Loyalty#: " + singleRec.AltCardNumber + ", Txn#: " + singleRec.TransactionNumber + " = " + e.Message);
                //filecommon.LogError(singleRec.RawTransactionString, "ReadConfigRecord - " + e.Message);
                //++filecommon.m_nErrors;
                //singleRec = null;
                return false;
            }

        }

        public bool ProcessBulkTlogRecord(string loyaltyNumber, DateTime txnDate, int txnNumber, out TlogRecordClass singleRec, out SKUItemDetail itemDtl, ref int numberOfRecords)
        {
            bool returnVal = false;
            string strRawRecord;
            singleRec = null;
            lstTenderTypes = new ArrayList();

            string[] parsedSKUs = new string[100];
            string[] parsedTenderTypes = new string[3];

            char[] param = new char[1];
            param[0] = ',';

            char[] paramTenderType = new char[1];
            paramTenderType[0] = '|';

            char[] paramSKU = new char[1];
            paramSKU[0] = '|';

            int ClassCode = 0;

            string sSKUNumber = string.Empty;
            string sItemReasonCode = string.Empty;
            int itemAmount = 2500;
            decimal itemDecimalAmount = 2500;
            const decimal oneCent = .01M;
            int itemType = 1;
            int columnNumber = 0;

            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, string.Empty, 0, 0, 0, 0, 0, 0, 0);

            try
            {

                returnVal = true;
                singleRec = new TlogRecordClass();
                singleRec.AltCardNumber = loyaltyNumber;
                ++columnNumber;

                singleRec.PurchaseDate = txnDate.ToShortDateString();
                if (singleRec.PurchaseDate.Length == 0)
                {
                    return false;
                }

                if (singleRec.AltCardNumber == "70101730476299")
                {
                    string stop = "Stop";
                }
                singleRec.WebOrderNumber = string.Empty;
                singleRec.NumberOfItems = 1;
                singleRec.TransType = 0;
                singleRec.TenderDetail.Add(new TenderTypeDetail(71, 0, 10));
                singleRec.TenderType = 0;
                singleRec.EmployeeNumber = string.Empty;
                singleRec.ReasonCode = string.Empty;
                ItemType = 1;
                singleRec.StoreCode = 62;
                ClassCode = 100;
                sSKUNumber = "11168127";
                singleRec.ActualAmount = 10000;
                singleRec.PurchaseTime = string.Empty;
                singleRec.Credits = 0;
                singleRec.Country = string.Empty;
                singleRec.GiftCardMode = 0;
                singleRec.String77 = string.Empty;
                singleRec.TransactionNumber = txnNumber.ToString();
                singleRec.OriginalStoreCode = 0;
                singleRec.AltStoreCode = 0;
                singleRec.String181Amount = 0;
                singleRec.String182Amount = 0;
                singleRec.GiftCertificateAmount = 0;
                sItemReasonCode = string.Empty;
                itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, ItemType, 0, 0, ClassCode, 0);
                singleRec.ItemDetail.Add(itemDtl);

                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error on Loyalty#: " + singleRec.AltCardNumber + ", Txn#: " + singleRec.TransactionNumber + " = " + e.Message);
                //filecommon.LogError(singleRec.RawTransactionString, "ReadConfigRecord - " + e.Message);
                //++filecommon.m_nErrors;
                //singleRec = null;
                return returnVal;
            }

        }

        public bool ReadConfigRecord(StreamReader sr, out TlogRecordClass singleRec, out SKUItemDetail itemDtl, ref int numberOfRecords)
        {
            bool returnVal = false;
            string strRawRecord;
            singleRec = null;
            lstTenderTypes = new ArrayList();

            string[] parsedStrings = new string[0];
            string[] parsedSKUs = new string[0];
            string[] parsedTenderTypes = new string[3];

            char[] param = new char[1];
            param[0] = ',';

            char[] paramTenderType = new char[1];
            paramTenderType[0] = '|';

            char[] paramSKU = new char[1];
            paramSKU[0] = '|';

            int ClassCode = 0;

            string sSKUNumber = string.Empty;
            string sItemReasonCode = string.Empty;
            int itemAmount = 2500;
            decimal itemDecimalAmount = 2500;
            const decimal oneCent = .01M;
            int itemType = 1;

            itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, string.Empty, 0, 0, 0, 0, 0, 0, 0);

            try
            {
                strRawRecord = sr.ReadLine();

                while (true)
                {
                    ++numberOfRecords;
                    //lblStatus.Text = "Processing record #" + numberOfRecords.ToString();
                    System.Windows.Forms.Application.DoEvents();
                    if (strRawRecord == null)
                    {
                        break;
                        //strRawRecord = sr.ReadLine();
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
                    singleRec = new TlogRecordClass();
                    singleRec.RawTransactionString = strRawRecord;
                    singleRec.AltCardNumber = parsedStrings[0].Trim();
                    singleRec.PurchaseDate = parsedStrings[1].Trim();
                    ++TransactionNumber;
                    singleRec.TransactionNumber = TransactionNumber.ToString();

                    if (singleRec.AltCardNumber == "70302147828905")
                    {
                        string stop = "Stop";
                    }
                    try
                    {
                        singleRec.WebOrderNumber = parsedStrings[2].Trim();
                    }
                    catch
                    {
                        singleRec.WebOrderNumber = string.Empty;
                    }
                    try
                    {
                        singleRec.NumberOfItems = int.Parse(parsedStrings[3].Trim());
                    }
                    catch
                    {
                        singleRec.NumberOfItems = 1;
                    }
                    try
                    {
                        singleRec.TransType = int.Parse(parsedStrings[4].Trim());
                    }
                    catch
                    {
                        singleRec.TransType = 0;
                    }
                    try
                    {
                        sTenderType = parsedStrings[5].Trim();
                        if (sTenderType.IndexOf("|") > 0)
                        {
                            lstTenderTypes.AddRange(sTenderType.Split(paramTenderType, 3));
                            foreach (string s in lstTenderTypes)
                            {
                                int TenderType = int.Parse(s);
                                singleRec.TenderDetail.Add(
                                    new TenderTypeDetail(
                                    TenderType,
                                    0,
                                    10));
                            }
                        }
                        else if (sTenderType.Length > 0)
                        {
                            singleRec.TenderDetail.Add(
                                new TenderTypeDetail(
                                int.Parse(sTenderType),
                                0,
                                10));
                        }
                        else
                        {
                            singleRec.TenderDetail.Add(
                                new TenderTypeDetail(
                                71,
                                0,
                                10));
                        }
                    }
                    catch (Exception e)
                    {
                        singleRec.TenderType = 0;
                    }
                    try
                    {
                        singleRec.EmployeeNumber = parsedStrings[6].Trim();
                    }
                    catch
                    {
                        singleRec.EmployeeNumber = string.Empty;
                    }
                    try
                    {
                        singleRec.ReasonCode = parsedStrings[7].Trim();
                    }
                    catch
                    {
                        singleRec.ReasonCode = string.Empty;
                    }
                    try
                    {
                        ItemType = int.Parse(parsedStrings[8].Trim());
                    }
                    catch
                    {
                        ItemType = 1;
                    }
                    try
                    {
                        singleRec.StoreCode = int.Parse(parsedStrings[9].Trim());
                    }
                    catch
                    {
                        singleRec.StoreCode = 0;
                    }
                    try
                    {
                        ClassCode = int.Parse(parsedStrings[10].Trim());
                    }
                    catch
                    {
                        ClassCode = 0;
                    }
                    try
                    {
                        sSKUNumber = parsedStrings[11].Trim();
                        if (sSKUNumber.Contains("|"))
                        {
                            parsedSKUs = sSKUNumber.Split(paramSKU);
                        }
                    }
                    catch
                    {
                        sSKUNumber = string.Empty;
                    }
                    try
                    {
                        string amount = parsedStrings[12].Trim();
                        singleRec.ActualAmount = decimal.Parse(parsedStrings[12].Trim());
                        if (!amount.Contains("."))
                        {
                            singleRec.ActualAmount = singleRec.ActualAmount * 100;
                        }
                    }
                    catch
                    {
                        singleRec.ActualAmount = 0;
                    }
                    try
                    {
                        singleRec.PurchaseTime = parsedStrings[13].Trim();
                    }
                    catch
                    {
                        singleRec.PurchaseTime = string.Empty;
                    }
                    try
                    {
                        singleRec.Credits = int.Parse(parsedStrings[14].Trim());
                    }
                    catch
                    {
                        singleRec.Credits = 0;
                    }
                    try
                    {
                        singleRec.Country = parsedStrings[15].Trim();
                    }
                    catch
                    {
                        singleRec.Country = string.Empty;
                    }
                    try
                    {
                        singleRec.GiftCardMode = int.Parse(parsedStrings[16].Trim());
                    }
                    catch
                    {
                        singleRec.GiftCardMode = 0;
                    }
                    try
                    {
                        singleRec.String77 = parsedStrings[17].Trim();
                    }
                    catch
                    {
                        singleRec.String77 = string.Empty;
                    }
                    try
                    {
                        if (parsedStrings[18].Trim().Length > 0)
                        {
                            singleRec.TransactionNumber = parsedStrings[18].Trim();
                        }
                    }
                    catch { }
                    if (singleRec.TransactionNumber == "153963")
                    {
                        string stop = "stop";
                    }
                    try
                    {
                        singleRec.OriginalTransactionNumber = parsedStrings[19].Trim();
                    }
                    catch { }
                    try
                    {
                        singleRec.OriginalStoreCode = int.Parse(parsedStrings[20].Trim());
                    }
                    catch
                    {
                        singleRec.OriginalStoreCode = 0;
                    }
                    try
                    {
                        singleRec.OriginalPurchaseDate = parsedStrings[21].Trim();
                    }
                    catch { }
                    try
                    {
                        singleRec.OriginalTenderType = int.Parse(parsedStrings[22].Trim());
                    }
                    catch { }
                    try
                    {
                        singleRec.OriginalWebOrderNumber = parsedStrings[23].Trim();
                    }
                    catch { }
                    try
                    {
                        singleRec.AltTransactionNumber = parsedStrings[24].Trim();
                    }
                    catch { }
                    try
                    {
                        singleRec.AltStoreCode = int.Parse(parsedStrings[25].Trim());
                    }
                    catch
                    {
                        singleRec.AltStoreCode = 0;
                    }
                    try
                    {
                        singleRec.AltTransactionDate = parsedStrings[26].Trim();
                    }
                    catch { }

                    if (singleRec.WebOrderNumber == "W8901")
                    {
                        string stop = "stop";
                    }
                    try
                    {
                        singleRec.String181Amount = decimal.Parse(parsedStrings[27].Trim());
                    }
                    catch
                    {
                        singleRec.String181Amount = 0;
                    }
                    try
                    {
                        singleRec.String182Amount = decimal.Parse(parsedStrings[28].Trim());
                    }
                    catch
                    {
                        singleRec.String182Amount = 0;
                    }
                    try
                    {
                        singleRec.GiftCertificateAmount = decimal.Parse(parsedStrings[29].Trim());
                    }
                    catch
                    {
                        singleRec.GiftCertificateAmount = 0;
                    }


                    try
                    {
                        sItemReasonCode = parsedStrings[30].Trim();
                    }
                    catch
                    {
                        sItemReasonCode = string.Empty;
                    }

                    // PI 26487, Akbar, Assigning value to single use coupon fields starts 
                    try
                    {
                        singleRec.SingleUseCoupon = parsedStrings[31].Trim();
                    }
                    catch
                    {
                        singleRec.SingleUseCoupon = string.Empty;
                    }

                    try
                    {
                        singleRec.SUCNotValid = parsedStrings[32].Trim();
                    }
                    catch
                    {
                        singleRec.SUCNotValid = string.Empty;
                    }
                    // PI 26487, Akbar, Assigning value to properties single use coupon fields end

                    if (singleRec.TransactionNumber == "089043")
                    {
                        string stop = "stop";
                    }
                    if (parsedSKUs.Length > 0)
                    {
                        singleRec.NumberOfItems = 0;
                        for (int i = 0; i <= parsedSKUs.Length - 1; i = i + 4)
                        {
                            string sClass = parsedSKUs[i].Trim();
                            string ssku = parsedSKUs[i + 1].Trim();
                            string sAmt = parsedSKUs[i + 2].Trim();
                            string sTxnType = parsedSKUs[i + 3].Trim();
                            ClassCode = int.Parse(parsedSKUs[i].Trim());
                            sSKUNumber = parsedSKUs[i + 1].Trim();
                            itemAmount = decimal.ToInt32(decimal.Parse(sAmt));
                            itemDecimalAmount = decimal.Parse(sAmt);
                            itemType = int.Parse(sTxnType);
                            if (sItemReasonCode.Length > 0)
                            {
                                itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, itemType, 0, 0, ClassCode, string.Empty, 0, 0, int.Parse(sItemReasonCode));
                            }
                            else
                            {
                                itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, itemType, 0, 0, ClassCode, 0);
                            }
                            singleRec.ItemDetail.Add(itemDtl);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= singleRec.NumberOfItems; i++)
                        {
                            if (singleRec.ActualAmount > 0)
                            {
                                if (singleRec.ActualAmount == oneCent)
                                {
                                    itemAmount = 1;
                                }
                                else
                                {
                                    itemAmount = decimal.ToInt32(singleRec.ActualAmount);
                                }
                                itemDecimalAmount = singleRec.ActualAmount;
                            }
                            if (sItemReasonCode.Length > 0)
                            {
                                itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, ItemType, 0, 0, ClassCode, string.Empty, 0, 0, int.Parse(sItemReasonCode));
                            }
                            else
                            {
                                itemDtl = new SKUItemDetail(1, itemAmount, itemDecimalAmount, sSKUNumber, 0, 0, ItemType, 0, 0, ClassCode, 0);
                            }
                            singleRec.ItemDetail.Add(itemDtl);
                        }
                        singleRec.NumberOfItems = 0;
                    }

                }

                return returnVal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //filecommon.LogError(singleRec.RawTransactionString, "ReadConfigRecord - " + e.Message);
                //++filecommon.m_nErrors;
                //singleRec = null;
                return returnVal;
            }

        }
        private void ProcessMultipleSKUNumbers(string SKUNumbers, ref TlogRecordClass singleRec)
        {
            int Position = 0;
            string SKUNumber = string.Empty;
            decimal ActualAmount = 0;
            int RetailAmount = 0;
            int ReasonCode = 0;
            string[] parsedStrings = new string[100];
            char[] param = new char[1];
            param[0] = '|';

            try
            {
                SKUItemDetail itemDtl = new SKUItemDetail(1, 2500, 25, string.Empty, 0, 0, 0, 0, 0, 0, string.Empty, 0, 0, 0);

                singleRec.NumberOfItems = 0;
                parsedStrings = SKUNumbers.Split(param);
                int NumberOfItems = parsedStrings.Length;
                if (NumberOfItems % 3 != 0)
                {
                    throw new Exception("ProcessMultipleSKUNumber: " + "SKU Number should have 3 items.  SKU Number, Amount, Reason Code");
                }

                for (int i = 0; i < parsedStrings.Length; i++)
                {
                    SKUNumber = parsedStrings[i]; i++;
                    ActualAmount = decimal.Parse(parsedStrings[i]); i++;
                    RetailAmount = (int)ActualAmount;
                    Position = SKUNumbers.IndexOf("|");
                    ReasonCode = int.Parse(parsedStrings[i]);
                    itemDtl = new SKUItemDetail(1, RetailAmount, ActualAmount, SKUNumber, 0, 0, ItemType, 0, 0, 0, string.Empty, 0, 0, ReasonCode);
                    if (ReasonCode > 0)
                    {
                        singleRec.NumberOfItems++;
                    }
                    singleRec.NumberOfItems++;
                    singleRec.ItemDetail.Add(itemDtl);
                }
                //Position = SKUNumbers.IndexOf("|");
                //while (Position > 0)
                //{
                //    SKUNumber = SKUNumbers.Substring(0, Position);
                //    SKUNumbers = SKUNumbers.Substring(Position + 1);
                //    Position = SKUNumbers.IndexOf("|");
                //    if (Position < 0)
                //    {
                //        Position = SKUNumbers.Length;
                //    }
                //    ActualAmount = decimal.Parse(SKUNumbers.Substring(0, Position));
                //    RetailAmount = (int)ActualAmount;
                //    Position = SKUNumbers.IndexOf("|");
                //    ReasonCode = int.Parse(SKUNumbers.Substring(0, Position));
                //    itemDtl = new SKUItemDetail(1, RetailAmount, ActualAmount, SKUNumber, 0, 0, ItemType, 0, 0, 0, string.Empty, 0, 0, ReasonCode);
                //    singleRec.NumberOfItems++;
                //    singleRec.ItemDetail.Add(itemDtl);
                //    if (Position < SKUNumbers.Length)
                //    {
                //        SKUNumbers = SKUNumbers.Substring(Position + 1);
                //        Position = SKUNumbers.IndexOf("|");
                //    }
                //    else
                //    {
                //        Position = 0;
                //    }
                //}
            }
            catch (Exception e)
            {
                throw new Exception("ProcessMultipleSKUNumber: " + e.Message);
            }
        }
        public string WriteTlogFile(List<TlogRecordClass> tlogList, string fileType, out string path)
        {
            TextWriter output = null;
            string FileName;
            int NumberOfRecords = 0;
            TlogRecordClass AERec = null;


            TransactionNumber = 50000;
            path = string.Empty;

            if (null == ConfigurationManager.AppSettings["OutputPath"])
            {
                MessageBox.Show("No path defined in app.config");
                return string.Empty;
            }
            else
            {
                path = ConfigurationManager.AppSettings["OutputPath"] + @"\Tlogs\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            //path = Application.StartupPath.Replace(@"bin\Debug", string.Empty);

            //path += @"Tlogs\";
            //IFCLTLG050530052332.TXT
            FileName = path + "IFCLTLG" + DateTime.Now.ToString("yyMMdd") + "052332_" + fileType + ".TXT";
            if (!File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            output = File.CreateText(FileName);


            try
            {
                foreach (TlogRecordClass rec in tlogList)
                {

                    AERec = rec;
                    string TransactionDate = string.Empty;
                    string LoyaltyNumber = AERec.AltCardNumber;
                    string ReasonCode = AERec.ReasonCode;
                    // PI 26487, Akbar, Single use coupon fields declaration starts
                    string SingleUseCoupon = AERec.SingleUseCoupon;
                    string SUCNotValid = AERec.SUCNotValid;
                    // PI 26487, Akbar, Single use coupon fields declaration ends
                    string HeaderComment = string.Empty;


                    try
                    {
                        TransactionDate = DateTime.Parse(AERec.PurchaseDate).ToString("MMddyy");
                    }
                    catch (Exception e1)
                    {
                        TransactionDate = DateTime.Now.ToString("MMddyy");
                    }

                    if (LoyaltyNumber == "70101730476299")
                    {
                        string stop = "Stop";
                    }
                    if (AERec.TransactionNumber == "089013")
                    {
                        string stop = "stop";
                    }

                    if (AERec.TenderType == 0)
                    {
                        TenderType = 71;
                    }
                    else
                    {
                        TenderType = AERec.TenderType;
                    }
                    RetailAmount = 5000;

                    NumberOfRecords = 0;
                    if (AERec.NumberOfItems == 0)
                    {
                        NumberOfRecords = 0;
                    }
                    else
                    {
                        NumberOfRecords += AERec.NumberOfItems;
                    }
                    if ((AERec.TransType > 0) && (AERec.OriginalStoreCode > 0))
                    {
                        NumberOfRecords++;
                    }

                    if (ReasonCode.Length > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (AERec.TenderDetail.Count > 1)
                    {
                        NumberOfRecords++;
                    }

                    if (AERec.WebOrderNumber.Length > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (AERec.GiftCardMode > 0)
                    {
                        if (AERec.String77 != "C")
                        {
                            NumberOfRecords++;
                        }
                    }
                    if (AERec.String77.Length > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (AERec.String181Amount > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (AERec.String182Amount > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (AERec.GiftCertificateAmount > 0)
                    {
                        NumberOfRecords++;
                    }
                    if (LoyaltyNumber.Length > 0)
                    {
                        NumberOfRecords++;
                    }
                    // if (foundorderfl)
                    //  {
                    //      NumberOfRecords++;
                    //  }

                    foreach (SKUItemDetail item in AERec.ItemDetail)
                    {
                        if (item.ActualAmount > 0 && item.ClassCode > 0) 
                        {
                            NumberOfRecords++;
                        }
                    }
                    HeaderComment = BuildHeaderComment(AERec);
                    //output.WriteLine("--");
                    //output.WriteLine("--" + HeaderComment);
                    //output.WriteLine("--");
                    if (AERec.PurchaseTime.Length > 0)
                    {
                        output.WriteLine(BuildHeaderRecord(NumberOfRecords, AERec.EmployeeNumber, TransactionDate, AERec.PurchaseTime, AERec.WebOrderNumber, AERec.StoreCode.ToString(), AERec.Country, AERec.TransactionNumber, AERec.TransType.ToString()));
                    }
                    else
                    {
                        output.WriteLine(BuildHeaderRecord(NumberOfRecords, AERec.EmployeeNumber, TransactionDate, "1438", AERec.WebOrderNumber, AERec.StoreCode.ToString(), AERec.Country, AERec.TransactionNumber, AERec.TransType.ToString()));
                    }

                    if (LoyaltyNumber.Length > 0)
                    {
                        output.WriteLine(Build2793Record(LoyaltyNumber));
                    }

                    if ((AERec.TransType > 0) && (AERec.OriginalStoreCode > 0))
                    {
                        output.WriteLine(Build2733Record(AERec));
                    }
                    if (ReasonCode.Length > 0)
                    {
                        // PI 26487, Akbar, Calling method with single use coupon fields
                        output.WriteLine(Build2751Record(ReasonCode, SingleUseCoupon, SUCNotValid, false));
                    }
                    if ((AERec.WebOrderNumber.Length > 0) || (AERec.OriginalWebOrderNumber.Length > 0))
                    {
                        output.WriteLine(BuildWebRecord(AERec));
                    }
                    if (AERec.GiftCardMode > 0)
                    {
                        if (AERec.String77 != "C")
                        {
                            output.WriteLine(Build2762Record());
                        }
                    }
                    if (AERec.String77.Length > 0)
                    {
                        output.WriteLine(Build77Record(AERec));
                    }
                    if (AERec.String181Amount > 0)
                    {
                        output.WriteLine(Build18Record(AERec, 1));
                    }
                    if (AERec.String182Amount > 0)
                    {
                        output.WriteLine(Build18Record(AERec, 2));
                    }
                    if (AERec.GiftCertificateAmount > 0)
                    {
                        output.WriteLine(Build05Record(AERec));
                    }

                    if (AERec.TransactionNumber == "400011")
                    {
                        string stop = "stop";
                    }
                    foreach (SKUItemDetail item in AERec.ItemDetail)
                    {
                        if (item.ActualAmount > 0 && item.ClassCode > 0) 
                        {
                            output.WriteLine(Build01Record(item, AERec.TransType, AERec));
                            if (item.ReasonCode > 0)
                            {
                                // PI 26487, Akbar, Calling with changed parameters
                                output.WriteLine(Build2751Record(item.ReasonCode.ToString(), "", "", true));
                            }
                        }
                    }
                    if (foundorderfl)
                    {
                        output.WriteLine(Build14Record(AERec)); // write found order
                    }
                    output.WriteLine(Build04Record(1, AERec));
                    output.WriteLine(Build04Record(2, AERec));
                    foreach (TenderTypeDetail tender in AERec.TenderDetail)
                    {
                        output.WriteLine(Build04Record(tender));
                    }
                }
                output.Close();
                return "Tlog file ( " + FileName + ") Written";

            }
            catch (Exception e)
            {
                throw new Exception("WriteTlogFile: LoyaltyNumber - " + AERec.AltCardNumber + " - " + e.Message);
            }


        }
        private string BuildHeaderComment(TlogRecordClass AERec)
        {
            string HeaderComment = string.Empty;
            if (AERec.AltCardNumber.Length > 0)
            {
                HeaderComment += "Loy# = " + AERec.AltCardNumber;
            }
            if (AERec.PurchaseDate.Length > 0)
            {
                HeaderComment += ", Txn Date = " + AERec.PurchaseDate;
            }
            if (AERec.EmployeeNumber.Length > 0)
            {
                HeaderComment += ", Emp# = " + AERec.EmployeeNumber;
            }
            if (AERec.NumberOfItems > 1)
            {
                HeaderComment += ", # Of Items = " + AERec.NumberOfItems;
            }
            if (AERec.WebOrderNumber.Length > 0)
            {
                HeaderComment += ", Web Order = " + AERec.WebOrderNumber;
            }
            if (AERec.TenderType > 0)
            {
                HeaderComment += ", Tender Type = " + AERec.TenderType;
            }
            foreach (SKUItemDetail item in AERec.ItemDetail)
            {
                if ((item.ClassCode != 315) && (item.ClassCode > 0))
                {
                    HeaderComment += ", ClassCode = " + item.ClassCode;
                    break;
                }
                if (item.SKUNumber != "10075414")
                {
                    HeaderComment += ", SKUNumber = " + item.SKUNumber;
                    break;
                }
            }
            if (AERec.Credits > 0)
            {
                HeaderComment += ", Credits = " + AERec.Credits;
            }
            if (AERec.Country.Length > 0)
            {
                HeaderComment += ", Country = " + AERec.Country;
            }
            if (AERec.StoreCode > 0)
            {
                HeaderComment += ", StoreCode = " + AERec.StoreCode;
            }
            if (AERec.ActualAmount > 0)
            {
                HeaderComment += ", Actual Amount = " + AERec.ActualAmount;
            }
            if (AERec.PurchaseTime.Length > 0)
            {
                HeaderComment += ", Txn Time = " + AERec.PurchaseTime;
            }
            return HeaderComment;

        }
        /// <summary>
        /// 27-33 string - Original Transaction Record
        /// </summary>
        /// <param name="AERec"></param>
        /// <returns></returns>
        private string Build2733Record(TlogRecordClass AERec)
        {
            string Return = "";
            string PurchaseYear = string.Empty;
            string PurchaseMonth = string.Empty;
            string PurchaseDay = string.Empty;

            PurchaseYear = DateTime.Parse(AERec.OriginalPurchaseDate).Year.ToString().Substring(2, 2);
            PurchaseMonth = DateTime.Parse(AERec.OriginalPurchaseDate).Month.ToString().PadLeft(2, '0');
            PurchaseDay = DateTime.Parse(AERec.OriginalPurchaseDate).Day.ToString().PadLeft(2, '0');

            StringBuilder sb = new StringBuilder();
            sb.Append("27000033" + Global.MakeFixedLength(AERec.OriginalTransactionNumber, 6, "0"));
            sb.Append(Global.MakeFixedLength(AERec.OriginalStoreCode.ToString(), 5, "0"));
            sb.Append(Global.MakeFixedLength(PurchaseMonth + PurchaseDay + PurchaseYear, 6, "0"));
            sb.Append(Global.MakeFixedLength(AERec.OriginalTenderType.ToString(), 2, "0"));
            Return = sb.ToString();
            return Return;
        }
        /// <summary>
        /// 27-99 string - Web record
        /// </summary>
        /// <param name="AERec"></param>
        /// <returns></returns>
        private string BuildWebRecord(TlogRecordClass AERec)
        {
            string Web = "";
            string OrderSuffix = "000";

            string PurchaseYear = string.Empty;
            string PurchaseMonth = string.Empty;
            string PurchaseDay = string.Empty;
            DateTime TransactionDate = DateTime.MinValue;
            try
            {
                TransactionDate = DateTime.Parse(AERec.PurchaseDate);
            }
            catch (Exception e1)
            {
                TransactionDate = DateTime.Now;
            }

            PurchaseYear = TransactionDate.Year.ToString();
            PurchaseMonth = TransactionDate.Month.ToString().PadLeft(2, '0');
            PurchaseDay = TransactionDate.Day.ToString().PadLeft(2, '0');

            StringBuilder sb = new StringBuilder();
            sb.Append("27999099");
            sb.Append(Global.MakeFixedLength(AERec.WebOrderNumber, 8, " "));
            sb.Append(OrderSuffix);
            sb.Append(Global.MakeFixedLength(AERec.OriginalWebOrderNumber, 8, " "));
            sb.Append(PurchaseYear + PurchaseMonth + PurchaseDay);
            Web = sb.ToString();
            return Web;
        }
        /// <summary>
        /// 27-51 string - Gift Certificate Redemption
        /// </summary>
        /// <param name="ReasonCode"></param>
        /// <param name="SingleUseCoupon"></param>
        /// <param name="SUCNotValid"></param>
        /// <returns></returns>

        // PI 26487, Akbar, Arguments changed according to requirement
        private string Build2751Record(string ReasonCode, string SingleUseCoupon, string SUCNotValid, bool isItem)
        {
            string Discount = "";

            StringBuilder sb = new StringBuilder();
            //If it is for an item record then we need to turn the mode to 1
            if (isItem)
            {
                sb.Append("2799905110700000000000000000000000000000" + ReasonCode + SingleUseCoupon + SUCNotValid);  // PI 26487, Akbar, Added single use coupon fields
            }
            else
            {
                sb.Append("2799905120700000000000000000000000000000" + ReasonCode + SingleUseCoupon + SUCNotValid);  // PI 26487, Akbar, Added single use coupon fields
            }
            Discount = sb.ToString();
            return Discount;
        }
        /// <summary>
        /// 27-62 string - Gift Card Purchase
        /// </summary>
        /// <returns></returns>
        private string Build2762Record()
        {
            string Discount = "";

            StringBuilder sb = new StringBuilder();
            sb.Append("279990620412345678901234567891005735");
            sb.Append("9910000000000015587355  0000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000991100991000000000 0000000000010                                                        00595200910281206004020631");
            Discount = sb.ToString();
            return Discount;
        }
        private string Build77Record(TlogRecordClass AERec)
        {
            string Record = "";
            string PurchaseYear = string.Empty;
            string PurchaseMonth = string.Empty;
            string PurchaseDay = string.Empty;

            if (AERec.AltTransactionDate.Length > 0)
            {
                PurchaseYear = DateTime.Parse(AERec.AltTransactionDate).Year.ToString().Substring(2, 2);
                PurchaseMonth = DateTime.Parse(AERec.AltTransactionDate).Month.ToString().PadLeft(2, '0');
                PurchaseDay = DateTime.Parse(AERec.AltTransactionDate).Day.ToString().PadLeft(2, '0');
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("27999077");
            sb.Append(Global.MakeFixedLength(AERec.AltStoreCode.ToString(), 5, "0"));
            sb.Append("001"); //Register
            sb.Append(Global.MakeFixedLength(PurchaseMonth + PurchaseDay + PurchaseYear, 6, " "));
            sb.Append(Global.MakeFixedLength(AERec.AltTransactionNumber.ToString(), 6, "0"));
            if (AERec.String77 == "C")
            {
                sb.Append("Y");
            }
            Record = sb.ToString();
            return Record;
        }
        private string Build18Record(TlogRecordClass AERec, int RecordNumber)
        {
            string Record = "";
            int Amount = 0;

            StringBuilder sb = new StringBuilder();
            if (AERec.TransType == 0)
            {
                sb.Append("1811");
            }
            else
            {
                sb.Append("1821"); //Return
            }
            if (RecordNumber == 1)
            {
                Amount = (int)(AERec.String181Amount * 100);
            }
            else
            {
                Amount = (int)(AERec.String182Amount * 100);
            }
            sb.Append(Global.MakeFixedLength(Amount.ToString(), 10, "0"));
            sb.Append(Global.MakeFixedLength(string.Empty, 10, "0"));
            Record = sb.ToString();
            return Record;
        }
        private string Build05Record(TlogRecordClass AERec)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("05012345678912345");
            sb.Append(Global.MakeFixedLength(AERec.GiftCertificateAmount.ToString(), 10, "0"));

            return sb.ToString();
        }
        /// <summary>
        /// 04 string - Tender Record
        /// </summary>
        /// <param name="tender"></param>
        /// <returns></returns>
        private string Build04Record(TenderTypeDetail tender)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("04");
            sb.Append(Global.MakeFixedLength(tender.TenderType.ToString(), 2, "0"));
            sb.Append(Global.MakeFixedLength("1", 2, "0")); //number of tenders
            sb.Append(Global.MakeFixedLength(tender.ActualAmount.ToString(), 8, "0"));
            sb.Append("00");

            return sb.ToString();
        }


        private string Build14Record(TlogRecordClass AERec)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("14");
            sb.Append(Global.MakeFixedLength("1", 1, "0"));
            sb.Append(Global.MakeFixedLength("F", 1, "0"));
            sb.Append(Global.MakeFixedLength(AERec.ActualAmount.ToString(), 8, "0"));
            sb.Append("00");
            return sb.ToString();
        }
        private string Build04Record(int Type, TlogRecordClass AERec)
        {
            string Tender = "";

            StringBuilder sb = new StringBuilder();
            //AERec.ActualAmount = AERec.ActualAmount * 10;
            if (Type == 1)
            {
                if (AERec.ActualAmount == 0)
                {
                    sb.Append("0210000004285");
                }
                else
                {
                    sb.Append("021");
                    sb.Append(Global.MakeFixedLength(AERec.ActualAmount.ToString().Replace(".", string.Empty), 8, "0"));
                    sb.Append("00");
                }
            }
            else if (Type == 2)
            {
                if (AERec.ActualAmount == 0)
                {
                    sb.Append("03110000000279");
                }
                else
                {
                    sb.Append("0311");
                    sb.Append(Global.MakeFixedLength(AERec.ActualAmount.ToString().Replace(".", string.Empty), 8, "0"));
                    sb.Append("00");
                }
            }
            else if (Type == 3)
            {
                foreach (TenderTypeDetail tender in AERec.TenderDetail)
                {
                    sb.Append("04");
                    sb.Append(Global.MakeFixedLength(tender.TenderType.ToString(), 2, "0"));
                    sb.Append(Global.MakeFixedLength("1", 2, "0")); //number of tenders
                    sb.Append(Global.MakeFixedLength(AERec.ActualAmount.ToString().Replace(".", string.Empty), 8, "0"));
                    sb.Append("00");
                }
            }
            Tender = sb.ToString();
            return Tender;
        }
        /// <summary>
        /// 01 string - Item Record
        /// </summary>
        /// <param name="item"></param>
        /// <param name="TransType"></param>
        /// <returns></returns>
        private string Build01Record(SKUItemDetail item, int TransType, TlogRecordClass AERec)
        {
            string Item = "";
            long m_SKUNumber = 18118364;
            int m_Quantity = 1000;

            if ((item.SKUNumber != null) && (item.SKUNumber.Length > 0))
            {
                m_SKUNumber = long.Parse(item.SKUNumber);
            }

            StringBuilder sb = new StringBuilder();
            if ((AERec.String77 == "C") && (AERec.GiftCardMode > 0))
            {
                sb.Append("99");
            }
            else
            {
                sb.Append("01");
            }
            if ((TransType == 0) || (TransType == 1) || (TransType == 4))
            {
                sb.Append(Global.MakeFixedLength(item.ItemType.ToString(), 1, "0")); //Normal Sale Transaction Mode
            }
            else
            {
                sb.Append(Global.MakeFixedLength("2", 1, "0")); //Return
            }
            //item.ActualAmount = item.ActualAmount * 100;

            sb.Append(Global.MakeFixedLength("1123", 9, "0")); //salesperson Number
            sb.Append("00"); // 10-digit sku; start with 00
            sb.Append(Global.MakeFixedLength(m_SKUNumber.ToString(), 10, " "));
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //Class indicator
            sb.Append(Global.MakeFixedLength(m_Quantity.ToString(), 9, "0"));
            sb.Append(Global.MakeFixedLength(item.ActualAmount.ToString().Replace(".", string.Empty), 10, "0")); //Final Selling Price
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //EXTENDED_TOTAL                  varchar2(10)  \*045 - 054       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //ORIGINAL_PRICE                  varchar2(10)  \*055 - 064       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //PLU_PRICE                       varchar2(10)  \*065 - 074       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //PRE_DISCOUNT_PRICE              varchar2(10)  \*075 - 084       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //PRICE_ENTERED                   varchar2(10)  \*085 - 094       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //TAXABLE_STATUS                  varchar2(1)   \*095 - 095*\
            sb.Append(Global.MakeFixedLength("0", 5, "0")); //TAX_OVERRIDE_CODE               varchar2(5)   \*096 - 100*\
            sb.Append(Global.MakeFixedLength("0", 5, "0")); //EFFECTIVE_TAX_RATE              varchar2(5)   \*101 - 105       99V999*\
            sb.Append(Global.MakeFixedLength("0", 10, "0")); //AMOUNT_TAXABLE                  varchar2(10)  \*106 - 115       9(8)V99*\
            sb.Append(Global.MakeFixedLength("0", 6, "0")); //ORIGINAL_TRANSACTION_NUMBER     varchar2(6)   \*116 - 121*\
            sb.Append(Global.MakeFixedLength(item.ClassCode.ToString(), 4, "0")); //CLASS_NUMBER                    varchar2(4)   \*122 - 125*\
            sb.Append(Global.MakeFixedLength("0", 4, "0")); //DEPARTMENT_NUMBER               varchar2(4)   \*126 - 129*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //MAIN_REASON_CODE                varchar2(1)   \*130 - 130*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //SUPPLEMENTAL_REASON_CODE        varchar2(1)   \*131 - 131*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //NOT_ON_FILE_INDICATOR           varchar2(1)   \*132 - 132*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //SPLIT_TRANSACTION_INDICATOR     varchar2(1)   \*133 - 133*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //PRICE_OVERRIDE_INDICATOR        varchar2(1)   \*134 - 134*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //ON_DEAL_INDICATOR               varchar2(1)   \*135 - 135*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //IN_PACKAGE_INDICATOR            varchar2(1)   \*136 - 136*\
            sb.Append(Global.MakeFixedLength("0", 3, "0")); //BOTTLE_DEPOSIT                  varchar2(3)   \*137 - 139       9V99*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //INCENTIVE_TYPE_INDICATOR        varchar2(1)   \*140 - 140*\
            sb.Append(Global.MakeFixedLength("0", 5, "0")); //INCENTIVE_DOLLARS               varchar2(5)   \*141 - 145       999V99*\
            sb.Append(Global.MakeFixedLength("0", 5, "0")); //INCENTIVE_POINTS                varchar2(5)   \*146 - 150*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //LUXURY_TAX_BREAK_INDICATOR      varchar2(1)   \*151 - 151       1 - 9*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //FLAT_RATE_TAX_BREAK_INDICATOR   varchar2(1)   \*152 - 152       1 - 9*\
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //DEAL_RECORD_NUMBER              varchar2(1)   \*153 - 153*\
            sb.Append(Global.MakeFixedLength("0", 3, "0")); //COLOR                           varchar2(3)   \*154 - 156*\
            sb.Append(Global.MakeFixedLength("0", 3, "0")); //SIZE_                           varchar2(3)   \*157 - 159*\
            sb.Append(Global.MakeFixedLength("0", 3, "0")); //WIDTH                           varchar2(3)   \*160 – 162*\
            sb.Append(Global.MakeFixedLength("0", 3, "0")); //MARKDOWN_FLAG                   varchar2(3)   \*236 - 236*\            

            Item = sb.ToString();
            return Item;
        }
        private void GetNextStoreNumber(string Country)
        {
            if (Country.Length > 0)
            {
                StoreNumber = 901;
            }
            else
            {
                if (StoreNumber == 0)
                {
                    StoreNumber = 62;
                    return;
                }
                ++StoreNumber;
                if (StoreNumber > 97)
                {
                    StoreNumber = 62;
                    return;
                }
            }

        }
        public string BuildHeaderRecord(int NumberOfItems, string EmployeeNumber, string TransactionDate, string TransactionTime, string OrderNumber, string StoreCode, string Country, string TransactionNumber, string TransactionType)
        {
            string Header = "";
            int NumberOfRecords = 4;
            int iStore;

            if (OrderNumber.Length > 0)
            {
                if (StoreCode.Length > 0 && StoreCode != "0")
                {
                    iStore = int.Parse(StoreCode);
                }
                else
                {
                    iStore = 657;
                }
            }
            else
            {
                if (StoreCode.Length > 0 && StoreCode != "0")
                    iStore = int.Parse(StoreCode);
                else
                {
                    GetNextStoreNumber(Country);
                    iStore = StoreNumber;
                }
            }

            if (TransactionDate.Length == 0)
            {
                TransactionDate = DateTime.Now.ToString("MMddyy");
            }
            if ((TransactionType.Length == 0) || (TransactionType == "0") || (TransactionType == "1"))
            {
                TransactionType = "0";
                foundorderfl = false;
            }
            if (TransactionType == "4")
            {
                TransactionType = "0";
                foundorderfl = true; // setting found order flag =  true
                ++NumberOfRecords;
            }
            if (TransactionType == "2")
            // else
            {
                TransactionType = "1";
                foundorderfl = false;
            }
            NumberOfRecords += NumberOfItems;

            StringBuilder sb = new StringBuilder();
            sb.Append("00");
            sb.Append(Global.MakeFixedLength(iStore.ToString(), 5, "0"));
            sb.Append(Global.MakeFixedLength(RegisterNumber, 3, " "));
            sb.Append(Global.MakeFixedLength("1123", 9, "0")); //Cashier Number
            sb.Append(Global.MakeFixedLength(TransactionNumber, 6, "0"));
            sb.Append(Global.MakeFixedLength(TransactionDate, 6, " "));
            sb.Append(Global.MakeFixedLength(TransactionTime, 4, " "));
            sb.Append(Global.MakeFixedLength("0", 1, "0")); //Transaction Mode
            sb.Append(Global.MakeFixedLength(TransactionType, 2, "0"));
            sb.Append(Global.MakeFixedLength("0", 24, "0")); //filler
            sb.Append(Global.MakeFixedLength(EmployeeNumber, 9, "0")); //EmployeeNumber
            sb.Append(Global.MakeFixedLength("0", 25, "0")); //filler
            sb.Append(Global.MakeFixedLength(NumberOfRecords.ToString(), 5, "0")); //Number of strings

            Header = sb.ToString();
            return Header;
        }
        /// <summary>
        /// 27-93 string - Loyalty Number Record
        /// </summary>
        /// <param name="LoyaltyNumber"></param>
        /// <returns></returns>
        private string Build2793Record(string LoyaltyNumber)
        {
            string LoyaltyNumberRecord = "";

            StringBuilder sb = new StringBuilder();
            sb.Append("27999093");
            sb.Append(Global.MakeFixedLength(LoyaltyNumber, 14, "0"));

            LoyaltyNumberRecord = sb.ToString();
            return LoyaltyNumberRecord;
        }

    }
}
