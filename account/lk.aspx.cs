using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using selforderlib;
using System.Data;

namespace wstcp.account
{
    public partial class lk : p_p
    {
        string SortField
        {
            get
            {
                if ("" + ViewState["sort"] == "") SortField = "ID desc"; return "" + ViewState["sort"];
            }
            set { ViewState["sort"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0) Response.Redirect("~/default.aspx");
            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "lk";
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;

                load_ordstatuses();

            }

            TabControl1.InitTabContainer("orderlistlk");

            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Заявки", pnlOrders);
            TabControl1.AddTabpage("Прайс-листы (запросы)", pnlQueries);
            TabControl1.AddTabpage("Профайл клиента", pnlProfile);


            if (!IsPostBack || TabControl1.ChangedTabpage)
            {

                if (TabControl1.IsActiveTabpage(pnlProfile))
                {
                    load_profile();
                    Load_balance();
                }
                else if (TabControl1.IsActiveTabpage(pnlOrders))
                {
                    Show_orders();
                }
                else
                {
                    Show_queries();
                }
            }
        }

        private void load_ordstatuses()
        {
            dlState.Items.Clear();
            dlState.Items.Add(new ListItem("Все актуальные заявки ", "'A','Z','U','','N','E','S','R','M'"));
            dlState.Items.Add(new ListItem("Все ожидающие подтверждения Клиента", "'U','S'"));
            //dlState.Items.Add(new ListItem("Ожидание предварительного подтверждения клиента","'U','','N'"));
            dlState.Items.Add(new ListItem("На согласовании в Сантехкомплекте", "'A','Z'"));
            //dlState.Items.Add(new ListItem("Ожитание окончательного подтверждения клиентом","'S'"));
            dlState.Items.Add(new ListItem("Готовые к отгрузке", "'R','M'"));
            dlState.Items.Add(new ListItem("Выполненные", "'F'"));
            dlState.Items.Add(new ListItem("Отмененные", "'D'"));

        }

        private void Load_balance()
        {

        }

