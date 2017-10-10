using System.IO;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface IStorageService : IService
    {
        Task<UploadMediaDTO> UploadAsync(Stream videoStream, string videoBlobName, Stream previewImageStream, string previewImageBlobName);
    }
}