using System;
using ensoCom;
using System.Data.SqlClient;
using System.Data;

namespace wstcp.Models
{
    public class NSIWarehouse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string OwnerCodes { get; set; }
        public string ContactInfo { get; set; }
        public int OwnerID { get; set; }
        
        public NSIWarehouse(int id)
        {
            ID = id;
            if (__load(this) != db.DbResult.OK)
            {
                OwnerID = 0;
                Name = "";
                ContactInfo = "";
                OwnerCodes = "";
            }
        }


        private static ensoCom.db.DbResult __load(NSIWarehouse obj)
        {
            ensoCom.db.DbResult result = db.DbResult.ERROR;
            string str = "select * from " + NSImeta.TDB_WAREHOUSES + " where ID=" + obj.ID;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    obj.OwnerID = cNum.cToInt(r["OwnerId"]);
                    obj.Name = "" + r["Name"];
                    obj.ContactInfo = "" + r["ContactInfo"];
                    obj.OwnerCodes = "" + r["OwnerCodes"];
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

        public static ensoCom.db.DbResult Save(NSIWarehouse rec, string corrector)
        {
            ensoCom.db.DbResult result = db.DbResult.ERROR;
            if (rec.Name == "") return db.DbResult.ERROR_EMPTYFIELD;
            if (rec.ID < 1)
            {
                DataTable dt = db.GetDbTable("select id from " + NSImeta.TDB_WAREHOUSES + " where (Name='" + rec.Name + "' or OwnerCodes='" + rec.OwnerCodes + "') and OwnerId=" + rec.OwnerID);
                if (dt.Rows.Count > 0)
                {
                    //rec = new Warehouse(cNum.cToInt(dt.Rows[0][0]));
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
                cmd.Parameters.AddWithValue("@ContactInfo", rec.ContactInfo);
                cmd.Parameters.AddWithValue("@OwnerCodes", rec.OwnerCodes);
                cmd.Parameters.AddWithValue("@OwnerId", rec.OwnerID);
                cmd.Parameters.AddWithValue("@Corrector", corrector);

                cmd.CommandText = "update " + NSImeta.TDB_WAREHOUSES + " set Name=@Name,OwnerCodes=@OwnerCodes, ContactInfo=@ContactInfo, OwnerId=@OwnerId, lc=@Corrector, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + NSImeta.TDB_WAREHOUSES + " (Name, OwnerCodes,ContactInfo, OwnerId, lc, lcd)" +
                        " values (@Name, @OwnerCodes,@ContactInfo, @OwnerId, @Corrector, getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + NSImeta.TDB_WAREHOUSES + " where lc=@Corrector";
                    tid = cNum.cToInt(cmd.ExecuteScalar());
                    
                }
                result = (tid > 0) ? ensoCom.db.DbResult.OK : db.DbResult.ERROR;
            }
            catch (Exception ex)
            {
                result = db.DbResult.ERROR;
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

        public static DataTable GetTable(int owner_id = 0)
        {
            DataTable dt = db.GetDbTable("select ID,Name, ContactInfo, OwnerID, OwnerCodes from " + NSImeta.TDB_WAREHOUSES + ((owner_id > 0) ? " where OwnerId=" + owner_id : ""));
            dt.TableName = "TABLEWAREHOUSES";
            return dt;
        }
    }
}