        private void load_profile()
        {
            Subject subj = new Subject(iam.PresentedSubjectID, iam);
            lbSubjName.Text = subj.Name;
            lbSubjINN.Text = subj.INN;
            pUser u = new pUser(iam.ID, iam);
            txIamName.Text = u.Name;
            txIamEmail.Text = u.Email;
            txIamPhone.Text = u.Phones;

            DgInfo1.ShowInfo(subj.CodeDG, subj.OwnerID);


            //DGinfo d = Subject.GetDg(subj.ID, subj.CodeDG);
            //lbDG.Text = (d.Num != "") ? d.Num + " срок действия " + d.StartDateS + " - <span " + ((cDate.GetDayDistance(cDate.TodayD, d.EndDate) < 14) ? "class='red'" : "") + " >" + d.EndDateS + "</span>" : d.CodeDogovor;
            //lbDgAgree.Text = (d.LimitDZ > 0) ? "Лимит дебиторской задолженности по договору " + d.LimitDZ + "руб., с отсрочкой " + d.PayOtsrok + " дней" : "100% предоплата";
            //lbCurrBlnc.Text = "" + ((d.CurrentDZ >= 0) ? ((d.CurrentDZ == 0) ? "0.00руб" : "<span class='red'>-" + d.CurrentDZ + "руб</span>") : "<span class='green'>+" + (d.CurrentDZ * -1) + "руб</span>");
            //lbAvail.Text = (((d.LimitDZ - d.CurrentDZ) > 0) ? "<span class='green'>ВОЗМОЖНА ОТГРУЗКА НА СУММУ " + (d.LimitDZ - d.CurrentDZ) + "руб.</span>" : "<span class='red'>без предоплаты отгрузка невозможна</span>");
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            txIamName.ReadOnly =
            txIamEmail.ReadOnly =
            txIamPhone.ReadOnly = false;
            btnChange.Visible = false;
            btnSave.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (iam.Email.ToLower() != txIamEmail.Text.ToLower().Trim() && pUserInfo.Exist(txIamEmail.Text.Trim()))
            {
                lbMess.Text = "С указанным email уже зарегистрирован пользователь";
                return;
            }
            pUser u = new pUser(iam.ID, iam);
            u.Name = txIamName.Text.Trim();
            u.Phones = txIamPhone.Text.Trim();
            u.Email = txIamEmail.Text.Trim();
            if (pUser.Save(u))
            {
                iam.Name = txIamName.Text.Trim();
                IamServices.Reload(iam, Session.SessionID);

                txIamName.ReadOnly =
                txIamEmail.ReadOnly =
                txIamPhone.ReadOnly = true;
                btnChange.Visible = true;
                btnSave.Visible = false;
            }
            else
            {
                lbMess.Text = "Ошибка при сохранении.";
            }
        }
        protected void btnChangePsw_Click(object sender, EventArgs e)
        {
            if (txIamPsw1.Text.Trim().Length < 3 || txIamPsw1.Text != txIamPsw2.Text)
            {
                lbMess.Text = "не удачная попытка сменить пароль. Новый пароль должен быть длинее 3х символов и Вы должны его повторить.";
                txIamPsw2.Text = "";
                return;

            }
            else
            {
                lbMess.Text = "";
            }
            pUser u = new pUser(iam.ID, iam);

            if (pUser.SetNewPassword(u, txIamPswOld.Text, txIamPsw1.Text, iam))
            {
                EmailMessage msg = new EmailMessage(CurrentCfg.EmailPortal, iam.Email, "santechportal.ru: смена пароля пользователя", "<p>Ваш пароль был изменен.</p><p>Если это сделали не Вы, то перешлите это письмо на email " + CurrentCfg.EmailSupport); //+", а так же Вы можете перейти по ссылке <a href='/account/login.aspx?act=crashpsw&email="+u.Email+"' >сбросить пароль</a>, в результате чего пароль будет изменен случайным образом (письмо с новым паролем будет выслано Вам следом).</p>");
                msg.Send();
                txIamPswOld.Text = "";
                txIamPsw1.Text = "";
                txIamPsw2.Text = "";
                lbMess.Text = "Пароль изменен";
            }
            else
            {
                lbMess.Text = "Неудачная попытка сменить пароль. Новый пароль должен быть длинее 3х символов и Вы должны его повторить.";
                txIamPsw2.Text = "";
                return;
            }
        }

        //public string make_where(List<string> fields, string searchString, int minLength=4 )
        //{
        //    if (fields.Count < 1 || searchString.Trim() == "") return "";
        //    string where = "";
        //    string wr = "";
        //    string w = "";
        //    if (searchString.Length > 0)
        //    {

        //        foreach (string word in searchString.Split(' '))
        //        {
        //            wr += ((wr != "") ? " and " : "");
        //            if (fields.Count == 1)
        //                wr += fields[0]+" like '%" + word + "%'";
        //            else
        //            {

        //                w = "";
        //                foreach (string fld in fields)
        //                {
        //                    w += ((w!="")?" or ":"") + fld + " like '%" + word + "%'";
        //                }
        //                wr += "("+w+")";
        //            }
        //        }
        //        if (wr != "") wr = "(" + wr + ")";

        //    }

        //    return wr;
        //}


