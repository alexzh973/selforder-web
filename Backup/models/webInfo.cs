using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Text;
using System.Data;

using System.Web.UI.WebControls;
using System.Web.UI;




    /// <summary>
    /// Сводное описание для webInfo
    /// </summary>
    public class webInfo
    {
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

        
    }
