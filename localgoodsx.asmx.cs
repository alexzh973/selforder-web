using System;
using System.Web.Services;
using System.Collections;
using System.Data;
using selforderlib;
using ensoCom;
using System.Collections.Generic;
using wstcp.models;

namespace wstcp
{
     
    [WebService(Namespace = "http://localhost:53280")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    
    public class localgoodsx : System.Web.Services.WebService
    {
        
        bool CANUPDATE = false;
        string KEY = "";
       
        


        [WebMethod]
        public bool InitConnection(string session_id, int owner_id)
        {
            bool res = false;
            if (session_id.Length<10 || owner_id<1)
            {
                res = false;
            }
            else //if (NSImeta.CheckDb)
            {
               Owner o = new Owner(owner_id);
               res = (o.ID>0);
            }
            //if (res)
            //{
            //    SESSION_ID = session_id;
            //    GOODSOWNERID = owner_id;
            //}
            return res;
        }

        [WebMethod]
        public int CreateNewAcc(SaleAcc doc, SaleAccItem[] items, string usr, string psw)
        {
            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(new sObject(100000, "ACC"), "webService", "", "connect", "", ""));
            if (("" + psw).Length < 10) return -2;
            IAM iam = IamServices.Login(usr, psw, usr, usr);if (iam.ID <= 0) return -3;
            int id=0;
            try
            {
                
                

                int goodId;
                foreach (SaleAccItem item in items)
                {
                    goodId = Good.GetID(iam.OwnerID, item.GoodCode);
                    item.GoodId = goodId;
                    doc.Items.Add(item);
                }
                log.Stack.Add(new eLog.LogRecord(new sObject(iam.OwnerID, "ACC"), "webService", "", "before save", ""+doc.Items.Count, ""));
                id = SaleAcc.Save(doc, iam);
                log.Stack.Add(new eLog.LogRecord(new sObject(iam.OwnerID, "ACC"), "webService", "", "after save", ""+id, ""));
                bool res = (id > 0);
            }
            catch (Exception ex)
            {

                log.Stack.Add(new eLog.LogRecord(new sObject(iam.OwnerID, "ACC"), "webService", "", ex.ToString(), "error", ""));

            }
            


            eLog.Save(log,iam.ID);
            return id;
        }

    }
    
}