        public void Show_orders(int pageIndex = -1)
        {
            string sql;
            string ttt_ord = webInfo.make_where(new List<string>() { "name", "descr" }, txSearch.Text.Trim());
            string ttt_good = webInfo.make_where(new List<string>() { "name", "goodcode" }, txSearch.Text.Trim());







            sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder,  ord.lcd from " + Order.TDB + " as ord" +
                  " where isnull(TypeOrd,'')='' and ord.SubjectID=" + iam.PresentedSubjectID +
                  ((dlState.SelectedValue != "") ? " and ord.State in (" + dlState.SelectedValue + ") " : "");
            if (ttt_ord != "" || ttt_good != "")
                sql += " and (" + ((ttt_ord != "") ? ttt_ord : "1=1") + " or " + ((ttt_good != "") ? "ord.id in (select orderid from ORDI where " + ttt_good + ")" : "1=1") + ")";

            sql += " order by ord.id desc";
            DataTable dt = db.GetDbTable(sql);
            if (dt.Rows.Count == 0 && ttt_good == "" && ttt_ord == "" && dlState.SelectedValue == "")
            {
                lbMsg.Text = "<div class='message center bold'><a href='../order/orderdefault.aspx'>У вас нет ни одной заявки<br>Перейти к подбору новой заявки</a></div>";
            }
            else if (dt.Rows.Count == 0)
            {
                lbMsg.Text = "<div class='message center bold'>Ни одна заявка не удовлетворяет запросу,<br/> измените условия поиска</div>";
            }
            else
            {
                lbMsg.Text = "";
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
                    e.Item.Cells[5].Text += "<a href=\"../downloadfile.ashx?id=" + id + "&act=invoice&sid="+iam.SessionID+"\"  title=' получить счет на оплату'><img width='48px' height='48px' src='../simg/icoinv.png' /></a>";
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
                e.Item.Cells[7].Text += get_buttons(id.ToString(), state);
                int olddays = cNum.cToInt(SysSetting.GetValue("DAYSOLDORDER"));
                //if ((state == "U" || state == "S") && cDate.GetDayDistance(cDate.cToDate(r["RegDate"]), cDate.TodayD) > olddays && olddays > 0)
                //{
                //    e.Item.Cells[6].Text += "&nbsp;" + setbtns(id.ToString(), "sns", "D", "time_go.png", "в архив");
                //    e.Item.CssClass += " toarch";
                //    e.Item.Cells[6].ToolTip += "кандидат в архив";
                //}
                e.Item.Cells[6].Attributes.Add("nowrap", "nowrap");
                e.Item.Cells[7].Attributes.Add("nowrap", "nowrap");
            }
        }

        private string get_infoorder(Order rec)
        {
            DataTable dt = db.GetDbTable("select (ordi.Qty-ordi.Realized) as [Нужно], owng.qty,case when ORDI.Booking>0 then ORDI.Booking else case when owng.zn='S' then case when (ordi.Qty-ordi.Realized) <= owng.qty  then (ordi.Qty-ordi.Realized)  else owng.qty end else 0 end end as [Можно] ,ord.State from ORDI inner join owng on owng.goodId=ordi.GoodID and OwnerId=100000 inner join ORD on ord.id=ordi.OrderID and ord.id=" + rec.ID + "");
            int qn = dt.Select("Нужно>0").Count();
            int qm = dt.Select("Нужно>0 and Можно>0").Count();

            return (qn > 0) ? "из " + qn + " позиций можно получить " + qm + "" : "все отгружено";


        }



