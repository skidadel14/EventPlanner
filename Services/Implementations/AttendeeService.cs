using Microsoft.EntityFrameworkCore;
using EventHubAPI.Data;
using EventHubAPI.Models;
using EventHubAPI.DTOs.Attendee;
using EventHubAPI.Services.Interfaces;
using BCrypt.Net;

namespace EventHubAPI.Services.Implementations;

public class AttendeeService : IAttendeeService
{
    private readonly AppDbContext _context;

    public AttendeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AttendeeResponseDto>> GetAllAsync()
    {
        return await _context.Attendees
            .AsNoTracking()
            .Select(a => new AttendeeResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                RegisteredEvents = a.Registrations.Select(r => r.Event.Title).ToList()
            })
            .ToListAsync();
    }

    public async Task<AttendeeResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Attendees
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AttendeeResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                RegisteredEvents = a.Registrations.Select(r => r.Event.Title).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AttendeeResponseDto> CreateAsync(CreateAttendeeDto dto)
    {
        var attendee = new Attendee
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Attendees.Add(attendee);
        await _context.SaveChangesAsync();

        return new AttendeeResponseDto
        {
            Id = attendee.Id,
            Name = attendee.Name,
            Email = attendee.Email,
            RegisteredEvents = new List<string>()
        };
    }

    public async Task<AttendeeResponseDto?> UpdateAsync(int id, UpdateAttendeeDto dto)
    {
        var attendee = await _context.Attendees.FindAsync(id);
        if (attendee == null) return null;

        if (!string.IsNullOrEmpty(dto.Name)) attendee.Name = dto.Name;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attendee = await _context.Attendees.FindAsync(id);
        if (attendee == null) return false;

        _context.Attendees.Remove(attendee);
        await _context.SaveChangesAsync();
        return true;
    }
}
