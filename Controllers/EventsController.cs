using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHubAPI.DTOs.Event;
using EventHubAPI.Services.Interfaces;
using System.Security.Claims;

namespace EventHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _eventService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null || !int.TryParse(userIdString, out var organizerId))
        {
            return Unauthorized("Invalid identity token.");
        }
        
        var result = await _eventService.CreateAsync(organizerId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _eventService.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _eventService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/register")]
    [Authorize(Roles = "Attendee")]
    public async Task<IActionResult> RegisterForEvent(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null || !int.TryParse(userIdString, out var attendeeId))
        {
            return Unauthorized("Invalid identity token.");
        }
        
        var success = await _eventService.RegisterAttendeeAsync(id, attendeeId);
        if (!success) return BadRequest("Could not register for event. It might be full, or you are already registered.");
        return Ok("Successfully registered for the event.");
    }
}
