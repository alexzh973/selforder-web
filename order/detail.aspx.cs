using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using ensoCom;
using System.Data;

namespace wstcp.order
{
    public partial class detail : p_p
    {
        Order RECORD;

        int SubjectId
        {
            get { return cNum.cToInt(ViewState["SubjectID"]); }
            set { ViewState["SubjectID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0)
            {
                Response.Write("Запрещено без авторизации");
                Response.End();
                return;
            }
            ((mainpage)Master).SelectedMenu = "order";
            ((mainpage)Master).VisibleLeftPanel = false;
            ((mainpage)Master).VisibleRightPanel = false;
            if (!IsPostBack)
            {
                eID = cNum.cToInt(Request["id"]);
                linkToCart.NavigateUrl = "cart.aspx?id=" + eID;
            }

            if (RECORD == null) RECORD = new Order(eID, iam);

            if (!IsPostBack)
            {
                SubjectId = RECORD.SubjectID;
                show_record();
                show_blnc();
                if (Request["cmd"] == "wishget")
                {
                    mvCmd.SetActiveView(vQuestDate);
                    lbtnNextStady.Visible = false;
                    ucWishDate.SelectedDate = (RECORD.WishDate == cDate.DateNull) ? cDate.AddWorkDays(cDate.TodayD, 2) : RECORD.WishDate;
                }
            }
            CommentList1.DataObject = new sObject(eID, Order.TDB);
            CommentList1.CanAddComment = true;

            CommentList1.Event_AddedComment = new EventHandler(this.CommentList1_AddingComment);
        }


        private void CommentList1_AddingComment(object sender, EventArgs e)
        {

            string emails = RECORD.Subject.EmailTAs;
            EmailMessage m = new EmailMessage(iam.Email, emails, "новый комментарий к Заявке №" + RECORD.ID + " от " + RECORD.RegDate.ToShortDateString() + " (код 1С " + RECORD.Code + ")", CommentList1.Text);
            m.Send();
        }



        private void show_record()
        {

            ViewArticle.AddView(iam, RECORD.ThisObject);
            string script = "";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "setActPunct", script, false);



            lbSubject.Text = RECORD.Subject.Name + ", ИНН " + RECORD.Subject.INN;
            lbTitle.Text = RECORD.Name;
            lbAttr.Text = "№" + RECORD.ID + " от " + RECORD.RegDate.ToShortDateString();
            lbCode.Text = RECORD.Code;
            rbTeoVariant.SelectedValue = (RECORD.TEONeed) ? "teo" : "self";
            teoaddress.Visible = RECORD.TEONeed;
            txTEOAddress.Text = RECORD.TEOAddress;
            if (RECORD.TEOTrans != "")
            {
                pnlTrans.Visible = true;
                lbTeoDate.Text = RECORD.TEODate.ToShortDateString();
                lbTEOTrans.Text = RECORD.TEOTrans;
                lbTeoAddress.Text = RECORD.TEOAddress;
            }
            else
            {
                pnlTrans.Visible = false;
            }
            if (RECORD.Subject.EmailTAs != "")
            {
                pUser ta = new pUser(pUser.FindByField("email", RECORD.Subject.EmailTAs), iam);
                lbTAs.Text = "";
                lbTAs.Text = ta.Name + " (" + ta.Email + ", " + ta.Phones + ")";
            }
            
            int qZ = RECORD.Items.Where(x => x.Zn != "S").Count();
            string reasonNoInvoice = "";
            if (qZ > 0 && !RECORD.ExistInvoice)
            {
                linkInvoice.Text += "<p class='message small'>В заявке есть заказные позиции, поэтому счет на оплату можно получить только у своего менеджера.</p>";
                reasonNoInvoice = "В заявке есть заказные позиции, поэтому счет на оплату можно получить только у своего менеджера.";
                lbtnRequestInvoice.Visible = false;
            } else
            {
                reasonNoInvoice = "Прикрепленный счет на оплату отсутствует";
            }
            if (RECORD.State != "D" && RECORD.State != "F" && RECORD.ExistInvoice)
            {
                linkInvoice.Text = "<a href=\"../downloadfile.ashx?act=invoice&id=" + RECORD.ID + "&sid=" + iam.SessionID + "\" title='получить счет на оплату'><img src='../simg/icoinv.png' /></a>";
            }
            else
            {
                linkInvoice.Text = "<img src='../simg/icoinvno.png' title='" + reasonNoInvoice + "' />";
            }
            
            //lbLock.Text = getLockComment(RECORD.State.ToUpper());

            lbDescr.Text = RECORD.Descr;

            lbState.Text = RECORD.StateDescr;

            DataGrid1.DataSource = RECORD.Items;
            DataGrid1.DataBind();

            //ucWishDate.ExceptWeekend = true;


            if (RECORD.WishDate >= cDate.TodayD)
                lbWishdate.Text = "<div>желаемая дата отгрузки <span class='bold'>" + RECORD.WishDate.ToShortDateString() + "</span></div>";
            else if (RECORD.WishDate != cDate.DateNull && RECORD.WishDate < cDate.TodayD)
                lbWishdate.Text = "<div>желаемая дата отгрузки была <span class='bold'>" + RECORD.WishDate.ToShortDateString() + "</span></div>";

            bool existZ = (RECORD.Items.FindAll(x => x.Zn != "S" && x.Realized < x.Qty).Count > 0);

            mvCmd.SetActiveView(vBtns);

            string State = RECORD.State.ToUpper();



            if (!(iam.SubjectID == RECORD.SubjectID || iam.IsSuperAdmin || IamServices.GetListMySubjectsID(iam).Contains(RECORD.SubjectID)))
            {
                mvCmd.SetActiveView(vMsgSuccess);
                lbSuccess.Text = "У вас нет прав управлять этой заявкой";
                return;
            }
            else
            {
                if (Request["cmd"] == "wishget")
                {
                    mvCmd.SetActiveView(vQuestDate);
                    lbtnNextStady.Visible = false;
                    ucWishDate.SelectedDate = cDate.AddWorkDays(cDate.TodayD, 2);
                    return;
                }
                else
                {
                    mvCmd.SetActiveView(vBtns);
                }
            }



            bool availGet = (State != "D" && State != "F") ? OrderInfo.CheckAvailibleGet(RECORD) : false;

            switch (State)
            {
                case "F":
                    lbLock.Text = "Заявка закрыта.";
                    lbtnNextStady.Visible = false;
                    lbtnCancel.Visible = false;
                    break;
                case "X":
                case "D":
                    lbLock.Text = "Заявка отменена. Ее можно восстановить, и тогда она будет восприниматься как новая заявка.";
                    lbtnCancel.Visible = true;
                    lbtnCancel.CommandArgument = "N";
                    lbtnCancel.Attributes.Add("onclick", "javascript: return myConfirm('Вы подтверждаете восстановление заявки?');");
                    //lbtnCancel.Text = "<img src='../simg/icorepair.png' title='Вернуть к жизни'/>";
                    lbtnCancel.Text = "<img src='../simg/16/arrow-left.png'/> Вернуть к жизни";
                    lbtnNextStady.Visible = false;
                    break;
                case "":
                case "N":
                    lbLock.Text = "";
                    lbtnNextStady.Visible = false;
                    lbtnCancel.Visible = false;
                    break;
                case "U":
                    lbLock.Text = "Заявка предварительно обработана в Сантехкомплекте.";
                    if (availGet)
                    {
                        if (RECORD.WishDate == cDate.DateNull || RECORD.WishDate < cDate.TodayD)
                        {
                            lbtnNextStady.Visible = true;
                            //lbtnNextStady.Text = "<img src='../simg/iconext.png' title='Готов получить товар!'/>";
                            lbtnNextStady.Text = "Готов получить товар <img src='../simg/16/fast-forward.png'/>";
                            lbtnNextStady.CommandArgument = "E";
                            lbLock.Text = "Весь товар есть в наличиии. Вы можете все получить в самое ближайшее время. Если готовы получить, то давите на кнопку:"; // \"Ujnjd gjkexbnm товар\"";
                        }
                    }
                    else
                    {
                        if (existZ)
                            lbLock.Text += " В Заявке есть заказные товары, поэтому требуется Ваше предварительное подтверждение заявки.";
                        else
                            lbLock.Text += " Если Вас устраивают предложенные цены, то требуется Ваше предварительное подтверждение заявки.";

                        lbtnNextStady.Visible = true;
                        //lbtnNextStady.Text = "<img src='../simg/iconext.png' title='Подтвердить к заказу'/>";
                        lbtnNextStady.Text = "Подтверждаю заказ <img src='../simg/16/document-accept.png'/>";
                        lbtnNextStady.CommandArgument = "A";
                    }
                    break;
                case "S":
                    if (availGet)
                    {
                        if (RECORD.WishDate == cDate.DateNull || RECORD.WishDate < cDate.TodayD)
                        {
                            lbLock.Text = "Весь товар есть в наличиии. Вы можете все получить в самое ближайшее время. Если готовы получить, то давите на кнопку:";// \"Далее\"";
                            lbtnNextStady.Visible = true;
                            //lbtnNextStady.Text = "<img src='../simg/iconext.png' title='Готов получить товар!'/>";
                            lbtnNextStady.Text = "Готов получить товар <img src='../simg/16/fast-forward.png'/>";
                            lbtnNextStady.CommandArgument = "E";
                        }
                        else
                        {
                            param_getting();
                        }
                    }
                    else
                    {
                        lbLock.Text = "Заявка прошла полное согласование в Сантехкомплекте.";
                        lbLock.Text += " Если Вас устраивают предложенные цены, то требуется Ваше окончательное подтверждение.";

                        lbtnNextStady.Visible = true;
                        //lbtnNextStady.Text = "<img src='../simg/iconext.png' title='Подтвердить к покупке'/>";
                        lbtnNextStady.Text = "Подтвердаю покупку <img src='../simg/16/document-accept.png'/>";
                        lbtnNextStady.CommandArgument = "M";
                    }
                    break;
                case "Z":
                case "A":
                    lbLock.Text = "Заявка в процессе Согласования в Сантехкомплекте. Пока никакие действия невозможны.";
                    lbtnNextStady.Visible = false;
                    break;
                case "M":
                case "E":
                    if (RECORD.WishDate == cDate.DateNull || RECORD.WishDate < cDate.TodayD)
                    {
                        if (availGet)
                        {
                            lbLock.Text = "Весь товар есть в наличиии. Вы можете все получить в самое ближайшее время. Если готовы получить, то давите на кнопку:";// \"Далее\"";
                            lbtnNextStady.Visible = true;
                            //lbtnNextStady.Text = "<img src='../simg/iconext.png' title='Готов получить товар!'/>";
                            lbtnNextStady.Text = "Готов получить товар <img src='../simg/16/fast-forward.png'/>";
                            lbtnNextStady.CommandArgument = "E";
                        }
                        else
                        {
                            lbLock.Text = "Заявка на стадии подготовки к комплектации.";
                            lbtnNextStady.Visible = false;
                        }
                    }
                    else
                    {
                        param_getting();
                    }
                    break;

                case "R":
                    lbLock.Text = "Заявка в процессе комплектации";

                    lbtnNextStady.Visible = false;
                    break;
            }
           
            bool delPossible = OrderInfo.DeletePossible(RECORD); 

            if (delPossible && cStr.Exist(RECORD.State.ToUpper(), new string[] { "N", "U", "A", "Z", "S", "R" }))
            {
                lbtnCancel.Visible = true;
                lbtnCancel.CommandArgument = "D";
                lbtnCancel.Text = "<img src='../simg/16/delete.png' title='Отменить заявку'/> Отменить";
                lbtnCancel.Attributes.Add("onclick", "javascript: return myConfirm('Вы подтверждаете отмену/закрытие заявки?');");
            }
            else if (RECORD.State.ToUpper() == "D")
            {
                lbtnCancel.Visible = true;
                lbtnCancel.CommandArgument = "N";
                lbtnCancel.Attributes.Add("onclick", "javascript: return myConfirm('Вы подтверждаете восстановление заявки?');");
                //lbtnCancel.Text = "<img src='../simg/icorepair.png' title='Вернуть к жизни'/>";
                lbtnCancel.Text = "<img src='../simg/16/arrow-left.png'/> Вернуть к жизни";
            }
            else
            {
                lbtnCancel.Visible = false;
                lbtnCancel.CommandArgument = "";
                lbtnCancel.Text = "...";
            }
            if ((!OrderInfo.DeletePossible(RECORD)  || RECORD.TEOTrans != "") && RECORD.State != "" && RECORD.State != "N")
            {
                lbtnCancel.Visible = false;
                lbtnCancel.CommandArgument = "";
                lbtnCancel.Text = "";
            }
            // пока не включаем:
            //lbtnNextStady.Visible = lbtnCancel.Visible = false;

            if ((iam.SubjectID == RECORD.SubjectID || iam.IsIamSallerForSubject(RECORD.SubjectID)) && (RECORD.State.ToUpper() == "" || RECORD.State.ToUpper() == "U"))
                linkToCart.Visible = true;
            else
                linkToCart.Visible = false;
                
            
        }



        private void param_getting()
        {
            lbLock.Text = "<p>Получение товара запрошено на " + RECORD.WishDate.ToShortDateString() + "</p><p>Для уточнения информации свяжитесь со своим менеджером</p>";
            if (RECORD.TEONeed)
            {
                lbLock.Text += "<p>Адрес доставки: " + RECORD.TEOAddress + "</p>";
            }
            else
            {
                Owner own = new Owner(RECORD.OwnerID);
                lbLock.Text += "<p>Самовывоз. Адрес: " + own.Address + "</p>";
            }
            if (RECORD.TEOTrans != "")
            {
                lbLock.Text += "<p>Назначен автомобиль " + RECORD.TEOTrans + "</p>";
            }
        }




        public static string get_info_enoutgh(decimal need_qty, decimal incash)
        {
            return (need_qty < incash) ? "на складе в наличии" : (need_qty == incash) ? "возможно хватит если быстро" : (incash > 0) ? "можно получить только " + incash : "нет на складе";
        }

        public static string get_ost_info(string current_state, string zn, decimal incash, decimal qty_in_order, decimal booking, decimal realized, DateTime WDate, string comment, int srokPost)
        {

            string osttext = "";
            decimal ost = qty_in_order - realized;

            if (current_state.ToUpper() == "F" || ost == 0)
            {
                osttext = "<div>полностью отгружено!</div>";
            }
            else if (current_state.ToUpper() == "R")
            {
                if (realized > 0) osttext = "<div>осталось отгр. " + ost + "</div>";


                if (booking > 0)
                    osttext += "<div>можно получить сегодня " + booking + "</div>";
                else // booking==0
                {
                    if (zn.ToUpper() == "S")
                    {
                        osttext += "<div>" + get_info_enoutgh(qty_in_order, incash) + "</div>";
                    }
                    else if (zn == "del")
                    {
                        osttext += "<div class='red'>выведена из ассортимента, обратитесь за заменой</div>";
                    }
                    else // zn == "Z"
                    {

                        if (incash > 0)
                            osttext += "<div title='Позиция заказная, согласуйте возможность отгрузки с менеджером'><span class='red'>*</span> " + get_info_enoutgh(ost, incash) + "</div>";
                        else if (WDate != cDate.DateNull)
                            osttext += "<div>ожидается " + cDate.cToString(WDate) + "</div>";
                        else if (srokPost > 0)
                            osttext += "<div title='Срок отсчитывается со дня подтверждения клиентом'> срок пост. " + srokPost + "дн.</div>";

                    }
                }
            }
            else
            {
                if (zn.ToUpper() == "S")
                {
                    osttext += (incash > 0) ?
                        "<div>" + get_info_enoutgh(qty_in_order, incash) + "</div>"
                        : ((WDate > cDate.DateNull) ? "<div>возможная дата поставки " + cDate.cToString(WDate) + "</div>" : "<div>срок поставки уточнить у менеджера</div>");
                }
                else // Z
                {
                    if (incash > 0)
                    {
                        osttext += "<div title='Позиция заказная, но на свободном остатке есть, обратитесь к своему менеджеру'><span class='red'>*</span> " + get_info_enoutgh(qty_in_order, incash) + "</div>";


                    }
                    else
                    {
                        if (WDate > cDate.DateNull)
                            osttext += "<div>под заказ, возможная дата " + cDate.cToString(WDate) + " </div>";
                        else
                            osttext += (srokPost > 0) ?
                            "<div title='Срок отсчитывается со дня подтверждения клиентом'>" + srokPost + "дн.</div>"
                            :
                            "<div>под заказ, срок поставки уточнить у менеджера</div>";
                    }


                }

            }
            return osttext;

        }


        protected void DataGrid1_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {

                OrderItem r = (OrderItem)e.Item.DataItem;
                string osttext = get_ost_info(RECORD.State, r.Zn, r.CurIncash, r.Qty, r.Booking, r.Realized, r.WDate, r.Comment, r.SrokPost);
                e.Item.Cells[6].Text = osttext;

                if (osttext.IndexOf("в наличии") > -1 || osttext.IndexOf("сегодня") > -1)
                {
                    e.Item.CssClass += "bold";
                }
                if (osttext.IndexOf("полностью отгружено!") > -1)
                    e.Item.CssClass += " fullreal";
                if (cStr.Exist(RECORD.State, new[] { "", "N", "U", "Z", "S" }) && "" + r.Zn == "Z" && r.PriceActual > cDate.DateNull)
                {
                    e.Item.Cells[2].CssClass = "red";
                    e.Item.Cells[2].ToolTip = "цена действительна до " + cDate.cToString(r.PriceActual);
                }

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                e.Item.Cells.Clear();
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells.Add(new TableCell());
                e.Item.Cells[0].Attributes.Add("colspan", "4");
                e.Item.Cells[2].Attributes.Add("colspan", "4");
                decimal smb = RECORD.SummBase;
                decimal sm = RECORD.Summ;
                decimal d = RECORD.Discount;
                if (d > 0 && RECORD.OWner.ShowInvDiscount)
                {
                    //e.Item.Cells[0].Text = "СУММА без скидки<br/> ";
                    //e.Item.Cells[1].Text = "" + cNum.cToDecimal(RECORD.SummBase, 2) + "руб.<br/>";
                    e.Item.Cells[0].Text += "СУММА со скидкой<br/>";
                    e.Item.Cells[1].Text += "" + cNum.cToDecimal(RECORD.Summ, 2) + "руб.<br/>";
                    e.Item.Cells[0].Text += "предоставлена скидка";
                    e.Item.Cells[1].Text += "" + cNum.cToDecimal(d * 100, 2) + "%";

                }
                else
                {
                    e.Item.Cells[0].Text = "СУММА";
                    e.Item.Cells[1].Text = "" + cNum.cToDecimal(RECORD.Summ, 2) + "руб.";
                }

            }
        }

