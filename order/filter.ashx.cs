using selforderlib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ensoCom;


namespace wstcp.order
{
    /// <summary>
    /// Сводное описание для filter
    /// </summary>
    public class filter : IHttpHandler
    {
        private IAM iam;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string sid = context.Request["sid"];

            iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("");
                return;
            }
            string src = context.Request["src"];
            //string trg = context.Request["src"];
            
            if (src == "brend")
            {
                
            }
        }

        private DataTable fltmatrix;
        private DataTable getMatrix()
        {
            if (iam.CF_SelectedTKs == "") return fltmatrix;
            if (fltmatrix != null) return fltmatrix;
            string seltk = iam.CF_SelectedTKs;
            DataTable dtchrs = db.GetDbTable("select keychars from imgtntk where t='tk' and name='" + seltk + "'");

            string[] chrs = ("" + dtchrs.Rows[0][0]).Split(',');
            int i = 0;
            string sql = "select distinct ";
            foreach (string chr in chrs)
            {
                sql += "(select ChrVal from GOODCH where goodcode=g.goodcode and Chr='" + chr + "' ) as [" + chr + "],";
            }
            sql += " from vGOOD100000 as g where g.xTK='" + seltk + "'";
            sql = sql.Replace(", from", " from");
            fltmatrix = db.GetDbTable(sql);
            return fltmatrix;
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