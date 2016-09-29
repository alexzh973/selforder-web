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

    public partial class orderdefault : p_p
    {
       

        private DataView dv;

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

        private Owner owner;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID < 100000)
            {
                Response.Redirect("~/default.aspx", true);
            }

            lbMessage.Text = (iam.PresentedSubjectID == 0) ? "Нельзя формировать заявку не выбрав клиента. <a href='../default.aspx'>Вернуться на главную страницу</a> " : "";



            TabControl1.InitTabContainer("tasklisttabs");
            TabControl1.AutoPostBack = false;
            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Классификатор", pnlStruct);
            TabControl1.AddTabpage("Справочник номенклатуры", pnlList);

            owner = new Owner(iam.OwnerID);
            TabControl1.AddTabpage("Текущая заявка","Текущая заявка", pnlOrder,"","cartbtn");
            

            if (!IsPostBack)
            {
                rbRegselect.SelectedValue = (UserFilter.Load(iam.ID,"regselectcat")=="multy")?"multy":"single";
                rbVidcat.SelectedValue = (UserFilter.Load(iam.ID,"vidcat")=="min")?"min":"max";

                ((mainpage)this.Master).SelectedMenu = "order";
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;

                if ((iam.IsSaller || iam.IsSuperAdmin || iam.IsTA || iam.IsAdmin) && iam.SubjectID != iam.PresentedSubjectID && iam.PresentedSubjectID > 0)
                    lbForSubj.Text = "для " + subj.Name;

                if (Request["act"] == "recl" && "" + Request["code"] != "")
                    iam.CF_SourceNomen = "all";
                else if (Request["act"] == "copy" && "" + Request["id"] != "")
                {

                }
                else
                {
                    iam.CF_SourceNomen = UserFilter.Load(iam.ID, "reg");
                    chIncash.Checked = (UserFilter.Load(iam.ID, "chincash") == "Y");
                    if (Request["tn"] != null)
                    {
                        xTN tn = xTN_db.Find(cNum.cToInt(Request["tn"]));
                        string tks = "";
                        foreach (xTK tk in xTK_db.GetList(tn.Title))
                        {
                            cStr.Add(ref tks, tk.Title.Replace(",", "@"));
                        }
                        iam.CF_SelectedTKs = tks;
                        TabControl1.SelectedTabIndex = 1;
                        lbSelTN.Text = tn.Title;
                    }
                    else if (Request["tk"] != null)
                    {
                        xTK tk = xTK_db.Find(cNum.cToInt(Request["tk"]));
                        iam.CF_SelectedTKs = tk.Title.Replace(",","@");
                        lbSelTN.Text = tk.xTN_Name + " / " + tk.Title;
                        TabControl1.SelectedTabIndex = 1;
                    }
                    else
                    {
                        lbSelTN.Text = "Все товарные направления";
                    }


                }

                set_list_regime();
                set_pagesize_regime();

                

                if (subj.ID > 0)
                {
                    DgInfo1.ShowInfo(subj.CodeDG, subj.OwnerID);
                }

                eID = cNum.cToInt(Request["id"]);
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
                    Order ord = iam.CurOrder;
                    _subj = null;
                    iam.PresentedSubjectID = ord.SubjectID;
                    
                    
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


            if (refresh.Value == "Y" && TabControl1.IsActiveTabpage(pnlOrder) || (!IsPostBack && Request["go"] == "ord"))
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
           if (TabControl1.IsActiveTabpage(pnlOrder) && (iam.CurOrder == null || iam.CurOrder.Items.Count==0))
               TabControl1.SelectedTabIndex = 0;
            
            if (TabControl1.IsActiveTabpage(pnlStruct) && struct_place.Text == "")
            {
                load_struct();
            }
            
            
            
            if ( iam.CurOrder != null && iam.CurOrder.Summ > 0)
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

        private string _seltk = "";

        private string SelectMutipleTK
        {
            get
            {
                if (_seltk == "") _seltk = SysSetting.GetValue("SelectMutipleTK");
                return _seltk;
            }
        }
        private void load_struct()
        {
            if (rbRegselect.SelectedValue == "single")
                load_struct_single();
            else
                load_struct_multi();
            
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
            lbStateOrder.Text = (ord.ID > 0) ? ord.StateDescr : "Заявка еще не сохранена";
            lbCode.Text = ord.Code;

            btnClear.Visible = lbtnSave.Visible = !OrderReadOnly;

            if (ord.ID == 0) btnClear.Visible = false;
            //set_state_button(ord.State);
            if (ord.ID == 0 || ord.Changed || ord.TypeOrder == "Q")
            {
                //btnClear.Visible = false;
                //lbtnSave.Visible = false;
                lbtnNextStady.Visible = false;
            }
            else
            {
                set_state_button(ord.State);
            }



            decimal smb = ord.SummBase;
            decimal sm = ord.Summ;

            foreach (OrderItem item in ord.Items)
            {
               
                decimal q = (Request["q_" + item.GoodId] != null) ? cNum.cToDecimal(Request["q_" + item.GoodId]) : item.Qty;

                if (q <= 0) q = 1;
                if (item.Qty != q )
                {
                    ord.ChangeItem(item, q, item.Descr);
                    item.Mark = "";
                }
            }
            
            dgOrder.DataSource = ord.Items;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();

            DgInfo1.ShowInfo(ord.Dg, ord.OwnerID);
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
                    lbMessSource.Text = "Список товарных категорий ограничен выбором режима \"Распродажа\"";
                    lbMessSource.ToolTip = "Выше и чуть правее можно сменить Источник списка номенклатуры";
                    break;
                case "my":
                    lnkMyFavor.CssClass = active;
                    lnkSpecial.CssClass = pass;
                    lnkJust.CssClass = pass;
                    lbMessSource.Text = "Список товарных категорий ограничен из \"Истории заказов клиента\"";
                    lbMessSource.ToolTip = "Выше и чуть правее можно сменить Источник списка номенклатуры";
                    break;
                case "all":
                default:
                    lnkMyFavor.CssClass = pass;
                    lnkSpecial.CssClass = pass;
                    lnkJust.CssClass = active;
                    lbMessSource.Text = "";
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
            else if (state.ToUpper() == "E")
            {
                lbtnNextStady.Visible = false;
                //lbtnNextStady.Visible = true;
                //lbtnNextStady.ToolTip = "Подтвердить к покупке из свободного остатка";
                //lbtnNextStady.CommandArgument = "E";
            }
            else if (state.ToUpper() == "S")
            {
                lbtnNextStady.Visible = true;
                lbtnNextStady.ToolTip = "Подтвердить к покупке";
                lbtnNextStady.CommandArgument = "M";
            }
            else if (state.ToUpper() == "R")
            {
                lbtnNextStady.Visible = true;
                lbtnNextStady.ToolTip = "Готов получить товар (тот который в наличии)";
                lbtnNextStady.CommandArgument = "E";
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


            if (iam.SubjectID != iam.CurOrder.SubjectID && !(iam.IsSaller && iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)))
                lbtnCancel.Visible = lbtnNextStady.Visible = false;
        }



        //private void set_state_button(string state)
        //{

        //    if (state.ToUpper() == "U")
        //    {
        //        lbtnNextStady.Visible = true;
        //        lbtnNextStady.ToolTip = "Подтвердить к заказу";
        //        lbtnNextStady.CommandArgument = "A";
        //    }
        //    else if (state.ToUpper() == "E")
        //    {
        //        lbtnNextStady.Visible = true;
        //        lbtnNextStady.ToolTip = "Подтвердить к покупке из свободного остатка";
        //        lbtnNextStady.CommandArgument = "E";
        //    }
        //    else if (state.ToUpper() == "S")
        //    {
        //        lbtnNextStady.Visible = true;
        //        lbtnNextStady.ToolTip = "Подтвердить к покупке";
        //        lbtnNextStady.CommandArgument = "M";
        //    }
        //    else
        //    {
        //        lbtnNextStady.Visible = false;
        //        lbtnNextStady.CommandArgument = "";
        //    }
        //    if (cStr.Exist(state.ToUpper(), new string[] { "N", "U", "A", "Z", "S" }))
        //    {
        //        lbtnCancel.Visible = true;
        //        lbtnCancel.CommandArgument = "D";
        //        lbtnCancel.ToolTip = "Отменить заявку";
        //        lbtnCancel.Text = "<img src=\"../simg/icodel.png\" title=\"Отменить заявку\"/>";
        //    }
        //    else if (state.ToUpper() == "D")
        //    {
        //        lbtnCancel.Visible = true;
        //        lbtnCancel.CommandArgument = "N";
        //        lbtnCancel.ToolTip = "Вернуть к жизни";
        //        lbtnCancel.Text = "<img src=\"../simg/icorepair.png\" title=\"Отменить заявку\"/>";
        //    }
        //    else
        //    {
        //        lbtnCancel.Visible = false;
        //        lbtnCancel.CommandArgument = "";
        //        lbtnCancel.ToolTip = "...";
        //    }

        //    //// пока оставим только отмену:
        //    //lbtnNextStady.Visible = false;

        //    if (iam.SubjectID != iam.CurOrder.SubjectID && !(iam.IsSaller && iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)))
        //        lbtnCancel.Visible = lbtnNextStady.Visible = false;
        //}



        private void load_struct_single()
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
                    regwhere = " good.id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + ")";
                    break;
            }
            List<xTN> listTN = xTN_db.GetList(regwhere);
            StringBuilder sb = new StringBuilder();
            if (listTN.Count == 0)
                sb.Append("<br/><br/><br/><p>У Вас, по всей видимости еще не было заказов, поэтому выберите другой <span class='bold'>Источник списка номенклатуры</span></p><br/><br/>");


            foreach (xTN r_tn in listTN)
            {
                sb.Append("<div class='blocktn' >");  // style=\"background-image: url('../media/grp" + r_tn.ID + ".png');\"
                sb.Append("<a href='orderdefault.aspx?tn=" + r_tn.ID + "' class='tnlink' title='Можно выбрать всё направление'  >" + r_tn.Title.ToUpper() + "</a>");
               if(rbVidcat.SelectedValue=="min") 
                   sb.Append(" <a href='javascript:return 0' onclick=\"javascript:$('.blocktk').hide();$('#tkdiv" + r_tn.ID + "').show('fast');\" class='small'>развернуть ...</a> <br/>");
               sb.Append("<div class='blocktk' id='tkdiv" + r_tn.ID + "' " + ((rbVidcat.SelectedValue == "min") ? "style='display:none;'" : "") + "><div class='floatLeft-90'><ul>");
                foreach (xTK r_tk in xTK_db.GetList(r_tn.Title, regwhere))
                {
                    string tk = r_tk.Title;
                    tk = tk.Replace(",", "@");

                    bool ch = (iam.CF_SelectedTKs.ToLower().IndexOf("" + tk.ToLower()) > -1);
                    sb.Append(" <li><a id='tk" + r_tk.ID + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" data='" + r_tk.Title.ToLower() + "' href='orderdefault.aspx?tk=" + r_tk.ID + "' " + ((ch) ? "class='tklink-active'" : "class='tklink'") + " >" + r_tk.Title + "</a></li>");
                    i += 1;
                }
                sb.Append("</ul></div><div class='floatRight-10'></div><div class='clearBoth'></div></div>");// block_TK
                //                sb.Append("</ul></div><div class='floatRight-10'><img class='arrowtolist' src='../simg/32/arrow16x32.png' style='cursor:pointer' title='перейти в справочник номенклатуры' alt='>>' onclick=\"setActPunct(3, 1,'ctl00_ContentPlaceHolder1_TabControl1')\"/></div><div class='clearBoth'></div></div>");// block_TK
                sb.Append("</div>");// block_TN
            }
            sb.Append("<div style='clear:both;'></div>");
            struct_place.Text = sb.ToString();
        }

        private void load_struct_multi()
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
                    regwhere = " good.id in (select goodid from OWNG where OwnerID=" + iam.OwnerID + ")";
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


        //private bool checkPicture(GoodGrp grp)
        //{
        //    bool ret = false;

        //    string filename = "grp" + grp.ID + ".png";
        //    if (!webIO.CheckExistFile(@"../media/" + filename))
        //    {
        //        FileStream stream = new FileStream(webIO.GetAbsolutePath(@"../media/" + filename), FileMode.OpenOrCreate);
        //        BinaryWriter writer = new BinaryWriter(stream);
        //        try
        //        {

        //            DataTable dt = db.GetDbTable("select img from imgtntk where id=" + grp.ID);
        //            byte[] buf = (byte[])dt.Rows[0][0];
        //            writer.Write(buf);
        //            ret = true;
        //        }
        //        catch
        //        {
        //            ret = false;
        //        }
        //        finally
        //        {
        //            writer.Close();
        //            stream.Close();
        //        }
        //    }
        //    else
        //    {
        //        ret = true;
        //    }
        //    return ret;
        //}

        //private void load_exist_orders()
        //{
        //    return;
        //    string sql;
        //    if (iam.PresentedSubjectID > 0)
        //        sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where isnull(ord.state,'') not in ('D','X') and ord.SubjectID=" + iam.PresentedSubjectID + "";
        //    else
        //        sql = "select ord.id, ord.Name, ord.RegDate, ord.State, subj.Name as SubjectName, ord.SummOrder from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where subj.EmailTAs like '%" + iam.Email + "%' and isnull(ord.state,'') not in ('D','X')";

        //    DataTable dt = db.GetDbTable(sql);
        //    DataTable dto = db.GetDbTable("select 0 as id, '' as Name, '' as RegDate, '' as State, '' as SubjectName, '' as SummOrder, '' as linkchange");
        //    dto.Rows.Clear();
        //    foreach (DataRow r in dt.Rows)
        //    {
        //        DataRow nr = dto.NewRow();
        //        nr["id"] = r["id"];
        //        nr["Name"] = r["Name"];

        //        nr["RegDate"] = cDate.cToDate(r["RegDate"]).ToShortDateString();
        //        nr["SubjectName"] = r["SubjectName"];
        //        nr["State"] = Order.get_stateorder_descr("" + r["State"]);
        //        nr["SummOrder"] = cNum.cToDecimal(r["SummOrder"], 2);
        //        //if (iam.SubjectID > 0)
        //        nr["linkchange"] = "<a href='goodlist.aspx?id=" + r["id"] + "&act=edit'>изменить</a>";
        //        dto.Rows.Add(nr);
        //    }
        //    rpOrders.DataSource = dto;
        //    rpOrders.DataBind();
        //}



        private bool ItsMySubj(int subjectId)
        {
            return iam.ItsMySubj.Contains(subjectId);
        }

        bool OrderReadOnly
        {
            get
            {

                return  !(   cStr.Exist(iam.CurOrder.State.ToUpper(), new string[] { "", "U", "N" }) && (iam.CurOrder.SubjectID == iam.PresentedSubjectID || ItsMySubj(iam.CurOrder.SubjectID) || iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)));
            }
        }




        protected void load_brends(string default_value = "")
        {

            string where = "";

            if (dlTK.SelectedValue != "")
                where += " and xTK='" + dlTK.SelectedValue + "'";

            string seltk = get_selected_tks();

            if (seltk != "")
                where = " and xTK in (" + seltk + ")";

            string whereqty = (chIncash.Checked) ? " and qty>0" : "";

            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select brend, count(goodId) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " and goodId in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((where != "") ? where : "") + " group by brend");
                    break;
                case "spec":
                    dt = db.GetDbTable("select brend, count(goodId) as qty from vGOOD" + iam.OwnerID + " where 1=1  " + whereqty + " and zn_z='NL' " + ((where != "") ? where : "") + " group by brend");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select brend, count(id) as qty from vGOOD" + iam.OwnerID + " where 1=1  " + whereqty + " " + ((where != "") ? where : "") + " group by brend ");
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
            string whereqty = (chIncash.Checked) ? " and qty>0" : "";
            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select xTK, count(goodId) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " and goodId in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
                    break;
                case "spec":
                    dt = db.GetDbTable("select xTK, count(goodId) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " and zn_z='NL' " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select xTK, count(goodId) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xTK");
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
            string whereqty = (chIncash.Checked) ? " and qty>0" : "";

            DataTable dt = new DataTable();
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select xName, count(id) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " and goodId in (select goodid from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + ")) " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
                    break;
                case "spec":
                    dt = db.GetDbTable("select xName, count(id) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " and zn_z='NL' " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select xName, count(id) as qty from vGOOD" + iam.OwnerID + " where 1=1 " + whereqty + " " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + " group by xName");
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

            string RowFilter = "";
            string where_conditions = "1=1";
            where_conditions = get_where_condition();

            string typePrice = "pr_b";
            if (subj != null)
                typePrice = subj.PriceType;

            switch (iam.CF_SourceNomen)
            {
                case "my":
                    //                    dt = db.GetDbTable("select GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend,good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D' and owng.goodcode in (select goodcode from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + "))" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    dt = db.GetDbTable("select *, isnull(" + typePrice + ",isnull(pr_b,0)) as price from vGood" + iam.OwnerID + " where goodcode in (select goodcode from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + "))" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "spec":
                    //dt = db.GetDbTable("select GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend, good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D' where owng.zn_z='NL' and isnull(owng.qty,0)>0" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    dt = db.GetDbTable("select *, isnull(" + typePrice + ",isnull(pr_b,0)) as price from vGood" + iam.OwnerID + " where zn_z='NL' and isnull(qty,0)>0" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "all":
                default:
                    //                    dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr, GOOD.Brend, good.img, owng.zn, owng.zt, owng.zn_z, owng.GoodCode,owng.Qty,isnull(owng.pr_b,0) as pr_b, isnull(owng." + typePrice + ",isnull(owng.pr_b,0)) as price, isnull(owng.pr_spr,0) as pr_spr from OWNG inner join GOOD on owng.GoodId=good.ID and owng.OwnerId=" + iam.OwnerID + " and owng.State<>'D'" + ((where_conditions == "") ? "" : " where " + where_conditions) + "");
                    dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " *, isnull(" + typePrice + ",isnull(pr_b,0)) as price from vGood" + iam.OwnerID + " " + ((where_conditions == "") ? "" : " where " + where_conditions) + "");
                    break;
            }


            Cache.Insert("MyDT", dt, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);

            string sort = (dv == null || dv.Sort == "") ? UserFilter.Load(iam.ID, "sortgood", "zn, name") : dv.Sort;

            dv = dt.DefaultView;

            dv.Sort = sort;
            dgList.DataSource = dv;
            dgList.CurrentPageIndex = 0; lbSelTN.Visible = true;


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
            string wr = "";
            if (txSearch.Text.Length > 0)
            {

                foreach (string word in txSearch.Text.Trim().Replace(":", @"/").Replace("  ", " ").Split(' '))
                {
                    //if (word.Length>=4)
                    wr += ((wr != "") ? " and " : "") + "name like '%" + word + "%'";
                    //else
                    //    wr += ((wr != "") ? " and " : "") + "name like '% " + word + "%'";
                }
                if (wr != "") wr = "(" + wr + ")";

            }
            if (txSearch.Text.Length >= 4)
            {
                where_conditions += string.Format(" and (" + wr + " or article like '%{0}%' or goodcode like '%{0}%')", searchW);
            }
            else if (txSearch.Text.Length > 0)
            {
                where_conditions += string.Format(" and (" + wr + " or name like '% {0}%' or article like '%{0}%' or goodcode like '%{0}%')", searchW);
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
                dv = dt.DefaultView;
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
            txSearch_TextChanged(sender, e);
        }

        string getQtyInCurrentOrder(int goodId)
        {
            if (iam.CurOrder == null) return "";
            OrderItem oi = iam.CurOrder.FindItem(goodId);
            return (oi != null) ? oi.Qty.ToString() : "";
        }


        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;

                string id = "" + r["goodId"];
                string zn = ("" + r["zn"]).ToUpper();
                string zn_z = ("" + r["zn_z"]).ToUpper();
                string zt = ("" + r["zt"]).ToUpper();

                string qs = getQtyInCurrentOrder(cNum.cToInt(r["goodId"]));
                e.Item.Attributes.Add("id", "tr_" + r["goodId"]);
                if (qs != "") e.Item.CssClass = "selitem";
                if (iam.CurOrder==null || !OrderReadOnly)
                {
                    e.Item.Cells[1].Text = "<input id='ch_" + id + "' type='checkbox' onclick=\"checkChange('" + id + "');shw(this.id,'" + id + "')\" " + ((qs != "") ? " value='" + id + "' checked='checked'" : "") + ")/>";
                    e.Item.Cells[1].Text += "&nbsp;<input class='inputqty' title='укажите нужное количество' id='qch_" + id + "' onchange=\"checkNumeric(this.id,1);changeQty('" + id + "');\"  maxlength='4' style='width:34px' type='text' value='" + qs + "' />";
                    e.Item.Cells[2].Text = "<img class='plusminus' src='../simg/16/btn_up.gif' alt='+' title='+1' onclick=\"plusq('" + id + "')\"/><br/><img class='plusminus' src='../simg/16/btn_dwn.gif' alt='-' title='-1' onclick=\"minusq('" + id + "')\"/>";
//                    e.Item.Cells[2].Text = "<table class='plusminus'><tr><td><img src='../simg/16/btn_up.gif' alt='+' onclick=\"plusq('" + id + "')\"/></td></tr><tr><td><img src='../simg/16/btn_dwn.gif' alt='-' onclick=\"minusq('" + id + "')\"/></td></tr></table>";
                }
                else
                    e.Item.Cells[1].ToolTip = "В текущую заявку нельзя добавлять товары. Воспользуйтесь кнопкой 'Новая заявка'";

                if (zn == "Z")
                {
                    e.Item.Cells[4].Text = "<span class='red' title='Заказной товар " + zn + "," + zt + "," + zn_z + "'>*</span> ";
                }
                else
                {
                    e.Item.Cells[4].Text = "<span title='Складской товар " + zn + "," + zt + "," + zn_z + "'>&nbsp;</span>";
                }

                e.Item.Cells[3].Text = (txSearch.Text.Length > 0) ? webInfo.select_search(txSearch.Text, "" + r["Goodcode"]) : "" + r["Goodcode"];
                e.Item.Cells[4].Text += (txSearch.Text.Length > 0) ? webInfo.select_search(txSearch.Text, "" + r["Name"]) : "" + r["Name"];
                e.Item.Cells[4].Text += " "+ get_angood(r);
                if ("" + r["ens"] != "")
                    e.Item.Cells[7].Text = String.Format("<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/{0}/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>", r["ens"]);
                if (webIO.CheckExistFile("../media/gimg/" + r["img"]))
                {
                    e.Item.Cells[7].Text += "<img class='microimg' src='../media/gimg/" + r["img"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                }

                decimal q = cNum.cToDecimal(r["qty"]);
                if (iam.SubjectID == 0 || owner.ShowIncash)
                {
                    e.Item.Cells[5].Text = "" + q;
                }
                else
                {
                    e.Item.Cells[5].Text = stategood(cNum.cToInt(id), q, zn, zn_z);
                }

                decimal c = cNum.cToDecimal(r["price"], 2);
                decimal c_b = cNum.cToDecimal(r["pr_b"], 2);

                if ((zn == "S" || (zn + zt) == "Z1" || zn_z == "NL") && (c_b > 0 || c > 0))
                {
                    e.Item.Cells[6].Text = "" + cNum.cToDecimal(c, 2);
                    e.Item.Cells[6].ToolTip = (c < c_b) ? "Это Ваша цена, а базовая цена " + c_b + "р." : "";
                }
                else if (q > 0 && (zn == "D" || zn == "Q"))
                {
                    e.Item.Cells[6].Text = "<span class='red' title='Товар выводится из ассортимента, это остаток, лучше свяжитесь с менеджером'>*</span>" + cNum.cToDecimal(c, 2);
                }
                else
                {
                    e.Item.Cells[6].Text = "<span class='red bold' title='Товар заказной, цену нужно получить/уточнить у своего менеджера'><img src='../simg/32/ph.png' style='cursor:pointer; width:16px;' onclick=\"openflywin('../common/gimail.aspx?id=" + id + "', 400, 300, 'Запрос информации о товаре')\" title='по запросу' alt='?' " + get_emailmess(id) + "/></span>";
                }

            }
            else if (e.Item.ItemType == ListItemType.Header)
            {
                e.Item.Cells[4].CssClass = "sortable";
                e.Item.Cells[6].CssClass = "sortable";
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
                e.Item.Cells[4].Text = "<input type='text' id='pr_"+r.GoodId+"' style='border:0; text-align:right; width:80px' readonly value='"+r.Price+"'/>";
                e.Item.Cells[5].Text = "<input " + ((ordReadOnly) ? "readonly" : "") + " class='center qtyfield inputqty' maxlength='5' name='q_" + r.GoodId + "' id='q_" + r.GoodId + "' type='text' onchange=\"checkNumeric(this.id,1);recount('" + r.GoodId + "');\" value='" + cNum.cToDecimal(r.Qty, 1) + "' />";

                e.Item.Cells[6].Text = "<img class='plusminus' src='../simg/16/btn_up.gif' alt='+' title='+1' onclick=\"plusqo('" + r.GoodId + "')\"/><br/><img class='plusminus' src='../simg/16/btn_dwn.gif' alt='-' title='-1' onclick=\"minusqo('" + r.GoodId + "')\"/>";



                e.Item.Cells[8].Attributes.Add("id", "sm_" + r.GoodId);

                e.Item.Cells[9].Text = "<input maxlength='150' style='width:100px;' name='ds_" + r.GoodId + "' id='ds_" + r.GoodId + "' type='text' " + ((ordReadOnly) ? "readonly" : "") + " class='descritem' value='" + r.Descr + "' onchange=\"chngitemdescr('" + r.GoodId + "')\"/>";
                string osttext = wstcp.order.detailfly.get_ost_info(iam.CurOrder.State, r.Zn, r.CurIncash, r.Qty,r.Booking, r.Realized, r.WDate, r.Comment, r.SrokPost);

                e.Item.Cells[10].Text = osttext;
                if (r.ens != "")
                    e.Item.Cells[10].Text += "<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r.ens + "/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>";
                if (webIO.CheckExistFile("../media/gimg/" + r.img))
                    e.Item.Cells[10].Text += "<img class='microimg' src='../media/gimg/" + r.img + "' id='imgeye" + r.GoodId + "'/><img style='margin:1px;' id='eye" + r.GoodId + "' class='eye' src='../simg/16/photo.png'/> ";

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
            ret += (( r.an_k>0) ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good="+r.GoodCode+"&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
                ((r.an_s>0) ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
                ((r.an_a>0) ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=a',500,500,'Аналоги');>а</a>" : "");
            return ret;
        }

        private string get_angood(DataRowView r)
        {
            string ret = "";
            ret += (("" + r["an_k"] != "0") ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good="+r["goodcode"]+"&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
                (("" + r["an_s"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
                (("" + r["an_a"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=a',500,500,'Аналоги');>а</a>" : "");
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
            ord.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Заявка от " + ord.RegDate.ToShortDateString() + " " + ord.RegDate.ToShortTimeString();
            ord.Name = txNameOrder.Text;
            ord.AuthorID = iam.ID;
            bool isNew = ord.IsNew;
            if (isNew) ord.Dg = ord.Subject.CodeDG;
            bool result = Order.Save(ord);
            bool existZ = ord.Items.FindAll(x => x.Zn.ToUpper() != "S").Count > 0;
            if (result && !existZ)
                db.ExecuteCmd("update ord set invoice='N' where id=" + ord.ID);
            lbOrder.Text = "" + ord.ID;
            lbRegDate.Text = ord.RegDate.ToShortDateString();
            if (result)
            {
                try
                {
                    string emailTo = (IsDev) ? "alexzh@santur.ru" : ord.Subject.EmailTAs;
                    EmailMessage msg = new EmailMessage(
                        iam.Email,
                        emailTo,
                        (ord.Code == "") ? "Создана новая заявка " : "Скорректирована заявка ",
                        "<p>Клиент " + ord.Subject.Name + ((ord.Code == "") ? " оформил" : " изменил") + " Заявку №" + ord.ID + " на сумму " + ord.Summ + "руб.</p><p>Автор " + ord.Author.Name + ", " + ord.Author.Email + ", " + ord.Author.Phones + ".</p>" + ((ord.Descr == "") ? "<p>Комментария нет</p>" : "<p>Комментарий к заявке: <i>" + ord.Descr + "</i></p>")
                        );
                    msg.Send(CurrentCfg.EmailSupport);
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
                        btnGetOrder.CommandArgument = ""+iam.CurOrder.ID;
                    } 
                    else
                    {
                        lbMsgResult.Text += "<p>Ваш баланс не позволяет получить товар сегодня. Дождитесь выставленного счета на оплату.</p>";
                        pnlWishDate.Visible = false;
                    }

                    
                }
                TabControl1.SetTabTitle(2, "Заявка на сумму " + iam.CurOrder .Summ+ "р.");
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
            //if (iam.CurOrder.ID>0)
                iam.CurOrder = new Order(iam.CurOrder.ID,iam);
//            iam.CurOrder = new Order(eID, iam, true);
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

        protected void btnSimSearch_Click(object sender, EventArgs e)
        {
            if (txSimSearch.Text.Trim() != "")
            {
                tkstack.Value = "";
                //             SELECTED_TKS = "" + tkstack.Value;
                iam.CF_SelectedTKs = "";
                iam.CF_SourceNomen = "";
                chIncash.Checked = false;

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
            lbSelTN.Text = "";
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
            dlBrends.ClearSelection();
            dlTK.ClearSelection();
            dlNames.ClearSelection();
            txSearch.Text = "";
            UserFilter.Save(iam.ID, "TK", "");
            UserFilter.Save(iam.ID, "brend", "");
            UserFilter.Save(iam.ID, "Name", "");
            load_TK("");
            load_brends("");
            load_xName("");
            txSearch_TextChanged(sender, e);
        }

        protected void txDescr_PreRender(object sender, EventArgs e)
        {
            txDescr.Attributes.Add("onchange", "setdescr()");
        }

        protected void txNameOrder_PreRender(object sender, EventArgs e)
        {
            txNameOrder.Attributes.Add("onchange", "setname()");
        }

        protected void dgList_SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            if (Cache["MyDT"] != null)
            {
                DataTable dt = (DataTable)Cache["MyDT"];
                dv = dt.DefaultView;
                dv.Sort = (dv.Sort == e.SortExpression) ? e.SortExpression + " desc" : e.SortExpression;
                UserFilter.Save(iam.ID, "sortgood", dv.Sort);
                dgList.DataSource = dv;
                dgList.DataBind();
            }
            else
            {
                txSearch_TextChanged(null, null);
            }



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

            string emails = (IsDev)?"alexzh@santur.ru":ord.Subject.EmailTAs;

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

        protected void rbRegselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "regselectcat", rbRegselect.SelectedValue);
            load_struct();
        }

        protected void lbtnClearsearchcat_Click(object sender, EventArgs e)
        {
            iam.CF_SelectedTKs = "";
            load_struct();
        }

        protected void rbVidcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "vidcat", rbVidcat.SelectedValue);
            load_struct();
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