using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ensoCom;
using System.Data;
using selforderlib;

namespace wstcp
{
    public partial class newspreview : p_p
    {
        protected override void SavePageStateToPersistenceMedium(object state)
        {
        }
        protected override object LoadPageStateFromPersistenceMedium()
        {
            return null;
        }
        News RECORD;
        protected void Page_Load(object sender, EventArgs e)
        {

           // check_auth();          
                
                if ("" + Request["id"] == "") return;
                eID = cNum.cToInt(Request.QueryString["id"]);
            
                if (RECORD == null) RECORD = new News(eID, iam);
                    
                show_detail();                              
        }
        
       

        
        void show_detail()
        {
            if (RECORD == null) RECORD = new News(eID, iam);
            
            ViewArticle.AddView(iam, RECORD.ThisObject);
            

            this.Title = RECORD.Name;
            lbTitleRecord.Text = RECORD.Name;
            lbAttributes.Text = String.Format("автор: {0}, дата {1}", RECORD.Author.Name, RECORD.RegDate.ToShortDateString());
            lbDescr.Text = RECORD.Descr;
            Image1.ImageUrl = "../MEDIA/NEWS/Trumb/" + RECORD.ID + ".png";
            lbText.Text = RECORD.Text;          

            

        }

    }
}