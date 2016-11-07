using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;
using selforderlib;
using ensoCom;
using System.Text;

namespace wstcp
{

    public partial class _orderdefault : p_p
    {
       

        private DataView dv;

        //private Subject _subj;

        //private Subject subj
        //{
        //    get
        //    {
        //        if (_subj == null)
        //            _subj = new Subject(iam.PresentedSubjectID, iam);
        //        return _subj;
        //    }
        //}

        //private Owner owner;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID < 100000)
            {
                Response.Redirect("~/default.aspx", true);
            }
            if (!IsPostBack)
            {
                if (""+Request["id"]!="")
                    Response.Redirect("cart.aspx?id="+Request["id"],true);


                ((mainpage) this.Master).SelectedMenu = "order";
                ((mainpage) this.Master).VisibleLeftPanel = false;
                ((mainpage) this.Master).VisibleRightPanel = false;
                if (iam.PresentedSubjectID == 0)
                {
                    lbMessage.Text = "Нельзя формировать заявку не выбрав клиента. <a href='../default.aspx'>Вернуться на главную страницу</a> ";
                    return;
                } else
                {
                    lbMessage.Text = "";
                }
                
                if ((iam.IsSaller || iam.IsSuperAdmin || iam.IsTA || iam.IsAdmin) && iam.SubjectID != iam.PresentedSubjectID && iam.PresentedSubjectID > 0)
                    lbForSubj.Text = "для " + iam.PresentedSubject.Name;
            }

            
            
            TabControl1.InitTabContainer("goodlisttabs");
            TabControl1.AutoPostBack = false;
            TabControl1.AutoPostBack = true;
            TabControl1.AddTabpage("Классификатор", pnlStruct);
            TabControl1.AddTabpage("Справочник номенклатуры", pnlList);
            TabControl1.AddTabpage("Текущая заявка","Текущая заявка", pnlOrder,"","cartbtn");
            

