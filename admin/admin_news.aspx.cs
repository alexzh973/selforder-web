using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

using System.Data;
using System.Drawing;
using System.Text;
using System.Net.Mail;
using selforderlib;

namespace wstcp
{
    public partial class admin_news : p_p
    {
        private DataTable _srcdt;
        private DataView _dv;

        News _news = null;
        private News RECORD
        {
            get
            {
                if (_news == null)
                    _news = new News(eID, iam);
                return _news;
            }

        }

        private void init_page()
        {
            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "news";

                _ACT_ = Request.QueryString["act"];
                eID = cNum.cToInt(Request.QueryString["id"]);

                BreadPath1.AddPuncts("Все новости", "../default.aspx");


            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            check_auth();
            init_page();

            if (!IsPostBack)
            {
                if (RECORD.ID > 0 || _ACT_ == "new")
                {
                    openProfile();
                }
                else
                {
                    openList();
                }


            }
            initCommandPanel();

        }

        private void openList(int page = 0)
        {
            MultiView1.SetActiveView(vList);
            //pnlFilter.Visible = true;
            //if (ParentID > 0)
            //{
            //    curpid.Value = ParentID.ToString();
            //    show_parent_path();
            //}
            //else
            //    lbParent.Text = "";


            string where = "1=1";// (txSearch.Text == "") ? "isnull(parentid,0)=" + ParentID : "";

            //switch (dlLoginEnabled.SelectedValue)
            //{
            //    case "Y": where += ((where.Length > 0) ? " and " : "") + "LoginEnabled='Y'"; break;
            //    case "N": where += ((where.Length > 0) ? " and " : "") + "LoginEnabled='N'"; break;
            //    default: break;
            //}
            switch (dlState.SelectedValue)
            {
                case "A": where += " and isnull(State,'')='A'"; break;
                case "D": where += " and isnull(State,'')='D'"; break;
                case "": 
                default:
                where += " and isnull(State,'') not in ('A','D')"; break;
                   
            }


            string search = (txSearch.Text.Trim() != "") ? txSearch.Text : "";
            if (search.Length > 0 && search.Length < 3)
                where += string.Format(" and (Name like '{0}%' or Name like '% {0}%' or Descr like '{0}%' or [Text] like '{0}%' )", search);
            else if (search.Length > 0 && search.Length >= 3)
                where +=  string.Format(" and (Name like '%{0}%' or Descr like '%{0}%' or [Text] like '%{0}%'   )", search); 


            _srcdt = db.GetDbTable("select *,"+db.SqlNameField(pUser.TDB,"AuthorID","AuthorName")+" from "+News.TDB+" where "+where);

            //string filter = "";
            _dv = _srcdt.DefaultView;


            _dv.Sort = "Regdate desc, Name";
            _dv.RowFilter = "";

            //dgList.AllowPaging = !( search.Length > 0);

            if (_dv.Count < page * dgList.PageSize)
                dgList.CurrentPageIndex = 0;
            else
                dgList.CurrentPageIndex = page;
            dgList.DataSource = _dv;
            dgList.DataBind();
        }

        void initCommandPanel()
        {
            CommandPanel4Profile1.Visible_ButtonSave = false;
            CommandPanel4Profile1.Event_SaveAndClose = new EventHandler(this.bntOK_Click);
            CommandPanel4Profile1.Event_Cancel = new EventHandler(this.btnCancel_Click);
            CommandPanel4Profile1.Visible_ButtonDelete = CommandPanel4Profile1.Visible_ButtonSaveAndClose = (RECORD.AuthorID == iam.ID || iam.IsSuperAdmin);
            CommandPanel4Profile1.Event_Delete = new EventHandler(this.btnDelete_Click);
        }


