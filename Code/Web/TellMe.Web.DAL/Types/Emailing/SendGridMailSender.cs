using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using TellMe.DAL.Contracts;
using TellMe.DAL.Types.Settings;

namespace TellMe.DAL.Types.Emailing
{
    public class SendGridMailSender : IMailSender
    {
        private readonly SendGridSettings _sendGridSettings;

        public SendGridMailSender(IOptions<SendGridSettings> sendGridSettings)
        {
            _sendGridSettings = sendGridSettings.Value;
        }

        public void Send(string fromAddress, string fromDisplayName, string mailSubject, string mailBody,
            ICollection<string> recipients, bool isHtml)
        {
            AsyncHelpers.RunSync(() =>
                SendAsync(fromAddress, fromDisplayName, mailSubject, mailBody, recipients, isHtml));
        }

        public async Task SendAsync(string fromAddress, string fromDisplayName, string mailSubject, string mailBody,
            IEnumerable<string> recipients, bool isHtml)
        {
            var sendGridClient = new SendGridClient(_sendGridSettings.ApiKey);
            var mail = new SendGridMessage
            {
                From = new EmailAddress(fromAddress, fromDisplayName),
                Subject = mailSubject,
                TemplateId = _sendGridSettings.MailTemplateId
            };

            mail.AddContent(isHtml ? "text/html" : "text/plain", mailBody);

            foreach (var recipient in recipients)
            {
                var personalization = new Personalization {Tos = new List<EmailAddress> {new EmailAddress(recipient)}};
                mail.Personalizations = new List<Personalization> {personalization};
            }

            try
            {
                await sendGridClient.SendEmailAsync(mail).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //Log.Warn(ex, "Email was not sent");
            }
        }
    }
}