using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ensoCom;
using selforderlib;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для downloadfile
    /// </summary>
    public class downloadfile : IHttpHandler
    {
        private void ResponceMistake(HttpContext context, string mistake)
        {
            byte[] err = cStr.ToByteArray(mistake);
            context.Response.OutputStream.Write(err, 0, err.Length);
            context.Response.End();
            
        }

        public void ProcessRequest(HttpContext context)
        {
            byte[] err;
            IAM iam = IamServices.GetIam(context.Request["sid"]);
            if (iam.ID < 100000)
            {
                ResponceMistake(context, "Ошибка авторизации/доступа");
                return;
            }
            string act = context.Request["act"];
            int id = cNum.cToInt(context.Request["id"]);
            Order RECORD;
            string filename;



            if (act == "invoice")
            {

                byte[] bufinvoice;
                if (webIO.CheckExistFile("../exch/" + id + ".pdf"))
                {
                    bool upd = true;
                    SqlConnection cn = new SqlConnection(db.DefaultCnString);
                    try
                    {
                        cn.Open();
                        SqlCommand cmd = cn.CreateCommand();
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.Parameters.Add("@src", System.Data.SqlDbType.Image);

                        FileStream stream = new FileStream(webIO.GetAbsolutePath("../exch/" + id + ".pdf"), FileMode.Open);
                        BinaryReader reader = new BinaryReader(stream);
                        bufinvoice = reader.ReadBytes((int)stream.Length);
                        reader.Close();
                        stream.Close();


                        cmd.Parameters["@src"].Value = bufinvoice;
                        cmd.CommandText = "update " + Order.TDB + " set invoicesrc=@src where id=" + id;
                        cmd.ExecuteNonQuery();

                    }
                    catch
                    {
                        upd = false;
                        ResponceMistake(context, "Ошибка при попытке сформировать счет");
                    }
                    finally
                    {
                        cn.Close();
                    }
                    if (upd)
                        File.Delete(webIO.GetAbsolutePath("../exch/" + id + ".pdf"));
                }
                DataTable dt = new DataTable();
                dt = db.GetDbTable("select code, invoicesrc from " + Order.TDB + " where id=" + id);
                if (dt.Rows.Count > 0 && dt.Rows[0]["invoicesrc"] != null && dt.Rows[0]["invoicesrc"].GetType().Name=="Byte[]")
                {
                    bufinvoice = (byte[])dt.Rows[0]["invoicesrc"];
                    context.Response.ContentType = webIO.GetFileContentType(".pdf");
                    context.Response.AddHeader("Content-Disposition", "attachment;filename=СчетНаОплату" + dt.Rows[0]["code"] + "");
                    context.Response.OutputStream.Write(bufinvoice, 0, bufinvoice.Length);
                    context.Response.End();
                }
                else
                {
                    ResponceMistake(context, "Счет на оплату не обнаружен");
                }
            }
            else if (act == "csv")
            {

                RECORD = new Order(id, iam);
                filename = RECORD.Name + ".csv";



                MemoryStream ms = new MemoryStream();
                StreamWriter wr = new StreamWriter(ms, Encoding.UTF8);
                string qstr, comment;
                string line = "код" + ";" + "Наименование" + ";" + "Цена, руб" + ";" + "Наличие" + ";" + "Примечание" + ";" + "Комментарий";
                wr.WriteLine(line);


                decimal q;
                bool showIncash = RECORD.OWner.ShowIncash;
                foreach (OrderItem r in RECORD.Items)
                {
                    line = "" + r.GoodCode + ";" + r.Name;
                    q = cNum.cToDecimal(r.CurIncash, 2);
                    if (showIncash)
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

                    line += ";" + r.Price + ";" + qstr + ";" + comment + ";" + r.Descr;

                    wr.WriteLine(line);
                }
                wr.Close();
                byte[] buf = ms.GetBuffer();

                context.Response.ContentType = webIO.GetFileContentType(".csv");
                context.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename + "");
                context.Response.OutputStream.Write(buf, 0, buf.Length);
                context.Response.End();
            }
            else if (act == "xml")
            {
                RECORD = new Order(id, iam);
                bool showIncash = RECORD.OWner.ShowIncash;
                DataTable pdt = new DataTable();
                pdt.TableName = "price" + RECORD.ID;
                pdt.Columns.Add("Код", typeof(string));
                pdt.Columns.Add("Наименование", typeof(string));

                pdt.Columns.Add("Цена_руб", typeof(decimal));
                if (showIncash)
                    pdt.Columns.Add("Остаток", typeof(decimal));
                else
                    pdt.Columns.Add("Остаток", typeof(string));
                pdt.Columns.Add("Комментарий", typeof(string));
                pdt.Columns.Add("Примечание", typeof(string));


                filename = iam.ID.ToString() + RECORD.ID + ".xls";

                decimal q;

                foreach (OrderItem r in RECORD.Items)
                {
                    DataRow nr = pdt.NewRow();
                    nr["Код"] = r.GoodCode;
                    nr["Наименование"] = r.Name;

                    q = cNum.cToDecimal(r.CurIncash, 2);
                    if (showIncash)
                    {
                        nr["Остаток"] = q;

                    }
                    else
                    {
                        if (q <= 0)
                            nr["Остаток"] = "-";
                        else
                        {

                            if (cStr.Exist(r.Zn_z, new[] { "NL", "P2", "PZ" }))
                            {
                                nr["Остаток"] = "в наличии";
                            }
                            else
                            {
                                nr["Остаток"] = (q < 5 && r.Zn == "S") ? "есть, но мало" : "в наличии";
                            }
                        }
                    }


                    if (r.Zn == "Z")
                        nr["Комментарий"] = "Товар заказной";
                    else if (r.Zn == "Q")
                        nr["Комментарий"] = "выводится из ассортимента";
                    else if (r.Zn == "N")
                        nr["Комментарий"] = "новинка";
                    else if (r.Zn == "G")
                        nr["Комментарий"] = "нестандартная позиция";
                    else
                        nr["Комментарий"] = "";

                    nr["Цена_руб"] = r.Price;
                    nr["Примечание"] = r.Descr;

                    pdt.Rows.Add(nr);

                }
                MemoryStream ms = new MemoryStream();
                pdt.WriteXml(ms);
                byte[] buf = ms.GetBuffer();

                context.Response.ContentType = webIO.GetFileContentType(".xml");
                context.Response.AddHeader("Content-Disposition", "attachment;filename=Прайс" + filename + "");
                context.Response.OutputStream.Write(buf, 0, buf.Length);
                context.Response.End();
            }
            else if (act == "xls")
            {
                RECORD = new Order(id, iam); bool showIncash = RECORD.OWner.ShowIncash;
                DataTable pdt = new DataTable();
                pdt.TableName = "price" + RECORD.ID;
                pdt.Columns.Add("Код", typeof(string));
                pdt.Columns.Add("Наименование", typeof(string));

                pdt.Columns.Add("Цена_руб", typeof(decimal));
                if (showIncash)
                    pdt.Columns.Add("Остаток", typeof(decimal));
                else
                    pdt.Columns.Add("Остаток", typeof(string));
                pdt.Columns.Add("Комментарий", typeof(string));
                pdt.Columns.Add("Примечание", typeof(string));


                filename = iam.ID.ToString() + RECORD.ID + ".xls";

                decimal q;

                foreach (OrderItem r in RECORD.Items)
                {
                    DataRow nr = pdt.NewRow();
                    nr["Код"] = r.GoodCode;
                    nr["Наименование"] = r.Name;

                    q = cNum.cToDecimal(r.CurIncash, 2);
                    if (showIncash)
                    {
                        nr["Остаток"] = q;

                    }
                    else
                    {
                        if (q <= 0)
                            nr["Остаток"] = "-";
                        else
                        {

                            if (cStr.Exist(r.Zn_z, new[] { "NL", "P2", "PZ" }))
                            {
                                nr["Остаток"] = "в наличии";
                            }
                            else
                            {
                                nr["Остаток"] = (q < 5 && r.Zn == "S") ? "есть, но мало" : "в наличии";
                            }
                        }
                    }


                    if (r.Zn == "Z")
                        nr["Комментарий"] = "Товар заказной";
                    else if (r.Zn == "Q")
                        nr["Комментарий"] = "выводится из ассортимента";
                    else if (r.Zn == "N")
                        nr["Комментарий"] = "новинка";
                    else if (r.Zn == "G")
                        nr["Комментарий"] = "нестандартная позиция";
                    else
                        nr["Комментарий"] = "";

                    nr["Цена_руб"] = r.Price;
                    nr["Примечание"] = r.Descr;

                    pdt.Rows.Add(nr);

                }

                //if (ExportToExcel.ExportJast(pdt, webIO.GetAbsolutePath("../exch/" + filename)))
                //{
                //    FileStream stream = new FileStream(webIO.GetAbsolutePath("../exch/" + filename), FileMode.Open);
                //    BinaryReader reader = new BinaryReader(stream);
                //    byte[] buf = reader.ReadBytes((int)stream.Length);
                //    context.Response.ContentType = webIO.GetFileContentType(".xls");
                //    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + RECORD.Name + "");
                //    context.Response.OutputStream.Write(buf, 0, buf.Length);
                //    context.Response.End();
                //}
                //else
                //{
                //    err = cStr.ToByteArray("ошибка");
                //    context.Response.OutputStream.Write(err, 0, err.Length);
                //    context.Response.End();
                //}

            }
            else
            {
                err = cStr.ToByteArray("ошибка");
                context.Response.OutputStream.Write(err, 0, err.Length);
                context.Response.End();
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}