using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ensoCom;
using System.Text;


    public partial class imgselect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             divBrowse.Text = load_browse();
        }
        public string load_browse()
        {
            StringBuilder sb = new StringBuilder();
            string up_level = "";
            if (hdSrvPath.Value.Length > 2)
            {
                string[] pp = hdSrvPath.Value.Replace(@"///", "/").Replace("//", "/").Split('/');
                string p_go = (pp.Length > 0 && pp[pp.Length - 1] != "") ? hdSrvPath.Value.Replace(pp[pp.Length - 1], "") : hdSrvPath.Value;
                up_level = " <span class='bold' style='cursor:pointer;width:60px; height:60px' onclick=\"go_to('" + hdSrvPath.ClientID + "','" + p_go + "')\"> <br /><br />..<br />наверх</span>";
            }
            else
            {
                hdSrvPath.Value = "";
            }
            string path = Server.MapPath(HelpersPath.MediaRootVirtualPath + hdSrvPath.Value + "/" + "index.htm");
            string[] dirs = Directory.GetDirectories(Server.MapPath(HelpersPath.MediaRootVirtualPath + hdSrvPath.Value));
            string[] filespath = Directory.GetFiles(Server.MapPath(HelpersPath.MediaRootVirtualPath + hdSrvPath.Value));
            string ext = "";
            int qimg = 0;
            for (int i = 0; i < filespath.Length; i++)
            {
                ext = Path.GetExtension(filespath[i]);
                if (ext == ".jpg" || ext == ".png" || ext == ".gif")
                    qimg += 1;

            }
            string[] all = new string[((up_level.Length > 0) ? 1 : 0) + dirs.Length + qimg];
            if (up_level != "") all[0] = up_level;
            int sti = (up_level.Length > 0) ? 1 : 0;
            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo di = new DirectoryInfo(dirs[i]);
                all[sti + i] = "<span onclick=\"go_to('" + hdSrvPath.ClientID + "','" + hdSrvPath.Value + "/" + di.Name + "')\"><img style='cursor:pointer;' src='../SIMG/32/folder.png' title='"+di.Name+"' /><br />" + cStr.CutString(di.Name,10) + "</span>";
            }
            sti += dirs.Length;
            int zi = 0;
            int mwidth = 300;

            decimal p_s;
            string img_name;
            string img_src;
            System.Drawing.Image img;
            int w=60, h=60;
            foreach (string imgp in filespath)
            {
                ext = Path.GetExtension(imgp);
                if (ext == ".jpg" || ext == ".png" || ext == ".gif" )
                {
                    
                    img_name = Path.GetFileName(imgp);
                    img_src = (HelpersPath.MediaRootVirtualPath + hdSrvPath.Value + (((HelpersPath.MediaRootVirtualPath + hdSrvPath.Value).Length > 2) ? "/" : "") + img_name).Replace("//", "/");
                    try
                    {
                        img = System.Drawing.Image.FromFile(HelpersPath.FisicalRoot + img_src.Replace("../", "").Replace("/", "\\"));

                        

                        p_s = ((decimal) mwidth)/((img.Height > img.Width) ? img.Height : img.Width);
                        if (p_s > 1) p_s = 1;
                        w = cNum.cToInt(img.Width*p_s);
                        h = cNum.cToInt(img.Height*p_s);
                    }
                    catch (Exception ex)
                    {
                        w = 100;
                        h = 100;
                    }
                    all[sti + zi] = "<img style='width:60px; height:60px; cursor:pointer;' src='" + img_src + "' onclick=\" selectImg('" + img_src + "');showselimg('" + img_src + "', " + w + ", " + h + ");\" title='"+img_name+"' /><br />" + cStr.CutString(img_name,10) + "";
                    zi += 1;
                }
            }

            for (int ind_img = 0; ind_img < all.Length; ind_img++)
            {
               sb.Append("<li id='tb" + ind_img + "' onmouseover=\"thisRow(this.id)\" >" + all[ind_img] + "</li>");
            }
            return sb.ToString();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string filetype = FileUpload1.PostedFile.ContentType;
            string file = Path.GetFileName(FileUpload1.PostedFile.FileName);
            string pathtosave = HelpersPath.MediaFisicalPath + hdSrvPath.Value + "/" + file;
            FileUpload1.SaveAs(pathtosave.Replace("///", "/").Replace("//", "/").Replace(@"\/", "/").Replace("/", "\\"));
            divBrowse.Text = load_browse();
        }
        protected void btnCreateNewFolder_Click(object sender, EventArgs e)
        {
            if (txNewFolder.Text.Length > 0)
            {
                try
                {
                    Directory.CreateDirectory(HelpersPath.MediaFisicalPath + hdSrvPath.Value + "/" + txNewFolder.Text);
                    hdSrvPath.Value += ((hdSrvPath.Value != "") ? "/" : "") + txNewFolder.Text;
                    txNewFolder.Text = "";
                    divBrowse.Text = load_browse();
                }
                catch
                {
                    txNewFolder.Text = "";
                }
            }
        }
        protected void reload_Click(object sender, EventArgs e)
        {
            divBrowse.Text = load_browse();
        }
    }
