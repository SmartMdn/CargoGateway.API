using Cargo.Libraries.Logistics.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CargoGateway.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SearchEntity> SearchEntities { get; set; }
    public DbSet<ShipmentEntity> ShipmentEntities { get; set; }
    public DbSet<LegEntity> LegEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<SearchEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Date).HasColumnType("date");
        });
        modelBuilder.Entity<ShipmentEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Search)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.SearchId);
        });
        modelBuilder.Entity<LegEntity>(entity =>
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