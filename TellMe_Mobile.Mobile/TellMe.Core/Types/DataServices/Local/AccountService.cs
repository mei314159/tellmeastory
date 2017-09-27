using System.Linq;
using SQLite;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Local
{
    public class AccountService
    {
        private readonly string _dbPath;

        public AccountService()
        {
            this._dbPath = Constants.LocalDbPath;
            using (var conn = new SQLiteConnection(_dbPath))
            {
                conn.CreateTable<AuthenticationInfoDTO>();
            }
        }

        public void SaveAuthInfo(AuthenticationInfoDTO authInfo)
        {
            var conn = new SQLiteConnection(this._dbPath);
            conn.RunInTransaction(() =>
            {
                if (authInfo == null)
                {
                    conn.DeleteAll<AuthenticationInfoDTO>();
                }
                else
                {
                    conn.InsertOrReplace(authInfo, typeof(AuthenticationInfoDTO));
                }
            });
        }

        public AuthenticationInfoDTO GetAuthInfo()
        {
            var conn = new SQLiteConnection(this._dbPath);
            var result = conn.Query<AuthenticationInfoDTO>("SELECT * FROM AuthenticationInfo").FirstOrDefault();
            return result;
        }
    }
}
