using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Web.DAL.DTO;

namespace TellMe.Web.DAL.Contracts.Services
{
    public interface IEventService : IService
    {
        Task<EventDTO> GetAsync(string currentUserId, int eventId);
        
        Task<ICollection<EventDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc);
        
        Task<EventDTO> CreateAsync(string currentUserId, EventDTO newEventDTO);
        
        Task<EventDTO> UpdateAsync(string currentUserId, EventDTO eventDTO);
        
        Task DeleteAsync(string currentUserId, int eventId);
    }
}