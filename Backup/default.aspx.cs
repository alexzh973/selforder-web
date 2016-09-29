using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using wstcp.Models;
using System.Data;
using ensoCom;

namespace wstcp
{
    public partial class _default : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (iam.ID > 0)
            {
                show_info();
                show_existorders();


            }
            else
            {
                MultiView1.SetActiveView(vLogin);

            }
        }

        private void show_info()
        {
            if (iam.SubjectID>0){
                Subject subj = new Subject(iam.SubjectID, iam);
                litInfo.Text += "<div>Текущий пользователь <strong>" + iam.Name+"</strong></div>";
                if (iam.SubjectID == 0)
                {
                }
                else
                {
                    litInfo.Text += "<div>Вы представитель <strong>" + subj.Name + "</strong> (ИНН <strong>" + subj.INN + "</strong>, код 1С <strong>" + subj.Code + "</strong>)</div>";
                    litInfo.Text += "<br/><br/>";
                    if (subj.TAs.Count == 0)
                    {
                        litInfo.Text += "<div>Для Вас не назначен менеджер в УЦСК</div>";
                    }
                    else if (subj.TAs.Count == 1)
                    {
                        litInfo.Text += "<div title='" + subj.TAs[0].Descr + "'>Ваши менеджер в УЦСК: <strong>" + subj.TAs[0].Name + "</strong>, email <strong>" + subj.TAs[0].Email + "</strong>, тел <strong>" + subj.TAs[0].Phones + "</strong></div>";
                    }
                    else
                    {
                        litInfo.Text += "<div>Ваши менеджеры в УЦСК: <ul>";
                        foreach (pUser p in subj.TAs)
                            litInfo.Text += "<li title='" + p.Descr + "'><strong>" + p.Name + "</strong>, email <strong>" + p.Email + "</strong>, тел <strong>" + p.Phones + "</strong></li>";
                        litInfo.Text += "<ul/></div>";
                    }
                }
            }
        }

        void show_existorders()
        {
            MultiView1.SetActiveView(vOrders);
            string sql;
            if (iam.SubjectID > 0)
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, ord.SummOrder, subj.Name as SubjectName from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where ord.SubjectID=" + iam.SubjectID + "";
            else
                sql = "select ord.id, ord.Name, ord.RegDate, ord.State, ord.SummOrder, subj.Name as SubjectName from " + Order.TDB + " as ord inner join " + Subject.TDB + " as subj on ord.SubjectID=subj.id where subj.EmailTAs like '%" + iam.Email + "%'";

            DataTable dt = db.GetDbTable(sql);

            DataTable dto = db.GetDbTable("select 0 as id, '' as Name, '' as RegDate, '' as State, '' as SubjectName, '' as SummOrder, '' as linkchange");
            dto.Rows.Clear();
            foreach (DataRow r in dt.Select("State<>'D'"))
            {
                DataRow nr = dto.NewRow();
                nr["id"] = r["id"];
                nr["Name"] = r["Name"];

                nr["RegDate"] = cDate.cToDate(r["RegDate"]).ToShortDateString();
                nr["SubjectName"] = r["SubjectName"];
                nr["State"] = Order.get_stateorder_descr("" + r["State"]);
                nr["SummOrder"] = cNum.cToDecimal(r["SummOrder"], 2);
                //if (iam.SubjectID > 0)
                nr["linkchange"] = "<a href='../good/goodlist.aspx?id=" + r["id"] + "&act=edit'>изменить</a>";
                dto.Rows.Add(nr);
            }

            rpOrders.DataSource = dto;
            rpOrders.DataBind();
            //Literal1.Text = (dt.Rows.Count > 0) ? "Сохраненные заявки" : "Нет сохраненных заявок";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            IAM.ClearMySession(IAM.GetMe(Session.SessionID), Session.SessionID);

            IAM _iam = IAM.Login(txLogin.Text, txPwd.Text, Session.SessionID, Request.ServerVariables["REMOTE_ADDR"]);
            if (_iam != null && _iam.ID > 0)
            {
                HttpCookie mc = new HttpCookie("usrcook", txLogin.Text + "#" + txPwd.Text);
                mc.Expires = DateTime.Now.AddDays(33);
                Response.Cookies.Add(mc);
                lbMessageLogin.Text = "";
                txPwd.Text = "";
                show_existorders();
                Response.Redirect("default.aspx");
            }
            else
            {
                lbMessageLogin.Text = "авторизация не прошла.";
                txPwd.Text = "";
            }
        }
    }
}