using System;
using System.Data.SqlClient;
using System.Web.Services;
using System.Collections;
using System.Data;
using selforderlib;
using ensoCom;
using System.Collections.Generic;
using wstcp.models;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для googies
    /// </summary>
    [WebService(Namespace = "http://santechportal.ru/")]
     
    
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
            else //if (NSImeta.CheckDb)
            {
               Owner o = new Owner(owner_id);
               res = (o.ID>0);
            }
            if (res)
            {
                SESSION_ID = session_id;
                GOODSOWNERID = owner_id;
            }
            return res;
        }

        [WebMethod]
        public int GetSubjectIdByINN(string inn)
        {
            int id = Subject.FindByField("INN", inn);
            return id;
        }
        [WebMethod]
        public bool CheckPersonByEmail(string email)
        {
            bool ex = pUserInfo.Exist(email);
            return ex;
        }

        [WebMethod]
        public int CreateNewSubject( string inn, string code, string name, string codedg, string emailtas, string type_price, string usr, string psw)
        {
            EmailMessage msg = new EmailMessage(
                    CurrentCfg.EmailPortal,
                    "alexzh@santur.ru",
                    HelpersPath.RootUrl + " :: goodsx_asmx: CreateNewSubject",
                    string.Format("<p>inn {0}</p><p>code {1}</p><p>name {2}</p><p>codedg {3}</p><p>emailtas {4}</p><p>type_price {5}</p>", inn, code, name, codedg, emailtas, type_price));
            msg.Send();

            if (("" + psw).Length < 10) return 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            if (("" + inn).Length < 10 || ("" + code).Length < 9 || ("" + name).Length < 3 ) return 0;
            int id = Subject.FindByField("INN", inn);
            if (id == 0) id = Subject.FindByField("Code", code);
            if (id > 0) return id;
            Subject s = new Subject(0, iam);
            s.INN = inn;
            s.Code = code;
            s.CodeDG = codedg;
            s.Name = name;
            s.EmailTAs = emailtas;
            s.OwnerID = iam.OwnerID;
            s.PriceType = type_price;
            return (Subject.Save(s)) ? Subject.FindByField("INN", inn) : 0;
        }

        [WebMethod]
        public int CreateNewPerson(string email, string name, int subjectId, string usr, string psw)
        {
            EmailMessage msg = new EmailMessage(
                    CurrentCfg.EmailPortal,
                    "alexzh@santur.ru",
                    HelpersPath.RootUrl + " :: goodsx_asmx: CreateNewPerson",
                    string.Format("<p>email {0}</p><p>name {1}</p><p>subjetId {2}</p>", email, name, subjectId));
            msg.Send();

            if (("" + psw).Length < 10) return 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            if (("" + email).Length < 10 || subjectId < 100000 || ("" + name).Length < 3) return 0;

            int id = pUser.FindByField("email", email);
            if (id > 0) return id;

            if (Subject.FindByField("ID", ""+subjectId) <= 0) return 0;

            pUser u = new pUser(0, iam);
            u.Name = name;
            u.Email = email;
            u.SubjectID = subjectId;
            u.LoginEnabled = true;
            return (pUser.Save(u)) ? pUser.FindByField("email", email) : 0;
        }

        [WebMethod]
        public int CreateNewOrder(int subjectId, string code, string codedg, OrderItem[] items, string usr, string psw)
        {
            if (("" + psw).Length < 10) return 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            Order ord = new Order(0, iam);
            ord.SubjectID = subjectId;
            ord.Dg = codedg;
            ord.Code = code;
            ord.State = "u";
            GoodOwnInfo gi;
            foreach (OrderItem item in items)
            {
                gi = GoodInfo.GetInfo(item.GoodCode, iam.OwnerID, "pr_b");
                item.GoodId = gi.GoodId;
                item.state = "u";
                item.SummBase = (gi.PriceBase < item.Price) ? item.Price*item.Qty : gi.PriceBase*item.Qty;
                item.Summ = item.Price*item.Qty;
                ord.AddItem(item);
            }
            bool res = Order.Save(ord);
            int id = ord.ID;
            //Order.FinishWork(iam);



            return id;
        }


        [WebMethod]
        public void CancelOrder(int orderId, string usr, string psw)
        {
            if (("" + psw).Length < 10) return ;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return ;

            Order ord = new Order(orderId, iam);
            ord.State = "D";
            Order.Save(ord);
        }

        [WebMethod]
        public int UpdateOrder(int subjectId, int orderId, string codedg, OrderItem[] items, string usr, string psw)
        {
            if (("" + psw).Length < 10) return 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            Order ord = new Order(orderId, iam);
            ord.Dg = codedg;
            ord.State = "u";
            GoodOwnInfo gi;
            if (ord.ID>0)
                ord.Items.Clear();
            foreach (OrderItem item in items)
            {
                OrderItem existitem = ord.Items.Find(x => x.GoodCode == item.GoodCode);
                gi = GoodInfo.GetInfo(item.GoodCode, iam.OwnerID, "pr_b");
                item.GoodId = gi.GoodId;
                item.state = "u";
                item.SummBase = (gi.PriceBase < item.Price) ? item.Price * item.Qty : gi.PriceBase * item.Qty;
                if (existitem != null)
                {
                    existitem.Qty = item.Qty;
                    existitem.Price = item.Price;
                    existitem.SummBase = (gi.PriceBase < item.Price) ? item.Price * item.Qty : gi.PriceBase * item.Qty;
                    existitem.Summ = item.Price*item.Qty;
                } else
                {
                    ord.AddItem(item);
                }
                
                
            }
            bool res = Order.Save(ord);
            int id = ord.ID;
            return id;
        }


        [WebMethod]
        public int CreateNewAcc(string code, string startdate, string finishdate, string descr , int ownerId, SaleAccItem[] items, string usr, string psw)
        {
            if (("" + psw).Length < 10) return -2;
            IAM iam = IamServices.Login(usr, psw, usr, usr);if (iam.ID <= 0) return -3;
            int id=0;
            SaleAcc doc = new SaleAcc() { ID=0, Code = code, Name = code, Descr = descr, StartDate = cDate.cToDate(startdate), FinishDate = cDate.cToDate(finishdate),OwnerID = ownerId, PriceType = "", RulePrice = ""};
            
            try
            {
                int goodId;
                foreach (SaleAccItem item in items)
                {
                    goodId = Good.GetID(iam.OwnerID, item.GoodCode);
                    item.GoodId = goodId;
                    doc.Items.Add(item);
                }
                id = SaleAcc.Save(doc, iam);
                bool res = (id > 0);
            }
            catch (Exception ex)
            {
            }
            


            return id;
        }



        [WebMethod]
        public int UpdateSubjectCount(string codedg, decimal summVz, string type_price, string usr, string psw)
        {
            EmailMessage msg = new EmailMessage(
                    CurrentCfg.EmailPortal,
                    "alexzh@santur.ru",
                    HelpersPath.RootUrl + " :: goodsx_asmx: UpdateSubjectCount",
                    string.Format("<p>codedg {0}</p><p>summVz {1}</p><p>typePrice {2}</p>", codedg, summVz, type_price));
            msg.Send();

            if (("" + psw).Length < 10) return 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            if ( ("" + codedg).Length < 6 ) return 0;


            return db.ExecuteCmd("update DG set currb="+cNum.cToDecimal(summVz)+", pr='"+type_price+"', lcd=(getdate())  where codedg='"+codedg+"' and ownerid=" + iam.OwnerID);
        }

        [WebMethod]
        public int UpdateTeoInfo(int orderId, string teodate, string stady, string car_and_driver, string usr, string psw)
        {
            if (orderId<=0 || ("" + psw).Length < 10) return 0;
            
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;

            db.ExecuteCmd("update ORD set TEOState='" + stady + "', TEODate=" + cDate.Date2Sql(teodate) + ", teotrans='" + car_and_driver + "' where id=" + orderId + " ");
            return 1;
        }

        [WebMethod]
        public int CloseTeoInfo(int orderId, string usr, string psw)
        {
            if (orderId <= 0 || ("" + psw).Length < 10) return 0;

            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;
            db.ExecuteCmd("update ORD set TEOState='F' where id=" + orderId + " ");
            return 1;
        }

        [WebMethod]
        public int UpdateGood(string goodCode, decimal salekrat, string zn, string zn_z, string zt, decimal qty, decimal pr_spr, decimal pr_ngc, decimal pr_vip, decimal pr_spec, decimal pr_kropt, decimal pr_opt, decimal pr_b, string usr, string psw)
        {
            int res = 0;
            IAM iam = IamServices.Login(usr, psw, usr, usr);
            if (iam.ID <= 0) return 0;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
                cn.Open();
                cmd.Parameters.AddWithValue("@OwnerId", iam.OwnerID);

                cmd.Parameters.AddWithValue("@GoodCode", goodCode);
                cmd.Parameters.AddWithValue("@zn", zn);
                cmd.Parameters.AddWithValue("@zn_z", zn_z);
                cmd.Parameters.AddWithValue("@zt", zt);
                cmd.Parameters.AddWithValue("@pr_spr", pr_spr);
                cmd.Parameters.AddWithValue("@pr_b", pr_b);
                cmd.Parameters.AddWithValue("@qty", qty);

                cmd.Parameters.AddWithValue("@pr_vip", pr_vip);
                cmd.Parameters.AddWithValue("@pr_spec", pr_spec);
                cmd.Parameters.AddWithValue("@pr_kropt", pr_kropt);
                cmd.Parameters.AddWithValue("@pr_opt", pr_opt);
                cmd.Parameters.AddWithValue("@pr_ngc", pr_ngc);
                cmd.Parameters.AddWithValue("@salekrat", salekrat);
                cmd.CommandText = "update OWNG set state='',lcd=getdate(),lc='" + iam.Name +
                                  "', owng.pr_spr=@pr_spr, owng.pr_b=@pr_b, owng.qty=@qty, owng.zn=@zn, owng.zn_z=@zn_z, owng.zt=@zt,owng.pr_vip=@pr_vip, owng.pr_spec=@pr_spec, owng.pr_kropt=@pr_kropt, owng.pr_opt=@pr_opt,owng.pr_ngc=@pr_ngc,owng.salekrat=@salekrat where owng.ownerId=@OwnerId and owng.GoodCode=@GoodCode";
                if (cmd.ExecuteNonQuery() <= 0)
                {
                    cmd.CommandText = "insert into OWNG (state, lcd, lc, pr_spr, pr_b, qty, zn, zn_z, zt, pr_vip, pr_spec, pr_kropt, pr_opt, pr_ngc, salekrat, ownerId, GoodCode) values" +
                                      " ('',getdate(),'" + iam.Name + "', @pr_spr, @pr_b, @qty, @zn, @zn_z, @zt, @pr_vip, @pr_spec, @pr_kropt, @pr_opt, @pr_ngc, @salekrat, @OwnerId, @GoodCode)";
                    cmd.ExecuteNonQuery();
                    res = 1;
                } else
                {
                    res = 2;
                }
                
            }
            catch
            {
                res = -1;
            }
            finally
            {
                cn.Close();
            }
            return res;
        }

        [WebMethod]
        public int GetGoodID(int owner_id, string owner_good_code)
        {
            return Good.GetID(owner_id, owner_good_code);
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
