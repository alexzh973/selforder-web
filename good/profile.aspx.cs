using System.Drawing;
using ensoCom;
using selforderlib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;

namespace wstcp.good
{
    public partial class profile : p_p
    {
        GoodOwnInfo gi {get{return  GoodInfo.GetInfo(eID, iam.OwnerID, typecen);}}

        private string typecen
        {
            get { return ""+ViewState["priceType"]; }
            set { ViewState["priceType"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((mainpage)this.Master).SelectedMenu = "incash";
                ((mainpage)this.Master).VisibleLeftPanel = false;
                ((mainpage)this.Master).VisibleRightPanel = false;
                eID = cNum.cToInt(Request["id"]);
                BreadPath1.AddPuncts("Вся номенклатура","list.aspx");
                typecen = Request["typecen"];
                if (typecen == "")
                    typecen = "pr_b";
                if (iam.PresentedSubjectID > 0)
                {
                    Subject subj = new Subject(iam.PresentedSubjectID, iam);
                    typecen = subj.PriceType;
                }
                showdetail();
                show_acc(eID, iam.OwnerID);
            }
        }

        private void showdetail()
        {

            
            

            //GoodOwnInfo gi = GoodInfo.GetInfo(eID, iam.OwnerID, priceType);
            lbName.Text = gi.Name;
            lbPrice.Text = wstcp.good.gdetail.GetNameCen(typecen) + ": " + cNum.cToDecimal(gi.PriceSale, 2) + "руб.";

            tblIncash.Text = prepareIncashTable(eID);


            imgGood.ImageUrl = (gi.img != "noimgs") ? "../img.ashx?act=good&id=" + gi.GoodId : "../media/nophoto.gif";


        }

        private void show_acc(int id, int ownerid)
        {
            DataTable ga = db.GetDbTable("select * from accgood t1 inner join ACC t2 on t1.accid=t2.id and goodId=" + id + " and t2.OwnerId=" + ownerid + " order by t2.startDate desc");

            dgAcc.DataSource = ga;
            dgAcc.DataBind();

            //DataTable acc = db.GetDbTable("select Name, StartDate, FinishDate, (select count(goodId) from ACCGOOD where goodID="+id+" and ACC.ID=accid), (select count(inq.goodid) from accgood as inq where inq.Accname=exq.accname and inq.ownerid=exq.ownerid and inq.goodId=" + id + " ) as isacc from accgood as exq where OwnerId=" + ownerid + " and LastDate>=getdate() group by AccName, FirstDate, LastDate, OwnerId order by FirstDate desc");
            //chlAcc.Items.Clear();
            //foreach (DataRow r in acc.Rows)
            //{
            //    ListItem li = new ListItem("" + r["Name"]+"("+cDate.cToString(r["startDate"])+" - "+cDate.cToString(r["finishDate"])+")", "" + r["AccName"]);
            //    if ("" + r["isacc"] == "1")
            //        li.Selected = true;
            //    chlAcc.Items.Add(li);

            //}

        }


        private string prepareIncashTable(int id)
        {
            DataTable dt = db.GetDbTable("select id as OwnerID, Name, isnull( (select SUM(qty) from OWNG where OwnerId=OWN.id and GoodId=" + id + "),0) as qty, (select max(lcd) from OWNG where OwnerId=OWN.id and GoodId=" + id + ") as lcd  from OWN ");//wstcp.Models.GoodInfo.GetIncashTable(good_id);
            string resp = ""; decimal q = 0;
            if (dt.Select("qty>0").Length > 0)
            {
                resp = "<table width='300px'>";

                foreach (DataRow r in dt.Rows)
                {
                    q = cNum.cToDecimal(r["qty"]);

                    resp += String.Format("<tr><td >{0}</td><td >{1}</td></tr>", r["Name"], ((q == 0) ? "-" : "" + q));
                }


                resp += "</table>";
            }
            else
            {
                resp = "в подразделениях нет остатков";
            }
            return resp;
        }

        //protected void btnSaveNewAcc_Click(object sender, EventArgs e)
        //{
        //    db.ExecuteCmd("delete from ACCGOOD where goodId=" + eID + " and OwnerId=" + iam.OwnerID);
        //    foreach (ListItem li in chlAcc.Items)
        //        if (li.Selected)
        //            db.ExecuteCmd("insert into ACCGOOD (GoodID,AccName,FirstDate,LastDate,OwnerId,lcd, lc) values (" + eID + ",'" + li.Value + "',(select top 1 FirstDate from ACCGOOD as inq where inq.ownerid=" + iam.OwnerID + " and inq.AccName='" + li.Value + "'),(select top 1 LastDate from ACCGOOD as inq where inq.ownerid="+iam.OwnerID+" and inq.AccName='" + li.Value + "')," + iam.OwnerID + ",getdate(), '" + iam.Email + "')");
        //    if (txNewAcc.Text != "")
        //        db.ExecuteCmd("insert into ACCGOOD (GoodID,AccName,FirstDate,LastDate,OwnerId,lcd, lc) values (" + eID + ",'" + txNewAcc.Text + "'," + cDate.Date2Sql(ucFirstDay.SelectedDate) + "," + cDate.Date2Sql(ucLastDay.SelectedDate) + "," + iam.OwnerID + ",getdate(), '" + iam.Email + "')");
        //    show_acc(eID, iam.OwnerID);
        //}

        protected void btnUploadImg_Click(object sender, EventArgs e)
        {
            string filetype = FileUpload1.PostedFile.ContentType;
            if (filetype.IndexOf("image") < 0 || FileUpload1.PostedFile.ContentLength > 10000000) return;
            System.Drawing.Image img = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
            int h = img.Height;
            int w = img.Width;
            int size_s = 64;
            int size_b = 250;
            decimal p_s, p_b;

            p_s = ((decimal)size_s) / ((h > w) ? h : w);
            p_b = (((h > w) ? h : w) > size_b) ? p_s * (size_b / size_s) : 1;
            string hashcode = "" + h + "_" + w + "_" + FileUpload1.PostedFile.ContentLength;
            //Bitmap bmp_s = new Bitmap(img, cNum.cToInt(img.Width * p_s), cNum.cToInt(img.Height * p_s));
            Bitmap bmp_b = new Bitmap(img, cNum.cToInt(img.Width * p_b), cNum.cToInt(img.Height * p_b));
            try
            {

                MemoryStream ms = new MemoryStream();
                bmp_b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);


                byte[] buf = ms.ToArray();
                ms.Close();
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.Parameters.Add("@goodId", SqlDbType.Int);
                cmd.Parameters.Add("@src", SqlDbType.Image);
                cmd.Parameters.Add("@hashcode", SqlDbType.NVarChar);
                cmd.Parameters.AddWithValue("@ext", ".png");
                
                cmd.Parameters["@goodId"].Value = gi.GoodId;
                cmd.Parameters["@src"].Value = buf;
                cmd.Parameters["@hashcode"].Value = hashcode;


                cmd.CommandText = "update GIMG set src=@src, ext=@ext where goodId=@goodId and hashcode=@hashcode";
                if (cmd.ExecuteNonQuery() == 0)
                {
                    cmd.CommandText = "insert into gimg (goodId,src, ext, hashcode) values (@goodId,@src,@ext,@hashcode)";
                    cmd.ExecuteNonQuery();
                }



                //bmp_b.Save(HelpersPath.MediaFisicalPath + @"/gimg/" + gi.GoodId + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //if (webIO.CheckExistFile("../media/gimg/" + gi.GoodId+".png"))
                //{
                //    db.ExecuteCmd("update GOOD set img='" + gi.GoodId + ".png' where id=" + gi.GoodId);
                //}
                
            }
            catch (Exception ex)
            {
            }
                
            finally
            {
                img = null;
                imgGood.ImageUrl = "../img.ashx?act=good&id="+gi.GoodId;
            }
            //hdPhoto.Value = "tmp_usr_" + iam.ID;
            

        }
    }
}