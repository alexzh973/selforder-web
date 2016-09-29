using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using wstcp.Models;
using ensoCom;
using System.Text;

namespace wstcp
{
    public partial class goodlist : p_p
    {
        enum ORDERSELECT { TN_BR, BR_TN };
        static ORDERSELECT _ORDERSELECT = ORDERSELECT.TN_BR;

        const int owner_id = 100000;

        int eID
        {
            get { return cNum.cToInt(ViewState["eid"]); }
            set { ViewState["eid"] = value; }
        }
        string SELECTED_TNS
        {
            get { return "" + ViewState["SELECTED_TNS"]; }
            set { ViewState["SELECTED_TNS"]=value;}
        }
        string SELECTED_TKS
        {
            get { return "" + ViewState["SELECTED_TKS"]; }
            set { ViewState["SELECTED_TKS"]=value;}
        }
        Order CurrentOrder
        {
            get { if (Session["CO" + eID] == null) Session["CO" + eID] = new Order(0, iam); return (Order)Session["CO" + eID]; }
            set { Session["CO" + eID] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            
            TabControl1.InitTabContainer("tasklisttabs");
            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Классификатор", pnlStruct);
            TabControl1.AddTabpage("Справочник номенклатуры", pnlList);
            if (iam.ID < 100000)
            {
                lbMessage.Text = "Для зарегистрированных пользователей доступна функция формирования Заявки.<br/><a href='../account/login.aspx?pp=goodlist'>зарегаться>></a>";

                pnlOrder.Visible = false;
            }
            else
            {
                lbMessage.Text = "";                
                TabControl1.AddTabpage("Текущая заявка", pnlOrder);

               
            }

            if (!IsPostBack)
            {
                if (iam.ID <= 0) Response.Redirect("../default.aspx");
                ((mainpage)this.Master).SelectedMenu = "order";
                ((mainpage)this.Master).VisibleLeftPanel = true;
                string ps = UserFilter.Load(iam.ID, "listsize");
                try
                {
                    rbPageSize.Items.FindByValue(ps).Selected = true;
                    
                }
                catch
                {
                    ps = rbPageSize.SelectedValue;
                    UserFilter.Save(iam.ID, "listsize", ps);
                }
                finally
                {
                    dgList.PageSize = cNum.cToInt(ps);
                }
                if (iam.ID > 0 )
                {
                    show_personalTAs();
                    load_exist_orders();

                }


                Session["OWNERID"] = "" + owner_id;
                eID = cNum.cToInt(Request["id"]);
                if (eID == 0)
                {
                    load_struct();
                }
                if (eID > 0)
                {
                    CurrentOrder = new Order(eID, iam);
                    lbOrder.Text = "" + CurrentOrder.ID;
                    txNameOrder.Text = CurrentOrder.Name;
                    txDescr.Text = CurrentOrder.Descr;
                    lbRegDate.Text = CurrentOrder.RegDate.ToShortDateString();
                    lbStateOrder.Text = CurrentOrder.StateDescr;
                    TabControl1.SelectedTabIndex = 2;
                    string stack = "";
                    foreach (OrderItem item in CurrentOrder.Items)
                    {
                        cStr.AddUnique(ref stack, "" + item.GoodId);
                    }
                    itemstack.Value = stack;
                    load_curentorder();
                }


                if (!iam.IsSuperAdmin && iam.SubjectID != CurrentOrder.SubjectID && CurrentOrder.Subject.EmailTAs.IndexOf(iam.Email) < 0)
                {
                    CurrentOrder = new Order(0, iam);
                }
                if (_ACT_ == "recl" && "" + Request["code"] != "")
                {
                    txSearch.Text = "" + Request["code"];
                    TabControl1.SelectedTabIndex = 1;
                }
                if (TabControl1.IsActiveTabpage(pnlList))
                {
                    load_brends();
                    load_TK();
                    load_xName();
                    txSearch_TextChanged(null, null);
                }
            }
            SELECTED_TKS = ""+tkstack.Value;
            //if (IsPostBack && ""+Request["ch_tks"] != "")
            //    SELECTED_TKS = Request["ch_tks"];

            if (refresh.Value == "Y" && TabControl1.IsActiveTabpage(pnlOrder))
            {
                
                load_curentorder();
                refresh.Value = "";
            }
            if (TabControl1.ChangedTabpage)
            {
                if (TabControl1.IsActiveTabpage(pnlOrder) && refresh.Value != "Y")
                {
                    load_curentorder();
                }
                else if (TabControl1.IsActiveTabpage(pnlList))
                {
                    
                    load_TK();
                    load_brends();
                    load_xName();
                    txSearch_TextChanged(null, null);
                }
                else
                {
                    load_struct();
                }

            }
            if (iam != null && iam.ID > 0 && CurrentOrder != null && CurrentOrder.Summ > 0)
            {
               // lbMessage.Text += "<p>Вами подготовлен заказ на сумму " + CurrentOrder.Summ + "р.</p>";
                TabControl1.TabTitles[2] = "Заявка на сумму " + cNum.cToDecimal(CurrentOrder.Summ, 2) + "р.</p>";
                TabControl1.TabCssPrefix[2] = "bold";
            }
        }

        private void show_personalTAs()
        {
            if (iam.SubjectID==0)
            {
                rpTAs.Visible = false;
                return;
            }
            DataTable dt = db.GetDbTable("select '' as Name, '' as email, '' as phone, '' as photo where 1=0");
            Subject subj = new Subject(iam.SubjectID, iam);
            DataRow nr;
            if (subj.TAs.Count == 0)
            {
                nr = dt.NewRow();
                nr["Name"] = "не назначен ни один менеджер";
                nr["email"] = "info@santur.ru";
                nr["phone"] = "(343)270-04-04";
                dt.Rows.Add(nr);
            }
            else
            {
                foreach (pUser p in subj.TAs)
                {
                    nr = dt.NewRow();
                    nr["Name"] = p.Name;
                    nr["email"] = p.Email;
                    nr["phone"] = p.Phones;
                    nr["photo"] = "../simg/phu/" + p.ID + "b.png?t=" + DateTime.Now.Ticks;
                    dt.Rows.Add(nr);
                }
            }
            rpTAs.DataSource = dt;
            rpTAs.DataBind();
        }

        private void load_struct()
        {
            int i = 0;//GoodInfo.DT_StructTN(owner_id);
            DataTable dt_tn = db.GetDbTable("select xTN, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") group by xTN order by xTN");
            DataTable dt_tks = db.GetDbTable("select xTN,xTK, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") group by xTN,xTK order by xTK");
            
            
            StringBuilder sb = new StringBuilder();
            foreach (DataRow r_tn in dt_tn.Select("xTN<>''"))
            {
                sb.Append("<div class='blocktn' >");
                sb.Append("<strong>"+r_tn["xTN"].ToString().ToUpper()+"</strong><br/>");
//                sb.Append("<h3><input type='checkbox' value='" + r_tn["xTN"] + "' " + ((SELECTED_TNS.IndexOf("" + r_tn["xTN"]) > -1) ? "checked" : "") + " name='ch_tns' />" + r_tn["xTN"] + "</h3>");
                sb.Append("<div class='blocktk'><div class='floatLeft-90'><ul>");
                foreach (DataRow r_tk in dt_tks.Select("xTN='" + r_tn["xTN"] + "'"))  //db.GetDbTable("select xTK, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") and xTN='" + r_tn["xTN"] + "' group by xTK order by xTK").Rows)
                {
                    string tk = ""+r_tk["xTK"];
                    tk = tk.Replace(",", "@");
                    bool ch = (SELECTED_TKS.ToLower().IndexOf("" + tk.ToLower()) > -1);
                    sb.Append(" <li><input type='checkbox' value='" + tk.ToLower() + "' " + ((ch) ? "checked" : "") + " name='ch_tks' id='ch_tks_" + i + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" /><label for='ch_tks_" + i + "' " + ((ch) ? "class='check'" : "class='uncheck'") + " >" + r_tk["xTK"] + "</label></li>");
                    i += 1;
                }
                sb.Append("</ul></div><div class='floatRight-10'><img class='arrowtolist' src='../simg/32/arrow16x32.png' style='cursor:pointer' title='перейти в справочник номенклатуры' alt='>>' onclick=\"setActPunct(3, 1,'ctl00_ContentPlaceHolder1_TabControl1')\"/></div><div class='clearBoth'></div></div>");// block_TK
                sb.Append("</div>");// block_TN
            }
            sb.Append("<div style='clear:both;'></div>");
            struct_place.Text = sb.ToString();
        }

        private void load_exist_orders()
        {
            string sql;
            if (iam.SubjectID > 0)
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where ord.state<>'D' and ord.SubjectID=" + iam.SubjectID + "";
            else
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where subj.EmailTAs like '%" + iam.Email + "%' and ord.state<>'D'";

            DataTable dt = db.GetDbTable(sql);
            DataTable dto = db.GetDbTable("select 0 as id, '' as Name, '' as RegDate, '' as State, '' as SubjectName, '' as SummOrder, '' as linkchange");
            dto.Rows.Clear();
            foreach (DataRow r in dt.Rows)
            {
                DataRow nr = dto.NewRow();
                nr["id"] = r["id"];
                nr["Name"] = r["Name"];
                
                nr["RegDate"] = cDate.cToDate(r["RegDate"]).ToShortDateString();
                nr["SubjectName"] = r["SubjectName"];
                nr["State"] = Order.get_stateorder_descr(""+r["State"]);
                nr["SummOrder"] = cNum.cToDecimal(r["SummOrder"], 2);
                //if (iam.SubjectID > 0)
                    nr["linkchange"] = "<a href='goodlist.aspx?id="+r["id"]+"&act=edit'>изменить</a>";
                dto.Rows.Add(nr);
            }
            rpOrders.DataSource = dto;
            rpOrders.DataBind();
        }

        private void load_curentorder()
        {

            string where_conditions = (itemstack.Value.Length >= 6) ? " id in (" + itemstack.Value + ")" : " id in (0)";
            DataTable dt = GoodInfo.Get_GoodesByOwner(owner_id, where_conditions, "");
            txINN.Text = CurrentOrder.Subject.INN;
            txNameSubject.Text = CurrentOrder.Subject.Name;
            txCodeSubject.Text = CurrentOrder.Subject.Code;
            lbCode.Text = CurrentOrder.Code;
            lbtnDelete.Visible = (CurrentOrder.SubjectID == iam.SubjectID);
            
            txINN.ReadOnly = txNameSubject.ReadOnly = txCodeSubject.ReadOnly = !(iam.IsSuperAdmin);
            
            foreach (DataRow r in dt.Rows)
            {
                OrderItem item = CurrentOrder.Items.Find(i => i.GoodId == cNum.cToInt(r["ID"]));
                decimal pr = (item == null) ? cNum.cToDecimal(r["pr_b"]) : item.Price;
                decimal q = (Request["q_" + r["ID"]] != null) ? cNum.cToDecimal(Request["q_" + r["ID"]]) : ((item == null) ? 1 : item.Qty);
                string descr = (Request["ds_" + r["ID"]] != null) ? "" + Request["ds_" + r["ID"]] : ((item == null) ? "" : item.Descr);

                if (q <= 0) q = 1;
                decimal s = q * pr;
                if (item == null)
                {
                    item = new OrderItem() { GoodId = cNum.cToInt(r["ID"]), Zn = "" + r["zn"], Zn_z = "" + r["zn_z"], CurIncash = cNum.cToDecimal(r["qty"]), GoodCode = "" + r["GoodCode"], Name = "" + r["Name"], Qty = q, Price = pr, Summ = s, state = "N", Descr = descr, article = "" + r["article"], ed = "" + r["ed"] };
                    CurrentOrder.Items.Add(item);
                }
                else
                {
                    item.Qty = q;
                    item.Summ = s;
                    item.Descr = descr;
                }
            }


            DataView dv = CurrentOrder.ItemsDt.DefaultView;
            dv.Sort = "name";
            dgOrder.DataSource = dv;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();
        }


        //protected void load_TN_new(string default_value = "")
        //{
        //    return;



        //    string where = "";
        //    if (_ORDERSELECT == ORDERSELECT.TN_BR)
        //        where = "";
        //    else if (_ORDERSELECT == ORDERSELECT.BR_TN)
        //        where = ((dlBrends.SelectedValue != "") ? "Brend='" + dlBrends.SelectedValue + "'" : "");
        //    DataTable dt = db.GetDbTable("select xTN, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") " + ((where != "") ? " and " + where : "") + " group by xTN order by xTN");
        //    DataList1.DataSource = dt;
        //    DataList1.DataBind();
        //}

        
        protected void load_brends(string default_value = "")
        {
            
            string where = "";

            //if (_ORDERSELECT == ORDERSELECT.TN_BR)
            //    where = ((tn != "") ? " and xTN='" + tn + "'" : "");
            //else if (_ORDERSELECT == ORDERSELECT.BR_TN)
            //    where = "";

            if (dlTK.SelectedValue != "")
                where += " and xTK='" + dlTK.SelectedValue + "'";

            string seltk = get_selected_tks();
            

            if (seltk != "")
                where = " and xTK in (" + seltk + ")";
            DataTable dt = db.GetDbTable("select brend,count(id) as qty from good where id in (select goodid from OWNG where OwnerID=" + owner_id + ") " + ((where != "") ? where : "") + " group by brend ");
            dlBrends.Items.Clear();
            dlBrends.Items.Add(new ListItem("Все", ""));
            foreach (DataRow r in dt.Select("Brend<>''", "Brend"))
            {
                dlBrends.Items.Add(new ListItem("" + r["Brend"] + " (" + r["qty"] + ")", "" + r["Brend"]));
            }
            try
            {
                dlBrends.Items.FindByValue(default_value).Selected = true;
            }
            catch
            {
            }
        }

        private string get_selected_tks()
        {
            string seltk = "";
            foreach (string s in SELECTED_TKS.Split(','))
            {
                if (s!="")
                    cStr.Add(ref seltk, "'" + s.Replace('@',',') + "'");
            }
            return seltk;
        }

        protected void load_TK(string default_value = "")
        {
            //string tn = SEL_TN;
            string brend = dlBrends.SelectedValue;
            dlTK.Items.Clear();
            dlTK.ClearSelection();
            dlTK.Items.Add(new ListItem("", ""));
            string seltk = get_selected_tks();

            DataTable dt = db.GetDbTable("select xTK, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") " + ((seltk != "") ? " and xTK in ("+seltk+")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");



            foreach (DataRow r in dt.Select("xTK<>''", "xTK"))
            {
                dlTK.Items.Add(new ListItem("" + r["xTK"] + " (" + r["qty"] + ")", "" + r["xTK"]));
            }
            try
            {
                dlTK.Items.FindByValue(default_value).Selected = true;
            }
            catch
            {
            }
        }

        protected void load_xName(string default_value = "")
        {
            //string tn = SEL_TN;
            string tk = dlTK.SelectedValue;
            string brend = dlBrends.SelectedValue;
            dlNames.Items.Clear();
            dlNames.Items.Add(new ListItem("", ""));
            dlNames.ClearSelection();
            string seltks = get_selected_tks();
            
            
            DataTable dt = db.GetDbTable("select xName, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((tk != "") ? " and xTK='" + tk + "'" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");



            foreach (DataRow r in dt.Select("", "xName"))
            {
                dlNames.Items.Add(new ListItem("" + r["xName"] + " (" + r["qty"] + ")", "" + r["xName"]));
            }
            try
            {
                dlNames.Items.FindByValue(default_value).Selected = true;
            }
            catch
            {
            }
        }



        protected void txSearch_TextChanged(object sender, EventArgs e)
        {
            //string tn = SEL_TN;
            string where_conditions = "1=1";

            where_conditions = get_where_condition();
            string RowFilter = "";

           // if (dlBrends.SelectedValue == "" && SELECTED_TKS == "" && txSearch.Text.Trim() == "")
           //     where_conditions = "2=2";
            if (where_conditions=="1=1")
                where_conditions = "2=2";
            DataTable dt = GoodInfo.Get_GoodesByOwner(owner_id, where_conditions, RowFilter);
            Cache.Insert("MyDT", dt, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
            DataView dv = dt.DefaultView;

            dv.Sort = "Name";
            dgList.DataSource = dv;
            dgList.CurrentPageIndex = 0;
            lbSelTN.Text = SELECTED_TKS;
            lbSelTN.Visible = true;

            try
            {
                dgList.DataBind();
            }
            catch { }


        }
        private string get_where_condition()
        {
            string where_conditions = "1=1";
            //string tn = SEL_TN;
            if (dlBrends.SelectedValue != "")
                where_conditions += " and brend like '" + dlBrends.SelectedValue + "%'";
            string seltks = get_selected_tks();
            //if (tn != "")
            //    where_conditions += " and xTN like '" + tn + "%'";
            if (seltks !="")
                where_conditions += " and xTK in (" + seltks + ")";
            if (dlTK.SelectedValue != "")
                where_conditions += " and xTK like '" + dlTK.SelectedValue + "%'";
            if (dlNames.SelectedValue != "")
                where_conditions += " and xName like '" + dlNames.SelectedValue + "%'";

            if (txSearch.Text.Length >= 4)
            {
                where_conditions += string.Format(" and (name like '%{0}%' or descr like '%{0}%' or goodcode like '%{0}%' )", txSearch.Text.Trim().Replace(":", @"/"));
            }
            else if (txSearch.Text.Length > 0)
            {
                where_conditions += string.Format(" and (name like '{0}%' or name like '% {0}%'  or goodcode like '%{0}%')", txSearch.Text.Trim().Replace(":", @"/"));
            }

            return where_conditions;
        }

 
        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {

            if (Cache["MyDT"] != null)
            {
                DataTable dt = (DataTable)Cache["MyDT"];
                DataView dv = dt.DefaultView;
                dv.Sort = "Name";
                dgList.DataSource = dv;
                dgList.DataBind();
            }
            else
            {
                txSearch_TextChanged(null, null);
            }
            


            //txSearch_TextChanged(null, null);
            if (((DataView)dgList.DataSource).Count / dgList.PageSize >= e.NewPageIndex)
            {
                dgList.CurrentPageIndex = e.NewPageIndex;
                dgList.DataBind();
            }
        }

        protected void dlBrends_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_TK(dlTK.SelectedValue);
            load_xName();
            txSearch_TextChanged(sender, e);
        }
        protected void dlTK_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_brends(dlBrends.SelectedValue);
            load_xName();
            txSearch_TextChanged(null, e);
        }
    



        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                bool exist = (itemstack.Value.IndexOf("" + r["ID"]) >= 0);
                e.Item.Attributes.Add("id", "tr_" + r["ID"]);
                if (exist) e.Item.CssClass = "selitem";
                if (iam.ID > 0)
                    e.Item.Cells[1].Text = "<input type='checkbox' onclick=\"itemInStack(this.id,'" + r["ID"] + "');shw(this.id,'" + r["ID"] + "')\" id='ch_" + r["ID"] + "' " + ((exist) ? " value='" + r["ID"] + "' checked='checked'" : "") + ")/>";
                e.Item.Cells[2].Text = (txSearch.Text.Length>0)?webInfo.select_search(txSearch.Text, "" + r["Goodcode"]):"" + r["Goodcode"];
                e.Item.Cells[3].Text = (txSearch.Text.Length>0)?webInfo.select_search(txSearch.Text, "" + r["Name"]):"" + r["Name"];
                if (""+r["ens"]!="")
                    e.Item.Cells[6].Text = "<img style='cursor:pointer;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r["ens"] + "/?key=x9BS1EFXj0', 800,700,'Описание товара')\" src='../simg/16/doc_view.png'/ alt='o' title='посмотреть детали'>";
                
