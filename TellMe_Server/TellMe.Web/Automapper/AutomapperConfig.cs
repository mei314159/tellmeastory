using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.Web.AutoMapper
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StoryRequestDTO, Story>();
                cfg.CreateMap<Story, StoryDTO>();
            });
        }
    }
}