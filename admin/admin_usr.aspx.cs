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
using selforderlib;


namespace wstcp
{
    public partial class admin_usr : p_p
    {
        private DataTable _srcdt;
        private DataView _dv;

        protected string _RGW_
        {
            get { if (!IsPostBack && Request["rgw"] != null) ViewState["RGW"] = Request["rgw"]; return "" + ViewState["RGW"]; }
            set { ViewState["RGW"] = value; }
        }


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
            if (!(iam.IsSuperAdmin || iam.IsAdmin))
            {
                Response.Write("<h2>Доступ запрещен</h2> <p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }

            
            initCommandPanel();
            

            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "users";
                
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
                    if (cNum.cToInt(Request["rem"]) > 0)
                    {
                        pUser uu = new pUser(cNum.cToInt(Request["rem"]), iam);
                        EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, uu.Email, "santechportal.ru: Напоминалка пароля", "<p>Уважаемый(ая) " + uu.Name + ".</p><p>Вероятно, по Вашей просьбе вам отправлена информация о Вашем пароле доступа в santechportal.ru.</p><p>Ваш пароль: <strong style='font-family: Courier New'>" + uu.Psw + "</strong></p><p>напоминаем Ваш логин: <strong>" + uu.Email + "</strong></p><br/><br/><p>ссылка для входа в портал <a href='http://santechportal.ru'>http://santechportal.ru</a></p>");
                        mess.Send();
                        lbMessage.Text = "на электронный адрес " + uu.Email + " отправлено напоминание о пароле.";
                    }
                    webInfo.LoadOwners(dlOrgList, "" + iam.OwnerID, true, iam);
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
                foreach (int id in pUser.PathToCurrent(ParentID, iam))
                {
                    pUser pu = new pUser(id, iam);
                    lbParent.Text += "&nbsp;/&nbsp;<a href='admin_usr.aspx?pid=" + id + "' class='bold'>" + pu.Name + "</a>";
                }
                pUser par = new pUser(ParentID, iam);
                lbParent.Text += "&nbsp;/&nbsp;<a href='admin_usr.aspx?pid=" + ParentID + "' class='bold'>" + par.Name + "</a>";
            }
        }








        void show_list(int page = 0)
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


            string where = (txSearch.Text == "") ? "isnull(parentid,0)=" + ParentID : "";

            //if (cNum.cToInt(dlOrgList.SelectedValue)>0)
            //    where += ((where.Length > 0) ? " and " : "") + "OwnerID="+dlOrgList.SelectedValue;

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

            if (Alphabet1.Letter != "")
                where += ((where.Length > 0) ? " and " : "") + string.Format("email like '{0}%' or Name like '{0}%'", Alphabet1.Letter);
            string search = (txSearch.Text.Trim() != "часть фио/e-mail/телефон/номер авто") ? txSearch.Text : "";
            if (search.Length > 0 && search.Length < 3)
                where += ((where != "") ? " and " : " ") + string.Format(" (Name like '{0}%' or Name like '% {0}%' or Descr like '{0}%' or [email] like '{0}%' or SubjectINN like '{0}%' or SubjectName like '{0}%' or SubjectName like '% {0}%' )", search);
            else if (search.Length > 0 && search.Length >= 3)
                where += ((where != "") ? " and " : " ") + string.Format(" (Name like '%{0}%' or Descr like '%{0}%' or [email] like '%{0}%' or SubjectName like '%{0}%'  or SubjectINN like '{0}%' )", search); ;


            _srcdt = pUserInfo.GetTable(iam, where);//, ParentID, ((Alphabet1.Letter != "" || txSearch.Text != "") ? false : chUseGroups.Checked), iam);

            //string filter = "";
            _dv = _srcdt.DefaultView;


            _dv.Sort = "IsFolder desc, Name";
            _dv.RowFilter = "";

            dgList.AllowPaging = !(Alphabet1.Letter != "" || search.Length > 0);

