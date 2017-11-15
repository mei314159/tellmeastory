using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalNotificationsDataService : ILocalNotificationsDataService
    {
        private readonly string _dbPath;

        public LocalNotificationsDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<NotificationDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task DeleteAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.DeleteAll<NotificationDTO>();
                c.Table<UpdateInfo>().Delete(x => x.TableName == "Notifications");
            }).ConfigureAwait(false);
        }

        public async Task SaveAllAsync(ICollection<NotificationDTO> entities)
        {
            if (entities == null)
                return;

            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.Trace = true;
                c.InsertOrReplaceAllWithChildren(entities);
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Notifications" });
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<NotificationDTO[]>> GetAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            var result = (await conn.GetAllWithChildrenAsync<NotificationDTO>().ConfigureAwait(false)).OrderByDescending(x => x.Date).ToArray();
            var updateInfo = await conn.FindAsync<UpdateInfo>("Notifications").ConfigureAwait(false);
            return new DataResult<NotificationDTO[]>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }

        public async Task SaveAsync(NotificationDTO notification)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                c.InsertOrReplace(notification, typeof(NotificationDTO));
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Notifications" });
            }).ConfigureAwait(false);
        }
    }
}
