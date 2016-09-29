using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using selforderlib;
using ensoCom;
using System.Text;
using System.Collections;
using System.IO;

namespace wstcp
{

    public partial class cart : p_p
    {



        private Subject subj;
        private Owner owner;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID < 100000)
            {
                Response.Redirect("~/default.aspx", true);
            }

            lbMessage.Text = (iam.PresentedSubjectID == 0) ? "Нельзя формировать заявку не выбрав клиента. <a href='../default.aspx'>Вернуться на главную страницу</a> " : "";
            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "order";
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;
            }


            //TabControl1.InitTabContainer("tasklisttabs");
            //TabControl1.AutoPostBack = false;
            //TabControl1.AutoPostBack = true;
            //TabControl1.AddTabpage("Классификатор", pnlStruct);
            //TabControl1.AddTabpage("Справочник номенклатуры", pnlList);


            owner = new Owner(iam.OwnerID);
            subj = new Subject(iam.PresentedSubjectID, iam);
            //TabControl1.AddTabpage("Текущая заявка", pnlOrder);
            //TabControl1.SelectedTabIndex = 2;

            if (!IsPostBack)
            {
                
                BreadPath1.AddPuncts("Список заявок","../default.aspx");
                BreadPath1.AddPuncts("Каталог номенклатуры", "../order/selectgood.aspx","/");
                mvForm.SetActiveView(vProfile);
                eID = cNum.cToInt(Request["id"]);
                if (iam.CurOrder == null || (iam.CurOrder.ID != eID && eID > 0))
                {
                    iam.CurOrder = new Order(eID, iam);
                    if (_ACT_ == "copy")
                    {
                        eID = 0;
                        iam.CurOrder.MakeCopy();
                    }
                    
                }
                else if (iam.CurOrder == null && eID==0)
                    Response.Redirect("~/order/orderdefault.aspx", true);
                
                iam.PresentedSubjectID = iam.CurOrder.SubjectID;
                if(subj==null || subj.ID!=iam.PresentedSubjectID)
                    subj = new Subject(iam.PresentedSubjectID, iam);
                
                if ((iam.IsSaller || iam.IsSuperAdmin || iam.IsTA || iam.IsAdmin) && iam.SubjectID != iam.PresentedSubjectID && iam.PresentedSubjectID > 0)
                    lbForSubj.Text = "для " + subj.Name;

                DgInfo1.ShowInfo(subj.CodeDG, subj.OwnerID);

                if (iam.CurOrder!=null)
                {
                    Order ord = iam.CurOrder;
                    
                    lbReadonly.Visible = OrderReadOnly;
                    load_curentorder();
                    CommentList1.DataObject = new sObject(eID, Order.TDB);
                    CommentList1.CanAddComment = true;
                    CommentList1.Event_AddedComment = new EventHandler(CommentList1_AddingComment);
                }
                else
                {
                    lbReadonly.Visible = false;
                    CommentList1.Visible = false;
                }
            }

            if (refresh.Value == "Y")
            {

                load_curentorder();
                refresh.Value = "";
            }


        }

        
        private void load_curentorder()
        {
            if (iam.CurOrder == null) iam.CurOrder = new Order(eID, iam);
            
            if (iam.CurOrder.ID == 0 && iam.CurOrder.Dg == "")
                iam.CurOrder.Dg = subj.CodeDG;
            Order ord = iam.CurOrder;
            if (ord.ID > 0)
                ViewArticle.AddView(iam, ord.ThisObject);
            link4prn.Text = (ord.ID > 0) ? "<a href='javascript:return false;' class='button' onclick=\"openflywin('../order/detailfly.aspx?id=" + ord.ID + "', 850, 800, 'Заявка " + ord.Name + " от " + ord.RegDate.ToShortDateString() + "')\"><img src='../simg/16/printer.png'> печать</a>" : "";

            link4prn.Visible = (link4prn.Text != "");
            lbSubject.Text = ord.Subject.Name + ", ИНН " + ord.Subject.INN;
            lbOrder.Text = "" + ord.ID;
            txNameOrder.Text = ord.Name;
            txDescr.Text = ord.Descr;
            lbRegDate.Text = ord.RegDate.ToShortDateString();
            lbStateOrder.Text = (ord.ID > 0) ? ord.StateDescr : "Заявка еще не сохранена";
            lbCode.Text = ord.Code;

            btnClear.Visible = lbtnSave.Visible = !OrderReadOnly;

            if (ord.ID == 0) btnClear.Visible = false;
            

            decimal smb = ord.SummBase;
            decimal sm = ord.Summ;

            foreach (OrderItem item in ord.Items)
            {

                decimal q = (Request["q_" + item.GoodId] != null) ? cNum.cToDecimal(Request["q_" + item.GoodId]) : item.Qty;

                if (q <= 0) q = 1;
                if (item.Qty != q)
                {
                    ord.ChangeItem(item, q, item.Descr);
                    item.Mark = "";
                }
            }


            

            dgOrder.DataSource = ord.Items;
            dgOrder.CurrentPageIndex = 0;
            dgOrder.DataBind();

            lbSumm.Text = ""+ord.Summ;
            lbDscnt.Text = "" + cNum.cToDecimal(ord.Discount*100, 2);
        }

        
        

        private void CommentList1_AddingComment(object sender, EventArgs e)
        {
            Order ord = iam.CurOrder;
            string emails = ord.Subject.EmailTAs;
            EmailMessage m = new EmailMessage(iam.Email, emails, "новый комментарий к Заявке №" + ord.ID + " от " + ord.RegDate.ToShortDateString() + " (код 1С " + ord.Code + ")", CommentList1.Text);
            m.Send();
        }





        private bool ItsMySubj(int subjectId)
        {
            return iam.ItsMySubj.Contains(subjectId);
        }

        bool OrderReadOnly
        {
            get
            {
                return !(cStr.Exist(iam.CurOrder.State.ToUpper(), new string[] { "", "U", "N" }) && (iam.CurOrder.SubjectID == iam.PresentedSubjectID || ItsMySubj(iam.CurOrder.SubjectID) || iam.IsIamSallerForSubject(iam.CurOrder.SubjectID)));
            }
        }


        string get_emailmess(string good_id)
        {
            string r = "ondblclick=\"openflywin('../common/gimail.aspx?id=" + good_id + "', 400, 300, 'Запрос информации о товаре')\"";
            return r;
        }
        string stategood(int goodId, decimal qty, string zn, string zn_ost, decimal qty_need = 0)
        {
            string ret = "";
            if (zn_ost == "NL" || zn_ost == "P2" || zn_ost == "PZ")
            {
                ret = "<span class='goodincash-full'>в наличии</span>";
            }
            else
            {

                ret = (qty - qty_need > 0) ? ((qty < 5 && zn == "S") ? "<span class='goodincash-little'>есть, но мало</span>" : "<span class='goodincash-full'>в наличии</span>") : "<img src='../simg/32/ph.png' style='cursor:pointer' onclick=\"openflywin('../common/gimail.aspx?id=" + goodId + "', 400, 300, 'Запрос информации о товаре')\" title='по запросу' alt='?'/>";

            }
            return ret;
        }

        protected void dgOrder_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                OrderItem r = (OrderItem)e.Item.DataItem;

                bool ordReadOnly = OrderReadOnly;

                if (ordReadOnly)
                    e.Item.Cells[1].Text = "";
                e.Item.Attributes.Add("id", "tr_" + r.GoodId);
                e.Item.Attributes.Add("onclick", "thisrow(this.id)");
                e.Item.Cells[3].Text = r.Name + " " + get_angood(r);


                if (r.Mark == "*")
                    e.Item.Cells[2].CssClass = e.Item.Cells[3].CssClass = e.Item.Cells[4].CssClass = e.Item.Cells[6].CssClass = e.Item.Cells[7].CssClass = "blue";
                e.Item.Cells[4].Attributes.Add("id", "pr_" + r.GoodId);
                //e.Item.Cells[4].Text = "<input readonly type='text' style='border:0; text-align:right; width:50px' id='pr_" + r.GoodId + "' value='" + r.Price + "'>";
                e.Item.Cells[5].Text = "<input " + ((ordReadOnly) ? "readonly" : "") + " class='center qtyfield inputqty' maxlength='5' name='q_" + r.GoodId + "' id='q_" + r.GoodId + "' type='text' onchange=\"checkNumeric(this.id,1);recount('" + r.GoodId + "');\" value='" + cNum.cToDecimal(r.Qty, 1) + "' />";

                e.Item.Cells[6].Text = "<img class='plusminus' src='../simg/16/btn_up.gif' alt='+' title='+1' onclick=\"plusqo('" + r.GoodId + "')\"/><br/><img class='plusminus' src='../simg/16/btn_dwn.gif' alt='-' title='-1' onclick=\"minusqo('" + r.GoodId + "')\"/>";



                e.Item.Cells[8].Attributes.Add("id", "sm_" + r.GoodId);

                e.Item.Cells[9].Text = "<input maxlength='150' style='width:100px;' name='ds_" + r.GoodId + "' id='ds_" + r.GoodId + "' type='text' " + ((ordReadOnly) ? "readonly" : "") + " class='descritem' value='" + r.Descr + "' onchange=\"chngitemdescr('" + r.GoodId + "')\"/>";
                string osttext = wstcp.order.detailfly.get_ost_info(iam.CurOrder.State, r.Zn, r.CurIncash, r.Qty, r.Booking, r.Realized, r.WDate, r.Comment, r.SrokPost);

                e.Item.Cells[10].Text = osttext;
                if (r.ens != "")
                    e.Item.Cells[10].Text += "<img style='cursor:pointer; margin:1px;' onclick=\"openflywin('http://santex-ens.webactives.ru/get/" + r.ens + "/?key=x9BS1EFXj0', 1000,700,'Описание товара')\" src='../simg/16/detail.png'/ alt='o' title='посмотреть детали'>";
                if (webIO.CheckExistFile("../media/gimg/" + r.img))
                    e.Item.Cells[10].Text += "<img class='microimg' src='../media/gimg/" + r.img + "' id='imgeye" + r.GoodId + "'/><img style='margin:1px;' id='eye" + r.GoodId + "' class='eye' src='../simg/16/photo.png'/> ";

            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {

                e.Item.Cells[1].ColumnSpan = 6;//.Attributes.Add("colspan", "6");
                e.Item.Cells[2].ColumnSpan = 3;//.Attributes.Add("colspan", "3");
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                e.Item.Cells.RemoveAt(3);
                Order ord = iam.CurOrder;
                decimal d = ord.Discount;
                if (d > 0)
                {

                    e.Item.Cells[1].Text = "СУММА со скидкой<br/>";

                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + " руб.<br/>";
                    e.Item.Cells[1].Text += "предоставлена скидка";
                    e.Item.Cells[2].Text += "" + cNum.cToDecimal(d * 100, 2) + "%";

                }
                else
                {
                    e.Item.Cells[1].Text = "СУММА";
                    e.Item.Cells[2].Text = "" + cNum.cToDecimal(ord.Summ, 2) + "руб.";

                }
                e.Item.Cells[2].Attributes.Add("id", "orditg");


            }
        }



        private string get_angood(OrderItem r)
        {
            string ret = "";
            ret += ((r.an_k > 0) ? "<a href='#' class='linkbutton small' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=k',500,500,'Комплектующие'); title='У позиции есть комплектующие'>к</a>" : "") +
                ((r.an_s > 0) ? "<a href='#' class='linkbutton small' title='У позиции есть сопутствующие товары' onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=s',500,500,'Сопутствующие');>c</a>" : "") +
                ((r.an_a > 0) ? "<a href='#' class='linkbutton small' title='У позиции есть аналоги'onclick=openflywin('../good/angood.aspx?reg=fly&good=" + r.GoodCode + "&ant=a',500,500,'Аналоги');>а</a>" : "");
            return ret;
        }

        

        protected void dgOrder_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            int id = cNum.cToInt(e.Item.Cells[0].Text);

            iam.CurOrder.RemoveItem(id);

            
            load_curentorder();
        }




        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {

            Order ord = iam.CurOrder;
            ord.Descr = txDescr.Text;
            if (txNameOrder.Text.Trim() == "")
                txNameOrder.Text = "Заявка от " + ord.RegDate.ToShortDateString() + " " + ord.RegDate.ToShortTimeString();
            ord.Name = txNameOrder.Text;
            ord.AuthorID = iam.ID;
            if (ord.IsNew) ord.Dg = ord.Subject.CodeDG;
            bool result = Order.Save(ord);

            //lbOrder.Text = "" + ord.ID;
            //lbRegDate.Text = ord.RegDate.ToShortDateString();
            if (result)
            {
                try
                {
                    string emailTo = (IsDev) ? "alexzh@santur.ru" : ord.Subject.EmailTAs;
                    EmailMessage msg = new EmailMessage(
                        iam.Email,
                        emailTo,
                        (ord.Code == "") ? "Создана новая заявка " : "Скорректирована заявка ",
                        "<p>Клиент " + ord.Subject.Name + ((ord.Code == "") ? " оформил" : " изменил") + " Заявку №" + ord.ID + " на сумму " + ord.Summ + "руб.</p><p>Автор " + ord.Author.Name + ", " + ord.Author.Email + ", " + ord.Author.Phones + ".</p>" + ((ord.Descr == "") ? "<p>Комментария нет</p>" : "<p>Комментарий к заявке: <i>" + ord.Descr + "</i></p>")
                        );
                    msg.Send(CurrentCfg.EmailSupport);
                }
                catch
                {
                }
                eID = ord.ID;
                iam.CurOrder = null;
                Session["taborders"] = "actualNU";
                Response.Redirect("../default.aspx");



            } else
            {
                lbMessage.Text = "Ошибка при сохранении заявки";
            }
        }





        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            iam.CurOrder = null;
            Response.Redirect("../order/orderdefault.aspx");
            // скорее всего нужно переход на выбор товара сделать 

            pnlFormOrder.Visible = true;
            pnlResult.Visible = false;
            eID = 0;
            iam.CurOrder = new Order(eID, iam);

            iam.CurOrder.SubjectID = iam.PresentedSubjectID;
            iam.CurOrder.State = "N";
            
            txDescr.Text = "";
            txNameOrder.Text = "";
            lbOrder.Text = "";
            lbRegDate.Text = "";
            
            
            CommentList1.Visible = false;
            lbStateOrder.Text = iam.CurOrder.StateDescr;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            iam.CurOrder = new Order(iam.CurOrder.ID, iam);
            load_curentorder();
        }




        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            if (mvForm.GetActiveView() != vDelete)
                mvForm.SetActiveView(vDelete);
            else
            {

                int id = iam.CurOrder.ID;
                bool res = Order.Delete(iam.CurOrder, iam);
                if (res)
                {
                    iam.CurOrder = null;
                    mvForm.SetActiveView(vResult);
                    lbResult.Text = "Заявка №" + id + " удалена";
                }
            }
        }

 
        protected void txDescr_PreRender(object sender, EventArgs e)
        {
            txDescr.Attributes.Add("onchange", "setdescr()");
        }

        protected void txNameOrder_PreRender(object sender, EventArgs e)
        {
            txNameOrder.Attributes.Add("onchange", "setname()");
        }







    }
}