using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using wstcp.Models;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace wstcp
{
    public partial class nsigoodies : p_p
    {

        string chQTY
        {
            get { return ""+Session["chQTY"];}
            set { Session["chQTY"] = value; }
        }
        string chNL
        {
            get { return "" + Session["chNL"]; }
            set { Session["chNL"] = value; }
        }
        string chP
        {
            get { return "" + Session["chP"]; }
            set { Session["chP"] = value; }
        }
        
        string TN
        {
            set { ViewState["TN"] = value; }
            get { return "" + ViewState["TN"]; }
        }
        string TK
        {
            set { ViewState["TK"] = value; }
            get { return "" + ViewState["TK"]; }
        }

        void show_last_upd()
        {
            //string sql = "select w.name, max(s.lcd) as lcd from good as g inner join gincash as s on g.id=s.goodId inner join own as w on w.id=s.ownerid group by w.name";
            string sql = "select id, Name, (select max(lcd) from OWNG where OwnerId=OWN.ID) as lastupd, (select top 1 oval from ELG where Entity='import' and Field=OWN.Descr order by LastUpd desc) as lastrep  from own";
            DataTable dt = db.GetDbTable(sql);
            Repeater1.DataSource = dt;
            Repeater1.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                if (!check_is_internal_ip() ) Response.Redirect("default.aspx");
            ((mainpage)this.Master).SelectedMenu = "incash";

            if (iam.SubjectID > 0)
            {
                Response.Write("В этот раздел доступ закрыт. извините");
                Response.End();
                return;
            }
            ((mainpage)this.Master).VisibleLeftPanel = true;

            show_last_upd();
            Session["OWNERID"] = 100000;
            if (!IsPostBack)
            {
                if (chQTY != "")
                {
                    rbQty.ClearSelection();
                    rbQty.Items.FindByValue(chQTY).Selected = true;                    
                }
                
                chZn.Items[0].Selected = (chNL == "True");
                chZn.Items[1].Selected = (chP == "True");
                load_owners();
                try
                {
                    dlOwners.Items.FindByValue("" + Session["OWNERID"]).Selected = true;
                }
                catch { }
                TN = "" + Request["tn"];
                TK = "" + Request["tk"];
                load_TN();
                load_TK();
                
                
                load_brends();
                btnSearch_Click(null, null);
            }
            

        }

        void load_TN()
        {
            DataTable dt = db.GetDbTable("select xTN, count(id) from good where  State<>'D' group by xTN ");// GoodInfo.GetCategories();

            foreach (DataRow r in dt.Select("", "xTN"))
                divCloud.Text += "<br/><a " + ((r["xTN"].ToString() == TN) ? "class='linkbutton bold selected shadow'" : "class='linkbutton'") + " href='nsigoodies.aspx?tn=" + r["xTN"] + "'>" + r["xTN"] + "</a>";

        }
        void load_TK()
        {
            blockCategory.Text = "";
            if (TN == "")
            {                
                return;
            }
            DataTable dt = db.GetDbTable("select xTK, count(id) from good where  State<>'D' and xTN='"+TN+"' group by xTK ");// GoodInfo.GetCategories();
            foreach (DataRow r in dt.Select("","xTK"))
            {
                blockCategory.Text += ((blockCategory.Text.Length > 0) ? " | " : "") + "<a " + ((r["xTK"].ToString() == TK) ? "class='bold'" : "") + " href='nsigoodies.aspx?tn=" + TN + "&tk=" + r["xTK"] + "'>" + r["xTK"] + "</a>";
            }
        }

        protected void load_brends()
        {

            string where = "";

            

            if (TN != "")
                where += " and xTN='" + TN + "'";

            if (TK != "")
                where += " and xTK='" + TK + "'";


            
            DataTable dt = db.GetDbTable("select brend,count(id) as qty from good where id in (select goodid from OWNG where OwnerID=" + dlOwners.SelectedValue + ") " + ((where != "") ? where : "") + " group by brend ");
            dlBrend.Items.Clear();
            dlBrend.Items.Add(new ListItem("Все", ""));

            foreach (DataRow r in dt.Select("Brend<>''", "Brend"))
            {
                dlBrend.Items.Add(new ListItem("" + r["Brend"] + " (" + r["qty"] + ")", "" + r["Brend"]));
            }
        }

        
        bool exist_number(string str)
        {
            bool res = false;
            Match match = Regex.Match(str, @"[0-9]");
            while (match.Success)
            {
                return true;
                //match = match.NextMatch();
            }
            return res;
        }

        protected void load_owners()
        {
            DataTable dt = Models.NSIOwner.GetTable();
            dlOwners.Items.Clear(); 
            dlOwners.ClearSelection();
            dlOwners.Items.Add(new ListItem("Все", "0"));
            foreach (DataRow r in dt.Select("", "Name"))
            {
                dlOwners.Items.Add(new ListItem(r["Name"].ToString(), r["ID"].ToString()));
            }
            try
            {
                dlOwners.Items.FindByValue(""+Session["OWNERID"]).Selected = true;
            }
            catch {
                dlOwners.SelectedIndex = 1;
            }

        }
        private string get_where_condition()
        {
            string where_conditions = "1=1";
            if (TN != "" )
                where_conditions += " and xTN like '%" + TN + "%'";
            if ( TK != "")
                where_conditions += " and xTK like '%" + TK + "%'";

            if (dlBrend.SelectedValue != "")
                where_conditions += " and brend like '" + dlBrend.SelectedValue + "%'";
            if (txSearch.Text.Length > 4)
            {
                if ("" + searchparam.Value == "")
                    where_conditions += " and (name like '%#SEARCH%' or descr like '%#SEARCH%' or goodcode like '%#SEARCH%' )";
                //else if (searchparam.Value == "category")
                //    where_conditions += " and category like '%#SEARCH%' ";
                else if (searchparam.Value == "brend")
                    where_conditions += " and brend like '%#SEARCH%' ";
            }
            else if (txSearch.Text.Length > 0)
            {
                if ("" + searchparam.Value == "")
                    where_conditions += " and (name like '#SEARCH%' or name like '% #SEARCH%'  or goodcode like '%#SEARCH%')";
                //else if (searchparam.Value == "category")
                //    where_conditions += " and category like '#SEARCH%' ";
                else if (searchparam.Value == "brend")
                    where_conditions += " and brend like '#SEARCH%' ";
            }
            where_conditions = where_conditions.Replace("#SEARCH", txSearch.Text.Trim().Replace(":", @"/"));


            return where_conditions;
        }
        private string get_whereZ_condition()
        {
            string RowFilter = " 1=1";
            switch (rbQty.SelectedValue)
            {
                case "1":
                    RowFilter += " and (Qty>0 or qtyother>0)";
                    break;
                case "2":
                    RowFilter += " and Qty>0";
                    break;
                case "3":
                    RowFilter += " and Qty=0 and qtyother>0";
                    break;
                case "0":
                default:
                RowFilter += "";
                    break;
            }

            

            if (chZn.Items[0].Selected && !chZn.Items[1].Selected)
                RowFilter += " and is_nl>0";
            else if (!chZn.Items[0].Selected && chZn.Items[1].Selected)
                RowFilter += " and is_p>0";
            else if (chZn.Items[0].Selected && chZn.Items[1].Selected)
                RowFilter += " and (is_nl>0 or is_p>0 )";
            return RowFilter;
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            chQTY = rbQty.SelectedValue;
            chNL = "" + chZn.Items[0].Selected;
            chP = "" + chZn.Items[1].Selected;

            string where_conditions = "1=1";
            selCatBrend.Text = "";
            if(TN!="" || TK!="")
                selCatBrend.Text = String.Format("{0}/{1}", TN,TK);
            
            //if (dlBrend.SelectedValue != "")
            //    selCatBrend.Text += "<div>Бренд: " + dlBrend.SelectedValue + "</div>";
            

            where_conditions = get_where_condition();
            string RowFilter = get_whereZ_condition();
            
            int owner_id = cNum.cToInt(dlOwners.SelectedValue);
            Session["OWNERID"] =  dlOwners.SelectedValue;

            
            DataTable dt = GoodInfo.Get_GoodesByOwner(owner_id, where_conditions, RowFilter);
            Cache.Insert("MyDT", dt, null, DateTime.Now.AddMinutes(15), TimeSpan.Zero);
            DataView dv = dt.DefaultView;

            dv.Sort = "Name";
            dgGoodies.DataSource = dv;
            dgGoodies.CurrentPageIndex = 0;
            try
            {
                dgGoodies.DataBind();
            }
            catch { }            
        }

        protected void dlOwners_SelectedIndexChanged(object sender, EventArgs e)
        {            
            btnSearch_Click(sender, e);
        }

        protected void chByOwner_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected void dgGoodies_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {

            if (Cache["MyDT"] != null)
            {
                DataTable dt = (DataTable)Cache["MyDT"];
                DataView dv = dt.DefaultView;
                dv.Sort = "Name";
                dgGoodies.DataSource = dv;
                dgGoodies.DataBind();
            }
            else
            {
                btnSearch_Click(null, null);
            }
            if (((DataView)dgGoodies.DataSource).Count / dgGoodies.PageSize >= e.NewPageIndex)
            {
                dgGoodies.CurrentPageIndex = e.NewPageIndex;
                dgGoodies.DataBind();
            }
        }


        protected void dgGoodies_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.SelectedItem)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)e.Item.DataItem;
                
                decimal qty = cNum.cToDecimal(r["Qty"]);
                if (qty > 0 )
                    e.Item.Cells[4].CssClass = "bold";
                if ("" + r["Zn_z"] == "NL" || "" + r["Zn_z"] == "P2" || "" + r["Zn_z"] == "Pz" || cNum.cToInt(r["is_nl"]) > 0 || cNum.cToInt(r["is_p"]) > 0)
                    e.Item.Cells[4].CssClass = e.Item.Cells[5].CssClass = "red bold";

                e.Item.Attributes.Add("id", "row_" + r["ID"]);
                e.Item.Attributes.Add("onclick", "thisrow(this.id);set4detail('" + r["ID"] + "','" + r["ens"] + "','"+iam.SessionID+"')");
                e.Item.Cells[6].Text = ""+r["ed"];//_get_qtydetail_(cNum.cToInt(r["ID"]), cNum.cToInt(r["qtyother"]));
                if ("" + r["ens"] != "")
                    e.Item.Cells[10].Text = " <a href='#' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r["ens"] + "/?key=x9BS1EFXj0', 830,600,'Описание товара')\"><img src='../simg/16/doc_view.png'/ alt='view' title='посмотреть детали'></a>";
                
                e.Item.Cells[3].Attributes.Add("id", "td_" + r["ID"]);
                
            }

        }

        string get_qtydetail(int good_id, string txt, string lastupd)
        {
            string qtydetail = "";
            qtydetail = "<table class='incashdetail'>";
            bool e = false;
            foreach (DataRow r in wstcp.Models.GoodInfo.GetIncashTable(good_id).Select("qty>0"))
            {
                qtydetail += "<tr><td>" + r["OwnerName"] + ": </td><td class='right'>" + r["qty"] + "</td></tr>";
                e = true;
            }
            qtydetail += "</table>";
            if (!e) qtydetail = "нет нигде";
            return String.Format("<a href='#' class='tt'>{0}<span class='tooltip'><span class='top'>остатки поподразделениям</span><span class='middle'>{1}</span><span class='bottom micro'>обновлено {2}</span></span></a>",txt,qtydetail,lastupd);
        }

        string _get_qtydetail_(int good_id, int qtytx)
        {
            string qtydetail = "";
            qtydetail = "<table class='incashdetail'>";
            bool e = false;
            foreach (DataRow r in wstcp.Models.GoodInfo.GetIncashTable(good_id).Select("qty>0"))
            {
                qtydetail += "<tr><td>" + r["OwnerName"] + ": </td><td class='right'>" + r["qty"] + "</td></tr>";
                e = true;
            }
            qtydetail += "</table>";
            if (!e) qtydetail = "нет нигде";
            return String.Format("<a href='#' class='tt'>{0}<span class='tooltip'><span class='top'>остатки поподразделениям</span><span class='middle' id='td_{1}'></span><span class='bottom micro'></span></span></a>", qtytx, good_id);
        }

        protected void dlBrend_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch_Click(sender, e);
            return;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch";
            divCloud.Text = path;
            if (File.Exists(path + @"\100000_100000.csv"))
            {
                divCloud.Text += " есть";
            }
            else
            {
                divCloud.Text += " нет";
            }
        }

        protected void chZn_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch_Click(sender, e);
        }

        protected void lbtnMakeFile_Click(object sender, EventArgs e)
        {
            string where_conditions = get_where_condition();
            string RowFilter = get_whereZ_condition();

            int owner_id = cNum.cToInt(dlOwners.SelectedValue);
            Session["OWNERID"] = dlOwners.SelectedValue;


            DataTable dt = GoodInfo.Get_GoodesByOwner(owner_id, where_conditions, RowFilter);

            


            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch\";
            string file = DateTime.Now.ToLongTimeString().Replace(":", "").Replace(" ", "") + ".csv";

            string line = "";
            FileStream fs = new FileStream(path + file, FileMode.Create);
            StreamWriter wr = new StreamWriter(fs, Encoding.GetEncoding(1251));
           
            line = "код" + ";" + "Наименование" + ";" + "уцск зн." + ";" + "уцск кол." + ";" + "чел зн." + ";" + "чел кол." + ";" + "таг зн." + ";" + "таг кол." + ";" + "тюм зн." + ";" + "тюм кол." + ";" + "сург зн." + ";" + "сург кол."+";"+"суппр";
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
            lbFile.Text = "<div><span class='message'>заберите выгруженный файл: <a href='../exch/"+file+"'>"+file+"</a></span></div>";
        }
        private DataTable owndt;
        private void load_own_pairs(int good_id)
        {
            owndt = GoodInfo.GetIncashTable(good_id);
        }
        private string get_onw_pair(int ownerid)
        {            
            DataRow[] r = owndt.Select("ownerid="+ownerid);
            return (r.Length>0)?";"+r[0]["zn_z"]+";"+r[0]["qty"]:";-;0";
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Global.forceimport();
            show_last_upd();
        }
    }
}