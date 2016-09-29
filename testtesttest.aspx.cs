using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

namespace wstcp
{
    public partial class testtesttest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TabCtrl1.Tabs.Add(new TabItem("Первый"));
                TabCtrl1.Tabs.Add(new TabItem("Второй"));
                TabCtrl1.Tabs.Add(new TabItem("Четвертый"));
                TabCtrl1.Tabs.Add(new TabItem("Пааа"));
                myDate.SetSelectDate(cDate.cToDate("23.09.2016"));
            }
        }

        protected void Tab_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Label1.Text = "Пункт "+e.Position + " ("+TabCtrl1.CurrentTabIndex+")";
        }

        protected void myDate_SelectionChanged(object sender, SelectionDateChangedEventArgs e)
        {
            Label1.Text = "выбрана дата "+myDate.SelectedDate.ToShortDateString();
            
        }
    }
}