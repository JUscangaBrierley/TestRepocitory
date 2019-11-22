using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XigniteQATestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtToken.Text.Length == 0)
            {
                MessageBox.Show("Please provide a token");
                return;
            }
            if (txtCurrencyCode.Text.Length == 0)
            {
                MessageBox.Show("Provide a currency code");
                return;
            }
            lblCurrencyRate.Text = GetCurrencyRate(txtCurrencyCode.Text);
        }

        public string GetCurrencyRate(string currencyCode)
        {

            string returnValue = string.Empty;

            RemoteGlobalCurrencies.GetOfficialRateRequest rateRequest = new RemoteGlobalCurrencies.GetOfficialRateRequest();

            
            RemoteGlobalCurrencies.XigniteGlobalCurrenciesSoapClient client = new RemoteGlobalCurrencies.XigniteGlobalCurrenciesSoapClient("XigniteGlobalCurrenciesSoap");

            // add authentication info
            RemoteGlobalCurrencies.Header objHeader = new RemoteGlobalCurrencies.Header();
            RemoteGlobalCurrencies.GetRealTimeRateRequest request = new RemoteGlobalCurrencies.GetRealTimeRateRequest();
            objHeader.Username = txtToken.Text;

            //request.Header = objHeader;
            //request.Symbol = "CANUSD";
            RemoteGlobalCurrencies.Rate rate = client.GetRealTimeRate(objHeader, currencyCode);

            if (rate == null)
            {
                // add error processing here 
                // this condition could be caused by an HTTP error (404,500...)
                txtMessage.Text = "Service is unavailable at this time.";
            }
            else
            {
                //RemoteGlobalCurrencies.Rate rate = response.GetRealTimeRateResult;
                switch (rate.Outcome)
                {
                    case RemoteGlobalCurrencies.OutcomeTypes.Success:
                        // add processing for displaying the results, e.g. 
                        // display the value for objRate.Source
                        // other values could be consumed in the same manner
                        Console.Write(rate.Source);
                        double currentRate = rate.Mid;
                        returnValue = currentRate.ToString();
                        txtMessage.Text = "Success";
                        break;
                    default:
                        // add processing for handling request problems, e.g. 
                        // you could pass back the info message received from the service 
                        txtMessage.Text = rate.Message;
                        break;
                }
            }
            return returnValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

    }
}
