using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using System.Data;
using ensoCom;

namespace wstcp
{
    public partial class _default01 : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0)
                Response.Redirect("default.aspx");

            lbMess.Text = "";
            if (!IsPostBack)
            {
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;

                show_news();

                if (iam.IsAdmin || iam.IsSuperAdmin || iam.IsSaller || iam.IsTA)
                {
                    string where = "";
                    if (iam.IsSuperAdmin)
                        where = "1=1";
                    else if (iam.IsAdmin)
                        where = "subj.ownerID=" + iam.OwnerID;
                    else if (iam.IsSaller)
                        where = "subj.ownerID=" + iam.OwnerID + " and " + Bind.SqlExistConditionByMasterObject("subj.id", iam.ThisObject, Subject.TDB);
                    else
                        where = "subj.emailtas like '%" + iam.Email + "%'";

                    DataTable dt = db.GetDbTable("select id, name, (select count(id) from ord where isnull(TypeOrd,'')<>'Q' and subjectid=subj.id and ord.state not in ('X','D','F')) as qtyords from SUBJ as subj where " + where + " order by Name");

                    dlSubjects.Items.Clear();
                    dlSubjects.Items.Add(new ListItem("----------", ""));
                    foreach (DataRow r in dt.Rows)
                        dlSubjects.Items.Add(new ListItem("" + r["Name"] + " (" + r["qtyords"] + ")", "" + r["id"]));
                    try
                    {
                        dlSubjects.Items.FindByValue("" + iam.PresentedSubjectID).Selected = true;
                    }
                    catch
                    {

                    }
                    pnlSubjectFilter.Visible = true;

                }
                else
                {
                    pnlSubjectFilter.Visible = false;
                }


                if (iam.CurOrder != null && (iam.CurOrder.ID == 0 && iam.CurOrder.Items.Count > 0 || iam.CurOrder.Changed))
                    litInfo.Text = "<div class='message center bold'>У вас есть несохраненная Заявка <a href='../order/cart.aspx'>перейти к заявке</a></div>";
                else
                {
                    litInfo.Text = "<div class='message center bold'><a href='../order/orderdefault.aspx'>Перейти к подбору новой заявки</a></div>";
                }

                if (iam.PresentedSubjectID > 100000)
                    show_nearestTeo();

                if (_ACT_ == "avl")
                {
                    show_avaliblegoodies();
                }
                else
                {
                    
                    show_orders();
                }

            }
            else if (hdRefresh.Value == "Y")
            {
                show_orders();
                hdRefresh.Value = "";

            }
        }

        private void show_nearestTeo()
        {
            DataTable dt = db.GetDbTable("select * from ord where isnull(Teotrans,'')<>'' and State not in ('F','D') and subjectId=" + iam.PresentedSubjectID + " and TeoDate>=" + cDate.Date2Sql(cDate.TodayD.AddDays(-10)));
            rpTeo.DataSource = dt;
            rpTeo.DataBind();

            rpTeo.Visible = (dt.Rows.Count > 0);

        }


        private void show_personalTAs()
        {
            Subject subj = new Subject(iam.PresentedSubjectID, iam);
            ((mainpage)this.Master).show_personalTAs(subj);
            ((mainpage)this.Master).show_subj_info(subj);
        }

        private void show_avaliblegoodies()
        {
           
            int subjId = (iam.SubjectID == 0) ? cNum.cToInt(dlSubjects.SelectedValue) : iam.SubjectID;
            DataTable dt = db.GetDbTable("select ordi.OrderId, ord.Code as OrderCode, ord.RegDate, ord.Name as OrderName, ordi.Name as GoodName, ordi.GoodCode,  ordi.Qty, ordi.Realized, ordi.Booking, (ordi.Qty-ordi.Realized) as [Нужно], owng.qty, case when (ordi.Qty-ordi.Realized) <= owng.qty  then (ordi.Qty-ordi.Realized)  else owng.qty end as [Можно] ,ordi.state, ord.State,ord.SubjectID, ordi.lcd, ord.lcd from ORDI inner join owng on owng.goodId=ordi.GoodID and OwnerId=100000 inner join ORD on ord.id=ordi.OrderID and ord.SubjectID=" + subjId + " where ord.State not in ('','D') and (ordi.Qty-ordi.Realized)>0 and owng.qty>=(ordi.Qty-ordi.Realized) and ordi.Booking>0 and ord.RegDate>=" + cDate.Date2Sql(cDate.TodayD.AddMonths(-3)) + " order by OrderID");
            if (dt.Rows.Count > 0)
            {
                rpAvailible.Visible = true;
                rpAvailible.DataSource = dt;
                rpAvailible.DataBind();
            }
            else
            {
                rpAvailible.Visible = false;
            }
        }


        private void show_news()
        {
            DataTable dt = db.GetDbTable("select id, name, descr, convert(nvarchar,regdate,104) as reg_date  from News where isnull(State,'')<>'D' and Published='Y' and EndDate>=getdate() order by regdate desc");
            if (dt.Rows.Count == 0)
                rpNews.Visible = false;
            else
            {
                rpNews.Visible = true;
                rpNews.DataSource = dt;
                rpNews.DataBind();
            }
        }

        string SortField
        {
            get
            {
                if ("" + ViewState["sort"] == "") SortField = "ID desc"; return "" + ViewState["sort"];
            }
            set { ViewState["sort"] = value; }
        }

        string getSelectState()
        {
            switch (rbStateOrders.SelectedValue)
            {
                case "W":
                    return "'U','S'";
                case "Z":
                    return "'','N','Z','A'";
                case "D":
                    return "'X','D'";
                case "F":
                    return "'F'";
                case "R":
                    return "'M','E','R'";
                default:
                    return "";
            }

        }
        void show_orders(int pageIndex = -1)
        {
            string sql;
            string ttt_ord = webInfo.make_where(new List<string>() { "name", "descr" }, txSearch.Text.Trim());
            string ttt_good = webInfo.make_where(new List<string>() { "name", "goodcode" }, txSearch.Text.Trim());




            string selectState = getSelectState();


            sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder,  ord.lcd from " + Order.TDB + " as ord" +
                  " where isnull(TypeOrd,'')='' and ord.SubjectID=" + iam.PresentedSubjectID +
                  ((selectState != "") ? " and ord.State in (" + selectState + ") " : "");
            if (ttt_ord != "" || ttt_good != "")
                sql += " and (" + ((ttt_ord != "") ? ttt_ord : "1=1") + " or " + ((ttt_good != "") ? "ord.id in (select orderid from ORDI where " + ttt_good + ")" : "1=1") + ")";

            sql += " order by ord.id desc";
            DataTable dt = db.GetDbTable(sql);
            //if (dt.Rows.Count == 0 && ttt_good == "" && ttt_ord == "" && dlState.SelectedValue == "")
            //{
            //    lbMsgOrders.Text = "<div class='message center bold'><a href='../order/orderdefault.aspx'>У вас нет ни одной заявки<br>Перейти к подбору новой заявки</a></div>";
            //}
            //else 
            if (dt.Rows.Count == 0)
            {
                lbMsgOrders.Text = "<div class='message center bold'>Ни одна заявка не удовлетворяет запросу,<br/> измените условия поиска</div>";
            }
            else
            {
                lbMsgOrders.Text = "";
            }


            DataView dv = dt.DefaultView;
            dv.Sort = SortField;
            dgList.Visible = true;
            dgList.DataSource = dv;

            if (pageIndex >= 0 && dgList.PageSize * pageIndex <= dt.Rows.Count)
            {
                dgList.CurrentPageIndex = pageIndex;
            }
            else
            {
                dgList.CurrentPageIndex = 0;
            }

            dgList.DataBind();


        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataRowView r = (DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);
                Order rec = new Order(id, iam);
                string state = ("" + r["State"]).ToUpper();
                bool isee = ViewArticle.CheckIseen(iam, new sObject(id, Order.TDB));
                //e.Item.Cells[0].Text = "<input type='checkbox' id='cho_" + id + "' />";
                if (!isee)
                {
                    e.Item.Cells[1].CssClass = "red"; //.Text = "<span class='big red bold' title='Вы еще не посмотрели эту заявку'>*</span>";
                    e.Item.Cells[1].ToolTip = "Вы еще не посмотрели эту заявку";
                    e.Item.Cells[1].Text = "* " + rec.ID;
                    e.Item.Cells[1].Attributes.Add("id", "recnum_" + rec.ID);
                }
                e.Item.Attributes.Add("id", "ord_" + id);
                e.Item.Cells[3].Text = "<a onclick=\"openflywin('../order/detailfly.aspx?id=" + id + "', 870, 850, 'Заявка " + r["Name"] + "')\" href='javascript:return false;'>" + r["Name"].ToString() + "</a>";
                if (rec.OWner.ShowInvDiscount)
                {
                    e.Item.Cells[3].Text += (rec.Discount > 0) ? "<div class='blue small'>предоставлена скидка <span class='bold'>" + cNum.cToDecimal(rec.Discount * 100, 2) + "%</span></div>" : ""; //Order.get_stateorder_descr(state);
                }

                if (rec.State != "D" && rec.State != "F")
                    e.Item.Cells[3].Text += "<div class='small'>" + get_infoorder(rec) + "</div>";


                bool avalibleGet = OrderInfo.CheckAvailibleGet(rec);


                string stateDescr = (avalibleGet) ? "<span class='small'>Весь товар в наличии.</span>" + ((rec.WishDate >= cDate.TodayD) ? "*Получение назначено на " + rec.WishDate.ToShortDateString() : "") : rec.StateDescr;


                e.Item.Cells[5].Text = "<span class='small'>" + stateDescr + "</span>";
                e.Item.Cells[6].Text = "";
                if (state != "D" && state != "F" && rec.ExistInvoice)
                {
                    e.Item.Cells[5].Text += "<a href=\"../downloadfile.ashx?id=" + id + "&act=invoice&sid=" + iam.SessionID + "\"  title=' получить счет на оплату'><img width='48px' height='48px' src='../simg/icoinv.png' /></a>";
                }
                if (state == "U")
                {
                    e.Item.Cells[6].Text += "&nbsp;<a href=\"javascript: return false;\" class='' title='Подтвердить к заказу' onclick=\"javascript: if (confirm('Подтверждаете Подтвердить к заказу?')) {setnewstate('" + id + "','A');}else return false;\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>";
                }

                if (state == "S")
                {
                    e.Item.Cells[6].Text += "&nbsp;<a href=\"javascript: return false;\" class='' title='Согласовать к покупке' onclick=\"javascript: if (confirm('Подтверждаете Согласовать к покупке?')) {setnewstate('" + id + "','M');}else return false;\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>";
                }
                if (state == "R")
                {
                    e.Item.Cells[6].Text += "&nbsp;<a href=\"javascript: return false;\" class='' title='Готов получить товар' onclick=\"openflywin('../order/detailfly.aspx?cmd=wishget&id=" + id + "', 870, 850, 'Заявка " + r["Name"] + "')\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>";
                }
                if (state == "D")
                {
                    e.Item.Cells[6].Text += "&nbsp;<a href=\"javascript: return false;\" class='' title='Вернуть к жизни' onclick=\"javascript: if (confirm('Подтверждаете Возврат к жизни?')) {setnewstate('" + id + "','N');}else return false;\"><img width='48px' height='48px' src='../simg/icorepair.png' /></a>";
                }
                //e.Item.Cells[7].Text += get_buttons(id.ToString(), state);
                //int olddays = cNum.cToInt(SysSetting.GetValue("DAYSOLDORDER"));

                e.Item.Cells[6].Attributes.Add("nowrap", "nowrap");
                e.Item.Cells[7].Attributes.Add("nowrap", "nowrap");
            }
        }

        private string get_infoorder(Order rec)
        {
            return "";
        }



        protected int getQtyOrders(int subjectId, string listStates, string where)
        {
            string flt = (where.Length > 4) ? "%" + where.Replace("  ", " ").Replace(" ", "%") + "%" : where.Replace("  ", " ").Replace(" ", "%") + "%";
            string w = " and (ord.id in (select orderid from ORDI where goodCode like '" + where + "' or Name like '" + flt + "' " + ((where.Length < 4) ? " or Name like '% " + where + "%'" : "") + " ) or (''+ord.id) like '%" + where + "%' or ord.name like '%" + where + "%' or ord.Code like '%" + where + "%')";
            DataTable dt = db.GetDbTable("select count(id) from ORD where isnull(TypeOrd,'')<>'Q' and subjectID=" + subjectId + " and state in (" + listStates + ") " + ((w.Length > 1) ? w : ""));
            return cNum.cToInt(dt.Rows[0][0]);
        }


        

        protected void dlSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {

            iam.PresentedSubjectID = (iam.SubjectID == 0) ? cNum.cToInt(dlSubjects.SelectedValue) : iam.SubjectID;
            if (iam.CurOrder != null && iam.CurOrder.SubjectID != iam.PresentedSubjectID)
                iam.CurOrder = null;
            show_personalTAs();

            show_nearestTeo();

            if (_ACT_ == "avl")
            {
                show_avaliblegoodies();
            }
            else
            {
                show_orders();
            }


        }

        

        protected void btnRempsw_Click(object sender, EventArgs e)
        {
            if (cNum.cToInt(dlSubjects.SelectedValue) > 0)
            {
                lbMess.Text = "";
                foreach (DataRow r in db.GetDbTable("select id from " + pUser.TDB + " where state<>'D' and loginenabled='Y' and subjectid=" + dlSubjects.SelectedValue).Rows)
                {
                    pUser uu = new pUser(cNum.cToInt(r["id"]), iam);
                    EmailMessage mess = new EmailMessage(iam.Email, uu.Email, "santechportal.ru: Напоминалка пароля", "<p>Уважаемый(ая) " + uu.Name + ".</p><p>Вероятно, по Вашей просьбе вам отправлена информация о Вашем пароле доступа в santechportal.ru.</p><p>Ваш пароль: <strong style='font-family: Courier New'>" + uu.Psw + "</strong></p><p>напоминаем Ваш логин: <strong>" + uu.Email + "</strong></p><br/><br/><p>ссылка для входа в портал <a href='http://santechportal.ru'>http://santechportal.ru</a></p>");
                    mess.Send();
                    lbMess.Text += "<p>на электронный адрес " + uu.Email + " (" + uu.Name + ") отправлено напоминание о пароле.</p>";
                }
            }
        }

        protected void lbtnSet2Arch_Click(object sender, EventArgs e)
        {
            int subjId = (iam.SubjectID == 0) ? cNum.cToInt(dlSubjects.SelectedValue) : iam.SubjectID;
            db.ExecuteCmd("update ORD set state='D' where lcd<DATEADD(MM,-1,getdate()) and state in ('u','s') and Subject=" + subjId);
            show_orders();
        }


        
        



        
        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            show_orders();
        }

        protected void lbtnCloseWin_Click(object sender, EventArgs e)
        {
            Session["IseeArch"] = "1";
            
        }



        public void Show_queries(int pageIndex = -1)
        {

            
            if ("" + Session["taborders"] == "")
                Session["taborders"] = "price";

            string selectTab = "" + Session["taborders"];

      


            string sql;
            string ttt_ord = webInfo.make_where(new List<string>() { "name", "descr" }, txSearch.Text.Trim());
            string ttt_good = webInfo.make_where(new List<string>() { "name", "goodcode" }, txSearch.Text.Trim());







            sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder,  ord.lcd from " + Order.TDB + " as ord" +
                  " where state not in ('X','D') and isnull(TypeOrd,'')='Q' and ord.SubjectID=" + iam.PresentedSubjectID;
            if (ttt_ord != "" || ttt_good != "")
                sql += " and (" + ((ttt_ord != "") ? ttt_ord : "1=1") + " or " + ((ttt_good != "") ? "ord.id in (select orderid from ORDI where " + ttt_good + ")" : "1=1") + ")";

            sql += " order by ord.id desc";
            DataTable dt = db.GetDbTable(sql);
            //if (dt.Rows.Count == 0 && ttt_good == "" && ttt_ord == "")
            //{
            //    lbQMsg.Text = "<div class='message center bold'>У вас нет ни одной сохраненного запроса<br><a href='../order/orderdefault.aspx'>Перейти к подбору</a></div>";
            //}
            //else if (dt.Rows.Count == 0)
            //{
            //    lbQMsg.Text = "<div class='message center bold'>Ни один запрос не удовлетворяет условию,<br/> измените условия поиска</div>";
            //}
            //else
            //{
            //    lbQMsg.Text = "";
            //}


            DataView dv = dt.DefaultView;
            //dv.Sort = SortField;
            dgQuery.Visible = true;
            dgQuery.DataSource = dv;

            if (pageIndex >= 0 && dgQuery.PageSize * pageIndex <= dt.Rows.Count)
            {
                dgQuery.CurrentPageIndex = pageIndex;
            }
            else
            {
                dgQuery.CurrentPageIndex = 0;
            }

            dgQuery.DataBind();


        }
        protected void btnQSeach_Click(object sender, EventArgs e)
        {
            Show_queries(0);
        }

        protected void dgQuery_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataRowView r = (DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);
                Order rec = new Order(id, iam);
                string state = ("" + r["State"]).ToUpper();
                bool isee = ViewArticle.CheckIseen(iam, new sObject(id, Order.TDB));
                e.Item.Attributes.Add("id", "ord_" + id);
                e.Item.Cells[3].Text = "<a onclick=\"openflywin('../order/quefly.aspx?id=" + id + "', 870, 850, '" + r["Name"] + "')\" href='javascript:return false;'>" + r["Name"].ToString() + "</a>";
                e.Item.Cells[4].Text = "" + get_infoquery(rec) + "";

                // e.Item.Cells[6].Text = "";
                //if (state != "D" && state != "F" && webIO.CheckExistFile("../exch/" + id + ".csv"))
                //{
                //    e.Item.Cells[5].Text += "<a href=\"../exch/" + id + ".csv\" target=\"_blank\" title=' получить прайс-лист'><img width='48px' height='48px' src='../simg/16/xlsx16.gif' />скачать</a>";
                //}
                if (state == "X" || state == "D")
                    e.Item.Attributes.Add("class", "deleted");
                e.Item.Cells[5].Text += get_qbuttons(id.ToString(), state);
                //e.Item.Cells[6].Attributes.Add("nowrap", "nowrap");
                e.Item.Cells[5].Attributes.Add("nowrap", "nowrap");
            }
        }
        private string get_infoquery(Order rec)
        {

            DataTable dt = db.GetDbTable("select good.Brend,count(*) from ordi inner join good on ordi.goodId=good.id where ordi.orderid=" + rec.ID + " group by good.Brend");
            string brend = "";
            foreach (DataRow r in dt.Rows)
                cStr.AddUnique(ref brend, " " + r[0] + " (" + r[1] + ")");

            dt = db.GetDbTable("select good.xtk,count(*) from ordi inner join good on ordi.goodId=good.id where ordi.orderid=" + rec.ID + " group by good.xtk");
            string tk = "";
            foreach (DataRow r in dt.Rows)
                cStr.AddUnique(ref tk, " " + r[0] + " (" + r[1] + ")");
            string res = "";
            res = (brend != "") ? "<p>бренды: " + brend + "</p>" : "";
            res += (tk != "") ? "<p>категории: " + tk + "</p>" : "";
            return res;


        }

        private string setbtns(string orderId, string act, string newstate, string pic, string btnword, string title = "", string css = "")
        {
            string btn = "<a href=\"#\" class='microbutton micro {css}' title='{btn}' onclick=\"javascript: if (confirm('Подтверждаете {btn}?')) {setnewstate('{orderid}','{newstate}');}else return false;\"><img src='../simg/16/{pic}' />{title}</a>";
            return btn.Replace("{orderid}", orderId).Replace("{newstate}", newstate).Replace("{pic}", pic).Replace("{btn}", btnword).Replace("{act}", act).Replace("{title}", title).Replace("{css}", css);
        }
        private string get_qbuttons(string orderId, string s)
        {
            string btn = "";
            string btn_del = "&nbsp;" + setbtns(orderId, "sns", "X", "delete.png", "отменить/в архив");
            string btn_copy = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=copy\" class='microbutton micro' title='Скопировать/повторить ' ><img src='../simg/16/page_copy.png' /></a>";
            string btn_edit = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=edit\" class='microbutton micro' title='Изменить' ><img src='../simg/16/document-edit.png' /></a>";
            btn = btn_edit + btn_copy + btn_del;

            return btn;
        }

        protected void dgQuery_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            Show_queries(e.NewPageIndex);
        }

        protected void dgQuery_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                e.Item.Cells[0].Controls.AddAt(0, new LiteralControl("Страницы "));
                e.Item.Cells[0].CssClass += " center";
            }
        }


        protected void dgList_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            switch (e.SortExpression)
            {
                case "regdate":
                    SortField = (SortField == "regdate, id") ? "regdate desc, id desc" : "regdate, id";
                    break;
                case "summ":
                    SortField = (SortField == "summorder, id") ? "summorder desc, id desc" : "summorder, id";
                    break;
                case "id":
                default:
                    SortField = (SortField == "id") ? "id desc" : "id";
                    break;
            }
            show_orders(0);
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            show_orders(e.NewPageIndex);
        }

        protected void dgList_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                e.Item.Cells[0].Controls.AddAt(0, new LiteralControl("Страницы "));
                e.Item.Cells[0].CssClass += " center";
            }
        }





    }
}