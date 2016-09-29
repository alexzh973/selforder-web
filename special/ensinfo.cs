using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace wstcp.special
{
    public class ensinfo
    {

        public static string GetENSinfo(string code)
        {
            string htm_text = "";


            if (code.Trim() == "")
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            try
            {

                WebRequest request = WebRequest.Create(@"http://santex-ens.webactives.ru/get/" + code + @"/?key=x9BS1EFXj0");
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;

                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                //try
                //{
                //    string htm = wc.DownloadString(@"http://santex-ens.webactives.ru/get/" + code + @"/?key=x9BS1EFXj0");
                //}
                //catch
                //{

                //}
                Stream objStream = request.GetResponse().GetResponseStream();

                StreamReader objReader = new StreamReader(objStream);



                htm_text = objReader.ReadToEnd();
                sb.Clear();
                List<string> imgs = new List<string>();
                if (htm_text.IndexOf(@"<img src=""/data/thumbs/") >= 0)
                {

                    string qstr = htm_text.Substring(htm_text.IndexOf(@"<img src=""/data/thumbs/"));
                    string[] q = new string[1];
                    q[0] = @"<img src=""/data/thumbs/";
                    string qs = "";
                    foreach (string imgstr in qstr.Split(q, StringSplitOptions.RemoveEmptyEntries))
                    {
                        qs = imgstr.Substring(0, imgstr.IndexOf("alt="));
                        imgs.Add(q[0].Replace(@"/data", @"http://santex-ens.webactives.ru/data") + qs + ">");
                    }
                }
                List<string> fls = new List<string>();
                if (htm_text.IndexOf(@"<a href=""/files/download/") >= 0)
                {

                    string qstr = htm_text.Substring(htm_text.IndexOf(@"<a href=""/files/download/"));
                    string[] q = new string[1];
                    q[0] = @"<a href=""/files/download/";
                    string qs = "";
                    foreach (string fstr in qstr.Split(q, StringSplitOptions.RemoveEmptyEntries))
                    {
                        qs = fstr.Substring(0, fstr.IndexOf("</a>"));
                        if (qs.IndexOf("&key") > 0)
                        {
                            fls.Add(q[0].Replace(@"/files", @"http://santex-ens.webactives.ru/files") + qs + "</a>");
                        }
                    }
                }

                sb.Append("<div>");
                if (fls.Count == 0)
                {
                    sb.Append("<strong>нет документов для этого товара</strong>");
                }
                else
                {
                    sb.Append("<ul>");
                    foreach (string fl in fls)
                    {
                        sb.Append("<li>" + fl + "</li>");
                    }
                    sb.Append("</ul><hr/>");
                }

                if (imgs.Count == 0)
                {
                    sb.Append("<strong>нет изображения этого товара</strong>");
                }
                else
                {
                    foreach (string img in imgs)
                    {
                        sb.Append(img);
                    }
                }






                sb.Append("</div>");
            }
            catch { }
            return sb.ToString();
        }


        public static bool SaveImg(string code, string virtual_path, bool async = false)
        {
            if (code.Trim() == "")
            {
                return false;
            }

            bool ret = false;
            string htm_text = "";
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            try
            {
                WebRequest request = WebRequest.Create(@"http://santex-ens.webactives.ru/get/" + code + @"/?key=x9BS1EFXj0");
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                Stream objStream = request.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                htm_text = objReader.ReadToEnd();

            }
            catch
            {
                htm_text = "";
                ret = false;
            }


            List<string> imgs = new List<string>();
            if (htm_text.IndexOf(@"<img src=""/data/thumbs/") >= 0)
            {

                string qstr = htm_text.Substring(htm_text.IndexOf(@"<img src=""/data/thumbs/"));
                string[] q = new string[1];
                q[0] = @"<img src=""/data/thumbs/";
                string qs = "";
                foreach (string imgstr in qstr.Split(q, StringSplitOptions.RemoveEmptyEntries))
                {
                    qs = imgstr.Substring(0, imgstr.IndexOf("\" alt="));
                    imgs.Add(@"http://santex-ens.webactives.ru/data/thumbs/" + qs);
                    break;
                }
            }




            if (imgs.Count == 0)
            {
                ret = false;
            }
            else
            {

                try
                {
                    //if (async)
                    //{
                    //    wc.DownloadFileAsync(new Uri(imgs[0]), webIO.GetAbsolutePath(virtual_path + @"/" + code + Path.GetExtension(imgs[0])));
                    //}
                    //else
                        wc.DownloadFile(imgs[0], webIO.GetAbsolutePath(virtual_path + @"/" + code + Path.GetExtension(imgs[0])));
                    ret = true;
                }
                catch
                {
                    ret = false;
                }
            }

            return ret;
        }
    }
}