using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteCommentsDataService : IRemoteDataService
    {
        Task<Result<CommentDTO>> AddCommentAsync(CommentDTO comment);
        Task<Result> DeleteCommentAsync(int storyId, int commentId);
        Task<Result<BulkDTO<CommentDTO>>> GetCommentsAsync(int storyId, DateTime? olderThanUtc = null);
        Task<Result<BulkDTO<CommentDTO>>> GetRepliesAsync(int storyId, int commentId, DateTime? olderThanUtc = null);
    }
}