        void openProfile()
        {

            if (RECORD == null)
            {
                btnCancel_Click(null, null);
                return;
            }
            MultiView1.SetActiveView(vProfile);
            btnUnlock.Visible = (RECORD.LockForModify(iam) && iam.IsSuperAdmin);



            if ((!RECORD.LockForModify(iam) || !RECORD.Can_I_Modify(iam)))
            {
                pUser cm = new pUser(eObject.GetCurrentModifierID(RECORD.ThisObject), iam);
                lbMessage.Text = "запись [id: " + eID + "] заблокирована (" + cm.Name + ")";

                txName.ReadOnly = txDescr.ReadOnly = txText.ReadOnly = true;
                chCommentPossible.Enabled = ImageButton1.Enabled = btnLoadPic.Enabled = chPublished.Enabled = chCommentPossible.Enabled = false;
                CommandPanel4Profile1.Visible_ButtonDelete = CommandPanel4Profile1.Visible_ButtonSave = CommandPanel4Profile1.Visible_ButtonSaveAndClose = false;
            }
            else
            {
                txName.ReadOnly = txDescr.ReadOnly = txText.ReadOnly = false;
                chCommentPossible.Enabled = ImageButton1.Enabled = btnLoadPic.Enabled = FileUpload1.Enabled = chPublished.Enabled = chCommentPossible.Enabled = true;
                CommandPanel4Profile1.Visible_ButtonDelete = CommandPanel4Profile1.Visible_ButtonSave = CommandPanel4Profile1.Visible_ButtonSaveAndClose = true;
            }
            txID.Text = RECORD.ID.ToString();

            chPublished.Checked = RECORD.Published;

            txName.Text = RECORD.Name;
            txDescr.Text = RECORD.Descr;
            ucEndDate.SelectedDate = RECORD.EndDate;
            txText.Text = RECORD.Text;
            imgPreview.ImageUrl = "../MEDIA/News/Trumb/" + RECORD.ID + ".png";

            chCommentPossible.Checked = RECORD.CommentPossible;



            lbLastCorrect.Text = RECORD.lc + " [" + RECORD.lcd.ToString() + "]";
        }



