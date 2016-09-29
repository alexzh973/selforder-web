using System;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;
using System.Web;


namespace wstcp
{
    /// <summary>
    /// Сводное описание для order
    /// </summary>
    public class orderajax : IHttpHandler
    {
        
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string res = "";

            string sid = context.Request["sid"];
            IAM iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            int requestId = cNum.cToInt(context.Request["id"]);
            string act = context.Request["act"];
            if (act == "countitemsumm")
            {
                context.Response.Write(cNum.cToDecimal(cNum.cToDecimal(context.Request["pr"]) * cNum.cToDecimal(context.Request["q"]),2));
                return;
            }

            //Order ord = (iam.CurOrder == null || ( iam.CurOrder.ID != requestId && requestId > 0)) ? new Order(requestId, iam) : iam.CurOrder;
            if (iam.CurOrder == null || (iam.CurOrder.ID != requestId && requestId > 0))
                iam.CurOrder = new Order(requestId, iam);


            switch (act)
            {
                case "sns":
                    if (iam.CurOrder.State != context.Request["ns"])
                    {
                        if (context.Request["ns"] == "X" && iam.CurOrder.Code == "")
                            iam.CurOrder.State = "D";
                        else
                            iam.CurOrder.State = context.Request["ns"];
                        
                        
                        Order.Save(iam.CurOrder);
                        context.Response.Write("Заявка переведена в новый статус " + iam.CurOrder.StateDescr);
                    }
                    context.Response.Write("");
                    return;
                    break;
                case "sel":
                    UserFilter.Save(iam.ID, "F:" + context.Request["what"].ToUpper(), context.Request["param"]);
                    UserFilter.Save(iam.ID, "FLTSEL", context.Request["what"].ToUpper());
                    return;
                    break;
                case "getsumm":
                    context.Response.Write("" + iam.CurOrder.Summ + "");
                    return;
                    break;
                case "getdscnt":
                    context.Response.Write("" + cNum.cToDecimal(iam.CurOrder.Discount * 100, 2) + "");
                    return;
                    break;
                case "currinfo":
                    int ch = iam.CurOrder.Items.Count;
                    string word = cStr.GetWordByNumerik(ch,"товар","","а","ов");
                    context.Response.Write(String.Format("{0} {1}<br/>{2}руб.", ch, word, cNum.cToDecimal(iam.CurOrder.Summ,2)));
                    return;
                case "summ":
                    context.Response.Write("Заявка на сумму " + iam.CurOrder.Summ + "р. " + ((iam.CurOrder.Changed) ? " не сохранена" : ""));
                    return;
                    break;
                case "rcnt":
                    int gid = cNum.cToInt(context.Request["gid"]);
                    int q = cNum.cToInt(context.Request["qty"]);
                    OrderItem item = iam.CurOrder.FindItem(gid);
                    iam.CurOrder.ChangeItem(item, q, "" + context.Request["descr"]);
                    context.Response.Write("" + iam.CurOrder.Summ);
                    return;
                    break;
                case "setname":
                    iam.CurOrder.Name = "" + context.Request["name"];
                    return;
                case "setdescr":
                    iam.CurOrder.Descr = "" + context.Request["descr"];
                    return;
                default:
                    break;
            }






