namespace EventHubAPI.Models;

public class Registration
{
    public int AttendeeId { get; set; }
    public Attendee Attendee { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
