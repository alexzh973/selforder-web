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
    public partial class workpage : System.Web.UI.MasterPage
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
            if (ensoCom.db.GetDbTable("select ID from " + GoodOwner.TDB + " where Name + ':' + CAST(ID AS nvarchar)='" + pwd + "'").Rows.Count > 0)
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

        
        public string SelectedMenu
        {
            set { ViewState["menupunct"] = value; }
            get { if (""+ViewState["menupunct"] == "")SelectedMenu = "home"; return ""+ViewState["menupunct"]; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!IsPostBack)
            {
                iam = IamServices.GetIam(Session.SessionID);
                paint_menu_2();
                
                if (iam.PresentedSubjectID > 0)
                {                   

                    
                    //Subject subj = new Subject(iam.PresentedSubjectID, iam);
                    show_personalTAs(iam.PresentedSubject);
                    show_subj_info(iam.PresentedSubject);

                }
                
            }
           
        }
        public void show_subj_info(Subject subj)
        {
            LoginStringInHeader.Reload();
            //lbCurrBlnc.Text = "";

            //lbSubject.Text = "<strong title='ИНН " + subj.INN + "'>" + subj.Name + "</strong>";

            //DGinfo d = Subject.GetDg(subj.ID, subj.CodeDG);

            //lbCurrBlnc.Text = "Баланс " + ((d.CurrentDZ >= 0) ? ((d.CurrentDZ == 0) ? "0.00руб" : "<span class='red'>-" + d.CurrentDZ + "руб</span>") : "<span class='green'>" + (d.CurrentDZ* -1) + "руб</span>");

        }
        public void show_personalTAs(Subject subj)
        {
            if (subj ==null || subj.ID == 0)
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
            

        //    blockMainMenu.Text += "<a href='../default.aspx' class='bold white " + ((SelectedMenu == "home") ? "mainmenuselected" : "mainmenu") + "'>Главная</a>";
        //    if (iam.ID < 100000)
        //    {
        //        blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
        //        return;
        //    }

        //    blockMainMenu.Text += " | <a href='../good/goodlist.aspx' class='bold white " + ((SelectedMenu == "order") ? "mainmenuselected" : "mainmenu") + "'>Самостоятельная заявка</a>";

            
        //    if (iam.SubjectID==0)
        //        blockMainMenu.Text += " | <a href='../nsigoodies.aspx' class='bold white " + ((SelectedMenu == "incash") ? "mainmenuselected" : "mainmenu") + "'>Тов. ост. минихолдинга</a>";

        //    blockMainMenu.Text += " | <a href='../common/help.aspx' class='bold white " + ((SelectedMenu == "help") ? "mainmenuselected" : "mainmenu") + "'>О программе</a>";
        //    blockMainMenu.Text += " | <a href='../msg.aspx' class='bold white " + ((SelectedMenu == "msg") ? "mainmenuselected" : "mainmenu") + "'>Отправить сообщение</a>";

            
        //}

        //private void paint_buttons()
        //{
        //    if (iam.ID < 100000) return;
        //    string btns = "";

        //    btns += "<a href='../order/orderdefault.aspx' class='btn-new-order' >Сделать заявку</a>";
        //    btns += "<li class='" + ((SelectedMenu == "msg") ? "selected" : "") + "'><a onclick=\"openflywin('../msg.aspx?rgw=fly', 600, 430)\" href='javascript:return 0;' class='btn-send-msg'>Отправить сообщение</a></li>";


        //}

        private void paint_menu_2()
        {
            if (iam==null)
                iam = IamServices.GetIam(Session.SessionID);
            blockMainMenu.Text = "";
            
            blockMainMenu.Text += "<ul >";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "home") ? "selected" : "") + "'><a href='../default.aspx' >Главная</a></li>";
            if (iam.ID >= 100000)
                blockMainMenu.Text += "<li class='bold white " + ((SelectedMenu == "order") ? "selected" : "") + "'><a href='../order/orderdefault.aspx' >Заявка</a></li>";
            if (iam.ID >= 100000 && iam.SubjectID == 0)
                blockMainMenu.Text += "<li class='bold white " + ((SelectedMenu == "incash") ? "selected" : "") + "'><a href='../nsigoodies.aspx'>Тов. ост. минихолдинга</a></li>";
            //if (iam.ID >= 100000)
            //    blockMainMenu.Text += "<li class='" + ((SelectedMenu == "msg") ? "selected" : "") + "'><a onclick=\"openflywin('../msg.aspx?rgw=fly', 600, 430)\" href='javascript:return 0;'>Отправить сообщение</a></li>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "sales") ? "selected" : "") + "'><a href='../good/sales.aspx'>Акции</a></li>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "lk") ? "selected" : "") + "'><a href='../account/lk.aspx'>Личный кабинет</a></li>";
            blockMainMenu.Text += "<li class='" + ((SelectedMenu == "help") ? "selected" : "") + "'><a  onclick=\"openflywin('../common/help.aspx?rgw=fly', 900, 700)\" href='javascript:return 0;'>Инструкция</a></li>";
            if (iam.IsSuperAdmin) 
                blockMainMenu.Text += "<li class='" + ((SelectedMenu == "admin") ? "selected" : "") + "'><a href='../admin/admin.aspx'>admin</a></li>";
            blockMainMenu.Text += "</ul>";
        }

        protected void btnQuit_Click(object sender, EventArgs e)
        {
            if (iam == null) iam = IamServices.GetIam(Session.SessionID);
            IamServices.ClearSession(iam, iam.SessionID);
            iam = IamServices.GetIam(Session.SessionID);
            paint_menu_2();            
            
                
                //rpTAs.Visible = false;
            
        }

        
    }
}