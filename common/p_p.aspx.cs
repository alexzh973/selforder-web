﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ensoCom;
using System.Collections;
using System.Threading;
using System.Globalization;
using selforderlib;

namespace wstcp
{
    public partial class p_p : System.Web.UI.Page
    {

        public bool IsDev
        {
            get { return (HelpersPath.RootUrl.ToLower().IndexOf("localhost") > -1); }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        }
        private IAM _iam;

        protected IAM iam
        {
            get
            {
                if (_iam == null || _iam.ID <= 0)
                    _iam = IamServices.GetIam(Session.SessionID);
                if ((_iam == null || _iam.ID <= 0) && !checkCookies())
                {
                    _iam = IamServices.GetGuest(Session.SessionID);
                    
                }
                return _iam;
            }
        }
        protected int OwnerID
        {
            get { if (cNum.cToInt(Session["OWNERID"]) <= 0) Session["OWNERID"] = 100000; return cNum.cToInt(Session["OWNERID"]); }
        }
        protected int eID
        {
            get { return cNum.cToInt(ViewState["EID"]); }
            set { ViewState["EID"] = value.ToString(); }
        }
        


        protected string _ACT_
        {
            get { if (!IsPostBack && Request.QueryString.Get("act") != null) ViewState["ACT"] = Request.QueryString.Get("act"); return "" + ViewState["ACT"]; }
            set { ViewState["ACT"] = value; }
        }
        




        protected string GetRequestByControlId(string controlId)
        {
            string[] fs = Request.Form.AllKeys;
            var f = fs.FirstOrDefault(c => c.EndsWith(controlId));
            return Request.Form[f];
        }
        
        
        
        
        protected void check_auth()
        {

            if (iam == null || iam.ID <= 0)
            {
                if (!checkCookies())
                {

                    StoreCurrentPage();
                    //Response.Redirect("../Account/login.aspx", true);
                }
                Response.Redirect("../Account/login.aspx", true);
            }


        }

        protected bool check_is_internal_ip()
        {
            string str = System.Configuration.ConfigurationManager.AppSettings["ipaccess"];
            string myip = Request.ServerVariables["REMOTE_ADDR"];
            return (str.IndexOf(myip)>=0);
        }

        bool checkCookies()
        {
            bool result = false;
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            HttpCookie uc = Request.Cookies["usrcook"];
            string usrcook = (uc != null) ? uc.Value : "";
            if (usrcook == "") return false;
            string[] u = usrcook.Split('#');
            string lgn = (u.Length > 0) ? u[0] : "";
            string pwd = (u.Length > 1) ? u[1] : "";
            _iam = IamServices.Login(lgn, pwd, Session.SessionID, ip);
            result = (_iam != null && _iam.ID > 0);
            return result;
        }

        private static Hashtable __stackpages;
        private static Hashtable __stackpagesss;
        public void StoreCurrentPage()
        {
            if (__stackpages == null) __stackpages = new Hashtable();
            if (__stackpagesss == null) __stackpagesss = new Hashtable();
            if (this.Page.Request.Url.ToString().IndexOf("login.aspx") <= -1)
            {
                __stackpages[Session.SessionID] = __stackpagesss[Session.SessionID] = this.Page.Request.AppRelativeCurrentExecutionFilePath;

            }
        }
        public static string GetPreviousPage(IAM current_user)
        {
            if (__stackpages == null) __stackpages = new Hashtable();
            if (__stackpagesss == null) __stackpagesss = new Hashtable();
            return ("" + __stackpages[current_user.SessionID] == "") ? "" + __stackpagesss[current_user.SessionID] : "" + __stackpages[current_user.SessionID];
        }



    }
}