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
    
    public partial class orderdefault2 : p_p
    {
        enum ORDERSELECT { TN_BR, BR_TN };
        static ORDERSELECT _ORDERSELECT = ORDERSELECT.TN_BR;


        //Order CurrentOrder
        //{
        //    get { Order ord = Order.GetCurrentOrder(iam); if (ord.IsEmpty && eID > 0) ord = new Order(eID, iam, true); else Order.BeginWork(iam, ord); return ord; }

        //}

        private Subject _subj;

        private Subject subj
        {
            get
            {
                if (_subj == null)
                    _subj = new Subject(iam.PresentedSubjectID, iam);
                return _subj;
            }
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
           
            
            TabControl1.InitTabContainer("tasklisttabs");
            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Классификатор", pnlStruct);
            TabControl1.AddTabpage("Справочник номенклатуры", pnlList);

            if (iam.ID < 100000)
            {
                lbMessage.Text = "Для зарегистрированных пользователей доступна функция формирования Заявки.<br/><a href='../account/login.aspx?pp=goodlist'>зарегистрироваться>></a>";

                pnlOrder.Visible = false;
            }
            else
            {
                lbMessage.Text = "";
                TabControl1.AddTabpage("Текущая заявка", pnlOrder);


            }
            loadDinFilter();

            if (!IsPostBack)
            {
                if (iam.ID <= 0) Response.Redirect("../default.aspx");
                ((workpage)this.Master).SelectedMenu = "order";




                if (Request["act"] == "recl" && "" + Request["code"] != "")
                    iam.CF_SourceNomen = "all";
                else if (Request["act"] == "copy" && "" + Request["id"] != "")
                {

                }
                else
                {
                    iam.CF_SourceNomen = UserFilter.Load(iam.ID, "reg");
                    chIncash.Checked = (UserFilter.Load(iam.ID, "chincash") == "Y");
                }

                set_list_regime();
                set_pagesize_regime();


                if (subj.ID > 0)
                {
                   // hdSubjectID.Value = subj.ID.ToString();

                    show_subj_info();
                                      
                    rpOrders.Visible = rpArchive.Visible = false;
                }
                
                eID = cNum.cToInt(Request["id"]);
                if (iam.CurOrder == null || (iam.CurOrder.ID != eID && eID>0))
                    iam.CurOrder = new Order(eID, iam, true);

                if (eID == 0)
                {
                    //Order.FinishWork(iam);
                }
                else if (eID > 0 || Request["go"]=="ord")
                {
                    if (_ACT_ == "edit")
                    {
                        Order o = new Order(eID, iam, true);
                        //Order.BeginWork(iam, o);
                    }
                    else if (_ACT_ == "copy")
                    {
                        //Order.FinishWork(iam);
                        eID = 0;
                        iam.CurOrder.MakeCopy();
                    }
                    Order ord = iam.CurOrder;
                    _subj = null;
                    iam.PresentedSubjectID = ord.SubjectID;
                    lbSubject.Text = ord.Subject.Name + ", ИНН " + ord.Subject.INN;
                    lbOrder.Text = "" + ord.ID;
                    txNameOrder.Text = ord.Name;
                    txDescr.Text = ord.Descr;
                    lbRegDate.Text = ord.RegDate.ToShortDateString();
                    lbStateOrder.Text = ord.StateDescr;
                    TabControl1.SelectedTabIndex = 2;

                    txNameOrder.ReadOnly = txDescr.ReadOnly = OrderReadOnly;
                    lbReadonly.Visible = OrderReadOnly;

                    load_curentorder();
                    CommentList1.DataObject = new sObject(eID, Order.TDB);
                    CommentList1.CanAddComment = true;
                    CommentList1.Event_AddedComment = new EventHandler(CommentList1_AddingComment);
                }
                else
                {
                    lbReadonly.Visible = false;
                    CommentList1.Visible = false;
                }

                if (_ACT_ == "recl" && "" + Request["code"] != "")
                {
                    txSearch.Text = "" + Request["code"];
                    TabControl1.SelectedTabIndex = 1;
                }
                else if (_ACT_ == "copy" && Request["id"] != "")
                {
                    TabControl1.SelectedTabIndex = 2;
                }
                
                if (TabControl1.IsActiveTabpage(pnlList))
                {
                    if (!(Request["act"] == "recl" && "" + Request["code"] != ""))
                    {
                        load_brends();
                        load_TK();
                        load_xName();
                    }
                    txSearch_TextChanged(null, null);
                }
            }


            if (refresh.Value == "Y" && TabControl1.IsActiveTabpage(pnlOrder) || (!IsPostBack && Request["go"]=="ord"))
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
                    load_TK(UserFilter.Load(iam.ID, "TK"));
                    load_brends(UserFilter.Load(iam.ID, "brend"));
                    load_xName(UserFilter.Load(iam.ID, "Name"));
                    txSearch_TextChanged(null, null);
                }
                else
                {
                    load_struct();
                }

            }
            if (TabControl1.IsActiveTabpage(pnlStruct) && struct_place.Text == "")
            {
                load_struct();
            }
            if (iam != null && iam.ID > 0 && iam.CurOrder != null && iam.CurOrder.Summ > 0)
            {

                TabControl1.TabTitles[2] = "Заявка на сумму " + cNum.cToDecimal(iam.CurOrder.Summ, 2) + "р.";
                TabControl1.TabCssPrefix[2] = "bold";
                if (iam.CurOrder.ID == 0 || iam.CurOrder.Changed)
                    TabControl1.TabTitles[2] += " не сохранена";

            }

        }

        private DataTable fltmatrix;
        private DataTable getMatrix()
        {
            if (iam.CF_SelectedTKs == "") return fltmatrix;
            if (fltmatrix != null) return fltmatrix;
            string seltk = iam.CF_SelectedTKs;
            DataTable dtchrs = db.GetDbTable("select keychars from imgtntk where t='tk' and name='" + seltk + "'");

            string[] chrs = ("" + dtchrs.Rows[0][0]).Split(',');
            int i = 0;
            string sql = "select distinct ";
            foreach (string chr in chrs)
            {
                sql += "(select ChrVal from GOODCH where goodcode=g.goodcode and Chr='" + chr + "' ) as [" + chr + "],";
            }
            sql += " from vGOOD100000 as g where g.xTK='"+seltk+"'";
            sql = sql.Replace(", from", " from");
            fltmatrix = db.GetDbTable(sql);
            return fltmatrix;
        }

        private void loadDinFilter()
        {
            /* попробовать матрицу сделать:
             * select distinct 
(select ChrVal from GOODCH where goodcode=g.goodcode and Chr='Бренд' ) as [Бренд],
(select ChrVal from GOODCH where goodcode=g.goodcode and Chr='Модель' ) as [Модель],
(select ChrVal from GOODCH where goodcode=g.goodcode and Chr='Объем' ) as [Объем]
 from vGOOD100000 as g where g.xTK='Баки расширительные' group by [Бренд],[Модель],[Объем]
             * 
             */
            pnlDinflt.Controls.Clear();

            if (iam.CF_SelectedTKs == "") return;
            string seltk = iam.CF_SelectedTKs;
            DataTable dtchrs = db.GetDbTable("select keychars from imgtntk where t='tk' and name='" + seltk + "'");

            string[] chrs = ("" + dtchrs.Rows[0][0]).Split(',');
            int i = 0;
            foreach (string chr in chrs)
            {
                Label lb = new Label();
                lb.Text = chr+": ";
                lb.CssClass = "white bold";
                DropDownList dl = new DropDownList();
                //dl.ClientIDMode = ClientIDMode.Static;
                dl.Attributes.Add("onchange", "changefilter()");
                //dl.Attributes.Remove("ID");
                //dl.Attributes.Add("ID", "dl" + (i++));
                dl.Attributes.Add("data", chr);
                dl.Items.Add(new ListItem("", ""));
                foreach (DataRow r in db.GetDbTable("select ChrVal, count(*) as q  from GOODCH where Chr='" + chr + "' and GoodCode in ( select GoodCode from vGood" + iam.OwnerID + " where xTK='" + seltk + "' ) group by ChrVal order by ChrVal").Rows)
                {
                    dl.Items.Add(new ListItem("" + r["ChrVal"] + " (" + r["q"] + ")", "" + r["ChrVal"]));
                }
                dl.AutoPostBack = true;
                dl.SelectedIndexChanged += new EventHandler(dinfilter_Change);
                pnlDinflt.Controls.Add(lb);
                pnlDinflt.Controls.Add(dl);
            }
           // string sql = make_query();
        }

        private void dinfilter_Change(object sender, EventArgs e)
        {
            string sql = make_query();
            //DataTable mtrx = getMatrix();
            string chr = "";
            string senderFlt = ((DropDownList) sender).Attributes["data"];
            string senderFltSel = ((DropDownList) sender).SelectedValue;
            string mtrxwhere = "";
            foreach (Control dl in pnlDinflt.Controls)
            {
                if (dl.GetType() == typeof(DropDownList) && ((DropDownList)dl).Attributes["data"] == senderFlt)
                {
                    ListItem li = ((DropDownList) sender).Items.FindByValue(senderFltSel);
                }
                if (dl.GetType() == typeof(DropDownList) && ((DropDownList)dl).Attributes["data"] != "")
                {
                    chr = ((DropDownList) dl).Attributes["data"];
                    DataTable dt = db.GetDbTable("select ChrVal, count(*) as q  from GOODCH where Chr='" + chr + "' and GoodCode in (" + sql + " ) group by ChrVal order by ChrVal");
                    foreach (ListItem li in ((DropDownList)dl).Items)
                    {
                        li.Attributes.Add("class", "");
                        mtrxwhere = (senderFltSel != "") ? senderFlt + "='" + senderFltSel + "'" : "";
                        mtrxwhere += ((mtrxwhere != "") ? " and " : "") + chr + "='" + li.Value + "'";
                        //if (mtrx.Select(mtrxwhere).Length > 0)
                        {
                            li.Attributes["class"] = (dt.Select("ChrVal='" + li.Value + "'").Length == 0)?"gray":"black";
                        }
                        //else
                        //{
                        //    li.Attributes["class"] = "gray"; //"gray micro";
                        //}
                    }
                    
                    
                }
            }
            //txSearch_TextChanged(sender, e);
            DataTable dtg = db.GetDbTable(make_query(false));
            dgList.DataSource = dtg;
            dgList.DataBind();
        }

        private string  make_query(bool shortselect=true)
        {
            
            int ownerId = iam.OwnerID;
            string sql = "select "+((shortselect)?"GoodCode":"*")+" from vGOOD" + ownerId + " where xTK='" + iam.CF_SelectedTKs + "' and GoodCode in (select goodcode from GOODCH where #WHERE#)";
            string where = "1=1";

            foreach (Control dl in pnlDinflt.Controls)
            {
                if (dl.GetType() == typeof(DropDownList) && ((DropDownList)dl).SelectedValue.Trim() != "" && ((DropDownList)dl).Attributes["data"] != "")
                {
                    where += String.Format("and GoodCode in (select goodcode from GOODCH where Chr='" + ((DropDownList)dl).Attributes["data"] + "' and ChrVal='{0}')", ((DropDownList)dl).SelectedValue);
                }
            }
            sql = sql.Replace("#WHERE#", where);
            return sql;
        }


        private void show_subj_info()
        {
            lbSubject.Text = subj.Name + ", ИНН " + subj.INN;
            lbBalance.Text = "";
            show_blnc(subj.ID, subj.CodeDG);
        }

        private void load_curentorder()
        {
            if (iam.CurOrder==null) iam.CurOrder = new Order(eID,iam, true);
            Hashtable iq = new Hashtable();
            if (iam.CurOrder.ID == 0 && iam.CurOrder.Dg == "")
                iam.CurOrder.Dg = subj.CodeDG;
            Order ord = iam.CurOrder;
            if (ord.ID > 0)
                ViewArticle.AddView(iam, ord.ThisObject);
            link4prn.Text = (ord.ID > 0) ? "<a href='javascript:return false;' class='button' onclick=\"openflywin('../order/detailfly.aspx?id=" + ord.ID + "', 850, 800, 'Заявка " + ord.Name + " от " + ord.RegDate.ToShortDateString() + "')\"><img src='../simg/16/printer.png'> печать</a>" : "";

            link4prn.Visible = (link4prn.Text != "");
            lbSubject.Text = ord.Subject.Name + ", ИНН " + ord.Subject.INN;
            lbOrder.Text = "" + ord.ID;
            txNameOrder.Text = ord.Name;
            txDescr.Text = ord.Descr;
            lbRegDate.Text = ord.RegDate.ToShortDateString();
            lbStateOrder.Text = ord.StateDescr;
            lbCode.Text = ord.Code;
            

            btnClear.Visible = lbtnSave.Visible = !OrderReadOnly;


            if (ord.ID == 0) btnClear.Visible = false;
            set_state_button(ord.State);

            decimal smb = ord.SummBase;
            decimal sm = ord.Summ;
            foreach (DataRow r in ord.ItemsDt.Rows)
            {
                OrderItem item = ord.FindItem(cNum.cToInt(r["goodID"]));
                decimal pr = (item == null) ? cNum.cToDecimal(r["pr_b"]) : item.Price;
                decimal q = (Request["q_" + r["goodID"]] != null) ? cNum.cToDecimal(Request["q_" + r["goodID"]]) : ((item == null) ? 1 : item.Qty);

                string descr = (Request["ds_" + r["goodID"]] != null) ? "" + Request["ds_" + r["goodID"]] : ((item == null) ? "" : item.Descr);

                if (q <= 0) q = 1;

                decimal s = q * pr;
                if (item == null)
                {
                    item = new OrderItem() { Mark = "*", GoodId = cNum.cToInt(r["goodID"]), Zn = "" + r["zn"], Zn_z = "" + r["zn_z"], CurIncash = cNum.cToDecimal(r["qty"]), GoodCode = "" + r["GoodCode"], Name = "" + r["Name"], Qty = q, Price = pr, Summ = s, state = "N", Descr = descr, article = "" + r["article"], ed = "" + r["ed"], SummBase = s };
                    ord.AddItem(item);
                }
                else
                {
                    ord.ChangeItem(item, q, descr);
                    item.Mark = "";
                }
            }
            DataView dv = ord.ItemsDt.DefaultView;
            dv.Sort = "name";
            dgOrder.DataSource = dv;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();

            show_blnc(ord.SubjectID, ord.Dg);
        }
        
        private void set_pagesize_regime()
        {
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
        }

        private void set_list_regime()
        {
            string active = "sourcelist-active";
            string pass = "sourcelist-passive";
            switch (iam.CF_SourceNomen)
            {
                case "spec":
                    lnkMyFavor.CssClass = pass;
                    lnkSpecial.CssClass = active;
                    lnkJust.CssClass = pass;
                    break;
                case "my":
                    lnkMyFavor.CssClass = active;
                    lnkSpecial.CssClass = pass;
                    lnkJust.CssClass = pass;
                    break;
                case "all":
                default:
                    lnkMyFavor.CssClass = pass;
                    lnkSpecial.CssClass = pass;
                    lnkJust.CssClass = active;
                    break;

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

            if (state.ToUpper() == "U")
            {
                lbtnNextStady.Visible = true;
                lbtnNextStady.ToolTip = "Подтвердить к заказу";
                lbtnNextStady.CommandArgument = "A";
            }
            else if (state.ToUpper() == "S")
            {
                lbtnNextStady.Visible = true;
                lbtnNextStady.ToolTip = "Подтвердить к покупке";
                lbtnNextStady.CommandArgument = "M";
            }
            else
            {
                lbtnNextStady.Visible = false;
                lbtnNextStady.CommandArgument = "";
            }
            if (cStr.Exist(state.ToUpper(), new string[] { "N", "U", "A", "Z", "S" }))
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

            if (iam.SubjectID != iam.CurOrder.SubjectID)
                lbtnCancel.Visible = lbtnNextStady.Visible = false;
        }

     

        //private void load_struct_old()
        //{
        //    int i = 0;
        //    string regwhere = "";
        //    switch (_REG_)
        //    {
        //        case "spec":
        //            regwhere = " and id in (select goodid from OWNG where OwnerID=" + OwnerID + " and zn_z='NL')";
        //            break;
        //        case "my":
        //            regwhere = " and id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.SubjectID + "))";
        //            break;
        //        case "all":
        //        default:
        //            regwhere = "";
        //            break;
        //    }

        //    DataTable dt_tn = db.GetDbTable("select xTN, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + OwnerID + ") " + regwhere + " group by xTN order by xTN");
        //    DataTable dt_tks = db.GetDbTable("select xTN,xTK, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + OwnerID + ") " + regwhere + " group by xTN,xTK order by xTK");

            
        //    StringBuilder sb = new StringBuilder();
        //    foreach (DataRow r_tn in dt_tn.Select("xTN<>''"))
        //    {
        //        TreeNode tnnode = new TreeNode(r_tn["xTN"].ToString().ToUpper());

        //        sb.Append("<div class='blocktn' >");
                
        //        sb.Append("<strong>" + r_tn["xTN"].ToString().ToUpper() + "</strong><br/>");
        //        sb.Append("<div class='blocktk'><div class='floatLeft-90'><ul>");
        //        foreach (DataRow r_tk in dt_tks.Select("xTN='" + r_tn["xTN"] + "'"))
        //        {

        //            string tk = "" + r_tk["xTK"];
        //            tk = tk.Replace(",", "@");
        //            TreeNode tntk = new TreeNode(r_tk["xTK"].ToString(), tk.ToLower());
                    
        //            bool ch = (SELECTED_TKS.ToLower().IndexOf("" + tk.ToLower()) > -1);
        //            tntk.Checked = ch;
        //            sb.Append(" <li><input type='checkbox' value='" + tk.ToLower() + "' " + ((ch) ? "checked" : "") + " name='ch_tks' id='ch_tks_" + i + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" /><label for='ch_tks_" + i + "' " + ((ch) ? "class='check'" : "class='uncheck'") + " >" + r_tk["xTK"] + "</label></li>");
        //            tnnode.ChildNodes.Add(tntk);
        //            i += 1;
        //        }
        //        sb.Append("</ul></div><div class='floatRight-10'><img class='arrowtolist' src='../simg/32/arrow16x32.png' style='cursor:pointer' title='перейти в справочник номенклатуры' alt='>>' onclick=\"setActPunct(3, 1,'ctl00_ContentPlaceHolder1_TabControl1')\"/></div><div class='clearBoth'></div></div>");// block_TK
        //        sb.Append("</div>");// block_TN
        //    }
        //    sb.Append("<div style='clear:both;'></div>");
        //    struct_place.Text = sb.ToString();
        //}

        private void load_struct11()
        {
            int i = 0;
            string regwhere = "";
            switch (iam.CF_SourceNomen)
            {
                case "spec":
                    regwhere = " good.id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + " and zn_z='NL')";
                    break;
                case "my":
                    regwhere = " good.id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + "))";
                    break;
                case "all":
                default:
                    regwhere = "";
                    break;
            }
            List<xTN> listTN = xTN_db.GetList(regwhere);
            StringBuilder sb = new StringBuilder();
            if (listTN.Count == 0)
                sb.Append("<br/><br/><br/><p>У Вас, по всей видимости еще не было заказов, поэтому выберите другой <span class='bold'>Источник списка номенклатуры</span></p><br/><br/>");


            foreach (xTN r_tn in listTN)
            {

                sb.Append("<div class='blocktn' style=\"background-image: url('../media/grp"+r_tn.ID+".png');\">");
                sb.Append("<strong>" + r_tn.Title.ToUpper() + "</strong><br/>");
                sb.Append("<div class='blocktk'><div class='floatLeft-90'><ul>");
                foreach (xTK r_tk in xTK_db.GetList(r_tn.Title, regwhere))
                {
                    string tk = r_tk.Title;
                    tk = tk.Replace(",", "@");

                    bool ch = (iam.CF_SelectedTKs.ToLower().IndexOf("" + tk.ToLower()) > -1);
                    sb.Append(" <li><input type='checkbox' value='" + tk.ToLower() + "' " + ((ch) ? "checked" : "") + " name='ch_tks' id='ch_tks_" + i + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" /><label for='ch_tks_" + i + "' " + ((ch) ? "class='check'" : "class='uncheck'") + " >" + r_tk.Title + "</label></li>");
                    i += 1;
                }
                sb.Append("</ul></div><div class='floatRight-10'><img class='arrowtolist' src='../simg/32/arrow16x32.png' style='cursor:pointer' title='перейти в справочник номенклатуры' alt='>>' onclick=\"setActPunct(3, 1,'ctl00_ContentPlaceHolder1_TabControl1')\"/></div><div class='clearBoth'></div></div>");// block_TK
                sb.Append("</div>");// block_TN
            }
            sb.Append("<div style='clear:both;'></div>");
            struct_place.Text = sb.ToString();
        }

        private void load_struct()
        {

            int i = 0;
            string regwhere = "";
            switch (iam.CF_SourceNomen)
            {
                case "spec":
                    regwhere = " good.id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + " and zn_z='NL')";
                    break;
                case "my":
                    regwhere = " good.id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + " and state<>'D'))";
                    break;
                case "all":
                default:
                    regwhere = "";
                    break;
            }
            List<xTN> listTN = xTN_db.GetList(regwhere);
            StringBuilder sb = new StringBuilder();
            if (listTN.Count == 0)
                sb.Append("<br/><br/><br/><p>У Вас, по всей видимости еще не было заказов, поэтому выберите другой <span class='bold'>Источник списка номенклатуры</span></p><br/><br/>");


            foreach (xTN r_tn in listTN)
            {
                sb.Append("<div class='blocktn'>");  // style=\"background-image: url('../media/grp" + r_tn.ID + ".png');\"
                sb.Append("<strong>" + r_tn.Title.ToUpper() + "</strong><br/>");
                sb.Append("<div class='blocktk'><div class='floatLeft-90'><ul>");
                foreach (xTK r_tk in xTK_db.GetList(r_tn.Title, regwhere))
                {
                    string tk = r_tk.Title;
                    tk = tk.Replace(",", "@");

                    bool ch = (iam.CF_SelectedTKs.ToLower().IndexOf("" + tk.ToLower()) > -1);
                    sb.Append(" <li><input type='checkbox' value='" + tk.ToLower() + "' " + ((ch) ? "checked" : "") + " name='ch_tks' id='ch_tks_" + i + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" /><label for='ch_tks_" + i + "' " + ((ch) ? "class='check'" : "class='uncheck'") + " >" + r_tk.Title + "</label></li>");
                    i += 1;
                }
                sb.Append("</ul></div><div class='floatRight-10'><img class='arrowtolist' src='../simg/32/arrow16x32.png' style='cursor:pointer' title='перейти в справочник номенклатуры' alt='>>' onclick=\"setActPunct(3, 1,'ctl00_ContentPlaceHolder1_TabControl1')\"/></div><div class='clearBoth'></div></div>");// block_TK
                sb.Append("</div>");// block_TN
            }
            sb.Append("<div style='clear:both;'></div>");
            struct_place.Text = sb.ToString();
            //strWin.Text = sb.ToString();
        }


        private bool checkPicture(GoodGrp grp)
        {
            bool ret = false;
            
            string filename = "grp" + grp.ID + ".png";
            if (!webIO.CheckExistFile(@"../media/" + filename))
            {
                FileStream stream = new FileStream(webIO.GetAbsolutePath(@"../media/" + filename), FileMode.OpenOrCreate);
                BinaryWriter writer = new BinaryWriter(stream);
                try
                {

                    DataTable dt = db.GetDbTable("select img from imgtntk where id=" + grp.ID);
                    byte[] buf = (byte[])dt.Rows[0][0];
                    writer.Write(buf);
                    ret = true;
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    writer.Close();
                    stream.Close();
                }
            } else
            {
                ret = true;
            }
            return ret;
        }

        private void load_exist_orders()
        {
            return;
            string sql;
            if (iam.PresentedSubjectID > 0)
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where isnull(ord.state,'') not in ('D','X') and ord.SubjectID=" + iam.PresentedSubjectID + "";
            else
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where subj.EmailTAs like '%" + iam.Email + "%' and isnull(ord.state,'') not in ('D','X')";

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
                nr["State"] = Order.get_stateorder_descr("" + r["State"]);
                nr["SummOrder"] = cNum.cToDecimal(r["SummOrder"], 2);
                //if (iam.SubjectID > 0)
                nr["linkchange"] = "<a href='goodlist.aspx?id=" + r["id"] + "&act=edit'>изменить</a>";
                dto.Rows.Add(nr);
            }
            rpOrders.DataSource = dto;
            rpOrders.DataBind();
        }

        

        bool OrderReadOnly
        {
            get
            {
                return !(cStr.Exist(iam.CurOrder.State.ToUpper(), new string[] { "", "U", "N" }) && iam.ID > 0 && iam.CurOrder.SubjectID == iam.PresentedSubjectID);
            }
        }


        protected void show_blnc(int subjectId, string codeDg)
        {
            DGinfo d = Subject.GetDg(subjectId, codeDg);
            lbBalance.Text = "ДОГОВОР #" + d.CodeDogovor + ": Тек. баланс " + (-1 * d.CurrentDZ) + " руб., допустимый лимит: " + d.LimitDZ + " руб.<br/>" + (((d.LimitDZ - d.CurrentDZ) > 0) ? " ВОЗМОЖНА ОТГРУЗКА НА СУММУ " + (d.LimitDZ - d.CurrentDZ) + "руб." : " БЕЗ ПРЕДОПЛАТЫ ОТГРУЗКА НЕВОЗМОЖНА") + "</li>";

        }


        protected void load_brends(string default_value = "")
        {

            string where = "";

            if (dlTK.SelectedValue != "")
                where += " and xTK='" + dlTK.SelectedValue + "'";

            string seltk = get_selected_tks();

            if (seltk != "")
                where = " and xTK in (" + seltk + ")";

            string whereqty = (chIncash.Checked) ? " and owng.qty>0" : "";

            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select brend, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + ") and id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((where != "") ? where : "") + " group by brend");
                    break;
                case "spec":
                    dt = db.GetDbTable("select brend, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + " and zn_z='NL') " + ((where != "") ? where : "") + " group by brend");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select brend, count(id) as qty from good where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + ") " + ((where != "") ? where : "") + " group by brend ");
                    break;
            }



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
            foreach (string s in iam.CF_SelectedTKs.Split(','))
            {
                if (s != "")
                    cStr.Add(ref seltk, "'" + s.Replace('@', ',') + "'");
            }
            return seltk;
        }

        protected void load_TK(string default_value = "")
        {
            string brend = dlBrends.SelectedValue;
            dlTK.Items.Clear();
            dlTK.ClearSelection();
            dlTK.Items.Add(new ListItem("", ""));
            string seltk = get_selected_tks();
            string whereqty = (chIncash.Checked) ? " and owng.qty>0" : "";
            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select xTK, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + " ) and id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
                    break;
                case "spec":
                    dt = db.GetDbTable("select xTK, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + " and zn_z='NL') " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select xTK, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + ") " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
                    break;
            }

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

            string tk = dlTK.SelectedValue;
            string brend = dlBrends.SelectedValue;
            dlNames.Items.Clear();
            dlNames.Items.Add(new ListItem("", ""));
            dlNames.ClearSelection();
            string seltks = get_selected_tks();
            string whereqty = (chIncash.Checked) ? " and owng.qty>0" : "";

            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select xName, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + ") and id in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
                    break;
                case "spec":
                    dt = db.GetDbTable("select xName, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + " and zn_z='NL') " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select xName, count(id) as qty from GOOD where id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + whereqty + ") " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
                    break;
            }


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
            DataTable dt = new DataTable();
            DataView dv;
            string RowFilter = "";
            string where_conditions = "1=1";
            where_conditions = get_where_condition();

            string typePrice = "pr_b";
            if (subj != null)
                typePrice = subj.PriceType;

            switch (iam.CF_SourceNomen)
            {
                case "my":
                dt = db.GetDbTable("select GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend,good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D' and owng.goodcode in (select goodcode from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + "))" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "spec":
                dt = db.GetDbTable("select GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend, good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D' where owng.zn_z='NL' and isnull(owng.qty,0)>0" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "all":
                default:
                dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend, good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D'" + ((where_conditions == "") ? "" : " where " + where_conditions) + "");
                    break;
            }
           

            Cache.Insert("MyDT", dt, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);


            dv = dt.DefaultView;

            dv.Sort = "zn, Name";
            dgList.DataSource = dv;
            dgList.CurrentPageIndex = 0; lbSelTN.Visible = true;
            if (iam.CF_SelectedTKs != "")
            {
                lbSelTN.Text = iam.CF_SelectedTKs;

            }
            else
            {
                lbSelTN.Text = "Все товарные направления";

            }

            try
            {
                dgList.DataBind();
            }
            catch { }


        }
        private string get_where_condition()
        {
            string where_conditions = "1=1";

            if (dlBrends.SelectedValue != "")
                where_conditions += " and brend like '" + dlBrends.SelectedValue + "%'";
            string seltks = get_selected_tks();
            if (seltks != "")
                where_conditions += " and xTK in (" + seltks + ")";
            if (dlTK.SelectedValue != "")
                where_conditions += " and xTK like '" + dlTK.SelectedValue + "%'";
            if (dlNames.SelectedValue != "")
                where_conditions += " and xName like '" + dlNames.SelectedValue + "%'";
            string searchW = txSearch.Text.Trim().Replace(":", @"/").Replace("  ", " ").Replace(" ", "%");
            if (txSearch.Text.Length >= 4)
            {
                where_conditions += string.Format(" and (name like '%{0}%' or descr like '%{0}%' or goodcode like '%{0}%' )", searchW);
            }
            else if (txSearch.Text.Length > 0)
            {
                where_conditions += string.Format(" and (name like '{0}%' or name like '% {0}%'  or goodcode like '%{0}%')", searchW);
            }
            if (chIncash.Checked)
                where_conditions += " and qty>0";
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
            txSearch_TextChanged(sender,e);
        }

        string getQtyInCurrentOrder(int goodId)
        {
            OrderItem oi = iam.CurOrder.FindItem(goodId);
            return (oi != null) ? oi.Qty.ToString() : "";
        }


        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                //bool exist = (itemstack.Value.IndexOf("" + r["ID"]) >= 0);
                string id = "" + r["id"];
                string zn = ("" + r["zn"]).ToUpper();
                string zn_z = ("" + r["zn_z"]).ToUpper();
                string zt = ("" + r["zt"]).ToUpper();

                string qs = getQtyInCurrentOrder(cNum.cToInt(r["Id"]));
                e.Item.Attributes.Add("id", "tr_" + r["ID"]);
                if (qs != "") e.Item.CssClass = "selitem";
                if (iam.ID > 0 && !OrderReadOnly)
                {
                    e.Item.Cells[1].Text = "<input id='ch_" + id + "' type='checkbox' onclick=\"checkChange('" + id + "');shw(this.id,'" + id + "')\" " + ((qs != "") ? " value='" + id + "' checked='checked'" : "") + ")/>";
                    e.Item.Cells[1].Text += "&nbsp;<input title='укажите нужное количество' id='qch_" + id + "' onchange=\"checkNumeric(this.id,1);changeQty('" + id + "');\"  maxlength='4' style='width:34px' type='text' value='" + qs + "' />";
                }
                else
                    e.Item.Cells[1].ToolTip = "В текущую заявку нельзя добавлять товары. Воспользуйтесь кнопкой 'Новая заявка'";

                if (zn == "Z")
                {
                    e.Item.Cells[3].Text = "<span class='red' title='Заказной товар " + zn + "," + zt + "," + zn_z + "'>*</span> ";
                }
                else
                {
                    e.Item.Cells[3].Text = "<span title='Складской товар " + zn + "," + zt + "," + zn_z + "'>&nbsp;</span>";
                }

                e.Item.Cells[2].Text = (txSearch.Text.Length > 0) ? webInfo.select_search(txSearch.Text, "" + r["Goodcode"]) : "" + r["Goodcode"];
                e.Item.Cells[3].Text += (txSearch.Text.Length > 0) ? webInfo.select_search(txSearch.Text, "" + r["Name"]) : "" + r["Name"];
                if ("" + r["ens"] != "")
                    e.Item.Cells[6].Text = "<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r["ens"] + "/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>";
                if (webIO.CheckExistFile("../media/gimg/" + r["img"]))
                {
                    e.Item.Cells[6].Text += "<img class='microimg' src='../media/gimg/" + r["img"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                }
                if (iam.ID > 0)
                {
                    decimal q = cNum.cToDecimal(r["qty"]);
                    if (iam.SubjectID > 0)
                    {
                        e.Item.Cells[4].Text = stategood(cNum.cToInt(id), q, zn, zn_z);
                    }
                    else
                    {
                        e.Item.Cells[4].Text = "" + q;// +" " + stategood(cNum.cToInt(id), q, zn, zn_z) + "";
                    }

                    decimal c = cNum.cToDecimal(r["pr_opt"], 2);//                    decimal c = cNum.cToDecimal(r["price"], 2);
                    decimal c_b = cNum.cToDecimal(r["pr_b"], 2);

                    if ((zn == "S" || (zn + zt) == "Z1" || zn_z == "NL") && (c_b > 0 || c > 0))
                    {
                        e.Item.Cells[5].Text = "" + cNum.cToDecimal(c, 2);
                        e.Item.Cells[5].ToolTip = (c < c_b) ? "Это Ваша цена, а базовая цена " + c_b + "р." : "";
                    }
                    else if (q > 0 && (zn == "D" || zn == "Q"))
                    {
                        e.Item.Cells[5].Text = "<span class='red' title='Товар выводится из ассортимента, это остаток, лучше свяжитесь с менеджером'>*</span>" + cNum.cToDecimal(c, 2);
                    }
                    else
                    //if (("" + r["zn"]).ToUpper() == "Z" && c>0)
                    {
                        e.Item.Cells[5].Text = "<span class='red bold' title='Товар заказной, цену нужно получить/уточнить у своего менеджера'><img src='../simg/32/ph.png' style='cursor:pointer; width:16px;' onclick=\"openflywin('../common/gimail.aspx?id=" + id + "', 400, 300, 'Запрос информации о товаре')\" title='по запросу' alt='?' " + get_emailmess(id) + "/></span>";
                    }
                    //e.Item.Cells[4].ToolTip = "Двойной клик в этом месте - это быстрый запрос менеджеру по этому товару.";
                    e.Item.Cells[3].Text += " <img src='../simg/16/email.png' style='cursor:pointer; width:10px;' onclick=\"openflywin('../common/gimail.aspx?id=" + r["ID"] + "', 400, 300, 'Запрос информации о товаре') \" title='Быстрый запрос по этой позиции'>";
                }
            }
        }

        string get_emailmess(string good_id)
        {
            string r = "ondblclick=\"openflywin('../common/gimail.aspx?id=" + good_id + "', 400, 300, 'Запрос информации о товаре')\"";
            return r;
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
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                bool exist = (itemstack.Value.IndexOf("" + r["GoodID"]) >= 0);
                bool ordReadOnly = OrderReadOnly;
                if (ordReadOnly)
                    e.Item.Cells[1].Text = "";
                e.Item.Attributes.Add("id", "tr_" + r["GoodID"]);
                if ("" + r["mark"] == "*")
                    e.Item.Cells[2].CssClass = e.Item.Cells[3].CssClass = e.Item.Cells[4].CssClass = e.Item.Cells[6].CssClass = e.Item.Cells[7].CssClass = "blue";
                e.Item.Cells[4].Attributes.Add("id", "pr_" + r["GoodID"]);
                e.Item.Cells[4].Text = "" + cNum.cToDecimal(r["pr"], 2);

                e.Item.Cells[5].Text = "<input " + ((ordReadOnly) ? "readonly" : "") + " class='center qtyfield' maxlength='5' name='q_" + r["GoodID"] + "' id='q_" + r["GoodID"] + "' type='text' onchange=\"checkNumeric(this.id,1);recount('" + r["GoodID"] + "');\" value='" + cNum.cToDecimal(r["qty"], 1) + "' />";


                decimal q = cNum.cToDecimal(r["Curincash"]);
                decimal q_ord = cNum.cToDecimal(r["qty"]);


                e.Item.Cells[7].Attributes.Add("id", "sm_" + r["GoodID"]);
                e.Item.Cells[7].Text = "" + cNum.cToDecimal(r["sum"], 2);

                e.Item.Cells[8].Text = "<input maxlength='150' style='width:150px;' name='ds_" + r["GoodID"] + "' id='ds_" + r["GoodID"] + "' type='text' " + ((ordReadOnly) ? "readonly" : "") + " class='descritem' value='" + r["descr"] + "' />";
                string osttext = wstcp.order.detailfly.get_ost_info(iam.CurOrder.State, "" + r["zn"], cNum.cToDecimal(r["Curincash"]), cNum.cToDecimal(r["qty"]), cNum.cToDecimal(r["booking"]), cNum.cToDecimal(r["realized"]), cDate.cToDate(r["WDate"]), "" + r["comment"], cNum.cToInt(r["srokpost"]));

                e.Item.Cells[9].Text = osttext;//"<div>Склад: "+stategood(q, "" + r["zn"], "" + r["zn_z"], q_ord)+"</div>";
                if ("" + r["ens"] != "")
                    e.Item.Cells[9].Text += "<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r["ens"] + "/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>";
                if (webIO.CheckExistFile("../media/gimg/" + r["img"]))
                    e.Item.Cells[9].Text += "<img class='microimg' src='../media/gimg/" + r["img"] + "' id='imgeye" + r["goodId"] + "'/><img style='margin:1px;' id='eye" + r["goodId"] + "' class='eye' src='../simg/16/photo.png'/> ";

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
                if (d > 0)
                {

                    e.Item.Cells[1].Text = "СУММА со скидкой<br/>";
                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + " руб.<br/>";

                    e.Item.Cells[1].Text += "предоставлена скидка";
                    e.Item.Cells[2].Text += "" + cNum.cToDecimal(d * 100, 2) + "%"; ;

                }
                else
                {
                    e.Item.Cells[1].Text = "СУММА";
                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + "руб.";

                }


            }
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
            load_curentorder();
            Order ord = iam.CurOrder;
            ord.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Заявка от " + ord.RegDate.ToShortDateString() + " " + ord.RegDate.ToShortTimeString();
            ord.Name = txNameOrder.Text;
            ord.AuthorID = iam.ID;
            if (ord.IsNew) ord.Dg = ord.Subject.CodeDG;
            bool result = Order.Save(ord);

            lbOrder.Text = "" + ord.ID;
            lbRegDate.Text = ord.RegDate.ToShortDateString();
            if (result)
            {
                try
                {
                    EmailMessage msg = new EmailMessage(
                        iam.Email,
                        ord.Subject.EmailTAs,
                        (ord.Code == "") ? "Создана новая заявка " : "Скорректирована заявка ",
                        "<p>Клиент " + ord.Subject.Name + ((ord.Code == "") ? " оформил" : " изменил") + " Заявку №" + ord.ID + " на сумму " + ord.Summ + "руб.</p><p>Автор " + ord.Author.Name + ", " + ord.Author.Email + ", " + ord.Author.Phones + ".</p>" + ((ord.Descr == "") ? "<p>Комментария нет</p>" : "<p>Комментарий к заявке: <i>" + ord.Descr + "</i></p>")
                        );
                    msg.Send(CurrentCfg.EmailSupport);
                }
                catch { }

                eID = ord.ID;

                //Order.FinishWork(iam);
                lbMessage.Text = "<p>Заявка успешно сохранена.</p><p>Заявка будет автоматически передана в УЦСК и после этого станет возможно Подтвердить Заявку к покупке и распечатать Счет на оплату.</p><p>Вы всегда можете связаться с Вашим менеджером.</p>";

                rpOrders.Visible = rpArchive.Visible = false;
                CommentList1.Visible = true;
                CommentList1.DataObject = new sObject(eID, Order.TDB);
                CommentList1.CanAddComment = true;
                CommentList1.Event_AddedComment = new EventHandler(CommentList1_AddingComment);
            }
        }

        protected void btnNewOrder_Click(object sender, EventArgs e)
        {

            //Order.FinishWork(iam);
            eID = 0;
            iam.CurOrder = new Order(eID,iam,true);

            iam.CurOrder.SubjectID = iam.PresentedSubjectID;//cNum.cToInt(hdSubjectID.Value);
            iam.CurOrder.State = "N";
            //if (CurrentOrder.SubjectID == 0)
            //    CurrentOrder.SubjectID = iam.PresentedSubjectID;
            itemstack.Value = "";
            txDescr.Text = "";
            txNameOrder.Text = "";
            lbOrder.Text = "";
            lbRegDate.Text = "";
            TabControl1.SelectedTabIndex = 0;
            TabControl1.TabTitles[2] = "Текущая заявка";
            load_struct();
            CommentList1.Visible = false;
            lbStateOrder.Text = iam.CurOrder.StateDescr;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            //Order.FinishWork(iam);
            
            iam.CurOrder = new Order(eID,iam,true);
            txNameOrder.Text = iam.CurOrder.Name;
            txDescr.Text = iam.CurOrder.Descr;
            load_curentorder();

        }

        

        protected void rbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "listsize", rbPageSize.SelectedValue);
            dgList.PageSize = cNum.cToInt(rbPageSize.SelectedValue);
            txSearch_TextChanged(null, e);
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

        protected void btnSimSearch_Click(object sender, EventArgs e)
        {
            if (txSimSearch.Text.Trim() != "")
            {
                tkstack.Value = "";
   //             SELECTED_TKS = "" + tkstack.Value;
                txSearch.Text = txSimSearch.Text;
                txSimSearch.Text = "";
                TabControl1.SelectedTabIndex = 1;
                if (TabControl1.IsActiveTabpage(pnlList))
                {
                    load_brends();
                    load_TK();
                    load_xName();
                    txSearch_TextChanged(null, null);
                }
            }
        }

        protected void btnAcptFlt_Click(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "brend", dlBrends.SelectedValue);
            UserFilter.Save(iam.ID, "TK", dlTK.SelectedValue);
            UserFilter.Save(iam.ID, "Name", dlNames.SelectedValue);
            txSearch_TextChanged(null, null);
        }

        protected void lnkSelectREG_Click(object sender, EventArgs e)
        {
            if (iam.CF_SourceNomen != ((LinkButton)sender).CommandArgument)
                iam.CF_SelectedTKs = "";
            iam.CF_SourceNomen = ((LinkButton)sender).CommandArgument;
            UserFilter.Save(iam.ID, "reg", iam.CF_SourceNomen);
            
            set_list_regime();
            if (TabControl1.CurrentTabIndex == 0)
                load_struct();
            else if (TabControl1.CurrentTabIndex == 1)
            {
                load_TK(UserFilter.Load(iam.ID, "TK"));
                load_brends(UserFilter.Load(iam.ID, "brend"));
                load_xName(UserFilter.Load(iam.ID, "Name"));
                txSearch_TextChanged(sender, e);
            }
        }

        protected void lbtnChangeTN_Click(object sender, EventArgs e)
        {
            TabControl1.SelectedTabIndex = 0;
 //           SELECTED_TKS = "";
            iam.CF_SelectedTKs = "";
            tkstack.Value = "";
            load_struct();
        }

        protected void dlSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            iam.CF_SourceNomen = dlSource.SelectedValue;
            UserFilter.Save(iam.ID, "reg", iam.CF_SourceNomen);
            set_list_regime();
            load_struct();
            load_TK(UserFilter.Load(iam.ID, "TK"));
            load_brends(UserFilter.Load(iam.ID, "brend"));
            load_xName(UserFilter.Load(iam.ID, "Name"));

            txSearch_TextChanged(sender, e);
        }

        protected void chIncash_CheckedChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "chincash", (chIncash.Checked) ? "Y" : "N");


            if (TabControl1.CurrentTabIndex == 0)
                load_struct();
            else if (TabControl1.CurrentTabIndex == 1)
            {
                load_TK(UserFilter.Load(iam.ID, "TK"));
                load_brends(UserFilter.Load(iam.ID, "brend"));
                load_xName(UserFilter.Load(iam.ID, "Name"));
                txSearch_TextChanged(sender, e);
            }
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            loadDinFilter();
            dlBrends.ClearSelection();
            dlTK.ClearSelection();
            dlNames.ClearSelection();
            txSearch.Text = "";
            UserFilter.Save(iam.ID, "TK","");
            UserFilter.Save(iam.ID, "brend","");
            UserFilter.Save(iam.ID, "Name", "");
            load_TK("");
            load_brends("");
            load_xName("");
            txSearch_TextChanged(sender, e);
        }




    }
}