using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TellMe.Shared.Contracts.DTO;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts;
using TellMe.Web.DAL.Contracts.PushNotifications;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Extensions;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Types.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Playlist> _playlistRepository;
        private readonly IRepository<PlaylistUser> _playlistUsersRepository;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IRepository<Story, int> _storyRepository;

        public PlaylistService(IUnitOfWork unitOfWork, IRepository<Playlist> playlistRepository,
            IRepository<PlaylistUser> playlistUsersRepository, IPushNotificationsService pushNotificationsService,
            IRepository<Story, int> storyRepository)
        {
            _unitOfWork = unitOfWork;
            _playlistRepository = playlistRepository;
            _playlistUsersRepository = playlistUsersRepository;
            _pushNotificationsService = pushNotificationsService;
            _storyRepository = storyRepository;
        }

        public async Task<PlaylistDTO> GetAsync(string currentUserId, int playlistId)
        {
            var playlist = await _playlistRepository.GetQueryable(true)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Stories)
                .ThenInclude(x => x.Story)
                .ThenInclude(x => x.Sender)
                .Where(x => x.Id == playlistId && x.Users.Any(y => y.UserId == currentUserId))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var result = Mapper.Map<PlaylistDTO>(playlist);

            return result;
        }

        public async Task<ICollection<PlaylistDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc)
        {
            var playlists = await _playlistRepository.GetQueryable(true)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Stories)
                .ThenInclude(x => x.Story)
                .ThenInclude(x => x.Sender)
                .Where(x => x.CreateDateUtc < olderThanUtc && x.Users.Any(y => y.UserId == currentUserId))
                .OrderBy(x => x.CreateDateUtc)
                .Take(20)
                .ToListAsync()
                .ConfigureAwait(false);

            var result = Mapper.Map<ICollection<PlaylistDTO>>(playlists);
            return result;
        }

        public async Task<PlaylistDTO> SaveAsync(string currentUserId, PlaylistDTO dto)
        {
            _unitOfWork.BeginTransaction();
            var playlist = dto.Id == default(int)
                ? new Playlist
                {
                    CreateDateUtc = DateTime.UtcNow,
                    Users = new List<PlaylistUser>
                    {
                        new PlaylistUser
                        {
                            UserId = currentUserId,
                            Type = PlaylistUserType.Author
                        }
                    }
                }
                : await _playlistRepository
                    .GetQueryable()
                    .Include(x => x.Stories)
                    .Include(x => x.Users)
                    .FirstAsync(x => x.Id == dto.Id)
                    .ConfigureAwait(false);
            Mapper.Map(dto, playlist);
            await _playlistRepository.SaveAsync(playlist).ConfigureAwait(false);
            _unitOfWork.SaveChanges();

            playlist = await _playlistRepository.GetQueryable(true)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Where(x => x.Id == playlist.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var result = Mapper.Map<PlaylistDTO>(playlist);

            return result;
        }

        public async Task ShareAsync(string currentUserId, int playlistId, SharePlaylistDTO dto)
        {
            var entity = await _playlistRepository
                .GetQueryable()
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Where(x => x.Id == playlistId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var currentUser = entity.Users.FirstOrDefault(x => x.UserId == currentUserId);
            if (currentUser == null)
            {
                throw new Exception("You don't have an access to requested playlist");
            }

            entity.Users.MapFrom(dto.UserIds, contactDTO => contactDTO, x => x.UserId,
                (userId, user) => { user.UserId = userId; },
                (e, dt) => e.Type == PlaylistUserType.Author);

            await _playlistRepository.SaveAsync(entity).ConfigureAwait(false);


            var playlistDTO = Mapper.Map<PlaylistDTO>(entity);

            var notifications = entity.Users.Where(x =>
                    dto.UserIds.Contains(x.UserId) && x.UserId != currentUserId && x.Type != PlaylistUserType.Author)
                .ToList()
                .Select(x => new Notification
                {
                    Date = DateTime.UtcNow,
                    Type = NotificationTypeEnum.SharePlaylist,
                    RecipientId = x.UserId,
                    Extra = playlistDTO,
                    Handled = true,
                    Text = $"{currentUser.User.UserName} has shared playlist {entity.Name} with you"
                }).ToArray();

            await _pushNotificationsService.SendPushNotificationsAsync(notifications).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string currentUserId, int playlistId)
        {
            _unitOfWork.BeginTransaction();

            var entity = await _playlistUsersRepository
                .GetQueryable()
                .Include(x => x.Playlist)
                .Where(x => x.PlaylistId == playlistId && x.UserId == currentUserId)
                .Select(x => x.Playlist)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (entity == null)
            {
                throw new Exception("Playlist was not found or you don't have permissions to delete it");
            }

            _playlistRepository.Remove(entity, true);
            _unitOfWork.SaveChanges();
        }

        public async Task<ICollection<StoryDTO>> GetStoriesAsync(string currentUserId, int playlistId)
        {
            var playlist = await _playlistRepository.GetQueryable(true).Include(x => x.Stories)
                .FirstOrDefaultAsync(x => x.Id == playlistId)
                .ConfigureAwait(false);
            var stories = await _storyRepository
                .GetQueryable(true)
                .Include(x => x.Sender)
                .Include(x => x.Likes)
                .Include(x => x.ObjectionableStories)
                .Include(x => x.Receivers).ThenInclude(x => x.User)
                .Include(x => x.Receivers).ThenInclude(x => x.Tribe)
                .Where(story => story.Playlists.Any(x => x.PlaylistId == playlistId)).ToListAsync()
                .ConfigureAwait(false);

            var list = playlist.Stories.OrderBy(a => a.Order)
                .Join(stories, x => x.StoryId, x => x.Id, (story, story1) => story1).ToList();

            var result = Mapper.Map<ICollection<StoryDTO>>(list, x => x.Items["UserId"] = currentUserId);

            return result;
        }
    }
}