using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using wstcp.Models;

namespace wstcp
{
    public partial class gdetail : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0)
            {
                Response.Write("Запрещено без авторизации");
                Response.End();
                return;
            }
            if (!IsPostBack)
            {

                if ("" + Request["id"] != "") 
                    show_detail(cNum.cToInt(Request["id"]), cNum.cToInt(Request["ownid"]));
            }
        }

        
        void show_detail(int good_id, int owner_id)
        {
            GoodOwnInfo gsumm = GoodInfo.GetInfo(good_id, 0);
            if (""+Request["v"] != "small")
            {
                lbSelectedGood.Text = gsumm.Name;

                lbDetail.Text = "<div>бренд: " + gsumm.Brend + "</div><div>категория: " + gsumm.TN+"/"+gsumm.TK  + "</div><div>код: " + gsumm.Code + "</div><div>код енс: " + gsumm.ENS + "</div>";
            }
            
            dgIncash.DataSource = GoodInfo.GetIncashTable(good_id);
            dgIncash.DataBind();
            
        }

        protected void dgIncash_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                e.Item.CssClass = "";
                if (cNum.cToDecimal(r["Qty"]) != 0)
                {
                    e.Item.CssClass = "bold";
                }
                if ("" + r["zn_z"] == "NL" || "" + r["zn_z"] == "P2" || "" + r["zn_z"] == "Pz")
                    e.Item.CssClass += " red";
            }

        }
    }
}