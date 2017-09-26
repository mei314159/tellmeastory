using System;
namespace TellMe.Core.Contracts.DataServices
{
    [SQLite.Table("UpdateInfo")]
    public class UpdateInfo
    {
        [SQLite.PrimaryKey]
        public string TableName { get; set; }

        public DateTime UtcDate { get; set; }
    }
}
