using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Data;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для msg1
    /// </summary>
    public class msg1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IAM iam = IAM.GetMe(""+context.Request["sid"]);
            string act = ""+context.Request["act"];
            string resp="";
            if (iam.ID > 0)
            {
                switch (act)
                {
                    case "sysmsg":
                        DataTable dt = db.GetDbTable("select * from msg where typemsg='s' and state=''");
                        if (dt.Rows.Count > 0 && dt.Columns.Count > 1)
                        {
                            resp = "" + dt.Rows[0]["msgtxt"];
                        }

                        break;
                    default:
                        resp = "";
                        break;
                }
            }
            else
            {
                resp = "";
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