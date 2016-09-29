using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;

namespace wstcp.account
{
    public partial class prnnew : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pUser usr = new pUser(cNum.cToInt(Request["id"]), iam);
            lbLogin.Text = usr.Email;
            lbPsw.Text = usr.Psw;
            Subject subj = new Subject(usr.SubjectID,iam);
            lbSubjectName.Text = subj.Name;
        }
    }
}