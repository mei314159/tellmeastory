using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;
using TellMe.Web.DTO;

namespace TellMe.Web.AutoMapper
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Story, StoryDTO>()
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.ReceiverName, x => x.MapFrom(z => z.Receiver.UserName))
                    .ForMember(x => x.SenderPictureUrl, x => x.MapFrom(z => z.Sender.PictureUrl))
                    .ForMember(x => x.ReceiverPictureUrl, x => x.MapFrom(z => z.Receiver.PictureUrl));
                cfg.CreateMap<ApplicationUser, UserDTO>();
            });
        }
    }
}