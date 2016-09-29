using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using ensoCom;

namespace wstcp
{
	/// <summary>
	/// Summary description for News.
	/// </summary>
	[Serializable]
	public class News : BigObject
	{
        public const string TDB = "NEWS";
        private string _backcounter;
        private string _text;        
        private DateTime _regdate;  
        private DateTime _enddate;       
        private string _published;
        protected string _comment_possible;
        private int _authorID;
        private pUser _author; 
        
		
        public News(int id, IAM user) : base(TDB, id, user)
		{
            
			if( !__load(this) )				
			{
				_text = "";
                Published = false;
                Backcounter = false;
                _comment_possible = "Y";
                RegDate = cDate.TodayD;
                EndDate = RegDate.AddDays(30);
			}
		}
        public DateTime RegDate
        {
            get
            {
                return _regdate;
            }
            set
            {
                _regdate = (value != cDate.DateNull) ? value : _regdate;                
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _enddate;
            }
            set
            {
                _enddate = (value >= _regdate) ? value : _regdate;
            }
        }

       
        public string Text
        {
            get { return _text; }
            set { _text = value.Trim(); }
        }

        public int AuthorID
        {
            get { return _authorID; }
            set { if (_authorID != value) { _authorID = value; _author = null; } }
        }
        public pUser Author
        {
            get { if (_author == null) _author = new pUser(_authorID, CurrentUser); return _author; }
        }

        public bool Published { get { return (_published.ToUpper() == "Y"); } set { _published = (value) ? "Y" : "N"; } }
        public bool Backcounter { get { return (_backcounter.ToUpper() == "Y"); } set { _backcounter = (value) ? "Y" : "N"; } }

        public bool CommentPossible
        {
            get { return (_comment_possible == "Y"); } set { _comment_possible = (value) ? "Y" : "N"; }
        }

		
 
		private static bool __load(News obj)
		{
			bool result = false;
            string str = "select * from " + TDB + "  where ID=" + obj.ID;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = str;
            cn.Open();
            try
            {
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    __readfields(obj, r);
                    
                    obj._text = "" + r["Text"];
                    obj._regdate = cDate.cToDate(r["RegDate"]);
                    obj._enddate = cDate.cToDate(r["EndDate"]);
                    obj._published = ""+r["Published"];
                    obj._comment_possible = "" + r["CommentPossible"];                    
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ErrorStack.AddMessage(ex.ToString());
                result = false;
            }
            finally { cn.Close(); }

			return result;
		}




        public static bool Save(News news)
        {
            bool result = false;            
           
            if (news.ID>0 && !eObject.LockForModify(news, news.CurrentUser)) return result;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            int tid = news.ID;
            try
            {
                __fillcmdparams(cmd, news);
                cmd.Parameters.AddWithValue("@RegDate", (news.RegDate == cDate.DateNull) ? db.dbNull : news.RegDate);
                cmd.Parameters.AddWithValue("@EndDate", (news.EndDate == cDate.DateNull) ? db.dbNull : news.EndDate);
                cmd.Parameters.AddWithValue("@Text", news.Text);
                cmd.Parameters.AddWithValue("@Published", news._published);
                cmd.Parameters.AddWithValue("@CommentPossible", news._comment_possible);
                cmd.Parameters.AddWithValue("@AuthorID", news._authorID);

                tid = _save2bd(cmd, news, new string[] { "RegDate", "EndDate", "Text", "Published", "CommentPossible", "AuthorID" });
                result = (tid > 0);
            }
            catch(Exception ex)
            {
                //ErrorStack.AddMessage(ex.ToString());
                result = false;
            }
            finally
            {
                cn.Close();
            }
            eObject.unlock_by_admin(news.Meta_TDB, news.ID);
            if (result)
            {
               
                news._id = tid;
                
            }
            return result;
        }

        public static bool Delete(int object_id, IAM current_user)
        {
            bool result = false;
            if (object_id > 0)
            {
                return mark2delete(TDB, object_id, current_user);
            }
            return result;
        }


        //public static bool SendToArchive(int object_id, IAM current_user)
        //{
        //    bool result = false;
        //    if (object_id > 0)
        //    {
        //        result = (db.ExecuteCmd("update " + TDB + " set State='A', lc='" + current_user.Name + "', lcd=getdate()  where ID=" + object_id)>0);
        //    }
        //    return result;
        //}

        //public static void SetActual(News news)
        //{
        //    bool result = false;
        //    if (news.ID > 0)
        //    {
        //        result = (db.ExecuteCmd("update " + TDB + " set State='', lc='" + news.CurrentUser.Name + "', lcd=getdate() where ID=" + news.ID)>0);
        //    }
        //    if (result)
        //        news = new News(news.ID, news.CurrentUser);
        //}

	}
}
