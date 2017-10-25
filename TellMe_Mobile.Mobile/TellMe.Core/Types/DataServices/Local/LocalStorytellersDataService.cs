using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalStorytellersDataService
    {
        private readonly string _dbPath;

        public LocalStorytellersDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<StorytellerDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task DeleteAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.DeleteAll<StorytellerDTO>();
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Storytellers");
            }).ConfigureAwait(false);
        }

        public async Task SaveAllAsync(IEnumerable<StorytellerDTO> items)
        {
            if (items == null)
                return;
            
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.Trace = true;
                c.InsertOrReplaceAllWithChildren(items);
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Storytellers" });
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<StorytellerDTO>> GetAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.GetAllWithChildrenAsync<StorytellerDTO>().ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Storytellers").ConfigureAwait(false);
            return new DataResult<StorytellerDTO>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(StorytellerDTO storyteller)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.InsertOrReplace(storyteller, typeof(StorytellerDTO));
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Storytellers" });
            }).ConfigureAwait(false);
        }
    }
}
