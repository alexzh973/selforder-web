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
    public partial class _default : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lbMess.Text = "";
            if (!IsPostBack)
            {
               
                
                ((mainpage)Master).VisibleLeftPanel = false;
                ((mainpage)Master).VisibleRightPanel = false;

                show_news();
                load_list_accs();
                lbMessageLogin.Text = "";

                if (iam.ID > 0)
                {
                    MultiView1.SetActiveView(vOrders);
                    if (iam.IsAdmin || iam.IsSuperAdmin || iam.IsSaller || iam.IsTA)
                    {
                        load_select_subjects();
                    }
                    else
                    {
                        pnlSubjectFilter.Visible = false;
                    }


                    if (iam.PresentedSubjectID > 100000)
                    {
                        if (iam.CurOrder != null && (iam.CurOrder.ID == 0 && iam.CurOrder.Items.Count > 0 || iam.CurOrder.Changed))
                        {
                            litInfo.Text = "<div class='message center bold'>У вас есть несохраненная Заявка <a href='../order/cart.aspx'>перейти к заявке</a></div>";
                        }
                        else
                        {
                            litInfo.Text = "<div class='message center bold'><a href='../order/orderdefault.aspx'>Перейти к подбору новой заявки</a></div>";
                        }
                        show_nearestTeo();


                        if (_ACT_ == "avl")
                        {
                            show_avaliblegoodies();
                        }
                        else
                        {
                            show_orders("");
                        }
                    }
                }
                else
                {
                    MultiView1.SetActiveView(vLogin);
                    txLogin.Attributes.Add("onchange", "showSmsCode()");
                }
            }
            else if (hdRefresh.Value == "Y" || !IsPostBack)
            {
                show_orders("");
                hdRefresh.Value = "";

            }
        }
        private void load_list_accs()
        {
            string date = cDate.Date2Sql(cDate.TodayD);
            DataTable dt = db.GetDbTable("select * from ACC where startdate<=" + date + " and finishdate>=" + date + ((iam.IsSuperAdmin) ? "" : " and ownerid=" + iam.OwnerID));
            rpAccs.DataSource = dt;
            rpAccs.DataBind();
        }
        protected void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (e.Position)
            {
                case 1:
                    show_orders("'U','S'"); // Ожидают решения клиента
                    break;
                case 2:
                    show_orders("'M','E','R'"); // на комплектации, готовые к выдаче
                    break;
                case 3:

                    break;
                case 4:
                    show_orders("'X','D'"); // отмененные
                    show_orders("'F'"); // выполненные
                    break;
                case 0:
                default:
                    show_orders("'','N','Z','A'"); // В процессе обработки/согласовании в Сантехе
                    break;
            }
            //show_orders();
            //if (e.Position == 2)
            //{
            //    show_current_acc();
            //}
            //else if (e.Position == 1)
            //{
            //    load_TK(UserFilter.Load(iam.ID, "TK"));
            //    load_brends(UserFilter.Load(iam.ID, "brend"));
            //    load_xName(UserFilter.Load(iam.ID, "Name"));
            //    txSearch_TextChanged(null, null);
            //}
            //else
            //{
            //    load_struct();
            //}
        }



        private void load_select_subjects()
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

        private void show_nearestTeo()
        {
            DataTable dt = db.GetDbTable("select * from ord where isnull(Teotrans,'')<>'' and State not in ('F','D') and TEOState in ('P','A') and subjectId=" + iam.PresentedSubjectID + " and TeoDate>=" + cDate.Date2Sql(cDate.TodayD.AddDays(-3)));
            rpTeo.DataSource = dt;
            rpTeo.DataBind();

            rpTeo.Visible = (dt.Rows.Count > 0);

        }



        private void show_avaliblegoodies()
        {
            MultiView1.SetActiveView(vOrders);
            //return;
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
        //private void show_nl()
        //{
        //    DataTable dt = db.GetDbTable("select top 15 goodid, good.name, img, convert(numeric(15,0),owng.pr_b) as price, ens, owng.goodCode from OWNG inner join good on owng.goodId=good.id  where ownerid=100000 and zn_z='NL' and qty>0 and owng.pr_b<100000 order by qty*owng.pr_b desc");
        //    foreach (DataRow r in dt.Rows)
        //    {

        //        string img = "../media/gimg/" + r["img"];

        //        r["img"] = (webIO.CheckExistFile(img)) ? img : "../media/nophoto.gif";
        //    }
        //    rpOffer.DataSource = dt;
        //    rpOffer.DataBind();
        //}

        void show_orders(string states = "")
        {
            if ("" + Session["taborders"] == "")
                Session["taborders"] = "U";

            string selectTab = "" + Session["taborders"];

            settabs(selectTab);


            MultiView1.SetActiveView(vOrders);
            int subjId = iam.PresentedSubjectID;
            int q = 0;

            q = get_qty_orders(txSearch.Text, "'','N','Z','A'");
            qZ.Visible = (q > 0);
            qZ.Text = q.ToString();

            q = get_qty_orders(txSearch.Text, "'U','S'");
            qU.Visible = (q > 0);
            qU.Text = q.ToString();

            q = get_qty_orders(txSearch.Text, "'M','E','R'");
            qR.Visible = (q > 0);
            qR.Text = q.ToString();

            switch (selectTab)
            {
                case "Z":
                    mvOrders.SetActiveView(vZ);
                    q = ucOrdersTable_Z.Show_orders(subjId, "АZ", "'A','Z','','N'", false, true, txSearch.Text);

                    break;
                case "U":
                    mvOrders.SetActiveView(vU);
                    q = ucOrdersTable_U.Show_orders(subjId, "S", "'S','U'", false, true, txSearch.Text);
                    break;
                case "R":
                    mvOrders.SetActiveView(vR);
                    q = ucOrdersTable_R.Show_orders(subjId, "R", "'E','R','M'", false, true, txSearch.Text);
                    break;
                case "D":
                    mvOrders.SetActiveView(vD);

                    break;
                case "F":
                    mvOrders.SetActiveView(vF);

                    ucOrdersTable_D.Visible = true;
                    q = ucOrdersTable_D.Show_orders(subjId, "D", "'D','X'", false, true, txSearch.Text);
                    ucOrdersTable_F.Visible = true;
                    q = ucOrdersTable_F.Show_orders(subjId, "F", "'F'", false, true, txSearch.Text);
                    break;
                case "Price":
                    mvOrders.SetActiveView(vPrices);

                    break;
                default:

                    break;
            }

        }

        DataTable _dtcountords;

        protected int get_qty_orders(string where, string listStates)
        {
            if (_dtcountords == null)
            {
                string flt = (where.Length > 4) ? "%" + where.Replace("  ", " ").Replace(" ", "%") + "%" : where.Replace("  ", " ").Replace(" ", "%") + "%";
                string w = (flt.Length > 16) ? " and (ord.id in (select orderid from ORDI where goodCode like '" + where + "' or Name like '" + flt + "' " + ((where.Length < 4) ? " or Name like '% " + where + "%'" : "") + " ) or (''+ord.id) like '%" + where + "%' or ord.name like '%" + where + "%' or ord.Code like '%" + where + "%')" : "";
                _dtcountords = db.GetDbTable("select state, count(id) as qty from ORD where isnull(TypeOrd,'')<>'Q' and subjectID=" + iam.PresentedSubjectID + ((w.Length > 1) ? w : "") + " group by state");

            }
            int q = cNum.cToInt(_dtcountords.Compute("sum(qty)", "state in (" + listStates + ")"));
            return q;
        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            IamServices.ClearSession(IamServices.GetIam(Session.SessionID), Session.SessionID);

            IAM _iam = IamServices.Login(txLogin.Text, txPwd.Text, Session.SessionID, Request.ServerVariables["REMOTE_ADDR"]);
            if (_iam != null && _iam.ID > 0)
            {
                HttpCookie mc = new HttpCookie("usrcook", txLogin.Text + "#" + txPwd.Text);
                mc.Expires = DateTime.Now.AddDays(33);
                Response.Cookies.Add(mc);
                lbMessageLogin.Text = "";
                txPwd.Text = "";

                //show_orders();
                Response.Redirect("default.aspx");
            }
            else
            {
                lbMessageLogin.Text = "авторизация не прошла.";
                txPwd.Text = "";

            }
        }

        protected void linkRememberPsw_Click(object sender, EventArgs e)
        {
            if (txLogin.Text.IndexOf("@") == -1)
                return;
            DataRow rr = db.GetDbRow("select convert(nvarchar,pass) as pass from " + pUser.TDB + " where email='" + txLogin.Text + "'");
            if (rr != null && rr["pass"] != null)
            {
                EmailMessage email = new EmailMessage(CurrentCfg.EmailSupport, txLogin.Text, "Портал: информация о забытом пароле", "Ваш пароль: " + rr["pass"]);
                email.Send();
            }
            lbMessageLogin.Text = "забытый пароль выслан на почту " + txLogin.Text;

        }

        protected void dlSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {

            iam.PresentedSubjectID = (iam.SubjectID == 0) ? cNum.cToInt(dlSubjects.SelectedValue) : iam.SubjectID;
            if (iam.CurOrder != null && iam.CurOrder.SubjectID != iam.PresentedSubjectID)
                iam.CurOrder = null;
            ((mainpage)Master).ReloadUserString();
            ((mainpage)Master).show_personalTAs(new Subject(iam.PresentedSubjectID, iam));

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

        protected void lbtnRemember_PreRender(object sender, EventArgs e)
        {
            lbtnRemember.Attributes.Add("onclick", "checkemail('" + txLogin.ClientID + "','" + lbMessageLogin.ClientID + "')");
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


        protected void lbtnShowZ_Click(object sender, EventArgs e)
        {
            settabs("Z");
            show_orders();
        }

        protected void lbtnShowU_Click(object sender, EventArgs e)
        {
            settabs("U");
            show_orders();
        }

        protected void lbtnShowR_Click(object sender, EventArgs e)
        {
            settabs("R");
            show_orders();
        }


        protected void lbtnShowD_Click(object sender, EventArgs e)
        {
            settabs("D");
            show_orders();
        }

        protected void lbtnShowF_Click(object sender, EventArgs e)
        {

            settabs("F");
            show_orders();
        }


        protected void lbtnShowPrice_Click(object sender, EventArgs e)
        {

            settabs("price");
            Show_queries();
        }
        private void settabs(string reg)
        {
            tab0.Attributes.Remove("class");
            tab1.Attributes.Remove("class");
            tab2.Attributes.Remove("class");
            //tab3.Attributes.Remove("class");
            tab4.Attributes.Remove("class");
            tab5.Attributes.Remove("class");

            switch (reg)
            {
                case "Z":
                    Session["taborders"] = "Z";
                    tab0.Attributes.Add("class", "active");
                    break;
                case "U":
                    Session["taborders"] = "U";
                    tab1.Attributes.Add("class", "active");
                    break;


                case "R":
                    Session["taborders"] = "R";
                    tab2.Attributes.Add("class", "active");
                    break;
                case "D":
                //Session["taborders"] = "D";
                //tab3.Attributes.Add("class", "active");
                //break;
                case "F":
                    Session["taborders"] = "F";
                    tab4.Attributes.Add("class", "active");
                    break;
                case "price":
                    Session["taborders"] = "price";
                    tab5.Attributes.Add("class", "active");
                    break;

                default:
                    break;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            show_orders();
        }

        protected void lbtnCloseWin_Click(object sender, EventArgs e)
        {
            Session["IseeArch"] = "1";
            pnl2arch.Visible = false;
        }



        public void Show_queries(int pageIndex = -1)
        {

            MultiView1.SetActiveView(vOrders);
            mvOrders.SetActiveView(vPrices);
            if ("" + Session["taborders"] == "")
                Session["taborders"] = "price";

            string selectTab = "" + Session["taborders"];

            settabs(selectTab);


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

            string btnDel = "&nbsp;" + setbtns(orderId, "sns", "X", "delete.png", "отменить/в архив");
            string btnCopy = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=copy\" class='microbutton micro' title='Скопировать/повторить ' ><img src='../simg/16/page_copy.png' /></a>";
            string btnEdit = "<a href=\"../order/orderdefault.aspx?id=" + orderId + "&act=edit\" class='microbutton micro' title='Изменить' ><img src='../simg/16/document-edit.png' /></a>";
            string btn = btnEdit + btnCopy + btnDel;

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

        protected void btnToOldStyle_Click(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "defaultpage", "old");
            Response.Redirect("defaultold.aspx");
        }

        protected void lbtnRemember_Click(object sender, EventArgs e)
        {
            if (!cStr.ValidEmail(txLogin.Text))
            {
                lbMessageLogin.Text = "Вы не указали свой e-mail (либо указали не корректно), а он необходим, чтобы на него был выслан забытый пароль";
            }
            else
            {
                DataRow rr = db.GetDbRow("select convert(nvarchar,pass) as pass from " + pUser.TDB + " where email='" + txLogin.Text.Trim() + "'");
                if (rr != null && rr["pass"] != null)
                {
                    EmailMessage email = new EmailMessage(CurrentCfg.EmailSupport, txLogin.Text.Trim(), "Портал: информация о забытом пароле", "Ваш пароль: " + rr["pass"]);
                    email.Send();
                    lbMessageLogin.Text = "Забытый пароль выслан на почту " + txLogin.Text.Trim();
                }
                else
                {
                    lbMessageLogin.Text = "E-mail " + txLogin.Text.Trim() + " не найден среди пользователей сайта. Либо указан не корректно, либо Вы можете пройти короткую процедуру регистрации.";
                }

            }
        }






    }
}