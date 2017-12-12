using System;
using System.Threading.Tasks;
using TellMe.Web.DAL.DTO;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface ICommentService : IService
    {
        Task<BulkDTO<CommentDTO>> GetAllAsync(int storyId, string currentUserId, DateTime olderThanUtc, int? replyToCommentId = null);

        Task<CommentDTO> AddCommentAsync(string currentUserId, int storyId, CommentDTO comment);

        Task DeleteCommentAsync(string currentUserId, int storyId, int commentId);
    }
}