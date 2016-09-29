using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using ensoCom;
using System.Data;
using wstcp.Models;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using System.Text;

namespace wstcp
{
    public class Global : System.Web.HttpApplication
    {

        public static bool NEEDSTOPIMPORT = false;
        private static string __url = "";
        public static string RootUrl
        {
            get { return __url; }
        }

        static string path = System.AppDomain.CurrentDomain.BaseDirectory + "exch";
        static bool is_dev = ((System.AppDomain.CurrentDomain.BaseDirectory + "exch").IndexOf("Projects") > -1);
        public static Hashtable adrs;
        public static List<string> ips;

        protected static System.Timers.Timer tmr = new System.Timers.Timer();
        protected static System.Timers.Timer tmrfull = new System.Timers.Timer();
        
        
        protected void Application_Start(object sender, EventArgs e)
        {
            ensoCom.db.InitDbConnection("default", System.Configuration.ConfigurationManager.AppSettings["defaultcn"]);
            //pUser.Init1stUser("alexzh@santur.ru");
            ImportAsync.Clear_buzy(100000);
            ImportAsync.Clear_buzy(100001);
            ImportAsync.Clear_buzy(100002);
            ImportAsync.Clear_buzy(100003);
            ImportAsync.Clear_buzy(100004);

            init_timer();
        }
        public static void init_timer()
        {
            //if (is_dev) return;
            tmrfull = new System.Timers.Timer(60000);
            tmrfull.Elapsed += new System.Timers.ElapsedEventHandler(tmrfull_event);
            tmrfull.Enabled = true;
            tmrfull.Start();
            
            
            tmr = new System.Timers.Timer(60000);
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(timer_event);
            tmr.Enabled = true;
            tmr.Start();

            
        }

        

        public static void StopImport()
        {
            NEEDSTOPIMPORT = true;
            tmr.Stop();
        }

        public static void StartImport()
        {
            NEEDSTOPIMPORT = false;   
            timer_event(null, null);
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

            tmrfull.Stop();
            db.ExecuteCmd("update owng set state='' where state in ('o','u')");
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
            tmr.Stop();
            import_ost();
            tmr.Interval = 300000;
            tmr.Start();
        }
        protected static void tmrfull_event(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrfull.Stop();
           // import_full();            
        }
        private static void import_full()
        {
            return;

            string SaveLocation = path + @"\full_import.csv";
            if (File.Exists(SaveLocation)) //УЦСК
            {
                tmr.Stop();
                tmrfull.Stop();
                try
                {
                    
                    ImportAsync.Set_buzy(100000);//УЦСК
                    


                    ImportAsync.Full_import(SaveLocation);

                    File.Delete(SaveLocation);
                    
                    
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("fullimp_err", ex.ToString());
                }
                finally
                {
                    ImportAsync.Clear_buzy(100000);
                    
                    
                    tmr.Start();
                    tmrfull.Interval = 3600000;
                    tmrfull.Start();
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





            string res = "";
            if (File.Exists(path + @"\full_import.csv") && !ImportAsync.Is_buzy_any() && !ImportAsync.checkTodayImported())
            {
                try
                {
                    ImportAsync.Set_buzy(100000);
                    res = ImportAsync.Full_import(path + @"\full_import.csv");
                    ImportAsync.Clear_buzy(100000);
                    File.Delete(path + @"\full_import.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("fullimport_err", ex.ToString());
                }
            }


            //if (NEEDSTOPIMPORT) return;
            if (File.Exists(path + @"\100000_100000.csv") && !ImportAsync.Is_buzy_any()) //УЦСК
            {
                try
                {
                    ImportAsync.Set_buzy(100000);

                    res = ImportAsync.Import_ost(100000, 100000, path + @"\100000_100000.csv", true);
                    //
                    ImportAsync.Save_log("ucsk", res);
                    ImportAsync.Clear_buzy(100000);
                    File.Delete(path + @"\100000_100000.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("ucsk_err", ex.ToString());
                }
            }
            //if (NEEDSTOPIMPORT) return;
            if (File.Exists(path + @"\100001_100001.csv") && !ImportAsync.Is_buzy_any()) //челяб
            {
                try
                {
                    ImportAsync.Set_buzy(100001);
                    res = ImportAsync.Import_ost(100001, 100001, path + @"\100001_100001.csv");
                    //

                    ImportAsync.Save_log("chel", res);
                    ImportAsync.Clear_buzy(100001);
                    File.Delete(path + @"\100001_100001.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("chel_err", ex.ToString());
                }
            }

           // if (NEEDSTOPIMPORT) return;

            if (File.Exists(path + @"\100002_100003.csv") && !ImportAsync.Is_buzy_any()) //тагил
            {
                try
                {
                    ImportAsync.Set_buzy(100002);
                    res = ImportAsync.Import_ost(100002, 100003, path + @"\100002_100003.csv");
                    //

                    ImportAsync.Save_log("tag", res);
                    ImportAsync.Clear_buzy(100002);
                    File.Delete(path + @"\100002_100003.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("tag_err", ex.ToString());
                }
            }
            
            //if (NEEDSTOPIMPORT) return;

            if (File.Exists(path + @"\100003_100004.csv") && !ImportAsync.Is_buzy_any()) //тюмень
            {
                try
                {
                    ImportAsync.Set_buzy(100003);
                    res = ImportAsync.Import_ost(100003, 100004, path + @"\100003_100004.csv");
                    //

                    ImportAsync.Save_log("tum", res);
                    ImportAsync.Clear_buzy(100003);
                    File.Delete(path + @"\100003_100004.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("tum_err", ex.ToString());
                }
            }

           // if (NEEDSTOPIMPORT) return;

            if (File.Exists(path + @"\100004_100002.csv") && !ImportAsync.Is_buzy_any()) // Сургут
            {
                try
                {
                    ImportAsync.Set_buzy(100004);
                    res = ImportAsync.Import_ost(100004, 100002, path + @"\100004_100002.csv");
                    

                    ImportAsync.Save_log("surg", res);
                    ImportAsync.Clear_buzy(100004);
                    File.Delete(path + @"\100004_100002.csv");
                }
                catch (Exception ex)
                {
                    ImportAsync.Save_log("surg_err", ex.ToString());
                }
            }

        }

    }
}