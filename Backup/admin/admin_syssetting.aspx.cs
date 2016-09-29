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
    public partial class admin_syssetting : p_p
    {
        protected string key
        {
            get
            {
                if (!IsPostBack)
                    ViewState["key"] = Request.QueryString["key"];
                return "" + ViewState["key"];
            }
            set { ViewState["key"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            check_auth();
            
            if (!iam.IsSuperAdmin)
            {
                Response.Write("<h2>Доступ запрещен</h2> <p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }
            // ----------------------------------------------------
            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "consts";
            }
            if ( !iam.IsSuperAdmin)
            {
                lbMessage.Text = "НЕ ДОСТАТОЧНО ПРАВ";
                return;
            }
            initCommandPanel();
            if (key=="")
            {
                                   
                    show_list();
               
            }

        }


        void initCommandPanel()
        {
            CommandPanel4List1.Event_NEW = new EventHandler(this.bntNew_Click);
            CommandPanel4List1.ButtonVisible_EDIT =
                CommandPanel4List1.ButtonVisible_DELETE =
                CommandPanel4List1.ButtonVisible_COPY = false;



            cmdProfileOrg.Event_SaveAndClose = new EventHandler(this.bntOK_Click);
            cmdProfileOrg.Event_Cancel = new EventHandler(this.btnCancel_Click);
        }




        void show_list()
        {
            MultiView1.SetActiveView(vList);
            DataTable dt = SysSetting.GetTable();
            dgList.DataSource = dt;
            dgList.DataBind();
        }


        protected void bntNew_Click(object sender, EventArgs e)
        {
            
            openProfile();
        }



        void openProfile()
        {
            MultiView1.SetActiveView(vOrgProfile);
            txName.Text = key;
            txDescr.Text = SysSetting.GetValue(key);            
        }

        protected void bntOK_Click(object sender, EventArgs e)
        {
            SysSetting.SetValue(txName.Text, txDescr.Text);
            btnCancel_Click(sender, e);
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            lbMessage.Text = "";

            show_list();
        }


        protected void dgList_SelectedIndexChanged1(object sender, EventArgs e)
        {
            key = dgList.SelectedItem.Cells[0].Text;
            dgList.SelectedIndex = -1;
            openProfile();
        }
    }
}