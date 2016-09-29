using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using wstcp.Models;
using ensoCom;
using System.Data;

namespace wstcp.order
{
    public partial class detailfly : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0)
            {
                Response.Write("Запрещено без авторизации");
                Response.End();
                return;
            }
            show_record(Request["id"]);
        }
        
        Order RECORD;
        private void show_record(string id)
        {
            RECORD = new Order(cNum.cToInt(id), iam);
            lbTitle.Text = RECORD.Name;
            lbAttr.Text = "№" + RECORD.ID + " от " + RECORD.RegDate.ToShortDateString();
            lbCode.Text = RECORD.Code;
            lbDescr.Text = RECORD.Descr;
            DataView dv = RECORD.ItemsDt.DefaultView;
            dv.Sort = "Name";
            dgItems.DataSource = dv;
            dgItems.DataBind();
            lbState.Text = RECORD.StateDescr;
        }

        protected void dgItems_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                e.Item.Cells.Clear();
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells[0].Attributes.Add("colspan", "4");
                e.Item.Cells[2].Attributes.Add("colspan", "2");
                e.Item.Cells[0].Text = "СУММА";
                e.Item.Cells[1].Text = ""+cNum.cToDecimal(RECORD.Summ, 2)+"руб.";
                
            }
        }
    }
}