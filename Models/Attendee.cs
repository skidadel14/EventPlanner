namespace EventHubAPI.Models;

public class Attendee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Attendee";

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
