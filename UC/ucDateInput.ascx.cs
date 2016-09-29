using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;

public class SelectionDateChangedEventArgs : EventArgs
{
    public DateTime Selected { get; set; }
}

public partial class ucDateInput : System.Web.UI.UserControl
{





    public bool AutoPostBack
    {
        get { return DateInput.AutoPostBack; }
        set { DateInput.AutoPostBack = value; }
    }
    //public EventHandler Event_ChangeDate
    //{
    //    set
    //    {
    //        if (value != null)
    //            DateInput.AutoPostBack = true;

    //        this.DateInput.TextChanged += value;
    //    }
    //}

    public bool ReadOnly
    {
        get { return DateInput.ReadOnly; }
        set
        {
            DateInput.ReadOnly = value;
        }
    }

    //public bool ExceptWeekend
    //{
    //    get { return ("" + ViewState["exwend"] == "y"); }
    //    set
    //    {
    //        ViewState["exwend"] = (value) ? "y" : "n";
    //    }
    //}


    //public string OnDayOver
    //{
    //    get { return "" + ViewState["OnDayOver"]; }
    //    set { ViewState["OnDayOver"] = value; }
    //}

    //public string MinDate
    //{
    //    get { return "" + ViewState["MinDate"]; }
    //    set { ViewState["MinDate"] = value; }
    //}

    //public string MaxDate
    //{
    //    get { return "" + ViewState["MaxDate"]; }
    //    set { ViewState["MaxDate"] = value; }
    //}

    //public DateTime DefaultDate
    //{
    //    get { return cDate.cToDate(ViewState["dd"]); }
    //    set { ViewState["dd"] = (value!=DateTime.MinValue)?value.ToShortDateString():""; }
    //}
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        {
            string script = "";

            script += "$(document).ready(function () {\n" +
                       "$.datepicker.setDefaults($.datepicker.regional[\"\"]);\n" +
                       "$(\".datepicker\").datepicker($.datepicker.regional[\"ru\"]);\n" +
                       "$(\".datepicker\").datepicker({\n" +
                       "minDate: \"+1d\",\n" +
                       "maxDate: \"+1m\",\n" +
                       "changeMonth: true,\n" +
                       "changeYear: true\n" +
                       "});\n" +
                       "$(\".datepicker\").datepicker(\"option\", \"dateFormat\", \"dd.mm.yy\");\n" +
                       "$(\".datepicker\").datepicker({\n" +
                       "minDate: \"+1d\",\n" +
                       "maxDate: \"+1m\",\n" +
                       "showOn: \"button\",\n" +
                       "buttonImage: \"../img/clnd.gif\",\n" +
                       "buttonImageOnly: true\n" +
                       "});\n" +
                       "});\n";



            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openClnd", script, true);
            //BindData();
        }



        //if (IsPostBack && ExceptWeekend && DateInput.Text!="" && cDate.DayOfWeek(DateInput.Text) > 5)
        //{
        //    RequiredFieldValidator1.ErrorMessage = "выбран выходной день";
        //    RequiredFieldValidator1.Visible = true;
        //}
    }

    //private void BindData()
    //{
    //    DateInput.Text = (DefaultDate != DateTime.MinValue) ? DefaultDate.ToShortDateString() : "";
    //}

    public DateTime SelectedDate
    {
        get { return cDate.cToDate(DateInput.Text); }
        set
        {
            DateInput.Text = (value != DateTime.MinValue) ? value.ToShortDateString() : "";
            mark_past_day();
        }
    }

    private void mark_past_day()
    {
        if (cDate.cToDate(DateInput.Text) == cDate.DateNull) DateInput.Text = "";
        else
            DateInput.ForeColor = (cDate.cToDate(DateInput.Text) < cDate.TodayD) ? cColor.Color("red") : cColor.Color("black");


    }


    protected void DateInput_TextChanged(object sender, EventArgs e)
    {
        SetSelectDate(cDate.cToDate(DateInput.Text.Trim().Replace(" ", ".").Replace(",", ".").Replace("-", ".")));
        
    }

    public void SetSelectDate(DateTime date)
    {
        DateInput.Text = date.ToShortDateString();
        mark_past_day();

        SelectionDateChangedEventArgs e = new SelectionDateChangedEventArgs(){Selected = date};
        //e.Selected = date;
        OnSelectionChanged(e);
    }


    public event EventHandler<SelectionDateChangedEventArgs> SelectionChanged;

    protected virtual void OnSelectionChanged(SelectionDateChangedEventArgs e)
    {
        if (SelectionChanged != null)
            SelectionChanged.Invoke(this, e);
    }

}
