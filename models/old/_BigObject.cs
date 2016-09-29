using ensoCom;
using System;
using System.Collections;
using System.Data.SqlClient;

namespace wstcp
{
	/// <summary>
	/// Summary description for BigObject.
	/// </summary>
	[Serializable]
	public abstract class BigObject : eObject
	{
        protected Hashtable _extF = new Hashtable();
        protected IAM _current_user;
        protected string       _lc;
        protected DateTime _lcd;
        protected string       _descr;

   
        public BigObject(string meta_tdb,int id, IAM CurrentUser)
            : base(meta_tdb, id)
		{
			_current_user = CurrentUser;
            if (id <= 0)
            {
                _lc = "";
                _lcd = cDate.DateNull;              
                _descr = "";
                
            }
            
		}
        

        public string  lc{ get { return _lc; } }

        public DateTime lcd { get { return _lcd; } }

		public IAM CurrentUser {	get{return _current_user;}	}
        
        public string Descr
        {
            get { return _descr; }
            set { _descr = (value != null) ? value.Trim() : "";  }
        }

        protected static void __readfields(BigObject obj, SqlDataReader reader)
        {
            obj._name = "" + reader["Name"];
            obj._descr = "" + reader["Descr"];
            obj._state = "" + reader["State"];            
            obj._lc = "" + reader["lc"];
            obj._lcd = cDate.cToDate(reader["lcd"]);
            
        }

        protected static void __fillcmdparams(SqlCommand cmd, BigObject obj)
        {
            cmd.Parameters.AddWithValue("@ID", obj.ID);
            cmd.Parameters.AddWithValue("@Name", obj.Name);
            cmd.Parameters.AddWithValue("@Descr", obj.Descr);            
            cmd.Parameters.AddWithValue("@lc", obj.CurrentUser.Name);
            cmd.Parameters.AddWithValue("@State", obj.State);            
        }

        protected static string __fields_4update
        {
            get { return "Name=@Name, Descr=@Descr, lc=@lc, lcd=getdate(),State=@State "; }
        }
        protected static string __fields_4insert
        {
            get { return "Name, Descr, lc, lcd, State "; }
        }
        protected static string __values_4insert
        {
            get { return "@Name, @Descr, @lc, getdate(), @State "; }
        }

        protected static int _save2bd(SqlCommand cmd, BigObject obj, string[] list_fields)
        {
            string[] p = new string[list_fields.Length];
            for (int i = 0; i < list_fields.Length; i++)
            {
                p[i] = "@" + list_fields[i];
            }
            return _save2bd(cmd, obj, list_fields, p);
        }

        protected static int _save2bd(SqlCommand cmd, BigObject obj, string[] list_fields, string[] list_params)
        {
            if (list_fields.Length != list_params.Length) return -1;
            int tid = obj.ID;
            string
                s1 = __fields_4update,
                s2 = __fields_4insert,
                s3 = __values_4insert;

            for (int i = 0; i < list_fields.Length; i++)
            {
                cStr.Add(ref s1, list_fields[i] + "=" + list_params[i]);
                cStr.Add(ref s2, list_fields[i]);
                cStr.Add(ref s3, list_params[i]);
            }
            cmd.CommandText = "update " + obj.Meta_TDB + " set " + s1 + " where id=@ID";
            if (cmd.ExecuteNonQuery() == 0)
            {
                cmd.CommandText = "insert into " + obj.Meta_TDB + " (" + s2 + ") values ( " + s3 + ")";
                cmd.ExecuteNonQuery();
                tid = __get_last_new_id(cmd, obj.Meta_TDB);
            }

            return tid;
        }
        protected static int __get_last_new_id(SqlCommand cmd, string meta_tdb)
        {

            cmd.CommandText = "select max(ID) from " + meta_tdb + " where lc=@lc";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        
        internal static bool mark2delete(string meta_tdb, int record_id, IAM current_user)
        {
            bool result = false;
            if (record_id > 0 && meta_tdb.Length>2)
            {
                return (db.ExecuteCmd("update " + meta_tdb + " set State='D', lc='" + current_user.Name + "', lcd=getdate() where ID=" + record_id) > 0);
            }
            return result;
        }
	}
}
