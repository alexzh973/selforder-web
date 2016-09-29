using System.Data;
using ensoCom;
using selforderlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wstcp.account
{
    /// <summary>
    /// Сводное описание для acc
    /// </summary>
    public class acc : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string sid = context.Request["sid"];
            string act = context.Request["act"];
            string val = context.Request["val"];
            int own = cNum.cToInt(context.Request["own"],100000);
            IAM iam = IamServices.GetIam(sid);
            //if (iam.ID < 100000)
            //{
            //    context.Response.Write("access denide");
            //    return;
            //}
            if (act == "finds")
            {
                Subject s = new Subject(Subject.FindByField("INN",val),iam);
                if (s.ID > 0)
                {
                    context.Response.Write(s.Name + "+" + s.EmailTAs+'+'+'.');
                    return;
                } 
                else
                {
                    DataTable dt = db.GetDbTable("select NameSubj,emailTas from TTT where INN='" + val + "'");
                    if (dt.Rows.Count > 0)
                    {
                        context.Response.Write(dt.Rows[0][0] + "+" + dt.Rows[0][1]+'+'+'*');
                        return;
                        
                    }    
                }
            }
            context.Response.Write("");
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