using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalTribesDataService : ILocalTribesDataService
    {
        private readonly string _dbPath;

        public LocalTribesDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<TribeDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task DeleteAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.DeleteAll<TribeDTO>();
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Tribes");
            }).ConfigureAwait(false);
        }

        public async Task DeleteAsync(TribeDTO dto)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.Delete(dto);
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Tribes");
            }).ConfigureAwait(false);
        }

        public async Task SaveAllAsync(IEnumerable<TribeDTO> items)
        {
            if (items == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.Trace = true;
                c.InsertOrReplaceAllWithChildren(items);
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Tribes"});
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<TribeDTO>> GetAsync(int tribeId)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.FindWithChildrenAsync<TribeDTO>(tribeId).ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Tribes").ConfigureAwait(false);
            return new DataResult<TribeDTO>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task<DataResult<ICollection<TribeDTO>>> GetAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = await conn.GetAllWithChildrenAsync<TribeDTO>().ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Tribes").ConfigureAwait(false);
            return new DataResult<ICollection<TribeDTO>>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(TribeDTO item)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync(c =>
            {
                c.InsertOrReplaceWithChildren(item);
                c.InsertOrReplace(new UpdateInfo {UtcDate = DateTime.UtcNow, TableName = "Tribes"});
            }).ConfigureAwait(false);
        }
    }
}