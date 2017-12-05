using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TellMe.DAL.Contracts;
using TellMe.DAL.Types.Settings;
using MailMessage = System.Net.Mail.MailMessage;
using MailPriority = System.Net.Mail.MailPriority;

namespace TellMe.DAL.Types.Emailing
{
    public class SmtpMailSender : IMailSender
    {
        private readonly AppSettings _appSettings;
        private static SmtpClient _mailClient;

        public SmtpMailSender(IOptions<AppSettings> emailingSettings)
        {
            _appSettings = emailingSettings.Value;
            _mailClient = new SmtpClient();
        }

        public void Send(string fromAddress, string fromDisplayName, string mailSubject, string mailBody, ICollection<string> recipients, bool isHtml)
        {
            AsyncHelpers.RunSync(() => SendAsync(fromAddress, fromDisplayName, mailSubject, mailBody, recipients, isHtml));
        }

        public async Task SendAsync(string fromAddress, string fromDisplayName, string mailSubject, string mailBody, IEnumerable<string> recipients,
            bool isHtml)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_appSettings.SupportEmail, fromDisplayName, Encoding.UTF8),
                Subject = mailSubject,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.Low,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess
            };

            foreach (var x in recipients)
            {
                mailMessage.To.Add(x);
            }

            mailMessage.Body = mailBody;
            
            try
            {
                await _mailClient.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Log.Warn(ex, "Email was not sent");
            }
        }
    }
}
