using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using selforderlib;
using ensoCom;
using System.Text;
using System.Collections;
using System.IO;

namespace wstcp
{

    public partial class _cart : p_p
    {


        private DataView dv;

        



        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID < 100000)
            {
                Response.Redirect("~/default.aspx", true);
            }
            if (!IsPostBack)
            {

                ((mainpage)this.Master).SelectedMenu = "order";
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;

                if ((iam.IsSaller || iam.IsSuperAdmin || iam.IsTA || iam.IsAdmin) && iam.SubjectID != iam.PresentedSubjectID && iam.PresentedSubjectID > 0)
                    lbForSubj.Text = "для " + iam.PresentedSubject.Name;

                lbMessage.Text = (iam.PresentedSubjectID == 0) ? "Нельзя формировать заявку не выбрав клиента. <a href='../default.aspx'>Вернуться на главную страницу</a> " : "";
                if (iam.PresentedSubjectID > 0)
                {
                    DgInfo1.ShowInfo(iam.PresentedSubject.CodeDG, iam.PresentedSubject.OwnerID);
                }
            }

           

            TabControl1.InitTabContainer("carttabs");
            TabControl1.AutoPostBack = false;
            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Классификатор", pnlStruct);
            TabControl1.AddTabpage("Справочник номенклатуры", pnlList);
            TabControl1.AddTabpage("Текущая заявка", "Текущая заявка", pnlOrder, "", "cartbtn");


            if (!IsPostBack)
            {
                TabControl1.SelectedTabIndex = 2;
                eID = cNum.cToInt((string)RouteData.Values["id"] ?? Request.QueryString["id"]);
                _ACT_ = (string) RouteData.Values["act"] ?? Request.QueryString["act"];

                if (iam.CurOrder == null || (iam.CurOrder.ID != eID && eID > 0))
                {
                    iam.CurOrder = new Order(eID, iam);
                }

                if (eID > 0 || Request["go"] == "ord")
                {
                    if (_ACT_ == "copy")
                    {
                        eID = 0;
                        iam.CurOrder.MakeCopy();
                    }

                    
                    iam.PresentedSubjectID = iam.CurOrder.SubjectID;

                    txNameOrder.ReadOnly = txDescr.ReadOnly = OrderReadOnly;
                    lbReadonly.Visible = OrderReadOnly;

                    CommentList1.DataObject = new sObject(eID, Order.TDB);
                    CommentList1.CanAddComment = true;
                    CommentList1.Event_AddedComment = new EventHandler(CommentList1_AddingComment);
                }
                else
                {
                    lbReadonly.Visible = false;
                    CommentList1.Visible = false;
                }


            }


            if (TabControl1.ChangedTabpage || !IsPostBack || refresh.Value=="Y")
            {
                if (TabControl1.IsActiveTabpage(pnlOrder))
                {
                    load_curentorder();
                }
                else if (TabControl1.IsActiveTabpage(pnlStruct))
                {
                    Session["goodlisttabsTBI"] = 0;
                    Response.Redirect("orderdefault.aspx");
                } else
                {
                    Session["goodlisttabsTBI"] = 1;
                    Response.Redirect("orderdefault.aspx");
                }
            }





