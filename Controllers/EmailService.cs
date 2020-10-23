using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace WebApplication1.Controllers
{
    public class EmailService
    {
        ApplicationContext _context { get; set; }
        public EmailService(ApplicationContext db)
        {
            _context = db;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var emailMessage = new MimeMessage();
            // Добавление информации об отправителе
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", _context.SendMail));
            // добавление инофрмации о получателе
            emailMessage.To.Add(new MailboxAddress("", email));
            // тема письма
            emailMessage.Subject = subject;
            // добавления тела месседжа
            emailMessage.Body = new TextPart("Plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                // подключение к сервису почты
                await client.ConnectAsync("smtp.gmail.com", 25, false);
                // аутентификация в почте
                await client.AuthenticateAsync(_context.SendMail, _context.SendPassword);
                // отправка месседжа
                await client.SendAsync(emailMessage);
                // отключения от сервиса почты
                await client.DisconnectAsync(true);
            }
        }
    }
}
