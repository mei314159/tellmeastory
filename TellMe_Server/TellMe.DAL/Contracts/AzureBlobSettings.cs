namespace TellMe.DAL.Contracts
{
    public class AzureBlobSettings
    {
        public string StorageAccount { get; set; }

        public string StorageKey { get; set; }

        public string VideoContainerName { get; set; }

        public string ImagePreviewContainerName { get; set; }
    }
}