using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using ensoCom;

namespace wstcp
{
    public abstract class pUserInfo
    {

        public static bool Exist(string emailorname)
        {
            string checkval = emailorname.Trim().ToLower();
            return (cNum.cToInt(db.GetDbTable("select max(id) from " + pUser.TDB + " where email='" + checkval + "' or name='" + checkval + "'").Rows[0][0]) > 0);
        }


        public static DataTable GetTable(IAM iam, string where)
        {
            return db.GetDbTable("select *, "+ db.SqlNameField(Subject.TDB,"SubjectID","SubjectName") +" from " + pUser.TDB + " where 1=1" + ((where != "") ? " and " + where : ""));
        }

        public static string LastIP(int user_id)
        {
            string ip = "";
            string str = "select top 1 IP from SESUSER where userID = " + user_id;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    ip = "" + r["IP"];
                }
            }
            catch { }
            finally { cn.Close(); }
            return ip;
        }
        
        public static DataTable GetBirthday(DateTime left_date, DateTime right_date, int org_id)
        {
            return db.GetDbTable("select ID from " + pUser.TDB + " where state<>'D' and  CONVERT(datetime, CAST(DAY(Birthday)  AS varchar) + '.' + CAST(MONTH(Birthday) AS varchar) + '.' + CAST(YEAR(GETDATE()) AS varchar), 104) between " + cDate.Date2Sql(left_date) + " and " + cDate.Date2Sql(right_date));
        }


        public static string sqlNameField(string field_source_table, string alias)
        {
            return db.SqlNameField(pUser.TDB, field_source_table, alias);// String.Format("(select uujdhgfu.Name from " + pUser.TDB + " as uujdhgfu where uujdhgfu.id={0}) as {1}", field_source_table, alias);
        }


    }
}
