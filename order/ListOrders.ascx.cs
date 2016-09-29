using ensoCom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using wstcp.Models;

namespace wstcp
{
    public partial class ListOrders : uc_base
    {
        public int SubjectID
        {
            get { return cNum.cToInt(ViewState["SID"]); }
            set { ViewState["SID"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        public void Show_orders()
        {
            if (SubjectID <= 0)
            {
                return;
            }
            string sql;
            sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder, subj.Name as SubjectName from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where ord.SubjectID=" + SubjectID + "";

            DataTable dt = db.GetDbTable(sql);

            DataTable dtNU = db.GetDbTable("select 0 as id, '' as Name, '' as RegDate, '' as State, '' as SubjectName, '' as SummOrder, '' as linkchange, '' as Descr, '' as css");
            dtNU.Rows.Clear();

            DataTable dtAZ = dtNU.Copy();
            DataTable dtS = dtNU.Copy();
            DataTable dtMR = dtNU.Copy();
            DataTable dtF = dtNU.Copy();
            DataTable dtDX = dtNU.Copy();

            foreach (DataRow r in dt.Select("State in ('','N','U')"))
                fill_dt(r, dtNU);
            foreach (DataRow r in dt.Select("State in ('A','Z')"))
                fill_dt(r, dtAZ);
            foreach (DataRow r in dt.Select("State in ('M','R')"))
                fill_dt(r, dtMR);
            foreach (DataRow r in dt.Select("State in ('S')"))
                fill_dt(r, dtS);
            foreach (DataRow r in dt.Select("State in ('D','X')"))
                fill_dt(r, dtDX);
            foreach (DataRow r in dt.Select("State in ('F')"))
                fill_dt(r, dtF);

            

            rpNU.Visible = (dtNU.Rows.Count > 0);
            rpAZ.Visible = (dtAZ.Rows.Count > 0);
            rpS.Visible = (dtS.Rows.Count > 0);
            rpMR.Visible = (dtMR.Rows.Count > 0);
            rpF.Visible = (dtF.Rows.Count > 0);
            rpDX.Visible = (dtDX.Rows.Count > 0);
            if (dtNU.Rows.Count > 0)
            {
                rpNU.DataSource = dtNU;
                rpNU.DataBind();
            }

            if (dtAZ.Rows.Count > 0)
            {
                rpAZ.DataSource = dtAZ;
                rpAZ.DataBind();
            }

            if (dtS.Rows.Count > 0)
            {
                rpS.DataSource = dtS;
                rpS.DataBind();
            }
            if (dtMR.Rows.Count > 0)
            {
                rpMR.DataSource = dtMR;
                rpMR.DataBind();
            }
            if (dtDX.Rows.Count > 0)
            {
                rpDX.DataSource = dtDX;
                rpDX.DataBind();
            }
            if (dtF.Rows.Count > 0)
            {
                rpF.DataSource = dtF;
                rpF.DataBind();
            }
        }

        void fill_dt(DataRow fromR, DataTable forNewRow)
        {
            DataRow nr = forNewRow.NewRow();
            nr["id"] = fromR["id"];
            nr["Name"] = fromR["Name"];
            nr["Descr"] = fromR["Descr"];
            nr["RegDate"] = cDate.cToDate(fromR["RegDate"]).ToShortDateString();
            nr["SubjectName"] = fromR["SubjectName"];
            nr["State"] = Order.get_stateorder_descr("" + fromR["State"]);
            nr["SummOrder"] = cNum.cToDecimal(fromR["SummOrder"], 2);
            nr["linkchange"] = "<a href='../order/orderdefault.aspx?id=" + fromR["id"] + "&act=edit'>изменить</a>";
            nr["css"] = (cStr.Exist(("" + fromR["State"]).ToUpper(), new string[] { "U", "S", "R" })) ? "bold" : "";
            forNewRow.Rows.Add(nr);
        }

        protected void rpNU_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        
    }
}