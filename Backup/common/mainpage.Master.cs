using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

namespace wstcp
{
    public partial class mainpage : System.Web.UI.MasterPage
    {
        bool checkCookies()
        {
            bool result = false;            
            HttpCookie uc = Request.Cookies["nsicook"];
            string usrcook = (uc != null) ? uc.Value : "";
            if (usrcook == "") return false;
            string[] u = usrcook.Split('#');
            string lgn = (u.Length > 0) ? u[0] : "";
            string pwd = (u.Length > 1) ? u[1] : "";
            if (ensoCom.db.GetDbTable("select ID from " + NSImeta.TDB_OWNERS + " where Name + ':' + CAST(ID AS nvarchar)='" + pwd + "'").Rows.Count > 0)
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

        public bool VisibleLeftPanel
        {
            set { leftplace.Visible = value; }
        }
        public bool VisibleRightPanel
        {
            set { rightplace.Visible = value; }
        }
        public string SelectedMenu
        {
            set { ViewState["menupunct"] = value; }
            get { if (""+ViewState["menupunct"] == "")SelectedMenu = "home"; return ""+ViewState["menupunct"]; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            lbIP.Text = ""+Request.ServerVariables["REMOTE_ADDR"];
            if (!IsPostBack)
            {
                paint_menu();
            }
           
        }

        private void paint_menu()
        {
            IAM iam = IAM.GetMe(Session.SessionID);
            blockMainMenu.Text = "";
            blockAdminMenu.Text = "";

            blockMainMenu.Text += "<a href='../default.aspx' class='bold white " + ((SelectedMenu == "home") ? "mainmenuselected" : "mainmenu") + "'>Главная</a>";
            if (iam.ID < 100000)
            {
                blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
                return;
            }

            blockMainMenu.Text += " | <a href='../good/goodlist.aspx' class='bold white " + ((SelectedMenu == "order") ? "mainmenuselected" : "mainmenu") + "'>Самостоятельная заявка</a>";

            
            if (iam.SubjectID==0)
                blockMainMenu.Text += " | <a href='../nsigoodies.aspx' class='bold white " + ((SelectedMenu == "incash") ? "mainmenuselected" : "mainmenu") + "'>Тов. ост. минихолдинга</a>";

            blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
            blockMainMenu.Text += " | <a href='../msg.aspx' class='bold white " + ((SelectedMenu == "msg") ? "mainmenuselected" : "mainmenu") + "'>Отправить сообщение</a>";

            if (iam.IsSuperAdmin)
            {
                blockAdminMenu.Text += "<a href='../admin/admin_partner.aspx' class='bold white " + ((SelectedMenu == "subjects") ? "mainmenuselected" : "mainmenu") + "'>Контрагенты</a>";
                blockAdminMenu.Text += " | <a href='../admin/admin_usr.aspx' class='bold white " + ((SelectedMenu == "users") ? "mainmenuselected" : "mainmenu") + "'>Пользователи</a>";
                blockAdminMenu.Text += " | <a href='../admin/admin_syssetting.aspx' class='bold white " + ((SelectedMenu == "consts") ? "mainmenuselected" : "mainmenu") + "'>Константы</a>";
                blockAdminMenu.Text += " | <a href='../admin/admin.aspx' class='bold white " + ((SelectedMenu == "setup") ? "mainmenuselected" : "mainmenu") + "'>Настройки</a>";
                blockAdminMenu.Text += " | <a href='../admin/admin_sysmsg.aspx' class='bold white " + ((SelectedMenu == "sysmsg") ? "mainmenuselected" : "mainmenu") + "'>Сист.сообщения</a>";
            }
        }

        
    }
}