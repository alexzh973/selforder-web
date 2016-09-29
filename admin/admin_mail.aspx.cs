using ensoCom;
using selforderlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using wstcp.models;

namespace wstcp
{
    public partial class admin_mail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string list = "";
            foreach (sPerson p in sPerson.ListPersons())
            {
                cStr.Add(ref list, p.Email,';'); 
            }
            txBcc.Text = list;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            EmailMessage m = new EmailMessage("alexzh@santur.ru", "alexzh@santur.ru", txTitle.Text, txMsg.Text);
            m.Send(txBcc.Text);
        }
    }
}