using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IViewTribeInfoBusinessLogic : IBusinessLogic
    {
        IViewTribeView View { get; set; }
        Task LoadAsync(bool forceRefresh);
        void ChooseMembers();
        Task SaveAsync();
        void NavigateTribeMember(TribeMemberDTO tribeMember);
        Task LeaveTribeAsync();
    }
}