using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.Services
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