using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
{
    public interface IEventService : IService
    {
        Task<ICollection<EventDTO>> GetAllAsync(string currentUserId, DateTime olderThanUtc);
        
        Task<EventDTO> CreateAsync(string currentUserId, EventDTO newEventDTO);
        
        Task<EventDTO> EditAsync(string currentUserId, EventDTO eventDTO);
        
        Task DeleteAsync(string currentUserId, int eventId);
    }
}