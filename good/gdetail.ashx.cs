using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Data;
using ensoCom;
using selforderlib;

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
            IAM iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            int good_id = cNum.cToInt(context.Request["id"]);
            string resp = "";
            string act = context.Request["act"];
            DataTable dt;
            switch (act)
            {

                case "incash":
                    resp = prepareIncashTable(good_id);
                    break;
                case "incashlist":
                    dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId=" + good_id + "),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId=" + good_id + ") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
                    if (dt.Select("qty>0").Length > 0)
                    {
                        decimal q = 0;
                        resp = "";

                        foreach (DataRow r in dt.Select("","qty desc, price desc"))
                        {
                            q = cNum.cToDecimal(r["qty"], 2);
                            resp += "" + r["Name"] + ": " + ((q == 0) ? "-" : "<b>" + q + "</b>") + "<br/>";
                        }

                    }
                    else
                    {
                        resp = "в подразделениях нет остатков";
                    }
                    break;
                case "isys":
                    resp = prepareISys(context.Request["code"]);

                    break;
                case "angood":
                    int ownerid = cNum.cToInt(context.Request["own"]);
                    string code = context.Request["code"];
                    string antype = context.Request["antype"];
                    string typecen = context.Request["typecen"];
                    if (ownerid == 0) ownerid = iam.OwnerID;

                    resp = prepareAngoodList(code, antype, typecen,ownerid);
                    break;
                default:
                    resp = "unknown act: " + act;
                    break;
            }
            context.Response.Write(resp);
        }

        private string prepareISys(string code)
        {
            //string resp = "";
            DataTable dt = db.GetDbTable("select isnull(isys,'') as isys from good where id in (select top 1 goodid from owng where goodcode='"+code+"')");
            return (dt.Rows.Count>0)?""+dt.Rows[0][0]:""; 

        }

        public static string prepareAngoodList(string code,string antype, string typecen, int ownerId)
        {
            string resp = "";
            DataTable dt = db.GetDbTable("select ang.antype, g.goodId, g.goodCode, g.Name,g.brend,g.qty, g."+typecen+" as price, g.ed, g.zn, g.zn_z,g.ens from vGood" + ownerId + " as g inner join ANGOOD as ang on g.goodcode=ang.ancode and antype in ("+antype+") and ang.goodcode='" + code + "'");
            foreach (DataRow r in dt.Select("price>0", "qty desc, price desc"))
            {
                resp += String.Format("<div class='small ipad' title='{3}'><a href='javascript:return 0;' onclick=\"openflywin('../good/card.aspx?id={3}&ownid={4}&typecen="+typecen+"', 500,600,'{0}')\">{0}</a><br/>цена "+GetNameCen(typecen)+" <strong>{1}</strong>руб.<br/>тек. остаток <strong>{2}</strong>шт.</div>", r["Name"], cNum.cToDecimal(r["price"], 2), r["qty"], r["goodid"], ownerId);
            }
            return resp;
        }

        public static string GetNameCen(string typecen)
        {
            switch (typecen)
            {
                case "pr_opt":
                    return "оптовая";
                case "pr_kropt":
                    return "кр. оптовая";
                case "pr_spec":
                    return "специальная";
                case "pr_vip":
                    return "VIP";
                case "pr_ngc":
                    return "НГЦ";
                case "pr_spr":
                    return "cуппрайс с НДС";
                case "pr_b":
                default:
                    return "базовая";
            }
        }

        public static string prepareIncashTable(int goodId, bool smalltable=true)
        {
            DataTable dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId=" + goodId + "),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId=" + goodId + ") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
            string resp = "";
            if (dt.Select("qty>0").Length > 0)
            {
                resp = "<table class='incashdetail'><caption class='bold'>Своб. ост. в подразделениях</caption>";
                resp += "<tr>";
                foreach (DataRow r in dt.Rows)
                {
                    if (smalltable)
                    resp += "<td title='" + r["Name"] + "'>" + getShortName("" + r["OwnerID"]) + "</td>";
                    else
                    {
                        resp += "<td >" + r["Name"] + "</td>";
                    }
                }
                resp += "</tr>";
                resp += "<tr>";
                decimal q = 0;
                foreach (DataRow r in dt.Rows)
                {
                    q = cNum.cToDecimal(r["qty"]);
                    resp += "<td title='" + r["lcd"] + "'>" + ((q == 0) ? "" : "" + q) + "</td>";
                }
                resp += "</tr>";
                resp += "</table>";
            }
            else
            {
                resp = "<table class='incashdetail'><tr><td>в подразделениях нет остатков</td></tr></table>";
            }
            return resp;
        }


        private static string getShortName(string p)
        {
            switch (p)
            {
                case "100004":
                    return "Сург";
                case "100003":
                    return "Тюм";
                case "100002":
                    return "Таг";
                case "100001":
                    return "Челяб";
                case "100000":
                default:
                    return "Екб";

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