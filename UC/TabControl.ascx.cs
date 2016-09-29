using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;


    public partial class TabControl : System.Web.UI.UserControl
    {
        protected static string _colorBorder = "#666699";
        protected List<string> _tabTitles = new List<string>();
        protected List<string> _tabCssPrefix = new List<string>();
        protected List<string> _tabTooltips = new List<string>();
        protected List<string> _punctMark = new List<string>();
        protected List<string> _pnlIDs = new List<string>();

        protected List<System.Web.UI.Control> _content = new List<System.Web.UI.Control>();
        private int _selectedTabIndex;
        private bool _autoPostBack = true;
        private bool _chTab = false;
        
        protected string _name = "cont";

        //private string _beginContainer = "<table width='100%' border='0' cellpadding='0' valign='top' cellspacing='0' style='PADDING:0;MARGIN:0;'><tr><td>";
        private string _divideHeader = "</td></tr>" +
            "<tr><td valign='top' style='border-left:" + _colorBorder + " 1px solid;border-right:" + _colorBorder + " 1px solid;border-bottom:" + _colorBorder + " 1px solid;'>";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _selectedTabIndex = (Session[_name + "TBI"] != null) ? (int)Session[_name + "TBI"] : 0;
                hdCurTabID.Value = _selectedTabIndex.ToString();
            }
            _selectedTabIndex = cNum.cToInt(hdCurTabID.Value);
            Session[_name + "TBI"] = _selectedTabIndex;

        }

        
        public bool ChangedTabpage
        {
            get { return _chTab; }
        }
        public void InitTabContainer(string NameTabContainer)
        {
            _name = NameTabContainer;
            if (!IsPostBack)
            {
                _selectedTabIndex = (Session[_name + "TBI"] != null) ? (int)Session[_name + "TBI"] : 0;
                hdCurTabID.Value = _selectedTabIndex.ToString();
            }
            if ("" + Session[_name + "TBI"] != "" + hdCurTabID.Value)
                _chTab = true;
        }

        public List<string> TabTitles
        {
            get { return _tabTitles; }
        }
