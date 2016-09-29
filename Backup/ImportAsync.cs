using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Data.SqlClient;
using ensoCom;
using System.Data;
using System.IO;
using System.Text;
namespace wstcp
{
    public class ImportAsync
    {
        int OwnID;
        int WhID;
        string file_name;
        string log_name;
        string path;
        bool is_dev; 
        
        public ImportAsync(int owner_id, int warehouse_id,string file_name, string log_name)
        {
            this.OwnID = owner_id;
            this.WhID = warehouse_id;
            this.file_name = file_name;
            this.log_name = log_name;
            this.path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch";
            this.is_dev = (path.IndexOf("Projects") > -1);
        }

        [STAThread]
        public void DoImport()
        {
            
            //УЦСК: 100000_100000.csv
            //челяб: 100001_100001.csv
            //тагил: 100002_100003.csv
            //тюмень: 100003_100004.csv
            //Сургут: 100004_100002.csv

            
            

            
            
            string res = "";
            if (File.Exists(path + @"\"+file_name) && !Is_buzy(OwnID) && !_is_file_denide(path + @"\"+file_name)) 
            {
                string log = DateTime.Now.ToString();
                Set_buzy(OwnID);
                
                //if (!is_dev) cleardoubles();
                //res = (is_dev)?"ok_dev:"+DateTime.Now : 
                res = Import_ost(OwnID, WhID, path + @"\"+file_name, (OwnID==100000));
                //if (!is_dev) 
                File.Delete(path + @"\"+file_name);
                log += " - " + DateTime.Now.ToString();
                Save_log(log_name, res+":"+log);
                Clear_buzy(OwnID);
            }
            

        }

        public static void Set_buzy(int owner_id)
        {
            if (db.ExecuteCmd("update buzy set buzyst='b', lcd=getdate() where ownerId=" + owner_id) == 0)
                db.ExecuteCmd("insert into buzy (ownerId,buzyst,lcd) values (" + owner_id + ",'b', getdate())");
        }
        public static void Clear_buzy(int owner_id)
        {
            if (db.ExecuteCmd("update buzy set buzyst='', lcd=getdate() where ownerId=" + owner_id) == 0)
                db.ExecuteCmd("insert into buzy (ownerId,buzyst,lcd) values (" + owner_id + ",'', getdate())");
        }

        public static bool Is_buzy(int owner_id)
        {
            int b = 1;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = "select count(*) from buzy where ownerId=" + owner_id + " and buzyst='b'";
                b = cNum.cToInt(cmd.ExecuteScalar());
            }
            finally
            {
                cn.Close();
            }
            return (b > 0);
        }
        public static bool Is_buzy_any()
        {
            int b = 1;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = "select count(*) from buzy where buzyst='b'";
                b = cNum.cToInt(cmd.ExecuteScalar());
            }
            finally
            {
                cn.Close();
            }
            return (b > 0);
        }

        private bool _is_file_denide(string file_name)
        {            
            bool res = true;
            try
            {
                StreamReader sr = File.OpenText(file_name);
                sr.Close();
                sr = null;
                res = false;
            }
            catch(Exception ex )
            {
                Save_log("buzyfile_" + file_name, "" + ex.ToString());
            }
            return res;
        }

        public static void Save_log(string owner, string param)
        {
            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(new sObject(1, "import"), owner, param, "" + DateTime.Now.ToString(), "I"));
            log.Save(973);
            
        }
        private static DataTable __load_file_ost(string filename)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("КодНоменклатуры");
            dt.Columns.Add("Наименование");
            dt.Columns.Add("Свободный", typeof(int));
            dt.Columns.Add("ТН");
            dt.Columns.Add("ТК");
            dt.Columns.Add("хНаименование");
            dt.Columns.Add("zn");
            dt.Columns.Add("Брэнд");
            dt.Columns.Add("zn_z");
            dt.Columns.Add("pr_spr", typeof(decimal));
            dt.Columns.Add("pr_b", typeof(decimal));
            dt.Columns.Add("ens");
            dt.Columns.Add("z2");
            dt.Columns.Add("z3");
            string[] lines = File.ReadAllLines(filename);
            string[] lg;
                    DataRow nr;
                    decimal qty = 0;
            try
            {
                
                
                    
                    foreach(string line in lines)
                    {                       
                        lg = line.Replace((char)160, ' ').Split(';');

                        qty = cNum.cToDecimal(lg[2]);
                        
                        if (qty > 0)
                        {
                            nr = dt.NewRow();
                            nr["КодНоменклатуры"] = lg[0].Trim();
                            nr["Наименование"] = lg[1].Trim();
                            nr["Свободный"] = qty;
                            nr["ТН"] = lg[3].Trim();
                            nr["ТК"] = lg[4].Trim();
                            nr["хНаименование"] = __get_xName(""+nr["Наименование"]);
                            nr["Брэнд"] = lg[5].Trim();
                            nr["zn"] = lg[6].Trim();
                            nr["zn_z"] = lg[7].Trim();
                            nr["pr_spr"] = cNum.cToDecimal(lg[8]);
                            nr["pr_b"] = cNum.cToDecimal(lg[9]);
                            nr["ens"] = lg[10].Trim();
                            nr["z2"] = (lg.Length>11)?lg[11].Trim():"";
                            nr["z3"] = (lg.Length>12)?lg[12].Trim():"";
                            dt.Rows.Add(nr);
                        }
                    } 
                
            }
            catch (Exception ex)
            {
                Save_log("buzyfile_"+filename, ""+ex.ToString());
            }
            return dt;
        }

