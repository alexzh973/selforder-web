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
    public partial class personcard : p_p
    {
        pUser usr;
        protected void Page_Load(object sender, EventArgs e)
        {
            check_auth();
            if (!IsPostBack)
            {
                eID = cNum.cToInt(Request.QueryString["id"]);
                _ACT_ = Request.QueryString["act"];
            }
            usr = new pUser(eID, iam);
            if (!IsPostBack)
            {
                if (webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + @"/" + eID + "b.png"))
                    photo.Text = "<img class='f-bwi-pic' src='" + HelpersPath.PersonsAvatarVirtualPath + @"/" + eID + "b.png" + "'/>";
                else
                    photo.Text = "<img class='f-bwi-pic' src='" + HelpersPath.PersonsAvatarVirtualPath + @"/0b.png" + "'/>";
                lbName.Text = usr.Name;
                phones.Text = usr.Phones;
                email.Text = usr.Email;



                if (usr.SubjectID > 0) // значит клиент
                {
                    blockOwner.Visible = false;
                    blockSubject.Visible = true;
                    show_subj_info(usr.SubjectID);
                }
                else
                {
                    blockSubject.Visible = false;
                    blockOwner.Visible = true;
                    Owner own = new Owner(usr.OwnerID);
                    lbOwnerName.Text = own.Name;
                    lbOwnerAddress.Text = own.Address;
                    lbOwnerPhones.Text = own.Phones;
                    lbOwnerTime.Text = own.Timework;


                   
                } 
                pnlFormMessage.Visible = false;
                if (pnlFormMessage.Visible)
                {
                    //lbsupportemail.Text = CurrentCfg.EmailSupport;
                    chTosupport.Checked = false;
                    chToTa.Checked = true;
                    if (usr.SubjectID > 0)
                    {
                        Subject subj = new Subject(usr.SubjectID, iam);
                        lbToTaEmail.Text = subj.EmailTAs;
                    }
                    else
                    {
                        chToTa.Visible = false;
                    }
                }
            }

        }


        private void show_subj_info(int subjId)
        {
            Subject subj = new Subject(subjId, iam);
            Owner own = new Owner(subj.OwnerID);
            lbSubjectInfo.Text = "";
            lbSubjectInfo.Text = "<p>Представитель компании:</p><p class='bold'>" + subj.Name + "</p><p>ИНН " + subj.INN + "</p>";

            DGinfo d = Subject.GetDg(subj.ID, subj.CodeDG);

            //if (d.CurrentDZ != 0 || d.LimitDZ != 0)
            {
                string ti = "<p>Текущий баланс " + (-1 * d.CurrentDZ) + " руб.,\n допустимый лимит: " + d.LimitDZ + " руб.</p>";
                lbSubjectInfo.Text += "<p title='" + ti + "'>" + (((d.LimitDZ - d.CurrentDZ) > 0) ? "Отгрузка возможна на сумму <b>" + (d.LimitDZ - d.CurrentDZ) + "</b> руб." : "Отгрузка без предоплаты не возможна") + "</p>"; // "<div>ДОГОВОР #" + d.CodeDogovor + ": " + (-1*d.CurrentDZ) + " руб.,<br/>допустимый лимит: " + d.LimitDZ + " руб.<br/>" + (((d.LimitDZ - d.CurrentDZ) > 0) ? "возможна отгрузка на сумму " + (d.LimitDZ - d.CurrentDZ) + "руб." : "<span title='БЕЗ ПРЕДОПЛАТЫ ОТГРУЗКА НЕВОЗМОЖНА'>?</span>") + "</div>";
            }
            string emailta = subj.EmailTAs.Split(',')[0];
            pUser ta = new pUser(pUser.FindByField("email", emailta), iam);
            if (!ta.IsNew)
            {
                imgPhotoTA.ImageUrl = webUser.PhotoSmallPath(ta.ID);
                lbTAName.Text = String.Format("<a class=\"bold\" href=\"javascript:return 0;\" onclick=\"openflywin('../account/card.aspx?id={0}', 500, 500, '{1}')\">{1}</a>", ta.ID, ta.Name);
                lbTAPhone.Text = ta.Phones;
                lbTAEmail.Text = ta.Email;
            }
            else
            {
                lbTAName.Text = "не назначен ни один менеджер";
                lbTAPhone.Text = own.Phones;
                lbTAEmail.Text = CurrentCfg.EmailSupport;
            }
        }

        //public void show_personalTAs(int subjId)
        //{

        //    //if (iam == null) iam = IAM.GetMe(Session.SessionID);
        //    //int subjId = (iam.SubjectID == 0) ? cNum.cToInt(Session["SubjectID"]) : iam.SubjectID;
        //    if (subjId == 0)
        //    {
        //        //rpTAs.Visible = false;
        //        return;
        //    }
        //    DataTable dt = db.GetDbTable("select 0 as id, '' as Name, '' as email, '' as phone, '' as photo where 1=0");
        //    Subject subj = new Subject(subjId, iam);
        //    DataRow nr;
        //    if (subj.EmailTAs == "")
        //    {
        //        nr = dt.NewRow();
        //        nr["id"] = 0;
        //        nr["Name"] = "не назначен ни один менеджер";
        //        nr["email"] = CurrentCfg.EmailSupport;// "info@santur.ru";
        //        nr["phone"] = "(343)270-04-04";
        //        dt.Rows.Add(nr);
        //    }
        //    else
        //    {
        //        pUser ta = new pUser(pUser.FindByField("email", subj.EmailTAs), iam);
        //        {
        //            nr = dt.NewRow();
        //            nr["id"] = ta.ID;
        //            nr["Name"] = ta.Name;
        //            nr["email"] = ta.Email;
        //            nr["phone"] = ta.Phones;
        //            nr["photo"] = webUser.PhotoSmallPath(ta.ID);
        //            dt.Rows.Add(nr);
        //        }
        //    }
        //    rpTAs.DataSource = dt;
        //    rpTAs.DataBind();
        //    //Subject subj = new Subject(iam.SubjectID, iam);

        //}

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (txMessage.Text.Trim().Length < 5)
            {
                lbMess.Text = "Пустое сообщение! не будет отправлено.";
                return;
            }
            if (!chTosupport.Checked && (!chToTa.Visible || !chToTa.Checked || lbToTaEmail.Text.Trim() == ""))
            {
                lbMess.Text = "Нет получателей! сооющение не будет отправлено.";
                return;
            }
            lbMess.Text = "";
            string eto = "";
            eto += (chTosupport.Checked) ? CurrentCfg.EmailSupport : "";
            cStr.AddUnique(ref eto, (chToTa.Checked) ? lbToTaEmail.Text : "");
            Subject s = new Subject(iam.ID, iam);
            EmailMessage mess = new EmailMessage(CurrentCfg.EmailPortal, eto, "сообщение с портала santechportal.ru", txMessage.Text + "<div>[автор " + iam.Email + ", " + iam.Name + " (" + s.Name + ")]</div>");
            mess.Send();
            txMessage.Text = "";
            lbMess.Text = "<div>Сообщение было отправлено</div>";

        }

    }
}