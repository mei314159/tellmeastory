namespace TellMe.Shared.Contracts.DTO.Interfaces
{
    public interface IStoryDTO
    {
        int Id { get; set; }
        string PreviewUrl { get; set; }
        string SenderPictureUrl { get; set; }
        string VideoUrl { get; set; }
        string Title { get; set; }
        bool Objectionable { get; set; }
    }
}