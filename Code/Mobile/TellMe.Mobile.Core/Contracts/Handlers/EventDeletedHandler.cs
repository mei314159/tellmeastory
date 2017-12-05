using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.Handlers
{
    public delegate void EventDeletedHandler (EventDTO eventDTO);
    public delegate void PlaylistDeletedHandler (PlaylistDTO playlistDTO);
}