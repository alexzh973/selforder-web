using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using System.Collections;
using System.IO;
using System.Drawing;


namespace wstcp
{
    public partial class admin_usr : p_p
    {
        private DataTable _srcdt;
        private DataView _dv;

        protected int ParentID
        {
            get { return cNum.cToInt(ViewState["ParentID"]); }
            set { ViewState["ParentID"] = value.ToString(); }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {


        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // ----------------------------------------------------
            check_auth();
            if (!iam.IsSuperAdmin)
            {
                Response.Write("<h2>Доступ запрещен</h2> <p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }

            if (iam.IsUserAdmin || iam.IsSuperAdmin)
                initCommandPanel();
            else
            {
                Response.Write("Недостаточно прав для доступа");
                Response.End();
            }

            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "users";
                ((mainpage)this.Master).VisibleLeftPanel = true;
                ParentID = cNum.cToInt(Request.QueryString["pid"]);
                eID = cNum.cToInt(Request.QueryString["id"]);
            }

            Alphabet1.initAlphabet();




            if (!IsPostBack)
            {
                if (_ACT_ == "edit" || _ACT_ == "new")
                {
                    eID = cNum.cToInt(Request.QueryString["id"]);
                    open_profile();
                    
                }
                else
                {
                    show_list();
                }

            }
            else if (ParentID != cNum.cToInt(curpid.Value))
            {
                ParentID = cNum.cToInt(curpid.Value);

                show_list();
            }
            else if (Alphabet1.Changed)
            {
                txSearch.Text = "";
                dgList.CurrentPageIndex = 0;
                ParentID = 0;
                curpid.Value = "";
                show_list();
            }
        }

        void show_parent_path()
        {
            if (ParentID <= 0)
            {
                lbParent.Text = "";
            }
            else
            {
                lbParent.Text = "<a href='admin_usr.aspx?pid=0'>наверх..</a>";
                foreach (int id in pUser.PathToCurrent( ParentID, iam))
                {
                    pUser pu = new pUser(id, iam);
                    lbParent.Text += "&nbsp;/&nbsp;<a href='admin_usr.aspx?pid=" + id + "' class='bold'>" + pu.Name + "</a>";
                }
                pUser par = new pUser(ParentID, iam);
                lbParent.Text += "&nbsp;/&nbsp;<a href='admin_usr.aspx?pid=" + ParentID + "' class='bold'>" + par.Name + "</a>";
            }
        }








        void show_list()
        {
            MultiView1.SetActiveView(vList);
            pnlFilter.Visible = true;
            if (ParentID > 0)
            {
                curpid.Value = ParentID.ToString();
                show_parent_path();
            }
            else
                lbParent.Text = "";


            string where = "isnull(parentid,0)=" + ParentID;
            switch (dlLoginEnabled.SelectedValue)
            {
                case "Y": where += ((where.Length > 0) ? " and " : "") + "LoginEnabled='Y'"; break;
                case "N": where += ((where.Length > 0) ? " and " : "") + "LoginEnabled='N'"; break;
                default: break;
            }
            switch (dlState.SelectedValue)
            {
                case "Y": where += ((where.Length > 0) ? " and " : "") + "isnull(State,'')<>'D'"; break;
                case "N": where += ((where.Length > 0) ? " and " : "") + "State='D'"; break;
                default: break;
            }



            _srcdt = pUserInfo.GetTable(iam, where);//, ParentID, ((Alphabet1.Letter != "" || txSearch.Text != "") ? false : chUseGroups.Checked), iam);
            _dv = _srcdt.DefaultView;
            _dv.Sort = "IsFolder desc, Name";
            string filter = "";
            if (Alphabet1.Letter != "")
                filter += ((filter.Length > 0) ? " and " : "") + string.Format("email like '{0}%' or Name like '{0}%'", Alphabet1.Letter);
            string search = (txSearch.Text.Trim() != "часть фио/e-mail/телефон/номер авто") ? txSearch.Text : "";
            if (search.Length > 0 && search.Length < 3)
                filter += ((filter != "") ? " and " : " ") + string.Format(" (Name like '{0}%' or Descr like '{0}%' or [email] like '{0}%' )", search);
            else if (search.Length > 0 && search.Length >= 3)
                filter += ((filter != "") ? " and " : " ") + string.Format(" (Name like '%{0}%' or Descr like '%{0}%' or [email] like '%{0}%'   )", search); ;

            _dv.RowFilter = filter;
            dgList.AllowPaging = !(Alphabet1.Letter != "" || search.Length > 0);

            dgList.DataSource = _dv;
            dgList.DataBind();

        }


        protected void bntNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin_usr.aspx?act=new&id=0&pid=" + ParentID);
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin_usr.aspx?act=edit&id=" + curid.Value);
        }
        protected void btnCopy_Click(object sender, EventArgs e)
        {
            Response.Redirect("admin_usr.aspx?act=copy&id=" + curid.Value);

        }




        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            //string txt2 = "";
            //string txt3 = "";

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);
                bool isFolder = ("" + r["IsFolder"] == "Y");

