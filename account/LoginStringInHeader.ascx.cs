using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using wstcp.order;

namespace wstcp
{
    public partial class LoginStringInHeader : uc_base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            {
                
                Reload();

            }

        }

        public void Reload()
        {
            spanUserName.Text = iam.Name;
                if (iam.ID > 0)
                {
                    MultiView1.SetActiveView(logined);
                    spanUserName.Attributes.Add("onclick", "openflywin('../account/card.aspx?id=" + iam.ID + "', 600, 600, '" + iam.Name + "')");
                    if (iam.ID >= 100000 && iam.PresentedSubjectID > 0)
                    {
                        lbSubject.Text = iam.PresentedSubject.Name;
                        lbSubject.ToolTip = iam.PresentedSubject.INN;
                        DGinfo d = Subject.GetDg(iam.PresentedSubject.ID, iam.PresentedSubject.CodeDG);

                        lbCurrBlnc.Text = "Баланс " + ((d.CurrentDZ >= 0) ? ((d.CurrentDZ == 0) ? "0.00руб" : "<span class='red'>-" + d.CurrentDZ + "руб</span>") : "<span class='green'>+" + (d.CurrentDZ*-1) + "руб</span>");

                    }
                } 
                else
                {
                    MultiView1.SetActiveView(logoffed);
                    spanUserName.Text = lbSubject.Text = lbCurrBlnc.Text = "";
                    lbSubject.ToolTip = "";
                    spanUserName.Attributes.Remove("onclick");

                }
        }
    }
}