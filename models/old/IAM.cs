using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using ensoCom;


namespace wstcp
{

	public class IAM 
	{
        private static Hashtable __curent_users_pool = new Hashtable();
        public Hashtable eSession = new Hashtable();
        
        private int _id;
        private string _name;
        private string _email;
        private string _rightstring;
        private string _sessionID;
        private int _subjectId;
        

        public static void ClearMySession(IAM current_user, string sessionID)
        {
            if (__curent_users_pool[sessionID] != null && ((IAM)__curent_users_pool[sessionID]).ID == current_user.ID)
            {
                __curent_users_pool.Remove(sessionID);
                IAM u = new IAM(0);
                __curent_users_pool[sessionID] = u;
            }
        }

        public static IAM GetMe(string sessionID)
        {
            if (__curent_users_pool[sessionID] != null)
                return (IAM)__curent_users_pool[sessionID];
            else
                return GetGuest(sessionID); 
        }

        public static IAM GetGuest(string sessionID)
        {
            
            IAM u = new IAM(0);
            __curent_users_pool[sessionID] = u;
            return u;
        }

		internal IAM(int id, string name="", string email="", int subjectId=0, string rightString="", string sessionID="") 
 		{			
			if(id>0 && sessionID.Length > 5) 
			{
                _id = id;
                _name = name;
                _email = email;                
                _rightstring = rightString;
                _sessionID = sessionID;
                _subjectId = subjectId;                
			}
			else
			{
				_id=0;
                _name = "guest";
                _sessionID = sessionID;
                _email = "";
                _subjectId = 0;
                _rightstring = "guest";
			}
		}




        public int ID
        {
            get { return _id; }
        }

        public int SubjectID
        {
            get { return _subjectId; }
        }
        public string SessionID
        {
            get { return _sessionID; }
        }
       public string Name
        {
            get { return _name; }
        }
      public string Email
        {
            get { return _email; }
        }

      
        
		public string RightString
		{
            get { return _rightstring; }
		}

        public bool IsSuperAdmin
        {
            get { return (ID == 100000 || RightString.IndexOf("SADM")>=0); }
        }
        
        public bool IsUserAdmin
        {
            get
            {
                return ( ID == 100000 || RightString.IndexOf("UADM")>-1);}
                
        }
      

        public static IAM Login(string email, string password, string sessionID, string IP)
        {
            IAM iam = null;

            iam = IAM.GetMe(sessionID);
            if (iam != null && iam.ID > 0)
            {
                if (iam.Email == email)
                    return iam;
                else
                    iam = null;
            }

            iam = __loginUser(email, password, sessionID, IP);

            if (iam !=null && iam.ID > 0)
            {
                __curent_users_pool[sessionID] = iam;               
            }
            else
            {
                remove_session(sessionID);               
            } 
            return iam;
        }


        private static IAM __loginUser(string emailorid, string Pswd, string sessionID, string IP)
        {
            if (sessionID.Length < 0 || emailorid.Length<6 || Pswd.Length<3) return null;
            int id = 0;
            IAM iam = null;
            DataTable dt = db.GetDbTable("select id,name, email, subjectid, EnsoUserType from " + pUser.TDB + " where (email='"+emailorid+"' or id=" + cNum.cToInt(emailorid) + ") and (convert(nvarchar,pass)='"+Pswd+"' or convert(varchar,pass)='"+Pswd+"') and loginEnabled='Y'");
            if (dt.Rows.Count > 0)
            {
                id = cNum.cToInt(dt.Rows[0]["ID"]);
                iam = new IAM(cNum.cToInt(dt.Rows[0]["ID"]), "" + dt.Rows[0]["name"], "" + dt.Rows[0]["email"], cNum.cToInt(dt.Rows[0]["subjectid"]), "" + dt.Rows[0]["EnsoUserType"], sessionID);
            }
/*
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
               
                cmd.Parameters.AddWithValue("@email",emailorid);
                cmd.Parameters.AddWithValue("@pass", Pswd);
                cmd.Parameters.AddWithValue("@le", "Y");
                cmd.CommandText = "select id,name, email, subjectid, EnsoUserType from " + pUser.TDB + " where (email=@email or id="+cNum.cToInt(emailorid)+") and Pass=convert(binary,@pass) and loginEnabled='Y'";
                cn.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read() && cNum.cToInt(r["ID"]) > 0)
                {
                    id = cNum.cToInt(r["ID"]);
                    iam = new IAM(cNum.cToInt(r["ID"]), "" + r["name"], "" + r["email"], cNum.cToInt(r["subjectid"]), "" + r["EnsoUserType"], sessionID);
                    
                }
            }
            catch
            {
            }
            finally
            {
                cn.Close();
            }
           */

            if (iam != null && iam.ID > 0)
            {
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
                try
                {
                    
                    SqlCommand cmd = cn.CreateCommand();
                    cn.Open();
                    cmd.CommandText = "insert into SESUSER (SessionID,userID,IP) values ('" + sessionID + "','" + iam.ID + "','" + IP + "')";
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                }
                finally
                {
                    cn.Close();
                }
            }
            else
            {
                iam = new IAM(0);
            }

            return iam;
        }
        
        internal static void remove_session(string sessionID)
        {
            __curent_users_pool.Remove(sessionID);
        }

	}
}
