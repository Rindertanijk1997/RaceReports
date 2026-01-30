using Microsoft.EntityFrameworkCore;
using RaceReports.Data.Entities;

namespace RaceReports.Data;

public class RaceReportsContext : DbContext
{
    public RaceReportsContext(DbContextOptions<RaceReportsContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RaceReport> RaceReports => Set<RaceReport>();
    public DbSet<RaceCategory> Categories => Set<RaceCategory>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed av kategorier
        modelBuilder.Entity<RaceCategory>().HasData(
            new RaceCategory { Id = 1, Name = "5K" },
            new RaceCategory { Id = 2, Name = "10K" },
            new RaceCategory { Id = 3, Name = "Halvmaraton" },
            new RaceCategory { Id = 4, Name = "Maraton" },
            new RaceCategory { Id = 5, Name = "Trail" }
        );

        // Unika index 
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // När en User tas bort och en RaceReport tas bort kan båda försöka cascade Comments.
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.RaceReport)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.RaceReportId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}

