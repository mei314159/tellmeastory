using System.Collections.Generic;
using System.Threading.Tasks;

namespace TellMe.Web.DAL.Contracts
{
    public interface IMailSender
    {
        void Send(string fromAddress, string fromDisplayName, string mailSubject, string mailBody, ICollection<string> recipients, bool isHtml);

        Task SendAsync(string fromAddress, string fromDisplayName, string mailSubject, string mailBody,
            IEnumerable<string> recipients, bool isHtml);
    }
}
