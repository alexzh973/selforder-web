using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using selforderlib;

namespace wstcp
{
    public partial class mainpage : System.Web.UI.MasterPage
    {
        IAM iam;

        bool checkCookies()
        {
            bool result = false;
            HttpCookie uc = Request.Cookies["nsicook"];
            string usrcook = (uc != null) ? uc.Value : "";
            if (usrcook == "") return false;
            string[] u = usrcook.Split('#');
            string lgn = (u.Length > 0) ? u[0] : "";
            string pwd = (u.Length > 1) ? u[1] : "";
            if (ensoCom.db.GetDbTable("select ID from " + Owner.TDB + " where Name + ':' + CAST(ID AS nvarchar)='" + pwd + "'").Rows.Count > 0)
            {
                Session["MYOWN"] = lgn;
                result = true;
            }

            return result;
        }

        void check_auth()
        {
            string str = System.Configuration.ConfigurationManager.AppSettings["ipaccess"];
            if (!Global.ips.Contains(Request.ServerVariables["REMOTE_ADDR"]))
                ContentPlaceHolder1.Visible = false;
            else
            {
                ContentPlaceHolder1.Visible = true;
                Session["MYOWN"] = Global.adrs[Request.ServerVariables["REMOTE_ADDR"]];
            }


        }

