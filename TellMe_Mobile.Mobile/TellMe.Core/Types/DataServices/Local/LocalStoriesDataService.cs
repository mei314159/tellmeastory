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
    public class LocalStoriesDataService : ILocalStoriesDataService
    {
        private readonly string _dbPath;

        public LocalStoriesDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<StoryDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }


        public async Task DeleteAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.DeleteAll<StoryDTO>();
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Stories");
            }).ConfigureAwait(false);
        }

        public async Task SaveStoriesAsync(ICollection<StoryDTO> stories)
        {
            if (stories == null)
                return;
            
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                foreach (var story in stories)
                {
                    c.InsertOrReplace(story, typeof(StoryDTO));
                }
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Stories" });
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<ICollection<StoryDTO>>> GetAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.Table<StoryDTO>().ToListAsync().ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Stories").ConfigureAwait(false);
            return new DataResult<ICollection<StoryDTO>>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(StoryDTO story)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.InsertOrReplace(story, typeof(StoryDTO));
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Stories" });
            }).ConfigureAwait(false);
        }
    }
}
