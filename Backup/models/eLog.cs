using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using ensoCom;


namespace wstcp
{
    /// <summary>
    /// Summary description for ELOG.
    /// </summary>

    public class eLog
    {
        public struct LogRecord
        {
            internal int
                _entityID;
            internal string _entity, _field, _oval, _nval, _oper, _reason;
            internal int _userid;
            internal DateTime _lastupdate;
            public LogRecord(sObject record, string Field, string OldValue, string NewValue, string Operation, string Reason = "")
            {
                _entity = record.Meta_TDB;
                _entityID = record.ID;
                _field = Field;
                _oval = OldValue;
                _nval = NewValue;
                _oper = Operation;
                _reason = Reason;
                _lastupdate = DateTime.MinValue;
                _userid = 0;
            }
            public int UserId
            {
                get { return _userid; }
            }
            public DateTime LastUpdate
            {
                get { return _lastupdate; }
            }
        }
        private List<LogRecord> _stack;
        public List<LogRecord> Stack
        {
            get { if (_stack == null) _stack = new List<LogRecord>(); return _stack; }
        }

        public void Save(int UserID)
        {
            Save(this, UserID);
        }
        public static void Save(sObject obj, string field, string old_value, string new_value, string operation, string reason, int UserID)
        {
            if (obj.IsEmpty || new_value == old_value) return;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
                cn.Open();
                cmd.CommandText = "insert into ELG (Entity, EntityID, Field, OVal,NVal,UserID,Oper, Reason,lastUpd) values ('" + obj.Meta_TDB + "','" + obj.ID + "','" + field + "', " + ((old_value == "null") ? "null" : "'" + old_value + "'") + ", " + ((new_value == "null") ? "null" : "'" + new_value + "'") + "," + UserID + ",'" + operation + "','" + reason + "',getdate()) ";
                cmd.ExecuteNonQuery();
            }
            catch { }
            finally
            { cn.Close(); }
        }

        public static void Save(eLog log, int UserID)
        {
            if (log.Stack.Count == 0) return;
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            try
            {
                cn.Open();
                foreach (LogRecord s in log.Stack)
                {
                    cmd.CommandText = "insert into ELG (Entity, EntityID, Field, OVal,NVal,UserID,Oper, Reason,LastUpd) values ('" + s._entity + "','" + s._entityID + "','" + s._field + "', " + ((s._oval == "null") ? "null" : "'" + s._oval + "'") + ", " + ((s._nval == "null") ? "null" : "'" + s._nval + "'") + "," + UserID + ",'" + s._oper + "','" + s._reason + "',getdate()) ";
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            finally
            {
                cn.Close();
                log._stack.Clear();
            }
        }

        public static DataTable GetHistory(sObject rec)
        {
            return db.GetDbTable("select LastUpd, convert(varchar,LastUpd,104) as Дата,convert(varchar, LastUpd,108) as Время, Oper as Операция, Field as Реквизит, OVal as СтароеЗначение, NVal as НовоеЗначение, UserID, " + db.SqlNameField(rec.Meta_TDB, "UserID", "Пользователь") + " , Reason as Причина from ELG where Entity='" + rec.Meta_TDB + "' and EntityID='" + rec.ID + "' order by lastupd");
        }

        public static LogRecord GetLastUpdateRecord(sObject rec, string field)
        {
            DataTable dt = GetHistory(rec);

            DataRow[] rr = dt.Select("Реквизит='" + field + "'");
            if (rr.Length == 0) return new LogRecord(rec, field, "", "", "");
            DataRow r = rr[rr.Length - 1];
            LogRecord lr = new LogRecord(rec, field, r["СтароеЗначение"].ToString(), r["НовоеЗначение"].ToString(), r["Операция"].ToString(), r["Причина"].ToString());
            lr._lastupdate = cDate.cToDate(r["LastUpd"]);
            lr._userid = cNum.cToInt(r["UserID"]);
            return lr;
        }
    }
}
