using System.Linq;
using SQLite;
using SQLiteNetExtensions.Extensions;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class LocalAccountService : ILocalAccountService
    {
        private readonly string _dbPath;

        public LocalAccountService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.CreateTable<AuthenticationInfoDTO>();
            }
        }

        public void SaveAuthInfo(AuthenticationInfoDTO authInfo)
        {
            using (var conn = new SQLiteConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                conn.RunInTransaction(() =>
                {
                    if (authInfo == null)
                    {
                        conn.DeleteAll<AuthenticationInfoDTO>();
                    }
                    else
                    {
                        conn.InsertOrReplaceWithChildren(authInfo);
                    }
                });
            }
        }

        public AuthenticationInfoDTO GetAuthInfo()
        {
            using (var conn = new SQLiteConnection(this._dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create))
            {
                var result = conn.GetAllWithChildren<AuthenticationInfoDTO>().FirstOrDefault();
                return result;
            }
        }
    }
}