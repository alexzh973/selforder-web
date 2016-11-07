using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Web;
using ensoCom;
using System.Data;
using selforderlib;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для subj
    /// </summary>
    public class subj : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string res = "";
            IAM iam = IamServices.GetIam(context.Request["sid"]);
            string act = context.Request["act"];
            switch (act)
            {
                case "trygetsms":
                    string email = context.Request["e"];
                    DataTable dt = db.GetDbTable("select s.UseSmsAuthorization, u.id, u.phones from SUBJ as s inner join ENSOUSER as u on s.id=u.subjectID where u.email='" + email + "'");
                    if (dt.Rows.Count > 0)
                    {
                        res = "" + dt.Rows[0][0];
                        if ((res == "Y" || Global.is_dev) && ("" + dt.Rows[0][2]).Length>=10)
                        {
                            sendSms(context.Request["sid"], cNum.cToInt(dt.Rows[0][1]), "" + dt.Rows[0][2]);
                        }
                    }
                    break;
                default:
                    res = "";
                    break;
            }
            context.Response.Write(res);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        private void sendSms(string sessionId, int id, string phone)
        {
            string psw = ensoCom.password.generate_password(4);
            if (Global.is_dev)
            {
                phone = "+89090091254";
                //sms.SendSMS(phone, "пароль " + psw);
                //return;
            }
            bool res = false;
            
            SqlConnection cn = new SqlConnection(db.DefaultCnString);
            SqlCommand cmd = cn.CreateCommand();
            cn.Open();
            try
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pass", psw);
                cmd.CommandText = "update " + pUser.TDB + " set Pass=cast(@pass as varbinary) where id=@id";
                res = (cmd.ExecuteNonQuery() > 0);
            }
            catch{}
            finally {cn.Close();}
            if (res)
            {
                sms.SendSMS(phone, "пароль " + psw);
            }

        }
    }
}