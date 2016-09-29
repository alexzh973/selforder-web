using System.Data;
using System.Text;
using ensoCom;
using selforderlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wstcp.special
{
    /// <summary>
    /// Сводное описание для ajfilter
    /// </summary>
    public class ajfilter : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string sid = context.Request["sid"];
            IAM iam = IamServices.GetIam(sid);
            string src = context.Request["src"];
            string trg = context.Request["trg"];
            string sel = context.Request["sel"];
            DataTable dt = db.GetDbTable("select '' as val from good where 1=0"); 
            switch (src)
            {
                case "brend":
                    switch (trg)
                    {
                        case "cat":
                            dt = db.GetDbTable("select distinct xtk as val from good inner join owng on owng.goodid=goodid where brend='"+sel+"' and ownerid="+iam.OwnerID);
                            break;
                        case "name":
                            dt = db.GetDbTable("select distinct xname as val from good inner join owng on owng.goodid=goodid where brend='" + sel + "' and ownerid=" + iam.OwnerID);
                            break;
                        case "":
                        default:
                            
                            break;
                    }

                    break;
                case "cat":
                    switch (trg)
                    {
                        case "brend":
                            dt = db.GetDbTable("select distinct brend as val from good inner join owng on owng.goodid=goodid where xtk='" + sel + "' and ownerid=" + iam.OwnerID);
                            break;
                        case "name":
                            dt = db.GetDbTable("select distinct xname as val from good inner join owng on owng.goodid=goodid where xtk='" + sel + "' and ownerid=" + iam.OwnerID);
                            break;
                        case "":
                        default:

                            break;
                    }

                    break;
                case "name":
                    switch (trg)
                    {
                        case "cat":
                            dt = db.GetDbTable("select distinct xtk as val from good inner join owng on owng.goodid=goodid where xname='" + sel + "' and ownerid=" + iam.OwnerID);
                            break;
                        case "brend":
                            dt = db.GetDbTable("select distinct brend as val from good inner join owng on owng.goodid=goodid where xname='" + sel + "' and ownerid=" + iam.OwnerID);
                            break;
                        case "":
                        default:

                            break;
                    }

                    break;
                case "":
                default:
                    
                    break;
            }
            StringBuilder sb = new StringBuilder();
            //sb.Append("<select id='dlCat'>");
            foreach (DataRow r in dt.Select("", "val"))
            {
                sb.Append("<option value='" + r[0] + "'>" + r[0] + "</option>");
            }
            //sb.Append("</select>");
            context.Response.Write(sb.ToString());
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