using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Data;
using ensoCom;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для ViewArticle
    /// </summary>
    public abstract class ViewArticle
    {
        public const string TDB = "VIEWARTICLE";
        public static int GetMark(IAM current_user, sObject article)
        {
            DataRow[] rr = db.GetDbTable("select mark from "+TDB+" where metatdb='" + article.Meta_TDB + "' and eid=" + article.ID + " and userid="+current_user.ID).Select();
            return (rr.Length > 0) ? cNum.cToInt(rr[0]["mark"]) : 0;
        }
        public static decimal GetMark(sObject article)
        {
            DataRow[] rr = db.GetDbTable("select cast(sum(mark) as decimal)/cast(count(*) as decimal) as midmark from " + TDB + " where mark>0 and metatdb='" + article.Meta_TDB + "' and eid=" + article.ID).Select();
            return (rr.Length > 0) ? cNum.cToDecimal(rr[0]["midmark"]) : 0;
        }
        public static void SetMark(IAM current_user, sObject article, int mark)
        {
            if (mark <= 0)
                db.ExecuteCmd("delete " + TDB + " where userid=" + current_user.ID + " and metatdb='" + article.Meta_TDB + "' and eid=" + article.ID);
            else
            {
                if (db.ExecuteCmd("update " + TDB + " set mark=" + mark + " where userid=" + current_user.ID + " and metatdb='" + article.Meta_TDB + "' and eid=" + article.ID) == 0)
                    db.ExecuteCmd("insert into " + TDB + " (userid,metatdb,eid,seedate,mark) values (" + current_user.ID + ",'" + article.Meta_TDB + "'," + article.ID + "," + cDate.Date2Sql(cDate.TodayD) + "," + mark + ")");
            }
        }
        public static bool CheckIseen(IAM current_user, sObject article)
        {
            return (article.IsEmpty) ? true : (db.GetDbTable("select eid from " + TDB + " where userid=" + current_user.ID + " and metatdb='" + article.Meta_TDB + "' and eid=" + article.ID + " ").Rows.Count > 0);
        }
        

        public static void AddView(IAM current_user, sObject article)
        {
            if (db.ExecuteCmd(String.Format("update " + TDB + " set lcd=getdate() where userid={0} and metatdb='{1}' and eid={2}", current_user.ID, article.Meta_TDB, article.ID)) == 0)
                db.ExecuteCmd("insert into " + TDB + " (userid,metatdb,eid,seedate) values (" + current_user.ID + ",'" + article.Meta_TDB + "'," + article.ID + "," + cDate.Date2Sql(cDate.TodayD) + ")");

        }

        /*
        public static int QuantView(sObject article)
        {
            return db.GetDbTable("select userid from " + TDB + " where metatdb='" + article.Meta_TDB + "' and eid=" + article.ID + " ").Rows.Count;
        }
        public static int QuantView(sObject article, DateTime on_date)
        {
            return QuantView(article, on_date, on_date);
        }
        public static int QuantView(sObject article, DateTime start_date, DateTime finish_date)
        {
            return db.GetDbTable("select userid from " + TDB + " where metatdb='" + article.Meta_TDB + "' and eid=" + article.ID + " and seedate between " + cDate.Date2Sql(start_date) + " and " + cDate.Date2Sql(finish_date) + "").Rows.Count;
        }

        public static void AddRating(IAM current_user, sObject article, int rating)
        {
            db.ExecuteCmd("insert into RATING (userid,metatdb,eid,rating) values (" + current_user.ID + ",'" + article.Meta_TDB + "'," + article.ID + "," + rating + ")");
        }
        */
    }
}