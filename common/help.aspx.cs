using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wstcp.common
{
    public partial class help : p_p
    {
       

        protected new void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender,e);
            if (Request["rgw"] != "fly")
            {
                Page.MasterPageFile = "~/common/mainpage.master";
                ((mainpage) this.Master).SelectedMenu = "help";
                ((mainpage) this.Master).VisibleLeftPanel = false;
                ((mainpage) this.Master).VisibleRightPanel = false;
            } 
            else
            {
                Page.MasterPageFile = "~/common/Preview.master";
            }
        }
    }
}