using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using ensoCom;
using wstcp;





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
        
    }
