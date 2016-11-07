using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;

using System.Collections;

using System.Drawing;
using selforderlib;

namespace wstcp
{
    public partial class admin_partner : p_p
    {
        int CategoryID
        {
            get { return cNum.cToInt(ViewState["cid"]); }
            set { ViewState["cid"] = "" + value; }
        }
        Subject _rec;
        Subject RECORD
        {
            get { if (_rec == null) _rec = new Subject(eID, iam); return _rec; }
        }
        

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
                ((adminmaster)this.Master).SelectedMenu = "subjects";

                _ACT_ = Request["act"];
                CategoryID = cNum.cToInt(Request["cid"]);
                eID = cNum.cToInt(Request["id"]);
                _rec = null;
                switch (_ACT_)
                {
                    case "new":
                    case "edit":
                        initCommandPanel();
                        openProfile();
                        break;
                    case "view":
                        break;
                    default:
                        restoreFilter("AdminPartner");
                        //show_categories();
                        show_list();
                        break;

                }
            }
            else
            {
                initCommandPanel();
            }
            
            


        }

        void initCommandPanel()
        {

            CommandPanel4Profile1.Event_SaveAndClose = new EventHandler(this.bntOK_Click);
            CommandPanel4Profile1.Event_Cancel = new EventHandler(this.btnCancel_Click);
            CommandPanel4Profile1.Visible_ButtonDelete = CommandPanel4Profile1.Visible_ButtonSaveAndClose =  iam.IsSuperAdmin;
            CommandPanel4Profile1.Event_Delete = new EventHandler(this.btnDelete_Click);
        }


        private void saveFilter(string page)
        {
            
                    
        }

        private bool restoreFilter(string page)
        {
            //if (!UserFilter.Exist(iam.ID, page + "_%"))
            //    return false;

            string sort = UserFilter.Load(iam.ID, "adminsubjsort");
            try
            {
                dlSortlist.Items.FindByValue(sort).Selected = true;
            }
            catch
            {
                dlSortlist.SelectedIndex = 0;
                UserFilter.Save(iam.ID, "adminsubjsort", dlSortlist.SelectedValue);
            }
           
             return true;
        }

        private string getFilterString()
        {
            string where;

            where = "1=1";

                
            if (txSearch.Text.Length > 0 && txSearch.Text.Length < 4)
                where += String.Format(" and (Name like '{0}%' or Name like '% {0}%' or INN like '{0}%' or Code like '{0}%' or emailtas like '{0}%')", txSearch.Text);
            else if (txSearch.Text.Length > 0 && txSearch.Text.Length >= 4)
                where += String.Format(" and (Name like '%{0}%' or INN like '%{0}%' or Code like '%{0}%' or emailtas like '%{0}%'  )", txSearch.Text);

            showFilter();
            return where;
        }



        private void showFilter()
        {
            lbFilter.Text = "";
            string str = "";
            
            if (str.Length > 0)
                lbFilter.Text = "<div class='small bold italic' style='color:blue;'>" + str + "</div>";

        }




        DataTable DT = new DataTable();

        //void show_categories()
        //{
        //    string qr = "select id, name, (SELECT CASE WHEN id=" + CategoryID + " THEN 'shadow' else ''END) as sel,(select count(news.ID) from " + Subject.TDB + " as news where news.CategoryID=cat.ID and news.state='' ) as qty, (select max(news.RegDate) from " + Partner.TDB + " as news where news.CategoryID=cat.ID and news.state='' ) as lastrecdate from " + meta.TDB_ECATEGORY + " as cat where cat.type='" + Partner.TDB + "' and cat.orgid=" + iam.Orgid;
        //    DataTable dt = db.GetDbTable(qr);
        //    rpCategories.DataSource = dt;
        //    rpCategories.DataBind();
        //}
        void show_list(int page=0)
        {
            MultiView1.SetActiveView(vList);
            
            string flt = getFilterString();
            DT = db.GetDbTable("select *, (select count(id) from ORD where subjectid=subj.id) as qtyord,(select max(regdate) from ORD where subjectid=subj.id) as lastord, (select top 1 name from "+pUser.TDB+" where subj.emailtas like '%'+email+'%' ) as ta from " + Subject.TDB + " as subj where " + ((flt.Length > 0) ? flt : ""));
            DataView dv = DT.DefaultView;
            showFilter();
            switch (dlSortlist.SelectedValue)
            {
                case "qty":
                    dv.Sort = "qtyord desc";
                    break;
                case "regdate":
                    dv.Sort = "lastord desc";
                    break;
                default:
                    dv.Sort = "name";
                    break;
            }
            //dv.Sort = "qtyord desc, Name";
            if (dv.Count < page * dgList.PageSize)
                dgList.CurrentPageIndex = 0;
            else
                dgList.CurrentPageIndex = page;
            dgList.DataSource = dv;
            dgList.DataBind();
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            show_list(e.NewPageIndex);
        }
        protected void dgList_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "select")
            show_orders(cNum.cToInt(e.Item.Cells[0].Text));

        }

        private void show_orders(int p)
        {
            DataTable dt = db.GetDbTable("select * from ORD where SubjectId="+p);
            dgOrders.DataSource = dt;
            dgOrders.DataBind();
        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                Subject sbj = new Subject(cNum.cToInt(r["ID"]), iam);

                e.Item.Attributes.Add("id", "object_" + r["ID"]);
                e.Item.Attributes.Add("onclick", "thisRow(this.id);setCurid('" + sbj.ID + "');");

                string css = "";
                switch (sbj.State)
                {
                    case "A":
                        css += " gray";
                        break;
                    case "D":
                        css += " deleted";
                        break;
                    default:
                        
                        break;
                }
                                
                
                e.Item.Attributes.Add("class", css);
                string name = "<a class='bold' href='admin_Partner.aspx?act=view&id=" + sbj.ID + "'>" + sbj.Name + "</a>";
                
                //string text = "<div>" + ((tsk.Descr.Length > 150) ? cStr.CutString(tsk.Descr, 150) + "<span title='" + tsk.Descr + "'> ...</span>" : tsk.Descr) + "</div>";

                e.Item.Cells[1].Text = "<div>" + name + "" + "</div>";
                e.Item.Cells[1].Text += "<div>ИНН "+sbj.INN+", код 1С "+sbj.Code+"</div>";
                e.Item.Cells[1].Text += "<div>код договора " + sbj.CodeDG + ", тип цен " + sbj.PriceType + "</div>";
                e.Item.Cells[1].Text += "<div class='small italic'>представитель: <strong>" +getinfo_person(sbj.ID)  + "</strong>" + "</div>";
                
                e.Item.Cells[1].Text += "<div class='small italic'>инфа по заказам: " + getinfo_orders(sbj.ID) + "" + "</div>";
                e.Item.Cells[1].Text += "<div class='small blue'>та: <strong>" + r["ta"]+ "</strong>" + "</div>";
                
               
                //e.Item.Cells[2].Text = "<div>" + name + "&nbsp;" + "<span class='small attributes'>[" + tsk.RegDate.ToShortDateString() + "]</span></div>" + "<div class='small'>" + text + "</div>";
                //e.Item.Cells[3].Text = cDate.cToString(tsk.EndDate) + "<br/>";
                
                //e.Item.Cells[4].Text = (tsk.Published)?HelpersImg.Ico16_Published_admin:HelpersImg.Ico16_NotPublished_admin;;
                
                e.Item.Cells[2].Text = "";
                if ( iam.IsSuperAdmin)
                    e.Item.Cells[2].Text += "&nbsp;<a href='admin_Partner.aspx?act=edit&id=" + sbj.ID + "' title='изменить'>[edit]</a>";
                e.Item.Cells[2].Text += "&nbsp;<a href='admin_Partner.aspx?act=copy&id=" + sbj.ID + "' title='скопировать'>[copy]</a>";
            }

        }

        private string getinfo_orders(int subject_id)
        {
           string ret = "";
           DataTable dt = db.GetDbTable("select id, regdate, summorder from " + Order.TDB + " where SubjectID=" + subject_id + " and State<>'D' order by regdate desc");
           if (dt.Rows.Count > 0 && dt.Columns.Count > 2)
           {
               ret = "всего " + dt.Rows.Count + " шт, последний от " + cDate.cToString(dt.Rows[0][1]) + " на сумму " + cNum.cToDecimal(dt.Rows[0][2],2)+"руб.";
           }
            return ret; 
        }

        private string getinfo_person(int subject_id)
        {
            string ret = "";
            DataTable dt = db.GetDbTable("select id, name, email, phones from " + pUser.TDB + " where SubjectID=" + subject_id + " and IsFolder='N' and State<>'D'");
            if (dt.Rows.Count > 0 && dt.Columns.Count > 2)
                ret = "" + dt.Rows[0][1] + ": " + dt.Rows[0][2] + ", " + dt.Rows[0][3];
            return ret;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //saveFilter("AdminPartner");
            //dgList.CurrentPageIndex = 0;
            show_list(0);
        }

        



        void openProfile()
        {
            MultiView1.SetActiveView(vProfile);
            if ((!RECORD.LockForModify(iam) || !RECORD.Can_I_Modify(iam)))
            {
                pUser cm = new pUser(eObject.GetCurrentModifierID(RECORD.ThisObject), iam);
                lbMessage.Text = "запись [id: " + eID + "] заблокирована (" + cm.Name + ")";

                txName.ReadOnly = txCode.ReadOnly = txEmailTAs.ReadOnly = txINN.ReadOnly = true;
            }
            else
            {
                txName.ReadOnly = txCode.ReadOnly = txEmailTAs.ReadOnly = txINN.ReadOnly = false;
            }
            txID.Text = ""+RECORD.ID;

            txName.Text = RECORD.Name;
            txINN.Text = RECORD.INN;
            txEmailTAs.Text = RECORD.EmailTAs;
            txCode.Text = RECORD.Code;
            txCodeAgr.Text = RECORD.CodeDG;
            chUseSmsAuthorization.Checked = RECORD.UseSmsAuthorization;


            webInfo.LoadOwners(dlOwner,""+RECORD.OwnerID,false,iam);

            dlPriceType.ClearSelection();
            try
            {
                dlPriceType.Items.FindByValue(RECORD.PriceType).Selected = true;
            }
            catch 
            {
                dlPriceType.SelectedIndex = 0;
            }
            chIsFolder.Checked = RECORD.IsFolder;
            webInfo.LoadSubjectFolders(dlParent, "" + RECORD.ParentID,true,iam);
            

            DataTable persdt = db.GetDbTable("select ID,Name,email,phones from " + pUser.TDB + " where SubjectID=" + RECORD.ID+" and isnull(SubjectID,0)<>0");
            dgPersons.DataSource = persdt;
            dgPersons.DataBind();

            if (RECORD.ID>0 && persdt.Rows.Count > 0)
            {
                show_personfornm(cNum.cToInt(persdt.Rows[0]["ID"]));
            }
            else
            {
                chLoginEnable.Checked = true;
                txIDPers.Text =
                txEmailPers.Text =
                txNamePers.Text =
                txPassPers.Text = "";
                txPhone.Text = "";
            }
        }

        protected void show_personfornm(int id)
        {
            pUser pers = new pUser(id, iam);
            txIDPers.Text = "" + pers.ID;
            txEmailPers.Text = pers.Email;
            txNamePers.Text = pers.Name;
            txPassPers.Text = pers.Psw;
            txPhone.Text = pers.Phones;
            chLoginEnable.Checked = pers.LoginEnabled;
        }

        protected void bntOK_Click(object sender, EventArgs e)
        {
            bool result = false;
            lbMessage.Text = "";
            if (RECORD.ID == 0 && Subject.FindByField("Name", txName.Text.Trim()) > 0)
            {
                lbMessage.Text = "контрагент с таким названием уже заведен в систему";
                return;
            }


            RECORD.Name = txName.Text;
            RECORD.Code = txCode.Text;
            RECORD.EmailTAs = txEmailTAs.Text;
            RECORD.INN = txINN.Text;
            RECORD.IsFolder = chIsFolder.Checked;
            RECORD.ParentID = cNum.cToInt(dlParent.SelectedValue);
            RECORD.CodeDG = txCodeAgr.Text;
            RECORD.PriceType = dlPriceType.SelectedValue;
            RECORD.UseSmsAuthorization = chUseSmsAuthorization.Checked;
            RECORD.OwnerID = cNum.cToInt(dlOwner.SelectedValue);
            result = Subject.Save(RECORD);

            if (result)
            {
                txID.Text = ""+RECORD.ID;
                eID = RECORD.ID;

                if (txNamePers.Text != "" && txEmailPers.Text != "")
                {
                    pUser pers = new pUser(cNum.cToInt(txIDPers.Text), iam, RECORD.OwnerID);
                    if ((pers.ID == 0 && pUser.FindByField("Email", txEmailPers.Text) <= 0) || pers.ID > 0)
                    {
                        pers.Name = txNamePers.Text;
                        pers.Email = txEmailPers.Text;
                        pers.SubjectID = RECORD.ID;
                        pers.ParentID = 100005;
                        pers.LoginEnabled = chLoginEnable.Checked;
                        pers.Phones = txPhone.Text;
                        pers.IsFolder = false;
                        
                        pUser.Save(pers);
                        if (txPassPers.Text != "") pUser.SetNewPassword(pers, pers.Psw, txPassPers.Text, iam);
                        Subject.Unlock(new sObject(eID, Subject.TDB), iam);
                        EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, RECORD.EmailTAs, "Подключение нового клиента к Системе самостоятельного заказа (" + RECORD.Name + ")", template_mess.Replace("#Партнер#", RECORD.Name).Replace("#Представитель#", pers.Name).Replace("#Логин#", pers.Email).Replace("#Пароль#", pers.Psw));
                        mess.Send("alexzh@santur.ru");
                        Session["Partner_profile"] = null;
                        Response.Redirect("admin_Partner.aspx?pid=" + RECORD.ParentID);
                    }
                    else
                    {
                        lbMessage.Text = "Представитель с указанным email уже заведен в систему";
                    }

                }
                else
                {
                    Subject.Unlock(new sObject(eID, Subject.TDB), iam);
                    Session["Partner_profile"] = null;
                    Response.Redirect("admin_Partner.aspx?pid=" + RECORD.ParentID);
                }
                
                


                
            }
        }
        string template_mess = "<p>к Системе самостоятельного заказа подключен клиент <strong>#Партнер#</strong></p><p>Доступ получает представитель партнера: <strong>#Представитель#</strong>. Вы можете передать ему/ей регистрационные данные:</p><p>адрес Системы самостоятельного заказа: <strong>http://www.santechportal.ru</strong></p><p>логин: <strong>#Логин#</strong></p><p>пароль: <strong>#Пароль#</strong></p><hr/><p></p>";

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Subject.Unlock(new sObject(eID, Subject.TDB), iam);
            Session["Partner_profile"] = null;
            Response.Redirect("admin_Partner.aspx?pid=" + RECORD.ParentID);
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Subject.Unlock(new sObject(cNum.cToInt(txID.Text), Subject.TDB), iam);
            //Subject.Delete(cNum.cToInt(txID.Text), iam);
            Session["Partner_profile"] = null;
            Response.Redirect("admin_Partner.aspx?pid=" + RECORD.ParentID);
        }

        protected void dgOrders_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                e.Item.Cells[2].Text = "<div class='bold' onclick=\"openflywin('../order/detailfly.aspx?id="+r["ID"]+"', 850, 500, 'Заявка "+r["Name"]+" от "+cDate.cToString(r["RegDate"])+"')\">" + r["Name"] + "</div><div class='small italic'>" + r["Descr"] + "</div>";
                e.Item.Cells[5].Text = "<div class='small'>" + r["lcd"] + "</div><div class='small italic'>" + r["lc"] + "</div>";
            }
        }

        protected void dgPersons_SelectedIndexChanged(object sender, EventArgs e)
        {           
            show_personfornm(cNum.cToInt(dgPersons.Items[dgPersons.SelectedIndex].Cells[0].Text));
        }

        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            show_personfornm(0);
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(vReport);
            DataTable dtsubj = db.GetDbTable("select * from (select subj.id, subj.Name, (select top 1 name from ensouser where subj.emailtas like '%'+email+'%') as ta,(select count(ord.id) from ord where ord.subjectid=subj.id) as qtyords, (select max(ord.regdate) from ord where ord.subjectid=subj.id) as lastord, isnull( (select sum(ord.summorder) from ord where ord.subjectid=subj.id),0) as summords from SUBJ) as rep order by summords desc");
            dgReport.DataSource = dtsubj;
            dgReport.DataBind();
        }

        protected void dlSortlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "adminsubjsort", dlSortlist.SelectedValue);
            show_list(0);
        }

        


       
    }
}