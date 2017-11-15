using System.IO;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteAccountDataService : IRemoteDataService
    {
        Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> SignInAsync(string email, string password);
        Task<Result> SignUpAsync(SignUpDTO dto);
        Task<Result<ProfilePictureDTO>> SetProfilePictureAsync(Stream profilePictureStream);
    }
}