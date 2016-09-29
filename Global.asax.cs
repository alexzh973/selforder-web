using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using ensoCom;
using System.Data;
using selforderlib;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Text;

namespace wstcp
{
    public class Global : System.Web.HttpApplication
    {
        public static DateTime LastTik;
        public static bool NEEDSTOPIMPORT = false;
        private static string __url = "";
        public static string RootUrl
        {
            get { return __url; }
        }

        static string path = System.AppDomain.CurrentDomain.BaseDirectory + "exch";
        public static bool is_dev = (HelpersPath.RootUrl.ToLower().IndexOf("localhost") > -1);
        public static Hashtable adrs;
        public static List<string> ips;

        protected static System.Timers.Timer tmr = new System.Timers.Timer();


        protected void Application_Start(object sender, EventArgs e)
        {

            ensoCom.db.InitDbConnection("default", System.Configuration.ConfigurationManager.AppSettings["defaultcn"]);
            ImportAsync.Clear_buzy(100000);
            ImportAsync.Clear_buzy(100001);
            ImportAsync.Clear_buzy(100002);
            ImportAsync.Clear_buzy(100003);
            ImportAsync.Clear_buzy(100004);

            init_timer();
            LastTik = DateTime.Now;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        public static void init_timer()
        {
            tmr = new System.Timers.Timer(60000);
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(timer_event);
            tmr.Enabled = true;
            tmr.Start();
        }



        public static void StopImport()
        {
            NEEDSTOPIMPORT = true;

            tmr.Stop();
            ImportAsync.Clear_buzy(100000); ImportAsync.Clear_buzy(100001); ImportAsync.Clear_buzy(100002); ImportAsync.Clear_buzy(100003); ImportAsync.Clear_buzy(100004);
            db.ExecuteCmd("update OWNG state='' where state<>''");
        }

        public static void StartImport()
        {
            NEEDSTOPIMPORT = false;
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(timer_event);
            tmr.Enabled = true;
            tmr.Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            __url = string.Format("http://{0}", Request.ServerVariables["HTTP_HOST"]);
            ensoCom.db.InitDbConnection("default", System.Configuration.ConfigurationManager.AppSettings["defaultcn"]);

            if (adrs == null)
                reload_adrs();
            if ((tmr == null || !tmr.Enabled) && !ImportAsync.Is_buzy_any())
            {
                init_timer();
            }
        }


        public static void reload_adrs()
        {
            adrs = new Hashtable();
            ips = new List<string>();
            foreach (DataRow r in db.GetDbTable("select id, adrs from own").Rows)
            {
                if ("" + r["Adrs"] != "")
                {
                    foreach (string ip in ("" + r["adrs"]).Split(','))
                    {
                        if (ip != "")
                        {
                            ips.Add(ip);
                            adrs[ip] = "" + r["id"];
                        }
                    }
                }
            }
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

            if ((tmr == null || !tmr.Enabled) && !ImportAsync.Is_buzy_any())
            {
                init_timer();
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }



        protected void Application_End(object sender, EventArgs e)
        {
            StopImport();
            tmr.Stop();

            db.ExecuteCmd("update owng set state='' where state in ('o','u')");
            db.ExecuteCmd("update angood set state='' where state<>''");
        }

        public static void forceimport()
        {
            ImportAsync.Clear_buzy(100000);
            ImportAsync.Clear_buzy(100001);
            ImportAsync.Clear_buzy(100002);
            ImportAsync.Clear_buzy(100003);
            ImportAsync.Clear_buzy(100004);

        }


        protected static void timer_event(object sender, System.Timers.ElapsedEventArgs e)
        {
            LastTik = DateTime.Now;
            tmr.Stop();
            check_newsubjects();
            import_ost();
            update_invoices();
            tmr.Interval = 300000;
            tmr.Start();
        }

        private static void check_newsubjects()
        {
            if (is_dev) return;
            DataTable dt = db.GetDbTable("select id,inn, name, emailtas, state from subj where state in ('u','?')");
            if (dt.Rows.Count == 0)
                return;
            DataTable udt = db.GetDbTable("select id, name, email, cast(pass as nvarchar) as psw from " + pUser.TDB + " where subjectID=" + dt.Rows[0]["ID"]);
            if (udt.Rows.Count == 0)
                return;

            EmailMessage email;
            string emaildefaultta = SysSetting.GetValue("emaildefaultta");
            if (emaildefaultta == "") emaildefaultta = CurrentCfg.EmailSupport;
            foreach (DataRow r in dt.Rows)
            {
                if ("" + r["state"] == "?")
                {
                    email = new EmailMessage(
                        CurrentCfg.EmailPortal,
                        emaildefaultta,
                        HelpersPath.RootUrl + ": запрос на регистрацию нового пользователя",
                        "<p>Новый пользователь:</p>" +
                        "<p>имя: " + udt.Rows[0]["name"] + "</p><p>e-mail: " + udt.Rows[0]["email"] + "</p>" +
                        "<p>представитель предприятия: " + r["Name"] + ", ИНН: " + r["INN"] + "</p>" +
                        "<p><strong>ТАКОГО ПРЕДПРИЯТИЯ в 1С НЕТ</strong></p>" +
                        "<p>ссылка на профайл пользователя: <a href='" + HelpersPath.RootUrl + "/ADMIN/admin_usr.aspx?id=" + udt.Rows[0]["ID"] + "'>открыть...</a></p>");
                    email.Send("" + CurrentCfg.EmailSupport);
                    continue;
                }
                if ("" + r["state"] == "u")
                {

                    db.ExecuteCmd("update SUBJ set state='' where id=" + r["id"]);
                    db.ExecuteCmd("update " + pUser.TDB + " set LoginEnabled='Y' where id=" + udt.Rows[0]["id"]);
                    email = new EmailMessage(
                        CurrentCfg.EmailPortal,
                        "" + udt.Rows[0]["email"],
                         "Регистрация на сайте Самостоятельной заявки " + HelpersPath.RootUrl,
                        "<p>Уважаемый(ая) " + udt.Rows[0]["name"] + "</p>" +
                        "<p>представитель предприятия: " + r["Name"] + ", ИНН: " + r["INN"] + "</p>" +
                        "<p>Ваш логин: " + udt.Rows[0]["email"] + "</p>" +
                        "<p>Ваш пароль: " + udt.Rows[0]["psw"].ToString().Replace("\0", "") + "</p>" +
                        "<p>ссылка на сайт: <a href='" + HelpersPath.RootUrl + "/default.aspx'>" + HelpersPath.RootUrl + "</a></p>");
                    email.Send("" + CurrentCfg.EmailSupport);

                    email = new EmailMessage(
                        CurrentCfg.EmailPortal,
                        "" + r["emailtas"],
                        HelpersPath.RootUrl + ": Регистрация нового пользователя",
                        "<p>Новый пользователь:</p>" +
                        "<p>имя: " + udt.Rows[0]["name"] + "</p><p>e-mail: " + udt.Rows[0]["email"] + "</p>" +
                        "<p>представитель предприятия: " + r["Name"] + ", ИНН: " + r["INN"] + "</p>" +
                        "<p>ссылка на сайт: <a href='" + HelpersPath.RootUrl + "/default.aspx'>" + HelpersPath.RootUrl + "</a></p>");
                    email.Send("" + CurrentCfg.EmailSupport);
                    continue;
                }

            }
        }


        private static void import_ost(int ownerId, int whId)
        {
            if (File.Exists(path + @"\" + ownerId + "_" + whId + ".csv") && (is_dev || ImportAsync.need_import(File.GetLastWriteTime(path + @"\" + ownerId + "_" + whId + ".csv"), ownerId)) && !ImportAsync.Is_buzy_any())
            {
                try
                {
                    eLog log = new eLog();
                    log.Stack.Add(new eLog.LogRecord(new sObject(ownerId, "import"), "ost", "", DateTime.Now.ToString(), "I"));
                    eLog.Save(log, ownerId);
                    ImportAsync.Import_ost(ownerId, path + @"\" + ownerId + "_" + whId + ".csv", true);
                    ImportAsync.Clear_buzy(ownerId);
                    File.Delete(path + @"\" + ownerId + "_" + whId + ".csv");
                }
                catch (Exception ex)
                {
                    appError.SaveError("global", 237, ex.Message, "timer");
                }
            }
        }

        private static void import_ost()
        {

            //УЦСК: 100000_100000.csv
            //челяб: 100001_100001.csv
            //тагил: 100002_100003.csv
            //тюмень: 100003_100004.csv
            //Сургут: 100004_100002.csv

            //DateTime filetime = File.GetLastWriteTime(path + @"\100000_100000.csv");
            //DateTime imptime = cDate.cToDateTime(db.GetDbTable("select max(lcd) from owng where ownerId=100000").Rows[0][0]);
            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(new sObject(1, "import"), "timer", "", DateTime.Now.ToString(), "T"));
            eLog.Save(log, 100000);




            string res = "";
            if (File.Exists(path + @"\full_import.csv") && !ImportAsync.Is_buzy_any() && !ImportAsync.checkTodayImported())
            {
                try
                {
                    log = new eLog();
                    log.Stack.Add(new eLog.LogRecord(new sObject(100000, "import"), "full", "", DateTime.Now.ToString(), "I"));
                    eLog.Save(log, 100973);

                    res = ImportAsync.Full_import(path + @"\full_import.csv");
                    ImportAsync.Clear_buzy(100000);

                    ImportAsync.ImportAngood();

                    File.Delete(path + @"\full_import.csv");
                    File.Delete(webIO.GetAbsolutePath("../exch/angood.csv"));
                }
                catch (Exception ex)
                {
                    appError.SaveError("global", 279, ex.Message, "timer");
                }
            }


            import_ost(100000, 100000); // уцск
            import_ost(100001, 100001); // челяб
            import_ost(100002, 100003); // тагил
            import_ost(100003, 100004); // тюмень
            import_ost(100004, 100002); // сургут





        }


        private static void update_invoices()
        {
            int i = 0;
            foreach (string filename in Directory.GetFiles(webIO.GetAbsolutePath("../exch"), "*.pdf"))
            {
                update_invoice(Path.GetFileName(filename));
                
                if (i==101) break;
                else i += 1;
            }
        }

        private static void update_invoice(string filename)
        {
            int id = cNum.cToInt(filename.Replace(".pdf", ""));
            byte[] bufinvoice;
            if (webIO.CheckExistFile("../exch/" + filename))
            {
                bool upd = true;
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
                try
                {
                    
                    cn.Open();
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.Parameters.Add("@src", System.Data.SqlDbType.Image);

                    FileStream stream = new FileStream(webIO.GetAbsolutePath("../exch/" + filename), FileMode.Open);
                    BinaryReader reader = new BinaryReader(stream);
                    bufinvoice = reader.ReadBytes((int)stream.Length);
                    reader.Close();
                    stream.Close();


                    cmd.Parameters["@src"].Value = bufinvoice;
                    cmd.CommandText = "update " + Order.TDB + " set invoicesrc=@src where id=" + id;
                    cmd.ExecuteNonQuery();

                }
                catch
                {
                    upd = false;
                    //ResponceMistake(context, "Ошибка при попытке сформировать счет");
                }
                finally
                {
                    cn.Close();
                }
                if (upd)
                    File.Delete(webIO.GetAbsolutePath("../exch/" + id + ".pdf"));
            }
        }
    }
}