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
        public string Mark = "";
        public string GoodCode = "";
        public string Name = "";
        public string Descr="";
        public string Comment="";
        public decimal Price=0;
        public decimal Qty=1;
        public decimal Realized = 0;
        public decimal Booking = 0;
        public DateTime WDate = cDate.DateNull;
        public decimal Summ = 0;
        public decimal SummBase = 0;
        public string state="N";
        public decimal CurIncash = 0;
        public string Zn = "";
        public string Zn_z = "";
        public string ed = "";
        public string article = "";
        public string img = "";
    }

    public class Order:eObject
    {
        private static Hashtable ORDSINWORK = new Hashtable();
        public const string TDB = "ORD";
        private static Hashtable RLUPD = new Hashtable();
        public bool WasChanged
        {
            get {
                return (ID > 0 && RLUPD.ContainsKey(ID) && (DateTime)RLUPD[ID] != Lastupdate);
            }
        }

        
        private int _subjectID;
        private Subject _subject;
        private int _authorID;
        private pUser _author;
        private bool _changed = false;
        private List<OrderItem> _items = new List<OrderItem>();
        public List<OrderItem> Items = new List<OrderItem>();
        public new string State
        {
            get { return _state; }
            set { if (value.ToUpper()!="D") _state = value; }
        }
        public string Descr = "";
        
        public string Code = "";
        
        public DateTime RegDate;
        public DateTime Lastupdate;
        public string Lastcorrector = "";
        public readonly IAM CurrentUser;
        
        public static void BeginWork(IAM user, Order order)
        {
            ORDSINWORK[user.ID] = order;
        }
        public static void FinishWork(IAM user)
        {
            ORDSINWORK.Remove(user.ID);
        }
        public static Order GetCurrentOrder(IAM user)
        {
            if (ORDSINWORK[user.ID] != null)
                return (Order)ORDSINWORK[user.ID];
            else
                return new Order(0, user, false);
        }

        public Order(int id, IAM user, bool editmode):
            base(TDB,id)
        {
            _id = (id > 0) ? id : 0;
            CurrentUser = user;

            if (!__load(this))
            {
                _id = 0;
                RegDate = DateTime.Today;
                _subjectID = user.SubjectID;
                _authorID = user.ID;
            }
            if (editmode)
                BeginWork(user, this);
        }
        public void MakeCopy(){
            int i = Items.Count();
            _id = 0;
            Code = "";
            _state = "";
            RegDate = DateTime.Today;
            Lastupdate = DateTime.Today;
            Lastcorrector = "";
            AuthorID = CurrentUser.ID;
            ORDSINWORK[CurrentUser.ID] = this;
        }
        
        public int AuthorID
        {
            get { return _authorID; }
            set { if (_authorID != value) { _authorID = value; _author = null; } }
        }
        public pUser Author
        {
            get { if (_author == null) _author = new pUser(_authorID, CurrentUser); return _author; }
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
        public decimal SummBase
        {
            get
            {
                decimal sm = 0;
                foreach (OrderItem item in Items)
                    sm += item.SummBase;
                return sm;
            }
        }
        public string StateDescr
        {
            get { return get_stateorder_descr(State); }
        }

        public decimal Discount
        {
            get
            {                
                decimal s0 = 0;
                decimal s1 = 0;
                foreach (OrderItem item in Items)
                {
                    if (item.SummBase != 0 && item.Summ != 0)
                    {s0 += item.SummBase; s1 += item.Summ; }
                }
                return (s0!=0)?(s0-s1)/s0:0;
            }
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
                curdt.Columns.Add("sumbase");
                curdt.Columns.Add("state");
                curdt.Columns.Add("descr");
                curdt.Columns.Add("comment");
                curdt.Columns.Add("curincash");
                curdt.Columns.Add("booking");
                curdt.Columns.Add("realized");
                curdt.Columns.Add("wdate");
                curdt.Columns.Add("zn");
                curdt.Columns.Add("zn_z");
                curdt.Columns.Add("ed");
                curdt.Columns.Add("article");
                curdt.Columns.Add("mark");
                curdt.Columns.Add("img");
                foreach (OrderItem item in Items)
                {
                    DataRow nr = curdt.NewRow();
                    nr["goodcode"] = item.GoodCode;
                    nr["goodid"] = item.GoodId;
                    nr["name"] = item.Name;
                    nr["pr"] = item.Price;
                    nr["qty"] = item.Qty;
                    nr["sum"] = item.Summ;
                    nr["sumbase"] = item.SummBase;
                    nr["state"] = item.state;
                    nr["descr"] = item.Descr;
                    nr["comment"] = item.Comment;
                    nr["curincash"] = item.CurIncash;
                    nr["booking"] = item.Booking;
                    nr["realized"] = item.Realized;
                    nr["WDate"] = item.WDate;
                    nr["zn"] = item.Zn;
                    nr["zn_z"] = item.Zn_z;
                    nr["ed"] = item.ed;
                    nr["article"] = item.article;
                    nr["mark"] = item.Mark;
                    nr["img"] = item.img;
                    curdt.Rows.Add(nr);
                }
                return curdt;
            }
        }
        public OrderItem FindItem(int good_id)
        {
            OrderItem item = Items.Find(i => i.GoodId == good_id);
            return item;
        }

        public void AddItem(OrderItem newitem)
        {           
            Items.Add(newitem);
            _changed = true;
        }

        public void RemoveItem(int goodId)
        {
            Items.Remove(Items.Find(i => i.GoodId == goodId));
            _changed = true;
        }
        public void ChangeItem(OrderItem item, decimal newqty, string descr)
        {
            if (newqty <= 0) newqty = 1;
            if (newqty != item.Qty)
            {
                item.SummBase = (item.SummBase / item.Qty) * newqty;
                item.Qty = newqty;
                item.Summ = item.Price * newqty;
                _changed = true;
            }
            item.Descr = descr;
            
        }
        private static bool __load(Order rec)
        {
            bool result = false;
            string str = "select * from " + Order.TDB + " where ID=" + rec.ID;
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
                    rec._state = "" + r["State"];
                    rec.SubjectID = cNum.cToInt(r["SubjectID"]);
                    rec.Lastcorrector = "" + r["lc"];
                    rec.Lastupdate = cDate.cToDate(r["lcd"]);
                    rec.AuthorID = cNum.cToInt(r["AuthorID"]);
                    if (rec.ID > 0 && !RLUPD.ContainsKey(rec.ID))
                        RLUPD[rec.ID] = rec.Lastupdate;
                    result = true;
                }
                
            }
            catch { }
            finally { cn.Close(); }
            foreach (DataRow r in db.GetDbTable("select itms.*, isnull(itms.Realized,0) as Realized, isnull(itms.Booking,0) as Booking, isnull(owng.qty,0) as curincash,owng.zn, owng.zn_z, good.ed, good.article, good.ens, good.img from " + OrderItem.TDB + " as itms inner join OWNG on owng.goodid=itms.goodid and owng.ownerid=100000 inner join GOOD on good.id=itms.goodid where itms.OrderId=" + rec.ID).Rows)
            {
                rec.Items.Add(new OrderItem() { GoodId = cNum.cToInt(r["GoodId"]), Zn = "" + r["zn"], Zn_z = "" + r["zn_z"], CurIncash = cNum.cToDecimal(r["curincash"]), Booking = cNum.cToDecimal(r["booking"]), Realized = cNum.cToDecimal(r["realized"]), WDate = cDate.cToDate(r["WDate"]), GoodCode = "" + r["GoodCode"], Name = "" + r["Name"], Price = cNum.cToDecimal(r["Pr"]), Qty = cNum.cToDecimal(r["Qty"]), Summ = cNum.cToDecimal(r["Summ"]), SummBase = cNum.cToDecimal(r["SummBase"]), state = "" + r["State"], Descr = "" + r["Descr"], Comment = "" + r["Comment"], article = "" + r["article"], ed = "" + r["ed"], img = ""+r["img"] });
            }

            return result;
        }
        
        public static bool Save(Order rec)
        {
            bool result = false;
            eLog log = new eLog(); 
            IAM user = rec.CurrentUser;
            if (rec.ID > 0)
            {                
                Order old = new Order(rec.ID, user, false);
                if (old.RegDate != rec.RegDate)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "RegDate", old.RegDate.ToShortDateString(), rec.RegDate.ToShortDateString(), "U"));
                if (old.Descr != rec.Descr)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "Descr", old.Descr, rec.Descr, "U"));
                if (old.State != rec.State)
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "State", old.State, rec.State, "U"));
                if (old.Summ != rec.Summ)
                {
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "Summ", "" + old.Summ, "" + rec.Summ, "U"));

                }
                if (rec._changed)
                {
                    rec.State = "N";
                    log.Stack.Add(new eLog.LogRecord(new sObject(old.ID, Order.TDB), "State", old.State, rec.State, "U","корректировка ТЧ"));
                }


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
                cmd.Parameters.AddWithValue("@AuthorID", rec.AuthorID);
                cmd.Parameters.AddWithValue("@RegDate", rec.RegDate);
                cmd.Parameters.AddWithValue("@SummOrder", rec.Summ);
                cmd.Parameters.AddWithValue("@SummBase", rec.SummBase);
                cmd.Parameters.AddWithValue("@lc", user.Name);
                cmd.Parameters.AddWithValue("@State", rec.State);

                cmd.CommandText = "update " + Order.TDB + " set Name=@Name, Descr=@Descr, RegDate=@RegDate, SummOrder=@SummOrder, SummBase=@SummBase, State=@State, lc=@lc, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + Order.TDB + " (Name, Descr, SubjectID,AuthorID, RegDate,SummOrder, SummBase,State,lc, lcd)" +
                        " values (@Name, @Descr, @SubjectID, @AuthorID, @RegDate, @SummOrder, @SummBase,@State, @lc, getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + Order.TDB + " where lc=@lc";
                    tid = cNum.cToInt(cmd.ExecuteScalar());

                }
                result = (tid > 0); 
                if (result)
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

                        cmd.CommandText = "insert into " + OrderItem.TDB + " (OrderId,GoodId,GoodCode, Name, Descr, Comment, Qty,Booking,Realized,WDate, Pr, Summ,SummBase, lc, lcd, State) values (@OrderId,@GoodId,@GoodCode,@Name, @Descr, @Comment,@Qty,0,0, null,@Pr, @Summ,@Summ, @lc, getdate(), @State)";
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
            if (rec.State=="D" || rec.State=="F")
                return false;

            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(rec.ThisObject, "State", rec.State, "D", "D"));
            res = (db.ExecuteCmd("update " + TDB + " set State='D' where id=" + rec.ID + " and SubjectID=" + user.SubjectID)>0);
            if (res)
            {
                db.ExecuteCmd("update " + OrderItem.TDB + " set State='D' where orderid=" + rec.ID);
                log.Save(user.ID);
                rec._state = "D";
                ORDSINWORK.Remove(user.ID);
            }
            return res;
        }
        
        
        public static string get_stateorder_descr(string state)
        {
            string r = "";
            switch (state.ToUpper())
            {
                case "N":
                case "":
                    r = "Новая, еще не рассмотрена";
                    break;
                case "U":
                    r = "Обработана поставщиком автоматически";
                    break;
                 case "A":
                    r = "Предварительно подтверждена клиентом";
                    break;
                case "Z":
                    r = "На согласовании поставщика";
                    break;
               case "S":
                    r = "Согласована поставщиком";
                    break;
                case "M":
                    r = "Подтверждена клиентом к заказу";
                    break;
                case "R":
                    r = "На комплектациии";
                    break;
                case "X":
                    r = "Архив";
                    break;
                case "F":
                    r = "Выполнена";
                    break;
                case "D":
                    r = "Отменена";
                    break;
                
                default:
                    r = state;
                    break;
            }
            return r;
        }
    }
}