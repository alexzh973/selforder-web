using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using selforderlib;
namespace wstcp
{
    public partial class admin_owner : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            check_auth();
            if (!iam.IsSuperAdmin)
            {
                Response.Write("<h2>Доступ запрещен</h2> <p><a href='../default.aspx'>вернуться на Главную</a></p>");
                Response.End();
                return;
            }
            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "owners";
                load_owners_dg();
            }

        }

        protected void load_owners_dg()
        {
            mv.SetActiveView(vList);
            DataTable dt = Owner.GetTable();
            dgOwners.DataSource = dt;
            dgOwners.DataBind();

        }



        protected void dgOwners_EditCommand(object source, DataGridCommandEventArgs e)
        {
            openProfile(cNum.cToInt(e.Item.Cells[0].Text));
        }

        protected void txText_PreRender(object sender, EventArgs e)
        {
            txText.Attributes.Add("name", "txText");
        }











        protected void btnNewOwner_Click(object sender, EventArgs e)
        {
            openProfile(0);
        }

        private void openProfile(int id)
        {
            mv.SetActiveView(vProfile);
            Owner rec = new Owner(id);
            txID.Text = "" + rec.ID;
            txName.Text = rec.Name;
            txAddress.Text = rec.Address;
            txDefaultTA.Text = rec.DefaultTA;
            txPhones.Text = rec.Phones;
            txText.Text = rec.StaticInfo;
            txWorktime.Text = rec.Timework;
            txInvoiceHead.Text = rec.InvoiceHead;
            txInvoiceFoot.Text = rec.InvoiceFoot;
            txInvoiceRekv.Text = rec.InvoiceRekv;
            chAutoActivate.Checked = rec.AutoActivatedUser;
            chShowIncash.Checked = rec.ShowIncash;
            chShowInvDiscount.Checked = rec.ShowInvDiscount;
            chUseSmsAuthorization.Checked = rec.UseSmsAuthorization;
        }














        protected void btnOK1_Click(object sender, EventArgs e)
        {
            Owner rec = new Owner(cNum.cToInt(txID.Text));

            rec.Name = txName.Text;
            rec.Address = txAddress.Text;
            rec.DefaultTA = txDefaultTA.Text;
            rec.Phones = txPhones.Text;
            rec.StaticInfo = txText.Text;
            rec.Timework = txWorktime.Text;
            rec.ShowIncash = chShowIncash.Checked;
            rec.AutoActivatedUser = chAutoActivate.Checked;
            rec.ShowInvDiscount = chShowInvDiscount.Checked;
            rec.InvoiceHead = txInvoiceHead.Text;
            rec.InvoiceFoot = txInvoiceFoot.Text;
            rec.InvoiceRekv = txInvoiceRekv.Text;
            rec.UseSmsAuthorization = chUseSmsAuthorization.Checked;

            if (Owner.Save(rec, iam.Email) != db.DbResult.OK)
            {
                lbMessageimport.Text = "Ошибка при сохранении";
            }
            else
            {
                Global.reload_adrs();
                load_owners_dg();
            }


        }

        protected void btnCancel1_Click(object sender, EventArgs e)
        {
            txID.Text = txName.Text = txAddress.Text = txDefaultTA.Text = txPhones.Text = txText.Text = txWorktime.Text = "";

            load_owners_dg();
        }

        protected void txInvoiceHead_PreRender(object sender, EventArgs e)
        {
            txInvoiceHead.Attributes.Add("txInvoiceHead", "txInvoiceHead");
        }
        protected void txInvoiceFoot_PreRender(object sender, EventArgs e)
        {
            txInvoiceFoot.Attributes.Add("txInvoiceFoot", "txInvoiceFoot");
        }
    }
}


