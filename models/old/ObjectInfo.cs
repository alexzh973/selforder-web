using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using wstcp.Models;
using ensoCom;

namespace wstcp
{
    public abstract class ObjectInfo
    {
        public static string GetFieldValue(string meta_tdb, int id, string field_name, IAM current_user)
        {
            if (meta_tdb == "" || id <= 0 || field_name == "") return "";
            string ret = "";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "select t." + field_name + " from " + meta_tdb + " as t where t.ID=" + id;

            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    ret = "" + r[0];
                }
            }
            catch
            {

            }
            finally
            {
                cn.Close();
            }
            return ret;
        }

        public static string GetName(int id, string meta_tdb, IAM current_user)
        {
            return GetFieldValue(meta_tdb, id, "Name", current_user);

        }

        public static string GetName(sObject obj, IAM current_user)
        {
            return GetName(obj.ID, obj.Meta_TDB, current_user);
        }


        public static pUser GetAuthor(sObject record, IAM current_user)
        {
            if (record.IsEmpty) return new pUser(current_user.ID, current_user);
            string sql = "";
            switch (record.Meta_TDB)
            {
                case Order.TDB:
                case Comment.TDB:
                    sql = GetFieldValue(record.Meta_TDB, record.ID, "AuthorID", current_user);
                    break;
                default:
                    sql = GetFieldValue(record.Meta_TDB, record.ID, "AuthorID", current_user);
                    break;
            }
            
            try
            {
                return new pUser(cNum.cToInt(sql), current_user); ;
            }
            catch
            {
                return new pUser(current_user.ID, current_user);
            }
        }

    }
}
