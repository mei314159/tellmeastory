using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalContactsDataService
    {
        private readonly string _dbPath;

        public LocalContactsDataService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath))
            {
                conn.CreateTable<ContactDTO>();
                conn.CreateTable<UpdateInfo>();
            }
        }

        public async Task SaveContactsAsync(ICollection<ContactDTO> contacts)
        {
            var conn = new SQLiteAsyncConnection(this._dbPath);
            await conn.RunInTransactionAsync((SQLiteConnection c) =>
            {
                foreach (var contact in contacts)
                {
                    c.InsertOrReplace(contact, typeof(ContactDTO));
                }
                c.InsertOrReplace(new UpdateInfo { UtcDate = DateTime.UtcNow, TableName = "Contacts" });
            }).ConfigureAwait(false);
        }

        public async Task<DataResult<ICollection<ContactDTO>>> GetAllAsync()
        {
            var conn = new SQLiteAsyncConnection(this._dbPath);
            var result = await conn.QueryAsync<ContactDTO>("SELECT * FROM Contacts").ConfigureAwait(false);
            var updateInfo = await conn.FindAsync<UpdateInfo>("Contacts").ConfigureAwait(false);
            return new DataResult<ICollection<ContactDTO>>(updateInfo?.UtcDate ?? DateTime.MinValue, result);
        }
    }
}
