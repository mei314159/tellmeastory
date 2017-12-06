using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Types.Domain
{
    public class Notification : EntityBase<int>
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal string _extra { get; set; }

        public string RecipientId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public bool Handled { get; set; }

        public NotificationTypeEnum Type { get; set; }

        [NotMapped]
        public object Extra
        {
            get => JObject.Parse(_extra);
            set => _extra = JsonConvert.SerializeObject(value);
        }

        public virtual ApplicationUser Recipient { get; set; }
    }
}
