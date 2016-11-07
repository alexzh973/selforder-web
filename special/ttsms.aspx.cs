using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using wstcp.DEVINO;
using wstcp.models;

namespace wstcp.special
{
    public partial class ttsms : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text +=  ""+get_balance();


           Label1.Text +=  sms.SendSMS(txtel.Text, msg.Text);
        }

        public decimal get_balance()
        {
            proxy_activate();
            decimal balance = 0;
            DEVINOSoapClient sms = new DEVINOSoapClient();
            string uid = webCfg.GetValue("DevinoUID");
            string psw = webCfg.GetValue("DevinoPsw");
            string sign = webCfg.GetValue("DevinoSign");
            try
            {
                sms.GetCreditBalance(uid, psw, out balance);
            }
            catch (Exception ex)
            {
                balance = -1;
            }
            return balance;
        }

        private void proxy_activate()
        {
            if (webCfg.GetValue("UseProxy") == "1")
            {
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                string proxy_uid = webCfg.GetValue("proxy_uid");
                string proxy_psw = webCfg.GetValue("proxy_psw");
                string proxy_domen = webCfg.GetValue("proxy_domen");
                string proxy_adr = webCfg.GetValue("proxy_adr");
                WebProxy proxy = new WebProxy(proxy_adr);
                proxy.Credentials = new NetworkCredential(proxy_uid, proxy_psw, proxy_domen);
                WebRequest.DefaultWebProxy = proxy;
                Label1.Text += "прокси пошло ";
            }
            else
                Label1.Text += "без прокси ";

        }
    }
}