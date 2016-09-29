using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ensoCom;
using selforderlib;

namespace wstcp
{
    public partial class CommentList : uc_base
    {         

        public sObject DataObject
        {
            get { return (ViewState["DataObject"] != null) ? (sObject)ViewState["DataObject"] : new sObject(0, ""); }
            set { ViewState["DataObject"] = value; }
        }

        public bool CanAddComment
        {
            get { return btnAddComment.Visible; }
            set { btnAddComment.Visible = value; }
        }

        public string Text = "";
        //{
        //    .get { return txComment.Text; }
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                plknSH.Visible = false;
                if (DataObject.IsEmpty) return;
                DataTable dt = Comment.GetTable(DataObject, iam);
                btnAddComment.Visible = CanAddComment;
                pnlAddComment.Visible = false; 
                lbQty.Text = dt.Rows.Count.ToString();
                if (!CanAddComment && dt.Rows.Count == 0)
                {
                    PlaceHolder1.Visible = false;
                    return;
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "ID desc";
                Repeater1.DataSource = dv;
                Repeater1.DataBind();
               
            }
        }

        public EventHandler Event_AddedComment
        {
            set { this.Button1.Click += value; }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (txComment.Text.Trim().Length < 2 || iam == null || DataObject.IsEmpty) return;
            Comment rec = new Comment(0, iam);
            rec.Descr = txComment.Text;

            rec.Set_ToObject(DataObject);
            if (Comment.Save(rec))
            {
                Text = txComment.Text;
                txComment.Text = "";
                pnlAddComment.Visible = false;
                DataView dv = Comment.GetTable(DataObject, iam).DefaultView;
                dv.Sort = "ID desc";
                Repeater1.DataSource = dv;
                Repeater1.DataBind();
                
            }
        }
        
        

        

        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            pnlAddComment.Visible = true;
            txComment.Focus();
        }



    }
    
}