using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

namespace wstcp.order
{
    public partial class testflt : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           loadFilter(); 
            
        }

        private void loadFilter()
        {
            if (txTK.Text == "") return;
            DataTable dtchrs = db.GetDbTable("select keychars from imgtntk where t='tk' and name='" + txTK.Text + "'");

            string[] chrs = ("" + dtchrs.Rows[0][0]).Split(',');
            int i = 0;
            foreach (string chr in chrs)
            {
                DropDownList dl = new DropDownList();
                dl.ClientIDMode = ClientIDMode.Static;
                dl.Attributes.Remove("ID");
                dl.Attributes.Add("ID","dl"+(i++));
                dl.Attributes.Add("data", chr);
                dl.Items.Add(new ListItem("", ""));
                foreach (DataRow r in db.GetDbTable("select ChrVal, count(*) as q  from GOODCH where Chr='" + chr + "' and GoodCode in ( select GoodCode from vGood" + dlOwner.SelectedValue + " where xTK='" + txTK.Text + "' ) group by ChrVal order by ChrVal").Rows)
                {
                    dl.Items.Add(new ListItem("" + r["ChrVal"] + " (" + r["q"] + ")", "" + r["ChrVal"]));
                }
                dl.AutoPostBack = true;
                dl.SelectedIndexChanged += new EventHandler(selfilter_Change);
                pnlFilter.Controls.Add(dl);
            }

        }
        protected void dlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            //loadFilter();
        }
        private void selfilter_Change(object sender, EventArgs e)
        {
            loadGoods();
        }

        private void loadGoods()
        {
            string sql = "select top 10 * from vGOOD" + dlOwner.SelectedValue + " where xTK='" + txTK.Text + "' and GoodCode in (select goodcode from GOODCH where #WHERE#)";
            string where = "1=1";

            foreach (Control dl in pnlFilter.Controls)
            {
                if (dl.GetType() == typeof(DropDownList) && ((DropDownList)dl).SelectedValue.Trim() != "" && ((DropDownList)dl).Attributes["data"]!="")
                {
                    where += String.Format("and GoodCode in (select goodcode from GOODCH where Chr='" + ((DropDownList)dl).Attributes["data"] + "' and ChrVal='{0}')", ((DropDownList)dl).SelectedValue);
                }
            }
            sql = sql.Replace("#WHERE#", where);
            DataTable dt = db.GetDbTable(sql);
            dgGoods.DataSource = dt;
            dgGoods.DataBind();
        }


    }
}