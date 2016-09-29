using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ensoCom;
using selforderlib;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для img
    /// </summary>
    public class img : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string act = ""+context.Request["act"];
            int id = cNum.cToInt(context.Request["id"]);
            DataTable dt;
            switch (act.ToLower())
            {
                case "acc":
                
                dt = db.GetDbTable("select banimg from " + act + " where id=" + id);
                if (dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].GetType().Name=="Byte[]")
                {
                    byte[] bufimg = (byte[])dt.Rows[0][0];
                    context.Response.ContentType = webIO.GetFileContentType(".png");
                    context.Response.BinaryWrite(bufimg);
                }
                    break;
                case "good":
                    dt = db.GetDbTable("select src,ext from gimg where goodid=" + id);
                    if (dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].GetType().Name == "Byte[]")
                    {
                        byte[] bufimg = (byte[])dt.Rows[0][0];
                        context.Response.ContentType = webIO.GetFileContentType(""+dt.Rows[0][1]);
                        
                        context.Response.BinaryWrite(bufimg);
                        
                    }
                    break;
                default:
                    break;
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