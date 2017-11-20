using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TellMe.DAL.Types.PushNotifications;

namespace TellMe.DAL.Types.Domain
{
    public class Notification : EntityBase<int>
    {
        internal string _extra { get; set; }

        public string RecipientId { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public bool Handled { get; set; }

        public NotificationTypeEnum Type { get; set; }

        [NotMapped]
        public object Extra
        {
            get
            {
                return JObject.Parse(_extra);
            }
            set
            {
                _extra = JsonConvert.SerializeObject(value);
            }
        }

        public virtual ApplicationUser Recipient { get; set; }
    }
}
