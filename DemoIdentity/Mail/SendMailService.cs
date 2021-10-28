using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Mail
{
    public class MailSettings
    {
        public string Mail { set; get; }
        public string DisplayName { set; get; }
        public string Password { set; get; }
        public string Host { set; get; }
        public int Port { set; get; }
    }

    // dịch vụ gửi mail
    public class SendMailService : IEmailSender
    {
        MailSettings mailSettings;
        public SendMailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            // người gửi
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            // người nhận
            message.To.Add(MailboxAddress.Parse(email));
            // tiêu đề email
            message.Subject = subject;
            // xử lý phần body
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            // dùng smtp client của MailKit để gửi mail
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                // kết nối đến server mail (trong bài là gmail)
                smtp.Connect(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                // xác thực để gửi mail
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                // gửi mail
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // có thể dùng log để ghi thông tin lại - tự xử lý
            }

            // đóng kết nối với server mail sau khi gửi mail
            smtp.Disconnect(true);
        }
    }
}
