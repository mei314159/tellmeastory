using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Local
{
    public class LocalPlaylistsDataService : ILocalPlaylistsDataService
    {
        private readonly string _dbPath;

        public LocalPlaylistsDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<EventDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task<DataResult<PlaylistDTO>> GetAsync(int id)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.FindWithChildrenAsync<PlaylistDTO>(id).ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Playlists").ConfigureAwait(false);
            return new DataResult<PlaylistDTO>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(PlaylistDTO entity)
        {
            if (entity == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.InsertOrReplace(entity, typeof(PlaylistDTO));
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Playlists"});
            }).ConfigureAwait(false);
        }
        
        public async Task SaveAllAsync(IEnumerable<PlaylistDTO> entities)
        {
            if (entities == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                foreach (var story in entities)
                {
                    c.InsertOrReplace(story, typeof(PlaylistDTO));
                }
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Playlists"});
            }).ConfigureAwait(false);
        }

        public async Task DeleteAsync(PlaylistDTO dto)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.Delete(dto);
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Playlists");
            }).ConfigureAwait(false);
        }
    }
}