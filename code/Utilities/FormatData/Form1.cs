using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormatData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string textInput = string.Empty;
            string[] textList = new string[50];
            StringBuilder sb = new StringBuilder();
            int itemCount = 0;

            textInput = txtInput.Text.Replace("\r\n", ",");
            
            textList = textInput.Split(',');
            itemCount = textList.Count();

            foreach (string s in textList)
            {
                if(s == "71030282583236")
                {
                    string stop = "stop";
                }
                if (s.Length > 0)
                {
                    sb.Append(BuildItem(s, itemCount));
                }
                itemCount--;
            }
            txtOutput.Text = sb.ToString();
            Clipboard.SetDataObject(txtOutput.Text, true);
        }
        private string BuildItem(string item, int itemCount)
        {
            string returnValue = string.Empty;

            if (chkLineBreaks.Checked)
            {
                if (itemCount == 1)
                {
                    if (chkIncludeQuotes.Checked)
                    {
                        returnValue = "'" + item + "'\r\n";
                    }
                    else
                    {
                        returnValue = item + "\r\n";
                    }

                }
                else
                {
                    if (chkIncludeQuotes.Checked)
                    {
                        returnValue = "'" + item + "',\r\n";
                    }
                    else
                    {
                        returnValue = item + ",\r\n";
                    }
                }
            }
            else
                if (itemCount == 1)
                {
                    if (chkIncludeQuotes.Checked)
                    {
                        returnValue = "'" + item + "',\r\n";
                    }
                    else
                    {
                        returnValue = item;
                    }
                }
                else
                {
                    if (chkIncludeQuotes.Checked)
                    {
                        returnValue = "'" + item + "',";
                    }
                    else
                    {
                        returnValue = item + ",";
                    }
                }

            return returnValue;
        }
    }
}
