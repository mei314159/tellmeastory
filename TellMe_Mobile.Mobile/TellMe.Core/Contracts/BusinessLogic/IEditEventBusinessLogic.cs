using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IEditEventBusinessLogic : IBusinessLogic
    {
        ICreateEventView View { get; set; }
        void NavigateCreateRequest();
        void NavigateAttendee(EventAttendeeDTO eventAttendeeDTO);
        void ChooseMembers();
        Task LoadAsync(bool forceRefresh);
        Task DeleteEventAsync();
        Task SaveAsync();
    }
}