                e.Item.Attributes.Add("id", "object_" + id);
                if ("" + r["LoginEnabled"] == "N") e.Item.ForeColor = cColor.Color("silver");
                e.Item.Attributes.Add("onclick", "thisRow(this.id);setCurid('" + id + "');");
                if (isFolder)
                {
                    e.Item.Cells[1].Text = (ParentID==id)?"<img src='../SIMG/16/folder.png'/>":"<img src='../SIMG/16/folder.png'/>";
                    e.Item.Attributes.Add("class", "bold");
                    e.Item.Cells[2].Text = "<div ><a href='admin_usr.aspx?pid=" + id + "' >" + webInfo.select_search(txSearch.Text, "" + r["Name"]) + "</a></div>";
                    //
                }
                else
                {
                    if (File.Exists(HelpersPath.PersonsAvatarFisicalPath + "/" + id + "s.png"))
                    {
                        e.Item.Cells[1].Text = "<img src='" + HelpersPath.PersonsAvatarVirtualPath + "/" + id + "s.png?d=" + DateTime.Now.Ticks + "'>";
                    }
                    e.Item.Attributes.Add("class", "rowItem");

                    e.Item.Cells[2].Text = "<div ><a href='javascript:return false;' title='" + r["Name"] + "' onclick=\"openflywin('../person/card.aspx?id=" + r["ID"] + "',300,400,'"+r["Name"]+"')\">" + webInfo.select_search(txSearch.Text, "" + r["Name"]) + "</a></div>";
                    e.Item.Cells[2].Text += "<div  >" + webInfo.select_search(txSearch.Text, "" + r["email"]) + "</div>";
                    e.Item.Cells[3].Text = ""+r["SubjectName"];
                    //if (cNum.cToInt() > 0)
                    //{
                        
                    //}
                    //if (!cStr.IsEmpty(r["Email"]))
                    //    e.Item.Cells[3].Text += "<div><a class='bold' href=\"#\" onclick=\"openflywin('../COMMON/email.aspx?to=" + r["Email"] + "',630,500,'e-mail сообщение');\"><img src='../img/email_16.png' title='написать письмо'></a> " + webInfo.select_search(txSearch.Text, "" + r["Email"]) + "</div>";

                }
                if (iam.IsSuperAdmin  || iam.ID == id)
                {
                    e.Item.Cells[4].Text = "<a href='admin_usr.aspx?act=edit&id=" + id + "'>...</a>";
                    e.Item.Cells[5].Text = get_last_activity(""+r["Name"]);
                }
                else
                {
                }
                if (_RGW_ == "fly")
                    e.Item.Cells[4].Text = "";
            }

        }
        string get_last_activity(string lc)
        {
            DataTable dt = db.GetDbTable("select max(lcd) from ORD where lc='" + lc + "'");
            return (dt.Rows.Count > 0) ? ""+cDate.cToDateTime(dt.Rows[0][0]) : "-";
        }
        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgList.CurrentPageIndex = e.NewPageIndex;
            eID = 0;
            curid.Value = "";
            show_list();
            dgList.DataSource = _dv;
            dgList.DataBind();
        }
        protected void dgList_SelectedIndexChanged(object sender, EventArgs e)
        {
           

        }

        protected void dlLoginEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            dgList.CurrentPageIndex = 0;
            show_list();
            dgList.DataSource = _dv;
            dgList.DataBind();

        }
        protected void dlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            dgList.CurrentPageIndex = 0;
            show_list();
            dgList.DataSource = _dv;
            dgList.DataBind();

        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            dgList.CurrentPageIndex = 0;
            show_list();
            dgList.DataSource = _dv;
            dgList.DataBind();
        }
        protected void dgList_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                //case "Select":
                //    show_detail(cNum.cToInt(e.Item.Cells[0].Text));
                //    dgList.SelectedIndex = -1;
                //    break;
                case "edit":
                    Response.Redirect("admin_usr.aspx?act=edit&id=" + e.Item.Cells[0].Text);
                    break;
                case "change":
                    //show_zamest(cNum.cToInt(e.Item.Cells[0].Text));
                    dgList.SelectedIndex = -1;
                    break;
                default:
                    break;
            }
        }
        protected void dgList_SortCommand(object source, DataGridSortCommandEventArgs e)
        {

        }

        protected void txSearch_PreRender(object sender, EventArgs e)
        {
            //txSearch.Attributes.Add("onmousedown", "search_blank('"+txSearch.ClientID+"',1)");
            //txSearch.Attributes.Add("onmouseout", "search_blank('" + txSearch.ClientID + "',0)");
        }


        void initCommandPanel()
        {
            CommandPanel4List1.Event_NEW = new EventHandler(btnEdit_Click);
            //CommandPanel4List1.Event_EDIT = new EventHandler(btnEdit_Click);
            //CommandPanel4List1.Event_COPY = new EventHandler(btnCopy_Click);
            CommandPanel4List1.ButtonVisible_EDIT =
                CommandPanel4List1.ButtonVisible_COPY =
                CommandPanel4List1.ButtonVisible_DELETE = false;
            
            CommandPanel4Profile1.Event_Save = new EventHandler(save_profile);
            CommandPanel4Profile1.Event_SaveAndClose = new EventHandler(save_exit_profile);
            CommandPanel4Profile1.Event_Cancel = new EventHandler(cancel_profile);
        }

        pUser u;
        void open_profile()
        {
            MultiView1.SetActiveView(vProfile);
            pnlFilter.Visible = false;
            u = new pUser(eID, iam);

            if (u.ID == 0)
                u.ParentID = ParentID;
            loaduserfolders(iam, dlParent, 0, u.ParentID);

            txID.Text = u.ID.ToString();
            txEmail.Text = u.Email;
            txDescr.Text = u.Descr;
            txPhones.Text = u.Phones;
            txName.Text = u.Name;
            chIsFolder.Checked = u.IsFolder;
            chLoginEnable.Checked = u.LoginEnabled;

            blockPassword.Visible = (iam.ID == eID || iam.IsSuperAdmin);
            place4admin.Visible = (iam.IsSuperAdmin || iam.IsUserAdmin);
            placeNotFolder.Visible = !chIsFolder.Checked;

            lbPsw.Text = (blockPassword.Visible) ? u.Psw : "";
            
            UserRights.Items.FindByValue("SADM").Selected = (u.RightsString.IndexOf("SADM") > -1);
            UserRights.Items.FindByValue("UADM").Selected = (u.RightsString.IndexOf("UADM") > -1);
            txType.Text = u.RightsString;
            divPreview.Text = "<div><img src=\"../simg/phu/" + u.ID + "b.png?d=" + DateTime.Now.Ticks + "\" /></div>";
            if (!u.IsFolder)
            {
                DataTable dt = db.GetDbTable("select id, name from " + Subject.TDB + " where Isfolder<>'Y'");
                dlSubject.Items.Clear();
                dlSubject.Items.Add(new ListItem("" , ""));
                foreach(DataRow r in dt.Rows){
                    dlSubject.Items.Add(new ListItem(""+r["name"], ""+r["id"]));
                }
                try
                {
                    dlSubject.Items.FindByValue("" + u.SubjectID).Selected = true;
                }
                catch { }
                
            
            }
        }

       
       
        public static void loaduserfolders(IAM iam, DropDownList dd_list,  int parent_id = 0, int default_id = 0, bool load_empty = true)
        {
            dd_list.Items.Clear();
            if (load_empty) dd_list.Items.Add(new ListItem("   -   ", ""));
            DataTable dt = pUserInfo.GetTable(iam, "isfolder='Y' and state<>'D' ");
            loaduserfolders_byparent(dd_list, dt,  parent_id, iam, 0);

            if (default_id > 0)
            {
                try
                {
                    dd_list.Items.FindByValue("" + default_id).Selected = true;
                }
                catch
                {
                    try { dd_list.Items[0].Selected = true; }
                    catch { }
                }
            }

        }
        private static void loaduserfolders_byparent(DropDownList dd_list, DataTable dt,  int parent_id, IAM iam, int level)
        {
            ListItem li;
            string style = "";
            foreach (DataRow r in dt.Select("isnull(ParentID,0)=" + parent_id, "Name"))
            {

                li = new ListItem("" + r["Name"], "" + r["ID"]);
                style = "padding-left:" + (level * 8) + "px;";

                if ("" + r["IsFolder"] == "Y") style += "font-weight: bold;";
                li.Attributes.Add("style", style);
                dd_list.Items.Add(li);
                if (cNum.cToInt(r["ID"]) > 0)
                    loaduserfolders_byparent(dd_list, dt,  cNum.cToInt(r["ID"]), iam, level + 1);
            }
        }

        protected void save_profile(object sender, EventArgs e)
        {
            save_user();
        }

        protected void save_exit_profile(object sender, EventArgs e)
        {
            if (save_user())
                Response.Redirect("admin_usr.aspx?pid=" + dlParent.SelectedValue);
        }
        protected void cancel_profile(object sender, EventArgs e)
        {
            Response.Redirect("admin_usr.aspx?pid=" + ParentID);
        }


        bool save_user()
        {
            if (cNum.cToInt(txID.Text) == 0 && pUserInfo.Exist(txEmail.Text))
            {
                lbMessage.Text = string.Format("email {0} уже существует в системе. Дублирование email недопустимо", txEmail.Text);
                return false;
            }

            pUser u = new pUser(cNum.cToInt(txID.Text), iam);
            u.IsFolder = chIsFolder.Checked;
            u.Email = txEmail.Text;
            u.Name = txName.Text;
            u.Descr = txDescr.Text;
            u.Phones = txPhones.Text;
            u.ParentID = cNum.cToInt(dlParent.SelectedValue);
            u.SubjectID = cNum.cToInt(dlSubject.SelectedValue);
            string rights = "";
            if (UserRights.Items.FindByValue("SADM").Selected) cStr.AddUnique(ref rights, "SADM", ' ');
            if (UserRights.Items.FindByValue("UADM").Selected) cStr.AddUnique(ref rights, "UADM", ' ');
            
            u.RightsString = rights;
                
            
            
            bool access_changed = (!u.LoginEnabled && chLoginEnable.Checked);
            u.LoginEnabled = chLoginEnable.Checked;
           
            bool res = pUser.Save(u, iam);
            if (res && hdPhoto.Value != "")
            {
                
                
                    try
                    {
                        Bitmap bmp_s = new Bitmap(System.AppDomain.CurrentDomain.BaseDirectory + @"simg\phu\" + hdPhoto.Value + "s.png");
                        Bitmap bmp_b = new Bitmap(System.AppDomain.CurrentDomain.BaseDirectory + @"simg\phu\" + hdPhoto.Value + "b.png");
                        bmp_b.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + u.ID + "b.png", System.Drawing.Imaging.ImageFormat.Png);
                        bmp_s.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + u.ID + "s.png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                       
                    }
                
            }
            if (res && blockPassword.Visible && (txOldPass.Text == u.Psw || iam.IsSuperAdmin) && txNewPass.Text != u.Psw && txNewPass.Text.Length >= 3)
            {
                pUser.SetNewPassword(u, txOldPass.Text, txNewPass.Text, iam);
            }

            

            if (res && access_changed)
            {

                EmailMessage email = new EmailMessage(
                    CurrentCfg.EmailPortal,
                    u.Email,
                    HelpersPath.RootUrl + " :: Уведомление о доступе к сайту",
                    string.Format("<p>Ваш запрос на регистрацию на сайт {0} удовлетворен.</p><p>логин для входа {1}</p><p>пароль {2}</p><p>ссылка {0}</p>", HelpersPath.RootUrl, u.Email, u.Psw));
                email.Send();

            }
            return res;
        }


        protected void chIsFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            placeNotFolder.Visible = !chIsFolder.Checked;
        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (hdPhoto.Value != "")
            {
                try { System.IO.File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + hdPhoto.Value + "b.png"); }
                catch { }
                try { System.IO.File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + hdPhoto.Value + "s.png"); }
                catch { }
            }
            else
            {
                try { System.IO.File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + txID.Text + "b.png", System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/delete_" + txID.Text + "b.png"); }
                catch { }
                try { System.IO.File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + txID.Text + "s.png", System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/delete_" + txID.Text + "s.png"); }
                catch { }
            }
            hdPhoto.Value = "";

            divPreview.Text = "<div></div>";
        }
        protected void btnLoadPic_Click(object sender, EventArgs e)
        {
            string filetype = FileUpload1.PostedFile.ContentType;
            if (filetype.IndexOf("image") < 0 || FileUpload1.PostedFile.ContentLength > 10000000) return;
            System.Drawing.Image img = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
            int h = img.Height;
            int w = img.Width;
            int size_s = 64;
            int size_b = 250;
            decimal p_s, p_b;

            p_s = ((decimal)size_s) / ((h > w) ? h : w);
            p_b = (((h > w) ? h : w) > size_b) ? p_s * (size_b / size_s) : 1;

            Bitmap bmp_s = new Bitmap(img, cNum.cToInt(img.Width * p_s), cNum.cToInt(img.Height * p_s));
            Bitmap bmp_b = new Bitmap(img, cNum.cToInt(img.Width * p_b), cNum.cToInt(img.Height * p_b));
            bmp_b.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + "tmp_usr_" + iam.ID + "b.png", System.Drawing.Imaging.ImageFormat.Png);
            bmp_s.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"simg/phu/" + "tmp_usr_" + iam.ID + "s.png", System.Drawing.Imaging.ImageFormat.Png);

            img = null;
            hdPhoto.Value = "tmp_usr_" + iam.ID;
            divPreview.Text = "<div><img  src='../simg/phu/" + hdPhoto.Value + "b.png?d=" + DateTime.Now.Ticks + "'></div>";

        }











    }
}