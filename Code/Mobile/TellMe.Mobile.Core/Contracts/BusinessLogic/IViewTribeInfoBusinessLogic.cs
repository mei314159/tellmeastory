using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
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