using ensoCom;
using selforderlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wstcp
{
    public partial class DgInfo : uc_base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ShowInfo(string DgCode, int ownerId)
        {
            DGinfo d = Subject.GetDgInfo(DgCode, ownerId);
            lbAvail.Text = (((d.LimitDZ - d.CurrentDZ) > 0) ? "<span class='green'>ВОЗМОЖНА ОТГРУЗКА НА СУММУ " + (d.LimitDZ - d.CurrentDZ) + "руб.</span>" : "<span class='red'>без предоплаты отгрузка невозможна</span>");
            
            lbDG.Text = (d.Num != "") ? d.Num:d.CodeDogovor;
            lbPeriod.Text = d.StartDateS + " - <span " + ((cDate.GetDayDistance(cDate.TodayD, d.EndDate) < 14) ? "class='red'" : "") + " >" + d.EndDateS + "</span>";
            lbLDZ.Text = (d.LimitDZ > 0) ? "" + d.LimitDZ + "руб." : "100% предоплата";
            lbWait.Text = (d.LimitDZ > 0)?""+d.PayOtsrok + " дней":"-";
            lbCurrBlnc.Text = "" + ((d.CurrentDZ >= 0) ? ((d.CurrentDZ == 0) ? "0.00руб" : "<span class='red'>-" + d.CurrentDZ + "руб.</span>") : "<span class='green'>+" + (d.CurrentDZ * -1) + "руб.</span>");

        }
    }
}