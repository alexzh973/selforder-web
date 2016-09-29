using System.Management;
using ensoCom;
using System;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wstcp.special
{
    public partial class test1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            bool infoens = false;
            foreach (DataRow r in db.GetDbTable("select top 5 id, ens from good where isnull(ens,'')<>'' and ens in ('0103-00616','0105-00342','0103-00030')").Rows)
            {
                infoens = wstcp.special.ensinfo.SaveImg(r["ens"].ToString(),@"../media/gimg");
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

            WqlObjectQuery q = new WqlObjectQuery(TextBox1.Text);
            ManagementObjectSearcher find = new ManagementObjectSearcher(q);
            foreach (ManagementObject mo in find.Get())
            {
                TextBox1.Text += mo["Description"]+" | "+mo["FileSystem"];
            }
            //GridView1.DataSource = find.Get();
            //GridView1.DataBind();
        }

        
    }
}