        public void ReloadUserString()
        {
            LoginStringInHeader1.Reload();

        }
        public bool VisibleLeftPanel
        {
            set
            {
                leftplace.Visible = value;
                __content.CssClass = value ? ((VisibleRightPanel) ? "col-md-6" : "col-md-9") : ((VisibleRightPanel) ? "col-md-9" : "col-md-12");
            }
            get { return leftplace.Visible; }
        }
        public bool VisibleRightPanel
        {
            set
            {
                rightplace.Visible = value;
                __content.CssClass = (value) ? ((VisibleLeftPanel) ? "col-md-6" : "col-md-9") : ((VisibleLeftPanel) ? "col-md-9" : "col-md-12");
            }
            get { return rightplace.Visible; }
        }
        public string SelectedMenu
        {
            set { ViewState["menupunct"] = value; }
            get { if ("" + ViewState["menupunct"] == "") SelectedMenu = "home"; return "" + ViewState["menupunct"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            iam = IamServices.GetIam(Session.SessionID);
            if (iam.PresentedSubjectID > 0)
            {
                //Subject subj = new Subject(iam.PresentedSubjectID, iam);
                show_personalTAs(iam.PresentedSubject);
                //show_subj_info(iam.PresentedSubject);
            }
            else
            {
                //lbSubject.Text = "";
            }
            paint_menu_2();
        }
        //public void show_subj_info(Subject subj)
        //{
        //    lbCurrBlnc.Text = "";           

        //    lbSubject.Text = "<strong title='ИНН "+subj.INN+"'>" + subj.Name + "</strong>";

        //    DGinfo d = Subject.GetDg(subj.ID, subj.CodeDG);

        //    lbCurrBlnc.Text = "Баланс "+((d.CurrentDZ >= 0) ? ((d.CurrentDZ == 0) ? "0.00руб" : "<span class='red'>-" + d.CurrentDZ + "руб</span>") : "<span class='green'>+" + (d.CurrentDZ* -1) + "руб</span>");

        //}
        public void show_personalTAs(Subject subj)
        {

            if (iam == null) iam = IamServices.GetIam(Session.SessionID);

            if (subj.ID == 0)
            {
                rpTAs.Visible = false;
                return;
            }
            DataTable dt = db.GetDbTable("select 0 as id, '' as Name, '' as email, '' as phone, '' as photo where 1=0");

            DataRow nr;
            if (subj.EmailTAs == "")
            {
                nr = dt.NewRow();
                nr["id"] = 0;
                nr["Name"] = "не назначен ни один менеджер";
                nr["email"] = CurrentCfg.EmailSupport;
                nr["phone"] = "(343)270-04-04";
                dt.Rows.Add(nr);
            }
            else
            {
                pUser ta = new pUser(pUser.FindByField("email", subj.EmailTAs), iam);
                {
                    nr = dt.NewRow();
                    nr["id"] = ta.ID;
                    nr["Name"] = ta.Name;
                    nr["email"] = ta.Email;
                    nr["phone"] = ta.Phones;
                    nr["photo"] = webUser.PhotoSmallPath(ta.ID);
                    dt.Rows.Add(nr);
                }
            }
            rpTAs.DataSource = dt;
            rpTAs.DataBind();
        }


        //private void paint_menu()
        //{
        //    IAM iam = IamServices.GetIam(Session.SessionID);
        //    blockMainMenu.Text = "";
        //    blockAdminMenu.Text = "";

        //    blockMainMenu.Text += "<a href='../default.aspx' class='bold white " + ((SelectedMenu == "home") ? "mainmenuselected" : "mainmenu") + "'>Главная</a>";
        //    if (iam.ID < 100000)
        //    {
        //        blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
        //        return;
        //    }

        //    blockMainMenu.Text += " | <a href='../good/goodlist.aspx' class='bold white " + ((SelectedMenu == "order") ? "mainmenuselected" : "mainmenu") + "'>Самостоятельная заявка</a>";


        //    if (iam.SubjectID == 0)
        //        blockMainMenu.Text += " | <a href='../nsigoodies.aspx' class='bold white " + ((SelectedMenu == "incash") ? "mainmenuselected" : "mainmenu") + "'>Тов. ост. минихолдинга</a>";

        //    blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
        //    blockMainMenu.Text += " | <a href='../msg.aspx' class='bold white " + ((SelectedMenu == "msg") ? "mainmenuselected" : "mainmenu") + "'>Отправить сообщение</a>";

        //    if (iam.IsSuperAdmin)
        //    {
        //        blockAdminMenu.Text += "<a href='../admin/admin_owner.aspx' class='bold white " + ((SelectedMenu == "owners") ? "mainmenuselected" : "mainmenu") + "'>Владельцы</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin_partner.aspx' class='bold white " + ((SelectedMenu == "subjects") ? "mainmenuselected" : "mainmenu") + "'>Контрагенты</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin_usr.aspx' class='bold white " + ((SelectedMenu == "users") ? "mainmenuselected" : "mainmenu") + "'>Пользователи</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin_syssetting.aspx' class='bold white " + ((SelectedMenu == "consts") ? "mainmenuselected" : "mainmenu") + "'>Константы</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin.aspx' class='bold white " + ((SelectedMenu == "setup") ? "mainmenuselected" : "mainmenu") + "'>Настройки</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin_import.aspx' class='bold white " + ((SelectedMenu == "import") ? "mainmenuselected" : "mainmenu") + "'>Импорт</a>";
        //        blockAdminMenu.Text += " | <a href='../admin/admin_sysmsg.aspx' class='bold white " + ((SelectedMenu == "sysmsg") ? "mainmenuselected" : "mainmenu") + "'>Сист.сообщения</a>";
        //    }
        //}

        private void paint_buttons()
        {
            if (iam.ID < 100000) return;
            string btns = "";

            btns += "<a href='../order/orderdefault.aspx' class='btn-new-order' >Сделать заявку</a>";
            btns += "<li class='" + ((SelectedMenu == "msg") ? "selected" : "") + "'><a onclick=\"openflywin('../msg.aspx?rgw=fly', 600, 430)\" href='javascript:return 0;' class='btn-send-msg'>Отправить сообщение</a></li>";


        }
        private void paint_menu_2()
        {
            if (iam == null)
                iam = IamServices.GetIam(Session.SessionID);
            blockMainMenu.Text = "";
            blockAdminMenu.Text = "";
            blockMainMenu.Text += "<ul class='nav navbar-nav'>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "home") ? "selected" : "") + "'><a href='../default.aspx' >Главная</a></li>";
            if (iam.ID >= 100000)
                blockMainMenu.Text += "<li class='  " + ((SelectedMenu == "order") ? "selected" : "") + "'><a href='../order/orderdefault.aspx' >Заявка " + ((iam.CurOrder != null && iam.CurOrder.Items.Count > 0) ? "<span title='Есть несохраненная заявка' class='red bold'>*</a>" : "") + "</a></li>";
            if (iam.ID >= 100000 && (iam.IsAdmin || iam.IsTA || iam.IsSaller))
                blockMainMenu.Text += "<li class='  " + ((SelectedMenu == "incash") ? "selected" : "") + "'><a href='../good/list.aspx'>Тов. ост. минихолдинга</a></li>";
            //    blockMainMenu.Text += "<li class='" + ((SelectedMenu == "msg") ? "selected" : "") + "'><a href='../msg.aspx'>Отправить сообщение</a></li>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "sales") ? "selected" : "") + "'><a href='../good/sales.aspx'>Акции</a></li>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "lk") ? "selected" : "") + "'><a href='../account/lk.aspx'>Личный кабинет</a></li>";

            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "help") ? "selected" : "") + "'><a href='../common/help.aspx'>Инструкция</a></li>";
            if (iam.ID >= 100000)
                blockMainMenu.Text += "<li><a onclick=\"openflywin('../msg.aspx?rgw=fly', 600, 430)\" href='javascript:return 0;'>Обратная связь</a></li>";


            if (iam.IsSuperAdmin || iam.IsAdmin)
            {
                blockMainMenu.Text += "<li class='" + ((SelectedMenu == "help") ? "selected" : "") + "'><a href='../admin/admin_orders.aspx'>админка</a></li>";

                //blockAdminMenu.Text = "<ul class='small'>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "owners") ? "selected" : "") + "'><a href='../admin/admin_owner.aspx'>Владельцы</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "subjects") ? "selected" : "") + "'><a href='../admin/admin_partner.aspx'>Контрагенты</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "users") ? "selected" : "") + "'><a href='../admin/admin_usr.aspx'>Пользователи</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "orders") ? "selected" : "") + "'><a href='../admin/admin_orders.aspx'>Заказы</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "consts") ? "selected" : "") + "'><a href='../admin/admin_syssetting.aspx'>Константы</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "setup") ? "selected" : "") + "'><a href='../admin/admin.aspx'>Настройки</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "news") ? "selected" : "") + "'><a href='../admin/admin_news.aspx'>Новости</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "import") ? "selected" : "") + "'><a href='../admin/admin_import.aspx'>Импорт</a></li>";
                //blockAdminMenu.Text += "<li class='" + ((SelectedMenu == "sysmsg") ? "selected" : "") + "'><a href='../admin/admin_sysmsg.aspx'>Сист.сообщения</a></li>";
                //blockAdminMenu.Text += "</ul>";
            }
            blockMainMenu.Text += "</ul>";
        }

        protected void btnQuit_Click(object sender, EventArgs e)
        {
            if (iam == null) iam = IamServices.GetIam(Session.SessionID);
            IamServices.ClearSession(iam, iam.SessionID);
            iam = IamServices.GetIam(Session.SessionID);
            paint_menu_2();

            //lbSubject.Text = "";
            LoginStringInHeader1.Reload();

        }


    }
}