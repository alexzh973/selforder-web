using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ensoCom;

namespace wstcp.good
{
    /// <summary>
    /// Сводное описание для gdetail
    /// </summary>
    public class gdetail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";            
            string sid = context.Request["sid"];
            IAM iam = IAM.GetMe(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            int good_id = cNum.cToInt(context.Request["id"]);
            string resp = "";
            string act = context.Request["act"];
            switch (act)
            {

                case "incash":
                    DataTable dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId="+good_id+"),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId="+good_id+") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
                    if (dt.Select("qty>0").Length > 0)
                    {
                        resp = "<table class='incashdetail'>";
                        resp += "<tr>";
                        foreach (DataRow r in dt.Rows)
                        {
                            resp += "<td>" + r["Name"] + "</td>";
                        }
                        resp += "</tr>";
                        resp += "<tr>";
                        foreach (DataRow r in dt.Rows)
                        {
                            resp += "<td title='"+r["lcd"]+"'>" + cNum.cToDecimal(r["qty"]) + "</td>";
                        }
                        resp += "</tr>";
                        resp += "</table>";
                    }
                    else
                    {
                        resp = "<table class='incashdetail'><tr><td>в подразделениях нет остатков</td></tr></table>";
                    }
                    break;
                default:
                    resp = "unknown act: " + act;
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