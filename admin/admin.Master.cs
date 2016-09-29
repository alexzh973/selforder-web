using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using wstcp.order;

namespace wstcp
{
    public partial class adminmaster : System.Web.UI.MasterPage
    {
        public string SelectedMenu
        {
            set { ViewState["menupunct"] = value; }
            get { if ("" + ViewState["menupunct"] == "")SelectedMenu = "home"; return "" + ViewState["menupunct"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lbLastTik.Text = Global.LastTik.ToShortDateString() + " " + Global.LastTik.ToShortTimeString();
            IAM iam = IamServices.GetIam(Session.SessionID);
            if (!iam.IsAdmin && !iam.IsSuperAdmin)
            {
                Response.Redirect("~/default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                paint_top_menu(Request.Url.PathAndQuery);
            }
        }

        private void paint_top_menu(string p)
        {
                lbTopMenu.Text = "";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "owners") ? "active" : "") + "'><a href='../admin/admin_owner.aspx'>Владельцы</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "subjects") ? "active" : "") + "'><a href='../admin/admin_partner.aspx'>Контрагенты</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "users") ? "active" : "") + "'><a href='../admin/admin_usr.aspx'>Пользователи</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "orders") ? "active" : "") + "'><a href='../admin/admin_orders.aspx'>Заказы</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "consts") ? "active" : "") + "'><a href='../admin/admin_syssetting.aspx'>Константы</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "setup") ? "active" : "") + "'><a href='../admin/admin.aspx'>Настройки</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "news") ? "active" : "") + "'><a href='../admin/admin_news.aspx'>Новости</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "import") ? "active" : "") + "'><a href='../admin/admin_import.aspx'>Импорт</a></li>";
                lbTopMenu.Text += "<li class='" + ((SelectedMenu == "sysmsg") ? "active" : "") + "'><a href='../admin/admin_sysmsg.aspx'>Сист.сообщения</a></li>";
                lbTopMenu.Text += "";
        }
    }
}