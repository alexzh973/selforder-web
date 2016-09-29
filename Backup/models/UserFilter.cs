using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using ensoCom;

namespace wstcp
{
	/// <summary>
	/// Summary description for Filter.
	/// </summary>
	[Serializable]
	public class UserFilter
	{
		public static bool Save(int UserID, string filterKey, string filterValue)
		{
			bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
			SqlCommand cmd = cn.CreateCommand();
			cmd.Parameters.Add("@UserID",UserID);
			cmd.Parameters.Add("@filterKey",filterKey);
			cmd.Parameters.Add("@filterValue",filterValue);
			try 
			{
				cn.Open();
				cmd.CommandText = "update FILTER_USER set FilterValue=@FilterValue where UserID=@UserID and FilterKey=@filterKey";
				if (cmd.ExecuteNonQuery()==0)
				{
                    cmd.CommandText = "insert into FILTER_USER (UserID,FilterKey,FilterValue) values(@UserID,@FilterKey,@FilterValue)";
					cmd.ExecuteNonQuery();
				}
				result = true;
			}
			catch(Exception e1)
			{
			}
			finally 
			{
				cn.Close();
			}
			return result;
		}
		public static string Load(int UserID, string filterKey)
		{	
			string res = "";
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
			SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select FilterValue  from FILTER_USER where UserID=@UserID and FilterKey=@filterKey";
			cmd.Parameters.Add("@UserID",UserID);
			cmd.Parameters.Add("@filterKey",filterKey);
			cn.Open();
			SqlDataReader r = cmd.ExecuteReader();
			if(r.Read())
			{
				res = ""+r["FilterValue"];
			}
			cn.Close();
			return res;
		}
		public static bool Exist(int UserID, string filterKey)
		{	
			bool res = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
			SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select top 1 FilterValue  from FILTER_USER where UserID="+UserID+" and FilterKey like '"+filterKey+"' and FilterValue!=''";
            //cmd.Parameters.Add("@UserID",UserID);
            //cmd.Parameters.Add("@filterKey",filterKey);
			cn.Open();
			SqlDataReader r = cmd.ExecuteReader();
			if(r.Read())
			{
				res = (""+r["FilterValue"]!="")?true:false;
			}
			cn.Close();
			return res;
		}

	}
}
