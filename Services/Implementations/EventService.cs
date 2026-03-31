using Microsoft.EntityFrameworkCore;
using EventHubAPI.Data;
using EventHubAPI.Models;
using EventHubAPI.DTOs.Event;
using EventHubAPI.Services.Interfaces;

namespace EventHubAPI.Services.Implementations;

public class EventService : IEventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EventResponseDto>> GetAllAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Capacity = e.Capacity,
                Location = e.Location,
                OrganizerName = e.Organizer.Name,
                RegisteredCount = e.Registrations.Count()
            })
            .ToListAsync();
    }

    public async Task<EventResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Events
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Capacity = e.Capacity,
                Location = e.Location,
                OrganizerName = e.Organizer.Name,
                RegisteredCount = e.Registrations.Count()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EventResponseDto> CreateAsync(int organizerId, CreateEventDto dto)
    {
        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            Capacity = dto.Capacity,
            Location = dto.Location,
            OrganizerId = organizerId
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(ev.Id))!;
    }

    public async Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto dto)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return null;

        if (dto.Title != null) ev.Title = dto.Title;
        if (dto.Description != null) ev.Description = dto.Description;
        if (dto.Date.HasValue) ev.Date = dto.Date.Value;
        if (dto.Capacity.HasValue) ev.Capacity = dto.Capacity.Value;
        if (dto.Location != null) ev.Location = dto.Location;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return false;

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RegisterAttendeeAsync(int eventId, int attendeeId)
    {
        var ev = await _context.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev == null) return false;

        if (ev.Registrations.Count >= ev.Capacity) return false; // At capacity

        if (ev.Registrations.Any(r => r.AttendeeId == attendeeId)) return false; // Already registered

        var registration = new Registration
        {
            EventId = eventId,
            AttendeeId = attendeeId,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();
        return true;
    }
}
