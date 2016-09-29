using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ensoCom;

using selforderlib;

namespace wstcp.models
{
    public class SaleAccItem
    {
        //public int OwnerID { get; set; }
        public int GoodId { get; set; }
        public string GoodCode { get; set; }

        public string NameGood { get; set; }
        public decimal QtyIncash { get; set; }
        public decimal Price { get; set; }
        public string img { get; set; }
        public string ens { get; set; }
        public decimal SaleKrat { get; set; }
    }

    public class SaleAcc
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string PriceType { get; set; }
        public string RulePrice { get; set; }
        public int OwnerID { get; set; }

        public List<SaleAccItem> Items = new List<SaleAccItem>();

        public static void Load(SaleAcc acc, int id, selforderlib.IAM iam)
        {

            string str = "select t1.*,case when banimg is null then 0 else t1.id end as img from acc as t1 where ownerId=" + iam.OwnerID + " and id=" + id + "";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    acc.ID = cNum.cToInt(r["ID"]);
                    acc.Code = "" + r["Code"];
                    acc.Name = "" + r["Name"];
                    acc.Descr = "" + r["Descr"];
                    acc.StartDate = cDate.cToDate(r["StartDate"]);
                    acc.FinishDate = cDate.cToDate(r["FinishDate"]);
                    acc.OwnerID = cNum.cToInt(r["OwnerID"]);
                    acc.PriceType = "" + r["PriceType"];
                    acc.RulePrice = "" + r["RulePrice"];
                }
                else
                {
                    acc.ID = 0;
                    acc.Name = "";
                    acc.StartDate = cDate.TodayD;
                    acc.FinishDate = cDate.TodayD;
                    acc.OwnerID = iam.OwnerID;
                    acc.PriceType = "pr_opt";
                    acc.RulePrice = "";
                }

            }
            catch (Exception ex)
            {

            }
            finally { cn.Close(); }
            if (acc.ID > 0)
            {
                DataTable dt = db.GetDbTable("select t2.goodId,t2.goodcode, t2.Name,case when t1.Price<=0 then t2.pr_opt else t1.price end as Price, t2.qty, t2.img, t2.ens, t2.SaleKrat from accgood t1 inner join vGood" + acc.OwnerID + " as t2 on t1.goodId=t2.goodId where t1.accId=" + acc.ID);
                acc.Items = dt.AsEnumerable().ToList().ConvertAll(x => new SaleAccItem()
                {
                    GoodId = (int)x.ItemArray[0],
                    GoodCode = (string)x.ItemArray[1],
                    NameGood = (string)x.ItemArray[2],
                    Price = cNum.cToDecimal(x.ItemArray[3]),
                    QtyIncash = cNum.cToDecimal(x.ItemArray[4]),
                    img = ""+x.ItemArray[5],
                    ens = ""+x.ItemArray[6],
                    SaleKrat = cNum.cToDecimal(x.ItemArray[7])
                });
            }

        }

        public static int Save(SaleAcc acc, selforderlib.IAM iam)
        {
            int tid = acc.ID;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            cn.Open();

            try
            {
                SqlCommand cmd = cn.CreateCommand();

                string where = (acc.ID > 0) ? " id=" + acc.ID : " code='" + acc.Code + "' and ownerID=" + acc.OwnerID;

                cmd.CommandText = "update acc set code='" + acc.Code + "', name='" + acc.Name + "', descr='" + acc.Descr + "' ,startdate=" + cDate.Date2Sql(acc.StartDate) + ",finishdate=" + cDate.Date2Sql(acc.FinishDate) + ",pricetype='" + acc.PriceType + "',ruleprice='" + acc.RulePrice + "',lc='" + iam.Name + "',lcd=getdate() where " + where;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into ACC (ownerId,code,name,descr,startdate,finishdate,pricetype,ruleprice,lc,lcd) values (" + acc.OwnerID + ",'" + acc.Code + "','" + acc.Name + "','" + acc.Descr + "'," + cDate.Date2Sql(acc.StartDate) + "," + cDate.Date2Sql(acc.FinishDate) + ",'" + acc.PriceType + "','" + acc.RulePrice + "','" + iam.Name + "',getdate())";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select top 1 id from ACC where lc='" + iam.Name + "' order by lcd desc";
                    tid = cNum.cToInt(cmd.ExecuteScalar());

                }
                if (tid > 0)
                {
                    cmd.CommandText = "update accgood set lc='x' where accid=" + tid;
                    cmd.ExecuteNonQuery();
                    foreach (SaleAccItem item in acc.Items)
                    {
                        cmd.CommandText = "update accgood set price=" + item.Price + ",lc='" + iam.Name + "',lcd=getdate() where accid=" + tid + " and goodid=" + item.GoodId + "";
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            cmd.CommandText = "insert into accgood (accid, goodid,price,lc,lcd) values (" + tid + ", " + item.GoodId + "," + item.Price + ",'" + iam.Name + "',getdate()) ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.CommandText = "delete from accgood where lc='x' and accid=" + tid;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                cn.Close();
            }
            acc.ID = tid;
            return tid;
        }

    }
}