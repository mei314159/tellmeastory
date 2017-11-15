using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class CreateTribeBusinessLogic : ICreateTribeBusinessLogic
    {
        private readonly IRemoteTribesDataService _remoteTribesService;
        private readonly ILocalTribesDataService _localTribesService;
        private readonly CreateTribeValidator _validator;
        public ICreateTribeView View { get; set; }

        public CreateTribeBusinessLogic(IRemoteTribesDataService remoteTribesService, ILocalTribesDataService localTribesService, CreateTribeValidator validator)
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
                Members = View.Members.Select(x => new TribeMemberDTO { UserId = x.Id }).ToList()
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
