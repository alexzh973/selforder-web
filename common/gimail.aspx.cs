using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;

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
                txMess.Text = get_handline();
                if (id > 0)
                {
                    Good good = new Good(id);
                    txMess.Text += " по поводу номенклатуры: " + good.Name + ". ";
                }
                
            }
        }

        private string get_handline()
        {
            pUser u = new pUser(iam.ID, iam);
            return "Прошу ответить по email "+iam.Email + ((u.Phones!="")?" либо перезвонить по тел. "+u.Phones:"");
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