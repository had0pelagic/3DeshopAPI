using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Settings;
using _3DeshopAPI.Services.Interfaces;
using Domain.Mail;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace _3DeshopAPI.Services
{
    public class MailService : IMailService
    {
        private readonly SMTPSettings _smptSettings;

        public MailService(IOptions<SMTPSettings> smptSettings)
        {
            _smptSettings = smptSettings.Value;
        }

        public async Task<bool> SendMail(Mail model)
        {
            try
            {
                using var mail = new MailMessage();
                mail.From = new MailAddress(_smptSettings.Username, _smptSettings.FullName);
                mail.To.Add(new MailAddress(model.ReceiverEmail, model.ReceiverFullname));
                mail.Subject = model.Subject;
                mail.Body = model.Body;
                mail.IsBodyHtml = true;

                using var client = new SmtpClient();
                client.Host = _smptSettings.ServerAddress;
                client.Port = int.Parse(_smptSettings.TLSPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smptSettings.Username, _smptSettings.Password);

                await client.SendMailAsync(mail);

                return true;
            }
            catch
            {
                throw new InvalidClientOperationException(ErrorCodes.Email);
            }
        }
    }
}