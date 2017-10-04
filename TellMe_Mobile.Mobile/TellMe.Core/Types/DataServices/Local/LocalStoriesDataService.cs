using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalStoriesDataService
    {
        private readonly string _dbPath;

        public LocalStoriesDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath))
            {
                conn.CreateTable<StoryDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task SaveStoriesAsync(ICollection<StoryDTO> stories)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                foreach (var story in stories)
                {
                    c.InsertOrReplace(story, typeof(StoryDTO));
                }
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Stories" });
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<ICollection<StoryDTO>>> GetAllAsync(string userId = null)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath);
            var query = conn.Table<StoryDTO>();
            if (userId != null)
            {
                query = query.Where(x => x.SenderId == userId || x.ReceiverId == userId);
            }

            var result = await query.ToListAsync().ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Stories").ConfigureAwait(false);
            return new DataResult<ICollection<StoryDTO>>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }
    }
}
