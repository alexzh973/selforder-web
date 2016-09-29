using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using wstcp.Models;

namespace wstcp.common
{
    public partial class gimail : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id = cNum.cToInt(Request["id"]);
                if (iam.SubjectID > 0)
                {
                    Subject s = new Subject(iam.SubjectID, iam);
                    hdS.Value = s.Name + ", инн " + s.INN + ", код 1С " + s.Code;
                    txTo.Text = s.EmailTAs;
                }
                if (id > 0)
                {
                    NSIGood good = new NSIGood(id);
                    txMess.Text = "Прошу перезвонить либо ответить по email по поводу номенклатуры: " + good.Name + ". ";
                }
                else
                {
                    txMess.Text = "Прошу перезвонить либо ответить по email.";
                }
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (txTo.Text != "" && txMess.Text != "")
            {
                EmailMessage mess = new EmailMessage(iam.Email, txTo.Text, "Запрос по номенклатуре","<div>"+txMess.Text+"</div><div>Запрос поступил от "+iam.Name+", "+iam.Email+" ("+hdS.Value+")</div>");
                mess.Send(CurrentCfg.EmailSupport);
                pnlForm.Visible = false;
                lbMess.Text = "Сообщение передано. С Вами свяжутся в ближайшее время.<br/><br/>Можно закрыть это окошко.";
            }
        }
    }
}