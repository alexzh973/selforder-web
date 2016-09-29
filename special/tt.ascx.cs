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
    public partial class tt : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = db.GetDbTable("select top 5 * from good");
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
}