        protected void lbtnCancel_Click(object sender, EventArgs e)
        {
            //if (RECORD == null)
            //    RECORD = new Order(eID, iam);
            if (lbtnCancel.CommandArgument == "D")
            {
                if (Order.Delete(RECORD, iam))
                {
                    show_record();
                    lbtnNextStady.Visible = false;
                    lbLock.Text = "";
                    linkInvoice.Visible = false;
                }
            }
            else if (lbtnCancel.CommandArgument == "N")
            {
                RECORD.State = "N";
                Order.Save(RECORD);
                show_record();
            }
        }

        protected void lbtnNextStady_Click(object sender, EventArgs e)
        {
            //if (RECORD == null) RECORD = new Order(eID, iam);


            if (lbtnNextStady.CommandArgument == "A")
            {
                RECORD.State = "A";
                Order.Save(RECORD);

                Session["taborders"] = "actualAZ";
                show_record();
            }
            else if (lbtnNextStady.CommandArgument == "M")
            {
                RECORD.State = "M";
                Order.Save(RECORD);
                Session["taborders"] = "work";
                show_record();
            }
            else if (lbtnNextStady.CommandArgument == "E")
            {
                mvCmd.SetActiveView(vQuestDate);

                ucWishDate.SelectedDate = cDate.AddWorkDays(cDate.TodayD, 2);
            }
            //lbtnNextStady.Visible = false;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            RECORD = new Order(eID, iam);

            show_record();
        }

