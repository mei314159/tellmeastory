using System;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteCommentsDataService : IRemoteCommentsDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteCommentsDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<CommentDTO>> AddCommentAsync(CommentDTO comment)
        {
            var result = await this._apiProvider.PostAsync<CommentDTO>($"stories/{comment.StoryId}/comments", comment).ConfigureAwait(false);

            return result;
        }

        public async Task<Result> DeleteCommentAsync(int storyId, int commentId)
        {
            var result = await this._apiProvider.DeleteAsync<object>($"stories/{storyId}/comments/{commentId}", null)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<Result<BulkDTO<CommentDTO>>> GetCommentsAsync(int storyId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider
                .GetAsync<BulkDTO<CommentDTO>>($"stories/{storyId}/comments/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }
        
        public async Task<Result<BulkDTO<CommentDTO>>> GetRepliesAsync(int storyId, int commentId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider
                .GetAsync<BulkDTO<CommentDTO>>($"stories/{storyId}/comments/{commentId}/replies/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }
    }
}