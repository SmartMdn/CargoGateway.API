using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Cargo.Libraries.Logistics.Models.Models;

namespace CargoGateway.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Search> SearchEntities { get; set; }
    public DbSet<Shipment> ShipmentEntities { get; set; }
    public DbSet<Leg> LegEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<Search>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Date).HasColumnType("date");
        });
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Search)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.SearchId);
        });
        modelBuilder.Entity<Leg>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x=>x.DepartureDate).HasColumnType("date");
            entity.Property(x=>x.DepartureTime).HasColumnType("time");
            entity.Property(x=>x.ArrivalDate).HasColumnType("date");
            entity.Property(x=>x.ArrivalTime).HasColumnType("time");
            entity.HasOne(x => x.Shipment)
                .WithMany(x => x.Legs)
                .HasForeignKey(x => x.ShipmentId);
        });
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}