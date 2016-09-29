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
                    DataTable dt = db.GetDbTable("select (select count(goodid) from OWNG where ownerid=100000 and state in ('','u','o')) as a, (select count(goodid) from OWNG where ownerid=100000 and state='u') as u, (select count(goodid) from OWNG where ownerid=100000 and state='o') as o,(select count(goodid) from OWNG where ownerid=100000 and state='') as e");
                    if (dt.Columns.Count > 1)
                    {
                        resp = string.Format("всего {0}, u:{1}, o: {2}, _:{3}", dt.Rows[0]["a"], dt.Rows[0]["u"], dt.Rows[0]["o"], dt.Rows[0]["e"]);
                    }
                //    break;
                //case "in":
                    DataTable dt1 = db.GetDbTable("select own.name, buzy.lcd from buzy inner join own on buzy.ownerid=own.id where buzyst<>''");
                    string re = "";
                    foreach (DataRow r in dt1.Rows)
                        cStr.Add(ref re, " " + r["name"] + " (" + r["lcd"] + ")");
                    if (re != "") re = "\nСейчас происходит импорт:" + re;
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