using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using ensoCom;
using System.Collections;
using System.Data;



namespace wstcp.Models
{
    public class OrderItem
    {
        eLog log = new eLog();
        public const string TDB = "ORDI";
        private NSIGood _good;     

        public int GoodId=0;
        public NSIGood Good{
            get { if (_good == null) _good = new NSIGood(GoodId); return _good; }
        }
        public string GoodCode = "";
        public string Name = "";
        public string Descr="";
        public string Comment="";
        public decimal Price=0;
        public decimal Qty=1;
        public decimal Summ=0;
        public string state="N";
        public decimal CurIncash = 0;
        public string Zn = "";
        public string Zn_z = "";
        public string ed = "";
        public string article = "";
    }

    public class Order:eObject
    {
        public const string TDB = "ORD";
        private static Hashtable RLUPD = new Hashtable();
        public bool WasChanged
        {
            get {
                return (ID > 0 && RLUPD.ContainsKey(ID) && (DateTime)RLUPD[ID] != Lastupdate);
            }
        }

        //private int _id;
        private int _subjectID;
        private Subject _subject;

        public List<OrderItem> Items = new List<OrderItem>();
        public string State = "";
        public string Descr = "";
        //public string Name = "";
        public string Code = "";
        
        public DateTime RegDate;
        public DateTime Lastupdate;
        public string Lastcorrector = "";
        public readonly IAM CurrentUser;

        public Order(int id, IAM user):
            base(TDB,id)
        {
            _id = (id > 0) ? id : 0;
            CurrentUser = user;

            if (!__load(this))
            {
                _id = 0;
                RegDate = DateTime.Today;
                _subjectID = user.SubjectID;
            }
        }

        public int ID
        {
            get { return _id; }
        }

        public int SubjectID
        {
            get { return _subjectID; }
            set { if (_subjectID != value) { _subjectID = value; _subject = null; } }
        }
        public Subject Subject
        {
            get { if (_subject == null) _subject = new Subject(_subjectID, CurrentUser); return _subject; }
        }
        public decimal Summ
        {
            get
            {
                decimal sm = 0;
                foreach (OrderItem item in Items)
                    sm += item.Summ;
                return sm;
            }
        }
        public string StateDescr
        {
            get { return get_stateorder_descr(State); }
        }

        public DataTable ItemsDt
        {
            get
            {
                DataTable curdt = new DataTable();
                curdt.Columns.Add("goodid");
                curdt.Columns.Add("goodcode");
                curdt.Columns.Add("name");
                curdt.Columns.Add("pr");
                curdt.Columns.Add("qty");
                curdt.Columns.Add("sum");
                curdt.Columns.Add("state");
                curdt.Columns.Add("descr");
                curdt.Columns.Add("comment");
                curdt.Columns.Add("curincash");
                curdt.Columns.Add("zn");
                curdt.Columns.Add("zn_z");
                curdt.Columns.Add("ed");
                curdt.Columns.Add("article");
                foreach (OrderItem item in Items)
                {
                    DataRow nr = curdt.NewRow();
                    nr["goodcode"] = item.GoodCode;
                    nr["goodid"] = item.GoodId;
                    nr["name"] = item.Name;
                    nr["pr"] = item.Price;
                    nr["qty"] = item.Qty;
                    nr["sum"] = item.Summ;
                    nr["state"] = item.state;
                    nr["descr"] = item.Descr;
                    nr["comment"] = item.Comment;
                    nr["curincash"] = item.CurIncash;
                    nr["zn"] = item.Zn;
                    nr["zn_z"] = item.Zn_z;
                    nr["ed"] = item.ed;
                    nr["article"] = item.article;
                    curdt.Rows.Add(nr);
                }
                return curdt;
            }
        }

        private static bool __load(Order rec)
        {
            bool result = false;
            string str = "select * from " + Order.TDB + " as g where ID=" + rec.ID;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    rec.Name = "" + r["Name"];
                    rec.Code = "" + r["Code"];
                    rec.Descr = "" + r["Descr"];
                    rec.RegDate = cDate.cToDate(r["RegDate"]);
                    rec.State = "" + r["State"];
                    rec.SubjectID = cNum.cToInt(r["SubjectID"]);
                    rec.Lastcorrector = "" + r["lc"];
                    rec.Lastupdate = cDate.cToDate(r["lcd"]);
                    if (rec.ID > 0 && !RLUPD.ContainsKey(rec.ID))
                        RLUPD[rec.ID] = rec.Lastupdate;
                    result = true;
                }
                
            }
            catch { }
            finally { cn.Close(); }
            foreach (DataRow r in db.GetDbTable("select itms.*,isnull((select sum(Qty) from GINCASH where GINCASH.GoodId=itms.GoodId),0) as curincash,owng.zn, owng.zn_z, good.ed, good.article from " + OrderItem.TDB + " as itms inner join OWNG on owng.goodid=itms.goodid and owng.ownerid=100000 inner join GOOD on good.id=itms.goodid where itms.OrderId=" + rec.ID).Rows)
            {
                rec.Items.Add(new OrderItem() { GoodId = cNum.cToInt(r["GoodId"]), Zn=""+r["zn"], Zn_z=""+r["zn_z"], CurIncash=cNum.cToDecimal(r["curincash"]), GoodCode = "" + r["GoodCode"], Name = "" + r["Name"], Price = cNum.cToDecimal(r["Pr"]), Qty = cNum.cToDecimal(r["Qty"]), Summ = cNum.cToDecimal(r["Summ"]), state = "" + r["State"], Descr = "" + r["Descr"], Comment = "" + r["Comment"], article=""+r["article"], ed=""+r["ed"] });
            }

