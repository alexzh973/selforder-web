using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Data;
using selforderlib;
using ensoCom;
using System.Text;
using System.IO;
using wstcp.models;

namespace wstcp
{

    public partial class goodies : p_p
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID < 100000 )
            {
                Response.Redirect("~/default.aspx", true);
            }
           

            
            if (!IsPostBack)
            {
                ((mainpage)Master).SelectedMenu = "incash";
                ((mainpage)Master).VisibleLeftPanel = true;
                ((mainpage)Master).VisibleRightPanel = false;

                webInfo.LoadOwners(dlOwners, UserFilter.Load(iam.ID, "fltowner", "" + iam.OwnerID), false, iam);
                webInfo.SetSelectedDropdownList(dlTypePr, UserFilter.Load(iam.ID, "typepr", "b"));

                _ACT_ = Request["act"];

                TabCtrl.Tabs.Add(new TabItem("Классификатор","Классификатор","Выбор товарного направления и товарной группы","slowfunc"));
                TabCtrl.Tabs.Add(new TabItem("Справочник номенклатуры", "Справочник номенклатуры", "Поиск и выбор номенклатуры","slowfunc"));
                TabCtrl.Tabs.Add(new TabItem("Акция", "Акция ", "","tabacc",""));

                load_list_accs();

                try { rbQty.Items.FindByValue(UserFilter.Load(iam.ID, "fltincash")).Selected = true; }
                catch { }
                chZn.SelectedValue = UserFilter.Load(iam.ID, "fltzn", "");
                if (Request["id"] != null)
                {
                    eID = cNum.cToInt(Request["id"]);
                    CSA = new SaleAcc();
                    SaleAcc.Load(CSA,eID, iam);
                    iam.CurrentObject = CSA;
                    TabCtrl.SelectTab(2);
                    TabCtrl.Tabs[2].Title = (CSA.ID>0)?"Акция " + CSA.Name:"Новая"+((CSA.Items.Count>0)?" (еще не сохраненная)":"")+" акция";
                    
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
                    TabCtrl.SelectTab(1);
                    
                    lbSelTN.Text = tn.Title;
                }
                else if (Request["tk"] != null)
                {
                    xTK tk = xTK_db.Find(cNum.cToInt(Request["tk"]));
                    iam.CF_SelectedTKs = tk.Title.Replace(",", "@");
                    lbSelTN.Text = tk.xTN_Name + " / " + tk.Title;
                    
                    TabCtrl.SelectTab(1);
                }
                else
                {
                    lbSelTN.Text = "Все товарные направления";
                }



                set_pagesize_regime();
                if (iam.CurrentObject != null)
                {
                    lbTitle.Text = "Справочник номенклатуры (режим Акции)";
                } 
                else
                {
                    lbTitle.Text = "Справочник номенклатуры";

                }
                //lbTitle.Text = (CSA.ID > 0) ? "Акция " + CSA.Name : "Новая" + ((CSA.Items.Count > 0) ? " (еще не сохраненная)" : "") + " акция";
               
                if (TabCtrl.CurrentTabIndex == 1)
                {
                    load_brends();
                    load_TK();
                    load_xName();

                    txSearch_TextChanged(null, null);
                } 
                else if (TabCtrl.CurrentTabIndex == 2)
                {
                    show_current_acc();
                }
                else
                {
                    load_struct();
                }
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
            if (e.Position == 2)
            {
                show_current_acc();
            }
            else if (e.Position == 1)
            {
                load_TK(UserFilter.Load(iam.ID, "TK"));
                load_brends(UserFilter.Load(iam.ID, "brend"));
                load_xName(UserFilter.Load(iam.ID, "Name"));
                txSearch_TextChanged(null, null);
            } else
            {
                load_struct();
            }
        }

        private void show_current_acc()
        {
            MultiView1.SetActiveView(vAcc);
            load_current_acc();
        }


        private SaleAcc CSA
        {
            get {SaleAcc acc = new SaleAcc();
                if (iam.CurrentObject == null)
                {

                    SaleAcc.Load(acc, eID, iam);
                    iam.CurrentObject = acc;
                    return acc;
                } 
                else
                    return (SaleAcc) iam.CurrentObject;
                
            }
            set { iam.CurrentObject = value; }
        }

    
        private void load_current_acc()
        {
            //if (iam.CurrentObject == null)
            //{
            //    SaleAcc.Load(CSA, eID, iam);
            //    iam.CurrentObject = CSA;
            //}
            //else
            //    CSA = (SaleAcc) iam.CurrentObject;

            txNameAcc.Text = CSA.Name;
            txText.Text = CSA.Descr;
            //StartDate.AutoPostBack = FinishDate.AutoPostBack = false;
            StartDate.SelectedDate = CSA.StartDate;
            FinishDate.SelectedDate = CSA.FinishDate;

            dgOrder.DataSource = CSA.Items;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();
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
            MultiView1.SetActiveView(vStruct);
            if (SelectMutipleTK == "0")
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



        private void load_isystems()
        {
            string[] isys = new[] {"",""};
            
        }

        private void load_struct_single()
        {
            //menuSys.FindItem(iam.CF_iSys).
            int i = 0;
            string regwhere = "";

            int ownerId = cNum.cToInt(dlOwners.SelectedValue);
            if (iam.CF_iSys != "") regwhere += " isys like '%" + iam.CF_iSys + "%'";
            List<xTN> listTN = xTN_db.GetList(regwhere);
            StringBuilder sb = new StringBuilder();
            //if (listTN.Count == 0)
            //    sb.Append("<br/><br/><br/><p>У Вас, по всей видимости еще не было заказов, поэтому выберите другой <span class='bold'>Источник списка номенклатуры</span></p><br/><br/>");


            foreach (xTN r_tn in listTN)
            {
                sb.Append("<div class='blocktn'>");
                sb.Append("<a href='list.aspx?tn=" + r_tn.ID + "' class='tnlink' >" + r_tn.Title.ToUpper() + "</a><br/>");
                sb.Append("<div class='blocktk'><div class='floatLeft-90'><ul>");
                foreach (xTK r_tk in xTK_db.GetList(r_tn.Title, regwhere))
                {
                    string tk = r_tk.Title;
                    tk = tk.Replace(",", "@");

                    bool ch = (iam.CF_SelectedTKs.ToLower().IndexOf("" + tk.ToLower()) > -1);
                    sb.Append(" <li><a id='tk" + r_tk.ID + "' onclick=\"tkInStack(this.id,'" + tk.ToLower() + "')\" data='" + r_tk.Title.ToLower() + "' href='list.aspx?tk=" + r_tk.ID + "' " + ((ch) ? "class='tklink-active'" : "class='tklink'") + " >" + r_tk.Title + "</a></li>");
                    i += 1;
                }
                sb.Append("</ul></div><div class='floatRight-10'></div><div class='clearBoth'></div></div>");// block_TK
                sb.Append("</div>");// block_TN
            }
            sb.Append("<div style='clear:both;'></div>");
            struct_place.Text = sb.ToString();
        }

        private void load_struct_multi()
        {

            int i = 0;
            string regwhere = "";

            List<xTN> listTN = xTN_db.GetList(regwhere);
            StringBuilder sb = new StringBuilder();


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

        }


        protected void load_brends(string default_value = "")
        {

            string where = "";

            if (dlTK.SelectedValue != "")
                where += " and xTK='" + dlTK.SelectedValue + "'";

            string seltk = get_selected_tks();

            if (seltk != "")
                where = " and xTK in (" + seltk + ")";

            int ownerId = cNum.cToInt(dlOwners.SelectedValue);
            string whereqty = "";//(chIncash.Checked) ? " and qty>0" : "";

            switch (rbQty.SelectedValue)
            {
                case "1":
                    whereqty += " and (Qty+qtyother)>0 ";
                    break;
                case "2":
                    whereqty += " and Qty>0";
                    break;
                case "3":
                    whereqty += " and Qty=0 and qtyother>0";
                    break;
                case "0":
                default:
                    break;
            }


            DataTable dt = new DataTable();

            dt = db.GetDbTable("select brend, count(id) as qty from vGOOD" + ownerId + " where 1=1  " + whereqty + " " + ((where != "") ? where : "") + ((iam.CF_iSys != "") ? " and isys like '%" + iam.CF_iSys + "%'" : "") + " group by brend ");



            dlBrends.Items.Clear();
            dlBrends.Items.Add(new ListItem("Все", ""));
            foreach (DataRow r in dt.Select("Brend<>''", "Brend"))
            {
                dlBrends.Items.Add(new ListItem("" + r["Brend"] + " (" + r["qty"] + ")", "" + r["Brend"]));
            }
            webInfo.SetSelectedDropdownList(dlBrends, default_value);
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
            int ownerId = cNum.cToInt(dlOwners.SelectedValue);
            string whereqty = "";//(chIncash.Checked) ? " and qty>0" : "";

            switch (rbQty.SelectedValue)
            {
                case "1":
                    whereqty += " and (Qty+qtyother)>0 ";
                    break;
                case "2":
                    whereqty += " and Qty>0";
                    break;
                case "3":
                    whereqty += " and Qty=0 and qtyother>0";
                    break;
                case "0":
                default:
                    break;
            }

            DataTable dt = db.GetDbTable("select xTK, count(goodId) as qty from vGOOD" + ownerId + " where 1=1 " + whereqty + " " + ((seltk != "") ? " and xTK in (" + seltk + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + ((iam.CF_iSys != "") ? " and isys like '%" + iam.CF_iSys + "%'" : "") + " group by xTK");


            foreach (DataRow r in dt.Select("xTK<>''", "xTK"))
                dlTK.Items.Add(new ListItem("" + r["xTK"] + " (" + r["qty"] + ")", "" + r["xTK"]));

            webInfo.SetSelectedDropdownList(dlTK, default_value);
        }

        protected void load_xName(string default_value = "")
        {

            string tk = dlTK.SelectedValue;
            string brend = dlBrends.SelectedValue;
            dlNames.Items.Clear();
            dlNames.Items.Add(new ListItem("", ""));
            dlNames.ClearSelection();
            string seltks = get_selected_tks();
            int ownerId = cNum.cToInt(dlOwners.SelectedValue);
            string whereqty = "";//(chIncash.Checked) ? " and qty>0" : "";

            switch (rbQty.SelectedValue)
            {
                case "1":
                    whereqty += " and (Qty+qtyother)>0 ";
                    break;
                case "2":
                    whereqty += " and Qty>0";
                    break;
                case "3":
                    whereqty += " and Qty=0 and qtyother>0";
                    break;
                case "0":
                default:
                    break;
            }



            DataTable dt = db.GetDbTable("select xName, count(id) as qty from vGOOD" + ownerId + " where 1=1 " + whereqty + " " + ((seltks != "") ? " and xTK in (" + seltks + ")" : "") + ((brend != "") ? " and Brend='" + brend + "'" : "") + ((iam.CF_iSys != "") ? " and isys like '%" + iam.CF_iSys + "%'" : "") + " group by xName");


            foreach (DataRow r in dt.Select("", "xName"))
            {
                dlNames.Items.Add(new ListItem("" + r["xName"] + " (" + r["qty"] + ")", "" + r["xName"]));
            }
            webInfo.SetSelectedDropdownList(dlNames, default_value);
        }



        protected void txSearch_TextChanged(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(vList);
            int ownerId = cNum.cToInt(dlOwners.SelectedValue); // заменить на из фильтра

            string where_conditions = get_where_condition();

            string wdate = cDate.Date2Sql(cDate.TodayD);
            DataTable dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " *,(select count(ancode) from angood as ang where ang.goodcode=t1.goodcode) as qang,isnull((select price from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<="+wdate+" and finishdate>="+wdate+" and t3.ownerID="+ownerId+" and t2.goodid=t1.goodid ),0) as accprice,isnull((select max(accid) from accgood as t2 inner join acc as t3 on t2.accid=t3.id and startdate<="+wdate+" and finishdate>="+wdate+" and t3.ownerID="+ownerId+" and t2.goodid=t1.goodid ),0) as accId from vGood" + ownerId + " as t1 " + ((where_conditions == "") ? "" : " where " + where_conditions) + "");
            DataView dv;
            Cache.Insert("MyDT", dt, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);

            dv = dt.DefaultView;

            dv.Sort = "zn, Name";
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

            switch (rbQty.SelectedValue)
            {
                case "1":
                    where_conditions += " and (Qty+qtyother)>0 ";
                    break;
                case "2":
                    where_conditions += " and Qty>0";
                    break;
                case "3":
                    where_conditions += " and Qty=0 and qtyother>0";
                    break;
                case "0":
                default:
                    break;
            }

            if (chZn.Items[0].Selected && !chZn.Items[1].Selected)
                where_conditions += " and zn_z='NL'";
            else if (!chZn.Items[0].Selected && chZn.Items[1].Selected)
                where_conditions += " and zn_z in ('P2','Pz')";
            else if (chZn.Items[0].Selected && chZn.Items[1].Selected)
                where_conditions += " and zn_z in ('P2','Pz','NL') ";

            if (iam.CF_iSys != "")
                where_conditions += " and isys like '%" + iam.CF_iSys + "%'";

            return where_conditions;
        }


        protected void dgList_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {

            if (Cache["MyDT"] != null)
            {
                DataTable dt = (DataTable)Cache["MyDT"];
                DataView dv = dt.DefaultView;
                dv.Sort = "zn, Name";
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




        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.SelectedItem)
            {
                DataRowView r = (DataRowView)e.Item.DataItem;
                int id = cNum.cToInt(r["ID"]);
                string code = "" + r["goodcode"];
                decimal qty = cNum.cToDecimal(r["Qty"]);
                if (qty > 0)
                    e.Item.Cells[4].CssClass = "bold";
                if ("" + r["Zn_z"] == "NL" || "" + r["Zn_z"] == "P2" || "" + r["Zn_z"] == "Pz")
                {
                    e.Item.Cells[4].CssClass = e.Item.Cells[5].CssClass = "red ";
                    e.Item.Cells[4].CssClass = e.Item.Cells[5].ToolTip = "Перезапас";
                }
                if (cNum.cToInt(r["accid"]) > 0 || CSA.Items.Exists(x => x.GoodId == id))
                {
                    e.Item.Cells[2].CssClass = "bold small";
                    e.Item.Cells[2].ToolTip = "Товар участвует в акции ";
                   // e.Item.Cells[2].Text += "<img id='kn_" + id + "' src='../simg/minus.png' title='убрать из Акции' style='cursor:pointer;' onclick=removeFromAcc('" + r["acc"] + "','" + id + "') alt='-A'/>";
                } else
                {e.Item.Cells[2].CssClass = " small";
                   // e.Item.Cells[2].Text += "<img id='kn_" + id + "' src='../simg/plus.png' title='добавить в Акцию' style='cursor:pointer;' onclick=addToAcc('" + CSA.ID + "','" + id + "') alt='+A'/>";
                }
                e.Item.Cells[3].Text = "<a href='#' onclick=\"openflywin('../good/card.aspx?ownid=" + dlOwners.SelectedValue + "&id=" + id + "', 830,600,'Карточка товара')\">" + r["Name"] + "</a>";
                if ((cNum.cToInt(r["qang"]) > 0))
                    e.Item.Cells[3].Text += " <span class='small' title='есть информация об аналогах/комплектующих/сопутствующих'>[" + r["qang"] + "]</span>";
                //e.Item.Cells[3].Text += get_angood(r);

                e.Item.Cells[5].Text = "<a href='#' onmouseover=\"set4detail('" + id + "','" + r["ens"] + "','" + iam.SessionID + "');\" class='tooltip' id='a_" + id + "'>" + cNum.cToDecimal(r["Qtyother"], 2) + "</a>";
                
                e.Item.Attributes.Add("id", "row_" + id);
                e.Item.Attributes.Add("data", code);
                e.Item.CssClass = "gooddrg";
                e.Item.Attributes.Add("onclick", "thisrow(this.id);showangood('" + code + "')");
                e.Item.Cells[6].Text = "" + r["ed"];
                if ("" + r["ens"] != "")
                    e.Item.Cells[10].Text = " <a href='#' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r["ens"] + "/?key=x9BS1EFXj0', 830,600,'Описание товара')\"><img src='../simg/16/doc_view.png'/ alt='view' title='посмотреть детали'></a>";


                if ("" + r["img"] != "noimgs")
                {
                    e.Item.Cells[10].Text += "<img class='microimg' src='../img.ashx?act=good&id=" + r["id"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                }
                
                //if (webIO.CheckExistFile("../media/gimg/" + r["img"]))
                //{
                //    e.Item.Cells[10].Text += "<img class='microimg' src='../media/gimg/" + r["img"] + "' id='imgeye" + id + "'/><img style='margin:1px;' id='eye" + id + "' class='eye' src='../simg/16/photo.png'/> ";
                //}

                if (iam.IsAdmin || iam.IsSuperAdmin)
                {
                    //e.Item.Cells[10].Text += " <a href='#' onclick=\"openflywin('../good/card.aspx?ownid=" + dlOwners.SelectedValue + "&id=" + r["id"] + "', 830,600,'Карточка товара')\"><img src='../simg/16/doc_view.png'/ alt='view' title='карточка детали'></a>";
                    e.Item.Cells[10].Text += " <a href='../good/profile.aspx?id=" + id + "' class='linkbutton' >...</a>";
                }
                e.Item.Cells[3].Attributes.Add("id", "td_" + id);


                e.Item.Cells[3].Attributes.Add("data", code);
                e.Item.Cells[8].Text = ""+cNum.cToDecimal(r[dlTypePr.SelectedValue],2);
                if (cNum.cToInt(r["accid"]) > 0)
                    e.Item.Cells[8].Text += "<br/><span class='small blue' title='цена по акции'>" + cNum.cToDecimal(r["accprice"], 2) +"</span>";
                e.Item.Cells[5].Attributes.Add("id", "q_" + id);
            }

            
        }
        //private string get_angood(DataRowView r)
        //{
        //    string ret = "";
        //    ret += (("" + r["an_k"] != "0") ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
        //        (("" + r["an_s"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
        //        (("" + r["an_a"] != "0") ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r["goodcode"] + "&ant=a',500,500,'Аналоги');>а</a>" : "");
        //    return ret;
        //}
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

                iam.CF_SelectedTKs = "";
                iam.CF_SourceNomen = "";
                // chIncash.Checked = false;

                txSearch.Text = txSimSearch.Text;
                txSimSearch.Text = "";
                TabCtrl.SelectTab(1);
                //TabControl1.SelectedTabIndex = 1;
                //if (TabControl1.IsActiveTabpage(pnlList))
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

            if (TabCtrl.CurrentTabIndex == 0)
                load_struct();
            else if (TabCtrl.CurrentTabIndex == 1)
            {
                load_TK(UserFilter.Load(iam.ID, "TK"));
                load_brends(UserFilter.Load(iam.ID, "brend"));
                load_xName(UserFilter.Load(iam.ID, "Name"));
                txSearch_TextChanged(sender, e);
            }

        }

        protected void lbtnChangeTN_Click(object sender, EventArgs e)
        {
            TabCtrl.SelectTab(0);
            iam.CF_SelectedTKs = "";
            lbSelTN.Text = "";
            load_struct();
        }


        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            iam.CF_iSys = "";
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



        private DataTable owndt;
        private void load_own_pairs(int good_id)
        {
            owndt = GoodInfo.GetIncashTable(good_id);
        }
        private string get_onw_pair(int ownerid)
        {
            DataRow[] r = owndt.Select("ownerid=" + ownerid);
            return (r.Length > 0) ? ";" + r[0]["zn_z"] + ";" + r[0]["qty"] : ";-;0";
        }

        protected void dlOwners_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "fltowner", dlOwners.SelectedValue);
            txSearch_TextChanged(sender, e);
        }

        protected void chZn_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "fltincash", rbQty.SelectedValue);
            UserFilter.Save(iam.ID, "fltzn", chZn.SelectedValue);
            txSearch_TextChanged(sender, e);
        }

        protected void dlTypePr_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserFilter.Save(iam.ID, "typepr", dlTypePr.SelectedValue);
            txSearch_TextChanged(sender, e);
        }

        protected void menuSys_MenuItemClick(object sender, MenuEventArgs e)
        {
            iam.CF_iSys = e.Item.Value;
            load_struct();
            
        }


        protected void dgAcc_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                SaleAccItem r = (SaleAccItem)e.Item.DataItem;

        
                e.Item.Attributes.Add("id", "tr_" + r.GoodId);
                e.Item.Attributes.Add("onclick", "thisrow(this.id)");
                e.Item.Cells[6].Text = " <a href='../good/profile.aspx?id=" + r.GoodId + "' class='linkbutton' >...</a>";


                //e.Item.Cells[1].Text = "<img src='../simg/minus.png' title='убрать из Акции' style='cursor:pointer;' onclick=removeFromAcc('" + CSA.ID + "','" + r.GoodId + "') alt='-A'/>";


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


        protected void dgAcc_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = cNum.cToInt(e.Item.Cells[0].Text);
            SaleAccItem delItem = CSA.Items.Find(x => x.GoodId == id);
            if (delItem != null)
                CSA.Items.Remove(delItem);

            show_current_acc();
        }

        protected void lbtnMakeFile_Click(object sender, EventArgs e)
        {

            int ownerId = cNum.cToInt(dlOwners.SelectedValue); // заменить на из фильтра
            DataTable dt = new DataTable();
            DataView dv;

            string where_conditions = "1=1";
            where_conditions = get_where_condition();




            dt = db.GetDbTable("select " + ((where_conditions == "1=1") ? "top 100" : "") + " * from vGood" + ownerId + " " + ((where_conditions == "") ? "" : " where " + where_conditions) + "");




            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch\";
            string file = DateTime.Now.ToLongTimeString().Replace(":", "").Replace(" ", "") + ".csv";

            string line = "";
            FileStream fs = new FileStream(path + file, FileMode.Create);
            StreamWriter wr = new StreamWriter(fs, Encoding.GetEncoding(1251));

            line = "код" + ";" + "Наименование" + ";" + "уцск зн." + ";" + "уцск кол." + ";" + "чел зн." + ";" + "чел кол." + ";" + "таг зн." + ";" + "таг кол." + ";" + "тюм зн." + ";" + "тюм кол." + ";" + "сург зн." + ";" + "сург кол." + ";" + "суппр";
            wr.WriteLine(line);
            foreach (DataRow r in dt.Rows)
            {
                line = "" + r["GoodCode"] + ";" + r["Name"];
                load_own_pairs(cNum.cToInt(r["ID"]));
                line += get_onw_pair(100000);
                line += get_onw_pair(100001);
                line += get_onw_pair(100002);
                line += get_onw_pair(100003);
                line += get_onw_pair(100004);
                line += ";" + cNum.cToInt(r["pr_spr"]);
                wr.WriteLine(line);
                owndt = null;
            }
            wr.Close();
            fs.Close();
            lbFile.Text = "<div><span class='message'>заберите выгруженный файл: <a href='../exch/" + file + "'>" + file + "</a></span></div>";
        }

        protected void btnSaveAcc_Click(object sender, EventArgs e)
        {
            CSA.Name = txNameAcc.Text;
            CSA.StartDate = StartDate.SelectedDate;
            CSA.FinishDate = FinishDate.SelectedDate;
            CSA.Descr = txText.Text;
            
            
            SaleAcc.Save(CSA, iam);
            if (ufBanner.FileName != "" && isImg(ufBanner.FileName) && ufBanner.FileBytes.Length < 1000000 )
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ufBanner.FileContent);
                int h = img.Height;
                int w = img.Width;
                int size_s = 96;
                decimal p_s;

                p_s = ((decimal) size_s)/((h > w) ? h : w);
                if (p_s > 1) p_s = 1;


                Bitmap bmp_s = new Bitmap(img, cNum.cToInt(img.Width*p_s), cNum.cToInt(img.Height*p_s));


                byte[] buf = ImageToByte(bmp_s);
                store(CSA.ID, buf);
            }
            lbMess.Text = "Акция \""+CSA.Name+"\" сохранена.";
            iam.CurrentObject = CSA = null;
        }

        public bool isImg(string filename)
        {
            switch (Path.GetExtension(filename).Replace(".",""))
            {
                case "jpg":
                case "gif":
                case "bmp":
                case "png":
                case "jpeg":
                    return true;
                default:
                    return false;
            }
        }
        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static void store(int id, byte[] img)
        {
           


            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.Add("@banimg", System.Data.SqlDbType.Image);
            cmd.Parameters["@banimg"].Value = img;
            if (id > 0)
            {
                db.ExecuteCmd(cmd, "update ACC set banimg=@banimg where ID=" + id);
            }
            cn.Close();
        }


    }
}