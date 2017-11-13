using System;

namespace TellMe.DAL.Contracts.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public int StoryId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorPictureUrl { get; set; }
    }
}
