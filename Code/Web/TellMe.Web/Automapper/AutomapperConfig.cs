using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TellMe.Shared.Contracts.DTO;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Extensions;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.Automapper
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StoryRequest, StoryRequestDTO>()
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.SenderPictureUrl,
                        x => x.MapFrom(z => z.TribeId == null ? z.Sender.PictureUrl : null))
                    .ForMember(x => x.ReceiverName,
                        x => x.MapFrom(z => z.TribeId == null ? z.Sender.UserName : z.Tribe.Name));

                cfg.CreateMap<Story, StoryDTO>()
                    .ForMember(x => x.Liked, x =>
                    {
                        x.PreCondition((e) => e.Items.ContainsKey("UserId"));
                        x.Condition((e) => e.Likes != null);
                        x.ResolveUsing((y, a, b, c) =>
                        {
                            var result = y.Likes.Any(m => m.UserId == (string) c.Items["UserId"]);
                            return result;
                        });
                    })
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.SenderPictureUrl, x => x.MapFrom(z => z.Sender.PictureUrl));

                cfg.CreateMap<Story, StoryListDTO>()
                    .ForMember(x => x.SenderName, x => x.MapFrom(z => z.Sender.UserName))
                    .ForMember(x => x.SenderPictureUrl, x => x.MapFrom(z => z.Sender.PictureUrl));

                cfg.CreateMap<Event, EventDTO>()
                    .ForMember(x => x.HostUserName, x => x.MapFrom(z => z.Host.UserName))
                    .ForMember(x => x.HostPictureUrl, x => x.MapFrom(z => z.Host.PictureUrl))
                    .ForMember(x => x.Attendees,
                        x => x.MapFrom(y => y.Attendees.Where(z =>
                                z.Status == EventAttendeeStatus.Accepted || z.Status == EventAttendeeStatus.Pending)
                            .ToList()));

                cfg.CreateMap<EventDTO, Event>()
                    .ForMember(x => x.Id, x => x.Ignore())
                    .ForMember(x => x.HostId, x => x.Ignore())
                    .ForMember(x => x.CreateDateUtc, x => x.MapFrom(y => DateTime.UtcNow))
                    .ForMember(x => x.DateUtc, x => x.MapFrom(y => y.DateUtc.Date))
                    .ForMember(x => x.Attendees, x => x.Ignore());

                cfg.CreateMap<Playlist, PlaylistDTO>()
                    .ForMember(x => x.Stories,
                        x => x.MapFrom(y => y.Stories.OrderBy(d => d.Order).Select(a => a.Story)));

                cfg.CreateMap<PlaylistDTO, Playlist>()
                    .ForMember(x => x.Id, x => x.Ignore())
                    .ForMember(x => x.UserId, x => x.Ignore())
                    .ForMember(x => x.CreateDateUtc, x => x.MapFrom(y => DateTime.UtcNow))
                    .ForMember(x => x.Stories, x => x.Ignore())
                    .BeforeMap((dto, entity) =>
                    {
                        if (entity.Stories == null)
                            entity.Stories = new List<PlaylistStory>();
                    })
                    .AfterMap((dto, entity, afterFunction) =>
                    {
                        entity.Stories.MapFrom(dto.Stories, x => x.Id, x => x.StoryId,
                            (storyDTO, playlistStory) =>
                            {
                                playlistStory.StoryId = storyDTO.Id;
                                playlistStory.PlaylistId = entity.Id;
                            });
                    });

                cfg.CreateMap<EventAttendeeDTO, EventAttendee>()
                    .ForMember(x => x.Id, x => x.Ignore())
                    .ForMember(x => x.UserId, x =>
                    {
                        x.PreCondition(y => y.TribeId == null);
                        x.MapFrom(z => z.UserId);
                    })
                    .ForMember(x => x.TribeId, x => x.MapFrom(z => z.TribeId))
                    .ForMember(x => x.CreateDateUtc, x => x.ResolveUsing(y => DateTime.UtcNow));

                cfg.CreateMap<EventAttendee, EventAttendeeDTO>()
                    .ForMember(x => x.AttendeePictureUrl,
                        x => x.MapFrom(z => z.TribeId == null ? z.User.PictureUrl : null))
                    .ForMember(x => x.AttendeeName,
                        x => x.MapFrom(z => z.TribeId == null ? z.User.UserName : z.Tribe.Name))
                    .ForMember(x => x.AttendeeFullName,
                        x => x.MapFrom(z => z.TribeId == null ? z.User.FullName : null));

                cfg.CreateMap<StoryReceiver, StoryReceiverDTO>()
                    .ForMember(x => x.StoryId, x => x.MapFrom(z => z.StoryId))
                    .ForMember(x => x.ReceiverPictureUrl,
                        x => x.MapFrom(z => z.TribeId == null ? z.User.PictureUrl : null))
                    .ForMember(x => x.ReceiverName,
                        x => x.MapFrom(z => z.TribeId == null ? z.User.UserName : z.Tribe.Name));

                cfg.CreateMap<Comment, CommentDTO>()
                    .ForMember(x => x.AuthorPictureUrl, x => x.MapFrom(z => z.Author.PictureUrl))
                    .ForMember(x => x.AuthorUserName, x => x.MapFrom(z => z.Author.UserName));

                cfg.CreateMap<CommentDTO, Comment>()
                    .ForMember(x => x.Text, x => x.MapFrom(z => z.Text))
                    .ForMember(x => x.AuthorId, x => x.MapFrom(z => z.AuthorId))
                    .ForMember(x => x.ReplyToCommentId, x => x.MapFrom(z => z.ReplyToCommentId))
                    .ForAllOtherMembers(x => x.Ignore());

                cfg.CreateMap<Tribe, SharedTribeDTO>()
                    .ForMember(x => x.CreatorName, x =>
                    {
                        x.PreCondition(y => y.Creator != null);
                        x.MapFrom(y => y.Creator.UserName);
                    })
                    .ForMember(x => x.CreatorPictureUrl, x =>
                    {
                        x.PreCondition(y => y.Creator != null);
                        x.MapFrom(y => y.Creator.PictureUrl);
                    })
                    .ForMember(x => x.Members, x => x.PreCondition((e) => e.Items.ContainsKey("Members")))
                    .ForMember(x => x.MembershipStatus, x =>
                    {
                        x.PreCondition((e) => e.Items.ContainsKey("UserId"));
                        x.Condition((e) => e.Members != null);
                        x.ResolveUsing((y, a, b, c) =>
                        {
                            var result = y.Members.First(m => m.UserId == (string) c.Items["UserId"]).Status;
                            return result;
                        });
                    });

                cfg.CreateMap<TribeMember, SharedTribeMemberDTO>()
                    .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.UserName))
                    .ForMember(x => x.FullName, x => x.MapFrom(y => y.User.FullName))
                    .ForMember(x => x.UserPictureUrl, x => x.MapFrom(y => y.User.PictureUrl));
                cfg.CreateMap<ApplicationUser, SharedTribeDTO>()
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