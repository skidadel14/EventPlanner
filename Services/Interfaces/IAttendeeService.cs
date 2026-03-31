using EventHubAPI.DTOs.Attendee;

namespace EventHubAPI.Services.Interfaces;

public interface IAttendeeService
{
    Task<IEnumerable<AttendeeResponseDto>> GetAllAsync();
    Task<AttendeeResponseDto?> GetByIdAsync(int id);
    Task<AttendeeResponseDto> CreateAsync(CreateAttendeeDto dto);
    Task<AttendeeResponseDto?> UpdateAsync(int id, UpdateAttendeeDto dto);
    Task<bool> DeleteAsync(int id);
}