        public static string  __get_xName(string full_name)
        {
            string[]  sp = full_name.Trim().Split(' ');
            return sp[0];
        }
/*
        public static string Import_ost(int owner_id, int warehouse_id, string filepath, bool correct_info = false)
        {
            
            bool result = false;
            //DataTable dt = __load_file_ost(filepath);
            

            string corrector = "site";

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            int i = 0;
            string err = "";
            string[] lines = File.ReadAllLines(filepath);
            string[] lg;
            string res = "read: "+ lines.Length;
            try
            {

                cn.Open();
                cmd.Parameters.AddWithValue("@Corrector", corrector);
                cmd.Parameters.AddWithValue("@OwnerId", owner_id);
                cmd.Parameters.AddWithValue("@WhId", warehouse_id);
                cmd.Parameters.AddWithValue("@Name", "");
                cmd.Parameters.AddWithValue("@Category", "");
                cmd.Parameters.AddWithValue("@xName", "");
                cmd.Parameters.AddWithValue("@TN", "");
                cmd.Parameters.AddWithValue("@TK", "");
                cmd.Parameters.AddWithValue("@Brend", "");
                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@zn", "");
                cmd.Parameters.AddWithValue("@zn_z", "");
                cmd.Parameters.AddWithValue("@z2", "");
                cmd.Parameters.AddWithValue("@z3", "");
                cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_b", (decimal)0);
                cmd.Parameters.AddWithValue("@ens", "");
                cmd.Parameters.AddWithValue("@State", "");
                cmd.Parameters.AddWithValue("@Qty", (decimal)0);
                cmd.Parameters.AddWithValue("@correctinfo", 0);
                
                cmd.CommandText = "ImportOst";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (string line in lines)
                {
                    //if (Global.NEEDSTOPIMPORT) break;
                    lg = line.Replace((char)160, ' ').Split(';');

                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        cmd.Parameters["@Name"].Value = lg[1].Trim();
                        cmd.Parameters["@Qty"].Value = cNum.cToDecimal(lg[2]);
                        cmd.Parameters["@Category"].Value = lg[3].Trim() + "|" + lg[4].Trim();
                        cmd.Parameters["@TN"].Value = lg[3].Trim();
                        cmd.Parameters["@TK"].Value = lg[4].Trim();
                        cmd.Parameters["@xName"].Value = __get_xName(lg[1].Trim());
                        cmd.Parameters["@Brend"].Value = lg[5].Trim();
                        cmd.Parameters["@zn"].Value = lg[6].Trim();
                        cmd.Parameters["@zn_z"].Value = lg[7].Trim();
                        cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(lg[8]);
                        cmd.Parameters["@pr_b"].Value = cNum.cToDecimal(lg[9]);
                        cmd.Parameters["@ens"].Value = lg[10].Trim();
                        cmd.Parameters["@z2"].Value = (lg.Length > 11) ? lg[11].Trim() : "";
                        cmd.Parameters["@z3"].Value = (lg.Length > 12) ? lg[12].Trim() : "";
                        
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        cStr.Add(ref err, "" + cmd.Parameters["@GoodCode"].Value);
                    }

                    i += 1;
                }
                cmd.CommandType = CommandType.Text;
                db.ExecuteCmd(cmd, "update OWNG set zn_z='D0' where goodid in (select goodid from gincash where isnull(St,'')<>'u' and OwnerId=@OwnerId)");
                db.ExecuteCmd(cmd, "update GINCASH set Qty=0, lcd=getdate() where isnull(St,'')<>'u' and OwnerId=@OwnerId");
                db.ExecuteCmd(cmd, "update GINCASH set St='', lcd=getdate() where St='u' and OwnerId=@OwnerId");
                result = true;
            }
            catch(Exception ex)
            {
                Save_log("ggg" + owner_id, ex.ToString() + " код товара: " + cmd.Parameters["@GoodCode"].Value);
                result = false;
            }
            finally
            {
                cn.Close();
            }
            if (result)
            {
                try
                {
                    File.Delete(filepath);
                }
                catch { }
            }
            return res+"; обновлено всего " + i + ". Ошибки с номенклатурой: "+err;
        }
*/

