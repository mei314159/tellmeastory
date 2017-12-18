using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TellMe.Shared.Contracts.DTO;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL.Contracts;
using TellMe.Web.DAL.Contracts.Repositories;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Extensions;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.DAL.Types.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Playlist> _playlistRepository;
        private readonly IRepository<PlaylistUser> _playlistUsersRepository;

        public PlaylistService(IUnitOfWork unitOfWork, IRepository<Playlist> playlistRepository,
            IRepository<PlaylistUser> playlistUsersRepository)
        {
            _unitOfWork = unitOfWork;
            _playlistRepository = playlistRepository;
            _playlistUsersRepository = playlistUsersRepository;
        }

        public async Task<PlaylistDTO> GetAsync(string currentUserId, int playlistId)
        {
            var playlist = await _playlistRepository.GetQueryable(true)
                .Include(x => x.Users)
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
                .Where(x => x.CreateDateUtc < olderThanUtc && x.Users.Any(y => y.UserId == currentUserId))
                .OrderBy(x => x.CreateDateUtc)
                .Take(20)
                .ToListAsync()
                .ConfigureAwait(false);

            var result = Mapper.Map<ICollection<PlaylistDTO>>(playlists);
            return result;
        }

        public async Task<PlaylistDTO> SaveAsync(string currentUserId, PlaylistDTO playlistDTO)
        {
            _unitOfWork.BeginTransaction();
            var playlist = playlistDTO.Id == default(int)
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
                : await _playlistRepository.GetQueryable().FirstAsync(x => x.Id == playlistDTO.Id)
                    .ConfigureAwait(false);
            Mapper.Map(playlistDTO, playlist);
            await _playlistRepository.SaveAsync(playlist).ConfigureAwait(false);
            _unitOfWork.SaveChanges();

            playlist = await _playlistRepository.GetQueryable(true)
                .Include(x => x.Users)
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
                .Where(x => x.Id == playlistId &&
                            x.Users.Any(y => y.UserId == currentUserId && y.Type == PlaylistUserType.Author))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            entity.Users.MapFrom(dto.UserIds, contactDTO => contactDTO, x => x.UserId,
                (userId, user) => { user.UserId = userId; });

            await _playlistRepository.SaveAsync(entity).ConfigureAwait(false);
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
    }
}