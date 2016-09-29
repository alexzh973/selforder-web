using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using ensoCom;



    public partial class UploadThrumbPicture : uc_base
    {
        public string DefaultVirtualImgPath
        {
            set { ViewState["DefaultVirtualImgPath"] = value; }
            get { return "" + ViewState["DefaultVirtualImgPath"]; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (DefaultVirtualImgPath!="") 
                    showthrumbs(DefaultVirtualImgPath);
            }
            if (IsPostBack && fuImg.FileName != "")
            {
                presaveFile();
            }
        }
        void presaveFile()
        {
            string filetype = fuImg.PostedFile.ContentType;
            if (filetype.IndexOf("image") < 0 || fuImg.PostedFile.ContentLength > 10000000)
            {
                lbResult.Text = "ошибка (либо размер оч большой либо вообще не изображение)";
                return;
            }
            lbResult.Text = "";
            string filename = System.IO.Path.GetFileName(fuImg.PostedFile.FileName);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fuImg.PostedFile.InputStream);
            int h = img.Height;
            int w = img.Width;
            int size_s = 64;
            decimal p_s;

            p_s = ((decimal)size_s) / ((h > w) ? h : w);
            if (p_s > 1) p_s = 1;

            Bitmap bmp_s = new Bitmap(img, cNum.cToInt(img.Width * p_s), cNum.cToInt(img.Height * p_s));
            try
            {                
                string tmppath = webIO.GetAbsolutePath(@"MEDIA") + @"\tmp_" + Session.SessionID + ".png";
                if (System.IO.File.Exists(tmppath))
                {
                    System.IO.File.Delete(tmppath);
                }
                bmp_s.Save(tmppath, System.Drawing.Imaging.ImageFormat.Png);
                
                    showthrumbs("../MEDIA/tmp_" + Session.SessionID + ".png");
                   
            }
            catch (Exception ex) { lbResult.Text = ex.ToString(); }

        }
        public void Save( )
        {
            if (DefaultVirtualImgPath == "") return;

            string tmppath = webIO.GetAbsolutePath(@"MEDIA") + @"\tmp_" + Session.SessionID + ".png";
            if (System.IO.File.Exists(tmppath))
            {
                try
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(webIO.GetAbsolutePath(DefaultVirtualImgPath))))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(webIO.GetAbsolutePath(DefaultVirtualImgPath)));
                    }
                    if (System.IO.File.Exists(webIO.GetAbsolutePath(DefaultVirtualImgPath)))
                    {
                        System.IO.File.Delete(webIO.GetAbsolutePath(DefaultVirtualImgPath));
                    }
                    System.IO.File.Move(tmppath, webIO.GetAbsolutePath(DefaultVirtualImgPath));
                    showthrumbs(DefaultVirtualImgPath);
                }
                catch (Exception ex) { lbResult.Text = ex.ToString(); }

            }


        }

        void showthrumbs(string virtpath)
        {
            if (System.IO.File.Exists(webIO.GetAbsolutePath(virtpath)))
            {
                lbImg.Text = "<img src='" + virtpath + "?" + DateTime.Now.ToLongTimeString() + "'>";
                btnDelete.Visible = true;
            }
            else
            {
                lbImg.Text = "";
                btnDelete.Visible = false;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(webIO.GetAbsolutePath(DefaultVirtualImgPath)))
            {
                System.IO.File.Delete(webIO.GetAbsolutePath(DefaultVirtualImgPath));
            }
            if (System.IO.File.Exists(webIO.GetAbsolutePath(@"MEDIA") + @"\tmp_" + Session.SessionID + ".png"))
            {
                System.IO.File.Delete(webIO.GetAbsolutePath(@"MEDIA") + @"\tmp_" + Session.SessionID + ".png");
            }
        }
    }