                if (iam.ID > 0)
                {
                    decimal q = cNum.cToDecimal(r["qty"]);
                    if ( iam.SubjectID > 0)
                    {
                        
                        e.Item.Cells[4].Text = stategood(q, "" + r["zn"], "" + r["zn_z"]);
                    }
                    else 
                    {
                        e.Item.Cells[4].Text = ""+q+ " ("+stategood(q, "" + r["zn"], "" + r["zn_z"])+")";
                    }
                    e.Item.Cells[4].Attributes.Add("ondblclick","openflywin('../common/gimail.aspx?id="+r["ID"]+"', 400, 300, 'Запрос информации о товаре')");
                    decimal c = ((""+r["zn"]).ToLower()=="z")?0:cNum.cToDecimal(r["pr_b"],2);
//                    decimal c = (("" + r["zn"]).ToLower() == "z" && cNum.cToInt(r["zt"]) > 1) ? 0 : cNum.cToDecimal(r["pr_b"], 2);
                    e.Item.Cells[5].Text = (c > 0) ? "" + cNum.cToDecimal(c, 2) + "р." : "<img src='../simg/32/ph.png' title='по запросу' alt='?' "+get_emailmess(""+r["ID"])+"/>";
                }
            }
        }

        string get_emailmess(string good_id)
        {
            string r = "ondblclick=\"openflywin('../common/gimail.aspx?id="+good_id+"', 400, 300, 'Запрос информации о товаре')\"";
            return r;
        }
        string stategood(decimal qty, string zn, string zn_ost, decimal qty_need=0)
        {
            string ret = "";
            if (zn_ost == "NL" || zn_ost == "P2" || zn_ost == "PZ")
            {
                ret = "<span class='goodincash-full'>в наличии</span>";
            }
            else
            {
                if (zn.ToUpper() == "Z")
                {
                    ret = "<span class='goodbyorder'>под заказ</span>";
                }
                else
                {
                    ret = (qty - qty_need > 0) ? ((qty < 5) ? "<span class='goodincash-little'>есть, но мало</span>" : "<span class='goodincash-full'>в наличии</span>") : "<img src='../simg/32/ph.png' title='по запросу' alt='?'/>";
                }
            }
            return ret;
        }
        protected void dgOrder_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                bool exist = (itemstack.Value.IndexOf("" + r["GoodID"]) >= 0);
                e.Item.Attributes.Add("id", "tr_" + r["GoodID"]);
                e.Item.Cells[4].Attributes.Add("id", "pr_" + r["GoodID"]);
                e.Item.Cells[4].Text = ""+cNum.cToDecimal(r["pr"], 2);
                e.Item.Cells[5].Text = "<input class='center qtyfield' maxlength='5' name='q_" + r["GoodID"] + "' id='q_" + r["GoodID"] + "' type='text' onchange=\"checkNumeric(this.id,1);recount('" + r["GoodID"] + "');\" value='" + cNum.cToDecimal(r["qty"],1) + "' />";
                decimal q = cNum.cToDecimal(r["Curincash"]);
                decimal q_ord = cNum.cToDecimal(r["qty"]);

                
                e.Item.Cells[7].Attributes.Add("id", "sm_" + r["GoodID"]);
                e.Item.Cells[7].Text = "" + cNum.cToDecimal(r["sum"], 2);

                e.Item.Cells[8].Text = "<input maxlength='150' width='150px' name='ds_" + r["GoodID"] + "' id='ds_" + r["GoodID"] + "' type='text' class='descritem' value='" + r["descr"] + "' />";
                e.Item.Cells[9].Text += "<div>"+stategood(q, "" + r["zn"], "" + r["zn_z"], q_ord)+"</div>";
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                e.Item.Cells[4].Text = "ИТОГО";
                e.Item.Cells[7].Text = "" + cNum.cToDecimal(CurrentOrder.Summ, 2);
                e.Item.Cells[8].Text = "руб.";
            }
        }

        

        protected void dgOrder_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = cNum.cToInt(e.Item.Cells[0].Text);
            CurrentOrder.Items.Remove(CurrentOrder.Items.Find(i => i.GoodId == id));
            string s = itemstack.Value;
            cStr.SubstringDelete(ref s, id.ToString());
            itemstack.Value = s;
            load_curentorder();
        }

        
        //string SEL_TN { get { return "" + ViewState["seltn"]; } set { ViewState["seltn"] = value; } }
        
        
        protected void DataList1_SelectedIndexChanged(object source, DataListCommandEventArgs e)
        {
            TabControl1.SelectedTabIndex = 0;
            //SEL_TN = "" + e.CommandArgument;
            DataList1.SelectedIndex = e.Item.ItemIndex;
            lbSelTN.Text = "" + e.CommandArgument;
            //load_TN_new(SEL_TN);
            load_TK();
            load_xName();
            load_brends();
            txSearch_TextChanged(null, e);
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            load_curentorder();
            CurrentOrder.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Заявка от " + CurrentOrder.RegDate.ToShortDateString() + " " + CurrentOrder.RegDate.ToShortTimeString();
            CurrentOrder.Name = txNameOrder.Text;
            bool result = Order.Save(CurrentOrder, iam);
            lbOrder.Text = "" + CurrentOrder.ID;
            lbRegDate.Text = CurrentOrder.RegDate.ToShortDateString();
            if (result)
            {
                try
                {
                    EmailMessage msg = new EmailMessage(CurrentCfg.EmailPortal, CurrentOrder.Subject.EmailTAs, (CurrentOrder.Code == "") ? "Создан новый заказ " : "Скорректирован заказ ", "Клиент " + CurrentOrder.Subject.Name + " оформил Заказ на сумму " + CurrentOrder.Summ);
                    msg.Send(CurrentCfg.EmailSupport);
                }
                catch { }
                int id = CurrentOrder.ID;
                CurrentOrder = null;
                eID = id;
                CurrentOrder = new Order(eID, iam);
                lbMessage.Text = "<p>Заказ успешно сохранен.</p><p>Минут через 5, заявка будет автоматически пересчитана с учетом персональных условий по Вашему договору.</p><p>Ваш персональный менеджер свяжется с Вами в ближайшее время. </p>";
                load_exist_orders();
            }
        }
        
        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            eID = 0;
            CurrentOrder = new Order(0, iam);
            itemstack.Value = "";
            txDescr.Text = "";
            txNameOrder.Text = "";
            lbOrder.Text = "";
            lbRegDate.Text = "";            
            TabControl1.SelectedTabIndex = 0;
            TabControl1.TabTitles[1] = "Текущая заявка";
            txSearch_TextChanged(sender, e);
        }

        protected void btnDelorder_Click(object sender, EventArgs e)
        {
            bool res = Order.Delete(CurrentOrder,iam);
            if (res)
            {
                
                lbMess.Text = "Заявка №" + CurrentOrder.ID + " удалена";
                CurrentOrder = new Order(0,iam);
                load_exist_orders();
            }
        }

        protected void btnHideExistsOrders_Click(object sender, EventArgs e)
        {
            if (btnHideExistsOrders.CommandArgument == "show")
            {
                ((mainpage)this.Master).VisibleLeftPanel = true;
                btnHideExistsOrders.Text = "<< скрыть свои заявки";
                btnHideExistsOrders.CommandArgument = "hide";
            }
            else
            {
                ((mainpage)this.Master).VisibleLeftPanel = false;
                btnHideExistsOrders.Text = "<< показать свои заявки";
                btnHideExistsOrders.CommandArgument = "show";
            }
            
        }

        protected void rbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "listsize", rbPageSize.SelectedValue);
            dgList.PageSize = cNum.cToInt(rbPageSize.SelectedValue);
            txSearch_TextChanged(null, e);
        }




    }
}