            return result;
        }
        
        public static bool Save(Order rec, IAM user)
        {
            bool result = false;
            eLog log = new eLog();
            if (rec.ID > 0)
            {                
                Order old = new Order(rec.ID, user);
                if (old.RegDate != rec.RegDate)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "RegDate", old.RegDate.ToShortDateString(), rec.RegDate.ToShortDateString(), "U"));
                if (old.Descr != rec.Descr)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "Descr", old.Descr, rec.Descr, "U"));
                if (old.Summ != rec.Summ)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "Summ", ""+old.Summ, ""+rec.Summ, "U"));

            }
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            int tid = rec.ID;
            try
            {
                cmd.Parameters.AddWithValue("@ID", rec.ID);
                cmd.Parameters.AddWithValue("@Name", rec.Name);
                cmd.Parameters.AddWithValue("@Descr", rec.Descr);
                cmd.Parameters.AddWithValue("@SubjectID", rec.SubjectID);
                cmd.Parameters.AddWithValue("@RegDate", rec.RegDate);
                cmd.Parameters.AddWithValue("@SummOrder", rec.Summ);
                cmd.Parameters.AddWithValue("@lc", user.Name);
                cmd.Parameters.AddWithValue("@State", rec.State);

                cmd.CommandText = "update "+Order.TDB+" set Name=@Name, Descr=@Descr, RegDate=@RegDate, SummOrder=@SummOrder, State=@State, lc=@lc, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + Order.TDB + " (Name, Descr, SubjectID, RegDate,SummOrder, State,lc, lcd)" +
                        " values (@Name, @Descr, @SubjectID, @RegDate, @SummOrder, @State, @lc, getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + Order.TDB + " where lc=@lc";
                    tid = cNum.cToInt(cmd.ExecuteScalar());

                }
                result = (tid > 0); if (result)
                {
                    rec._id = tid;
                    

                    
                    cmd.CommandText = "delete from " + OrderItem.TDB + " where OrderId=" + rec.ID;
                    cmd.ExecuteNonQuery();

                    foreach (OrderItem item in rec.Items)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@OrderId", rec.ID);
                        cmd.Parameters.AddWithValue("@GoodId", item.GoodId);
                        cmd.Parameters.AddWithValue("@GoodCode", item.GoodCode);
                        cmd.Parameters.AddWithValue("@Name", item.Name);
                        cmd.Parameters.AddWithValue("@Pr", item.Price);
                        cmd.Parameters.AddWithValue("@Qty", item.Qty);
                        cmd.Parameters.AddWithValue("@Summ", item.Summ);
                        cmd.Parameters.AddWithValue("@Descr", item.Descr);
                        cmd.Parameters.AddWithValue("@Comment", item.Comment);
                        cmd.Parameters.AddWithValue("@lc", user.Name);
                        cmd.Parameters.AddWithValue("@State", item.state);

                        cmd.CommandText = "insert into " + OrderItem.TDB + " (OrderId,GoodId,GoodCode, Name, Descr, Comment, Qty, Pr, Summ, lc, lcd, State) values (@OrderId,@GoodId,@GoodCode,@Name, @Descr, @Comment,@Qty, @Pr, @Summ, @lc, getdate(), @State)";
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            if (result && log != null && log.Stack.Count > 0)
            {
               
                    log.Save(user.ID);
            }
            return result;
        }
        public static bool Delete(Order rec, IAM user)
        {
            bool res = false;
            if (rec.ID <= 0)
                return false;
            if (rec.State=="D" || rec.State=="C")
                return false;

            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(rec.ThisObject, "State", rec.State, "D", "D"));
            res = (db.ExecuteCmd("update " + TDB + " set State='D' where id=" + rec.ID + " and SubjectID=" + user.SubjectID)>0);
            if (res)
                log.Save(user.ID);
            return res;
        }
        
        
        public static string get_stateorder_descr(string state)
        {
            string r = "";
            switch (state.ToUpper())
            {
                case "U":
                    r = "Обработана в УЦСК автоматически";
                    break;
                case "S":
                    r = "Подтверждена";
                    break;
                case "A":
                    r = "Принята в работу";
                    break;
                case "C":
                    r = "Закрыта";
                    break;
                case "D":
                    r = "Удалена";
                    break;
                default:
                    r = "Не рассмотрена";
                    break;
            }
            return r;
        }
    }
}