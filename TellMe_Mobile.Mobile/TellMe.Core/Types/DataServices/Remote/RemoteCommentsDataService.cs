using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteCommentsDataService : BaseDataService
    {
        public async Task<Result<CommentDTO>> AddCommentAsync(int storyId, string text)
        {
            var result = await this.PostAsync<CommentDTO>($"stories/{storyId}/comments", new CommentDTO
            {
                Text = text
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<Result> DeleteCommentAsync(int storyId, int commentId)
        {
            var result = await this.DeleteAsync<object>($"stories/{storyId}/comments/{commentId}", null).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<BulkDTO<CommentDTO>>> GetCommentsAsync(int storyId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this.GetAsync<BulkDTO<CommentDTO>>($"stories/{storyId}/comments/older-than/{olderThan.Ticks}").ConfigureAwait(false);
            return result;
        }
    }
}
