using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class ViewTribeBusinessLogic
    {
        private readonly IViewTribeView _view;
        private readonly RemoteTribesDataService _remoteTribeDataService;
        private readonly LocalTribesDataService _localTribeDataService;
        private readonly CreateTribeValidator _validator;
        private readonly IRouter _router;

        public ViewTribeBusinessLogic(RemoteTribesDataService remoteTribeDataService, IRouter router, IViewTribeView view)
        {
            _view = view;
            _router = router;
            _localTribeDataService = new LocalTribesDataService();
            _remoteTribeDataService = remoteTribeDataService;
            _validator = new CreateTribeValidator();
        }

        public async Task LoadAsync(bool forceRefresh)
        {
            TribeDTO tribe = _view.Tribe;
            var tribeResult = await _localTribeDataService.GetAsync(tribe.Id).ConfigureAwait(false);
            if (forceRefresh || tribeResult.Expired || tribeResult.Data.Members == null || tribeResult.Data.Members.Count == 0)
            {
                var result = await _remoteTribeDataService.GetTribeAsync(tribe.Id).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localTribeDataService.SaveAsync(result.Data).ConfigureAwait(false);
                    tribe = result.Data;
                }
                else
                {
                    result.ShowResultError(this._view);
                    return;
                }
            }
            else
            {
                tribe = tribeResult.Data;
            }

            this._view.Display(tribe);
        }

        public void ChooseMembers()
        {
            var selectedItems = new HashSet<string>(_view.Tribe.Members.Select(x => x.UserId));
            _router.NavigateChooseTribeMembers(_view, HandleStorytellerSelectedEventHandler, true, selectedItems);
        }

        void HandleStorytellerSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            foreach (var contact in selectedContacts)
            {
                _view.Tribe.Members.Add(new TribeMemberDTO
                {
                    UserId = contact.User.Id,
                    UserName = contact.User.UserName,
                    UserPictureUrl = contact.User.PictureUrl,
                    FullName = contact.User.FullName,
                    TribeId = _view.Tribe.Id,
                    TribeName = _view.Tribe.Name,
                    Status = TribeMemberStatus.Invited
                });
            }
            _view.DisplayMembers();
        }

        public async Task SaveAsync()
        {
            var dto = new TribeDTO
            {
                Id = _view.Tribe.Id,
                Name = _view.Tribe.Name,
                Members = _view.Tribe.Members.Select(x => new TribeMemberDTO { UserId = x.UserId }).ToList()
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
                    this._view.ShowSuccessMessage("Tribe successfully updated");
                }
                else
                {
                    result.ShowResultError(this._view);
                }
            }
            else
            {
                validationResult.ShowValidationResult(this._view);
            }
        }

        public async Task LeaveTribeAsync()
        {
            var result = await _remoteTribeDataService
                    .LeaveAsync(_view.Tribe.Id)
                    .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                _view.Tribe.Members.RemoveAll(x => x.UserId == App.Instance.AuthInfo.UserId);
                await _localTribeDataService.DeleteAsync(_view.Tribe).ConfigureAwait(false);
                this._view.ShowSuccessMessage("You've left this tribe", () => _view.Close(_view.Tribe));
            }
            else
            {
                result.ShowResultError(this._view);
            }
        }
    }
}