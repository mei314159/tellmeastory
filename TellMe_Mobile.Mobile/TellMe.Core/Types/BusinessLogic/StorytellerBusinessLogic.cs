﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class StorytellerBusinessLogic : StoriesTableBusinessLogic, IStorytellerBusinessLogic
    {
        private readonly IRemoteStorytellersDataService _remoteStorytellesDataService;
        private readonly ILocalStorytellersDataService _localStorytellesDataService;

        public StorytellerBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, IRouter router,
            ILocalStoriesDataService localStoriesService, IRemoteStorytellersDataService remoteStorytellesDataService,
            ILocalStorytellersDataService localStorytellesDataService) : base(remoteStoriesDataService, router,
            localStoriesService)
        {
            _remoteStorytellesDataService = remoteStorytellesDataService;
            _localStorytellesDataService = localStorytellesDataService;
        }

        public new IStorytellerView View
        {
            get => (IStorytellerView) base.View;
            set => base.View = value;
        }

        public override async Task LoadStoriesAsync(bool forceRefresh = false, bool clearCache = false)
        {
            if (forceRefresh)
            {
                Stories.Clear();
            }

            var result = await RemoteStoriesDataService
                .GetStoriesAsync(View.Storyteller.Id, forceRefresh ? null : Stories.LastOrDefault()?.CreateDateUtc)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                await LocalStoriesService.SaveStoriesAsync(result.Data).ConfigureAwait(false);
                Stories.AddRange(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
                return;
            }

            this.View.DisplayStories(Stories.OrderByDescending(x => x.CreateDateUtc).ToList());
        }

        public override async Task<bool> InitAsync()
        {
            if (View.Storyteller == null)
            {
                var localStoryteller =
                    await _localStorytellesDataService.GetAsync(View.StorytellerId).ConfigureAwait(false);
                if (localStoryteller.Data == null || localStoryteller.Expired)
                {
                    var result = await _remoteStorytellesDataService.GetByIdAsync(View.StorytellerId)
                        .ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        View.Storyteller = result.Data;
                    }
                    else
                    {
                        result.ShowResultError(this.View);
                        return false;
                    }
                }
                else
                {
                    View.Storyteller = localStoryteller.Data;
                }
            }

            View.DisplayStoryteller(View.Storyteller);
            return true;
        }

        public void SendStory()
        {
            Router.NavigateRecordStory(View, contact: new ContactDTO
            {
                Type = ContactType.User,
                UserId = View.Storyteller.Id,
                User = View.Storyteller
            });
        }

        public void RequestStory()
        {
            var contacts = new[]
            {
                new ContactDTO
                {
                    Type = ContactType.User,
                    UserId = View.Storyteller.Id,
                    User = View.Storyteller
                }
            };
            Router.NavigatePrepareStoryRequest(this.View, contacts, CreateStoryRequestAsync);
        }

        private async void CreateStoryRequestAsync(RequestStoryDTO dto, ICollection<ContactDTO> recipients)
        {
            var overlay = this.View.DisableInput();
            var result = await this.RemoteStoriesDataService.RequestStoryAsync(dto, recipients).ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Story successfully requested");
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }
    }
}