            int good_id = cNum.cToInt(context.Request["gid"]);
            decimal good_qty = cNum.cToDecimal(context.Request["qty"]);
            string descr = ""+context.Request["descr"];
            string resp = "";

            
            if (act == "r")// убрать позицию
            {
                OrderItem item = iam.CurOrder.FindItem(good_id);
                if (item != null)
                {
                    iam.CurOrder.RemoveItem(good_id);
                    res = "удалена позиция " + item.Name;
                   
                }
                else
                    res = "ничего не удалено";
            }
            else
            {
                OrderItem item = iam.CurOrder.FindItem(good_id);
                if (item == null)
                {
                    string priceType = iam.CurOrder.Subject.PriceType;
                    if (priceType == "") priceType = "pr_b";
                    GoodOwnInfo gi = GoodInfo.GetInfo(good_id, iam.OwnerID, priceType);
                    item = new OrderItem() { SaleKrat = gi.SaleKrat, Mark = "*", GoodId = good_id, Zn = gi.Zn, Zn_z = gi.Zn_z, CurIncash = gi.Qty, GoodCode = gi.Code, Name = gi.Name, Qty = good_qty, Price = gi.PriceSale, Summ = good_qty * gi.PriceSale, state = "N", Descr = descr, article = gi.Article, ed = gi.ed, SummBase = good_qty * gi.PriceBase, ens = gi.ENS, img = gi.img, an_a = gi.an_a, an_k = gi.an_k, an_s = gi.an_s};
                    iam.CurOrder.AddItem(item);
                    res = "добавлена позиция " + item.Name;
                    
                }
                else
                {
                    iam.CurOrder.ChangeItem(item, good_qty, descr);
                    res = "изменено кол-во по позиции " + item.Name;
                }

            }
            context.Response.Write(res);
        }
        public void ProcessRequest_old(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string res = "";

            string sid = context.Request["sid"];
            IAM iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            int requestId = cNum.cToInt(context.Request["id"]);
            string act = context.Request["act"];
            if (act == "countitemsumm")
            {
                context.Response.Write(cNum.cToDecimal(cNum.cToDecimal(context.Request["pr"]) * cNum.cToDecimal(context.Request["q"]), 2));
                return;
            }

            Order ord = (iam.CurOrder == null || (iam.CurOrder.ID != requestId && requestId > 0)) ? new Order(requestId, iam) : iam.CurOrder;
            //if (iam.CurOrder == null || (iam.CurOrder.ID != requestId && requestId > 0))
            //    iam.CurOrder = new Order(requestId, iam);


            switch (act)
            {
                case "sns":
                    if (ord.State != context.Request["ns"])
                    {
                        if (context.Request["ns"] == "X" && ord.Code == "")
                            ord.State = "D";
                        else
                            ord.State = context.Request["ns"];

                        //if (ord.State.ToUpper() == "X")
                        //    ord.Code = "";
                        Order.Save(ord);
                        context.Response.Write("Заявка переведена в новый статус " + ord.StateDescr);
                    }
                    return;
                    break;
                case "sel":
                    UserFilter.Save(iam.ID, "F:" + context.Request["what"].ToUpper(), context.Request["param"]);
                    UserFilter.Save(iam.ID, "FLTSEL", context.Request["what"].ToUpper());
                    return;
                    break;
                case "getsumm":
                    context.Response.Write("" + ord.Summ + "");
                    return;
                    break;
                case "getdscnt":
                    context.Response.Write("" + cNum.cToDecimal(ord.Discount * 100, 2) + "");
                    return;
                    break;
                case "summ":
                    context.Response.Write("Заявка на сумму " + ord.Summ + "р. " + ((ord.Changed) ? " не сохранена" : ""));
                    return;
                    break;
                case "rcnt":
                    int gid = cNum.cToInt(context.Request["gid"]);
                    int q = cNum.cToInt(context.Request["qty"]);
                    OrderItem item = ord.FindItem(gid);
                    ord.ChangeItem(item, q, "" + context.Request["descr"]);
                    context.Response.Write("" + ord.Summ);
                    return;
                    break;
                case "setname":
                    ord.Name = "" + context.Request["name"];
                    return;
                case "setdescr":
                    ord.Descr = "" + context.Request["descr"];
                    return;
                default:
                    break;
            }






            int good_id = cNum.cToInt(context.Request["gid"]);
            decimal good_qty = cNum.cToDecimal(context.Request["qty"]);
            string descr = context.Request["descr"];
            string resp = "";


            //Order.BeginWork(iam, ord);
            if (act == "r")// убрать позицию
            {
                OrderItem item = ord.FindItem(good_id);
                if (item != null)
                {
                    ord.RemoveItem(good_id);
                    res = "удалена позиция " + item.Name;
                    iam.CurOrder = ord;
                }
                else
                    res = "ничего не удалено";
            }
            else
            {
                OrderItem item = ord.FindItem(good_id);
                if (item == null)
                {
                    string priceType = ord.Subject.PriceType;
                    if (priceType == "") priceType = "pr_b";
                    GoodOwnInfo gi = GoodInfo.GetInfo(good_id, iam.OwnerID, priceType);
                    item = new OrderItem() { SaleKrat = gi.SaleKrat, Mark = "*", GoodId = good_id, Zn = gi.Zn, Zn_z = gi.Zn_z, CurIncash = gi.Qty, GoodCode = gi.Code, Name = gi.Name, Qty = good_qty, Price = gi.PriceSale, Summ = good_qty * gi.PriceSale, state = "N", Descr = descr, article = gi.Article, ed = gi.ed, SummBase = good_qty * gi.PriceBase, ens = gi.ENS, img = gi.img, an_a = gi.an_a, an_k = gi.an_k, an_s = gi.an_s };
                    ord.AddItem(item);
                    res = "добавлена позиция " + item.Name;
                    iam.CurOrder = ord;
                }
                else
                {
                    ord.ChangeItem(item, good_qty, descr);
                    res = "изменено кол-во по позиции " + item.Name;
                }

            }
            context.Response.Write(res);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}