            if (iam.CurOrder != null && iam.CurOrder.Summ > 0)
            {
                if (iam.CurOrder.TypeOrder == "Q")
                    TabControl1.TabTitles[2] = iam.CurOrder.Name;
                else
                    TabControl1.TabTitles[2] = "Заявка на сумму " + cNum.cToDecimal(iam.CurOrder.Summ, 2) + "р.";
                TabControl1.TabCssPrefix[2] = "bold";
                if (iam.CurOrder.ID == 0 || iam.CurOrder.Changed)
                    TabControl1.TabTitles[2] += " не сохранена";

            }

        }




        private void load_curentorder()
        {
            lbtnNextStady.Visible = false;
            lbtnNextStady.CommandArgument = "";
            lbtnCancel.Visible = false;
            lbtnCancel.CommandArgument = "";
            if (iam.PresentedSubjectID == 0)
            {
                iam.CurOrder = null;

                return;
            }
            else if (iam.CurOrder == null)
                iam.CurOrder = new Order(eID, iam);

            if (iam.CurOrder.ID == 0 && iam.CurOrder.Dg == "")
                iam.CurOrder.Dg = iam.PresentedSubject.CodeDG;
            //Order ord = iam.CurOrder;
            if (iam.CurOrder.ID > 0)
                ViewArticle.AddView(iam, iam.CurOrder.ThisObject);

            link4prn.Text = (iam.CurOrder.ID > 0) ? "<a href='javascript:return false;' class='button' onclick=\"openflywin('../order/detailfly.aspx?id=" + iam.CurOrder.ID + "', 850, 800, 'Заявка " + iam.CurOrder.Name + " от " + iam.CurOrder.RegDate.ToShortDateString() + "')\"><img src='../simg/16/printer.png'> печать</a>" : "";

            link4prn.Visible = (link4prn.Text != "");
            lbSubject.Text = iam.CurOrder.Subject.Name + ", ИНН " + iam.CurOrder.Subject.INN;
            lbOrder.Text = "" + iam.CurOrder.ID;
            txNameOrder.Text = iam.CurOrder.Name;
            txDescr.Text = iam.CurOrder.Descr;
            lbRegDate.Text = iam.CurOrder.RegDate.ToShortDateString();
            lbStateOrder.Text = (iam.CurOrder.ID > 0) ? iam.CurOrder.StateDescr : "Заявка еще не сохранена";
            lbCode.Text = iam.CurOrder.Code;

            btnClear.Visible = lbtnSave.Visible = !OrderReadOnly;

            if (iam.CurOrder.ID == 0 || iam.CurOrder.Changed || iam.CurOrder.TypeOrder == "Q")
            {
                lbtnNextStady.Visible = false;
            } else
            {
                set_state_button(iam.CurOrder.State);
            }

            if ((iam.CurOrder.Items.Select(x => x.Zn != "S").Count() > 0 || iam.CurOrder.TEOTrans != "") && !iam.CurOrder.IsNew)
            {
                lbtnCancel.Visible = false;
                lbStateOrder.Text += "<p>Отменить заявку без менеджера нельзя потому что " + ((iam.CurOrder.TEOTrans != "") ? "назначена доставка" : "в Заявке есть заказные позиции") + "</p>";
            }
            if (iam.CurOrder.IsNew)
                lbtnCancel.Visible = false;
            decimal smb = iam.CurOrder.SummBase;
            decimal sm = iam.CurOrder.Summ;

            foreach (OrderItem item in iam.CurOrder.Items)
            {

                decimal q = (Request["q_" + item.GoodId] != null) ? cNum.cToDecimal(Request["q_" + item.GoodId]) : item.Qty;

                if (q <= 0) q = 1;
                if (item.Qty != q)
                {
                    iam.CurOrder.ChangeItem(item, q, item.Descr);
                    item.Mark = "";
                }
            }

            dgOrder.DataSource = iam.CurOrder.Items;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();
            if (iam.CurOrder.TypeOrder == "Q")
            {
                if (iam.CurOrder.Changed)
                    lbStateOrder.Text = "набор для прайс-листа изменен";
            }
            else
            {
                DgInfo1.ShowInfo(iam.CurOrder.Dg, iam.CurOrder.OwnerID);
            }
        }


        private void CommentList1_AddingComment(object sender, EventArgs e)
        {
            Order ord = iam.CurOrder;
            string emails = ord.Subject.EmailTAs;
            EmailMessage m = new EmailMessage(iam.Email, emails, "новый комментарий к Заявке №" + ord.ID + " от " + ord.RegDate.ToShortDateString() + " (код 1С " + ord.Code + ")", CommentList1.Text);
            m.Send();
        }

        private void set_state_button(string state)
        {
            string st = state.ToUpper();
            switch (st)
            {
                case "U":
                    lbtnNextStady.Visible = true;
                    lbtnNextStady.ToolTip = "Подтвердить к заказу";
                    lbtnNextStady.CommandArgument = "A";
                    break;
                case "E":
                    lbtnNextStady.Visible = false;
                    break;
                case "S":
                    lbtnNextStady.Visible = true;
                    lbtnNextStady.ToolTip = "Подтвердить к покупке";
                    lbtnNextStady.CommandArgument = "M";
                    break;
                case "R":
                    lbtnNextStady.Visible = true;
                    lbtnNextStady.ToolTip = "Готов получить товар (тот который в наличии)";
                    lbtnNextStady.CommandArgument = "E";
                    break;
                default:
                    lbtnNextStady.Visible = false;
                    lbtnNextStady.CommandArgument = "";
                    break;
            }

            if (cStr.Exist(st, new string[] { "N", "U", "A", "Z", "S" })
                && !presentZgoodes() && !presentTeo())
            {
                lbtnCancel.Visible = true;
                lbtnCancel.CommandArgument = "D";
                lbtnCancel.ToolTip = "Отменить заявку";
                lbtnCancel.Text = "<img src=\"../simg/icodel.png\" title=\"Отменить заявку\"/>";
            }
            else if (state.ToUpper() == "D")
            {
                lbtnCancel.Visible = true;
                lbtnCancel.CommandArgument = "N";
                lbtnCancel.ToolTip = "Вернуть к жизни";
                lbtnCancel.Text = "<img src=\"../simg/icorepair.png\" title=\"Отменить заявку\"/>";
            }
            else
            {
                lbtnCancel.Visible = false;
                lbtnCancel.CommandArgument = "";
                lbtnCancel.ToolTip = "...";
            }

            //// пока оставим только отмену:
            //lbtnNextStady.Visible = false;

            if (iam.SubjectID != iam.CurOrder.SubjectID && !(iam.IsSaller && iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)))
                lbtnCancel.Visible = lbtnNextStady.Visible = false;
        }

        private bool presentTeo()
        {
            return (iam.CurOrder.TEOTrans != "");
        }

        private bool presentZgoodes()
        {
            return (iam.CurOrder.Items.FindAll(x => x.Zn != "S" && x.Realized < x.Qty).Count > 0);
        }





        private bool ItsMySubj(int subjectId)
        {
            return iam.ItsMySubj.Contains(subjectId);
        }

        bool OrderReadOnly
        {
            get
            {

                return !(cStr.Exist(iam.CurOrder.State.ToUpper(), new string[] { "", "U", "N" }) && (iam.CurOrder.SubjectID == iam.PresentedSubjectID || ItsMySubj(iam.CurOrder.SubjectID) || iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)));
            }
        }






        string stategood(int goodId, decimal qty, string zn, string zn_ost, decimal qty_need = 0)
        {
            string ret = "";
            if (zn_ost == "NL" || zn_ost == "P2" || zn_ost == "PZ")
            {
                ret = "<span class='goodincash-full'>в наличии</span>";
            }
            else
            {

                ret = (qty - qty_need > 0) ? ((qty < 5 && zn == "S") ? "<span class='goodincash-little'>есть, но мало</span>" : "<span class='goodincash-full'>в наличии</span>") : "<img src='../simg/32/ph.png' style='cursor:pointer' onclick=\"openflywin('../common/gimail.aspx?id=" + goodId + "', 400, 300, 'Запрос информации о товаре')\" title='по запросу' alt='?'/>";

            }
            return ret;
        }

        protected void dgOrder_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                OrderItem r = (OrderItem)e.Item.DataItem;

                bool ordReadOnly = OrderReadOnly;

                if (ordReadOnly)
                    e.Item.Cells[1].Text = "";
                e.Item.Attributes.Add("id", "tr_" + r.GoodId);
                e.Item.Attributes.Add("onclick", "thisrow(this.id)");
                e.Item.Cells[3].Text = r.Name + " " + get_angood(r);


                if (r.Mark == "*")
                    e.Item.Cells[2].CssClass = e.Item.Cells[3].CssClass = e.Item.Cells[4].CssClass = e.Item.Cells[6].CssClass = e.Item.Cells[7].CssClass = "blue";
                //e.Item.Cells[4].Attributes.Add("id", "pr_" + r.GoodId);
                e.Item.Cells[4].Text = "<input type='text' id='pr_" + r.GoodId + "' style='border:0; text-align:right; width:80px' readonly value='" + r.Price + "'/>";
                e.Item.Cells[5].Text = "<input " + ((ordReadOnly) ? "readonly" : "") + " class='center qtyfield inputqty' maxlength='5' name='q_" + r.GoodId + "' id='q_" + r.GoodId + "' type='text' title='" + ((r.SaleKrat > 1) ? "Кол-во должно быть кратно " + r.SaleKrat : "") + "' onchange=\"checkNumeric(this.id,1);recount('" + r.GoodId + "','" + r.SaleKrat + "');\" value='" + cNum.cToDecimal(r.Qty, 1) + "' />";
                string tooltipP = (r.SaleKrat>1)?" выписывать только кратно "+r.SaleKrat:"+1";
                string tooltipM = (r.SaleKrat > 1) ? " выписывать только кратно " + r.SaleKrat : "-1";
                e.Item.Cells[6].Text = "<img class='plusminus' src='../simg/plus.png' alt='+' title='" + tooltipP + "' onclick=\"plusqo('" + r.GoodId + "','" + r.SaleKrat + "')\"/><br/><img class='plusminus' src='../simg/minus.png' alt='-' title='" + tooltipM + "' onclick=\"minusqo('" + r.GoodId + "','" + r.SaleKrat + "')\"/>";



                e.Item.Cells[8].Attributes.Add("id", "sm_" + r.GoodId);

                e.Item.Cells[9].Text = "<input maxlength='150' style='width:100px;' name='ds_" + r.GoodId + "' id='ds_" + r.GoodId + "' type='text' " + ((ordReadOnly) ? "readonly" : "") + " class='descritem' value='" + r.Descr + "' onchange=\"chngitemdescr('" + r.GoodId + "')\"/>";
                string osttext = wstcp.order.detailfly.get_ost_info(iam.CurOrder.State, r.Zn, r.CurIncash, r.Qty, r.Booking, r.Realized, r.WDate, r.Comment, r.SrokPost);

                e.Item.Cells[10].Text = osttext;
                if (r.ens != "")
                    e.Item.Cells[10].Text += "<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r.ens + "/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>";

                if ("" + r.img != "noimgs")
                {
                    e.Item.Cells[10].Text += "<img class='microimg' src='../img.ashx?act=good&id=" + r.GoodId + "' id='imgeye" + r.GoodId + "'/><img style='margin:1px;' id='eye" + r.GoodId + "' class='eye' src='../simg/16/photo.png'/> ";
                }
                //if (webIO.CheckExistFile("../media/gimg/" + r.img))
                //    e.Item.Cells[10].Text += "<img class='microimg' src='../media/gimg/" + r.img + "' id='imgeye" + r.GoodId + "'/><img style='margin:1px;' id='eye" + r.GoodId + "' class='eye' src='../simg/16/photo.png'/> ";

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {

                e.Item.Cells[1].ColumnSpan = 6;//.Attributes.Add("colspan", "6");
                e.Item.Cells[2].ColumnSpan = 3;//.Attributes.Add("colspan", "3");
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                Order ord = iam.CurOrder;
                decimal d = ord.Discount;
                if (d > 0 && ord.OWner.ShowInvDiscount)
                {

                    e.Item.Cells[1].Text = "СУММА со скидкой<br/>";

                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + " руб.<br/>";
                    e.Item.Cells[1].Text += "предоставлена скидка";
                    e.Item.Cells[2].Text += "" + cNum.cToDecimal(d * 100, 2) + "%";

                }
                else
                {
                    e.Item.Cells[1].Text = "СУММА";
                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + "руб.";

                }
                e.Item.Cells[2].Attributes.Add("id", "orditg");


            }
        }



        private string get_angood(OrderItem r)
        {
            string ret = "";
            ret += ((r.an_k > 0) ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
                ((r.an_s > 0) ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
                ((r.an_a > 0) ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=a',500,500,'Аналоги');>а</a>" : "");
            return ret;
        }


        protected void dgOrder_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = cNum.cToInt(e.Item.Cells[0].Text);

            iam.CurOrder.RemoveItem(id);

            TabControl1.TabTitles[2] = "Заявка на сумму " + iam.CurOrder.Summ + "р.";
            load_curentorder();
        }




        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {

            Order ord = iam.CurOrder;
            if (ord.Items.Count == 0)
            {
                lbMess.Text = "Невозможно записать заявку без товаров.";
                return;
            }
            ord.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Заявка от " + ord.RegDate.ToShortDateString() + " " + ord.RegDate.ToShortTimeString();
            ord.Name = txNameOrder.Text;
            ord.AuthorID = iam.ID;
            bool isNew = ord.IsNew;
            if (isNew) ord.Dg = ord.Subject.CodeDG;
            else if (ord.Changed)
                ord.State = "N";
            bool result = Order.Save(ord);
            bool existZ = ord.Items.FindAll(x => x.Zn.ToUpper() != "S").Count > 0;
            if (result && !existZ)
                db.ExecuteCmd("update ord set invoice='N' where id=" + ord.ID);
            lbOrder.Text = "" + ord.ID;
            lbRegDate.Text = ord.RegDate.ToShortDateString();
            if (result)
            {
                EmailMessage msg;
                try
                {
                    string emailTo = (IsDev) ? "alexzh@santur.ru" : ord.Subject.EmailTAs;
                    msg = new EmailMessage()
                    {
                        AddressFrom = CurrentCfg.EmailPortal,
                        AddressTo = emailTo,
                        TitleMessage = ord.Subject.Name + ": " + ((ord.Code == "") ? "Создана новая заявка " : "Скорректирована заявка "),
                        BodyText = "<p>Клиент " + ord.Subject.Name + ((ord.Code == "") ? " оформил" : " изменил") + " Заявку №" + ord.ID + " на сумму " + ord.Summ + "руб.</p>"+
                        "<p>Автор " + ord.Author.Name + ", " + ord.Author.Email + ", " + ord.Author.Phones + ".</p>" +
                        ((ord.Descr == "") ? "<p>Комментария нет</p>" : "<p>Комментарий к заявке: <i>" + ord.Descr + "</i></p>")
                    };
                      
                    msg.Send("alexzh@santur.ru");
                }
                catch { }

                try
                {
                    string emailTo = (IsDev) ? "alexzh@santur.ru" : iam.Email;
                    msg = new EmailMessage()
                    {
                        AddressFrom = CurrentCfg.EmailPortal,
                        AddressTo = emailTo,
                        TitleMessage = "santechportal.ru: " + ((ord.Code == "") ? "Создана новая заявка " : "Скорректирована заявка "),
                        BodyText = "<p>Вы " + ((ord.Code == "") ? " оформили" : " изменили") + " Заявку №" + ord.ID + " на сумму " + ord.Summ + "руб.</p>"+
                        "<p>Для контроля и участия в процессе обработки Вашей заявки перейдите по ссылке <a href='http://santechportal.ru/order/cart.aspx?id="+ord.ID+"'>заявка на santechportal.ru</a></p>"
                    };
                    msg.Send("alexzh@santur.ru");
                }
                catch { }



                bool avail = OrderInfo.CheckAvailibleGet(iam.CurOrder);
                if (avail)
                {
                    lbMsgResult.Text = "<p>Весь товар по этой заявке есть в наличии</p>";
                    DGinfo dgi = Subject.GetDgInfo(iam.CurOrder.Dg, iam.CurOrder.OwnerID);
                    if ((dgi.CurrentDZ + dgi.LimitDZ) >= iam.CurOrder.Summ)
                    {
                        //lbMsgResult.Text = "";
                        pnlWishDate.Visible = true;
                        btnGetOrder.CommandArgument = "" + iam.CurOrder.ID;
                    }
                    else
                    {
                        lbMsgResult.Text += "<p>Ваш баланс не позволяет получить товар сегодня. Дождитесь выставленного счета на оплату.</p>";
                        pnlWishDate.Visible = false;
                    }


                }
                TabControl1.SetTabTitle(2, "Заявка на сумму " + iam.CurOrder.Summ + "р.");
                eID = 0;
                iam.CurOrder = null;
                pnlFormOrder.Visible = false;
                lbResultID.Text = ord.ID.ToString();
                lbResultInvoice.Text = (existZ) ? "В заказе присутствуют заказные позиции, поэтому счет на оплату можно получить у своего менеджера." : "После этого, к заявке будет прикреплен счет на оплату";
                pnlResult.Visible = true;
                Session["taborders"] = "actualNU";
            }
        }





        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            pnlFormOrder.Visible = true;
            pnlResult.Visible = false;
            eID = 0;
            iam.CurOrder = new Order(eID, iam);

            iam.CurOrder.SubjectID = iam.PresentedSubjectID;
            iam.CurOrder.State = "N";
            Response.Redirect("orderdefault.aspx");
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            iam.CurOrder = new Order(iam.CurOrder.ID, iam);
            txNameOrder.Text = iam.CurOrder.Name;
            txDescr.Text = iam.CurOrder.Descr;
            load_curentorder();
        }




        protected void lbtnNextStady_Click(object sender, EventArgs e)
        {

            if (lbtnNextStady.CommandArgument == "A")
            {
                iam.CurOrder.State = "A";
                Order.Save(iam.CurOrder);
                load_curentorder();
            }
            else if (lbtnNextStady.CommandArgument == "M")
            {
                iam.CurOrder.State = "M";
                Order.Save(iam.CurOrder);
                load_curentorder();
            }
            else if (lbtnNextStady.CommandArgument == "E")
            {
                iam.CurOrder.State = "E";
                Order.Save(iam.CurOrder);
                load_curentorder();
            }
        }

        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            int id = iam.CurOrder.ID;
            bool res = Order.Delete(iam.CurOrder, iam);
            if (res)
            {

                lbMessage.Text = "Заявка №" + id + " удалена";
                btnNewOrder_Click(sender, e);
            }
        }

        protected void txDescr_PreRender(object sender, EventArgs e)
        {
            txDescr.Attributes.Add("onchange", "setdescr()");
        }

        protected void txNameOrder_PreRender(object sender, EventArgs e)
        {
            txNameOrder.Attributes.Add("onchange", "setname()");
        }

        protected void btnGetOrder_Click(object sender, EventArgs e)
        {
            if (ucWishDate.SelectedDate < cDate.AddWorkDays(cDate.TodayD, 1))
            {
                lbMess.Text = "Дата не совсем удачная выбрана";
                return;
            }
            Order ord = new Order(cNum.cToInt(btnGetOrder.CommandArgument), iam);


            ord.WishDate = ucWishDate.SelectedDate;
            ord.State = "E";
            Order.Save(ord);


            DGinfo d = Subject.GetDgInfo(ord.Dg, ord.OwnerID);

            bool moneyenough = ((d.CurrentDZ + d.LimitDZ) >= ord.Summ);

            string emails = (IsDev) ? "alexzh@santur.ru" : ord.Subject.EmailTAs;

            EmailMessage m;

            m = new EmailMessage(iam.Email, emails, "Заявка с сайта № " + ord.ID + " от " + ord.RegDate.ToShortDateString() + ": Готовность к получению ", "<p>Клиент <strong>" + ord.Subject.Name + "</strong> выразил желание получить товар по Заявке № " + ord.ID + " " + ((ord.Code == "") ? " (на текущий момент Заказ покупателя еще не сформирован, на это требуется минут 5, и сможете найти его по сумме " + ord.Summ + "руб.)" : "") + "</p><p>" + ((chNeedTrans.Checked) ? "Клиент хочет Доставку (нужно решить по ее стоимости)" : "Доставка не требуется") + "</p><p>" + ((!moneyenough) ? "Внимание! Денег недостаточно ..." : "") + "</p>");
            m.Send();

            lbMess.Text = "<p>Запрос на отгрузку отправлен Вашему менеджеру.</p><p>В ближайшее время он свяжется с Вами для уточненния деталей.</p>";
            pnlResult.Visible = false;
            pnlWishDate.Visible = false;
            Session["taborders"] = "work";
        }

        protected void btnCancelGetOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("../default.aspx");
        }

        protected void lbtnGotoCart_Click(object sender, EventArgs e)
        {
            Response.Redirect("cart.aspx");
        }





        protected void lbtnStoreQFile_PreRender(object sender, EventArgs e)
        {

        }

        protected void btnStoreQFile_Click(object sender, EventArgs e)
        {
            Order ord = iam.CurOrder;
            ord.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Прайс-лист (подготовлен " + cDate.TodayD.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ")";
            ord.Name = txNameOrder.Text;
            ord.AuthorID = iam.ID;
            bool isNew = ord.IsNew;
            if (isNew) ord.Dg = ord.Subject.CodeDG;
            ord.TypeOrder = "Q";
            bool result = Order.Save(ord);



            if (result)
            {

                TabControl1.SetTabTitle(2, iam.CurOrder.Name);
                lbIDQ.Text = "[#" + ord.ID + "] " + ord.Name;
                lbOrder.Text = "" + ord.ID;
                lbStateOrder.Text = "Набор для прайс-листа сохранен";
                eID = 0;
                iam.CurOrder = null;
                pnlFormOrder.Visible = false;
                pnlResultQ.Visible = true;

            }
        }
        protected void btnNewQ_Click(object sender, EventArgs e)
        {
            pnlFormOrder.Visible = true;
            pnlResultQ.Visible = false;
            eID = 0;
            iam.CurOrder = new Order(eID, iam);
            iam.CurOrder.TypeOrder = "Q";
            iam.CurOrder.SubjectID = iam.PresentedSubjectID;
            iam.CurOrder.State = "";
            Response.Redirect("orderdefault.aspx");
        }

        protected void btnCurrentQ_Click(object sender, EventArgs e)
        {
            pnlFormOrder.Visible = true;
            pnlResultQ.Visible = false;
            eID = cNum.cToInt(lbOrder.Text);
            iam.CurOrder = new Order(eID, iam);
            iam.CurOrder.TypeOrder = "Q";
            iam.CurOrder.SubjectID = iam.PresentedSubjectID;
            iam.CurOrder.State = "";
            Response.Redirect("orderdefault.aspx");
        }

    }
}