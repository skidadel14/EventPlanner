using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHubAPI.DTOs.Attendee;
using EventHubAPI.Services.Interfaces;

namespace EventHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendeesController : ControllerBase
{
    private readonly IAttendeeService _attendeeService;

    public AttendeesController(IAttendeeService attendeeService)
    {
        _attendeeService = attendeeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _attendeeService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Attendee")]
    public async Task<IActionResult> GetById(int id)
    {
        var attendee = await _attendeeService.GetByIdAsync(id);
        if (attendee == null) return NotFound();
        return Ok(attendee);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateAttendeeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _attendeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Attendee")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendeeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _attendeeService.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _attendeeService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
