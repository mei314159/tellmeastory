using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class ViewTribeInfoBusinessLogic : IViewTribeInfoBusinessLogic
    {
        private readonly IRemoteTribesDataService _remoteTribeDataService;
        private readonly ILocalTribesDataService _localTribeDataService;
        private readonly ILocalAccountService _localLocalAccountService;
        private readonly CreateTribeValidator _validator;
        private readonly IRouter _router;

        public ViewTribeInfoBusinessLogic(IRemoteTribesDataService remoteTribeDataService, IRouter router,
            ILocalTribesDataService localTribeDataService, CreateTribeValidator validator,
            ILocalAccountService localLocalAccountService)
        {
            _localTribeDataService = localTribeDataService;
            _validator = validator;
            _localLocalAccountService = localLocalAccountService;
            _router = router;
            _remoteTribeDataService = remoteTribeDataService;
        }

        public IViewTribeView View { get; set; }

        public async Task LoadAsync(bool forceRefresh)
        {
            var tribe = View.Tribe;
            var tribeResult = await _localTribeDataService.GetAsync(tribe.Id).ConfigureAwait(false);
            if (forceRefresh || tribeResult.Expired || tribeResult.Data.Members == null ||
                tribeResult.Data.Members.Count == 0)
            {
                var result = await _remoteTribeDataService.GetTribeAsync(tribe.Id).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localTribeDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    tribe = result.Data;
                }
                else
                {
                    result.ShowResultError(this.View);
                    return;
                }
            }
            else
            {
                tribe = tribeResult.Data;
            }

            this.View.Display(tribe);
        }

        public void ChooseMembers()
        {
            var selectedItems = new HashSet<string>(View.Tribe.Members.Select(x => x.UserId));
            _router.NavigateChooseTribeMembers(View, HandleStorytellerSelectedEventHandler, true, selectedItems);
        }

        private void HandleStorytellerSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            foreach (var contact in selectedContacts)
            {
                View.Tribe.Members.Add(new TribeMemberDTO
                {
                    UserId = contact.User.Id,
                    UserName = contact.User.UserName,
                    UserPictureUrl = contact.User.PictureUrl,
                    FullName = contact.User.FullName,
                    TribeId = View.Tribe.Id,
                    TribeName = View.Tribe.Name,
                    Status = TribeMemberStatus.Invited
                });
            }
            View.DisplayMembers();
        }

        public async Task SaveAsync()
        {
            var dto = new TribeDTO
            {
                Id = View.Tribe.Id,
                Name = View.Tribe.Name,
                Members = View.Tribe.Members.Select(x => new TribeMemberDTO {UserId = x.UserId}).ToList()
            };

            var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                var result = await _remoteTribeDataService
                    .UpdateAsync(dto)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localTribeDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    this.View.ShowSuccessMessage("Tribe successfully updated");
                }
                else
                {
                    result.ShowResultError(this.View);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this.View);
            }
        }

        public void NavigateTribeMember(TribeMemberDTO tribeMember)
        {
            _router.NavigateStoryteller(View, tribeMember.UserId);
        }

        public async Task LeaveTribeAsync()
        {
            var result = await _remoteTribeDataService
                .LeaveAsync(View.Tribe.Id)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                View.Tribe.Members.RemoveAll(x => x.UserId == _localLocalAccountService.GetAuthInfo().UserId);
                await _localTribeDataService.DeleteAsync(View.Tribe).ConfigureAwait(false);
                this.View.ShowSuccessMessage("You've left this tribe", () => View.Close(View.Tribe));
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}