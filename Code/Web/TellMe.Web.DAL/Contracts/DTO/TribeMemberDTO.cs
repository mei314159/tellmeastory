using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class TribeMemberDTO
    {
        public int Id { get; set; }

        public int TribeId { get; set; }

        public string TribeName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string UserPictureUrl { get; set; }

        public TribeMemberStatus Status { get; set; }
    }
}
