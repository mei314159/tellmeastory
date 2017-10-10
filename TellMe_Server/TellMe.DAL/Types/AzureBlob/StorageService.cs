using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            UploadMediaDTO result = new UploadMediaDTO
            {
                VideoUrl = videoBlob.Uri.ToString(),
                PreviewImageUrl = previewImageBlob.Uri.ToString(),
            };

            return result;
        }

        private async Task<CloudBlobContainer> GetContainerAsync(string containerName)
        {
            //Account
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(_settings.StorageAccount, _settings.StorageKey), true);

            //Client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Container
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            return blobContainer;
        }

        private async Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blobName)
        {
            //Container
            CloudBlobContainer blobContainer = await GetContainerAsync(containerName).ConfigureAwait(false);

            //Blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

            return blockBlob;
        }

        private async Task<List<AzureBlobItem>> GetBlobListAsync(string containerName, bool useFlatListing = true)
        {
            //Container
            CloudBlobContainer blobContainer = await GetContainerAsync(containerName).ConfigureAwait(false);

            //List
            var list = new List<AzureBlobItem>();
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment =
                    await blobContainer
                    .ListBlobsSegmentedAsync(string.Empty, useFlatListing, new BlobListingDetails(), null, token, null, null)
                    .ConfigureAwait(false);
                token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    list.Add(new AzureBlobItem(item));
                }
            } while (token != null);

            return list.OrderBy(i => i.Folder).ThenBy(i => i.Name).ToList();
        }
    }
}