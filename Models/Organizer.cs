namespace EventHubAPI.Models;

public class Organizer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Organizer";

    public OrganizerProfile? Profile { get; set; }          // One-to-One
    public ICollection<Event> Events { get; set; } = new List<Event>();  // One-to-Many
}
