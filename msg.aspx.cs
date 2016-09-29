using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;

namespace wstcp
{
    public partial class msg : p_p
    {
        protected string _RGW_
        {
            get { return "" + ViewState["RGW"]; }
            set { ViewState["RGW"] = value; }
        }

        protected new void Page_PreInit(object sender, EventArgs e)
        {
           

            base.Page_PreInit(sender, e);
            if (Request["rgw"] == "fly")
            {
                Page.MasterPageFile = "~/common/Preview.master";
            }
            else
            {
                Page.MasterPageFile = "~/common/Mainpage.master";
                ((mainpage)this.Master).SelectedMenu = "msg";
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _ACT_ = "ghbd";
                _RGW_ = Request["rgw"];
                if (iam.ID <= 0)
                {
                    lbMessage.Text = "Сообщения разрешено отправлять только зарегистрированным пользователям.";
                    pnlMsg.Visible = false;
                }
                else
                {
                    lbMessage.Text = "";
                    pnlMsg.Visible = true;
                    chTosupport.Checked = true;
                    if (iam.SubjectID > 0)
                    {
                        Subject subj = new Subject(iam.SubjectID, iam);
                        lbtaemail.Text = subj.EmailTAs;
                    }
                    else
                    {
                        chToTa.Visible = false;
                    }
                    txAny.Visible = iam.IsSuperAdmin;
                }
            }
        }

        protected void btnCancel_PreRender(object sender, EventArgs e)
        {
            if (_RGW_ == "fly")
                btnCancel.OnClientClick = "closethis()";
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (txText.Text.Trim().Length < 5)
            {
                lbMess.Text = "<div class='message'>Пустое сообщение! не будет отправлено.</div>";
                return;
            }
            if (!chTosupport.Checked && (!chToTa.Visible || !chToTa.Checked || lbtaemail.Text.Trim() == ""))
            {
                lbMess.Text = "<div class='message'>Нет получателей! сооющение не будет отправлено.</div>";
                return;
            }
            lbMess.Text = "";
            string eto = "";
            eto += (chTosupport.Checked) ? CurrentCfg.EmailSupport : "";
            cStr.AddUnique(ref eto, (chToTa.Checked)?lbtaemail.Text:"");
            if (iam.IsSuperAdmin && txAny.Text!="")
                cStr.AddUnique(ref eto, txAny.Text);

            Subject s = new Subject(iam.ID, iam);
            EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, eto, "сообщение с портала santechportal.ru", txText.Text+"<div>[автор "+iam.Email+", "+iam.Name+" ("+s.Name+")]</div>");
            mess.Send();
            txText.Text = "";
            lbMess.Text = "<div class='message'><div>Сообщение было отправлено</div>";
            if (_RGW_=="fly")
                lbMess.Text += "<div>закройте это окно</div>";
            else
              lbMess.Text +=  "<div><a href='default.aspx' >вернуться на главную страницу.</a></div>";
            lbMess.Text +=  "</div>";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txText.Text = "";
            if (_RGW_ == "fly")
                lbMess.Text += "<div>закройте это окно</div>";
            else
                Response.Redirect("~/default.aspx");//lbMess.Text = "<div class='message'><div><a href='default.aspx' >вернуться на главную страницу.</a></div></div>";

        }
    }
}