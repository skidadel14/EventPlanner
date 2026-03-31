using EventHubAPI.DTOs.Event;

namespace EventHubAPI.Services.Interfaces;

public interface IEventService
{
    Task<IEnumerable<EventResponseDto>> GetAllAsync();
    Task<EventResponseDto?> GetByIdAsync(int id);
    Task<EventResponseDto> CreateAsync(int organizerId, CreateEventDto dto);
    Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> RegisterAttendeeAsync(int eventId, int attendeeId);
}
