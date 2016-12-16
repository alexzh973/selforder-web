using ensoCom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;

namespace wstcp.order
{
    public partial class ucOrdersTable : uc_base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
            string script = "function showlist(idlist){if($('#'+idlist).css('display')=='none'){$('#'+idlist).show();}else{$('#'+idlist).hide();}} ";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showlist", script, true);
        }



        public string TitleList
        {
            set { lbTitleList.Text = value; h3.Visible = (lbTitleList.Text != ""); }
            get { return lbTitleList.Text; }
        }

        private int SubjectID
        {
            set { ViewState["SubjectId"] = "" + value; }
            get { return cNum.cToInt(ViewState["SubjectId"]); }
        }
        private string stateIN
        {
            set { ViewState["stateIN"] = value; }
            get { return "" + ViewState["stateIN"]; }
        }
        private string filterGood
        {
            set { ViewState["filterGood"] = value; }
            get { return "" + ViewState["filterGood"]; }
        }
        public int Show_orders(int SubjectId, string titleList, string state_in = "'','N','U'", bool showTitle_when_empty = false, bool expanded = false, string filterbygood = "", int pageIndex = -1)
        {
            filterGood = filterbygood;
            SubjectID = SubjectId;
            stateIN = state_in;
            if (SubjectId <= 0)
            {
                h3.Visible = false;
                dgList.Visible = false;
                return 0;
            }
            string sql;
            string flt = (filterbygood.Length > 4) ? "%" + filterbygood.Replace("  ", " ").Replace(" ", "%") + "%" : filterbygood.Replace("  ", " ").Replace(" ", "%") + "%";
            //             sql = "select ord.id, ord.Name, ord.RegDate, ord.Descr, ord.State, ord.SummOrder, subj.Name as SubjectName, ord.lcd from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where ord.SubjectID=" + SubjectId + " and ord.State in (" + state_in + ")"+((filterbygood.Length>0)?" and ord.id in (select orderid from ORDI where goodCode like '"+flt+"' or Name like '"+flt+"' "+((filterbygood.Length<4)?" or Name like '% "+filterbygood+"%'":"")+" )":"");
            sql = "select " +
                  "(select count(*) from viewarticle as t1 where t1.eid=ord.id and t1.metatdb='ORD' and userid="+iam.ID+" ) as iseen," +
                  "case when ord.summbase<>0 and ord.summbase>ord.summorder then (ord.summbase-ord.summorder)/ord.summbase else 0 end*100 as discount," +
                  "(select count(t1.goodid) from ordi as t1 where (t1.qty-t1.realized)>0 and t1.orderid=ord.id) as qtyitemneed," +
                  "(select count(t1.goodid) from ordi as t1 inner join owng t2 on t1.goodid=t2.goodid and (t1.qty-t1.realized)>0 and (t1.booking>0 or (t2.qty>0 and t2.zn in ('S','D')) )  and ord.ownerid=t2.ownerid and  t1.orderid=ord.id) as qtyitemavail," +
                  "(select count(t1.goodid) from ordi as t1 inner join owng t2 on t1.goodid=t2.goodid and (t1.qty-t1.realized)>0 and (t2.qty+t1.booking)>=(t1.qty-t1.realized)  and ord.ownerid=t2.ownerid and  t1.orderid=ord.id) as qtyitemavailall," +
                  "(select count(t1.goodid) from ordi as t1 inner join owng t2 on t1.goodid=t2.goodid and (t1.qty-t1.realized)>0 and t2.zn in ('Z','G','Q')  and ord.ownerid=t2.ownerid and  t1.orderid=ord.id) as isz," +
                  "ord.id, ord.code, ord.Name, ord.subjectID, ord.RegDate, ord.Descr, ord.State, ord.SummOrder, ord.lcd,ord.invoice, ord.wishdate, ord.teotrans, ord.teodate from ORD as ord where isnull(ord.TypeOrd,'')='' and ord.SubjectID=" + SubjectId + " and ord.State in (" + state_in + ")" + ((filterbygood.Length > 0) ? " and (ord.id in (select orderid from ORDI where goodCode like '" + flt + "' or Name like '" + flt + "' " + ((filterbygood.Length < 4) ? " or Name like '% " + filterbygood + "%'" : "") + " ) or (''+ord.id) like '%" + filterbygood + "%' or ord.name like '%" + filterbygood + "%' or ord.Code like '%" + filterbygood + "%')" : "");
            sql += " order by ord.id desc";
            DataTable dt = db.GetDbTable(sql);
            if (!showTitle_when_empty && dt.Rows.Count <= 0)
            {
                h3.Visible = false;
                dgList.Visible = false;
                return 0;
            }
            h3.Visible = true;
            dgList.Visible = true;
            dgList.DataSource = dt;

            if (pageIndex >= 0 && dgList.PageSize * pageIndex <= dt.Rows.Count)
            {
                dgList.CurrentPageIndex = pageIndex;
            }
            else
            {
                dgList.CurrentPageIndex = 0; //(dt.Rows.Count-1)/dgList.PageSize; //0;
            }

            dgList.DataBind();

            if (!expanded)
            {
                list.Style.Add("display", "none");

            }
            return dt.Rows.Count;
        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataRowView r = (DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);
                //Order rec = new Order(id, iam);
                string state = ("" + r["State"]).ToUpper();
                bool is_arch = (state == "D" || state == "F" || state == "X");
                bool isee = ("" + r["iseen"] != "0");//ViewArticle.CheckIseen(iam, new sObject(id, Order.TDB));
                if (!isee)
                {
                    e.Item.Cells[1].CssClass = "red";
                    e.Item.Cells[1].ToolTip = "Вы еще не посмотрели эту заявку";
                    e.Item.Cells[1].Text = "* " + id;

                    e.Item.Cells[1].Attributes.Add("id", "recnum_" + id);
                }
                if (""+r["code"] != "") e.Item.Cells[1].Text += " <span class='micro'>" + r["code"] + "</span>";
                e.Item.Attributes.Add("id", "ord_" + id);
                e.Item.Cells[3].Text = "<a onclick=\"openflywin('../order/detailfly.aspx?id=" + id + "', 870, 850, 'Заявка " + r["Name"] + "')\" href='javascript:return false;'>" + r["Name"] + "</a>";
                if (iam.Owner.ShowInvDiscount)
                {
                    decimal disc = cNum.cToDecimal(r["discount"]);
                    e.Item.Cells[3].Text += "<div class=''>предоставлена скидка <span class='bold'>" + disc + "%</span></div>";
                }

                if (!is_arch)
                {
                    e.Item.Cells[3].Text += "<div>Из " + r["qtyitemneed"] + " позиций можно получить " + r["qtyitemavail"] + "</div>";
                }
                e.Item.Cells[3].Text += "<div class='left'><a href='../order/detail.aspx?id="+id+"'>Подробнее</a></div>";

                string invoice = (is_arch || (""+r["invoice"]).Length<7) ? "" : "<a href=\"../downloadfile.ashx?act=invoice&id=" + id + "&sid=" + iam.SessionID + "\" title=' получить счет на оплату'><i class='fa fa-file-text-o icon-act-table' aria-hidden='true'></i></a>";

                bool avalibleGet = !is_arch & (cNum.cToInt(r["qtyitemneed"]) == cNum.cToInt(r["qtyitemavailall"]));


                string stateDescr = (avalibleGet) ? "<span class='small'>Весь товар в наличии.</span>" + 
                    ((cDate.cToDate(r["WishDate"]).Date >= cDate.TodayD.Date) ? "*Получение назначено на " + cDate.cToString(r["WishDate"]) : "") : Order.get_stateorder_descr(""+r["state"]);

                //string btnNextE = (avalibleGet && rec.WishDate < cDate.TodayD) ? "<a href=\"javascript: return false;\" class='' title='Подтвердить к покупке из свободного остатка' onclick=\"javascript: if (confirm('Подтверждаете намерение получить товар?')) {setnewstate('" + id + "','E');}else return false;\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>" : "";
                //string btnNextA = (!avalibleGet && state == "U") ? "<a href=\"javascript: return false;\" class='' title='Предварительно подтвердить к заказу' onclick=\"javascript: if (confirm('Подтверждаете Подтвердить к заказу?')) {setnewstate('" + id + "','A');}else return false;\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>" : "";
                //string btnNextM = (state == "S" && btnNextE=="") ? "<a href=\"javascript: return false;\" class='' title='Согласовать к покупке' onclick=\"javascript: if (confirm('Подтверждаете Согласовать к покупке?')) {setnewstate('" + id + "','M');}else return false;\"><img width='48px' height='48px' src='../simg/iconext.png' /></a>" : "";

                //string btnRepair = (state == "D") ? "<a href=\"javascript: return false;\" class='' title='Вернуть к жизни' onclick=\"javascript: if (confirm('Подтверждаете Возврат к жизни?')) {setnewstate('" + id + "','N');}else return false;\"><img width='48px' height='48px' src='../simg/icorepair.png' /></a>" : "";

                e.Item.Cells[5].Text = stateDescr;

                e.Item.Cells[6].Text = invoice;// + btnRepair;


                e.Item.Cells[7].Text = get_buttons(id,cNum.cToInt(r["subjectID"]),state);

                e.Item.Cells[6].Attributes.Add("nowrap", "nowrap");
                e.Item.Cells[7].Attributes.Add("nowrap", "nowrap");
            }
        }



        private string get_infoorder(Order rec)
        {
            int qn = rec.Items.FindAll(x => (x.Qty - x.Realized) > 0).Count;
            int qm = rec.Items.FindAll(x => (x.Qty - x.Realized) > 0 && x.AvalibleGet > 0).Count;

            return (qn > 0) ? "из " + qn + " позиций можно получить " + qm + "" : "все отгружено";


        }

        private string setbtns(object orderId, string act, string newstate, string pic, string btnword, string title = "", string css = "")
        {
            string btn = "<a href=\"#\" class='microbutton micro " + css + "' title='" + btnword + "' onclick=\"javascript: if (confirm('Подтверждаете " + btnword + "?')) {setnewstate('" + orderId + "','" + newstate + "');}else return false;\"><img src='../simg/16/" + pic + "' />" + title + "</a>";
            return btn;
        }
        private string get_buttons(int id, int subjectID, string state)
        {
            if (!(iam.IsSuperAdmin || iam.ItsMySubj.Contains(subjectID) || iam.PresentedSubjectID == subjectID))
                return "";

            string btn = "";
            string btn_del = "";//"&nbsp;" + setbtns(rec.ID, "sns", "X", "delete.png", "отменить/в архив");

            string btn_copy = "<a href=\"../order/cart.aspx?id=" + id + "&act=copy\" class='microbutton micro' title='Скопировать/повторить' ><i class='fa fa-files-o icon-act-table' aria-hidden='true'></i></a>";
            string btn_edit = "<a href=\"../order/cart.aspx?id=" + id + "&act=edit\" class='microbutton micro' title='Изменить' ><i class='fa fa-pencil icon-act-table' aria-hidden='true'></i></a>";
            string btn_lock = "<a href=\"#\" title='#tooltip#'><i class='fa fa-lock icon-act-table' aria-hidden='true'></i></a>";
            switch (state.ToUpper())
            {
                case "":
                case "N":
                    btn = btn_edit + btn_copy + btn_del;
                    break;
                case "U":
                    btn = btn_edit + btn_copy + btn_del;
                    break;
                case "A":
                case "Z":
                    btn = btn_lock.Replace("#tooltip#", "Заявка в процессе согласования в Сантехкомплект, поэтому её нельзя изменять. Для корректировки свяжитесь со своим менеджером, либо можете ее Отменить и Восстановить") + btn_copy + btn_del;
                    break;
                case "E":
                case "M":
                    btn = btn_lock.Replace("#tooltip#", "Заявка Вами подтверждена и принята к исполнению в Сантехкомплект, поэтому её нельзя изменять. Для корректировки свяжитесь со своим менеджером, либо можете ее Отменить и Восстановить") + btn_copy + btn_del;
                    break;
                case "S":
                    btn = btn_lock.Replace("#tooltip#", "Заявка согласована в Сантехкомплекте, поэтому её нельзя изменять. Для корректировки свяжитесь со своим менеджером, либо можете ее Отменить и Восстановить") + btn_copy + btn_del;
                    break;
                case "R":
                    btn = btn_lock.Replace("#tooltip#", "Заявка в процессе комплектации, поэтому её нельзя изменять. Для корректировки свяжитесь со своим менеджером, либо можете ее Отменить и Восстановить") + btn_copy + btn_del;
                    break;
                case "F":
                    btn = btn_lock.Replace("#tooltip#", "Заявка выполнена, поэтому её нельзя изменять. Воспользуйтесь функцией копирования") + btn_copy;
                    break;
                case "X":
                case "D":
                    btn = btn_lock.Replace("#tooltip#", "Заявка отменена, поэтому её нельзя изменять. Для корректировки восстановите её") + btn_copy;

                    break;

            }
            return btn;
        }

        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            Show_orders(SubjectID, TitleList, stateIN, false, true, filterGood, e.NewPageIndex);
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