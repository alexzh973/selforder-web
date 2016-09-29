using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ensoCom;
using System.Text;


    public partial class imgsel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack){
                txSelected.Text = HelpersPath.MediaRootVirtualPath;
                show_explorer(txSelected.Text);
            }
             //divBrowse.Text = load_browse();
        }

        private void show_explorer(string path)
        {
            StringBuilder sb = new StringBuilder();
            string[] folders = Directory.GetDirectories(Server.MapPath(path));
            string[] files = Directory.GetFiles(Server.MapPath(path));
            if (path != HelpersPath.MediaRootVirtualPath)
                sb.Append(" <a href='" + HelpersPath.MediaRootVirtualPath + "' class='bold' style='width:60px; height:60px' > <br /><br />..<br />наверх</a>");
            foreach (string fold in folders)
            {
                sb.Append(" <a href='" + String.Format("{0}/{1}", HelpersPath.MediaRootVirtualPath, fold) + "' class='bold' style='width:60px; height:60px' ><img style='cursor:pointer;' src='../SIMG/32/folder.png' title='" + fold + "' /><br />"+fold+"</a>");
            }

            int zi = 0;
            int mwidth = 300;

            List<string> all = new List<string>();
            foreach (string imgp in files)
            {
                string ext = Path.GetExtension(imgp);
                if (ext == ".jpg" || ext == ".png" || ext == ".gif")
                {
                    string img_name = Path.GetFileName(imgp);
                    //string img_src = (HelpersPath.MediaRootVirtualPath + hdSrvPath.Value + ((hdSrvPath.Value.Length > 2) ? "/" : "") + img_name).Replace("//", "/");
                    System.Drawing.Image img = System.Drawing.Image.FromFile(Server.MapPath(imgp));

                    decimal p_s;

                    p_s = ((decimal)mwidth) / ((img.Height > img.Width) ? img.Height : img.Width);
                    if (p_s > 1) p_s = 1;
                    int w = cNum.cToInt(img.Width * p_s);
                    int h = cNum.cToInt(img.Height * p_s);
                    all.Add("<img style='width:60px; height:60px; cursor:pointer;' src='" + imgp + "' onclick=\"showselimg('" + imgp + "', " + w + ", " + h + ");\" title='" + imgp + "' /><br />" + cStr.CutString(imgp, 10) + "");
                    zi += 1;
                }
            }

            for (int ind_img = 0; ind_img < all.Count; ind_img++)
            {
                sb.Append("<li id='tb" + ind_img + "' onmouseover=\"thisRow(this.id)\" >" + all[ind_img] + "</li>");
            }
            divBrowse.Text = sb.ToString();
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
            int qimg = 0;
            for (int i = 0; i < filespath.Length; i++)
            {
                string ext = Path.GetExtension(filespath[i]);
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


            foreach (string imgp in filespath)
            {
                string ext = Path.GetExtension(imgp);
                if (ext == ".jpg" || ext == ".png" || ext == ".gif" )
                {
                    string img_name = Path.GetFileName(imgp);
                    string img_src = (HelpersPath.MediaRootVirtualPath + hdSrvPath.Value + ((hdSrvPath.Value.Length > 2) ? "/" : "") + img_name).Replace("//", "/");
                    System.Drawing.Image img = System.Drawing.Image.FromFile(HelpersPath.FisicalRoot + img_src.Replace("../", "").Replace("/", "\\"));

                    decimal p_s;

                    p_s = ((decimal)mwidth) / ((img.Height > img.Width) ? img.Height : img.Width);
                    if (p_s > 1) p_s = 1;
                    int w = cNum.cToInt(img.Width * p_s);
                    int h = cNum.cToInt(img.Height * p_s);
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
