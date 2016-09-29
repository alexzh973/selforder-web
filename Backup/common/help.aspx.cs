using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wstcp.common
{
    public partial class help : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((mainpage)this.Master).SelectedMenu = "help";
            ((mainpage)this.Master).VisibleLeftPanel = false;
            ((mainpage)this.Master).VisibleRightPanel = false;
        }
    }
}