using System.ComponentModel.DataAnnotations;

namespace EventHubAPI.DTOs.Attendee;

public class CreateAttendeeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class UpdateAttendeeDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
}

public class AttendeeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> RegisteredEvents { get; set; } = new List<string>();
}
