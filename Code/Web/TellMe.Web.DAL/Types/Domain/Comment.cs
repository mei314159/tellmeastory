using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace TellMe.DAL.Types.Domain
{
    public class Comment : EntityBase<int>
    {
        [MaxLength(500)]
        public string Text { get; set; }

        public DateTime CreateDateUtc { get; set; }
        public string AuthorId { get; set; }
        public int StoryId { get; set; }
        public int? ReplyToCommentId { get; set; }
        public int RepliesCount { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual Story Story { get; set; }
        public virtual Comment ReplyToComment { get; set; }
    }
}