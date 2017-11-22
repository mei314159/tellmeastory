using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalEventsService : ILocalEventsService
    {
        private readonly string _dbPath;

        public LocalEventsService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<EventDTO>();
                conn.CreateTable<UpdateInfo>();
            }
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
    }
}