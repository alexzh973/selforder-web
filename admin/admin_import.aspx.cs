using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using selforderlib;
namespace wstcp
{
    public partial class admin_import : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            check_auth();
            if (!iam.IsSuperAdmin)
            {
                Response.Write("<h2>Доступ запрещен</h2> <p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }
            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "import";
                placeContent.Visible = true;
                //pnlLoginAdmin.Visible = false;
                
                show_buzy();

                dlTypeELG_SelectedIndexChanged(null, null);
                showimportstatus();

                lbMess.Text = "счетов " + Directory.GetFiles(webIO.GetAbsolutePath("../exch"), "*.pdf").Length + " шт.<br/>";
            }

        }
        protected void show_buzy()
        {
            dgDbl2.DataSource = db.GetDbTable("select buzy.ownerid, own.Name, buzy.buzyst, buzy.lcd from buzy inner join own on buzy.ownerid=own.id order by id");
            dgDbl2.DataBind();
        }
        




        protected void btnClearDoubles_Click(object sender, EventArgs e)
        {

        }

        protected void clear_dbls_byname(string name, int bestId)
        {
            if (name.Trim().Length < 3) return;
            db.ExecuteCmd("delete from " + GoodOwner.TDB + " where GoodID<>" + bestId + " and GoodID in (select ID from " + Good.TDB + " where ltrim(rtrim(name))='" + name.Trim() + "' )");

            db.ExecuteCmd("delete from " + Good.TDB + " where ltrim(rtrim(name))='" + name.Trim() + "' and id<>" + bestId);


        }

        protected void btnCheckDoubles_Click(object sender, EventArgs e)
        {
            DataTable dt1 = db.GetDbTable("select qty, name from (SELECT count(ID) as qty, Name FROM GOOD AS GOOD_1 GROUP BY Name) AS derivedtbl_1 WHERE qty > 1 order by qty desc,name");//db.GetDbTable("SELECT COUNT(ID) AS q, Name,(SELECT     TOP (1) g1.ID FROM OWNG AS gi INNER JOIN GOOD AS g1 ON gi.GoodId = g1.ID AND g1.Name = g.Name ORDER BY gi.lcd DESC) AS bestId FROM GOOD AS g GROUP BY Name HAVING (COUNT(ID) > 1) ORDER BY q DESC");
            dgDbl1.DataSource = dt1;
            dgDbl1.DataBind();

        }





        protected void btnStartWatch_Click(object sender, EventArgs e)
        {
            db.ExecuteCmd("delete from imp_100000");
            db.ExecuteCmd("delete from imp_100001");
            db.ExecuteCmd("delete from imp_100002");
            db.ExecuteCmd("delete from imp_100003");
            db.ExecuteCmd("delete from imp_100004");

            Global.StartImport();
        }

        protected void brnStopImport_Click(object sender, EventArgs e)
        {
            Global.StopImport();
        }

        System.Timers.Timer fitmr;

        

        private static int __fullimport(string filename, int owner_id = 100000)
        {
            ImportAsync.Full_import(filename);
            return 1;
            
        }