        public static string Import_ost(int owner_id, int warehouse_id, string filepath, bool correct_info = false)
        {
            if (Global.NEEDSTOPIMPORT) return "";

            bool result = false;
           


            string corrector = "site";

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            int i = 0;
            string err = "";
            string[] lines = File.ReadAllLines(filepath);
            string[] lg;
            string res = "read: " + lines.Length;
            try
            {

                cn.Open();
                cmd.Parameters.AddWithValue("@Corrector", corrector);
                cmd.Parameters.AddWithValue("@OwnerId", owner_id);
                cmd.Parameters.AddWithValue("@WhId", warehouse_id);

                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@zn", "");
                cmd.Parameters.AddWithValue("@zn_z", "");
                cmd.Parameters.AddWithValue("@z2", "");
                cmd.Parameters.AddWithValue("@z3", "");
                cmd.Parameters.AddWithValue("@zt", "");
                cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_b", (decimal)0);
               
                
                cmd.Parameters.AddWithValue("@Qty", (decimal)0);


                cmd.CommandText = "microImport";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (string line in lines)
                {
                    if (Global.NEEDSTOPIMPORT) break;
                    lg = line.Replace((char)160, ' ').Split(';');

                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        
                        cmd.Parameters["@Qty"].Value = cNum.cToDecimal(lg[2]);
                        cmd.Parameters["@zn"].Value = lg[6].Trim();
                        cmd.Parameters["@zn_z"].Value = lg[7].Trim();
                        cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(lg[8]);
                        cmd.Parameters["@pr_b"].Value = (lg[6].Trim().ToLower()=="z" && lg.Length > 13 && cNum.cToInt(lg[13]) > 1) ? 0 : cNum.cToDecimal(lg[9]);
                        
                        cmd.Parameters["@z2"].Value = lg[11].Trim();
                        cmd.Parameters["@z3"].Value = lg[12].Trim();
                        cmd.Parameters["@zt"].Value = (lg[6].Trim().ToLower()=="z" && lg.Length > 13) ? lg[13].Trim() : "";

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        cStr.Add(ref err, "" + cmd.Parameters["@GoodCode"].Value);
                    }

                    i += 1;
                }
                cmd.CommandType = CommandType.Text;

                if (!Global.NEEDSTOPIMPORT)
                {
                    db.ExecuteCmd(cmd, "update OWNG set Qty=0, lcd=getdate() where isnull([State],'')<>'u' and OwnerId=@OwnerId");
                    db.ExecuteCmd(cmd, "update OWNG set zn_z='D0' where isnull([State],'')<>'u' and OwnerId=@OwnerId");
                }
                db.ExecuteCmd(cmd, "update OWNG set [State]='', lcd=getdate() where [State]='u' and OwnerId=@OwnerId");

                result = true;
            }
            catch (Exception ex)
            {
                Save_log("ggg" + owner_id, ex.ToString() + " код товара: " + cmd.Parameters["@GoodCode"].Value);
                result = false;
            }
            finally
            {
                cn.Close();
            }
            if (result)
            {
                try
                {
                    File.Delete(filepath);
                }
                catch { }
            }
            return res + "; обновлено всего " + i + ". Ошибки с номенклатурой: " + err;
        }








