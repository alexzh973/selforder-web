using System;
using System.Net.Mail;
using System.Text;
using System.Net;


namespace wstcp
{
	/// <summary>
	/// Summary description for EmailMessage.
	/// </summary>
	public class EmailMessage
	{
		private string 
			_addressFrom, 
			_addressTo, 
			_title, 
			_body;
		public EmailMessage(string addressFrom, string addressTo, string title, string body)
		{
			_addressFrom = addressFrom; 
			_addressTo = addressTo; 
			_title = title; 
			_body = body; 
		}

		public void Send(string bcc="")
		{
			try
			{
                SmtpClient client = new SmtpClient(CurrentCfg.MailServer);
                char splt = (_addressTo.IndexOf(",") > -1) ? ',' : ';';
                using (System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage())
                {
                    m.To.Add(_addressTo.Split(splt)[0]);
                    if (_addressTo.IndexOf(""+splt) > -1)
                    {
                        foreach (string adr in _addressTo.Split(splt))
                        {
                            m.CC.Add(adr);
                        }
                    }
                    if (bcc != "")
                    {
                        m.Bcc.Add(bcc);
                        if (bcc.IndexOf(""+splt) > -1)
                        {
                            foreach (string adr in bcc.Split(splt))
                            {
                                m.Bcc.Add(adr);
                            }
                        }
                    }
                    m.From = new MailAddress(_addressFrom);
                    m.Subject = _title;
                    m.Body = _body;
                    m.BodyEncoding = Encoding.UTF8;
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;
                    client.Credentials = new NetworkCredential("info@santechportal.ru", "mseng45c");
                    client.Send(m);
                }
                
                
			}
			catch(Exception ex)
			{

			}
		}
	}
}
