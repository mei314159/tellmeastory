using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalEventsDataService : ILocalEventsDataService
    {
        private readonly string _dbPath;

        public LocalEventsDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<EventDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task<DataResult<EventDTO>> GetAsync(int id)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.FindWithChildrenAsync<EventDTO>(id).ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Events").ConfigureAwait(false);
            return new DataResult<EventDTO>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(EventDTO entity)
        {
            if (entity == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.InsertOrReplace(entity, typeof(EventDTO));
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Events"});
            }).ConfigureAwait(false);
        }
        
        public async Task SaveAllAsync(IEnumerable<EventDTO> entities)
        {
            if (entities == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                foreach (var story in entities)
                {
                    c.InsertOrReplace(story, typeof(EventDTO));
                }
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Events"});
            }).ConfigureAwait(false);
        }

        public async Task DeleteAsync(EventDTO dto)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.Delete(dto);
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Events");
            }).ConfigureAwait(false);
        }
    }
}