        protected void testFile(object sender, EventArgs e)
        {

            string filename = System.AppDomain.CurrentDomain.BaseDirectory + @"\" + txNamefiletest.Text;
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



                foreach (string line in lines)
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
                        nr["хНаименование"] = ImportAsync.__get_xName("" + nr["Наименование"]);
                        nr["Брэнд"] = lg[5].Trim();
                        nr["zn"] = lg[6].Trim();
                        nr["zn_z"] = lg[7].Trim();
                        nr["pr_spr"] = cNum.cToDecimal(lg[8]);
                        nr["pr_b"] = cNum.cToDecimal(lg[9]);
                        nr["ens"] = lg[10].Trim();
                        nr["z2"] = (lg.Length > 11) ? lg[11].Trim() : "";
                        nr["z3"] = (lg.Length > 12) ? lg[12].Trim() : "";
                        dt.Rows.Add(nr);
                    }
                }

            }
            catch (Exception ex)
            {
                filename = ex.ToString();
            }

        }

        protected void testDigi(object sender, EventArgs e)
        {
            lbTest.Text = "" + cNum.cToDecimal(txNamefiletest.Text);
        }

        void loadELG()
        {
            DataTable dt = db.GetDbTable("select * from ELG where Entity='" + dlTypeELG.SelectedValue + "' order by LastUpd desc");
            Cache.Insert("ELGDT", dt, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
        }

        protected void dlTypeELG_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadELG();
            DataTable dt = (DataTable)Cache["ELGDT"];
            dgELG.DataSource = dt;
            dgELG.CurrentPageIndex = 0;
            dgELG.DataBind();
        }

        protected void dgELG_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            if (Cache["ELGDT"] == null)
                loadELG();

            if (Cache["ELGDT"] != null)
            {
                DataTable dt = (DataTable)Cache["ELGDT"];
                dgELG.DataSource = dt;
            }

            if (((DataTable)dgELG.DataSource).Rows.Count / dgELG.PageSize >= e.NewPageIndex)
            {
                dgELG.CurrentPageIndex = e.NewPageIndex;
            }
            dgELG.DataBind();
        }

        

        protected void showimportstatus()
        {
            DataTable dt = db.GetDbTable("select ownerId, state ,count(goodId), max(lcd)  from OWNG group by ownerId, state");
            dgImpNow.DataSource = dt;
            dgImpNow.DataBind();


            bool res = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();

                cmd.CommandText = "select max(lcd) from OWNG where ownerId=100000";
                DateTime lcd = cDate.cToDateTime(cmd.ExecuteScalar());
                string flt = File.GetLastWriteTime(webIO.GetAbsolutePath("../exch/1.csv")).ToString();
                lbMessageimport.Text = "db: " + lcd.AddHours(2).ToString() + " | file: " + flt;
            }
            finally
            {
                cn.Close();
            }


        }

        protected void full_import(object sender, EventArgs e)
        {
            bool buzy = ImportAsync.Is_buzy_any();
            if (buzy && btnFullImport.CommandArgument == "fullimp" || !buzy)
            {
                //Global.NEEDSTOPIMPORT = true;
                ImportAsync.Clear_buzy(100000);
                ImportAsync.Clear_buzy(100001);
                ImportAsync.Clear_buzy(100002);
                ImportAsync.Clear_buzy(100003);
                ImportAsync.Clear_buzy(100004);
                __fullimport(webIO.GetAbsolutePath("../exch/full_import.csv"), 100000);
                btnFullImport.CommandArgument = "";
                Global.NEEDSTOPIMPORT = false;
            }
            else
            {
                lbMessageimport.Text = "сейчас производится импорт";
                showimportstatus();
                btnFullImport.CommandArgument = "fullimp";
            }
        }

        protected void loadPictures_Click(object sender, EventArgs e)
        {



            DataTable dt = db.GetDbTable("select id, ens,img from good where isnull(ens,'')<>'' and isnull(img,'')=''");
            if (cNum.cToInt(txQtyImgs.Text) == 0)
            {
                txQtyImgs.ToolTip = "кол-во " + dt.Rows.Count;
            }
            else
            {


                dt = db.GetDbTable("select top " + cNum.cToInt(txQtyImgs.Text) + " id, ens,img from good where isnull(ens,'')<>'' and isnull(img,'')=''");

                foreach (DataRow r in dt.Rows)
                {

                    string img = "../media/gimg/" + r["ens"] + ".jpg";
                    if ("" + r["ens"] != "" && !webIO.CheckExistFile(img))
                        if (wstcp.special.ensinfo.SaveImg(r["ens"].ToString(), @"../media/gimg", true)) { db.ExecuteCmd("update good set img='" + img + "' where id=" + r["id"]); }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            
            
            foreach(string file in Directory.GetFiles(webIO.GetAbsolutePath("../media/gimg")))
            {
                db.ExecuteCmd("update good set img='../media/gimg/" + Path.GetFileName(file) + "' where ens='" + Path.GetFileName(file).Replace(Path.GetExtension(file), "") + "'");
            }
        }

        protected void btnSetAllViewArticle_Click(object sender, EventArgs e)
        {
            foreach (DataRow r in db.GetDbTable("select id, authorid from ord where id not in (select eid from " + ViewArticle.TDB + " where metatdb='ORD')").Rows)
            {
                db.ExecuteCmd("insert into " + ViewArticle.TDB + " (userid,metatdb,eid,mark,seedate,lcd)values(" + r["AuthorId"] + ",'ORD'," + r["id"] + ",'',getdate(),getdate())");
            }
        }

        protected void lbtnImpTas_Click(object sender, EventArgs e)
        {
            DataTable dt = db.GetDbTable("select distinct emailtas, nametas from TTT");
            int id;
            foreach (DataRow r in dt.Rows)
            {
                id = pUser.FindByField("email", "" + r["emailtas"]);
                if (id>0)
                    continue;
                pUser u = new pUser(0,iam);
                u.Email = "" + r["emailtas"];
                u.Name = "" + r["nametas"];
                u.ParentID = 100004;
                pUser.Save(u);
            }
        }

        protected void btnImportAnalog_Click(object sender, EventArgs e)
        {
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

        protected void btnUpdateInvoices_Click(object sender, EventArgs e)
        {
            lbMess.Text = "счетов " + Directory.GetFiles(webIO.GetAbsolutePath("../exch"), "*.pdf").Length + " шт.<br/>";
            update_invoices(lbMess);
            lbMess.Text += "счетов " + Directory.GetFiles(webIO.GetAbsolutePath("../exch"), "*.pdf").Length + " шт.<br/>";
        }
        private static void update_invoices(Label lbMess)
        {
            int i = 0;
            foreach (string filename in Directory.GetFiles(webIO.GetAbsolutePath("../exch"), "*.pdf"))
            {
                update_invoice(Path.GetFileName(filename),lbMess);

                if (i == 1001) break;
                else i += 1;
            }
        }

        private static void update_invoice(string filename, Label lbMess)
        {
            int id = cNum.cToInt(filename.Replace(".pdf", ""));
            byte[] bufinvoice;
            if (webIO.CheckExistFile("../exch/" + filename))
            {
                //lbMess.Text += "нашли " + filename + "<br/>";
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
                    lbMess.Text += "прочитали " + filename + "<br/>";

                    cmd.Parameters["@src"].Value = bufinvoice;
                    cmd.CommandText = "update " + Order.TDB + " set invoicesrc=@src where id=" + id;
                    cmd.ExecuteNonQuery();

                }
                catch(Exception ex)
                {
                    upd = false;
                    //lbMess.Text += "Ошибка 442 " + filename + " ("+ex.ToString()+")<br/>";
                }
                finally
                {
                    cn.Close();
                }
                if (upd)
                {
                    File.Delete(webIO.GetAbsolutePath("../exch/" + filename));
                    //lbMess.Text += "Удален " + filename + "<br/>";
                }
            }
        }

        protected void btnUpdateTNTK_Click(object sender, EventArgs e)
        {
            ImportAsync.__updatetntk();
        }
       
    }
}


