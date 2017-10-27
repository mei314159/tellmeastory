using System;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class CreateTribeBusinessLogic
    {
        private readonly ICreateTribeView _view;
        private readonly RemoteTribesDataService _remoteTribesService;
        private readonly LocalTribesDataService _localTribesService;
        private readonly CreateTribeValidator _validator;

        public CreateTribeBusinessLogic(RemoteTribesDataService remoteTribesService, ICreateTribeView view)
        {
            _view = view;
            _localTribesService = new LocalTribesDataService();
            _remoteTribesService = remoteTribesService;
            _validator = new CreateTribeValidator();
        }

        public async Task CreateTribeAsync()
        {
            var dto = new TribeDTO
            {
                Name = _view.TribeName,
                Members = _view.Members.Select(x => new TribeMemberDTO { UserId = x.Id }).ToList()
            };
            var validationResult = await _validator.ValidateAsync(dto).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                var result = await _remoteTribesService
                    .CreateAsync(dto)
                    .ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    await _localTribesService.SaveAsync(result.Data).ConfigureAwait(false);
                    this._view.ShowSuccessMessage("Tribe successfully created", () => _view.Close(result.Data));
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
    }
}
