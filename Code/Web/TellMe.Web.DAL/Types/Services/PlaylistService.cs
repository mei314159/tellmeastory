using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Types.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Playlist> _playlistRepository;

        public PlaylistService(IUnitOfWork unitOfWork, IRepository<Playlist> playlistRepository)
        {
            _unitOfWork = unitOfWork;
            _playlistRepository = playlistRepository;
        }

        public async Task<PlaylistDTO> GetAsync(string currentUserId, int playlistId)
        {
            var result = await _playlistRepository
                .GetQueryable(true)
                .Include(x => x.User)
                .Where(x => x.Id == playlistId && x.UserId == currentUserId)
                .ProjectTo<PlaylistDTO>()
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return result;
        }

        public async Task<ICollection<PlaylistDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc)
        {
            var result = await _playlistRepository
                .GetQueryable(true)
                .Include(x => x.User)
                .Where(x => x.UserId == currentUserId && x.CreateDateUtc < olderThanUtc)
                .OrderBy(x => x.CreateDateUtc)
                .Take(20)
                .ProjectTo<PlaylistDTO>()
                .ToListAsync()
                .ConfigureAwait(false);

            return result;
        }

        public async Task<PlaylistDTO> SaveAsync(string currentUserId, PlaylistDTO playlistDTO)
        {
            _unitOfWork.BeginTransaction();
            var playlist = playlistDTO.Id == default(int)
                ? new Playlist
                {
                    CreateDateUtc = DateTime.UtcNow,
                    UserId = currentUserId
                }
                : await _playlistRepository.GetQueryable().FirstAsync(x => x.Id == playlistDTO.Id)
                    .ConfigureAwait(false);
            Mapper.Map(playlistDTO, playlist);
            _unitOfWork.SaveChanges();

            var result = await _playlistRepository
                .GetQueryable(true)
                .Include(x => x.User)
                .Where(x => x.Id == playlist.Id)
                .ProjectTo<PlaylistDTO>()
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return result;
        }

        public async Task DeleteAsync(string currentUserId, int playlistId)
        {
            _unitOfWork.BeginTransaction();

            var entity = await _playlistRepository
                .GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == playlistId && x.UserId == currentUserId)
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