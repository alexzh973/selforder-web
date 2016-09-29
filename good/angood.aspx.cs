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
    public partial class angood : p_p
    {

        private void writescript()
        {
            string script = "<script  type='text/javascript'>$(document).ready(function () {closethiswithrefresh()});</script>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "close", script, false);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string goodcode = Request["good"];
                string antype = Request["ant"];
                if ("" + Request["sel"] != "")
                {
                    if (iam.CurOrder==null )
                            iam.CurOrder = new Order(0,iam);
                    string other = Request["sel"]; // выбрали

                    decimal preQty = 1;
                    string predescr = "";
                    if (antype == "a")
                    {
                        
                        OrderItem pre = iam.CurOrder.FindItem(goodcode);
                        int ind = iam.CurOrder.Items.FindIndex(i => i.GoodCode == goodcode);
                        if (pre != null)
                        {
                            preQty = pre.Qty;
                            predescr = pre.Descr;
                            //iam.CurOrder.RemoveItem(pre.GoodId);
                           
                        }
                        GoodOwnInfo gi = GoodInfo.GetInfo(other, iam.CurOrder.OwnerID, iam.CurOrder.Subject.PriceType);
                        OrderItem item = new OrderItem() {SaleKrat = gi.SaleKrat, Mark = "*", GoodId = gi.GoodId, Zn = gi.Zn, Zn_z = gi.Zn_z, CurIncash = gi.Qty, GoodCode = gi.Code, Name = gi.Name, Qty = preQty, Price = gi.PriceSale, Summ = preQty*gi.PriceSale, state = "N", Descr = predescr, article = gi.Article, ed = gi.ed, SummBase = preQty*gi.PriceBase, ens = gi.ENS, img = gi.img, an_a = gi.an_a, an_k = gi.an_k, an_s = gi.an_s};
                        if (ind > -1)
                            iam.CurOrder.Items[ind] = item;
                        
                        //iam.CurOrder.AddItem(item);
                        writescript();
                    } else if (antype == "k" || antype == "s")
                    {
                        OrderItem pre = iam.CurOrder.FindItem(goodcode);
                        if (pre != null)
                        {
                            preQty = pre.Qty;
                        }
                        predescr = ((antype == "k") ? " комплектующая для " : " сопутствующая к ") + goodcode;
                        GoodOwnInfo gi = GoodInfo.GetInfo(other, iam.CurOrder.OwnerID, iam.CurOrder.Subject.PriceType);
                        OrderItem item = new OrderItem() {SaleKrat = gi.SaleKrat ,Mark = "*", GoodId = gi.GoodId, Zn = gi.Zn, Zn_z = gi.Zn_z, CurIncash = gi.Qty, GoodCode = gi.Code, Name = gi.Name, Qty = preQty, Price = gi.PriceSale, Summ = preQty*gi.PriceSale, state = "N", Descr = predescr, article = gi.Article, ed = gi.ed, SummBase = preQty*gi.PriceBase, ens = gi.ENS, img = gi.img, an_a = gi.an_a, an_k = gi.an_k, an_s = gi.an_s};
                        iam.CurOrder.AddItem(item);
                        writescript();
                    }
                } 
                else
                {

                    string price = (iam.CurOrder != null) ? iam.CurOrder.Subject.PriceType : "pr_b";
                    int ownerId = (iam.CurOrder != null) ? iam.CurOrder.OwnerID : iam.OwnerID;
                    DataTable dt;
                    rpK.Visible = rpS.Visible = rpA.Visible = false;
                    if (antype == "k")
                    {
                        dt = getAngoodies(goodcode, price, ownerId, "'k'");
                        if (dt.Rows.Count > 0)
                        {
                            rpK.Visible = true;
                            rpK.DataSource = dt;
                            rpK.DataBind();
                        } 
                    }
                    else if (antype == "s")
                    {
                        dt = getAngoodies(goodcode, price, ownerId, "'so','sz'");
                        if (dt.Rows.Count > 0)
                        {
                            rpS.Visible = true;
                            rpS.DataSource = dt;
                            rpS.DataBind();
                            
                        } 
                    }
                    else if (antype == "a")
                    {
                        dt = getAngoodies(goodcode, price, ownerId, "'a'");
                        if (dt.Rows.Count > 0)
                        {
                            rpA.Visible = true;
                            rpA.DataSource = dt;
                            rpA.DataBind();
                            
                        } 
                    }
                }
            }
        }


        private DataTable getAngoodies(string goodCode, string typecen, int ownerId, string antype)
        {

            DataTable dt = db.GetDbTable("select ang.antype, g.goodId, g.goodCode, g.Name,g.brend,g.qty, g." + typecen + " as price, g.ed, g.zn, g.zn_z,g.ens from vGood" + ownerId + " as g inner join ANGOOD as ang on g.goodcode=ang.ancode and antype in (" + antype + ") and ang.goodcode='" + goodCode + "'"); // + ((antype == "'a'") ? " and g." + typecen + ">0" : "")
            return dt;
        }

    }
}