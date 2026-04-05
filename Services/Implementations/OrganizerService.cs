using Microsoft.EntityFrameworkCore;
using EventHubAPI.Data;
using EventHubAPI.Models;
using EventHubAPI.DTOs.Organizer;
using EventHubAPI.Services.Interfaces;
using BCrypt.Net;

namespace EventHubAPI.Services.Implementations;

public class OrganizerService : IOrganizerService
{
    private readonly AppDbContext _context;

    public OrganizerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrganizerResponseDto>> GetAllAsync()
    {
        return await _context.Organizers
            .AsNoTracking()
            .Select(o => new OrganizerResponseDto
            {
                Id = o.Id,
                Name = o.Name,
                Email = o.Email,
                Bio = o.Profile != null ? o.Profile.Bio : null,
                Phone = o.Profile != null ? o.Profile.Phone : null,
                Website = o.Profile != null ? o.Profile.Website : null,
                TotalEvents = o.Events.Count
            })
            .ToListAsync();
    }

    public async Task<OrganizerResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Organizers
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizerResponseDto
            {
                Id = o.Id,
                Name = o.Name,
                Email = o.Email,
                Bio = o.Profile != null ? o.Profile.Bio : null,
                Phone = o.Profile != null ? o.Profile.Phone : null,
                Website = o.Profile != null ? o.Profile.Website : null,
                TotalEvents = o.Events.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<OrganizerResponseDto> CreateAsync(CreateOrganizerDto dto)
    {
        var organizer = new Organizer
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Profile = new OrganizerProfile
            {
                Bio = dto.Bio ?? string.Empty,
                Phone = dto.Phone ?? string.Empty,
                Website = dto.Website ?? string.Empty
            }
        };

        _context.Organizers.Add(organizer);
        await _context.SaveChangesAsync();

        return new OrganizerResponseDto
        {
            Id = organizer.Id,
            Name = organizer.Name,
            Email = organizer.Email,
            Bio = organizer.Profile.Bio,
            Phone = organizer.Profile.Phone,
            Website = organizer.Profile.Website,
            TotalEvents = 0
        };
    }

    public async Task<OrganizerResponseDto?> UpdateAsync(int id, UpdateOrganizerDto dto)
    {
        var organizer = await _context.Organizers
            .Include(o => o.Profile)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (organizer == null) return null;

        if (!string.IsNullOrEmpty(dto.Name)) organizer.Name = dto.Name;
        
        if (organizer.Profile == null) 
        {
            organizer.Profile = new OrganizerProfile { OrganizerId = id };
        }
        
        if (dto.Bio != null) organizer.Profile.Bio = dto.Bio;
        if (dto.Phone != null) organizer.Profile.Phone = dto.Phone;
        if (dto.Website != null) organizer.Profile.Website = dto.Website;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var organizer = await _context.Organizers.FindAsync(id);
        if (organizer == null) return false;

        _context.Organizers.Remove(organizer);
        await _context.SaveChangesAsync();
        return true;
    }
}
