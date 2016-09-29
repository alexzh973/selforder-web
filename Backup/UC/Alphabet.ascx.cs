using System.Web.UI;
using System.Web.UI.WebControls;

namespace wstcp
{
    public partial class Alphabet : System.Web.UI.UserControl
    {
        string[] al = { "все", "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Э", "Ю", "Я", "все" };
        public void initAlphabet()
        {
            if ("" + ViewState["Letter"] != curLetter.Value)
                _ch = true;
            else _ch = false;
            ViewState["Letter"] = curLetter.Value;
        }

        private bool _ch;
        public bool Changed
        {
            get
            {
                return _ch;
            }
        }
        public string Letter
        {
            get
            {

                return curLetter.Value;
            }
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (IsPostBack)
            {

            }
            
            drawAlphabet();
        }
        protected string ThisPage
        {
            get { return Request.Path + (("" + Request.QueryString != "") ? "?" + Request.QueryString : ""); ;}
        }

        
        public void drawAlphabet()
        {
            string script = "<script type='text/javascript'>function setLetter(fieldId,letter){$('#'+fieldId).val(letter);this.document.forms[0].submit();}</script>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "setLetter", script);
            string curletter = Letter;
            for (int i = 0; i < al.Length; i++)
            {
                HyperLink a = new HyperLink();
                if (al[i] != "все")
                    a.CssClass = (al[i] != curletter) ? "linkbutton small" : "invert linkbutton small bold";
                a.NavigateUrl = "javascript:setLetter('" + curLetter.ClientID + "','" + ((al[i] != "все") ? al[i] : "") + "')";
                //..CommandName = "letter";
                a.Text = al[i] + "";

                pnl.Controls.Add(a);
            }
        }

    }
}