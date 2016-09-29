using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace wstcp
{
    public partial class CaptchaControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        // здесь я буду сохранять текст CAPTCHA, который уже был показан на странице
        protected string PreviousCaptcha { get; private set; }

        // это свойство для использования на конечной странице, возвращает true,
        // если пользователь правильно ввел текст.
        public bool RightInput
        {
            get
            {
                return (PreviousCaptcha == txCap.Text);
            }
        }

        public void ResetCapchaText()
        {
            txCap.Text = "";
        }
        protected override void OnLoad(EventArgs e)
        {
            if (HttpContext.Current.Session["Captcha"] != null)
                PreviousCaptcha = HttpContext.Current.Session["Captcha"].ToString();

            // CAPTCHA в моем примере будет содержать только цифры,
            // но вы можете вписать любые символы
            const string symbols = "0123456789";
            var rnd = new byte[6];
            var chars = new char[6];
            Rand.GetBytes(rnd);

            // случайным образом забираю 6 цифр
            for (var i = 0; i < 6; i++)
                chars[i] = symbols[rnd[i] % 10];

            // формирую из нее строку CAPTCHA и записываю в переменную сессии,
            // чтобы впоследствие хендлер изображений мог ее получить
            HttpContext.Current.Session.Add("Captcha", new String(chars));
        } 
    }
}