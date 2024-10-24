using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TT_ECommerce.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "hoangtuavatar2x@gmail.com";
        private readonly string _fromName = "TT_ECommerce";
        private readonly string _fromPassword = "psae wane kftu mkio\r\n";
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(_fromEmail, _fromName);
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromEmail, _fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                try
                {
                    await smtp.SendMailAsync(message);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    // Thực hiện xử lý lỗi và thông báo cho người dùng nếu cần
                }
            }
        }
    }

}
