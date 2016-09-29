using System;

namespace wstcp
{
    public partial class CommandPanel4Profile : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public EventHandler Event_Save
        {
            set { this.lbtnSave.Click += value; }
        }
        public EventHandler Event_SaveAndClose
        {
            set { this.lbtnOK.Click += value; }
        }
        public EventHandler Event_Cancel
        {
            set { this.lbtnCancel.Click += value; }
        }
        public EventHandler Event_Delete
        {
            set { this.lbtnDelete.Click += value; }
        }
        

        public bool Visible_ButtonSave
        {
            get { return lbtnSave.Visible; }
            set { lbtnSave.Visible = value; }
        }
        public bool Visible_ButtonSaveAndClose
        {
            get { return lbtnOK.Visible; }
            set { lbtnOK.Visible = value; }
        }
        public bool Visible_ButtonDelete
        {
            get { return lbtnDelete.Visible; }
            set { lbtnDelete.Visible = value; }
        }
        public bool Visible_ButtonCancel
        {
            get { return lbtnCancel.Visible; }
            set { lbtnCancel.Visible = value; }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

        }
        protected void btnOK_Click(object sender, EventArgs e)
        {

        }
        protected void lbtnCancel_Click(object sender, EventArgs e)
        {

        }
        protected void lbtnOK_Click(object sender, EventArgs e)
        {

        }
        protected void lbtnDelete_Click(object sender, EventArgs e)
        {

        }
        protected void lbtnOK_PreRender(object sender, System.EventArgs e)
        {
            lbtnOK.Attributes.Add("onclick", "javascript:$('.btnsave').hide();");
            //lbtnOK.Attributes.Add("style", "visibility='visible'");

        }


        protected void lbtnSave_PreRender(object sender, System.EventArgs e)
        {
            lbtnSave.Attributes.Add("onclick", "javascript:$('.btnsave').hide();");
            //lbtnSave.Attributes.Add("style", "visibility='visible'");
            //initScript();
        }
    }
}