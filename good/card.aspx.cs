using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;
using System.Data;

namespace wstcp.good
{
    public partial class card : p_p
    {
        private int OwnerId
        {
            get { if (ViewState["own"] == null) return iam.OwnerID;
                return cNum.cToInt(ViewState["own"]);
            }
            set { ViewState["own"] = (value>=100000)?value:iam.OwnerID; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                eID = cNum.cToInt(Request["id"]);
                OwnerId = cNum.cToInt(Request["ownid"]);
                string typecen = Request["typecen"];
                if ("" + typecen == "")
                    typecen = "pr_b";
                showdetail( typecen);
                show_acc(eID,OwnerId);
            }
        }

        private void showdetail(string typecen)
        {

            string priceType = typecen;
            if (iam.PresentedSubjectID > 0)
            {
                Subject subj = new Subject(iam.PresentedSubjectID, iam);
                priceType = subj.PriceType;
            }
            GoodOwnInfo gi = GoodInfo.GetInfo(eID, OwnerId, priceType);
            lbName.Text = gi.Name;
            lbPrice.Text = wstcp.good.gdetail.GetNameCen(priceType)+": " + cNum.cToDecimal(gi.PriceSale, 2)+"руб.";

            tblIncash.Text = prepareIncashTable(eID);

           
            imgGood.ImageUrl = (gi.img!="noimgs")?"../img.ashx?act=good&id=" + gi.GoodId:"../media/nophoto.gif";
            
           
        }

        private void show_acc(int id, int ownerid)
        {
            DataTable ga = db.GetDbTable("select * from accgood where goodId=" + id + " and OwnerId=" + ownerid + " order by FirstDate desc");

            dgAcc.DataSource = ga;
            dgAcc.DataBind();

        }


        private string prepareIncashTable(int id)
        {
            DataTable dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId=" + id + "),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId=" + id + ") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
            string resp = ""; decimal q = 0;
            if (dt.Select("qty>0").Length > 0)
            {
                resp = "<table>";
                
                foreach (DataRow r in dt.Rows)
                {
                    q = cNum.cToDecimal(r["qty"]);

                    resp += String.Format("<tr><td >{0}</td><td >{1}</td></tr>", r["Name"], ((q == 0) ? "-" : "" + q));
                }
                

                resp += "</table>";
            }
            else
            {
                resp = "в подразделениях нет остатков";
            }
            return resp;
        }

        
    }
}