using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using System.Data.SqlClient;


namespace wstcp
{
    public partial class login : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divMessage.Text = "";

            if (!IsPostBack)
            {
                if ("" + Request.QueryString["act"] == "exit")
                {
                    IAM.ClearMySession(IAM.GetMe(Session.SessionID), Session.SessionID);
                    HttpCookie uc = Request.Cookies["usrcook"];
                    if (uc != null)
                    {
                        uc.Value = "";
                        Response.Cookies.Add(uc);
                    }
                    show_login_form(true);
                    //Response.Redirect("../default.aspx", true);
                }
                //check_connect_db();
                if ("" + Request.QueryString["act"] == "new")
                {
                    if (iam.ID >= 100000)
                        Response.Redirect("login.aspx?act=exit", true);
                    else
                        MultiView1.SetActiveView(vNewUser);
                }
                else
                {

                    show_login_form(true);
                    MultiView1.SetActiveView(vLinks);
                }
            }
        }
        //void check_connect_db()
        //{
        //    SqlConnection cn = db.GetSqlConnection(db.DefaultCnName);
        //    try
        //    {
        //        cn.Open();
        //        cn.Close();
        //        divMessage.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        divMessage.Text = "ошибка соединения с БД. "+ex.ToString();
        //    }
        //    finally
        //    {
        //        if (cn.State == ConnectionState.Open) cn.Close();
        //    }
        //}


        void show_login_form(bool visible)
        {
            login_div.Visible = visible;
            txPwd.Text = "";
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lbLogin.Text = "";

            IAM.ClearMySession(IAM.GetMe(Session.SessionID), Session.SessionID);

            IAM _iam = IAM.Login(txEmail.Text, txPwd.Text, Session.SessionID, Request.ServerVariables["REMOTE_ADDR"]);
            if (_iam != null && _iam.ID > 0)
            {
                HttpCookie mc = new HttpCookie("usrcook", txEmail.Text + "#" + txPwd.Text);
                mc.Expires = DateTime.Now.AddDays(33);
                Response.Cookies.Add(mc);
            }
            show_login_form(_iam == null);
            //if (_iam != null && _iam.ID > 0) Server.Transfer(portal_pages.GetPreviousPage(_iam), false);
            if (_iam != null && _iam.ID > 0)
            {

                Server.Transfer((p_p.GetPreviousPage(_iam) == "") ? "../default.aspx" : p_p.GetPreviousPage(_iam), false);
            }
            else
            {
                lbLogin.Text = "авторизация не прошла.";
                //captcha.ResetCapchaText();
            }
        }

        protected void linkRememberPsw_Click(object sender, EventArgs e)
        {
            if (txEmail.Text.IndexOf("@") == -1)
                return;
            DataRow rr = db.GetDbRow("select convert(nvarchar,pass) as pass from " + pUser.TDB + " where email='" + txEmail.Text + "'");
            if (rr != null && rr["pass"] != null)
            {
                EmailMessage email = new EmailMessage(CurrentCfg.EmailSupport, txEmail.Text, "Портал: информация о забытом пароле", "Ваш пароль: " + rr["pass"]);
                email.Send();
            }
            lbSendMessage.Text = "забытый пароль выслан на почту " + txEmail.Text;
            MultiView1.SetActiveView(vSendMessage);
        }
        protected void linkQueryNewUser_Click(object sender, EventArgs e)
        {
            //webOrg.LoadOrgs(dlNewUserOrg, 0, false, null);
            MultiView1.SetActiveView(vNewUser);
        }
        protected void btnNewCancel_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(vLinks);
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(vLinks);
        }
        protected void btnNewOK_Click(object sender, EventArgs e)
        {

            if (txNewName.Text.Trim() == "")
            {
                RequiredFieldValidator2.Visible = true;
                return;
            }
            if (txNewEmail.Text.Trim() == "" )
            {
                RequiredFieldValidator5.Visible = true;
                return;
            }
            if (pUserInfo.Exist(txNewEmail.Text))
            {
                RequiredFieldValidator5.Visible = true;
                RequiredFieldValidator5.Text = "пользователь с таким email уже зарегистрирован на сайте";
                return;
            }

           //if (!CaptchaControl1.RightInput)
            {
                lbCapchaMessage.Visible = true;
                return;
            }


            
           
            pUser usr = new pUser(0, iam);
            usr.Name = txNewName.Text;
            usr.Email = txNewEmail.Text;
            
            usr.LoginEnabled = false;

           bool res = pUser.Save(usr, iam);

           if (res)
           {
               //сообщение администратору
               EmailMessage email = new EmailMessage(CurrentCfg.EmailPortal, CurrentCfg.EmailSupport, HelpersPath.RootUrl + ": запрос на регистрацию нового пользователя", "<p>Новый пользователь:</p><p>имя: " + usr.Name + "</p><p>e-mail: " + usr.Email + "</p><p>ссылка на профайл: <a href='" + HelpersPath.RootUrl + "/ADMIN/admin_usr.aspx?id=" + usr.ID + "'>открыть...</a></p>");
               email.Send();

               // сообщение новому пользователю
               lbSendMessage.Text = "<p>Данные отправлены администратору сайта.</p><p>В течение 2-3х часов Вам будет выслано письмо с паролем для входа.</p>";
               email = new EmailMessage(CurrentCfg.EmailPortal, usr.Email, "Ваш запрос на регистрацию в " + HelpersPath.RootUrl, lbSendMessage.Text);
               email.Send();
               MultiView1.SetActiveView(vSendMessage);
           }

        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx", true);
        }
    }
}