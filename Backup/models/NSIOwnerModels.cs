using System;
using ensoCom;
using System.Data.SqlClient;
using System.Data;
namespace wstcp.Models
{
    public class NSIOwner
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Adrs { get; set; }
        public NSIOwner(int id)
        {
            ID = id;
            if (__load(this) != db.DbResult.OK)
            {
                Name = "Сантехкомплект-Урал";
                Adrs = "";
            }
        }


        private static ensoCom.db.DbResult __load(NSIOwner obj)
        {
            ensoCom.db.DbResult result = db.DbResult.ERROR;
            string str = "select * from " + NSImeta.TDB_OWNERS + " where ID=" + obj.ID;
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
                    obj.Adrs = "" + r["Adrs"];
                    result = db.DbResult.OK;
                }
                else
                {
                    result = db.DbResult.RECORD_NOTEXIST;
                }
            }
            catch { }
            finally { cn.Close(); }

            return result;
        }

        public static ensoCom.db.DbResult Save(NSIOwner rec, string corrector)
        {
            ensoCom.db.DbResult result = db.DbResult.ERROR;
            if (rec.Name == "") return db.DbResult.ERROR_EMPTYFIELD;

            if (rec.ID < 1)
            {
                DataTable dt = db.GetDbTable("select id from "+NSImeta.TDB_OWNERS+" where Name='" + rec.Name + "'");
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
                cmd.Parameters.AddWithValue("@Adrs", rec.Adrs.Replace(" ",""));
                cmd.Parameters.AddWithValue("@Corrector", corrector);

                cmd.CommandText = "update " + NSImeta.TDB_OWNERS + " set Name=@Name, Adrs=@Adrs,lc=@Corrector, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + NSImeta.TDB_OWNERS + " (Name, Adrs,lc, lcd)" +
                        " values (@Name, @Adrs,@Corrector, getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + NSImeta.TDB_OWNERS + " where lc=@Corrector";
                    tid = cNum.cToInt(cmd.ExecuteScalar());
                }
                result = (tid > 0) ? ensoCom.db.DbResult.OK : db.DbResult.ERROR;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                cn.Close();
            }
            if (result == db.DbResult.OK)
            {
                rec.ID = tid;
            }


            return result;
        }


        public static DataTable GetTable()
        {
            DataTable dt = db.GetDbTable("select id,Name,Descr,State,lc,lcd,Access,Adrs from " + NSImeta.TDB_OWNERS);
            dt.TableName = "TABLEOWNERS";
            return dt;
        }

        
    }

}