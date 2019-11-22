using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FormatDBScript
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string recordIn = string.Empty;
            string recordOut = string.Empty;
            string inputFileName = string.Empty;
            string outputFileName = string.Empty;
            string path = string.Empty;

            if (txtScriptName.Text.Length == 0)
            {
                MessageBox.Show("Please enter a script path ");
                return;
            }
            InitMaster();
            if (chkLoyaltyMember.Checked)
            {
                ProcessFile("1 - LoyaltyMember");
            }
            if (chkMemberDetails.Checked)
            {
                ProcessFile("2 - Memberdetails");
            }
            if (chkVirtualCard.Checked)
            {
                ProcessFile("3 - Virtualcard");
            }
            if (chkTxnHeader.Checked)
            {
                ProcessFile("4a - TxnHeader");
            }
            if (chkTxnDetailItem.Checked)
            {
                ProcessFile("4b - TxnDetailitem");
            }
            if (chkMemberReceipts.Checked)
            {
                ProcessFile("4d - MemberReceipts");
            }
            if (chkMemberBraSummary.Checked)
            {
                ProcessFile("5c - MemberBraSummary");
            }
            if (chkPointTransaction.Checked)
            {
                ProcessFile("6 - Pointtransaction");
            }
            MessageBox.Show("Process complete");

        }
        private void InitMaster()
        {
            string path = string.Empty;
            string inputFileName = string.Empty;
            string outputFileName = string.Empty;
            char quote = '"';

            path = txtScriptName.Text;
            outputFileName = path + @"\99 - Download_Master.sql";
            StreamWriter sw = new StreamWriter(outputFileName, false);

            sw.WriteLine("SET echo ON");
            sw.WriteLine("SET feedback ON");
            sw.WriteLine("SET serveroutput ON");
            sw.WriteLine("set timing ON");
            sw.Close();
        }
        private void WriteMaster(string fileName)
        {
            string path = string.Empty;
            string inputFileName = string.Empty;
            string outputFileName = string.Empty;
            char quote = '"';

            path = txtScriptName.Text;
            inputFileName = path + @"\" + fileName + ".sql";
            outputFileName = path + @"\99 - Download_Master.sql";
            StreamWriter sw = new StreamWriter(outputFileName, true);

            sw.WriteLine("@" + quote + path + @"\" + fileName + ".sql" + quote);
            sw.Close();
        }
        private void ProcessFile(string fileName)
        {
            string inputFileName = string.Empty;
            string outputFileName = string.Empty;
            string path = string.Empty;

            path = txtScriptName.Text;
            inputFileName = path + @"\" + fileName + ".bak";

            if (!chkWriteMasterOnly.Checked)
            {
                if (File.Exists(inputFileName))
                {
                    File.Delete(inputFileName);
                }
                if (fileName == "6 - Pointtransaction")
                {
                    inputFileName = path + @"\" + fileName + ".sql";
                    if (File.Exists(inputFileName))
                    {
                        ProcessPointTransactionFile(inputFileName);
                    }
                }
                else
                {
                    inputFileName = path + @"\" + fileName + ".sql";
                    if (File.Exists(inputFileName))
                    {
                        ProcessOtherFiles(inputFileName);
                    }
                }
            }
            WriteMaster(fileName);
        }
        private void ProcessOtherFiles(string inputFileName)
        {
            string recordIn = string.Empty;
            string recordOut = string.Empty;
            string outputFileName = string.Empty;

            outputFileName = inputFileName.Replace(".sql", ".txt");
            StreamReader sr = new StreamReader(inputFileName);
            StreamWriter sw = new StreamWriter(outputFileName);
            recordIn = sr.ReadLine();
            sw.WriteLine("----------------------------");
            sw.WriteLine("-- " + inputFileName + " --");
            sw.WriteLine("----------------------------");

            while (true)
            {
                if (recordIn == null)
                {
                    sw.WriteLine("Commit;");
                    sr.Close();
                    sw.Close();
                    break;
                }
                sw.WriteLine(recordIn);
                recordIn = sr.ReadLine();
            }
            File.Copy(inputFileName, inputFileName.Replace(".sql", ".bak"));
            File.Delete(inputFileName);
            File.Copy(outputFileName, inputFileName);
            File.Delete(outputFileName);
        }
        private void ProcessPointTransactionFile(string inputFileName)
        {
            string recordIn = string.Empty;
            string recordOut = string.Empty;
            string outputFileName = string.Empty;

            outputFileName = inputFileName.Replace(".sql", ".txt");
            StreamReader sr = new StreamReader(inputFileName);
            StreamWriter sw = new StreamWriter(outputFileName);
            recordIn = sr.ReadLine();
            sw.WriteLine("----------------------------");
            sw.WriteLine("-- " + inputFileName + " --");
            sw.WriteLine("----------------------------");

            while (true)
            {
                if (recordIn == null)
                {
                    sw.WriteLine("Commit;");
                    sr.Close();
                    sw.Close();
                    break;
                }
                recordOut = recordIn.Replace("'xxpointtypexx', ", "(select pointtypeid from bp_ae.lw_pointtype where name = ");
                recordOut = recordOut.Replace(", 'yypointtypeyy'", ")");
                recordOut = recordOut.Replace("'xxpointeventxx', ", "(select pointeventid from bp_ae.lw_pointevent where name = ");
                recordOut = recordOut.Replace(", 'yypointeventyy'", ")");
                recordOut = recordOut.Replace("POINTTYPEPREFIX, NAME, ", "pointtypeid, ");
                recordOut = recordOut.Replace("POINTTYPESUFFIX, POINTEVENTPREFIX, NAME, POINTEVENTSUFFIX, ", "pointeventid, ");
                sw.WriteLine(recordOut);
                recordIn = sr.ReadLine();
            }
            File.Copy(inputFileName, inputFileName.Replace(".sql", ".bak"));
            File.Delete(inputFileName);
            File.Copy(outputFileName, inputFileName);
            File.Delete(outputFileName);
        }

        private void txtScriptName_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
