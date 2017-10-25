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

                cfg.CreateMap<Tribe, TribeDTO>()
                    .ForMember(x => x.Members, x => x.Ignore());

                cfg.CreateMap<TribeMember, TribeMemberDTO>()
                  .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.UserName))
                  .ForMember(x => x.UserPictureUrl, x => x.MapFrom(y => y.User.PictureUrl));
                cfg.CreateMap<ApplicationUser, TribeDTO>()
                        .ForMember(x => x.CreatorName, x => x.MapFrom(y => y.UserName))
                        .ForMember(x => x.CreatorPictureUrl, x => x.MapFrom(y => y.PictureUrl))
                        .ForMember(x => x.CreatorId, x => x.MapFrom(y => y.Id))
                        .ForAllOtherMembers(x => x.Ignore());


                // cfg.CreateMap<TribeDTO, Tribe>()
                //     .ForMember(x => x.Members, x => x.MapFrom(y => y.Members.Select(m => new TribeMember { UserId = m.Id }).ToList()));

                cfg.CreateMap<ApplicationUser, UserDTO>();
                cfg.CreateMap<Notification, NotificationDTO>()
                .ForMember(x => x.Extra, x => x.MapFrom(y => y.Extra));
            });
        }
    }
}