        protected void show_blnc()
        {
            DgInfo1.ShowInfo(RECORD.Dg, RECORD.OwnerID);
        }

        protected void btnCancelCmd_Click(object sender, EventArgs e)
        {
            mvCmd.SetActiveView(vBtns);

        }

        protected void btnSendCmd_Click(object sender, EventArgs e)
        {
            lbMess.Text = "";
            bool valid = true;

            valid = checkWishDate(ucWishDate.SelectedDate);

            if (rbTeoVariant.SelectedValue == "teo" && txTEOAddress.Text.Trim().Length < 10)
            {
                lbMess.Text += "<p>Адрес не указан либо некорректен.</p>";
                valid = false;
            }

            if (!valid)
                return;
            //if (RECORD == null)
            //    RECORD = new Order(eID, iam);

            RECORD.WishDate = ucWishDate.SelectedDate;
            RECORD.TEONeed = (rbTeoVariant.SelectedValue == "teo");
            RECORD.TEOAddress = txTEOAddress.Text;


            if (!cStr.Exist(RECORD.State, new string[] { "R", "D", "F" }))
                RECORD.State = "M";
            Order.Save(RECORD);

            param_getting();


            lbWishdate.Text = "<div>желаемая дата отгрузки <span class='bold'>" + RECORD.WishDate.ToShortDateString() + "</span></div>";

            DGinfo d = Subject.GetDgInfo(RECORD.Dg, RECORD.OwnerID);

            bool moneyenough = ((d.CurrentDZ + d.LimitDZ) >= RECORD.Summ);//((d.LimitDZ - d.CurrentDZ) > 0);

            string emails = (IsDev) ? "alexzh@santur.ru" : RECORD.Subject.EmailTAs;

            EmailMessage m = new EmailMessage(iam.Email, emails, "Заказ № " + RECORD.Code + " от " + RECORD.RegDate.ToShortDateString() + ": Готовность к отгрузке ", "<p>Клиент выразил желание получить товар по Заказу покупателя № " + RECORD.Code + "</p><p>" + ((rbTeoVariant.SelectedValue == "teo") ? "Клиент хочет Доставку (нужно решить по ее стоимости) по адресу " + txTEOAddress.Text : "Доставка не требуется") + "</p><p>" + ((!moneyenough) ? "Внимание! Денег недостаточно ..." : "") + "</p>");
            m.Send();

            lbSuccess.Text = "<p>Запрос на отгрузку отправлен Вашему менеджеру.</p><p>В ближайшее время он свяжется с Вами для уточненния деталей.</p>";


            Session["taborders"] = "work";

            mvCmd.SetActiveView(vMsgSuccess);

            //btnClosemess.Visible = true;
           
        }



        