            if (_dv.Count < page * dgList.PageSize)
                dgList.CurrentPageIndex = 0;
            else
                dgList.CurrentPageIndex = page;
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
                    e.Item.Cells[1].Text = (ParentID == id) ? "<img src='../SIMG/16/folder.png'/>" : "<img src='../SIMG/16/folder.png'/>";
                    e.Item.Attributes.Add("class", "bold");
                    e.Item.Cells[2].Text = "<div ><a href='admin_usr.aspx?pid=" + id + "' >" + webInfo.select_search(txSearch.Text, "" + r["Name"]) + "</a></div>";
                    if (iam.IsAdmin)
                    {
                        e.Item.Cells[4].Text = "<a href='admin_usr.aspx?act=edit&id=" + id + "'>...</a>";
                        e.Item.Cells[5].Text = "";

                    }
                }
                else
                {
                    e.Item.Attributes.Add("class", "rowItem");
                    if (File.Exists(HelpersPath.PersonsAvatarFisicalPath + "/" + id + "s.png"))
                    {
                        e.Item.Cells[1].Text = "<img src='" + HelpersPath.PersonsAvatarVirtualPath + "/" + id + "s.png?d=" + DateTime.Now.Ticks + "'>";
                    }
                    

                    e.Item.Cells[2].Text = "<div ><a href='javascript:return false;' title='" + r["Name"] + "' onclick=\"openflywin('../account/card.aspx?id=" + r["ID"] + "',500,500,'" + r["Name"] + "')\">" + webInfo.select_search(txSearch.Text, "" + r["Name"]) + "</a></div>";
                    e.Item.Cells[2].Text += "<div  >" + webInfo.select_search(txSearch.Text, "" + r["email"]) + "</div>";
                    e.Item.Cells[3].Text = webInfo.select_search(txSearch.Text, "" + r["SubjectName"]);
                    //if (cNum.cToInt() > 0)
                    //{

                    //}
                    //if (!cStr.IsEmpty(r["Email"]))
                    //    e.Item.Cells[3].Text += "<div><a class='bold' href=\"#\" onclick=\"openflywin('../COMMON/email.aspx?to=" + r["Email"] + "',630,500,'e-mail сообщение');\"><img src='../img/email_16.png' title='написать письмо'></a> " + webInfo.select_search(txSearch.Text, "" + r["Email"]) + "</div>";
                    if (iam.IsAdmin || iam.IsSuperAdmin || iam.ID == id)
                    {
                        e.Item.Cells[4].Text = "<a href='admin_usr.aspx?act=edit&id=" + id + "'>...</a>";
                        e.Item.Cells[5].Text = get_last_activity("" + r["Name"]);
                        e.Item.Cells[4].Text += "<div><a href='admin_usr.aspx?" + Request.QueryString + "&rem=" + id + "' class='micro italic' title='помочь вспомнить пароль'>напомнить</a></div>";

                    }
                }


                if (_RGW_ == "fly")
                    e.Item.Cells[4].Text = "";
            }

        }
        string get_last_activity(string lc)
        {
            DataTable dt = db.GetDbTable("select max(lcd) from ORD where lc='" + lc + "'");
            return (dt.Rows.Count > 0 && cDate.cToDateTime(dt.Rows[0][0])>cDate.DateNull) ? "" + cDate.cToDateTime(dt.Rows[0][0]) : "-";
        }
        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            //dgList.CurrentPageIndex = e.NewPageIndex;
            //eID = 0;
            //curid.Value = "";
            show_list(e.NewPageIndex);
            //dgList.DataSource = _dv;
            //dgList.DataBind();
        }
        protected void dgList_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        protected void dlLoginEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            //dgList.CurrentPageIndex = 0;
            show_list(0);
            //dgList.DataSource = _dv;
            //dgList.DataBind();

        }
        protected void dlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            //dgList.CurrentPageIndex = 0;
            show_list(0);
            //dgList.DataSource = _dv;
            //dgList.DataBind();

        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ParentID = 0;
            eID = 0;
            curid.Value = "";
            //dgList.CurrentPageIndex = 0;
            show_list(dgList.CurrentPageIndex);
            //dgList.DataSource = _dv;
            //dgList.DataBind();
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

            blockPassword.Visible = (iam.ID == eID || iam.IsAdmin || iam.IsSuperAdmin);
            place4admin.Visible = (iam.IsAdmin || iam.IsSuperAdmin);
            placeNotFolder.Visible = !chIsFolder.Checked;

            lbPsw.Text = (blockPassword.Visible) ? u.Psw : "";

            UserRights.Items.FindByValue("SADM").Selected = u.IsSuperAdmin;
            UserRights.Items.FindByValue("UADM").Selected = u.IsAdmin;
            UserRights.Items.FindByValue("TP").Selected = u.IsSaller;
            
            txType.Text = u.RightsString;
            divPreview.Text = (webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + "/" + u.ID + "b.png")) ? "<div><img src=\"" + HelpersPath.PersonsAvatarVirtualPath + "/" + u.ID + "b.png?d=" + DateTime.Now.Ticks + "\" /></div>" : "";
            if (!u.IsFolder)
            {
                DataTable dt = db.GetDbTable("select id, name from " + Subject.TDB + " where Isfolder<>'Y'");
                dlSubject.Items.Clear();
                dlSubject.Items.Add(new ListItem("", ""));
                foreach (DataRow r in dt.Rows)
                {
                    dlSubject.Items.Add(new ListItem("" + r["name"], "" + r["id"]));
                }
                try
                {
                    dlSubject.Items.FindByValue("" + u.SubjectID).Selected = true;
                }
                catch { }

                webInfo.LoadOwners(dlOwner, "" + u.OwnerID, false, iam);
            }
        }



        public static void loaduserfolders(IAM iam, DropDownList dd_list, int parent_id = 0, int default_id = 0, bool load_empty = true)
        {
            dd_list.Items.Clear();
            if (load_empty) dd_list.Items.Add(new ListItem("   -   ", ""));
            DataTable dt = pUserInfo.GetTable(iam, "isfolder='Y' and state<>'D' ");
            loaduserfolders_byparent(dd_list, dt, parent_id, iam, 0);

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
        private static void loaduserfolders_byparent(DropDownList dd_list, DataTable dt, int parent_id, IAM iam, int level)
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
                    loaduserfolders_byparent(dd_list, dt, cNum.cToInt(r["ID"]), iam, level + 1);
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
            if (cNum.cToInt(txID.Text) == 0 && ((pUserInfo.Exist(txEmail.Text)&& !chIsFolder.Checked) || (pUserInfo.Exist(txName.Text) && chIsFolder.Checked)))
            {
                lbMessage.Text = (chIsFolder.Checked) ? string.Format("имя {0} уже существует в системе. Дублирование недопустимо", txName.Text) : string.Format("email {0} уже существует в системе. Дублирование email недопустимо", txEmail.Text);
                return false;
            }

            pUser u = new pUser(cNum.cToInt(txID.Text), iam, cNum.cToInt(dlOwner.SelectedValue));
            bool isnewuser = (u.ID == 0);
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
            if (UserRights.Items.FindByValue("TP").Selected) cStr.AddUnique(ref rights, "TP", ' ');
            u.RightsString = rights;



            bool access_changed = (!u.LoginEnabled && chLoginEnable.Checked);
            u.LoginEnabled = chLoginEnable.Checked;

            bool res = pUser.Save(u);
            if (res && hdPhoto.Value != "")
            {


                try
                {
                    Bitmap bmp_s = new Bitmap(HelpersPath.PersonsAvatarFisicalPath + @"\" + hdPhoto.Value + "s.png");
                    Bitmap bmp_b = new Bitmap(HelpersPath.PersonsAvatarFisicalPath + @"\" + hdPhoto.Value + "b.png");
                    bmp_b.Save(HelpersPath.PersonsAvatarFisicalPath + @"/" + u.ID + "b.png", System.Drawing.Imaging.ImageFormat.Png);
                    bmp_s.Save(HelpersPath.PersonsAvatarFisicalPath + @"/" + u.ID + "s.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {

                }

            }
            if (!isnewuser && (res && blockPassword.Visible && (txOldPass.Text == u.Psw || iam.IsSuperAdmin) && txNewPass.Text != u.Psw && txNewPass.Text.Length >= 3))
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
                try { System.IO.File.Delete(HelpersPath.PersonsAvatarFisicalPath + @"/" + hdPhoto.Value + "b.png"); }
                catch { }
                try { System.IO.File.Delete(HelpersPath.PersonsAvatarFisicalPath + @"/" + hdPhoto.Value + "s.png"); }
                catch { }
            }
            else
            {
                try { System.IO.File.Move(HelpersPath.PersonsAvatarFisicalPath + @"/" + txID.Text + "b.png", HelpersPath.PersonsAvatarFisicalPath + @"/delete_" + txID.Text + "b.png"); }
                catch { }
                try { System.IO.File.Move(HelpersPath.PersonsAvatarFisicalPath + @"/" + txID.Text + "s.png", HelpersPath.PersonsAvatarFisicalPath + @"/delete_" + txID.Text + "s.png"); }
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
            bmp_b.Save(HelpersPath.PersonsAvatarFisicalPath + @"/tmp_usr_" + iam.ID + "b.png", System.Drawing.Imaging.ImageFormat.Png);
            bmp_s.Save(HelpersPath.PersonsAvatarFisicalPath + @"/tmp_usr_" + iam.ID + "s.png", System.Drawing.Imaging.ImageFormat.Png);

            img = null;
            hdPhoto.Value = "tmp_usr_" + iam.ID;
            divPreview.Text = "<div><img  src='" + HelpersPath.PersonsAvatarVirtualPath + @"/" + hdPhoto.Value + "b.png?d=" + DateTime.Now.Ticks + "'></div>";

        }

        protected void dgList_PageIndexChanged1(object source, DataGridPageChangedEventArgs e)
        {

        }


        protected void dlSubject_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Subject s = new Subject(cNum.cToInt(dlSubject.SelectedValue), iam);
            webInfo.LoadOwners(dlOwner, "" + s.OwnerID, false, iam);
        }
    }
}