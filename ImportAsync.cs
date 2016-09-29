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
using selforderlib;
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

        public ImportAsync(int owner_id, int warehouse_id, string file_name, string log_name)
        {
            this.OwnID = owner_id;
            this.WhID = warehouse_id;
            this.file_name = file_name;
            this.log_name = log_name;
            this.path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch";
            this.is_dev = (path.IndexOf("Projects") > -1);
        }


        public void DoImport()
        {
            //УЦСК: 100000_100000.csv
            //челяб: 100001_100001.csv
            //тагил: 100002_100003.csv
            //тюмень: 100003_100004.csv
            //Сургут: 100004_100002.csv

            if (!File.Exists(path + @"\" + file_name) || Is_buzy_any() || _is_file_denide(path + @"\" + file_name))
                return;





            string res = "";

            res = Import_ost(OwnID, path + @"\" + file_name, (OwnID == 100000));
            //if (!is_dev) 
            File.Delete(path + @"\" + file_name);
            Clear_buzy(OwnID);

        }

        public static void Clear_buzy(int owner_id)
        {
            db.ExecuteCmd("update OWNG set state='', lcd=getdate() where ownerId=" + owner_id);
        }
        public static bool need_import(DateTime filetime, int ownerId)
        {
            return true;
            bool res = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();

                cmd.CommandText = "select max(lcd) from OWNG where ownerId=" + ownerId;
                DateTime lcd = cDate.cToDateTime(cmd.ExecuteScalar());
                res = (filetime.ToUniversalTime() > lcd.ToUniversalTime()); // нужно вернуть 2
            }
            finally
            {
                cn.Close();
            }
            return res;
        }
        public static bool Is_buzy(int owner_id)
        {
            int b = 1;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = "select count(*) from OWNG where ownerId=" + owner_id + " and state in ('u','o')";
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
                cmd.CommandText = "select count(*) from OWNG where state in ('u','o')";
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
            catch (Exception ex)
            {
            }
            return res;
        }

        public static string Import_ost(int ownerId, string filepath, bool correct_info = false)
        {
            if (Global.NEEDSTOPIMPORT) return "";
            if (db.GetDbTable("select goodcode from imp_" + ownerId + " where CONVERT(varchar,lastupd,104)=CONVERT(varchar,getdate(),104)").Rows.Count > 100)
                return "";
            db.ExecuteCmd("delete from imp_" + ownerId + "");
            string logfile_path = "io" + ownerId + ".txt";
            StringBuilder logtxt = new StringBuilder();
            bool result = false;

            string corrector = "impost";

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            int i = 0;
            DateTime lcd = File.GetLastWriteTime(filepath);
            string[] lines = File.ReadAllLines(filepath);
            string[] lg;
            if (lines.Length < 10) return "";
            logtxt.AppendLine("impost: read: " + lines.Length + " " + DateTime.Now.ToString());
            try
            {
                cn.Open();

                cmd.Parameters.AddWithValue("@OwnerId", ownerId);

                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@zn", "");
                cmd.Parameters.AddWithValue("@zn_z", "");
                cmd.Parameters.AddWithValue("@z2", "");
                cmd.Parameters.AddWithValue("@z3", "");
                cmd.Parameters.AddWithValue("@zt", "");
                cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_b", (decimal)0);
                cmd.Parameters.AddWithValue("@qty", (decimal)0);

                cmd.Parameters.AddWithValue("@name", "");
                cmd.Parameters.AddWithValue("@xname", "");
                cmd.Parameters.AddWithValue("@xtn", "");
                cmd.Parameters.AddWithValue("@xtk", "");
                cmd.Parameters.AddWithValue("@brend", "");
                cmd.Parameters.AddWithValue("@ens", "");

                cmd.Parameters.AddWithValue("@pr_vip", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_spec", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_kropt", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_opt", (decimal)0);
                cmd.Parameters.AddWithValue("@pr_ngc", (decimal)0);
                cmd.Parameters.AddWithValue("@salekrat", (decimal)1);
                
                foreach (string line in lines)
                {
                    if (Global.NEEDSTOPIMPORT)
                    {
                        break;
                    }
                    lg = line.Replace((char)160, ' ').Split(';');

                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        cmd.Parameters["@Name"].Value = lg[1].Trim();
                        cmd.Parameters["@xname"].Value = __get_xName(lg[1].Trim());
                        cmd.Parameters["@qty"].Value = cNum.cToDecimal(lg[2]);
                        cmd.Parameters["@xtn"].Value = lg[3].Trim();
                        cmd.Parameters["@xtk"].Value = lg[4].Trim();
                        cmd.Parameters["@brend"].Value = lg[5].Trim();
                        cmd.Parameters["@zn"].Value = lg[6].Trim();
                        cmd.Parameters["@zn_z"].Value = lg[7].Trim();
                        cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(lg[8]);
                        cmd.Parameters["@pr_b"].Value = cNum.cToDecimal(lg[9]);

                        cmd.Parameters["@ens"].Value = lg[10].Trim();

                        cmd.Parameters["@z2"].Value = lg[11].Trim();
                        cmd.Parameters["@z3"].Value = lg[12].Trim();
                        cmd.Parameters["@zt"].Value = (lg[6].Trim().ToLower() == "z" && lg.Length > 13) ? lg[13].Trim() : "";
                       
                        cmd.Parameters["@pr_vip"].Value = cNum.cToDecimal(lg[14]);
                        cmd.Parameters["@pr_spec"].Value = cNum.cToDecimal(lg[15]);
                        cmd.Parameters["@pr_kropt"].Value = cNum.cToDecimal(lg[16]);
                        cmd.Parameters["@pr_opt"].Value = cNum.cToDecimal(lg[17]);
                        cmd.Parameters["@pr_ngc"].Value = cNum.cToDecimal(lg[18]);
                        decimal sk = cNum.cToDecimal(lg[19]);
                        cmd.Parameters["@salekrat"].Value = (sk<=0)?1:sk;



                        cmd.CommandText = "insert into imp_" + ownerId + " (GoodCode,zn,zn_z,pr_spr,pr_b,z2,z3,qty,zt,name,xtn,xtk,brend,xname,ens, pr_vip, pr_spec, pr_kropt, pr_opt, pr_ngc,salekrat) values "+
                                                                        "(@GoodCode,@zn,@zn_z,@pr_spr,@pr_b,@z2,@z3,@qty,@zt,@name,@xtn,@xtk,@brend,@xname,@ens, @pr_vip, @pr_spec, @pr_kropt, @pr_opt,@pr_ngc,@salekrat)";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        appError.SaveError(cmd, "ImportAcync", 228, "["+ownerId+":GoodCode:"+cmd.Parameters["@GoodCode"].Value+"] "+ex.Message, "import_ost");
                    }

                    i += 1;
                }

                if (!Global.NEEDSTOPIMPORT)
                {
                    try
                    {
                        DataTable notImp = db.GetDbTable("select name,xtn,xtk,brend,xname,ens from imp_" + ownerId + " as imp where goodcode not in (select goodcode from owng)");
                        foreach (DataRow r in notImp.Rows)
                        {
                            db.ExecuteCmd(cmd, "insert into GOOD (name,xtn,xtk,brend,xname,ens,lc,lcd) values"+
                                " ('" + r["name"] + "','" + r["xtn"] + "','" + r["xtk"] + "','" + r["brend"] + "','" + r["xname"] + "','" + r["ens"] + "','" + corrector + "',getdate())");

                        }

                        notImp = db.GetDbTable("select imp.goodcode, isnull((select max(owng.goodid) from owng where owng.goodcode=imp.goodcode),(select max(ID) from good where name=imp.name)) as goodId from imp_" + ownerId + " as imp  where goodcode not in (select goodcode from owng where ownerId=" + ownerId + ")");
                        foreach (DataRow r in notImp.Select("goodId>0"))
                        {
                            db.ExecuteCmd(cmd, "insert into OWNG (goodId, goodCode,ownerId,lc,lcd) values (" + r["goodid"] + ",'" + r["goodcode"] + "'," + ownerId + ",'" + corrector + "',getdate())");
                        }
                    }
                    catch (Exception ex)
                    {
                        appError.SaveError(cmd, "ImportAcync", 252, "[" + ownerId + "] " + ex.Message, "import_ost");
                    }
                    try
                    {

                        cmd.CommandText = "update OWNG set state='u',lcd=getdate(),lc='" + corrector +
                                          "', owng.pr_spr=imp.pr_spr, owng.pr_b=imp.pr_b, owng.qty=imp.qty, owng.zn=imp.zn, owng.zn_z=imp.zn_z, owng.z2=imp.z2, owng.z3=imp.z3, owng.zt=imp.zt,owng.pr_vip=isnull(imp.pr_vip,0), owng.pr_spec=isnull(imp.pr_spec,0), owng.pr_kropt=isnull(imp.pr_kropt,0), owng.pr_opt=isnull(imp.pr_opt,0),owng.pr_ngc=isnull(imp.pr_ngc,0),owng.salekrat=imp.salekrat from imp_" +
                                          ownerId + " as imp where owng.ownerId=" + ownerId +
                                          " and owng.GoodCode=imp.GoodCode";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update OWNG set Qty=0,zn_z='D0', lcd=getdate(),lc='" + corrector + "' where isnull(state,'')<>'u' and OwnerId=" + ownerId;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "update OWNG set state='', lcd=getdate(),lc='" + corrector + "' where state='u' and OwnerId=" + ownerId;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "delete from imp_" + ownerId + " where goodcode in (select goodcode from owng where ownerId=" + ownerId + ")";
                        cmd.ExecuteNonQuery();


                        result = true;
                    }
                    catch (Exception ex)
                    {
                        appError.SaveError(cmd, "ImportAcync", 273, "[" + ownerId + "] " + ex.Message, "import_ost");
                        result = false;
                    }
                }

            }
            catch (Exception ex)
            {
                appError.SaveError(cmd, "ImportAcync", 281, "[" + ownerId + "] " + ex.Message, "import_ost");
                result = false;
            }
            finally
            {
                cn.Close();
                db.ExecuteCmd("update owng set salekrat=1 where isnull(salekrat,0)=0");
            }
            //clearfile(filepath, i.ToString() + " ### " + logtxt.ToString());
            eLog log = new eLog();
            log.Stack.Add(new eLog.LogRecord(new sObject(ownerId, "import"), "ostresult", "import (" + i + ")", "" + DateTime.Now.ToString(), "I"));
            //if (logtxt.Length > 0)
            //    log.Stack.Add(new eLog.LogRecord(new sObject(ownerId, "import"), "errors", logtxt.ToString(), "" + DateTime.Now.ToString(), "I"));
            eLog.Save(log, 100000);
            return "обновлено всего " + i;
        }



        //private static void clearfile(string filepath, string qty_imported)
        //{
        //    StreamWriter sw = File.CreateText(filepath);
        //    sw.Write(qty_imported);
        //    sw.Close();
        //}



        public static void ImportAngood()
        {
            if (cNum.cToInt(db.GetDbTable("select count(*) from angood where state='u'").Rows[0][0])>0)
                return;

            StringBuilder logtxt = new StringBuilder();
            bool result = false;

            string corrector = "impost";

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            int i = 0;
            //DateTime lcd = File.GetLastWriteTime(filepath);
            string[] lines = File.ReadAllLines(webIO.GetAbsolutePath("../exch/angood.csv"));
            string[] lg;

            logtxt.AppendLine("impost: read: " + lines.Length + " " + DateTime.Now.ToString());
            try
            {
                cn.Open();

                cmd.Parameters.AddWithValue("@GoodCode", "");
                cmd.Parameters.AddWithValue("@AnCode", "");
                cmd.Parameters.AddWithValue("@AnType", "");

                foreach (string line in lines)
                {
                    if (Global.NEEDSTOPIMPORT)
                    {
                        break;
                    }
                    lg = line.Replace((char)160, ' ').Split(';');

                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        cmd.Parameters["@AnCode"].Value = lg[1].Trim();
                        cmd.Parameters["@AnType"].Value = lg[2].Trim();
                        cmd.CommandText = "update ANGOOD set AnType=@AnType, state='u' where goodCode=@GoodCode and AnCode=@AnCode";
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            cmd.CommandText = "insert into ANGOOD (GoodCode,AnCode,AnType,state) values (@GoodCode,@AnCode,@AnType,'u')";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    i += 1;
                }
                cmd.CommandText = "delete angood where state=''";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update angood set state=''";
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                cmd.CommandText = "update angood set state=''";
                cmd.ExecuteNonQuery();
                result = false;
            }
            finally
            {
                cn.Close();
            }
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
                db.ExecuteCmd(cmd, "DELETE FROM OWNG WHERE (GoodCode IN (SELECT GoodCode FROM (SELECT DISTINCT GoodId, GoodCode FROM OWNG AS OWNG_1) AS tbd GROUP BY GoodCode HAVING (COUNT(GoodId) > 1)))");
                db.ExecuteCmd(cmd, "DELETE FROM GOOD WHERE (ID NOT IN (SELECT GoodId  FROM OWNG))");
                db.ExecuteCmd(cmd, "DELETE FROM OWNG WHERE (GoodId NOT IN (SELECT ID FROM GOOD))");
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
            if (Global.is_dev) return false;
            bool ret = false;
            DataTable dt = db.GetDbTable("select max(lastupd) from ELG where Entity like 'fullimport%'");
            ret = (cDate.cToDate(dt.Rows[0][0]).Date == DateTime.Today.Date);
            return ret;
        }


        public static string Full_import(string pathfile)
        {

            int i = __fullimport(pathfile);
            if (i > 0)
            {
                eLog log = new eLog();
                log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "import (" + i + ")", "" + DateTime.Now.ToString(), "I"));
                eLog.Save(log, 100000);
            } else
            {
                appError.SaveError("ImportAcync",425," fullimport 0 позиций","timer");
            }
            GoodInfo.__structdt = new System.Collections.Hashtable();
            return " Полное обновление " + i + " позиций.";
        }
        private static int __fullimport_new(string filename, int owner_id = 100000)
        {
            bool testing = true;
            string corrector = "fullimp";
            int ownerId = 100000;

            string logfile = "fullimprep.txt";
            StringBuilder logtxt = new StringBuilder();
            int i = 0;
            string[] lines = ("").Split(',');
            bool result = false;
            eLog log = new eLog();
            try
            {
                lines = File.ReadAllLines(filename);
                logtxt.AppendLine("fullimport:100000 file read: " + lines.Length + " lines " + DateTime.Now.ToString());
                if (lines.Length == 0)
                {
                    log_save(logfile, logtxt.ToString());
                    return 0;
                }

            }
            catch
            {
                logtxt.AppendLine("fullimport:100000 ERROR file read " + DateTime.Now.ToString());

                //log.Stack.Add(new eLog.LogRecord(new sObject(1,"fullimport"),"100000","error read file",""+DateTime.Now.ToString(),"I"));
                log_save(logfile, logtxt.ToString());
                return 0;
            }

            if (db.GetDbTable("select goodcode from fullimp where CONVERT(varchar,lastupd,104)=CONVERT(varchar,getdate(),104)").Rows.Count > 100)
                return 0;
            db.ExecuteCmd("delete from fullimp");
            int w = 0;
            //string err = "";
            string[] lg;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();

            cn.Open();
            try
            {
                //w = db.ExecuteCmd(cmd, "update fullimp set state='o' where ownerid=" + owner_id);

                cmd.Parameters.AddWithValue("@Corrector", corrector);
                cmd.Parameters.AddWithValue("@OwnerId", owner_id);

                cmd.Parameters.AddWithValue("@Name", "");
                cmd.Parameters.AddWithValue("@xname", "");
                cmd.Parameters.AddWithValue("@xtn", "");
                cmd.Parameters.AddWithValue("@xtk", "");
                cmd.Parameters.AddWithValue("@brend", "");
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

                //cmd.CommandText = "savegoodfullimport";
                //cmd.CommandType = CommandType.StoredProcedure;
                int qtyall = 0;
                foreach (string r in lines)
                {
                    if (Global.NEEDSTOPIMPORT)
                    {
                        result = false;
                        break;
                    }
                    lg = r.Replace((char)160, ' ').Split(';');

                    if (lg[0].Trim() == "" || lg.Length < 11 || lg.Length > 16)
                    {
                        logtxt.AppendLine("fullimport:100000 пропуск строки " + i + " " + DateTime.Now.ToString());
                        continue;
                    }
                    try
                    {
                        cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
                        cmd.Parameters["@name"].Value = lg[1].Trim();
                        //cmd.Parameters["@Category"].Value = lg[3].Trim() + "|" + lg[4].Trim();
                        cmd.Parameters["@xtn"].Value = lg[3].Trim();
                        cmd.Parameters["@xtk"].Value = lg[4].Trim();
                        cmd.Parameters["@xname"].Value = __get_xName(lg[1].Trim());
                        cmd.Parameters["@brend"].Value = lg[5].Trim();
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

                        cmd.CommandText = "insert into fullimp (GoodCode,zn,zn_z,pr_spr,pr_b,z2,z3,zt,name,xtn,xtk,brend,article,xname,ens,ed) values (@GoodCode,@zn,@zn_z,@pr_spr,@pr_b,@z2,@z3,@zt,@name,@xtn,@xtk,@brend,@article,@xname,@ens,@ed)";
                        cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        logtxt.AppendLine("fullimport:100000 ERROR GoodCode=" + cmd.Parameters["@GoodCode"].Value + " " + DateTime.Now.ToString());
                    }

                    i += 1;
                    if (testing && i > 5) break;
                    
                }



            }
            catch (Exception ex)
            {
                logtxt.AppendLine("fullimp: ERROR (4) {" + ex.ToString() + "} " + DateTime.Now.ToString());
                result = false;
            }
            finally
            {
                cn.Close();
            }


            if (!Global.NEEDSTOPIMPORT)
            {


                cn.Open();
                dbTransaction tr = new dbTransaction("fullimp");
                SqlCommand cmdtr = tr.beginTransaction();

                try
                {
                    DataTable notImp = db.GetDbTable("select name,xtn,xtk,brend,article,xname,ens,ed from fullimp as imp where goodcode not in (select goodcode from owng)");
                    foreach (DataRow r in notImp.Rows)
                    {
                        cmdtr.CommandText = "insert into GOOD (name,xtn,xtk,brend,article,xname,ens,ed,lc,lcd,State) values ('" + r["name"] + "','" + r["xtn"] + "','" + r["xtk"] + "','" + r["brend"] + "','" + r["article"] + "','" + r["xname"] + "','" + r["ens"] + "','" + r["ed"] + "','" + corrector + "',getdate(),'o')";
                        cmdtr.ExecuteNonQuery();
                    }

                    notImp = db.GetDbTable("select imp.goodcode, isnull((select max(owng.goodid) from owng where owng.goodcode=imp.goodcode),(select max(ID) from good where name=imp.name)) as goodId from fullimp as imp  where goodcode not in (select goodcode from owng where ownerId=" + ownerId + ")");
                    foreach (DataRow r in notImp.Select("goodId>0"))
                    {
                        cmdtr.CommandText = "insert into OWNG (goodId, goodCode,ownerId,lc,lcd,State) values (" + r["goodid"] + ",'" + r["goodcode"] + "'," + ownerId + ",'" + corrector + "',getdate(),'o')";
                        cmdtr.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    logtxt.AppendLine("fullimp: ERROR (2) {" + ex.ToString() + "} " + DateTime.Now.ToString());
                }
                try
                {
                    cmdtr.CommandText = "update GOOD set state='o',lcd=getdate(),lc='" + corrector +
                                      "', good.article=imp.article, good.name=imp.name, good.brend=imp.brend, good.ens=imp.ens, good.xname=imp.xname, good.xtn=imp.xtn, good.xtk=imp.xtk,good.ed=imp.ed from fullimp" +
                                      " as imp where good.id=( select goodid from owng where owng.ownerId=" + ownerId + " and owng.GoodCode=imp.GoodCode)";
                    cmdtr.ExecuteNonQuery();

                    cmdtr.CommandText = "update OWNG set state='o',lcd=getdate(),lc='" + corrector +
                                      "', owng.pr_spr=imp.pr_spr, owng.pr_b=imp.pr_b, owng.zn=imp.zn, owng.zn_z=imp.zn_z, owng.z2=imp.z2, owng.z3=imp.z3, owng.zt=imp.zt from fullimp" +
                                      " as imp where owng.ownerId=" + ownerId + " and owng.GoodCode=imp.GoodCode";
                    cmdtr.ExecuteNonQuery();
                    if (!testing)
                    {
                        cmdtr.CommandText = "delete GOOD where isnull(state,'')<>'o' ";
                        cmdtr.ExecuteNonQuery();

                        cmdtr.CommandText = "delete OWNG where isnull(state,'')<>'o' and OwnerId=" + ownerId;
                        cmdtr.ExecuteNonQuery();
                    }
                    cmdtr.CommandText = "update OWNG set state='', lcd=getdate(),lc='" + corrector + "' where state='o' and OwnerId=" + ownerId;
                    cmdtr.ExecuteNonQuery();
                    cmdtr.CommandText = "delete from fullimp where goodcode in (select goodcode from owng where ownerId=" + ownerId + ")";
                    cmdtr.ExecuteNonQuery();
                    tr.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    logtxt.AppendLine("fullimp: ERROR (3) {" + ex.ToString() + "} " + DateTime.Now.ToString());
                    result = false;
                    tr.Rollback();
                }
                cn.Close();
            }



            if (logtxt.Length > 0)
                log_save(logfile, logtxt.ToString());
            return i;
        }

        public static void __updatetntk()
        {
            List<xTN> tns = xTN_db.GetSourceList();
            var exptn = from t0 in tns
                        where t0.ID == 0
                        orderby t0.Title
                        select t0;

            foreach (xTN tn in exptn)
            {
                xTN_db.Save(tn);
            }

            foreach (xTN tn in xTN_db.GetList())
            {
                List<xTK> tks = xTK_db.GetSourceList(tn.Title);
                var exptk = from t0 in tks
                            where t0.ID == 0
                            orderby t0.Title
                            select t0;
                foreach (xTK tk in exptk)
                    xTK_db.Save(tk);
            }

            db.ExecuteCmd("delete from imgtntk where t='tn' and name not in (select xtn from GOOD where state<>'D')");

            db.ExecuteCmd("delete from imgtntk where t='tk' and name not in (select xtk from GOOD where state<>'D')");
        }

        private static int __fullimport(string filename, int owner_id = 100000)
        {
            string logfile = "fullimprep.txt";
            StringBuilder logtxt = new StringBuilder();
            int i = 0;
            string[] lines = ("").Split(',');
            bool result = false;
            eLog log = new eLog();
            try
            {
                lines = File.ReadAllLines(filename);
                //logtxt.AppendLine("fullimport:100000 file read: " + lines.Length + " lines " + DateTime.Now.ToString());
                if (lines.Length == 0)
                {

                    appError.SaveError("ImportAcync", 650, " fullimport прочитано 0 позиций", "timer");
                    return 0;
                } 
            }
            catch
            {
             //   logtxt.AppendLine("fullimport:100000 ERROR file read " + DateTime.Now.ToString());
                appError.SaveError("ImportAcync", 657, " fullimport ERROR file read", "timer");
                return 0;
            }

            //if (db.GetDbTable("select goodcode from fullimp where CONVERT(varchar,lastupd,104)=CONVERT(varchar,getdate(),104)").Rows.Count > 100)
            //    return 0;
            //db.ExecuteCmd("delete from fullimp");
            int w = 0;
            
            string[] lg;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();

            cn.Open();
            try
            {
                w = db.ExecuteCmd(cmd, "update OWNG set state='o' where ownerid=" + owner_id);

                cmd.Parameters.AddWithValue("@Corrector", "fullimp");
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
                    if (Global.NEEDSTOPIMPORT)
                    {
                        result = false;
                        break;
                    }
                    lg = r.Replace((char)160, ' ').Split(';');

                    if (lg[0].Trim() == "" || lg.Length < 11 || lg.Length > 16)
                    {
                        //logtxt.AppendLine("fullimport:100000 пропуск строки " + i + " " + DateTime.Now.ToString());
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
                        //logtxt.AppendLine("fullimport:100000 ERROR GoodCode=" + cmd.Parameters["@GoodCode"].Value + " " + DateTime.Now.ToString());
                        appError.SaveError(cmd,"ImportAcync",741,"[GoodCode:"+cmd.Parameters["@GoodCode"].Value+"]"+ex.Message,"fullimport");
                    }

                    i += 1;
                }
                if (!Global.NEEDSTOPIMPORT) result = true;
            }
            catch (Exception ex)
            {
                //logtxt.AppendLine("fullimport:100000 ERROR: {" + ex.ToString() + "} " + DateTime.Now.ToString());
                appError.SaveError(cmd, "ImportAcync", 751, ex.Message, "fullimport");
                result = false;
            }
            finally
            {
                cn.Close();
            }
            if (result)
            {
                db.ExecuteCmd("delete from OWNG where goodid not in (select id from good)");
                db.ExecuteCmd("delete from OWNG where ownerid=@OwnerId and State='o'");
                db.ExecuteCmd("delete from GOOD where id not in (select goodid from owng)");
                //try
                //{
                //    clearfile(filename, i.ToString());
                //}
                //catch { }
                log = new eLog();
                log.Stack.Add(new eLog.LogRecord(new sObject(1, "fullimport"), "100000", "import (" + i + ")", "" + DateTime.Now.ToString(), "I"));
                eLog.Save(log, 100000);
            }
            else
            {
                db.ExecuteCmd("update OWNG set state='' where ownerid=@OwnerId and State='o'");
            }
            //if (logtxt.Length > 0)
            //    log_save(logfile, logtxt.ToString());
            return i;
        }

        public static string __get_xName(string full_name)
        {
            string[] sp = full_name.Trim().Split(' ');
            return sp[0];
        }

        public static void log_save(string file_name, string logtxt)
        {
            string fp = webIO.GetAbsolutePath("../exch/" + file_name);
            StreamWriter sw = File.CreateText(fp);
            sw.Write(logtxt);
            sw.Close();

        }


        

    }
}