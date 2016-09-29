using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Data;
namespace wstcp
{
    /// <summary>
    /// Сводное описание для import
    /// </summary>
    public class import : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string resp = "";
            switch ("" + context.Request["act"])
            {
                case "fi":
                    DataTable dt = db.GetDbTable("select (select count(goodid) from OWNG where ownerid=100000 and state in ('','u','o')) as a, (select count(goodid) from OWNG where ownerid=100000 and state='o') as o");
                    if (dt.Columns.Count > 1 && dt.Rows.Count>0)
                    {
                        resp = string.Format("полн: всего {0}, имп: {1}, ост:{2}", dt.Rows[0]["a"], dt.Rows[0]["o"], cNum.cToInt(dt.Rows[0]["a"])-cNum.cToInt(dt.Rows[0]["o"]));
                    }
                    if (Global.NEEDSTOPIMPORT) resp += " NEEDSTOPIMPORT";
                    break;
                case "in":
                    DataTable dt1 = db.GetDbTable("select max(lcd) as lcd, (select name from own where ownerid=id) as owner, ownerid  from owng group by ownerid order by ownerid");
                    string re = "";
                    foreach (DataRow r in dt1.Rows)
                        cStr.Add(ref re, " " + r["owner"] + ": " + r["lcd"] + "");
                    if (re != "") re = "Посл импорт: " + re;
                    resp += re;
                    if (Global.NEEDSTOPIMPORT) resp += " NEEDSTOPIMPORT";
                    
                    break;
                default:
                    break;
            }
            context.Response.Write(resp);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}