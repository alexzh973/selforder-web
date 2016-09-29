using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


    public partial class mediaBrowser : System.Web.UI.UserControl
    {
        public string TEXTAREANAME
        {
            set { ViewState["TEXTAREANAME"] = value; }
            get { return ("" + ViewState["TEXTAREANAME"]=="")?"TEXTAREANAMETAG":"" + ViewState["TEXTAREANAME"]; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            btnInsert.Text = "<input type=\"button\" value=\"выбрать\" onclick=\"insertTag('" + TEXTAREANAME + "')\" />";
        }
    }
