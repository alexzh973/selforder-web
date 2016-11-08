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
    public partial class admin_remember : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            int rem = cNum.cToInt(Request["rem"]);
            if (rem > 0)
            {
                pUser uu = new pUser(rem, iam);
                EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, uu.Email, "santechportal.ru: Напоминалка пароля", "<p>Уважаемый(ая) " + uu.Name + ".</p><p>Вероятно, по Вашей просьбе вам отправлена информация о Вашем пароле доступа в santechportal.ru.</p><p>Ваш пароль: <strong style='font-family: Courier New'>" + uu.Psw + "</strong></p><p>напоминаем Ваш логин: <strong>" + uu.Email + "</strong></p><br/><br/><p>ссылка для входа в портал <a href='http://santechportal.ru'>http://santechportal.ru</a></p>");
                mess.Send();
                lbMessage.Text = "на электронный адрес " + uu.Email + " отправлено напоминание о пароле.";
            } else
            {
                lbMessage.Text = "не выбран пользователь, которому нужно отправить напоминание";
            }
        }
    }
}