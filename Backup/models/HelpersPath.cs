using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using wstcp;





    public abstract class HelpersPath
    {
        // Virtual ------------------------------------------------------
        public static string VirtaulRoot
        {
            get
            {
                return ".."; //VirtualPathUtility.ToAbsolute(contentVirtualRoot);
            }
        }
        public static string ImageRootVirtualPath
        {
            get { return string.Format("{0}/{1}", VirtaulRoot, "img"); }
        }

        public static string MediaRootVirtualPath
        {
            get { return string.Format("{0}/{1}", VirtaulRoot, "media"); }
        }


        public static string CssRootVirtualPath
        {
            get { return string.Format("{0}/{1}", VirtaulRoot, "css"); }
        }
        public static string PersonsAvatarVirtualPath
        {
            get { return string.Format("{0}/{1}", ImageRootVirtualPath, "persons"); }
        }
        
        public static string CategoriesAvatarVirtualPath
        {
            get { return string.Format("{0}/{1}", ImageRootVirtualPath, "categories"); }
        }
        
        
        // Fisical -------------------------------------------------------
        public static string FisicalRoot
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }
       
        

        public static string ImageFisicalPath
        {
            get { return string.Format("{0}{1}", FisicalRoot, "img"); }
        }
        public static string MediaFisicalPath
        {
            get { return string.Format("{0}{1}", FisicalRoot, "media"); }
        }
        public static string PersonsAvatarFisicalPath
        {
            get { return string.Format("{0}\\{1}", ImageFisicalPath, "persons"); }
        }
        public static string CategoriesAvatarFisicalPath
        {
            get { return string.Format("{0}\\{1}", ImageFisicalPath, "categories"); }
        }


        
        
        // External ------------------------------------------
        public static string RootUrl
        {
            get
            {
                return wstcp.Global.RootUrl;
            }
        }

 
    }
