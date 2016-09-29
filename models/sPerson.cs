using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ensoCom;
using System.Data;

namespace wstcp.models
{
    public class sPerson
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Email {get; set;}
        public int SubjectID { get; set; }
        public bool LoginEnabled { get; set; }

        public static List<sPerson> ListPersons()
        {
            List<sPerson> res = new List<sPerson>();
            DataTable dt = db.GetDbTable("select id, name, email, subjectid, LoginEnabled from ensouser where isfolder<>'Y' and state<>'D'");
            res = dt.AsEnumerable().ToList().ConvertAll(x => new sPerson { ID = (int)x.ItemArray[0], Name = (string)x.ItemArray[1], Email = (string)x.ItemArray[2], SubjectID = cNum.cToInt(x.ItemArray[3]), LoginEnabled = ((string)x.ItemArray[4]=="Y") });
            return res;
        }
    }
}