        protected void lbtnChangeWishDate_Click(object sender, EventArgs e)
        {
            //if (RECORD == null) RECORD = new Order(eID, iam);
            mvCmd.SetActiveView(vQuestDate);
            ucWishDate.SelectedDate = RECORD.WishDate;
            rbTeoVariant.SelectedValue = (RECORD.TEONeed) ? "teo" : "self";
            teoaddress.Visible = RECORD.TEONeed;

            txTEOAddress.Text = RECORD.TEOAddress;
        }

        protected void chNeedTrans_CheckedChanged(object sender, EventArgs e)
        {
            teoaddress.Visible = (rbTeoVariant.SelectedValue == "teo");
            if (teoaddress.Visible && txTEOAddress.Text == "")
            {
                DataTable dt = db.GetDbTable("select top 3 isnull(TEOAddress,'') as adr from ORD where SubjectID=" + SubjectId + " and isnull(TEOAddress,'')<>'' order by id desc");
                if (dt.Rows.Count > 0)
                    txTEOAddress.Text = "" + dt.Rows[0][0];

            }
        }

        private bool checkWishDate(DateTime date)
        {
            if (ucWishDate.SelectedDate < cDate.TodayD || (ucWishDate.SelectedDate.Date == cDate.TodayD.Date && DateTime.Now.Hour > 12) || cDate.DayOfWeek(ucWishDate.SelectedDate) > 5)
            {
                lbMessDate.Text = "Пожалуйста измените выбор даты. Ближайшая выбранная дата может быть ";
                if (cDate.DayOfWeek(cDate.TodayD) == 5)
                    lbMessDate.Text += "" + cDate.TodayD.AddDays(3).ToShortDateString();
                else if (cDate.DayOfWeek(cDate.TodayD) == 6)
                    lbMessDate.Text += "" + cDate.TodayD.AddDays(2).ToShortDateString();
                else
                    lbMessDate.Text += "" + cDate.TodayD.AddDays(1).ToShortDateString();

            }
            else
                lbMessDate.Text = "";

            return lbMessDate.Text == "";
        }

        protected void ucWishDate_SelectionChanged(object sender, SelectionDateChangedEventArgs e)
        {
            checkWishDate(ucWishDate.SelectedDate);

        }

        protected void lbtnRequestInvoice_Click(object sender, EventArgs e)
        {
            if (RECORD.State != "D" && RECORD.State != "F" )
            {
                linkInvoice.Text = "Запрос на обновление счета отправлен, через пару минут он будет прикреплен";
                db.ExecuteCmd("update " + Order.TDB + " set invoice='N', invoicesrc=null where id=" + RECORD.ID);
                
            }
            else
            {
                linkInvoice.Text = "";
            }
            List<OrderItem> iav = RECORD.Items.FindAll(x => x.Zn != "S");
            if (iav.Count > 0)
                linkInvoice.Text += "<p class='message small'>В заявке есть заказные позиции, поэтому счет на оплату можно получить только у своего менеджера.</p>";

        }

    }
}