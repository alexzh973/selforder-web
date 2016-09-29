using System;

namespace wstcp
{
    public partial class CommandPanel4List : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public string Title
        {
            get { return lbTitle.Text; }
            set { lbTitle.Text = value; }
        }


        public EventHandler Event_NEW
        {
            set { this.btnNew.Click += value; }
        }
        public EventHandler Event_EDIT
        {
            set { this.btnEdit.Click += value; }
        }
        public EventHandler Event_COPY
        {
            set { this.btnCopy.Click += value; }
        }
        public EventHandler Event_DELETE
        {
            set { this.btnDelete.Click += value; }
        }


        public bool ButtonsVisible
        {
            get { return (btnNew.Visible || btnEdit.Visible || btnCopy.Visible || btnDelete.Visible); }
            set { btnNew.Visible = btnEdit.Visible = btnCopy.Visible = btnDelete.Visible = value; }
        }

        public bool ButtonVisible_NEW
        {
            get { return btnNew.Visible; }
            set { btnNew.Visible = value; }
        }

        public bool ButtonVisible_EDIT
        {
            get { return btnEdit.Visible; }
            set { btnEdit.Visible = value; }
        }
        public bool ButtonVisible_COPY
        {
            get { return btnCopy.Visible; }
            set { btnCopy.Visible = value; }
        }
        public bool ButtonVisible_DELETE
        {
            get { return btnDelete.Visible; }
            set { btnDelete.Visible = value; }
        }


        protected void btnDelete_PreRender(object sender, EventArgs e)
        {
            btnDelete.Attributes.Add("onclick", "javascript:return confirm('Подтверждаете удаление?');this.style.visibility='hidden';");
            btnDelete.Attributes.Add("style", "visibility='visible'");

        }
        protected void bntEdit_Click(object sender, EventArgs e)
        {

        }
        protected void bntNew_Click(object sender, EventArgs e)
        {

        }
        protected void bntCopy_Click(object sender, EventArgs e)
        {

        }
        protected void bntDelete_Click(object sender, EventArgs e)
        {

        }
    }
}