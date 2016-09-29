using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Web.SessionState;
using System.Security.Cryptography;
using ensoCom;

namespace wstcp
{
    /// <summary>
    /// Сводное описание для CaptchaHandler
    /// </summary>
    public class CaptchaHandler : IHttpHandler, IReadOnlySessionState
    {
        
        

       public void ProcessRequest(HttpContext context)
       {
            context.Response.CacheControl = "no-cache";
           context.Response.Expires = -1;       
         
           // константы
           const int width = 95; // ширина изображения CAPTCHA
           const int height = 30; // высота изображения CAPTCHA
           const int linesCount = 5; // число линий «мусора»
           var backgroundColor = Color.DarkBlue; // цвет фона
           var foregroundBrush = Brushes.White; // brush для текста и линий
           const int fontSize = 16; // размер текста

           // текст для CAPTCHA передается через переменную сессии
           if ( context.Session["Captcha"] == null) return;
           var captcha = context.Session["Captcha"].ToString();

           // создаем bitmap-объект изображение в качестве холста для рисования
           var bitmap = new Bitmap(width, height);

           // получаем Graphics – своего рода «контроллер» рисования,
           // мы его создаем на основе нашего bitmap
           var graphics = Graphics.FromImage(bitmap);

           // следующие два свойства выставляем, чтобы границы текста
           // были плавные, это усложнит его машинное распознавание
           graphics.SmoothingMode = SmoothingMode.AntiAlias;
           graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
           graphics.Clear(backgroundColor); // заливаем фон

           // шрифт для текста
           var font = new Font("Arial", fontSize, FontStyle.Bold);

           // рисуем текст.
           
         //  graphics.DrawString(captcha, font, foregroundBrush, 3, 3);
           long xi = 6;
           foreach (char ch in captcha.ToArray<char>())
           {
               graphics.DrawString(""+ch, font, foregroundBrush, xi, 1+ensoCom.cMath.GetRandom(6));
               xi += 14;
           }
           font.Dispose();

           // теперь можно было бы остановиться, но так текст легко
           // распознать с изображения, поэтому необходимо его как-то трансформировать;
           // я вытяну его вправо за середину и наложу сверху случайные линии мусора

           // вытягиваю текст путем перемещения части пикселов bitmap,
           // чем ближе пиксел к серединной линии – тем сильнее будет
           // смещение пиксела вправо
/*
           for (var i = 0; i < width; i++)
           {
               for (var j = 0; j < height; j++)
               {
                   var offset = Math.Abs(height/3 - j);
                   if (i + offset < width)
                   {
                       var pixel = bitmap.GetPixel(i + offset, j);
                       bitmap.SetPixel(i, j, pixel);
                   }
               }
           }
*/
           int px, py;
           for (int i = 0; i < 300; i++)
           {
               px = cMath.GetRandom(width - 1);
               py = cMath.GetRandom(height - 1);
               try
               {
                   bitmap.SetPixel(px,py , Color.Yellow);
               }
               catch
               {
                   var ex = "hh";
               }
           }

           // теперь рисую линии мусора
           var pen = new Pen(foregroundBrush); // карандаш

           // случайные значения для координат
           var rnd = new byte[linesCount*4];
           CaptchaControl.Rand.GetBytes(rnd);

           // рисую линии
           int  x1, y1, x2, y2;

           for (int i = 0; i < linesCount; i++)
           {
               x1 = cMath.GetRandom(width / 2);
               x2 = (width / 2) + cMath.GetRandom(width / 2);
               y1 = cMath.GetRandom(height);
               y2 = cMath.GetRandom(height);
               graphics.DrawLine(pen,x1,y1,x2,y2);
               //graphics.DrawLine(pen, rnd[4 * i] % width, rnd[4 * i + 1] % height,
               //                   rnd[4 * i + 2] % width, rnd[4 * i + 3] % height);
           }
           
           pen.Dispose();

           // теперь изображение готово, мне нужно преобразовать его
           // в поток байт, чтобы вернуть браузеру
           var stream = new MemoryStream();
           bitmap.Save(stream, ImageFormat.Png); // сохраняю в бинарный поток
                                                 // в памяти в формате PNG

           graphics.Dispose();

           // выставляю заголовок ответа Content-Type,
           // чтобы браузер понял формат изображения
           context.Response.ContentType = "image/png";

           // выгружаю в ответ двоичный поток изображения
           context.Response.BinaryWrite(stream.GetBuffer());
           context.Response.Flush();

           stream.Close();

           bitmap.Dispose();
       }

       public bool IsReusable
       {
           get
           {
               return false;
           }
       }    }
}