using System.Web.Services;
using System.Collections;
using System.Data;
using wstcp.Models;
using ensoCom;
using System.Collections.Generic;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для googies
    /// </summary>
    [WebService(Namespace = "http://santechportal.ru/")]
     
    /*[WebService(Namespace = "http://portal:90")]*/
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class goodsx : System.Web.Services.WebService
    {
        //protected static Hashtable __owncn = new Hashtable();

        //bool MAYUSE = false;
        bool CANUPDATE = false;
        string KEY = "";
        string SESSION_ID = "";
        int GOODSOWNERID = 0;
        


        [WebMethod]
        public bool InitConnection(string session_id, int owner_id)
        {
            bool res = false;
            if (session_id.Length<10 || owner_id<1)
            {
                res = false;
            }
            else if (NSImeta.CheckDb)
            {
               NSIOwner o = new NSIOwner(owner_id);
               //__owncn[session_id]=o.ID;
               res = (o.ID>0);
               
            }
            else
            {
                res = false;
            }
            //MAYUSE = res;
            if (res)
            {
                SESSION_ID = session_id;
                GOODSOWNERID = owner_id;
            }
            return res;
        }
        

        [WebMethod]
        public int GetGoodID(int owner_id, string owner_good_code)
        {
            return NSIGood.GetID(owner_id, owner_good_code);
        }

        

        public struct GI
        {
            public int OwnerId;
            public string Owner;
            public string GoodCode;
            public int qty;
            public string zn;
            public string zn_z;
            public string zt;
        }
        
        [WebMethod (Description="на вход список кодов номенклатуры рзделенный ; на выходе список структур GI")]
        public List<GI> wGetIncash(string listgoodcodes)
        {

            string lg = "'" + listgoodcodes.Replace((char)160, ' ').Replace(" ", "").Replace(";", "','") + "'";
            DataTable dt = db.GetDbTable("SELECT owng.OwnerId, OWN.Name as OwnerName, OWNG.GoodCode, ISNULL(OWNG.zn, 'S') AS zn, ISNULL(OWNG.zn_z, 'D0') AS zn_z, ISNULL(OWNG.pr_spr, 0) AS SuppriceWnds, ISNULL(OWNG.pr_b, 0) AS BasePrice, ISNULL(owng.qty,0) AS Qty FROM OWNG" +
                " INNER JOIN OWN ON OWNG.OwnerId = OWN.ID" +
                          " WHERE OWNG.GoodCode in (" + lg + ")");
            dt.TableName = "INCASH";
            List<GI> list = new List<GI>();
            foreach (DataRow r in dt.Select("", "GoodCode,OwnerName"))
            {
                list.Add(new GI() { OwnerId = cNum.cToInt(r["OwnerId"]), Owner = "" + r["OwnerName"], GoodCode = "" + r["GoodCode"], zn = "" + r["zn"], zn_z = "" + r["zn_z"], qty = cNum.cToInt(r["qty"]) });
            }
            return list;
        }

        

        [WebMethod]      
        public string IncashStringCSV(string good_code, int owner_id)
        {
            string csv = "";
            DataRow[] rr = GoodInfo.GetIncashTable(good_code).Select("ownerid="+owner_id);
            for (int i = 0; i < rr.Length; i++)
                cStr.Add(ref csv, ""+rr[i], ';');
            return csv;
        }
  
       


        
    }
    struct GoodUpdStruct
        {
            public int warehouse_id;
            public string good_code;
            public decimal qty;
           

            public GoodUpdStruct(int warehouse_id, string good_code, decimal qty)
            {                
                this.warehouse_id = warehouse_id;
                this.good_code = good_code;
                this.qty = qty;
            }
        }

    public struct GoodStruct
    {
        public string
            Article,
            Name,
            Descr,
            OwnerGoodCode;
        public GoodStruct(string article, string name, string descr, string owner_goodcode)
        {
            this.Article = article;
            this.Name = name;
            this.Descr = descr;
            this.OwnerGoodCode = owner_goodcode;
        }
    }
}
