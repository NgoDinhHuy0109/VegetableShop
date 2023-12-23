    using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Shop.SendMail
{
    public class MailUtils
    {



        /// <summary>
        /// Gửi email sử dụng máy chủ SMTP Google (smtp.gmail.com)
        /// </summary>
        public static async Task<String> SendMailGoogleSmtp(string _from, string _to, string _subject,
                                                            string _body, string _gmailsend, string _gmailpassword)
        {

            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            // Tạo SmtpClient kết nối đến smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmailsend, _gmailpassword);
                client.EnableSsl = true;
                try
                {
                    await client.SendMailAsync(message);
                    Console.WriteLine("Ok");
                    return "Gui mail thanh cong";
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                    return "Gui mail that bai";
                }
            }

        }
    }
}
