using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteCommentsDataService : IRemoteDataService
    {
        Task<Result<CommentDTO>> AddCommentAsync(int storyId, string text);
        Task<Result> DeleteCommentAsync(int storyId, int commentId);
        Task<Result<BulkDTO<CommentDTO>>> GetCommentsAsync(int storyId, DateTime? olderThanUtc = null);
    }
}