            if (!IsPostBack)
            { 
                if (iam.CurOrder == null)
                {
                    iam.CurOrder = new Order(0, iam);
                }

                rbRegselect.SelectedValue = (UserFilter.Load(iam.ID,"regselectcat")=="multy")?"multy":"single";
                rbVidcat.SelectedValue = (UserFilter.Load(iam.ID,"vidcat")=="min")?"min":"max";


                if (TabControl1.IsActiveTabpage(pnlOrder))
                    TabControl1.SelectedTabIndex = 0;

                if (Request["act"] == "recl" && "" + Request["code"] != "")
                {
                    iam.CF_SourceNomen = "all";
                    txSearch.Text = "" + Request["code"];
                    TabControl1.SelectedTabIndex = 1;
                    lbSelTN.Text = "Все товарные направления";
                }
                else
                {
                    iam.CF_SourceNomen = UserFilter.Load(iam.ID, "reg");
                    chIncash.Checked = (UserFilter.Load(iam.ID, "chincash") == "Y");
                    if (Request["tk"] != null)
                    {
                        xTK tk = xTK_db.Find(cNum.cToInt(Request["tk"]));
                        iam.CF_SelectedTKs = tk.Title.Replace(",", "@");
                        lbSelTN.Text = tk.xTN_Name + " / " + tk.Title;
                        TabControl1.SelectedTabIndex = 1;
                    } 
                    else if (Request["tn"] != null)
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
                    else
                    {
                        lbSelTN.Text = "Все товарные направления";
                    }


                }

                set_list_regime();
                set_pagesize_regime();
            }

            
            if (TabControl1.ChangedTabpage || !IsPostBack)
            {
                if (TabControl1.IsActiveTabpage(pnlOrder))
                {
                    Response.Redirect("cart.aspx");
                }
                else if (TabControl1.IsActiveTabpage(pnlList))
                {
                    load_TK(UserFilter.Load(iam.ID, "TK"));
                    load_brends(UserFilter.Load(iam.ID, "brend"));
                    load_xName(UserFilter.Load(iam.ID, "Name"));
                    txSearch_TextChanged(null, null);
                }
                else if (struct_place.Text == "")
                {
                    load_struct();
                } 
                
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

        

        
        private void load_struct()
        {
            //return;
            if (rbRegselect.SelectedValue == "single")
                load_struct_single();
            else
                load_struct_multi();
            
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
            const string active = "sourcelist-active";
            const string pass = "sourcelist-passive";
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

                    bool ch = (iam.CF_SelectedTKs.ToLower().IndexOf(tk.ToLower()) > -1);
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
            dlBrends.Items.Add(new ListItem("--- Все ---", ""));
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
            dlTK.Items.Add(new ListItem("-- все категории --", ""));
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

dlNames.Items.Add(new ListItem("-- все ---", ""));
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
            //if (subj != null)
                typePrice = iam.PresentedSubject.PriceType;
            string wdate = cDate.Date2Sql(cDate.TodayD);
            int ownerId = iam.PresentedSubject.OwnerID;
            switch (iam.CF_SourceNomen)
            {
                case "my":
                    dt = db.GetDbTable("select id, goodid,zn, zn_z, zt,an_a, an_s,an_k, salekrat, ens,qty,img,pr_b,name,goodcode,brend, isnull(" + typePrice + ",isnull(pr_b,0)) as price, isnull((select price from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accprice,isnull((select max(accid) from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accId from vGood" + ownerId + " as t1 where goodcode in (select goodcode from ORDI where orderid in (select id from ORD where subjectid=" + iam.PresentedSubjectID + "))" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "spec":
                    dt = db.GetDbTable("select id, goodid,zn, zn_z, zt,an_a, an_s,an_k, salekrat, ens, qty, img,pr_b,name,goodcode,brend, isnull(" + typePrice + ",isnull(pr_b,0)) as price,isnull((select price from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accprice,isnull((select max(accid) from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accId from vGood" + ownerId + " as t1 where zn_z='NL' and isnull(qty,0)>0" + ((where_conditions == "") ? "" : " and " + where_conditions) + "");
                    break;
                case "all":
                default:
                    dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " id, goodid,zn, zn_z, zt,an_a, an_s,an_k, salekrat, ens, qty, img,pr_b,name,goodcode,brend, isnull(" + typePrice + ",isnull(pr_b,0)) as price,isnull((select price from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accprice,isnull((select max(accid) from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<=" + wdate + " and finishdate>=" + wdate + " and t3.ownerID=" + ownerId + " and t2.goodid=t1.goodid ),0) as accId from vGood" + ownerId + " as t1 " + ((where_conditions == "") ? "" : " where " + where_conditions) + "");
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
                where_conditions += " and brend='" + dlBrends.SelectedValue + "'";
            string seltks = get_selected_tks();
            if (seltks != "")
                where_conditions += " and xTK in (" + seltks + ")";
            if (dlTK.SelectedValue != "")
                where_conditions += " and xTK like '" + dlTK.SelectedValue + "%'";
            if (dlNames.SelectedValue != "")
                where_conditions += " and xName ='" + dlNames.SelectedValue + "'";
            string searchW = txSearch.Text.Trim().Replace(":", @"/").Replace("  ", " ").Replace(" ", "%");
            string wr = "";
            if (txSearch.Text.Length > 0)
            {

                foreach (string word in txSearch.Text.Trim().Replace(":", @"/").Replace("  ", " ").Split(' '))
                {
                    wr += ((wr != "") ? " and " : "") + "name like '%" + word + "%'";
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
                var dt = (DataTable)Cache["MyDT"];
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
                var r = (DataRowView)e.Item.DataItem;

                string id = "" + r["goodId"];
                string zn = ("" + r["zn"]).ToUpper();
                string zn_z = ("" + r["zn_z"]).ToUpper();
                string zt = ("" + r["zt"]).ToUpper();
                int saleKrat = (cNum.cToInt(r["salekrat"])<1) ? 1 : cNum.cToInt(r["salekrat"]);
                string qs = getQtyInCurrentOrder(cNum.cToInt(r["goodId"]));
                e.Item.Attributes.Add("id", "tr_" + r["goodId"]);
                if (qs != "") e.Item.CssClass = "selitem";
                if (iam.CurOrder==null || !OrderReadOnly)
                {
                    string tooltipP = (saleKrat > 1) ? " выписывать только кратно " + saleKrat : "+1";
                    string tooltipM = (saleKrat > 1) ? " выписывать только кратно " + saleKrat : "-1";
                    e.Item.Cells[1].Text = "<input id='ch_" + id + "' type='checkbox' onclick=\"checkChange('" + id + "','"+saleKrat+"');shw(this.id,'" + id + "')\" " + ((qs != "") ? " value='" + id + "' checked='checked'" : "") + ")/>";
                    e.Item.Cells[1].Text += "&nbsp;<input class='inputqty' title='укажите нужное количество " + ((saleKrat>1) ? "кратно " + saleKrat : "") + "' id='qch_" + id + "' onchange=\"checkNumeric(this.id,1);changeQty('" + id + "','" + saleKrat + "');\"  maxlength='4' style='width:34px' type='text' value='" + qs + "' />";
                    e.Item.Cells[2].Text = "<img class='plusminus' src='../simg/plus.png' alt='+' title='" + tooltipP + "' onclick=\"plusq('" + id + "','" + saleKrat + "')\"/><br/><img class='plusminus' src='../simg/minus.png' alt='-' title='" + tooltipM + "' onclick=\"minusq('" + id + "','" + saleKrat + "')\"/>";
//                    e.Item.Cells[2].Text = "<table class='plusminus'><tr><td><img src='../simg/16/btn_up.gif' alt='+' onclick=\"plusq('" + id + "')\"/></td></tr><tr><td><img src='../simg/16/btn_dwn.gif' alt='-' onclick=\"minusq('" + id + "')\"/></td></tr></table>";
                }
                else
                    e.Item.Cells[1].ToolTip = "В текущую заявку нельзя добавлять товары. Воспользуйтесь кнопкой 'Новая заявка'";

                if (zn != "S")
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
                if ("" + r["img"]!="noimgs" )
                {
                    e.Item.Cells[7].Text += "<img class='microimg' src='../img.ashx?act=good&id=" + r["id"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                }
                //if (webIO.CheckExistFile("../media/gimg/" + r["img"]))
                //{
                //    e.Item.Cells[7].Text += "<img class='microimg' src='../media/gimg/" + r["img"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                //}

                decimal q = cNum.cToDecimal(r["qty"]);
                if (iam.SubjectID == 0 || iam.PresentedSubject.Owner.ShowIncash)
                {
                    e.Item.Cells[5].Text = "" + q;
                }
                else
                {
                    e.Item.Cells[5].Text = stategood(cNum.cToInt(id), q, zn, zn_z);
                }

                decimal c = cNum.cToDecimal(r["price"], 2);
                decimal c_b = cNum.cToDecimal(r["pr_b"], 2);
                if (cNum.cToInt(r["accId"]) > 0)
                {
                    e.Item.Cells[6].Text = "" + cNum.cToDecimal(r["accprice"], 2);
                    e.Item.Cells[6].CssClass = "bold green";
                    e.Item.Cells[6].ToolTip = "Сейчас Акция!!!";
                }
                else if ((zn == "S" || (zn + zt) == "Z1" || zn_z == "NL") && (c_b > 0 || c > 0))
                {
                    e.Item.Cells[6].Text = "" + cNum.cToDecimal(c, 2);
                    if (iam.PresentedSubject.Owner.ShowInvDiscount)
                    {
                        e.Item.Cells[6].ToolTip = (c < c_b) ? "Это Ваша цена, а базовая цена " + c_b + "р." : "";
                    }
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

        string get_emailmess(string goodId)
        {
            string r = "ondblclick=\"openflywin('../common/gimail.aspx?id=" + goodId + "', 400, 300, 'Запрос информации о товаре')\"";
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
        
 
        private string get_angood(DataRowView r)
        {
            string ret = "";
            ret += (("" + r["an_k"] != "0") ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good="+r["goodcode"]+"&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
                (("" + r["an_s"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
                (("" + r["an_a"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=a',500,500,'Аналоги');>а</a>" : "");
            return ret;
        }

 


        


        protected void rbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "listsize", rbPageSize.SelectedValue);
            dgList.PageSize = cNum.cToInt(rbPageSize.SelectedValue);
            txSearch_TextChanged(null, e);
        }

 
        protected void btnSimSearch_Click(object sender, EventArgs e)
        {
            if (txSimSearch.Text.Trim() != "")
            {
                tkstack.Value = "";
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
            
            iam.CF_SelectedTKs = "";
            tkstack.Value = "";
            lbSelTN.Text = "";
            load_struct();
        }

        //protected void dlSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    iam.CF_SourceNomen = dlSource.SelectedValue;
        //    UserFilter.Save(iam.ID, "reg", iam.CF_SourceNomen);
        //    set_list_regime();
        //    load_struct();
        //    load_TK(UserFilter.Load(iam.ID, "TK"));
        //    load_brends(UserFilter.Load(iam.ID, "brend"));
        //    load_xName(UserFilter.Load(iam.ID, "Name"));

        //    txSearch_TextChanged(sender, e);
        //}

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
            load_TK();
            load_brends();
            load_xName();
            txSearch_TextChanged(sender, e);
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

       





    }
}