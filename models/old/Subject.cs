using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using ensoCom;
using System.Collections.Generic;


namespace wstcp
{
	/// <summary>
    /// Summary description for Subject.
	/// </summary>
	[Serializable]
	public class Subject : eObject
	{
        public const string TDB = "SUBJ";

        private string _emailtas = "";
        public string Code { get; set; }
        public string INN { get; set; }
        public string EmailTAs { get { return _emailtas; } set { if (_emailtas != value)_tas = null; _emailtas = value; } }			

        public bool IsFolder { get; set; }
        public int ParentID { get; set; }
        private IAM _user;
		public Subject(int id,IAM CurrentUser)
			:base(TDB, id)
		{
            _user = CurrentUser;
            if (!__load(this))
            {
                Code = INN = EmailTAs = "";
                IsFolder = false;
                ParentID = 0;
            }
		}
		
		

		
		private static bool __load(Subject e)
		{
			if(e.ID<=0) return false;
			bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
			cn.Open();	
			SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select * from  " + TDB + " where id = " + e.ID;
			
			SqlDataReader r = cmd.ExecuteReader();
			try
			{
				if (r.Read()) 
				{
                    

					e.Name=(""+r["Name"]).Replace("''","'");
                    e.INN = "" + r["INN"];
                    e.Code = "" + r["Code"];
                    e.EmailTAs = "" + r["EmailTAs"];
                    e.ParentID = cNum.cToInt(r["ParentID"]);
                    e.IsFolder = ("" + r["IsFolder"] == "Y");
                    result = true;
				}
			}
			catch(Exception ex)
			{
				ErrorStack.Messages.Add(ex.Message);
			}
			finally
			{
				cn.Close();
			}
			return result;
		}
        private List<pUser> _tas;
        public List<pUser> TAs
        {
            get {
                if (_tas == null)
                {
                    _tas = new List<pUser>();
                    string tas = "";
                    foreach (string e in EmailTAs.Split(','))
                    {
                        cStr.Add(ref tas, "'" + e + "'");
                    }
                    DataTable dtu = pUserInfo.GetTable(_user, "email in (" + tas + ")");
                    foreach(DataRow r in dtu.Rows)                    
                    {
                        pUser p = new pUser(cNum.cToInt(r["id"]), _user);
                        _tas.Add(p);
                    }
                }
                return _tas;
            }
        }

        public static bool Save(Subject ent, IAM user)
        {
            bool result = false;
            if (ent.Name.Trim() == "") return false;
            if (ent.ID > 0 && !eObject.LockForModify(ent, user)) return result;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            int tid = ent.ID;
            try
            {
                cmd.Parameters.AddWithValue("@ID", ent.ID);
                cmd.Parameters.AddWithValue("@Code", ent.Code);
                cmd.Parameters.AddWithValue("@Name", ent.Name);
                cmd.Parameters.AddWithValue("@INN", ent.INN);
                cmd.Parameters.AddWithValue("@EmailTAs", ent.EmailTAs);
                cmd.Parameters.AddWithValue("@ParentID", ent.ParentID);
                cmd.Parameters.AddWithValue("@IsFolder", ent.IsFolder? "Y" : "N" );
                cmd.Parameters.AddWithValue("@lc", user.Email);
                cmd.CommandText = "update " + TDB + " set Name=@Name, Code=@Code, INN=@INN, EmailTAs=@EmailTAs, IsFolder=@IsFolder, ParentID=@ParentID, lc=@lc, lcd=getdate() where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + TDB + " ( Name, Code, INN, EmailTAs, IsFolder, ParentID, lc, lcd) values (@Name, @Code, @INN, @EmailTAs,@IsFolder, @ParentID, @lc, getdate() )";
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "select max(ID) as ID from " + TDB + " where lc=@lc";
                    tid = Convert.ToInt32("" + cmd.ExecuteScalar());
                }
                result = (tid > 0);
            }
            catch
            {
            }
            finally
            {
                cn.Close();
            }
            eObject.unlock_by_admin(TDB, ent.ID);
            if (result)
                ent._id = tid;
            return result;
        }


        public static int FindByField(string field, string val)
        {
            DataTable dt = db.GetDbTable("select id from " + TDB + " where " + field + "='" + val + "'");
            return (dt.Rows.Count > 0) ? cNum.cToInt(dt.Rows[0][0]) : 0;
        }

	}
}