        private string setbtns(string orderId, string act, string newstate, string pic, string btnword, string title = "", string css = "")
        {
            string btn = "<a href=\"#\" class='microbutton micro {css}' title='{btn}' onclick=\"javascript: if (confirm('Подтверждаете {btn}?')) {setnewstate('{orderid}','{newstate}');}else return false;\"><img src='../simg/16/{pic}' />{title}</a>";
            return btn.Replace("{orderid}", orderId).Replace("{newstate}", newstate).Replace("{pic}", pic).Replace("{btn}", btnword).Replace("{act}", act).Replace("{title}", title).Replace("{css}", css);
        }
        private string get_buttons(string orderId, string s)
        {
            string btn = "";
            string btn_del = "";//"&nbsp;" + setbtns(orderId, "sns", "D", "delete.png", "отменить/в архив");
            string btn_copy = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=copy\" class='microbutton micro' title='Скопировать/повторить ' ><img src='../simg/16/page_copy.png' /></a>";
            string btn_edit = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=edit\" class='microbutton micro' title='Изменить' ><img src='../simg/16/document-edit.png' /></a>";
            switch (s.ToUpper())
            {
                case "":
                case "N":
                    btn = btn_copy + btn_del;
                    break;
                case "U":
                    // btn = setbtns(orderId, "sns", "A", "document-accept.png", "Подтвердить к заказу", "Подтвердить к заказу", "linkbutton") + btn_edit + btn_copy + btn_del;                     
                    btn = btn_edit + btn_copy + btn_del;
                    break;
                case "A":
                case "Z":
                case "M":
                    btn = btn_copy + btn_del;
                    break;
                case "S":
                    //btn = setbtns(orderId, "sns", "M", "document-accept.png", "Согласовать к покупке", "Согласовать к покупке", "linkbutton") + btn_copy + btn_del;
                    btn = btn_copy + btn_del;
                    break;
                case "R":
                    btn = btn_copy + btn_del;
                    break;
                case "F":
                case "D":
                    btn = btn_copy;
                    break;

            }
            return btn;
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            Show_orders(e.NewPageIndex);
        }

        protected void dgList_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                e.Item.Cells[0].Controls.AddAt(0, new LiteralControl("Страницы "));
                e.Item.Cells[0].CssClass += " center";
            }
        }

        protected void btnSeach_Click(object sender, EventArgs e)
        {
            Show_orders(0);
        }

        protected void dlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            Show_orders(0);
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
            Show_orders(0);
        }
        public void Show_queries(int pageIndex = -1)
        {

            string sql;
            string ttt_ord = webInfo.make_where(new List<string>() { "name", "descr" }, txSearch.Text.Trim());
            string ttt_good = webInfo.make_where(new List<string>() { "name", "goodcode" }, txSearch.Text.Trim());







            sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder,  ord.lcd from " + Order.TDB + " as ord" +
                  " where state not in ('X','D') and isnull(TypeOrd,'')='Q' and ord.SubjectID=" + iam.PresentedSubjectID;
            if (ttt_ord != "" || ttt_good != "")
                sql += " and (" + ((ttt_ord != "") ? ttt_ord : "1=1") + " or " + ((ttt_good != "") ? "ord.id in (select orderid from ORDI where " + ttt_good + ")" : "1=1") + ")";

            sql += " order by ord.id desc";
            DataTable dt = db.GetDbTable(sql);
            if (dt.Rows.Count == 0 && ttt_good == "" && ttt_ord == "")
            {
                lbQMsg.Text = "<div class='message center bold'>У вас нет ни одной сохраненного запроса<br><a href='../order/orderdefault.aspx'>Перейти к подбору</a></div>";
            }
            else if (dt.Rows.Count == 0)
            {
                lbQMsg.Text = "<div class='message center bold'>Ни один запрос не удовлетворяет условию,<br/> измените условия поиска</div>";
            }
            else
            {
                lbQMsg.Text = "";
            }


            DataView dv = dt.DefaultView;
            dv.Sort = SortField;
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
                e.Item.Cells[3].Text += "<div class='small'>" + get_infoquery(rec) + "</div>";

                e.Item.Cells[6].Text = "";
                if (state != "D" && state != "F" && webIO.CheckExistFile("../exch/" + id + ".csv"))
                {
                    e.Item.Cells[5].Text += "<a href=\"../exch/" + id + ".csv\" target=\"_blank\" title=' получить прайс-лист'><img width='48px' height='48px' src='../simg/16/xlsx16.gif' />скачать</a>";
                }
                if (state == "X" || state == "D")
                    e.Item.Attributes.Add("class", "deleted");
                e.Item.Cells[7].Text += get_qbuttons(id.ToString(), state);
                e.Item.Cells[6].Attributes.Add("nowrap", "nowrap");
                e.Item.Cells[7].Attributes.Add("nowrap", "nowrap");
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



    }
}