using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventHubAPI.Data;
using EventHubAPI.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace EventHubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("organizer/login")]
    public async Task<IActionResult> OrganizerLogin([FromBody] LoginDto dto)
    {
        var organizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Email == dto.Email);
        if (organizer == null || !BCrypt.Net.BCrypt.Verify(dto.Password, organizer.PasswordHash))
        {
            return BadRequest("Invalid email or password");
        }

        var token = GenerateToken(organizer.Id.ToString(), organizer.Email, organizer.Role);
        return Ok(new { Token = token });
    }

    [HttpPost("attendee/login")]
    public async Task<IActionResult> AttendeeLogin([FromBody] LoginDto dto)
    {
        var attendee = await _context.Attendees.FirstOrDefaultAsync(a => a.Email == dto.Email);
        if (attendee == null || !BCrypt.Net.BCrypt.Verify(dto.Password, attendee.PasswordHash))
        {
            return BadRequest("Invalid email or password");
        }

        var token = GenerateToken(attendee.Id.ToString(), attendee.Email, attendee.Role);
        return Ok(new { Token = token });
    }

    private string GenerateToken(string id, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
