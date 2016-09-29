using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wstcp
{
    public class webUser
    {
        public static string PhotoSmallPath(object userId)
        {
            return HelpersPath.PersonsAvatarVirtualPath + @"/" + ((webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + @"/" + userId + "s.png")) ? userId + "s.png" : "0s.png"); 
        }
        public static string PhotoBigPath(object userId )
        {
            return HelpersPath.PersonsAvatarVirtualPath + @"/" + ((webIO.CheckExistFile(HelpersPath.PersonsAvatarVirtualPath + @"/" + userId + "b.png")) ? userId + "b.png" : "0b.png"); 
        }



    }
}