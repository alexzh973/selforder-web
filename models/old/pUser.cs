using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using ensoCom;


namespace wstcp
{


    public class pUser : eObject
    {
        public const string TDB = "ENSOUSER";
        private string _email;
        private int _subjectID;
        private int _parentID;
        private string _loginEnabled;
        private string _rightString;
        private string _isfolder;
        private string _psw;
        private string _phones;
        private string _descr;
        

        private bool _emailchanged = false;


        public pUser(int id, IAM current_user) :
            base(TDB, id)
        {
            if (id <= 0 || !__load(this))
            {
                _name = _email = _rightString = _psw = _descr = _phones = "";
                _isfolder = "N";
                _loginEnabled = "N";

                _psw = generate_password(6);

            }
        }

        static string generate_password(int maxlength = 6)
        {
            string ltrs = "0123456789QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
            //char[] chs = ltrs.ToCharArray();
            string ret = "";
            Random rnd = new Random();
            int l = 0;
            for (int i = 0; i <= 6; i++)
            {
                l = rnd.Next(ltrs.Length - 1);
                ret += ltrs.ToCharArray()[l];
            }
            return ret;
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_id > 0 && _email != value) _emailchanged = true;
                _email = ("" + value).Trim(); 
            }
        }
        public int SubjectID
        {
            get { return _subjectID; }
            set { _subjectID = value; }
        }
        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }
        public string Descr
        {
            get { return _descr; }
            set { _descr = value; }
        }
        public string Phones
        {
            get { return _phones; }
            set { _phones = value; }
        }

        public string Psw
        {
            get { return _psw; }

        }

        public bool LoginEnabled
        {
            get { return (_loginEnabled == "Y"); }
            set { _loginEnabled = (value) ? "Y" : "N"; }
        }
        public bool IsFolder
        {
            get { return (_isfolder == "Y"); }
            set { _isfolder = (value) ? "Y" : "N"; }
        }

        public string RightsString
        {
            get { return _rightString; }
            set { _rightString = value.ToUpper().Trim(); }
        }

        public string PhotoSmallPath
        {
            get { return HelpersPath.PersonsAvatarVirtualPath + @"/" + ((webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + @"/" + _id + "s.png")) ? _id + "s.png" : "0s.png"); }
        }
        public string PhotoBigPath
        {
            get { return HelpersPath.PersonsAvatarVirtualPath + @"/" + ((webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + @"/" + _id + "b.png")) ? _id + "b.png" : "0b.png"); }
        }
        internal static bool __check_admin_right(IAM user)
        {
            return (user.ID == 100000 || user.RightString.IndexOf("USERADM") >= 0 || (user.ID < 100004 && user.ID > 100000 && user.RightString.IndexOf("SUPERADM") >= 0));

        }


        private static bool __load(pUser u)
        {
            bool ret = false;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "select *, cast(pass as nvarchar) as psw from " + TDB + " where ID=" + u.ID;
            cn.Open();
            SqlDataReader r = cmd.ExecuteReader();
            try
            {
                if (r.Read())
                {
                    u.Name = "" + r["Name"];
                    u.ParentID = cNum.cToInt(r["ParentID"]);
                    u._email = "" + r["Email"];
                    u.Phones = "" + r["Phones"];
                    u.Descr =  "" + r["Descr"];
                    u.SubjectID = cNum.cToInt(r["SubjectID"]);
                    u._rightString = "" + r["EnsoUserType"];
                    u._loginEnabled = "" + r["LoginEnabled"];
                    u._psw = ("" + r["psw"]).Replace("\0","");
                    ret = true;
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

        public static bool SetNewPassword(pUser forUser, string oldPass, string newPass, IAM current_user)
        {
            bool ret = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            try
            {
                cmd.Parameters.AddWithValue("@ID", forUser.ID);
                cmd.Parameters.AddWithValue("@NewPass",  newPass);
                cmd.Parameters.AddWithValue("@OldPass",  oldPass);
                cmd.CommandText = "update " + pUser.TDB + " set Pass=cast(@NewPass as varbinary) where ID=@ID" + ((__check_admin_right(current_user) || current_user.IsSuperAdmin) ? "" : " and cast(Pass as nvarchar)=@OldPass");
                ret = (cmd.ExecuteNonQuery() > 0);
            }
            catch (Exception ex)
            {
                ret = false;
            }
            finally
            {
                cn.Close();
            }
            if (ret)
            {
                forUser._psw = newPass;
            }
            return ret;
        }
        public static bool Delete(int UserID, IAM current_user)
        {
            if (!__check_admin_right(current_user)) return false;

            string cmd = "update " + TDB + " set State='D' where ID=" + UserID;
            return (db.ExecuteCmd(cmd) > 0);

        }


        public static bool Save(pUser user, IAM curr_user)
        {
            //if ( !__check_admin_right(current)) return false;
            if (!((user.Email.IndexOf("@") >= 1 && user.Email.IndexOf(".") >= 2 && user.Email.Length > 6) || user.Name.Length > 6)) return false;
            if (user.Name == "")
                user.Name = user.Email;


            if (user.ID > 0 && !eObject.LockForModify(user, curr_user)) return false;
            bool result = false;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            int tid = user.ID;
            bool isnew = (tid == 0);
            try
            {
                user._state = "";
                cmd.Parameters.AddWithValue("@ID", user.ID);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@ParentID", user.ParentID);
                cmd.Parameters.AddWithValue("@IsFolder", user._isfolder);
                cmd.Parameters.AddWithValue("@State", user.State);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@SubjectID", user.SubjectID);
                cmd.Parameters.AddWithValue("@Pass", user.Psw);
                cmd.Parameters.AddWithValue("@Phones",  user.Phones);
                cmd.Parameters.AddWithValue("@Descr", user.Descr);
                cmd.Parameters.AddWithValue("@EnsoUserType", user.RightsString);
                cmd.Parameters.AddWithValue("@LoginEnabled", user._loginEnabled);
                cmd.Parameters.AddWithValue("@lc", curr_user.Name);


                cmd.CommandText = "update " + TDB + " set Name=@Name, Email=@Email,Descr=@Descr, Phones=@Phones, State=@State, SubjectID=@SubjectID, EnsoUserType=@EnsoUserType,LoginEnabled=@LoginEnabled, ParentID=@ParentID, IsFolder=@IsFolder where ID=@ID";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into " + TDB + " ( Name,Email,Descr,Phones,State, SubjectID, EnsoUserType,LoginEnabled, Pass, ParentID, IsFolder, lc,lcd) values (@Name, @Email,@Descr,@Phones,@State,@SubjectID, @EnsoUserType, @LoginEnabled, cast(@Pass as varbinary),@ParentID,@IsFolder, @lc, getdate() )";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select max(ID) as ID from " + TDB + " where lc=@lc";
                    tid = Convert.ToInt32("" + cmd.ExecuteScalar());
                }
                result = (tid > 0);
                //if (result && isnew)
                //{
                //    db.ExecuteCmd("update " + pUser.TDB + " set Pass=convert(binary,'" + user.Psw + "') where ID=" + tid);
                //}

            }
            catch (Exception ex)
            {
                result = false;
                ErrorStack.Messages.Add(ex.ToString());
            }
            finally
            {
                cn.Close();
            }
            eObject.unlock_by_admin(TDB, user.ID);

            user._id = tid;
            return result;
        }



        public static int FindByField(string field, string val)
        {
            DataTable dt = db.GetDbTable("select id from " + TDB + " where " + field + "='" + val + "'");
            return (dt.Rows.Count > 0) ? cNum.cToInt(dt.Rows[0][0]) : 0;
        }

        public static void Init1stUser(string email)
        {
            if (cNum.cToInt(db.GetDbTable("select count(id) from " + TDB + "").Rows[0][0]) > 0) return;

            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            try
            {
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@Name", "admin");
                cmd.Parameters.AddWithValue("@lc", "init");
                cmd.Parameters.AddWithValue("@State", "");
                cmd.Parameters.AddWithValue("@IsFolder", "N");
                cmd.Parameters.AddWithValue("@ParentID", db.dbNull);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Pass", "admin");
                cmd.Parameters.AddWithValue("@EnsoUserType", "ADM");
                cmd.Parameters.AddWithValue("@LoginEnabled", "Y");


                cmd.CommandText = "INSERT INTO [" + TDB + "] ( LoginEnabled,EnsoUserType,parentid,isfolder,Name,Pass,email,lc,lcd,State)" +
                     " VALUES (@LoginEnabled,@EnsoUserType,@ParentID,@IsFolder,@Name,cast(@Pass as varbinary),@Email,@lc,GETDATE(),@State)";
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }

        }

        static List<int> _ar;
        public static List<int> PathToCurrent(int current_id, IAM current_user)
        {
            _ar = new List<int>();
            __build_parentpath(current_id, current_user);
            return _ar;
        }

        private static void __build_parentpath(int current_id, IAM current_user)
        {
            DataTable dt = db.GetDbTable("select d.parentid from " + TDB + " as d where state<>'D' and d.id=" + current_id);
            if (dt.Rows.Count > 0)
            {
                int id = cNum.cToInt(dt.Rows[0]["ParentID"]);
                if (id > 0)
                {
                    _ar.Add(id);
                    __build_parentpath(id, current_user);
                }
            }
            else
                return;
        }
    }
}
