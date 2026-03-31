namespace EventHubAPI.Models;

public class OrganizerProfile
{
    public int Id { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public int OrganizerId { get; set; }           // FK
    public Organizer Organizer { get; set; } = null!;
}