        protected void bntOK_Click(object sender, EventArgs e)
        {
            bool result = false;

            ViewArticle.AddView(iam, RECORD.ThisObject);
            RECORD.Name = txName.Text;
            RECORD.Descr = txDescr.Text;
            RECORD.Text = txText.Text;
            RECORD.CommentPossible = chCommentPossible.Checked;
            RECORD.EndDate = ucEndDate.SelectedDate;
            bool needsendsubscribe = ((RECORD.ID > 0 && !RECORD.Published && chPublished.Checked) || (RECORD.ID == 0 && chPublished.Checked));
            bool needsendtocurator = (RECORD.ID == 0 && !chPublished.Checked);
            RECORD.Published = chPublished.Checked;

            result = News.Save(RECORD);

            if (result)
            {
                txID.Text = RECORD.ID.ToString();
                eID = RECORD.ID;

                if (hdImgSmall.Value != "")
                {
                    try
                    {
                        Bitmap bmp_s = new Bitmap(System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA\News\Trumb\" + hdImgSmall.Value + ".png");
                        bmp_s.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA/News/Trumb/" + RECORD.ID + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (needsendsubscribe)
                {

                    string tmpl_Title = "Новость santechportal.ru: " + RECORD.Name;
                    string tmpl_Body = cDate.TodayS + "<h4>" + RECORD.Name + "</h4><p>" + RECORD.Descr + "</p><p>" + "---------------------------</p/><p><a href='http://www.santechportal.ru/news/view.aspx?id=" + RECORD.ID + "' >читать полностью...</a></p>";
                    SmtpClient client = new SmtpClient(CurrentCfg.MailServer, 25);
                    MailMessage m = new MailMessage(CurrentCfg.EmailPortal, iam.Email, tmpl_Title, tmpl_Body);
                    m.BodyEncoding = Encoding.UTF8;
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;
                    foreach (DataRow r in db.GetDbTable("select subs.userid, usr.email as email_to from " + Subject.TDB + " as subs inner join " + pUser.TDB + " as usr on subs.userid=usr.id and usr.state not in ('D','A') and usr.LoginEnabled='Y' AND usr.email NOT LIKE '%?%' and usr.getmails='Y'").Select(""))
                    {
                        try
                        {

                            m.Bcc.Add("" + r["email_to"]);
                        }
                        catch
                        {
                        }
                    }
                    client.Send(m);
                }
                eID = RECORD.ID;

                btnCancel_Click(sender, e);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pUser.Unlock(new sObject(cNum.cToInt(txID.Text), News.TDB), iam);
            Session["NEWS_profile"] = null;
            Response.Redirect("../default.aspx");
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            pUser.Unlock(new sObject(cNum.cToInt(txID.Text), News.TDB), iam);
            News.Delete(cNum.cToInt(txID.Text), iam);

            Session["NEWS_profile"] = null;
            Response.Redirect("../default.aspx");
        }


        protected void btnLoadPic_Click(object sender, EventArgs e)
        {
            string filetype = FileUpload1.PostedFile.ContentType;
            if (filetype.IndexOf("image") < 0 || FileUpload1.PostedFile.ContentLength > 10000000) return;
            System.Drawing.Image img = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
            int h = img.Height;
            int w = img.Width;
            int size_s = 64;
            decimal p_s;

            p_s = ((decimal)size_s) / ((h > w) ? h : w);
            if (p_s > 1) p_s = 1;

            Bitmap bmp_s = new Bitmap(img, cNum.cToInt(img.Width * p_s), cNum.cToInt(img.Height * p_s));
            try
            {
                bmp_s.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA\News\Trumb\tmp_news_" + iam.ID + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex) { }

            img = null;

            hdImgSmall.Value = "tmp_news_" + iam.ID;
            imgPreview.ImageUrl = "../MEDIA/News/Trumb/" + hdImgSmall.Value + ".png";

        }
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            if (hdImgSmall.Value != "")
            {
                try { System.IO.File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA\News\Trumb\" + hdImgSmall.Value + ".png"); }
                catch (Exception ex) { }
            }
            else
            {
                try { System.IO.File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA\News\Trumb\" + txID.Text + ".png", System.AppDomain.CurrentDomain.BaseDirectory + @"MEDIA\News\Trumb\delete_" + txID.Text + ".png"); }
                catch (Exception ex) { }
            }
            hdImgSmall.Value = "";

            imgPreview.ImageUrl = "";

        }

        protected void txText_PreRender(object sender, EventArgs e)
        {
            txText.Attributes.Add("name", "txText");
        }

        protected void btnUnlock_Click(object sender, ImageClickEventArgs e)
        {
            News.Unlock(RECORD, iam);
            openProfile();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            openList();
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            openList(e.NewPageIndex);
        }
        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);


                e.Item.Attributes.Add("id", "object_" + id);
                if ("" + r["State"] == "A") e.Item.ForeColor = cColor.Color("silver");
                e.Item.Attributes.Add("onclick", "thisRow(this.id);");
                e.Item.Attributes.Add("class", "rowItem");

                e.Item.Cells[3].Text = "<div><a href='javascript:return false;'  onclick=\"openflywin('../news/preview.aspx?id=" + r["ID"] + "',600,700,'" + r["Name"] + "')\">" + webInfo.select_search(txSearch.Text, "" + r["Name"]) + "</a></div>";
                e.Item.Cells[3].Text += "<div  >" + webInfo.select_search(txSearch.Text, "" + r["Descr"]) + "</div>";
                e.Item.Cells[4].Text = "" + r["AuthorName"];
                if (iam.IsSuperAdmin || iam.ID == cNum.cToInt(r["AuthorID"]))
                {
                    e.Item.Cells[5].Text = "<a href='admin_news.aspx?act=edit&id=" + id + "'>[изменить]</a><a href='../news/view.aspx?id=" + id + "'>[просмотр]</a>";

                }
            }

        }
    }
}