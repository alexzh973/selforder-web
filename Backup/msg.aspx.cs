using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

namespace wstcp
{
    public partial class msg : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "msg";

                lbsupportemail.Text = CurrentCfg.EmailSupport;
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
            }
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
            eto += (chTosupport.Checked)?""+lbsupportemail.Text:"";
            cStr.AddUnique(ref eto, (chToTa.Checked)?lbtaemail.Text:"");

            EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, eto, "сообщение с портала santechportal.ru", txText.Text);
            mess.Send();
            txText.Text = "";
            lbMess.Text = "<div class='message'><div>Сообщение было отправлено</div><div><a href='default.aspx' >вернуться на главную страницу.</a></div></div>";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            txText.Text = "";
            lbMess.Text = "<div class='message'><div><a href='default.aspx' >вернуться на главную страницу.</a></div></div>";

        }
    }
}