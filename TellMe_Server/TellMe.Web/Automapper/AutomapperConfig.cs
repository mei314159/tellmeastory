using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;
using TellMe.Web.DTO;
using System;

namespace TellMe.Web.AutoMapper
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StoryRequestDTO, StoryRequest>()
                    .ForMember(x => x.UserId, x =>
                    {
                        x.Condition(y => y.TribeId == null);
                        x.MapFrom(z => z.UserId);
                    })
                    .ForMember(x => x.TribeId, x => x.MapFrom(z => z.TribeId))
                    .ForMember(x => x.CreateDateUtc, x => x.ResolveUsing(y => DateTime.UtcNow));

                cfg.CreateMap<StoryRequest, StoryRequestDTO>()
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.SenderPictureUrl, x => x.MapFrom(z => z.TribeId == null ? z.Sender.PictureUrl : null))
                    .ForMember(x => x.ReceiverName, x => x.MapFrom(z => z.TribeId == null ? z.Sender.UserName : z.Tribe.Name));

                cfg.CreateMap<Story, StoryDTO>()
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.SenderPictureUrl, x => x.MapFrom(z => z.Sender.PictureUrl));

                cfg.CreateMap<StoryReceiver, StoryReceiverDTO>()
                    .ForMember(x => x.ReceiverPictureUrl, x => x.MapFrom(z => z.TribeId == null ? z.User.PictureUrl : null))
                    .ForMember(x => x.ReceiverName, x => x.MapFrom(z => z.TribeId == null ? z.User.UserName : z.Tribe.Name));

                cfg.CreateMap<ApplicationUser, UserDTO>();
                cfg.CreateMap<Notification, NotificationDTO>()
                .ForMember(x => x.Extra, x => x.MapFrom(y => y.Extra));
            });
        }
    }
}