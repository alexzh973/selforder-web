using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wstcp.models
{
    public class webCfg
    {
        public static string GetValue(string key)
        {
            return "" + System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}