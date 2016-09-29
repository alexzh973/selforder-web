using System;
using ensoCom;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Collections;

namespace wstcp.Models
{
    public class NSIGood
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public string Article { get; set; }
        
        public NSIGood(int id)
        {
            this.ID = id;
            if (__load(this) != db.DbResult.OK)
            {
                this.Name = "";
                this.Descr = "";
                this.Article = "";
                //this.Category = "";
            }
        }
        public NSIGood(int owner_id, string owner_good_code)
            : this(GetID(owner_id, owner_good_code))
        {
            
        }
        
        public static int GetID(int owner_id, string owner_good_code)
        {
            int result=0;
            string s_zn="",s_cat="";
            string str = "select GoodId from OWNG where OwnerId=" + owner_id+" and GoodCode='"+owner_good_code+"'";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = cNum.cToInt(r["GoodId"]);
                    //s_zn = "" + r["zn"];
                    //s_cat = "" + r["category"];

                }                
            }
            catch { }
            finally { cn.Close(); }
            //if (zn != "" || category != "")
            //{
            //    if (s_zn != zn || s_cat != category && result > 0)
            //    {
            //        db.ExecuteCmd("update " + NSImeta.TDB_OWNERSGOODIES + " set zn='" + ((zn != "") ? zn : s_zn) + "', category='" + ((category != "") ? category : s_cat) + "' where OwnerId=" + owner_id + " and GoodCode='" + owner_good_code + "'");
            //    }
            //}
            return result;   
        }


        //public db.DbResult GoodCodeAdd(int owner_id, string owner_good_code, string zn, string category)
        //{
        //    if (owner_good_code == "" || owner_id<1 || ID<1) return db.DbResult.ERROR_EMPTYFIELD;
        //    ensoCom.db.DbResult result = db.DbResult.ERROR;
        //    SqlConnection cn = new SqlConnection(db.DefaultCnString);
        //    SqlCommand cmd = cn.CreateCommand();
        //    cn.Open();
            
        //    try
        //    {
        //        cmd.Parameters.AddWithValue("@GoodId", ID);
        //        cmd.Parameters.AddWithValue("@OwnerId", owner_id);
        //        cmd.Parameters.AddWithValue("@GoodCode", owner_good_code);
        //        cmd.Parameters.AddWithValue("@State", "");
        //        cmd.Parameters.AddWithValue("@zn", zn);
        //        cmd.Parameters.AddWithValue("@category", category);
        //        cmd.Parameters.AddWithValue("@Corrector", ""+owner_id);

        //        cmd.CommandText = "update OWNG set GoodCode=@GoodCode,zn=@zn,category=@category, State=@State, lc=@Corrector, lcd=getdate() where GoodId=@GoodId and OwnerId=@OwnerId";
        //        if (cmd.ExecuteNonQuery() == 0)
        //        {
        //            cmd.CommandText = "insert into OWNG (GoodId, OwnerId, GoodCode,zn,category,State, lc, lcd)" +
        //                " values (@GoodId, @OwnerId, @GoodCode,@zn,@category,@State, @Corrector, getdate())";
        //            cmd.ExecuteNonQuery();                    
        //        }
        //        result = ensoCom.db.DbResult.OK;
        //    }
        //    catch 
        //    {
                
        //    }
        //    finally
        //    {
        //        cn.Close();
        //    }           
        //    return result; 
        //}

        //public void GoodCodeRemove(int owner_id)
        //{
        //    db.ExecuteCmd("update " + NSImeta.TDB_OWNERSGOODIES + " set State='D' where GoodId=" + ID + " and OwnerId=" + owner_id); 
        //}
        
        
        


        private static ensoCom.db.DbResult __load(NSIGood obj)
        {
            ensoCom.db.DbResult result =  db.DbResult.ERROR;
            string str = "select * from "+NSImeta.TDB_GOODIES+" as g where g.ID=" + obj.ID;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    obj.Name = "" + r["Name"];
                    obj.Descr = "" + r["Descr"];
                    obj.Article = "" + r["Article"];
                    //obj.Category = "" + r["Category"];
                    result = db.DbResult.OK;
                }
                else
                {
                    result = db.DbResult.RECORD_NOTEXIST;
                }
            }
            catch {  }
            finally { cn.Close(); }

            return result;
        }
        /*
        public static ensoCom.db.DbResult Save(NSIGood rec, string corrector)
        {
            ensoCom.db.DbResult result =  db.DbResult.ERROR;
            if (rec.Name == "") return db.DbResult.ERROR_EMPTYFIELD;

            if (rec.ID < 1)
            {
                DataTable dt = db.GetDbTable("select id from " + NSImeta.TDB_GOODIES + " where Name='" + rec.Name +((rec.Article!="")? "' or Article='"+rec.Article+"'":""));
                if (dt.Rows.Count > 0)
                {
                    return db.DbResult.ERROR_DUPLICATEFIELD;
                }

            }

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            int tid = rec.ID;
            try
            {
                cmd.Parameters.AddWithValue("@ID", rec.ID);
                cmd.Parameters.AddWithValue("@Name", rec.Name);
                cmd.Parameters.AddWithValue("@Descr", rec.Descr);
                cmd.Parameters.AddWithValue("@Article", rec.Article);
                cmd.Parameters.AddWithValue("@Category", rec.Category);
                cmd.Parameters.AddWithValue("@Corrector", corrector);

                cmd.CommandText = "update " + NSImeta.TDB_GOODIES + " set Name=@Name, Descr=@Descr, Article=@Article,Category=@Category, lc=@Corrector, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + NSImeta.TDB_GOODIES + " (Name, Descr, Article, Category,lc, lcd)" +
                        " values (@Name, @Descr, @Article, @Category, @Corrector, getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + NSImeta.TDB_GOODIES + " where lc=@Corrector";
                    tid = cNum.cToInt(cmd.ExecuteScalar());
                }
                result = (tid > 0)? ensoCom.db.DbResult.OK: db.DbResult.ERROR;
            }
            catch (Exception ex)
            {
                //kjdfh
            }
            finally
            {
                cn.Close();
            }
            if (result== db.DbResult.OK)
            {
                rec.ID = tid;               
            }
            

            return result;
        }
        
      */  
        
    }
    
   
    public struct GoodOwnInfo
    {
        public string Name;
        public string Article;
        public string TN;
        public string TK;
        public string Brend;
        public string Code;
        public string ENS;
        public decimal Qty;
        public string ed;
        public string Zn;
        public string Zn_z;
        public string Zt;
        public decimal Price;
        public decimal PriceB;
        public DateTime lcd;
        
    }
    public abstract class GoodInfo
    {
        internal static Hashtable __structdt = new Hashtable();

        public static DataTable DT_StructTN(int owner_id)
        {            
                if (!__structdt.ContainsKey(owner_id) )__structdt[owner_id] = db.GetDbTable("select xTN, count(id) from GOOD where id in (select goodid from OWNG where OwnerID=" + owner_id + ") group by xTN order by xTN");
                return (DataTable)__structdt[owner_id];            
        }

        public static GoodOwnInfo GetInfo(int id,int owner_id)
        {
            GoodOwnInfo go;// = new GoodOwnInfo();
            string str = (owner_id>0)? "select good.id, good.ens, good.Article, good.Name, good.xTN, good.xTK, good.Brend, g.GoodCode, good.ed, g.Qty, g.zn, g.zn_z,g.zt, g.pr_spr,g.pr_b, g.lcd from OWNG as g inner join GOOD on good.id=g.GoodID where g.GoodID=" + id+" and g.OwnerId="+owner_id +" ":
                "select good.id, good.ens,good.Article, good.Name, good.xTN, good.xTK, good.Brend, good.ed, (SELECT TOP (1) GoodCode AS Expr1 FROM OWNG  WHERE (GoodId = GOOD.ID) AND (ISNULL(GoodCode, '') <> '')  ORDER BY lcd DESC) AS  GoodCode, isnull(sum(g.Qty),0) as Qty, '' as zn, '' as zn_z,'' as zt, isnull(max(g.pr_spr),0) as pr_spr,isnull(max(g.pr_b),0) as pr_b, max(g.lcd) as lcd from GOOD inner join OWNG as g on good.id=g.GoodID where good.ID=" + id + " group by good.ENS, good.ID,good.Article, good.Name,good.xTN,good.xTK, good.Brend, good.ed";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    go = new GoodOwnInfo()
                    {
                        ENS = "" + r["ENS"],
                        Name = "" + r["Name"],
                        Article = "" + r["Article"],
                        TN = "" + r["xTN"],
                        TK = "" + r["xTN"],
                        ed = "" + r["ed"],
                        Brend = "" + r["Brend"],
                        Code = "" + r["GoodCode"],
                        Zn = "" + r["zn"],
                        Zn_z = "" + r["zn_z"],
                        Zt = "" + r["zt"],
                        Price = cNum.cToDecimal("" + r["pr_spr"]),
                        PriceB = cNum.cToDecimal("" + r["pr_b"]),
                        Qty = cNum.cToDecimal("" + r["qty"]),
                        lcd = cDate.cToDate(r["lcd"])
                    };
                }
                else
                {
                    go = new GoodOwnInfo()
                    {
                        ENS = "",
                        Name = "Not found",
                        Article = "",
                        Brend = "",
                        TN = "",
                        TK = "",
                        ed = "",
                        Code = "",
                        Price = 0,
                        PriceB = 0,
                        Qty = 0,
                        Zn = "",
                        Zn_z = "",
                        Zt = "",
                        lcd = cDate.DateNull
                    };
                }
            }
            catch (Exception ex)
            {
                go = new GoodOwnInfo()
                {
                    ENS = "",
                    Name = ex.ToString(),
                    Article = "",
                    Brend = "",
                    TN = "",
                    TK = "",
                    ed = "",
                    Code = "",
                    Price = 0,
                    PriceB = 0,
                    Qty = 0,
                    Zn = "",
                    Zn_z = "",
                    Zt = "",
                    lcd = cDate.DateNull
                };
            }
            finally { cn.Close(); }
            return go;
        }
        public static GoodOwnInfo GetInfo(string code,int owner_id)
        {
            GoodOwnInfo go;// = new GoodOwnInfo();
            string str = (owner_id>0)? "select good.Article, good.ens,good.Name,good.xTN, good.xTK, good.ed, good.Brend, g.GoodCode, g.Qty, g.zn, g.zn_z, g.zt, g.pr_spr, g.lcd from OWNG as g inner join GOOD on good.id=g.GoodID where g.GoodCode=" + code+" and g.OwnerId="+owner_id +" ":
                "select good.Article, good.ens, good.Name,good.xTN, good.xTK, good.ed, good.Brend, '' as GoodCode, isnull(sum(g.Qty),0) as Qty, '' as zn, '' as zn_z, '' as zt, isnull(max(g.pr_spr),0) as pr_spr, max(g.lcd) as lcd from GOOD inner join OWNG as g on good.id=g.GoodID where g.GoodCode=" + code + " group by good.ID,good.Article, good.Name,good.xTN, good.xTK, good.ed, good.Brend";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    go = new GoodOwnInfo()
                    {
                        Name = "" + r["Name"],
                        Article = "" + r["Article"],
                        TN = "" + r["xTN"],
                        TK = "" + r["xTN"],
                        ed = "" + r["ed"],
                        Brend = "" + r["Brend"],
                        Code = "" + r["GoodCode"],
                        Zn = "" + r["zn"],
                        Zn_z = "" + r["zn_z"],
                        Zt = "" + r["zt"],
                        ENS = "" + r["ens"],
                        Price = cNum.cToDecimal("" + r["pr_spr"]),
                        Qty = cNum.cToDecimal("" + r["qty"]),
                        lcd = cDate.cToDate(r["lcd"])
                    };
                }
                else
                {
                    go = new GoodOwnInfo()
                    {
                        Name = "Not found",
                        Article = "",
                        Brend = "",
                        TN = "",
                        TK = "",
                        ed = "",
                        Code = "",
                        Price = 0,
                        Qty = 0,
                        Zn = "",
                        Zn_z = "",
                        Zt = "",
                        ENS = "",
                        lcd = cDate.DateNull
                    };
                }
            }
            catch (Exception ex)
            {
                go = new GoodOwnInfo()
                {
                    Name = ex.ToString(),
                    Article = "",
                    Brend = "",
                    TN = "",
                    TK = "",
                    ed = "",
                    Code = "",
                    Price = 0,
                    Qty = 0,
                    Zn = "",
                    Zn_z = "",
                    Zt = "",
                    lcd = cDate.DateNull
                };
            }
            finally { cn.Close(); }
            return go;
        }
        public static List<wstcp.goodsx.GI> wGetIncash(string listgoodcodes)
        {

            string lg = "'" + listgoodcodes.Replace((char)160, ' ').Replace(" ", "").Replace(";", "','") + "'";
            DataTable dt = db.GetDbTable("SELECT owng.OwnerId, OWN.Name as OwnerName, OWNG.GoodCode, ISNULL(OWNG.zn, 'S') AS zn, ISNULL(OWNG.zn_z, 'D0') AS zn_z, isnull(owng.zt,'') as zt, ISNULL(OWNG.pr_spr, 0) AS SuppriceWnds, ISNULL(OWNG.pr_b, 0) AS BasePrice, ISNULL(OWNG.qty,0) AS Qty FROM OWNG" +
                " INNER JOIN OWN ON OWNG.OwnerId = OWN.ID" +
                          " WHERE OWNG.GoodCode in (" + lg + ")");
            dt.TableName = "INCASH";
            List<wstcp.goodsx.GI> list = new List<wstcp.goodsx.GI>();
            foreach (DataRow r in dt.Select("", "GoodCode,OwnerName"))
            {
                list.Add(new wstcp.goodsx.GI() { OwnerId = cNum.cToInt(r["OwnerId"]), Owner = "" + r["OwnerName"], GoodCode = "" + r["GoodCode"], zn = "" + r["zn"], zn_z = "" + r["zn_z"], zt="" + r["zt"], qty = cNum.cToInt(r["qty"]) });
            }
            return list;
        }

        public static DataTable Get_OwnerGoodCodes(int good_id)
        {
            DataTable dt = db.GetDbTable("select OwnerId, GoodCode, State from OWNG where GoodId=" + good_id);
            dt.TableName = "TABLEGOODCODES";
            return dt;
        }

        public static DataTable Get_GoodesByOwner_special(int owner_id, string where_condition = "")
        {
            DataTable dt;
            if (owner_id < 1)
                dt = db.GetDbTable("select " + ((where_condition == "") ? "top 500" : "") + " *,'' as GoodCode from GOOD " + ((where_condition == "") ? "" : " where " + where_condition));
            else
                dt = db.GetDbTable("select " + ((where_condition == "") ? "top 500" : "") + " g.*, og.GoodCode,og.zn_z, og.zt, og.pr_spr from GOOD as g inner join OWNG as og on og.GoodId=g.ID and og.OwnerId=" + owner_id + " and og.State<>'D'" + ((where_condition == "") ? "" : " where " + where_condition));
            return dt;
        }

        //public static DataTable GetCategories()
        //{
        //    return db.GetDbTable("select category, count(ID) as qty from GOOD where category <>'' group by category");
        //}
        public static DataTable GetBrends()
        {
            return db.GetDbTable("select brend, count(ID) as qty from GOOD where brend<>'' group by brend");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner_id"></param>
        /// <param name="where_condition"></param>
        /// <returns>ID,Article,GoodCode,ens,Name,Descr,State,lc,qtylcd, qty, qtyother, zn_z, is_z, is_p</returns>

        private static Hashtable GOODSDT = new Hashtable();
        public static DataTable Get_GoodesByOwner(int owner_id, string where_condition="", string where_z_condition="")
        {            
            DataTable dt;
            if (GOODSDT.ContainsKey(owner_id) )
                dt = (DataTable)GOODSDT[owner_id];
            else{
                if (owner_id < 1)
                {
                    dt = db.GetDbTable("select tt.* from (select ggg.*, ggg.qty as qtyother from (select distinct " + ((where_condition + where_z_condition == "2=2") ? "top 100" : "") + " GOOD.ID, GOOD.ed, GOOD.ENS,GOOD.Article, GOOD.Name, GOOD.Descr, GOOD.xTN, GOOD.xTK, GOOD.xName, GOOD.Brend, owng.zn, owng.zt, owng.GoodCode,(SELECT SUM(Qty) AS Expr1 from owng AS inc WHERE (GoodId = good.ID)) AS qty,(SELECT max(lcd) AS Expr1 from owng AS inc WHERE (GoodId = good.ID)) AS qtylcd, '' as zn_z, (select max(pr_b) as expr1 from OWNG where goodId=good.id) as pr_b,(select max(pr_spr) as expr1 from OWNG where goodId=good.id) as pr_spr,(SELECT COUNT(OwnerId) AS gg FROM OWNG AS OWNG_1 WHERE (GoodId = GOOD.ID) AND (zn_z = 'NL')) AS is_nl,(SELECT COUNT(OwnerId) AS gg FROM OWNG AS OWNG_1 WHERE (GoodId = GOOD.ID) AND (zn_z in ('P2','PZ'))) AS is_p from GOOD left join (SELECT GoodCode, goodid FROM OWNG WHERE ISNULL(GoodCode, N'') <> '') as owng on owng.goodid=good.id " + ((where_condition == "") ? "" : " where " + where_condition) + ") as ggg) as tt " + ((where_z_condition != "") ? " where " + where_z_condition : ""));
                }
                else
                {
                    dt = db.GetDbTable("select ggg.* from (select " + ((where_condition + where_z_condition == "1=1 1=1") ? "top 100" : "") + " GOOD.ID, GOOD.ENS, GOOD.Article, GOOD.ed, GOOD.Name, GOOD.Descr,GOOD.xTN, GOOD.xTK, GOOD.xName, GOOD.Brend, og.zn, og.zt, og.GoodCode,og.Qty, (SELECT SUM(Qty) AS Expr1 from owng AS inc WHERE (GoodId = good.ID) and OwnerID<>" + owner_id + ") AS qtyother,og.lcd AS qtylcd,og.zn_z,isnull(og.pr_b,0) as pr_b, isnull(og.pr_spr,0) as pr_spr,(SELECT COUNT(OwnerId) AS gg FROM OWNG AS OWNG_1 WHERE (GoodId = GOOD.ID) AND (zn_z = 'NL')) AS is_nl,(SELECT COUNT(OwnerId) AS gg FROM OWNG AS OWNG_1 WHERE (GoodId = GOOD.ID) AND (zn_z in ('P2','PZ'))) AS is_p from GOOD inner join OWNG as og on og.GoodId=good.ID and og.OwnerId=" + owner_id + " and og.State<>'D'" + ((where_condition == "") ? "" : " where " + where_condition) + ") as ggg " + ((where_z_condition != "") ? " where " + where_z_condition : ""));
                }
                //GOODSDT[owner_id] = dt;
            }
            dt.TableName = "TABLEGOODCODES";
            return dt;
       }


        public static DataTable GetIncashTable(string goodcode)
        {
            DataTable dt = db.GetDbTable("SELECT owng.OwnerId, OWN.Name as OwnerName, OWNG.GoodCode, ISNULL(OWNG.zn, 'S') AS zn, ISNULL(OWNG.zn_z, 'D0') AS zn_z, ISNULL(OWNG.zt, '') AS zt,ISNULL(OWNG.pr_spr, 0) AS SuppriceWnds,ISNULL(OWNG.pr_b, 0) AS BasePrice, ISNULL(owng.qty,0) AS Qty, owng.lcd FROM OWNG" +
                " INNER JOIN OWN ON OWNG.OwnerId = OWN.ID" +
                          " WHERE OWNG.GoodCode='" + goodcode + "'");
            dt.TableName = "INCASH";
            return dt;
            //if (dt.Rows.Count == 0 || dt.Columns.Count)
            //{

            //}
        }
        public static DataTable GetIncashTable(int goodid, int owner_id=0)
        {
            DataTable dt = db.GetDbTable("SELECT owng.OwnerId, OWN.Name as OwnerName, OWNG.GoodCode, ISNULL(OWNG.zn, 'S') AS zn, ISNULL(OWNG.zn_z, 'D0') AS zn_z,ISNULL(OWNG.zt, '') AS zt, ISNULL(OWNG.pr_spr, 0) AS SuppriceWnds,ISNULL(OWNG.pr_b, 0) AS BasePrice, ISNULL(OWNG.qty,0) AS Qty, OWNG.lcd FROM OWNG" +
                " INNER JOIN OWN ON OWNG.OwnerId = OWN.ID" +
                          " WHERE OWNG.GoodId=" + goodid + " "+((owner_id>0)? " and owng.ownerid="+owner_id:""));
            dt.TableName = "INCASH";
            return dt;
        }


    }
}