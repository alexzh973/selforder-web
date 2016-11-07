using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using ensoCom;
using wstcp.DEVINO;
using wstcp.models;


public class sms
    {

        private static string prepare_phone(string src_number)
        {
            string tmp = src_number.Trim().ToLower().Replace("с", "").Replace("c", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").Replace(@"\", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            if (tmp.StartsWith("8"))
            {
                tmp = "7" + tmp.Substring(1, tmp.Length - 1);
            }
            return (tmp.StartsWith("7")) ? "+"+tmp : "+7" + tmp;
        }

         private static void proxy_activate()
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
            }


        }

         public static decimal get_balance()
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
        public static string SendSMS(string recioient_phone_number, string text_sms){
        
            proxy_activate();
            
            decimal balance = get_balance();
            string rep = "";
            
            DEVINOSoapClient sms = new DEVINOSoapClient();
            string uid = webCfg.GetValue("DevinoUID");
            string psw = webCfg.GetValue("DevinoPsw");
            string sign = webCfg.GetValue("DevinoSign");
            if (balance >= 1)
            {
                try
                {
                    string txt = text_sms;
                    ArrayOfString ar = new ArrayOfString();
                    CommandStatus res = sms.SendTextMessage(uid, psw, prepare_phone(recioient_phone_number), txt, sign, true, false, 10, out ar);
                    

                    rep = (res.ToString()=="OK_Operation_Completed")?"":res.ToString();
                }
                catch (Exception ex2)
                {
                    rep = "СООБЩЕНИЕ НЕ ОТПРАВЛЕНО. " + ex2.ToString() + "";
                }
            }
            else
            {
                rep = "СООБЩЕНИЕ НЕ ОТПРАВЛЕНО. Баланс провайдера: " + balance + "";
            }
            return rep;

        }
    }
