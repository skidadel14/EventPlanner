using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHubAPI.DTOs.Organizer;
using EventHubAPI.Services.Interfaces;

namespace EventHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizersController : ControllerBase
{
    private readonly IOrganizerService _organizerService;

    public OrganizersController(IOrganizerService organizerService)
    {
        _organizerService = organizerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _organizerService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> GetById(int id)
    {
        var organizer = await _organizerService.GetByIdAsync(id);
        if (organizer == null) return NotFound();
        return Ok(organizer);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateOrganizerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _organizerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Organizer")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _organizerService.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _organizerService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
