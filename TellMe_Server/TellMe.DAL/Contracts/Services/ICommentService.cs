using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface ICommentService : IService
    {
        Task<BulkDTO<CommentDTO>> GetAllAsync(int storyId, string currentUserId, DateTime olderThanUtc);

        Task<CommentDTO> AddCommentAsync(string currentUserId, int storyId, CommentDTO comment);

        Task DeleteCommentAsync(string currentUserId, int storyId, int commentId);
    }
}