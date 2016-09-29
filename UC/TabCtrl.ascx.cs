using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public class SelectionChangedEventArgs : EventArgs
{
    public int Position { get; set; }
}

[Serializable]
public class TabItem
{
    public TabItem(string title, string titledefault="",string tooltip = "", string css="", string imgurl = "")
    {
        Title = title;
        TitleDefault = (titledefault == "") ? title : titledefault;
        Tooltip = tooltip;
        ImgUrl = imgurl;
        Css = css;
    }
    public string Title { get; set; }
    public string TitleDefault { get; set; }
    public string ImgUrl { get; set; }
    public string Tooltip { get; set; }
    public string Css { get; set; }
}

public partial class TabCtrl : System.Web.UI.UserControl
{
    
    public List<TabItem> Tabs
    {
        get
        {
            object t = (List<TabItem>)ViewState["_tabs"];
            if (t == null)
            {
                ViewState["_tabs"] = new List<TabItem>();
                return (List<TabItem>)ViewState["_tabs"];
            }
            return (List<TabItem>)t;
        }
        
    }

    public void TabTitlesResetToDefault()
    {
        foreach (TabItem t in Tabs.Where(t => t.Title != t.TitleDefault))
        {
            t.Title = t.TitleDefault;
        }
    }

    
    

    public int CurrentTabIndex
    {
        get
        {
            object res = ViewState["selind"];
            if (res == null)
                return 0;
            return (int)res;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        Repeater1.DataSource = Tabs;
        Repeater1.DataBind();
    }

    protected string getCss(object data)
    {
        RepeaterItem tab = (RepeaterItem)data;
        if (tab.ItemIndex == CurrentTabIndex)
            return "tabactive";
        else
        {
            return "tabpassive";
        }
    }
    protected void ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        SelectTab(e.Item.ItemIndex);
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex > Tabs.Count)
            tabIndex = 0;
        ViewState["selind"] = tabIndex;
        BindData();

        SelectionChangedEventArgs e = new SelectionChangedEventArgs();
        e.Position = tabIndex;
        OnSelectionChanged(e);
    }



    

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        if (SelectionChanged != null)
            SelectionChanged.Invoke(this, e);
    }
}
