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
        SetAuthCookie(token);
        return Ok(new { Message = "Login successful", Role = organizer.Role });
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
        SetAuthCookie(token);
        return Ok(new { Message = "Login successful", Role = attendee.Role });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        return Ok(new { Message = "Logged out successfully" });
    }

    private string GenerateToken(string id, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        const string jwtKey = "EventHubSuperSecretJwtKeyThatIsAtLeast32Chars!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "EventHubAPI",
            audience: "EventHubClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void SetAuthCookie(string token)
    {
        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,        // Not accessible via JavaScript (XSS protection)
            Secure = false,         // Set to true in production with HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });
    }
}
