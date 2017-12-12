using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Mobile.Core.Validation;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class CreateTribeBusinessLogic : ICreateTribeBusinessLogic
    {
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly ILocalTribesDataService _localTribesService;
        private readonly CreateTribeValidator _validator;
        public ICreateTribeView View { get; set; }

        public CreateTribeBusinessLogic(IRemoteTribesDataService remoteTribesService,
            ILocalTribesDataService localTribesService, CreateTribeValidator validator)
        {
            _localTribesService = localTribesService;
            _validator = validator;
            _remoteTribesService = remoteTribesService;
        }

        public async Task CreateTribeAsync()
        {
            var dto = new TribeDTO
            {
                Name = View.TribeName,
                Members = View.Members.Select(x => new TribeMemberDTO {UserId = x.Id}).ToList()
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
                    this.View.ShowSuccessMessage("Tribe successfully created", () => View.Close(result.Data));
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
    }
}