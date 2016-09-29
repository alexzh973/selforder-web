using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

namespace wstcp.special
{
    public partial class ajtest : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = db.GetDbTable("select distinct brend as val from good inner join owng on owng.goodid=goodid where ownerid=" + iam.OwnerID);
                foreach (DataRow r in dt.Select("", "val"))
                {
                    dlBrend.Items.Add(new ListItem(""+r[0],""+r[0]));
                }
            }
        }

        

        protected void Button1_Click(object sender, EventArgs e)
        {
            dlBrend.SelectedValue = GetRequestByControlId(dlBrend.ClientID);
            dlCat.SelectedValue = GetRequestByControlId(dlCat.ClientID);
            dlName.SelectedValue = GetRequestByControlId(dlName.ClientID);
        }

 

        
    }
}