        public static void Cleardoubles()
        {
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
                cn.Open();
                db.ExecuteCmd(cmd, "delete from good where Name in (select name from (SELECT count(ID) as qty, Name FROM GOOD AS GOOD_1 GROUP BY Name) AS derivedtbl_1 WHERE qty > 1)");
                db.ExecuteCmd(cmd, "DELETE FROM OWNG WHERE (GoodId NOT IN (SELECT ID FROM GOOD))");
//                db.ExecuteCmd(cmd, "DELETE FROM GINCASH WHERE (GoodId NOT IN (SELECT ID FROM GOOD))");
                db.ExecuteCmd(cmd, "DELETE FROM OWNG WHERE (GoodCode IN (SELECT GoodCode FROM (SELECT DISTINCT GoodId, GoodCode FROM OWNG AS OWNG_1) AS tbd GROUP BY GoodCode HAVING (COUNT(GoodId) > 1)))");
                db.ExecuteCmd(cmd, "DELETE FROM GOOD WHERE (ID NOT IN (SELECT GoodId  FROM OWNG))");
                db.ExecuteCmd(cmd, "DELETE FROM OWNG WHERE (GoodId NOT IN (SELECT ID FROM GOOD))");
  //              db.ExecuteCmd(cmd, "DELETE FROM GINCASH WHERE (GoodId NOT IN (SELECT ID FROM GOOD))");
            }
            catch
            {
            }
            finally
            {
                cn.Close();
            }
            return;
        }

        public static bool checkTodayImported()
        {
            bool ret = false;
            DataTable dt = db.GetDbTable("select max(lastupd) from ELG where Entity='fullimport'");
            ret = (cDate.cToDate(dt.Rows[0][0]).Date == DateTime.Today.Date);
            return ret;
        }
        
        public static string Full_import(string pathfile)
        {
       
            int i = __fullimport(pathfile);
            
            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(new sObject(1,"fullimport"),"100000",""+i+" records",""+DateTime.Now.ToString(),"I"));
            log.Save(973);            
            wstcp.Models.GoodInfo.__structdt = new System.Collections.Hashtable();
            return " Полное обноление " + i + " позиций.";
        }
        private static int __fullimport(string filename, int owner_id = 100000)
        {
            int i = 0;
            string[] lines = ("").Split(',');
            bool result = false;
            eLog log = new eLog();
            try
            {
                lines = File.ReadAllLines(filename);
                if (lines.Length > 0)
                {
                    log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "file read: " + lines.Length + " lines", "" + DateTime.Now.ToString(), "I"));
                    log.Save(973);
                }
                else
                {
                    log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "file read: " + lines.Length + " lines", "" + DateTime.Now.ToString(), "I"));
                    log.Save(973);
                    return 0;
                }
                
            }
            catch
            {
                
                log.Stack.Add(new eLog.LogRecord(new sObject(1,"fullimport"),"100000","error read file",""+DateTime.Now.ToString(),"I"));
               log.Save(973); 
                return 0;
            }
            string repfile = webIO.GetAbsolutePath("../exch/fullimprep.txt");
            File.CreateText(repfile);
            int w = 0;
            string err = "";
            string[] lg;
            Set_buzy(owner_id);
            
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            
            cn.Open();
            try
            {


                // w = db.ExecuteCmd(cmd, "delete from owng where goodid not in (select id from good)");


                w = db.ExecuteCmd(cmd, "update OWNG set state='o' where ownerid=" + owner_id);


                cmd.Parameters.AddWithValue("@Corrector", "import");
                cmd.Parameters.AddWithValue("@OwnerId", owner_id);

                cmd.Parameters.AddWithValue("@Name", "");
                cmd.Parameters.AddWithValue("@Category", "");
                cmd.Parameters.AddWithValue("@xName", "");
                cmd.Parameters.AddWithValue("@TN", "");
                cmd.Parameters.AddWithValue("@TK", "");
                cmd.Parameters.AddWithValue("@Brend", "");
                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@zn", "");
                cmd.Parameters.AddWithValue("@zn_z", "");
                cmd.Parameters.AddWithValue("@z2", "");
                cmd.Parameters.AddWithValue("@z3", "");
                cmd.Parameters.AddWithValue("@zt", "");
                cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_b", (decimal)0);

                cmd.Parameters.AddWithValue("@ens", "");
                cmd.Parameters.AddWithValue("@State", "");
                cmd.Parameters.AddWithValue("@article", "");
                cmd.Parameters.AddWithValue("@ed", "");

                cmd.CommandText = "savegoodfullimport";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (string r in lines)
                {
                    lg = r.Replace((char)160, ' ').Split(';');

                    if (lg[0].Trim() == "" || lg.Length < 11 || lg.Length > 16)
                    {
                        //.AppendTextwebIO.GetAbsolutePath("../exch")
                        cStr.Add(ref err, "пропуск строки " + i);
                        continue;
                    }
                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        cmd.Parameters["@Name"].Value = lg[1].Trim();
                        cmd.Parameters["@Category"].Value = lg[3].Trim() + "|" + lg[4].Trim();
                        cmd.Parameters["@TN"].Value = lg[3].Trim();
                        cmd.Parameters["@TK"].Value = lg[4].Trim();
                        cmd.Parameters["@xName"].Value = __get_xName(lg[1].Trim());
                        cmd.Parameters["@Brend"].Value = lg[5].Trim();
                        cmd.Parameters["@zn"].Value = lg[6].Trim();
                        cmd.Parameters["@zn_z"].Value = lg[7].Trim();
                        cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(lg[8]);
                        cmd.Parameters["@pr_b"].Value = cNum.cToDecimal(lg[9]);
                        cmd.Parameters["@ens"].Value = lg[10].Trim();
                        cmd.Parameters["@article"].Value = lg[11].Trim();
                        cmd.Parameters["@ed"].Value = lg[12].Trim();
                        cmd.Parameters["@z2"].Value = lg[13].Trim();
                        cmd.Parameters["@z3"].Value = lg[14].Trim();
                        cmd.Parameters["@zt"].Value = (lg[6].Trim().ToLower() == "z" && lg.Length > 15) ? lg[15].Trim() : "";


                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        cStr.Add(ref err, "" + cmd.Parameters["@GoodCode"].Value);
                    }

                    i += 1;
                }             
                result = true;
            }
            catch (Exception ex)
            {
                log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "error: " + ex.ToString(), "" + DateTime.Now.ToString(), "I"));
            }
            finally
            {
                cn.Close();
            }
            if (result)
            {
                w = db.ExecuteCmd(cmd, "delete from owng where goodid not in (select id from good)");
                w = db.ExecuteCmd(cmd, "delete from OWNG where ownerid=@OwnerId and State='o'");
                try
                {
                    File.Delete(filename);
                }
                catch { }
            }
            else
            {
                db.ExecuteCmd(cmd, "update OWNG set state='' where ownerid=@OwnerId and State='o'");
            }
            if (err != "")
            {
                log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "error save to db ("+err+")", "" + DateTime.Now.ToString(), "I"));
                log.Save(973);
            }
            
            return i;
        }

