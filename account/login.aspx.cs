using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using System.Data.SqlClient;
using selforderlib;


namespace wstcp
{
    public partial class login : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divMessage.Text = "";

            if (!IsPostBack)
            {
                _ACT_ = "" + Request.QueryString["act"];
                if (_ACT_ == "crashpsw")
                {

                }
                if (_ACT_ == "exit")
                {
                    IamServices.ClearSession(iam, Session.SessionID);
                    HttpCookie uc = Request.Cookies["usrcook"];
                    if (uc != null)
                    {
                        uc.Value = "";
                        Response.Cookies.Add(uc);
                    }
                    show_login_form(true);
                    Response.Redirect("../default.aspx", true);
                }
                //check_connect_db();
                if (_ACT_ == "new")
                {
                    dgFinded.Visible = false;

                    if (iam.ID >= 100000 && iam.IsSaller)
                        show_newuser_form();
                    else if (iam.ID >= 100000)
                        Response.Redirect("login.aspx?act=exit", true);
                    else
                        show_newuser_form();
                }
                else
                {

                    show_login_form(true);
                    MultiView1.SetActiveView(vLinks);
                }
            }
        }

        private void show_newuser_form()
        {
            MultiView1.SetActiveView(vNewUser);
            webInfo.LoadOwners(dlOwners, "" + iam.OwnerID, false, iam);
            if (iam.IsSaller)
            {
                fieldseller.Visible = true;
                txEmailTAs.Text = iam.Email;

                lbFIO.Text = "ФИО нового контактного лица от клиента";
                lbEmail.Text = "Email нового контактного лица от клиента";
                lbPhones.Text = "Телефоны нового контактного лица от клиента";
                lbFIO.CssClass = "";
                lbEmail.CssClass = "";
            }
        }

        protected void lbtnDemo_Click(object sender, EventArgs e)
        {
            txEmail.Text = "z2@sango.ru";
            txPwd.Text = "123456";
            btnLogin_Click(sender, e);
        }

        void show_login_form(bool visible)
        {
            login_div.Visible = visible;
            txPwd.Text = "";
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lbLogin.Text = "";

            IamServices.ClearSession(iam, Session.SessionID);

            IAM _iam = IamServices.Login(txEmail.Text.Trim(), txPwd.Text.Trim(), Session.SessionID, Request.ServerVariables["REMOTE_ADDR"]);
            if (_iam != null && _iam.ID > 0)
            {
                HttpCookie mc = new HttpCookie("usrcook", txEmail.Text.Trim() + "#" + txPwd.Text.Trim());
                mc.Expires = DateTime.Now.AddDays(33);
                Response.Cookies.Add(mc);
            }

            if (_iam != null && _iam.ID > 0)
            {

                Server.Transfer((p_p.GetPreviousPage(_iam) == "") ? "../default.aspx" : p_p.GetPreviousPage(_iam), false);
            }
            else
            {
                lbLogin.Text = "авторизация не прошла.";
            }
        }

        protected void linkRememberPsw_Click(object sender, EventArgs e)
        {
            if (txEmail.Text.Trim().Length<7)
            {
                RequiredFieldValidator1.Visible = true;
                return;
            }
            DataRow rr = db.GetDbRow("select convert(nvarchar,pass) as pass from " + pUser.TDB + " where email='" + txEmail.Text + "'");
            if (rr != null && rr["pass"] != null)
            {
                EmailMessage email = new EmailMessage(CurrentCfg.EmailSupport, txEmail.Text.Trim(), "Портал: информация о забытом пароле", "Ваш пароль: " + rr["pass"]);
                email.Send();
            }
            lbSendMessage.Text = "забытый пароль выслан на почту " + txEmail.Text.Trim();
            MultiView1.SetActiveView(vSendMessage);
        }
        protected void linkQueryNewUser_Click(object sender, EventArgs e)
        {
            show_newuser_form();
            //MultiView1.SetActiveView(vNewUser);
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
            string SubjectName = "";
            lbError.Text = "";
            int SubjectID = 0;
            if (txINN.Text.Length < 10 || cNum.cToDecimal(txINN.Text) >= 999999999999 || cNum.cToDecimal(txINN.Text) < 100000000)
            {
                lbError.Text = "Некорректно заведен ИНН";
                return;
            }
            if (txSubjName.Text.Trim() == "")
            {
                lbError.Text = "Некорректно заведено Наименование";
                return;
            }

            if (!CaptchaControl1.RightInput)
            {
                lbError.Text = "Непопали в капчу";
                return;
            }
            SubjectID = Subject.FindByField("INN", txINN.Text);
            //Subject existsubj = new Subject(,iam);
            if (SubjectID > 0 && iam.IsSaller)
            {

            }
            else
            {
                if (!cStr.ValidEmail(txNewEmail.Text))
                {
                    lbError.Text = "Некорректно заведен email";
                    return;
                }
            }


            if (txNewEmail.Text!="" && pUserInfo.Exist(txNewEmail.Text))
            {
                lbError.Text = "пользователь с таким email уже зарегистрирован на сайте";
                return;
            }


            //DataTable dt = db.GetDbTable("select ID, Name from Subj where INN='" + txINN.Text + "'");
            //if (dt.Rows.Count > 0)
            //{
            //    SubjectID = cNum.cToInt(dt.Rows[0][0]);
            //    SubjectName = ""+dt.Rows[0][1];
            //}




            Subject subj = new Subject(SubjectID, iam);
            bool exists = (subj.ID > 0);
            Owner own = (exists) ? new Owner(subj.OwnerID) : new Owner(cNum.cToInt(dlOwners.SelectedValue));
            if (subj.ID > 0)
            {
                txSubjName.Text = subj.Name;
            }
            else
            {
                subj.INN = txINN.Text;
                subj.OwnerID = own.ID;
                subj.Name = txSubjName.Text;
                subj.State = "N";
                subj.Address = "";

                subj.EmailTAs = (fieldseller.Visible && txEmailTAs.Text != "") ? txEmailTAs.Text : own.DefaultTA;//SysSetting.GetValue("emaildefaultta");
                Subject.Save(subj);
                
            }
            if (iam.IsSaller)
                {
                    Bind.Save(iam.ThisObject, subj.ThisObject, iam);
                }
            bool res = false;
            if (!exists || (txNewName.Text.Trim() != "" && txNewEmail.Text.Trim() != ""))
            {
                pUser usr = new pUser(0, iam, own.ID);
                usr.Name = txNewName.Text.Trim();
                usr.Email = txNewEmail.Text.Trim();
                usr.Phones = txPhones.Text.Trim();
                usr.Descr = "";
                usr.LoginEnabled = (iam.IsSaller) ? false : own.AutoActivatedUser;
                usr.SubjectID = subj.ID;

                res = pUser.Save(usr);
                pUser adm = new pUser(own.AdminId, iam);

                if (res)
                {
                    //сообщение администратору
                    EmailMessage email = new EmailMessage(CurrentCfg.EmailPortal, adm.Email, HelpersPath.RootUrl + ": запрос на регистрацию нового пользователя", "<p>Новый пользователь:</p><p>имя: " + usr.Name + "</p><p>e-mail: " + usr.Email + "</p><p>представитель предприятия: " + subj.Name + ", ИНН: " + subj.INN + "</p><p>ссылка на профайл: <a href='" + HelpersPath.RootUrl + "/ADMIN/admin_usr.aspx?id=" + usr.ID + "'>открыть...</a></p>");
                    email.Send();
                    if (usr.LoginEnabled)
                    {
                        IAM _iam = IamServices.Login(usr.Email, usr.Psw, Session.SessionID, Request.ServerVariables["REMOTE_ADDR"]);
                        if (_iam != null && _iam.ID > 0)
                        {
                            HttpCookie mc = new HttpCookie("usrcook", usr.Email + "#" + usr.Psw);
                            mc.Expires = DateTime.Now.AddMinutes(21);
                            Response.Cookies.Add(mc);
                        }

                        if (_iam != null && _iam.ID > 0)
                        {
                            Server.Transfer((p_p.GetPreviousPage(_iam) == "") ? "../default.aspx" : p_p.GetPreviousPage(_iam), false);
                        }

                    } else
                    {
                        lbSendMessage.Text = "<p>Данные отправлены администратору сайта.</p><p>В течение нескольких минут Вам будет выслано письмо с паролем для входа.</p>";
                    }

                    if (exists && usr.LoginEnabled)
                    {
                        // сообщение новому пользователю

                        //db.ExecuteCmd("update " + pUser.TDB + " set LoginEnabled='Y' where id=" + usr.ID);

                        email = new EmailMessage(
                            CurrentCfg.EmailPortal,
                            usr.Email,
                            "Регистрация на сайте Самостоятельной заявки " + HelpersPath.RootUrl,
                            "<p>Уважаемый(ая) " + usr.Name + "</p>" +
                            "<p>представитель предприятия: " + subj.Name + ", ИНН: " + subj.INN + "</p>" +
                            "<p>Ваш логин: " + usr.Email + "</p>" +
                            "<p>Ваш пароль: " + usr.Psw + "</p>" +
                            "<p>ссылка на сайт: <a href='" + HelpersPath.RootUrl + "/default.aspx'>" + HelpersPath.RootUrl + "</a></p>");

                        email.Send(adm.Email);

                        email = new EmailMessage(
                            CurrentCfg.EmailPortal,
                            "" + subj.EmailTAs,
                            HelpersPath.RootUrl + ": Регистрация нового пользователя",
                            "<p>Новый пользователь:</p>" +
                            "<p>имя: " + usr.Name + "</p><p>e-mail: " + usr.Email + "</p>" +
                            "<p>представитель предприятия: " + subj.Name + ", ИНН: " + subj.INN + "</p>" +
                            "<p>ссылка на сайт: <a href='" + HelpersPath.RootUrl + "/default.aspx'>" + HelpersPath.RootUrl + "</a></p>");
                        email.Send(adm.Email);
                    }
                    linkPrn.Text = "<a href='prnnew.aspx?id=" + usr.ID + "' target='_blank'>Распечатать</a>";
                    MultiView1.SetActiveView(vSendMessage);
                }
            } else
            {
                
                res = true;
            }
            if (iam.IsSaller)
            {
                lbSendMessage.Text = "<p>Прикрепление клиента произошло успешно. <a href='../default.aspx'>Вернуться на главную</a></p>";
                MultiView1.SetActiveView(vSendMessage);
            }
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx", true);
        }



        protected void lbtnFind_Click(object sender, EventArgs e)
        {
            if (txSubjName.Text.Trim().Length > 2 || txINN.Text.Length >= 4)
            {
                string sql = "select INN, NameSubj," + db.SqlNameField("OWN", "OwnerID", "Owner") + ", OwnerID from TTT where 1=1 " + ((txSubjName.Text.Length > 2) ? " and NameSubj like '%" + txSubjName.Text + "%'" : "") + ((txINN.Text.Length >= 4) ? " and inn like '%" + txINN.Text + "%'" : "") + "";
                DataTable dt = db.GetDbTable(sql);

                dgFinded.DataSource = dt;
                dgFinded.DataBind();
                dgFinded.Visible = (dt.Rows.Count > 0);
            }
            else
            {
                dgFinded.DataSource = null;
                dgFinded.DataBind();
                dgFinded.Visible = false;
            }

        }

        protected void dgFinded_SelectedIndexChanged(object sender, EventArgs e)
        {
            txINN.Text = dgFinded.SelectedRow.Cells[0].Text.Trim();
            txSubjName.Text = dgFinded.SelectedRow.Cells[1].Text.Trim().Replace("&quot;", " ").Replace("  ", " ");


            webInfo.LoadOwners(dlOwners, dgFinded.SelectedRow.Cells[3].Text, false, iam);
            dgFinded.SelectedIndex = -1;
            lbError.Text = "";
        }





        protected void dgFinded_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            string sql = "select INN, NameSubj , " + db.SqlNameField("OWN", "OwnerID", "Owner") + ", OwnerID from TTT where 1=1 " + ((txSubjName.Text.Length > 2) ? " and NameSubj like '%" + txSubjName.Text + "%'" : "") + ((txINN.Text.Length >= 4) ? " and inn like '%" + txINN.Text + "%'" : "") + "";
            DataTable dt = db.GetDbTable(sql);
            //DataTable dt = db.GetDbTable("select INN, NameSubj from TTT where NameSubj like '%" + txSubjName.Text + "%' " + ((txINN.Text.Length >= 4) ? " or inn like '%" + txINN.Text + "%'" : ""));

            dgFinded.DataSource = dt;
            dgFinded.PageIndex = e.NewPageIndex;
            dgFinded.DataBind();
        }

    }
}