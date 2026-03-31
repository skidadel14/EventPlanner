using Microsoft.EntityFrameworkCore;
using EventHubAPI.Models;

namespace EventHubAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organizer> Organizers => Set<Organizer>();
    public DbSet<OrganizerProfile> OrganizerProfiles => Set<OrganizerProfile>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Attendee> Attendees => Set<Attendee>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One-to-One: Organizer <-> OrganizerProfile
        modelBuilder.Entity<Organizer>()
            .HasOne(o => o.Profile)
            .WithOne(p => p.Organizer)
            .HasForeignKey<OrganizerProfile>(p => p.OrganizerId);

        // One-to-Many: Organizer -> Events
        modelBuilder.Entity<Organizer>()
            .HasMany(o => o.Events)
            .WithOne(e => e.Organizer)
            .HasForeignKey(e => e.OrganizerId);

        // Many-to-Many: Attendee <-> Event via Registration
        modelBuilder.Entity<Registration>()
            .HasKey(r => new { r.AttendeeId, r.EventId });

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Attendee)
            .WithMany(a => a.Registrations)
            .HasForeignKey(r => r.AttendeeId);

        modelBuilder.Entity<Registration>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId);
    }
}
