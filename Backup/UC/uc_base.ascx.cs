using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ensoCom;
using wstcp;



    public partial class uc_base : System.Web.UI.UserControl
    {
        private IAM _iam;
        protected IAM iam
        {
            get
            {
                if (_iam == null || _iam.ID <= 0)
                    _iam = IAM.GetMe(Session.SessionID);

                return _iam;
            }
        }

        
    }
