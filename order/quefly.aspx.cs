using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using selforderlib;
using ensoCom;
using System.Data;

namespace wstcp.order
{
    public partial class quefly : p_p
    {
        Order RECORD;

        int SubjectId
        {
            get { return cNum.cToInt(ViewState["SubjectID"]); }
            set { ViewState["SubjectID"] = value; }
        }

        private Owner owner;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (iam.ID <= 0)
            {
                Response.Write("Запрещено без авторизации");
                Response.End();
                return;
            }
            if (!IsPostBack)
                eID = cNum.cToInt(Request["id"]);

            if (RECORD == null) RECORD = new Order(eID, iam);
            owner = new Owner(RECORD.OwnerID);

            if (!IsPostBack)
            {
                SubjectId = RECORD.SubjectID;
                show_record();
            }
        }



        private void show_record()
        {
            //string script = "";
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "setActPunct", script, false);



            lbSubject.Text = RECORD.Subject.Name + ", ИНН " + RECORD.Subject.INN;
            lbTitle.Text = RECORD.Name;
            lbAttr.Text = "№" + RECORD.ID + " от " + RECORD.RegDate.ToShortDateString();
            if (RECORD.State != "D" 
                //&& webIO.CheckExistFile("../exch/"+RECORD.ID+".xls") 
                )
            {
                linkInvoice.Text = "<a href=\"../downloadfile.ashx?act=csv&id=" + RECORD.ID + "&sid=" + iam.SessionID + "\" title='скачать прайс-лист'><img src='../simg/16/xlsx16.gif' /> скачать прайс-лист CSV</a><br/>"+
                    //"<a href=\"../downloadfile.ashx?act=xls&id=" + RECORD.ID + "&sid=" + iam.SessionID + "\" title='скачать прайс-лист'><img src='../simg/16/xlsx16.gif' /> скачать прайс-лист XLS</a><br/>"+
                    "<a href=\"../downloadfile.ashx?act=xml&id=" + RECORD.ID + "&sid=" + iam.SessionID + "\" title='скачать прайс-лист'><img src='../simg/16/xlsx16.gif' /> скачать прайс-лист XML</a>";
                
//                linkInvoice.Text = "<a href=\"../exch/" + RECORD.ID + ".xls\" target=\"_blank\"  title='скачать прайс-лист'><img src='../simg/16/xlsx16.gif' /> ссылка на прайс-лист </a>";
            }
            else
            {
                linkInvoice.Text = "";
            }

            lbDescr.Text = RECORD.Descr;


