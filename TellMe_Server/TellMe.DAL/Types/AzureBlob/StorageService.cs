using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Contracts.Services;

namespace TellMe.DAL.Types.AzureBlob
{
    public class StorageService : IStorageService
    {
        private readonly AzureBlobSettings _settings;

        public StorageService(IOptions<AzureBlobSettings> settings)
        {
            _settings = settings.Value;
        }
        
        public async Task<UploadMediaDTO> UploadAsync(Stream videoStream, string videoBlobName, Stream previewImageStream, string previewImageBlobName)
        {
            CloudBlockBlob videoBlob = await GetBlockBlobAsync(_settings.VideoContainerName, videoBlobName);
            videoStream.Position = 0;
            await videoBlob.UploadFromStreamAsync(videoStream).ConfigureAwait(false);

            CloudBlockBlob previewImageBlob = await GetBlockBlobAsync(_settings.ImagePreviewContainerName, previewImageBlobName);
            previewImageStream.Position = 0;
            await previewImageBlob.UploadFromStreamAsync(previewImageStream).ConfigureAwait(false);

            var result = new UploadMediaDTO
            {
                VideoUrl = videoBlob.Uri.ToString(),
                PreviewImageUrl = previewImageBlob.Uri.ToString(),
            };

            return result;
        }

        public async Task<ProfilePictureDTO> UploadProfilePictureAsync(Stream profilePictureStream, string profilePictureBlobName)
        {
            CloudBlockBlob profilePictureBlob = await GetBlockBlobAsync(_settings.ProfilePictureContainerName, profilePictureBlobName);
            profilePictureStream.Position = 0;
            await profilePictureBlob.UploadFromStreamAsync(profilePictureStream).ConfigureAwait(false);

            var result = new ProfilePictureDTO
            {
                PictureUrl = profilePictureBlob.Uri.ToString(),
            };

            return result;
        }

        private async Task<CloudBlobContainer> GetContainerAsync(string containerName)
        {
            //Account
            var storageAccount = new CloudStorageAccount(new StorageCredentials(_settings.StorageAccount, _settings.StorageKey), true);

            //Client
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Container
            var blobContainer = blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            return blobContainer;
        }

        private async Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blobName)
        {
            //Container
            var blobContainer = await GetContainerAsync(containerName).ConfigureAwait(false);

            //Blob
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            return blockBlob;
        }
    }
}