public List<string> TabCssPrefix
        {
            get { return _tabCssPrefix; }
        }
        public int AddTabpage(string title, System.Web.UI.Control tabpanel)
        {
            return AddTabpage("", title, tabpanel);
        }

        

        public int AddTabpage (string tooltip, string title, System.Web.UI.Control tabpanel, string Mark="", string CssPrefix="")
        {
            _tabTitles.Add(title);
            _tabCssPrefix.Add(CssPrefix);
            _tabTooltips.Add(tooltip);
            _punctMark.Add(Mark);
            _content.Add(tabpanel);
            _pnlIDs.Add(tabpanel.ClientID);
            
            return _tabTitles.Count;
        }
        public void SetTabTitle(int TabIndex, string newTitle)
        {
            _tabTitles[TabIndex] = newTitle;
        }
        public string HeightPanel
        {
            set { ViewState[_name + "Height"] = value; }
            get { return ("" + ViewState[_name + "Height"] != "") ? "" + ViewState[_name + "Height"] : "100%"; }
        }
        public int SelectedTabIndex
        {
            //get { return _selectedTabIndex; }
            set { Session[_name + "TBI"] = _selectedTabIndex = value; hdCurTabID.Value = _selectedTabIndex.ToString(); }
        }
        public int CurrentTabIndex
        {
            get { return cNum.cToInt(hdCurTabID.Value); }
        }
        public bool IsActiveTabpage(System.Web.UI.Control tabpanel)
        {
            bool retVal = false;
            int i;
            i = cNum.cToInt(hdCurTabID.Value);
            try
            {
                if ((System.Web.UI.Control)_content[i] == tabpanel)
                    retVal = true;
            }
            catch { retVal = false; }
            return retVal;
        }

        public bool AutoPostBack
        {
            get { return _autoPostBack; }
            set { _autoPostBack = value; }
        }

        private string _punctHead = "";

        protected void _Container_PreRender(object sender, System.EventArgs e)
        {
            string divider = "<td class='tabdivider'>&nbsp;</td>";
            _punctHead = "<table id='tabctrl_" + this.ClientID + "'  cellpadding='3' cellspacing='0' width='100%' class='tabcontrol'><tr>";
            string sema = "";

            if (_selectedTabIndex > _tabTitles.Count - 1)
                SelectedTabIndex = (_tabTitles.Count != 0) ? _tabTitles.Count - 1 : 0;
            int wdth = 50 / (_tabTitles.Count + 1);
            for (int i = 0; i < _tabTitles.Count; i++)
            {
                sema = _punctMark[i].ToString();
                if (this.AutoPostBack)
                {
                    _punctHead += ((_punctHead.Length > 0) ? divider : "") + "<td style='width:" + wdth + "%;' id='tab" + this.ClientID + i + "' class='" + ((i == _selectedTabIndex) ? "tabactive" : "tabpassive") + " " + _tabCssPrefix[i] + " mazz" + _tabCssPrefix[i] + "afakka' " + ((i == _selectedTabIndex) ? "" : " onclick=\"setActPunct(" + _tabTitles.Count + ", " + i + ",'" + this.ClientID + "','" + _pnlIDs[i] + "')\"") + " title='" + _tabTooltips[i].ToString() + "' nowrap><span id='" + this.ClientID + "tp" + i + "'>" + _tabTitles[i].ToString() + "</span>" + ((sema != "") ? "&nbsp;<img src='../img/sema" + sema + ".gif'>" : "") + "</td>\n";
                }
                else
                {
                    _punctHead += ((_punctHead.Length > 0) ? divider : "") + "<td style='width:" + wdth + "%;'  id='tab" + this.ClientID + i + "' class='" + ((i == _selectedTabIndex) ? "tabactive" : "tabpassive") + " " + _tabCssPrefix[i] + "' onclick=\"setActPunct(" + _tabTitles.Count + ", " + i + ",'" + this.ClientID + "','"+_pnlIDs[i]+"')\"  title='" + _tabTooltips[i].ToString() + "' nowrap>" + _tabTitles[i].ToString() + ((sema != "") ? "&nbsp;<img src='../img/sema" + sema + ".gif'>" : "") + "</td>\n";
                }
               // ((System.Web.UI.Control)_content[i]).ID = "pnl" + this.ClientID + i;
                if (_content[i].GetType() == typeof(Panel))
                {
                    ((Panel)_content[i]).CssClass = (i == _selectedTabIndex) ? "visi" : "hidi";
                }
                if (_autoPostBack == true)
                {
                    ((System.Web.UI.Control)_content[i]).Visible = (i == _selectedTabIndex) ? true : false;
                }
            }
            _punctHead += "<td class='tabdivider tabspace'>&nbsp;</td></tr></table>";

            lit.Text = _punctHead;

            string script = "<script  type='text/javascript'>" +
                            "function setActPunct(qty, curr_ind, megaid,pnlId){ ";
            if (!_autoPostBack)
                script += "	for(i=0;i<qty;i++){ if(i==curr_ind){$('#tab' + megaid + '' + i).removeClass('tabpassive');$('#tab' + megaid + '' + i).addClass('tabactive');}else{$('#tab' + megaid + '' + i).removeClass('tabactive');$('#tab' + megaid + '' + i).addClass('tabpassive');} " +
                          "			$('#ytab' + megaid + '' + i).attr('class', ((i == curr_ind) ? 'tabactive' : 'tabpassive'));" +
                          "	}\n" +
                          "$('.visi').attr('class','hidi');$('#' + pnlId).attr('class','visi');\n";

            script += "  $('#" + hdCurTabID.ClientID + "').val(curr_ind);" +
                ((_autoPostBack) ? " if(this.document.forms[0].Refresh){this.document.forms[0].Refresh.value='y';};	this.document.forms[0].submit();" : "") +
                "}" +
                "</script>" +
                "<style>" +
                ".visi { DISPLAY: block; }" +
                ".hidi { DISPLAY: none; }" +
                "</style>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "setActPunct", script,false);
        }

    }
