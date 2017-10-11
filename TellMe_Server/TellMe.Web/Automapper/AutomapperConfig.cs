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
                cfg.CreateMap<Story, StoryDTO>();
                cfg.CreateMap<ApplicationUser, UserDTO>();
            });
        }
    }
}