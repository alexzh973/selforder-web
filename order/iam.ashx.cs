using selforderlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wstcp.order
{
    /// <summary>
    /// Сводное описание для iam
    /// </summary>
    public class iam : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string sid = context.Request["sid"];
            IAM iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            string act = context.Request["act"];

            if (act == "seltks")
            {
                iam.CF_SelectedTKs = context.Request["val"];
                return;
            }
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