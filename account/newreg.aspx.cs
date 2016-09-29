using System;
using selforderlib;

namespace wstcp.account
{
    public partial class newreg : p_p
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Subject news = new Subject(0, iam);
            news.Name = "новый Инн "+txINN.Text;
            news.INN = txINN.Text;
            Subject.Save(news);

        }
    }
}