using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;

namespace wstcp
{
    public partial class admin_sysmsg : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!iam.IsSuperAdmin)
            {
                Response.Write("<h2>Доступ запрещен</h2><p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }
            // ----------------------------------------------------
            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "sysmsg";
            }
            if (!IsPostBack)
            {
                show_list();
            }
        }

        private void show_list()
        {
            DataTable dt = db.GetDbTable("select * from msg where typemsg='s' order by id desc");
            dgList.DataSource = dt;
            dgList.DataBind();
        }

        protected void dgList_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = cNum.cToInt( e.Item.Cells[0].Text);
            db.ExecuteCmd("update msg set state='D', lc='" + iam.Name + "', lcd=getdate() where id=" + id);
            show_list();
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (txSysmsg.Text.Trim() != "")
            {
                db.ExecuteCmd("insert into msg (msgtxt,state,lc,lcd,typemsg) values ('"+txSysmsg.Text+"','','" + iam.Name + "', getdate(),'s')");
                txSysmsg.Text = "";
                show_list();
            }
        }
    }
}