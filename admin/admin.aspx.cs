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
    public partial class admin : p_p
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
                ((adminmaster)this.Master).SelectedMenu = "setup";
                placeContent.Visible = true;
                //pnlLoginAdmin.Visible = false;
                load_owners_dg();
                load_whs();
                show_buzy();

                dlTypeELG_SelectedIndexChanged(null, null);
                showimportstatus();
            }

        }
        protected void show_buzy()
        {
            dgDbl2.DataSource = db.GetDbTable("select buzy.ownerid, own.Name, buzy.buzyst, buzy.lcd from buzy inner join own on buzy.ownerid=own.id order by id");
            dgDbl2.DataBind();
        }
        protected void load_owners_dg()
        {
            DataTable dt = Owner.GetTable();
            dgOwners.DataSource = dt;
            dgOwners.DataBind();

            dlOwners.Items.Clear();
            foreach (DataRow r in dt.Select("", "Name"))
            {
                dlOwners.Items.Add(new ListItem(r["Name"].ToString(), r["ID"].ToString()));
            }

        }
        protected void load_whs()
        {
            DataTable dt = Warehouse.GetTable(cNum.cToInt(dlOwners.SelectedValue));
            dgWarehauses.DataSource = dt;
            dgWarehauses.DataBind();
        }


        protected void dgOwners_EditCommand(object source, DataGridCommandEventArgs e)
        {
            dgOwners.EditItemIndex = e.Item.ItemIndex;
            load_owners_dg();


        }

        protected void dgOwners_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            Owner rec = new Owner(cNum.cToInt(e.Item.Cells[0].Text));
            rec.Name = ((TextBox)e.Item.Cells[1].Controls[0]).Text;
            rec.IPAdrs = ((TextBox)e.Item.Cells[2].Controls[0]).Text;

            Owner.Save(rec, "admin");
            dgOwners.EditItemIndex = -1;
            Global.reload_adrs();
            load_owners_dg();
        }

        protected void dgOwners_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "edit")
            {

                //dgOwners.EditItemIndex = e.Item.ItemIndex;
                //load_owners_dg();
            }
            else if (e.CommandName.ToLower() != "edit")
            {

            }
        }


        protected void dgOwners_CancelCommand(object source, DataGridCommandEventArgs e)
        {

            dgOwners.EditItemIndex = -1;
            load_owners_dg();

        }

        protected void dlOwners_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_whs();
        }

        protected void dgWarehauses_EditCommand(object source, DataGridCommandEventArgs e)
        {
            dgWarehauses.EditItemIndex = e.Item.ItemIndex;
            load_whs();
        }

        protected void dgWarehauses_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            Warehouse rec = new Warehouse(cNum.cToInt(e.Item.Cells[0].Text));
            rec.OwnerID = cNum.cToInt(dlOwners.SelectedValue);
            rec.Name = ((TextBox)e.Item.Cells[1].Controls[0]).Text;
            rec.ContactInfo = ((TextBox)e.Item.Cells[2].Controls[0]).Text;
            rec.OwnerCodes = ((TextBox)e.Item.Cells[3].Controls[0]).Text;
            Warehouse.Save(rec, "admin");

            dgWarehauses.EditItemIndex = -1;
            load_whs();
        }

        protected void dgWarehauses_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            dgWarehauses.EditItemIndex = -1;
            load_whs();
        }

        protected void btnNewOwner_Click(object sender, EventArgs e)
        {
            DataTable dt = Owner.GetTable();
            DataRow r = dt.NewRow();
            dt.Rows.InsertAt(r, 0);
            dgOwners.EditItemIndex = 0;
            dgOwners.DataSource = dt;
            dgOwners.DataBind();
        }

        protected void btnNewWH_Click(object sender, EventArgs e)
        {
            DataTable dt = Warehouse.GetTable(cNum.cToInt(dlOwners.SelectedValue));
            DataRow r = dt.NewRow();
            dt.Rows.InsertAt(r, 0);
            dgWarehauses.EditItemIndex = 0;
            dgWarehauses.DataSource = dt;
            dgWarehauses.DataBind();
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

            //DataTable dt2 = db.GetDbTable("select count(*) as q, GoodCode,OwnerId  from OWNG group by OwnerId, GoodCode order by q desc");
            //dgDbl2.DataSource = dt2;
            //dgDbl2.DataBind();
            //DataTable dt3 = db.GetDbTable("select count(*) as q, GoodId,WhId  from GINCASH group by GoodId,WhId order by q desc");
            //dgDbl3.DataSource = dt3;
            //dgDbl3.DataBind();
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
            //string logfile = "fullimprep.txt";
            //StringBuilder logtxt = new StringBuilder();
            //int i = 0;
            //string[] lines = ("").Split(',');
            //bool result = false;
            //eLog log = new eLog();
            //try
            //{
            //    lines = File.ReadAllLines(filename);
            //    logtxt.AppendLine("fullimport:100000 file read: " + lines.Length + " lines " + DateTime.Now.ToString());
            //    if (lines.Length == 0)
            //    {
            //        ImportAsync.log_save(logfile, logtxt.ToString());
            //        return 0;
            //    }

            //}
            //catch
            //{
            //    logtxt.AppendLine("fullimport:100000 ERROR file read " + DateTime.Now.ToString());


            //    ImportAsync.log_save(logfile, logtxt.ToString());
            //    return 0;
            //}
            //int w = 0;

            //string[] lg;

            //SqlConnection cn = new SqlConnection(db.DefaultCnString);
            //SqlCommand cmd = cn.CreateCommand();

            //cn.Open();
            //try
            //{
            //    w = db.ExecuteCmd(cmd, "update OWNG set state='o' where ownerid=" + owner_id);

            //    cmd.Parameters.AddWithValue("@Corrector", "import");
            //    cmd.Parameters.AddWithValue("@OwnerId", owner_id);

            //    cmd.Parameters.AddWithValue("@Name", "");
            //    cmd.Parameters.AddWithValue("@Category", "");
            //    cmd.Parameters.AddWithValue("@xName", "");
            //    cmd.Parameters.AddWithValue("@TN", "");
            //    cmd.Parameters.AddWithValue("@TK", "");
            //    cmd.Parameters.AddWithValue("@Brend", "");
            //    cmd.Parameters.AddWithValue("@GoodCode", "");
            //    cmd.Parameters.AddWithValue("@zn", "");
            //    cmd.Parameters.AddWithValue("@zn_z", "");
            //    cmd.Parameters.AddWithValue("@z2", "");
            //    cmd.Parameters.AddWithValue("@z3", "");
            //    cmd.Parameters.AddWithValue("@zt", "");
            //    cmd.Parameters.AddWithValue("@pr_spr", (decimal)0);
            //    cmd.Parameters.AddWithValue("@pr_b", (decimal)0);

            //    cmd.Parameters.AddWithValue("@ens", "");
            //    cmd.Parameters.AddWithValue("@State", "");
            //    cmd.Parameters.AddWithValue("@article", "");
            //    cmd.Parameters.AddWithValue("@ed", "");

            //    cmd.CommandText = "savegoodfullimport";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    foreach (string r in lines)
            //    {
            //        lg = r.Replace((char)160, ' ').Split(';');

            //        if (lg[0].Trim() == "" || lg.Length < 11 || lg.Length > 16)
            //        {
            //            logtxt.AppendLine("fullimport:100000 пропуск строки " + i + " " + DateTime.Now.ToString());

            //            continue;
            //        }
            //        try
            //        {
            //            cmd.Parameters["@GoodCode"].Value = lg[0].Trim();
            //            cmd.Parameters["@Name"].Value = lg[1].Trim();
            //            cmd.Parameters["@Category"].Value = lg[3].Trim() + "|" + lg[4].Trim();
            //            cmd.Parameters["@TN"].Value = lg[3].Trim();
            //            cmd.Parameters["@TK"].Value = lg[4].Trim();
            //            cmd.Parameters["@xName"].Value = get_xName(lg[1].Trim());
            //            cmd.Parameters["@Brend"].Value = lg[5].Trim();
            //            cmd.Parameters["@zn"].Value = lg[6].Trim();
            //            cmd.Parameters["@zn_z"].Value = lg[7].Trim();
            //            cmd.Parameters["@pr_spr"].Value = cNum.cToDecimal(lg[8]);
            //            cmd.Parameters["@pr_b"].Value = cNum.cToDecimal(lg[9]);
            //            cmd.Parameters["@ens"].Value = lg[10].Trim();
            //            cmd.Parameters["@article"].Value = lg[11].Trim();
            //            cmd.Parameters["@ed"].Value = lg[12].Trim();
            //            cmd.Parameters["@z2"].Value = lg[13].Trim();
            //            cmd.Parameters["@z3"].Value = lg[14].Trim();
            //            cmd.Parameters["@zt"].Value = (lg[6].Trim().ToLower() == "z" && lg.Length > 15) ? lg[15].Trim() : "";


            //            cmd.ExecuteNonQuery();
            //        }
            //        catch (Exception ex)
            //        {
            //            logtxt.AppendLine("fullimport:100000 ERROR GoodCode=" + cmd.Parameters["@GoodCode"].Value + " " + DateTime.Now.ToString());

            //        }

            //        i += 1;
            //    }
            //    result = true;
            //}
            //catch (Exception ex)
            //{
            //    logtxt.AppendLine("fullimport:100000 ERROR: {" + ex.ToString() + "} " + DateTime.Now.ToString());
            //}
            //finally
            //{
            //    cn.Close();
            //}
            //if (result)
            //{
            //    w = db.ExecuteCmd(cmd, "delete from OWNG where goodid not in (select id from good)");
            //    w = db.ExecuteCmd(cmd, "delete from OWNG where ownerid=@OwnerId and State='o'");
            //    try
            //    {
            //        File.Delete(filename);
            //    }
            //    catch { }
            //}
            //else
            //{
            //    db.ExecuteCmd(cmd, "update OWNG set state='' where ownerid=@OwnerId and State='o'");
            //}
            //if (logtxt.Length > 0)
            //    ImportAsync.log_save(logfile, logtxt.ToString());
            //return i;
        }





        //private static DataTable load_file(string filename)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("КодНоменклатуры");
        //    dt.Columns.Add("Наименование");

        //    dt.Columns.Add("ТН");
        //    dt.Columns.Add("ТК");
        //    dt.Columns.Add("хНаименование");
        //    dt.Columns.Add("Брэнд");
        //    dt.Columns.Add("zn");
        //    dt.Columns.Add("zn_z");
        //    dt.Columns.Add("pr_spr");
        //    dt.Columns.Add("pr_b");
        //    dt.Columns.Add("ens");


        //    try
        //    {
        //        using (StreamReader sr = File.OpenText(filename))
        //        {
        //            string line = null;
        //            string[] lg;
        //            DataRow nr;

        //            do
        //            {
        //                line = sr.ReadLine();
        //                if (line == null) break;
        //                lg = line.Replace((char)160, ' ').Split(';');
        //                if (lg[0].Trim() == "" )
        //                    continue;                        
        //                nr = dt.NewRow();
        //                nr["КодНоменклатуры"] = lg[0].Trim();
        //                nr["Наименование"] = lg[1].Trim();
        //                nr["ТН"] = lg[3].Trim();
        //                nr["ТК"] = lg[4].Trim();
        //                nr["хНаименование"] = get_xName("" + nr["Наименование"]);
        //                nr["Брэнд"] = lg[5].Trim();
        //                nr["zn"] = lg[6].Trim();
        //                nr["zn_z"] = lg[7].Trim();
        //                nr["pr_spr"] = cNum.cToDecimal(lg[8]);
        //                nr["pr_b"] = cNum.cToDecimal(lg[9]);
        //                nr["ens"] = lg[10].Trim();
        //                cmd.Parameters["@article"].Value = lg[11].Trim();
        //                cmd.Parameters["@ed"].Value = lg[12].Trim();
        //                dt.Rows.Add(nr);

        //            } while (line != null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return dt;
        //}

        static string get_xName(string full_name)
        {
            string[] sp = full_name.Trim().Split(' ');
            return sp[0];
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

        protected void btnMail_Click(object sender, EventArgs e)
        {
            SmtpClient client = new SmtpClient(txMailServer.Text);
            try
            {
                using (System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage())
                {
                    m.To.Add(txMailTo.Text);
                    m.From = new MailAddress(txMailFrom.Text);
                    m.Subject = txMailSubj.Text;
                    m.Body = txMailBody.Text;
                    m.BodyEncoding = Encoding.UTF8;
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;
                    client.Credentials = new NetworkCredential("info@santechportal.ru", "wNyh96*5");
                    client.Send(m);
                }
                lbMailMess.Text = "Ok";
            }
            catch (Exception ex)
            {
                lbMailMess.Text = ex.ToString();
            }
            finally
            {

                client = null;
            }
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
    }
}