/*
        private static DataTable __load_filefullimport(string filename)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("КодНоменклатуры");
            dt.Columns.Add("Наименование");

            dt.Columns.Add("ТН");
            dt.Columns.Add("ТК");
            dt.Columns.Add("хНаименование");
            dt.Columns.Add("Брэнд");
            dt.Columns.Add("zn");
            dt.Columns.Add("zn_z");
            dt.Columns.Add("pr_spr");
            dt.Columns.Add("pr_b");
            dt.Columns.Add("ens");
            dt.Columns.Add("артикул");
            dt.Columns.Add("ед");


            try
            {
                string[] lines = File.ReadAllLines(filename);
                using (StreamReader sr = File.OpenText(filename))
                {
                    string line = null;
                    string[] lg;
                    DataRow nr;

                    do
                    {
                        line = sr.ReadLine();
                        if (line == null) break;
                        lg = line.Replace((char)160, ' ').Split(';');
                        if (lg[0].Trim() == "")
                            continue;
                        nr = dt.NewRow();
                        nr["КодНоменклатуры"] = lg[0].Trim();
                        nr["Наименование"] = lg[1].Trim();//.Replace("\"","");
                        nr["ТН"] = lg[3].Trim();
                        nr["ТК"] = lg[4].Trim();
                        nr["хНаименование"] = __get_xName("" + nr["Наименование"]);
                        nr["Брэнд"] = lg[5].Trim();
                        nr["zn"] = lg[6].Trim();
                        nr["zn_z"] = lg[7].Trim();
                        nr["pr_spr"] = cNum.cToDecimal(lg[8]);
                        nr["pr_b"] = cNum.cToDecimal(lg[9]);
                        nr["ens"] = lg[10].Trim();
                        nr["артикул"] = lg[11].Trim();
                        nr["ед"] = lg[12].Trim(); 
                        dt.Rows.Add(nr);

                    } while (line != null);
                }
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        //static string get_xName(string full_name)
        //{
        //    string[] sp = full_name.Trim().Split(' ');
        //    return sp[0];
        //}

        private static int __save_to_db_fullimport(DataTable dt, int owner_id = 100000)
        {
            bool result = false;
            //int gId;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            int i = 0;            
            string err = "";
            try
            {
                Set_buzy(owner_id);
                cn.Open();
                db.ExecuteCmd(cmd, "delete owng where goodid not in (select id from good)");
                db.ExecuteCmd(cmd, "delete gincash where goodid not in (select id from good)");

                db.ExecuteCmd(cmd, "update OWNG set state='o' where ownerid=" + owner_id);


                cmd.Parameters.AddWithValue("@Corrector", "import");
                cmd.Parameters.AddWithValue("@OwnerId", owner_id);

                cmd.Parameters.AddWithValue("@Name", "");
                cmd.Parameters.AddWithValue("@Category", "");
                cmd.Parameters.AddWithValue("@xName", "");
                cmd.Parameters.AddWithValue("@TN", "");
                cmd.Parameters.AddWithValue("@TK", "");
                cmd.Parameters.AddWithValue("@Brend", "");
                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@zn", "");
                cmd.Parameters.AddWithValue("@zn_z", "");
                cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_b", (decimal)0);

                cmd.Parameters.AddWithValue("@ens", "");
                cmd.Parameters.AddWithValue("@State", "");
                //int g_id = 0;

                foreach (DataRow r in dt.Rows)
                {
                    //continue;
                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = r["КодНоменклатуры"].ToString();
                        cmd.Parameters["@Name"].Value = r["Наименование"].ToString();
                        cmd.Parameters["@Category"].Value = "" + r["ТН"] + "|" + r["ТК"];
                        cmd.Parameters["@TN"].Value = "" + r["ТН"];
                        cmd.Parameters["@TK"].Value = "" + r["ТК"];
                        cmd.Parameters["@xName"].Value = "" + r["хНаименование"];
                        cmd.Parameters["@Brend"].Value = "" + r["Брэнд"];
                        cmd.Parameters["@zn"].Value = "" + r["zn"];
                        cmd.Parameters["@zn_z"].Value = "" + r["zn_z"];
                        cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(r["pr_spr"]);
                        cmd.Parameters["@pr_b"].Value = cNum.cToDecimal(r["pr_b"]);

                        cmd.Parameters["@ens"].Value = "" + r["ens"];
                        cmd.CommandText = "savegoodfullimport";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();


                    }
                    catch (Exception ex)
                    {
                        cStr.Add(ref err, "" + cmd.Parameters["@GoodCode"].Value);
                    }

                    i += 1;
                }
                cmd.CommandType = CommandType.Text;
                db.ExecuteCmd(cmd, "delete owng where goodid not in (select id from good)");
                db.ExecuteCmd(cmd, "delete gincash where goodid not in (select id from good)");
                db.ExecuteCmd(cmd, "delete GINCASH where goodid not in (select goodid from OWNG where OwnerId=@OwnerId)");
                db.ExecuteCmd(cmd, "delete from OWNG where ownerid=@OwnerId and State='o'");


                result = true;
            }
            catch (Exception ex)
            {
                cStr.Add(ref err, " ошибка " + cmd.Parameters["@GoodCode"].Value);
                result = false;
            }
            finally
            {
                cn.Close();
                Clear_buzy(owner_id);
            }
            if (err != "")
            {
                Save_log("fullimport", "error:"+DateTime.Now.ToString()+": "+err);                
            }
            return i;
        }
*/


    }
}