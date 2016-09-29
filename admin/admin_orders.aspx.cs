using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;

namespace wstcp
{
    public partial class admin_orders : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!iam.IsSuperAdmin) Response.Redirect("~/default.aspx");

            if (!IsPostBack)
            {
                ((adminmaster)this.Master).SelectedMenu = "orders";
                load_subjects();
                load_tas();
                btnSearch_Click(null, null);
            }
        }

        private void load_tas()
        {
            dlTa.Items.Clear();
            dlTa.Items.Add(new ListItem("",""));
            foreach(DataRow r in db.GetDbTable("select count(ord.id) as qty,subj.EmailTAs,(select top 1 name from ENSOUSER where subj.EmailTAs like '%'+email+'%') as ta from ORD inner join subj on ord.SubjectID=subj.ID group by subj.EmailTAs").Select("","ta"))
            {
                dlTa.Items.Add(new ListItem("" + r["ta"]+" ("+r["qty"]+")", "" + r["EmailTAs"]));  
            }
        }

        private void load_subjects()
        {
            dlSubject.Items.Clear();
            dlSubject.Items.Add(new ListItem("", ""));
            foreach (DataRow r in db.GetDbTable("select count(ord.id) as qty,subj.id,subj.name from ORD inner join subj on ord.SubjectID=subj.ID group by subj.id,subj.Name").Select("", "name"))
            {
                dlSubject.Items.Add(new ListItem("" + r["name"] + " (" + r["qty"] + ")", "" + r["id"]));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string where = "";
            if (dlSubject.SelectedValue != "")
                where += " and ord.subjectId=" + dlSubject.SelectedValue;
            if (dlTa.SelectedValue != "")
                where += " and subj.emailtas='" + dlTa.SelectedValue+"'";
            if (rbSelf.SelectedValue == "s")
                where += " and authorid not in (select id from ensouser where name like '1с %')";
            else if (rbSelf.SelectedValue == "1c")
                where += " and authorid in (select id from ensouser where name like '1с %')";
            DataTable dt = db.GetDbTable("select ord.id, ord.Code, ord.Dg,ord.Name,ord.descr,ord.subjectId,subj.Name as SubjectName, subj.INN,subj.Code as SubjectCode,(select top 1 id from ENSOUSER where subj.EmailTAs like '%'+email+'%') as taid,(select top 1 name from ENSOUSER where subj.EmailTAs like '%'+email+'%') as taname, ord.regdate,ord.lc, ord.lcd,ord.summorder,ord.summbase,ord.state, ord.authorid," + db.SqlNameField(pUser.TDB, "ord.authorid", "authorname") + " from ORD inner join SUBJ on subj.id=ord.subjectid where 1=1 " + ((where != "") ? where : "") + " order by ord.id desc");
            dgList.DataSource = dt;
            dgList.DataBind();

        }

        private System.Data.DataRowView r;
        private int id;
        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                r = (System.Data.DataRowView) e.Item.DataItem;
                id = cNum.cToInt(r["ID"]);
                e.Item.Cells[0].Text = String.Format("<div>{0}</div>", r["id"]);
                e.Item.Cells[0].Text += String.Format("<div><strong>{0}</strong></div>", r["code"]);

                e.Item.Cells[1].Text = String.Format("<div>{0}, <span class='small'>ИНН {1}, код 1С {2}</span></div>", r["subjectName"],r["inn"],r["subjectcode"]);
                e.Item.Cells[1].Text += String.Format("<div class='small'>договор <strong>{0}</strong></div>", r["dg"]);
                e.Item.Cells[1].Text += String.Format("<div class='small'>отв. ТА <a href='javascript:return 0;' onclick=\"openflywin('../account/card.aspx?id={0}', 500, 500, '{1}')\" >{1}</a></div>", r["taid"], r["taName"]);

                e.Item.Cells[2].Text = String.Format("<div><a href='javascript:return 0;' onclick=\"openflywin('../order/detailfly.aspx?id={0}', 870, 850, 'Заявка " + r["Name"] + "')\" >{1}</a></div>", id,r["Name"]);
                e.Item.Cells[2].Text += String.Format("<div class='small'>дата {0}</div>", cDate.cToString(r["RegDate"]));
                e.Item.Cells[2].Text += String.Format("<div class='small'>автор <a href='javascript:return 0;' onclick=\"openflywin('../account/card.aspx?id={0}', 500, 500, '{1}')\" >{1}</a></div>",r["Authorid"], r["Authorname"]);
                //e.Item.Cells[2].Text += String.Format("<div class='small'>код 1С <strong>{0}</strong></div>", r["Code"]);

                e.Item.Cells[3].Text = String.Format("{0}", cNum.cToDecimal(r["summorder"], 2));
                e.Item.Cells[4].Text = String.Format("<div class='small'>{0}</div>", Order.get_stateorder_descr("" + r["state"]));

                e.Item.Cells[5].Text = String.Format("<div class='small'>{0}</div><div class='small'>{1}</div>", r["lc"],r["lcd"]);
            }
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgList.CurrentPageIndex = e.NewPageIndex;
            btnSearch_Click(null, null);
            
        }

        protected void lbtnUpdateInvoice_Click(object sender, EventArgs e)
        {
            update_invoicefield(100000);
            update_invoicefield(100001);
            update_invoicefield(100002);
            update_invoicefield(100003);
            
        }

        void update_invoicefield(int ownerid)
        {
            DataTable dt = db.GetDbTable("select * from ord where id not in (select ordi.orderid from ORDI inner join vGood"+OwnerID+" as vg on ORDI.GoodID=vg.ID where vg.zn not in ('S')) and ord.id > 104277 and ownerId="+OwnerID+" and ord.state not in ('D','F')");
            foreach (DataRow r in dt.Rows)
            {
                if (!webIO.CheckExistFile("../exch/" + r["id"] + ".pdf"))
                    db.ExecuteCmd("update ORD set invoice='N' where id=" + r["id"]);
                else
                    db.ExecuteCmd("update ORD set invoice='" + r["id"] + ".pdf' where id=" + r["id"]);
            }
        }
    }
}