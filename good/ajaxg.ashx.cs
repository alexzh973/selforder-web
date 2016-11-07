using ensoCom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using selforderlib;

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
string sid = context.Request["sid"];
            IAM iam = IamServices.GetIam(sid);
            string regwhere = "";
            switch (act)
            {
                case "getlisttn":
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
                    foreach (xTN tn in listTN)
                    {
                        List<xTK> ch = xTK_db.GetList(tn.Title);
                        tn.Childs = ch;
                    }
                    resp = JsonConvert.SerializeObject(listTN);
                    break;
                case "getlisttk":
                    
                    break;
                default:
                    break;

            }
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