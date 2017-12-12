using System.IO;
using System.Threading.Tasks;
using TellMe.Web.DAL.DTO;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface IStorageService : IService
    {
        Task<UploadMediaDTO> UploadAsync(Stream videoStream, string videoBlobName, Stream previewImageStream, string previewImageBlobName);

        Task<ProfilePictureDTO> UploadProfilePictureAsync(Stream profilePictureStream, string profilePictureBlobName);

    }
}