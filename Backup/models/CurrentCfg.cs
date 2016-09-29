using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ensoCom;

namespace wstcp
{
	/// <summary>
	/// Summary description for CurrentCfg.
	/// </summary>
	[Serializable]
	public class CurrentCfg
	{
		private static string _mail="";
        
		
		public static int OwnerID
		{
			get{return 100000;}
		}
		
        

		public static string EmailPortal
		{
            get
            {
                return (SysSetting.GetValue("infoemail")=="")?"alexzh@santur.ru":SysSetting.GetValue("infoemail");//; 
            }
		}
        public static string EmailSupport
        {
            get
            {
                return (SysSetting.GetValue("supportemail")=="")?"alexzh@santur.ru":SysSetting.GetValue("supportemail");//; 
            }
        }
        public static string MailServer
		{
			get 
			{
				if (_mail=="")
					_mail = ensoCom.SysSetting.GetValue("mailserver");
				return _mail;
			}		
		}


	}
}
