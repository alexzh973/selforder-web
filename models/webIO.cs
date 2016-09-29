using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using ensoCom;
using wstcp;
using selforderlib;





    /// <summary>
    /// Сводное описание для webIO
    /// </summary>
    public abstract class webIO
    {
        public static string GetBasePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
        public static string GetAbsolutePath(string virtual_path)
        {
            string virt_path = virtual_path.Replace("..", "");
            return (GetBasePath() + ((virt_path.StartsWith("/") || virt_path.StartsWith(@"\")) ? "" : "\\") + virt_path).Replace("/", @"\").Replace(@"\\\", @"\").Replace(@"\\", @"\");
        }
        public static bool CheckDirectory(string virtual_path, bool createIfnotexist, IAM current_user)
        {
            bool result = Directory.Exists(GetAbsolutePath(virtual_path));
            if (!result && createIfnotexist)
            {
                try
                {
                    Directory.CreateDirectory(GetAbsolutePath(virtual_path));
                    result = Directory.Exists(GetAbsolutePath(virtual_path));
                }
                catch (Exception ex)
                {
                    //Mistake.SendMistake("при попытке создать директорию " + GetAbsolutePath(virtual_path) + " произошла ошибка: " + ex.ToString(), current_user,"webIO.cs");
                }

            }
            return result;
        }
        public static bool CheckExistFile(string virtual_path)
        {
            return File.Exists(GetAbsolutePath(virtual_path));
        }

        public static string GetIcoFiletype(string fileExtention)
        {
            string ico = "wiki_link.png";
            switch (fileExtention.ToLower())
            {
                case ".rtf":
                case ".doc":
                case ".docx":
                    ico = "doc.gif";
                    break;
                case ".csv":
                case ".xls":
                case ".xlsx":
                    ico = "xls.gif";
                    break;
                case ".pdf":
                    ico = "pdf.gif";
                    break;
                case ".ppt":
                case ".pptx":
                    ico = "ppt.gif";
                    break;
                case ".rar":
                case ".zip":
                case ".7z":
                case ".tar":
                    ico = "rar.gif";
                    break;
                case ".jpg":
                case ".bmp":
                case ".png":
                case ".gif":
                    ico = "jpg.gif";
                    break;
                default:
                    ico = "document.png";
                    break;
            }
            return "<img src='../simg/16/" + ico + "' border='0' title='" + fileExtention + "' />";
        }

        public static string GetFileContentType(string fileExtension)
        {
            switch (fileExtension.ToLower().Replace(".",""))
            {
                case "htm":
                case "html":
                case "log":
                    return "text/HTML";
                case "txt":
                    return "text/plain";
                case "doc":
                case "docx":
                    return "application/msword";
                case "tiff":
                case "tif":
                    return "image/tiff";
                case "asf":
                    return "video/x-ms-asf";
                case "avi":
                    return "video/avi";
                case "zip":
                    return "application/zip";
                case "xls":
                case "xlsx":
                case "csv":
                    return "application/vnd.ms-excel";
                case "png":
                    return "image/png";
                case "gif":
                    return "image/gif";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "bmp":
                    return "image/bmp";
                case "wav":
                    return "audio/wav";
                case "mp3":
                    return "audio/mpeg3";
                case "mpg":
                case "mpeg":
                    return "video/mpeg";
                case "rtf":
                    return "application/rtf";
                case "asp":
                    return "text/asp";
                case "pdf":
                    return "application/pdf";
                case "fdf":
                    return "application/vnd.fdf";
                case "ppt":
                case "pptx":
                    return "application/vnd.ms-powerpoint";
                case "dwg":
                    return "image/vnd.dwg";
                case "msg":
                    return "application/msoutlook";
                case "xml":
                case "sdxl":
                    return "application/xml";
                case "xdp":
                    return "application/vnd.adobe.xdp+xml";
                default:
                    return "application/octet-stream";
            }
        }

    }
