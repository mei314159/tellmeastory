using TellMe.Shared.Contracts.DTO.Interfaces;

namespace TellMe.Shared.Contracts.DTO
{
    public class StoryOrderDTO : IStoryOrderDTO
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }
}