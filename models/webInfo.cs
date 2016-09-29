using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Text;
using System.Data;

using System.Web.UI.WebControls;
using System.Web.UI;
using selforderlib;


/// <summary>
    /// Сводное описание для webInfo
    /// </summary>
    public class webInfo
    {
        public static string make_where(List<string> fields, string searchString, int minLength = 4)
        {
            if (fields.Count < 1 || searchString.Trim() == "") return "";
            
            string wr = "";
            string w = "";
            if (searchString.Length > 0)
            {

                foreach (string word in searchString.Split(' '))
                {
                    wr += ((wr != "") ? " and " : "");
                    if (fields.Count == 1)
                        wr += fields[0] + " like '%" + word + "%'";
                    else
                    {

                        w = "";
                        foreach (string fld in fields)
                        {
                            w += ((w != "") ? " or " : "") + fld + " like '%" + word + "%'";
                        }
                        wr += "(" + w + ")";
                    }
                }
                if (wr != "") wr = "(" + wr + ")";

            }

            return wr;
        }

        
        
        
        private static int __point_index=1;
        public static void DebugTime(Literal control, string mark="point"){
            control.Text += " [" + ((mark == "point") ? mark + "(" + __point_index + ")" : mark) + ":" + DateTime.Now.ToLongTimeString() + "]";
            __point_index += 1;
        }
        public static void DebugTime(Label control, string mark = "point")
        {
            control.Text += " [" + ((mark == "point") ? mark + "(" + __point_index + ")" : mark) + ":" + DateTime.Now.ToLongTimeString() + "]";
        }
        public static string select_search(string search_string, string src_text)
        {
            string ret = src_text;
            foreach (string s in search_string.Split(' '))
            {
                if (s != "")
                {
                    if (ret.IndexOf(s) > -1)
                    {
                        ret = ret.Replace(s, "<span class='searchselected bold'>" + s + "</span>");
                    }
                    else
                    {
                        string fl = s.Substring(0, 1).ToUpper();
                        string oo = fl + s.Substring(1, s.Length - 1);
                        ret = ret.Replace(oo, "<span class='searchselected bold'>" + oo + "</span>");

                    }
                }
                //else
                //    return ret;
            }
            return ret;
            if (search_string != "")
            {
                if (src_text.IndexOf(search_string) > -1)
                {
                    return src_text.Replace(search_string, "<span class='searchselected bold'>" + search_string + "</span>");
                }
                else
                {
                    string fl = search_string.Substring(0, 1).ToUpper();
                    string oo = fl + search_string.Substring(1, search_string.Length - 1);
                    return src_text.Replace(oo, "<span class='searchselected bold'>" + oo + "</span>");

                }
            }
            else
                return src_text;
        }
        

        public static string get_http_address_currentpage(Page page)
        {
            return (page.Request.Url.Query != "") ? page.Request.Url.ToString().Replace(page.Request.Url.Query, "") : page.Request.Url.ToString();
        }
        public static string get_hhtp_address_currentsite(Page page)
        {
            return "http://" + page.Request.Params["HTTP_HOST"];
        }

        public static void LoadOwners(DropDownList dd_list, string default_select, bool load_empty, IAM current_user)
        {
            dd_list.Items.Clear();
            if (load_empty) dd_list.Items.Add(new ListItem("", ""));
            foreach (DataRow r in db.GetDbTable("select * from OWN order by Name").Rows)
            {
                dd_list.Items.Add(new ListItem(""+r["name"], ""+r["id"]));
            }
            if (default_select != "")
            {
                SetSelectedDropdownList(dd_list, default_select);
            }

        }

        public static void LoadSubjectFolders(DropDownList dd_list, string default_select, bool load_empty, IAM current_user)
        {
            dd_list.Items.Clear();
            if (load_empty) dd_list.Items.Add(new ListItem("", ""));
            foreach (DataRow r in db.GetDbTable("select * from SUBJ where State<>'D' and IsFolder='Y' order by Name").Rows)
            {
                dd_list.Items.Add(new ListItem("" + r["name"], "" + r["id"]));
            }
            if (default_select != "")
            {
                SetSelectedDropdownList(dd_list, default_select);

            }

        }

        public static void LoadPersons(DropDownList dd_list, string default_select, bool load_empty, string filter, IAM current_user)
        {
            dd_list.Items.Clear();
            if (load_empty) dd_list.Items.Add(new ListItem("", ""));
            foreach (DataRow r in db.GetDbTable("select id, name, email from "+pUser.TDB+" where isfolder='N', State<>'D' "+((filter!="")?" and "+filter:"")+" order by Name").Rows)
            {
                ListItem li = new ListItem("" + r["name"], "" + r["id"]);
                li.Attributes.Add("tooltip",""+r["email"]);
                dd_list.Items.Add(li);

            }
            if (default_select != "")
            {
                SetSelectedDropdownList(dd_list, default_select);
            }

        }

    public static int SetSelectedDropdownList(DropDownList dd_list, string selectValue)
    {
        try
        {
            dd_list.Items.FindByValue(selectValue).Selected = true;
        }
        catch
        {
            try { dd_list.Items[0].Selected = true; }
            catch { }
        }
        return dd_list.SelectedIndex;
    }

    }
