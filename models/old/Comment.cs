using System;
using System.Data;
using System.Data.SqlClient;
using ensoCom;
	
namespace wstcp
{
	/// <summary>
	/// Summary description for Note.
	/// </summary>
	[Serializable]
	public class Comment 
	{
        public const string TDB = "CMNT";
        private string _private;
        private string _descr;
        private int _id;
        private int _authorID;
        private DateTime _regDate;
		private sObject _to_object;
		private IAM _current_user;
        private pUser _author;
        private string _state;
		public Comment(int id, IAM user)			
		{
            _id = id;
            _current_user = user;
			if(	!__load(this))
			{
                _descr = "";
                _authorID = _current_user.ID;
                _regDate = DateTime.Today;
				_private="N";
                _state = "";
			}
		}
        public int ID
        {
            get { return _id; }
        }
        public string Descr
        {
            get { return _descr;            }
            set { _descr = value.Trim(); }
        }
        public string State
        {
            get { return _state;            }
            set { _state = value.Trim().ToUpper(); }
        }
        public DateTime RegDate
        {
            get { return _regDate; }
        }
        public int AuthorID
        {
            get { return _authorID; }
            set
            {
                if (_authorID != value)
                {
                    _author = null;
                    _authorID = value;
                }
            }
        }
        public pUser Author
        {
            get
            {
                if (_author == null) _author = new pUser(_authorID, CurrentUser);
                return _author;
            }
        }
       public sObject ToObject
        {
            get { return _to_object; }
        }

		public bool Private
		{
			get{return (_private!="");}
			set{_private=(value)?"Y":"";}
		}

 
        public void Set_ToObject(sObject to_object) 
		{
            _to_object = to_object;
		}

        public IAM CurrentUser
        {
            get { return _current_user; }
        }

		private static bool __load(Comment n)
		{
			bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
			SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select Descr, RegDate,AuthorID, eTable, eTableID,Private, State from " + TDB + " where ID=" + n.ID;
			cn.Open();
			SqlDataReader r = cmd.ExecuteReader();
			if(r.Read())
			{
                n._authorID = cNum.cToInt(r["AuthorID"]);
                n._descr = r["Descr"].ToString();
                n._regDate = cDate.cToDate(r["RegDate"]);
                n._to_object = new sObject(cNum.cToInt(r["eTableID"]), "" + r["eTable"]);                
				n.Private = (""+r["Private"]=="Y");
				result = true;
			}
			cn.Close();
			return result;
		}




        public static DataTable GetTable(sObject main_linked_object, IAM current_user)
        {
            return GetTable(main_linked_object.ID, main_linked_object.Meta_TDB, current_user);
        }

        public static DataTable GetTable(object mainobject_id, object mainobject_meta_tdb, IAM current_user)
        {
            string str = "select NOTE.ID,NOTE.Descr, NOTE.RegDate, NOTE.eTable, NOTE.eTableID,NOTE.Private, NOTE.AuthorID," + pUserInfo.sqlNameField("NOTE.AuthorID", "AuthorName") + ", NOTE.State" +
                " from " + TDB + " as NOTE" +
                " where 1=1 " +
                ((mainobject_meta_tdb != "") ? " and isnull(NOTE.eTable,'')='" + mainobject_meta_tdb + "'" : "") +
                ((cNum.cToInt(mainobject_id) > 0) ? " and isnull(NOTE.eTableID,'')='" + mainobject_id + "'" : "") +
                " and ((isnull(NOTE.Private,'')='Y' and NOTE.AuthorID=" + current_user.ID + ") or (isnull(NOTE.Private,'')<>'Y'))" +
                " order by NOTE.RegDate desc";

            return db.GetDbTable(str);
        }

        public static bool Save(Comment nt)
        {
            bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            int tid = nt.ID;
            try
            {
                cmd.Parameters.AddWithValue("@ID",nt.ID);
                cmd.Parameters.AddWithValue("@Descr", nt.Descr);
                cmd.Parameters.AddWithValue("@AuthorID", nt.AuthorID);
                cmd.Parameters.AddWithValue("@RegDate", nt.RegDate);
                cmd.Parameters.AddWithValue("@State", nt.State);
                cmd.Parameters.AddWithValue("@eTableID", nt.ToObject.ID);
                cmd.Parameters.AddWithValue("@eTable", nt.ToObject.Meta_TDB);
                cmd.Parameters.AddWithValue("@Private", nt._private.ToUpper());
                
                cmd.CommandText = "update "+TDB+" set State=@State, Descr=@Descr,  eTable=@eTable,eTableID=@eTableID,AuthorID=@AuthorID,Private=@Private where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + TDB + " (RegDate,Descr,eTable,eTableID,Private,AuthorID)" +
                        " values (@RegDate,@Descr,@eTable,@eTableID,@Private,@AuthorID)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(ID) as ID from " + TDB + " where Descr=@Descr and AuthorID=@AuthorID";
                    tid = cNum.cToInt("" + cmd.ExecuteScalar());
                }
                nt._id = tid;
                result = true;
            }
            catch (Exception e1)
            {
            }
            finally
            {
                cn.Close();
            }
            nt = new Comment(tid, nt.CurrentUser);
            return result;
        }

        public static void Delete(Comment nt, IAM current_user)
        {
            //bool result = false;
            dbTransaction tx = new dbTransaction();
            SqlCommand cmd = tx.beginTransaction();
            try
            {
                cmd.CommandText = "delete from " + TDB + " where ID='" + nt.ID + "'";
                cmd.ExecuteNonQuery();
                tx.Commit();
                //result = true;
            }
            catch (Exception e1)
            {
                tx.rollbackTransaction();
            }
        }

        public static void toArchive(Comment nt, IAM current_user)
        {
            //bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            SqlTransaction tx = cn.BeginTransaction();
            cmd.Connection = cn;
            cmd.Transaction = tx;
            try
            {
                cmd.CommandText = "update "+TDB+" set State='A' where ID='" + nt.ID + "'";
                cmd.ExecuteNonQuery();
                tx.Commit();
                //result = true;
            }
            catch (Exception e1)
            {
                try { tx.Rollback(); }
                catch (Exception ex) { }
            }
            finally
            {
                cn.Close();
            }
        }



	}
}