            DataGrid1.DataSource = RECORD.Items;
            DataGrid1.DataBind();


        }




        //public static string get_info_enoutgh(decimal need_qty, decimal incash)
        //{
        //    return (need_qty < incash) ? "на складе в наличии" : (need_qty == incash) ? "возможно хватит если быстро" : (incash > 0) ? "можно получить только " + incash : "нет на складе";
        //}

        //public static string get_ost_info(string current_state, string zn, decimal incash, decimal qty_in_order, decimal booking, decimal realized, DateTime WDate, string comment, int srokPost)
        //{

        //    string osttext = "";
        //    decimal ost = qty_in_order - realized;

        //    if (current_state.ToUpper() == "F" || ost == 0)
        //    {
        //        osttext = "<div>полностью отгружено!</div>";
        //    }
        //    else if (current_state.ToUpper() == "R")
        //    {
        //        if (realized > 0) osttext = "<div>осталось отгр. " + ost + "</div>";


        //        if (booking > 0)
        //            osttext += "<div>можно получить сегодня " + booking + "</div>";
        //        else // booking==0
        //        {
        //            if (zn.ToUpper() == "S")
        //            {
        //                osttext += "<div>" + get_info_enoutgh(qty_in_order, incash) + "</div>";
        //            }
        //            else if (zn == "del")
        //            {
        //                osttext += "<div class='red'>выведена из ассортимента, обратитесь за заменой</div>";
        //            }
        //            else // zn == "Z"
        //            {

        //                if (incash > 0)
        //                    osttext += "<div title='Позиция заказная, согласуйте возможность отгрузки с менеджером'><span class='red'>*</span> " + get_info_enoutgh(ost, incash) + "</div>";
        //                else if (WDate != cDate.DateNull)
        //                    osttext += "<div>ожидается " + cDate.cToString(WDate) + "</div>";
        //                else if (srokPost > 0)
        //                    osttext += "<div title='Срок отсчитывается со дня подтверждения клиентом'> срок пост. " + srokPost + "дн.</div>";

        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (zn.ToUpper() == "S")
        //        {
        //            osttext += (incash > 0) ?
        //                "<div>" + get_info_enoutgh(qty_in_order, incash) + "</div>"
        //                : ((WDate > cDate.DateNull) ? "<div>возможная дата поставки " + cDate.cToString(WDate) + "</div>" : "<div>срок поставки уточнить у менеджера</div>");
        //        }
        //        else // Z
        //        {
        //            if (incash > 0)
        //            {
        //                osttext += "<div title='Позиция заказная, но на свободном остатке есть, обратитесь к своему менеджеру'><span class='red'>*</span> " + get_info_enoutgh(qty_in_order, incash) + "</div>";


        //            }
        //            else
        //            {
        //                if (WDate > cDate.DateNull)
        //                    osttext += "<div>под заказ, возможная дата " + cDate.cToString(WDate) + " </div>";
        //                else
        //                    osttext += (srokPost > 0) ?
        //                    "<div title='Срок отсчитывается со дня подтверждения клиентом'>" + srokPost + "дн.</div>"
        //                    :
        //                    "<div>под заказ, срок поставки уточнить у менеджера</div>";
        //            }


        //        }

        //    }
        //    return osttext;

        //}


        protected void DataGrid1_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {

                OrderItem r = (OrderItem)e.Item.DataItem;

                decimal q = cNum.cToDecimal(r.CurIncash,2);
                if (owner.ShowIncash)
                {
                    e.Item.Cells[3].Text = "" + q;
                } else
                {
                    if (q <= 0)
                        e.Item.Cells[3].Text = "нет";
                    else
                    {

                        if (cStr.Exist(r.Zn_z,new []{"NL","P2","PZ"}))
                        {
                            e.Item.Cells[3].Text = "<span class='goodincash-full'>в наличии</span>";
                        } else
                        {
                            e.Item.Cells[3].Text = (q < 5 && r.Zn == "S") ? "<span class='goodincash-little'>есть, но мало</span>" : "<span class='goodincash-full'>в наличии</span>";
                        }
                    }
                }
                

                if (r.Zn == "Z")
                    e.Item.Cells[4].Text = "Товар заказной";
                else if (r.Zn == "Q")
                    e.Item.Cells[4].Text = "выводится из ассортимента";
                else if (r.Zn == "N")
                    e.Item.Cells[4].Text = "новинка";
                else if (r.Zn == "G")
                    e.Item.Cells[4].Text = "нестандартная позиция";


                
                

            }
            //else if (e.Item.ItemType == ListItemType.Footer)
            //{
            //    e.Item.Cells.Clear();
            //    e.Item.Cells.Add(new TableCell());
            //    e.Item.Cells.Add(new TableCell());
            //    e.Item.Cells.Add(new TableCell());
            //    e.Item.Cells[0].Attributes.Add("colspan", "4");
            //    e.Item.Cells[2].Attributes.Add("colspan", "4");
                

            //}
        }




        

       
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            RECORD = new Order(eID, iam);

            show_record();
        }

        protected void btnMakefile_Click(object sender, EventArgs e)
        {
            try
            {
 

            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\exch\";
            string file = RECORD.ID+ ".csv";
                WebClient wc = new WebClient();
                wc.Credentials = new NetworkCredential("sp_imp", "yQo9m7&0");
                Uri ftp = new Uri("ftp://89.108.97.34");
            string line = "";
            MemoryStream ms = new MemoryStream();
            StreamWriter wr = new StreamWriter(ms, Encoding.UTF8);
            string qstr, comment;
            line = "код" + ";" + "Наименование" + ";" + "Цена, руб" + ";" + "Наличие" + ";" + "Примечание" + ";" + "Комментарий";
            wr.WriteLine(line);
            foreach (OrderItem r in RECORD.Items)
            {
                line = "" + r.GoodCode + ";" + r.Name;

                decimal q = cNum.cToDecimal(r.CurIncash, 2);
                if (owner.ShowIncash)
                {
                    qstr = "" + q;
                }
                else
                {
                    if (q <= 0)
                        qstr = "-";
                    else
                    {

                        if (cStr.Exist(r.Zn_z, new[] { "NL", "P2", "PZ" }))
                        {
                            qstr = "в наличии";
                        }
                        else
                        {
                            qstr = (q < 5 && r.Zn == "S") ? "есть, но мало" : "в наличии";
                        }
                    }
                }


                if (r.Zn == "Z")
                    comment = "Товар заказной";
                else if (r.Zn == "Q")
                    comment = "выводится из ассортимента";
                else if (r.Zn == "N")
                    comment = "новинка";
                else if (r.Zn == "G")
                    comment = "нестандартная позиция";
                else
                    comment = "";
                
                line += ";" +r.Price+";"+ qstr + ";" + comment + ";" + r.Descr;


                wr.WriteLine(line);
            }
            wr.Close();
                byte[] fl = ms.GetBuffer();
                wc.UploadData(ftp+"/"+file, fl);
                ms.Close();
            //fs.Close();
            linkInvoice.Text = "<div>заберите выгруженный файл: <a href='../exch/" + file + "'>" + file + "</a></div>";
            }
            catch (Exception ex)
            {
                linkInvoice.Text = "возникла ошибка при создании файла.";
                try
                {
                    EmailMessage msg = new EmailMessage(CurrentCfg.EmailPortal, "alexzh@santur.ru", "Ошибка при формировании прайса", "<p>" + iam.Name + " (представитель " + iam.SubjectID + ") пытался сохранить файл прайс листа. Ошибка:</p> <fieldset>" + ex.Message + "</fieldset>");
                    msg.Send();
                    linkInvoice.Text += "Сообщение об ошибке отправлено в службу поддержки.";
                }
                catch
                {
                    
                }
            }
        }

        //private void make_xls_file()
        //{
        //    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
        //    ExcelApp.Application.Workbooks.Add(Type.Missing);


        //    for (int i = 1; i < DataGridView.Columns.Count + 1; i++)
        //    {
        //        ExcelApp.Cells[1, i] = DataGridView.Columns[i - 1].HeaderText;
        //    }


        //    for (int i = 0; i < DataGridView.Rows.Count - 1; i++)
        //    {
        //        for (int j = 0; j < DataGridView.Columns.Count; j++)
        //        {
        //            ExcelApp.Cells[i + 2, j + 1] = DataGridView.Rows[i].Cells[j].Value.ToString();
        //        }
        //    }

        //    ExcelApp.ActiveWorkbook.SaveCopyAs("E:\\файл.xls");
        //    ExcelApp.ActiveWorkbook.Saved = true;
        //    ExcelApp.Quit();
        //}
    }
}