using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ensoCom;


public abstract class appError
    {
        public static void SaveError(string page, int strnum, string message, string user)
        {
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
                cn.Open();
                SaveError(cmd, page, strnum, message, user);
            }
            catch
            {
            }
            finally
            {
                cn.Close();
            }
        }
        public static void SaveError(SqlCommand cmd, string page, int strnum, string message, string user)
        {
            try
            {
                cmd.CommandText = "insert into errlog (src,msg,lc,lcd) values ('"+page+":"+strnum+"','"+message.Replace("'","~")+"','"+user+"',getdate())";
            }
            catch 
            {
                
                
            }
        }
    }
