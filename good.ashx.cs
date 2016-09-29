using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using ensoCom;
using System.Data;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для good
    /// </summary>
    public class good : IHttpHandler
    {
        static Hashtable __owncn = new Hashtable();
        public void ProcessRequest(HttpContext context)
        {
            /* good.ashx?oid=100000&act=init
             * good.ashx?sid=sessionID&act=getqty&code=OWNER_GOOD_CODE
             * good.ashx?sid=sessionID&act=getqty&code=OWNER_GOOD_CODE&whid=WAREHOUSE_ID
             * good.ashx?sid=sessionID&act=setqty&code=OWNER_GOOD_CODE&whid=WAREHOUSE_ID&qty=973
             * good.ashx?sid=sessionID&act=setcode&gid=100000&code=OWNER_GOOD_CODE
             * good.ashx?sid=sessionID&act=getlistg
             * good.ashx?sid=sessionID&act=getlisto
             * good.ashx?sid=sessionID&act=getlistw
             */

            string act = "" + context.Request["act"];
            string sid = "" + context.Request["sid"];
            
            if (!__owncn.ContainsKey(sid) && act != "init" )
            {
                context.Response.Write("access denide");
            }
            else if (act == "init" && "" + context.Request["oid"]=="")
            {
                context.Response.Write("access denide");
            }
            else
            {
                int whid;
                int ownerid; 
                string goodcode;
                wstcp.Models.Good g;
                string resp = "";
                switch (act)
                {
                    case "tableincash":
                        //GoodOwnInfo gsumm = GoodInfo.GetInfo(cNum.cToInt(context.Request["goodid"]), 0);
                        resp = "<table>";
                        foreach (DataRow r in wstcp.Models.GoodInfo.GetIncashTable(cNum.cToInt(context.Request["goodid"])).Rows)
                        {
                            resp += "<tr><td>" + r["OwnerName"] + "</td><td>"+r["qty"]+"</td></tr>";
                        }
                        resp += "</table>";
                        break;
                    case "init": // good.ashx?oid=100000&act=init
                        ownerid = cNum.cToInt(context.Request["oid"]);
                        if (meta.CheckDb)
                        {
                            wstcp.Models.Owner o = new Models.Owner(ownerid);
                            __owncn[context.Session.SessionID]=o.ID;
                            resp = (o.ID>0)?context.Session.SessionID:"";
                        }
                        break;
                    case "setcode":
                        // good.ashx?sid=sessionID&act=setcode&gid=100000&code=OWNER_GOOD_CODE
                        ownerid = cNum.cToInt(__owncn[sid]);
                        goodcode = "" + context.Request["code"];
                        g = new Models.Good(cNum.cToInt(context.Request["gid"]));
                        if (ownerid < 1 || goodcode == "" || g.ID < 1)
                            resp = "error";
                        else
                        {
                            resp=(g.GoodCodeAdd(ownerid, goodcode)== db.DbResult.OK)?"":"error";
                        }
                        break;
                    case "setqty":
                        // good.ashx?sid=sessionID&act=setqty&code=OWNER_GOOD_CODE&whid=WAREHOUSE_ID&qty=973
                        whid = cNum.cToInt(context.Request["whid"]);
                        ownerid = cNum.cToInt(__owncn[sid]);
                        goodcode = "" + context.Request["code"];
                        int qty = cNum.cToInt(context.Request["qty"]);
                        g = Models.Good.Get(ownerid, goodcode);
                        if (g.ID < 1)
                        {
                            resp = "";
                        }
                        else
                        {
                            Models.GoodInfo.SetGoodQty(g.ID, ownerid, whid, qty, sid);
                        }
                        break;
                    case "getqty":
                        // good.ashx?sid=sessionID&act=getqty&code=OWNER_GOOD_CODE
                        // good.ashx?sid=sessionID&act=getqty&code=OWNER_GOOD_CODE&whid=WAREHOUSE_ID
                        whid = cNum.cToInt(context.Request["whid"]);
                        ownerid = cNum.cToInt(__owncn[sid]);
                        goodcode = "" + context.Request["code"];
                        g = Models.Good.Get(ownerid, goodcode);
                        if (g.ID < 1)
                        {
                            resp = "0";
                        }
                        if ( whid < 1)
                        {
                            DataTable dt = Models.GoodInfo.GetIncash(cNum.cToInt(context.Request["gid"]), ownerid);
                            foreach (DataRow r in dt.Rows)
                            {
                                resp += "<tr><td>"+r["WarehouseId"]+"</td><td>"+r["WarehouseName"]+"</td><td>"+r["Qty"]+"</td></tr>";
                            }
                        }
                        else
                        {
                            DataTable dt = Models.GoodInfo.GetIncash(cNum.cToInt(context.Request["gid"]), ownerid);
                            foreach (DataRow r in dt.Select("WarehouseId="+whid))
                            {
                                resp =  "" + r["Qty"];
                            }
                        }
                        break;
                    case "getlistg":
                        ownerid = cNum.cToInt(__owncn[sid]);
                        foreach (DataRow r in Models.GoodInfo.Get_Goodes("").Rows)
                            {
                                resp += "<tr><td>" + r["Id"] + "</td><td>" + r["Name"] + "</td><td>" + r["Descr"] + "</td><td>" + r["Article"] + "</td></tr>";
                            }
                        break;
                    case "getlisto":                        
                        foreach (DataRow r in Models.Owner.GetTable().Rows)
                        {
                            resp += "<tr><td>" + r["Id"] + "</td><td>" + r["Name"] + "</td></tr>";
                        }
                        break;
                    case "getlistw":
                        ownerid = cNum.cToInt(__owncn[sid]);
                        foreach (DataRow r in Models.Warehouse.GetTable(ownerid).Rows)
                        {
                            resp += "<tr><td>" + r["Id"] + "</td><td>" + r["Name"] + "</td><td>" + r["ContactInfo"] + "</td><td>" + r["OwnerId"] + "</td></tr>";
                        }
                        break;
                    //case "getincash":
                    //    // good.ashx?act=getincash&sid=sessionid&listg=listgoodcodes
                    //    // return table: goodcode;ownerid;qty;zn_z
                    //    string goodes = ("" + context.Request["listg"]).Replace(";", ",");
                    //    db.GetDbTable();
                    default:
                        resp = "access denide";
                        break;
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(resp);
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