using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;


    public partial class BreadPath : System.Web.UI.UserControl
    {
        private string _titles { get { return "" + ViewState["titlespunct"]; } set { ViewState["titlespunct"] = value; } }
        private string _paths { get { return "" + ViewState["pathspunct"]; } set { ViewState["pathspunct"] = value; } }
        public void AddPuncts(string title, string linkpath)
        {
            string t = _titles;
            string p = _paths;
            cStr.AddUnique(ref t, (title != "") ? title : "..");
            cStr.AddUnique(ref p, (linkpath != "") ? linkpath : "default.aspx");
            _titles = t;
            _paths = p;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                string[] t = _titles.Split(',');
                string[] p = _paths.Split(',');
                if (_paths != "")
                {
                    bread_path.Text = "<div class='attributes bold'>";
                    for (int i = 0; i < Math.Min(p.Length, t.Length); i++)
                    {
                        bread_path.Text += "<span><a href='" + p[i] + "'>" + t[i] + "</a></span>";
                        bread_path.Text += "<span>&nbsp;»&nbsp;</span>";
                    }
                    bread_path.Text += "</div>";
                }
            }
        }
    }
