using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using System.Data;
using ensoCom;
using wstcp.models;

namespace wstcp
{
    public partial class _sales : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {if (iam.ID <= 0) Response.Redirect("~/default.aspx");
            lbMess.Text = "";
            if (!IsPostBack)
            {
                ((mainpage)this.Master).VisibleRightPanel = false;
                ((mainpage) this.Master).SelectedMenu = "sales";
                
                eID = cNum.cToInt(Request["id"]);
                string today = cDate.Date2Sql(cDate.TodayD);
                string wherestartdate = (iam.IsAdmin || iam.IsSuperAdmin) ? " 1=1" : "startdate<=" + today + "";
                if (eID > 0)
                {
                    SaleAcc acc = new SaleAcc();
                    SaleAcc.Load(acc,eID, iam);
                    lbTitleAcc.Text = acc.Name;
                    rpAccItems.DataSource = acc.Items;
                    rpAccItems.DataBind();
                    rpOffer.Visible = false;
                    
                    ((mainpage) this.Master).VisibleLeftPanel = true;
                    
                    DataTable dt = db.GetDbTable("select * from ACC where " + wherestartdate + " and finishdate>=" + today + ((iam.IsSuperAdmin) ? "" : " and ownerid=" + iam.OwnerID));
                    Repeater1.DataSource = dt;
                    Repeater1.DataBind();
                } 
                else
                {
                    ((mainpage) this.Master).VisibleLeftPanel = false;
                    //lbTitleAcc.Text = "Распродажа остатков";
                    DataTable dt = db.GetDbTable("select *,case when banimg is null then 0 else id end as img from ACC where " + wherestartdate + " and finishdate>=" + today + ((iam.IsSuperAdmin) ? "" : " and ownerid=" + iam.OwnerID));
                    rpListAcc.DataSource = dt;
                    rpListAcc.DataBind();
                    show_nl();

                }

            }
           
        }


        //private void show_personalTAs()
        //{
        //    Subject subj = new Subject(iam.PresentedSubjectID, iam);
        //    ((mainpage)this.Master).show_personalTAs(subj);
        //    ((mainpage)this.Master).show_subj_info(subj);
        //}

        private void show_nl()
        {
            string typecen = "pr_b";
            if (iam.PresentedSubjectID > 0)
            {
                Subject subj = new Subject(iam.PresentedSubjectID,iam);
                typecen = subj.PriceType;
            }
            DataTable dt = db.GetDbTable("select top 500 goodid, good.name, img, convert(numeric(15,0),owng."+typecen+") as price, ens, owng.goodCode,owng.SaleKrat from OWNG inner join good on owng.goodId=good.id  where ownerid="+iam.OwnerID+" and zn_z in ('NL','Pz') and qty>0 and owng.pr_b<100000 order by owng.pr_b*qty desc");
            foreach (DataRow r in dt.Rows)
            {

                string img = "../media/gimg/" + r["img"];

                r["img"] = (webIO.CheckExistFile(img)) ? img : "../media/nophoto.gif";
            }
            rpOffer.DataSource = dt;
            rpOffer.DataBind();
        }



    }
}