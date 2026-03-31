using System.ComponentModel.DataAnnotations;

namespace EventHubAPI.DTOs.Event;

public class CreateEventDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Range(1, 10000)]
    public int Capacity { get; set; }

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;
}

public class UpdateEventDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    [Range(1, 10000)]
    public int? Capacity { get; set; }

    [MaxLength(300)]
    public string? Location { get; set; }
}

public class EventResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public int RegisteredCount { get; set; }
}
