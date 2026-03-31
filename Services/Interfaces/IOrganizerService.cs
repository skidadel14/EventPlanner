using EventHubAPI.DTOs.Organizer;

namespace EventHubAPI.Services.Interfaces;

public interface IOrganizerService
{
    Task<IEnumerable<OrganizerResponseDto>> GetAllAsync();
    Task<OrganizerResponseDto?> GetByIdAsync(int id);
    Task<OrganizerResponseDto> CreateAsync(CreateOrganizerDto dto);
    Task<OrganizerResponseDto?> UpdateAsync(int id, UpdateOrganizerDto dto);
    Task<bool> DeleteAsync(int id);
}
