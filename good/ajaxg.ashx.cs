using ensoCom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для ajaxg
    /// </summary>
    public class ajaxg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string resp = "";
            context.Response.ContentType = "text/plain";
            string act = context.Request["act"];
            //switch (act)
            //{

            //    case "brend2tk":
            //        DataTable dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId=" + good_id + "),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId=" + good_id + ") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
            //        if (dt.Select("qty>0").Length > 0)
            //        {
                        
            //            foreach (DataRow r in dt.Rows)
            //            {
            //                resp += "<option value=''>" + r["Name"] + "</option>";
            //            }
                        
            //        }
            //        else
            //        {
            //        }
            //        break;
            //    default:
            //        resp = "unknown act: " + act;
            //        break;
            //}
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