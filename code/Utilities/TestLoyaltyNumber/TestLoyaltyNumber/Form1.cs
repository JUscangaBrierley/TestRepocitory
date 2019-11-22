using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TestLoyaltyNumber
{
    public partial class Form1 : Form
    {
        public short digit4 = 0;
        public short digit8 = 0;
        public short digit12 = 0;
        public short digit14 = 0;

        public short checkDigit4 = 0;
        public short checkDigit8 = 0;
        public short checkDigit12 = 0;
        public short checkDigit14 = 0;

        public string rawNumber = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //80203216658876
            long loyaltyNumber = 0;

            loyaltyNumber = Convert.ToInt64(txtLoyaltyNumber.Text);

            bool isValid = IsLoyaltyNumberValid(loyaltyNumber);

            if(isValid)
            {
            lblStatus.Text = "Loyalty Number is valid";
            }
            else
            {
                lblStatus.Text = "Loyalty Number is inValid";
            }

            lblCalc4.Text = checkDigit4.ToString();
            lblCalc8.Text = checkDigit8.ToString();
            lblCalc12.Text = checkDigit12.ToString();
            lblCalc14.Text = checkDigit14.ToString();

            lblLID4.Text = digit4.ToString();
            lblLID8.Text = digit8.ToString();
            lblLID12.Text = digit12.ToString();
            lblLID14.Text = digit14.ToString();

            lblRawNumber.Text = rawNumber;
        }
        public short[] NumberToArray(long Input)
        {
            // Input 1440 and return {1, 4, 4, 0}.
            int n = 0;
            do
                n++;
            while (Input >= Math.Pow(10, n));

            short[] output = new short[n];

            while (0 < Input)
            {
                output[--n] = (short)(Input % 10);
                Input /= 10;
            }

            return output;
        }

        public bool IsLoyaltyNumberValid(long loyaltyNumber)
        {
            bool bIsValid = false;
            int digitsChecked = 0;

            checkDigit4 = 0;
            checkDigit8 = 0;
            checkDigit12 = 0;
            checkDigit14 = 0;

            digit4 = 0;
            digit8 = 0;
            digit12 = 0;
            digit14 = 0;

                // Convert long to array of shorts.
                short[] digits = NumberToArray(loyaltyNumber);

                rawNumber = digits[0].ToString() + digits[1].ToString() + digits[2].ToString() + digits[4].ToString() + digits[5].ToString() + digits[6].ToString() + digits[8].ToString() + digits[9].ToString() + digits[10].ToString() + digits[12].ToString();

                // 5 == ([1]*7+[2]*3+[3]*7) % 7
                checkDigit4 = (short)((digits[0] * 7 + digits[1] * 3 + digits[2] * 7) % 7);
                digit4 = digits[3];
                if (digit4 == checkDigit4)
                {
                    digitsChecked++;
                }

                // 9 == ([4]+[5]+[6]+[7]) % 11
                checkDigit8 = (short)(((digits[3] + digits[4] + digits[5] + digits[6]) % 11) % 10);
                digit8 = digits[7];
                if (digit8 == checkDigit8)
                {
                    digitsChecked++;
                }
                // 4 == ([1]*1+[2]*7+[3]*3+[5]*3+[6]*7+[7]*1+[8]*1+[9]*7+[10]*3+[11]*3)/13
                checkDigit12 = (short)(((digits[0] + digits[1] * 7 + digits[2] * 3 + digits[4] * 3 + digits[5] * 7
                    + digits[6] + digits[7] + digits[8] * 7 + digits[9] * 3 + digits[10] * 3) % 13) % 10);
                digit12 = digits[11];
                if (digit12 == checkDigit12)
                {
                    digitsChecked++;
                }
                // 5 == ([1]*3+[2]*1+[3]*3+[4]*1+[5]*3+[6]*1+[7]*3+[8]*1+[9]*3+[10]*1+[11]*3+[12]*1+[13]*3)/10
                checkDigit14 = (short)((digits[0] * 3 + digits[1] + digits[2] * 3 + digits[3] +
                    digits[4] * 3 + digits[5] + digits[6] * 3 + digits[7] + digits[8] * 3 +
                    digits[9] + digits[10] * 3 + digits[11] + digits[12] * 3) % 10);

                digit14 = digits[13];
                if (digit14 == checkDigit14)
                {
                    digitsChecked++;
                }

                if (digitsChecked == 4)
                {
                    // Number is valid.
                    bIsValid = true;
                }

            return bIsValid;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                txtFileName.Text = openFileDialog1.FileName;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string recordIn = string.Empty;
            string recordOut = string.Empty;

            StreamReader sr = new StreamReader(txtFileName.Text);
            StreamWriter sw = new StreamWriter("LoyaltyNumberValidations.csv");

            recordIn =  sr.ReadLine();

            if (recordIn == null)
            {
                MessageBox.Show("file is empty");
                return;
            }

            recordOut = string.Format("Valid(Y/N),LoyaltyNumber,CheckDigit1(L/C),CheckDigit2(L/C),CheckDigit3(L/C),CheckDigit4(L/C),");
            sw.WriteLine(recordOut);

            while(true)
            {
                long loyaltynumber = 0;

                if(recordIn == null)
                {
                    sr.Close();
                    break;
                }
                loyaltynumber = Convert.ToInt64(recordIn);

                if(IsLoyaltyNumberValid(loyaltynumber))
                {
                    recordOut = string.Format("Y, {0}, {1}/{2}, {3}/{4}, {5}/{6}, {7}/{8}", loyaltynumber, digit4, checkDigit4, digit8, checkDigit8, digit12, checkDigit12, digit14, checkDigit14);
                }
                else
                {
                    recordOut = string.Format("N, {0}, {1}/{2}, {3}/{4}, {5}/{6}, {7}/{8}", loyaltynumber, digit4, checkDigit4, digit8, checkDigit8, digit12, checkDigit12, digit14, checkDigit14);
                }

                sw.WriteLine(recordOut);
                recordIn = sr.ReadLine();
            }
            lblStatus.Text = "File Processed";
        }

    }
}
