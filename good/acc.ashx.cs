using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using selforderlib;
using wstcp.models;
using wstcp.order;

namespace wstcp.good
{
    /// <summary>
    /// Сводное описание для acc
    /// </summary>
    public class acc : IHttpHandler
    {
        private IAM iam;
        public void ProcessRequest(HttpContext context)
        {
            string res = "";
            string sid = context.Request["sid"];
            iam = IamServices.GetIam(sid);
            if (iam.ID < 100000)
            {
                context.Response.Write("access denide");
                return;
            }
            string act = context.Request["act"];
            int accId = cNum.cToInt(context.Request["accid"]);
            int goodId = cNum.cToInt(context.Request["gid"]);
            switch (act)
            {
                case "add":
                    addItem(accId, goodId, iam);
                    res = "add ok";
                    break;
                case "rem":
                    remItem(accId, goodId, iam);
                    res = "remove ok";
                    break;
                default:
                    res = "unknown act " + act;
                    break;
            }
            context.Response.Write(res);
        }

        private void addItem(int accId, int goodId, IAM iam)
        {
           
            SaleAcc acc = new SaleAcc();
            if (iam.CurrentObject==null )
            {
                SaleAcc.Load(acc, accId, iam);
            }
            else
                acc = (SaleAcc)iam.CurrentObject;

            if (acc.Items.Find(x => x.GoodId == goodId) == null)
            {
                GoodOwnInfo gi = GoodInfo.GetInfo(goodId, acc.OwnerID, acc.PriceType);
                acc.Items.Add(new SaleAccItem()
                {
                    GoodId = gi.GoodId,
                    NameGood = gi.Name,
                    GoodCode = gi.Code,
                    Price = gi.PriceSale,
                    QtyIncash = gi.Qty
                });
            }
            iam.CurrentObject = acc;
        }

        private void remItem(int accId, int goodId, IAM iam)
        {
            if (db.ExecuteCmd("delete from ACCGOOD where GoodId=" + goodId+" and accId="+accId)>0)
                iam.CurrentObject = null;
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