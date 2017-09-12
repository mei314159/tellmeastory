using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TellMe.DAL.Types.Domain
{
    public class RefreshToken:EntityBase<int>
    {
        public string ClientId { get; set; }

        public string Token { get; set; }
        
        public bool Expired { get; set; }

        public string UserId{ get; set; }
    }    
}
