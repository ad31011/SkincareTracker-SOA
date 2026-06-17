using Microsoft.EntityFrameworkCore;
using SkincareTracker.API.Models;

namespace SkincareTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>             Users             { get; set; }
    public DbSet<Product>          Products          { get; set; }
    public DbSet<Ingredient>       Ingredients       { get; set; }
    public DbSet<ProductIngredient> ProductIngredients { get; set; }
    public DbSet<Routine>          Routines          { get; set; }
    public DbSet<RoutineProduct>   RoutineProducts   { get; set; }
    public DbSet<SkinLog>          SkinLogs          { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite keys for join tables
        modelBuilder.Entity<ProductIngredient>()
            .HasKey(pi => new { pi.ProductId, pi.IngredientId });

        modelBuilder.Entity<RoutineProduct>()
            .HasKey(rp => new { rp.RoutineId, rp.ProductId });

        // Relationships
        modelBuilder.Entity<ProductIngredient>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.ProductIngredients)
            .HasForeignKey(pi => pi.ProductId);

        modelBuilder.Entity<ProductIngredient>()
            .HasOne(pi => pi.Ingredient)
            .WithMany(i => i.ProductIngredients)
            .HasForeignKey(pi => pi.IngredientId);

        modelBuilder.Entity<RoutineProduct>()
            .HasOne(rp => rp.Routine)
            .WithMany(r => r.RoutineProducts)
            .HasForeignKey(rp => rp.RoutineId);


        modelBuilder.Entity<RoutineProduct>()
            .HasOne(rp => rp.Product)
            .WithMany(p => p.RoutineProducts)
            .HasForeignKey(rp => rp.ProductId);

        modelBuilder.Entity<Routine>()
            .HasOne(r => r.User)
            .WithMany(u => u.Routines)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<SkinLog>()
            .HasOne(sl => sl.User)
            .WithMany(u => u.SkinLogs)
            .HasForeignKey(sl => sl.UserId);

        // Indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Seed data
        modelBuilder.Entity<Ingredient>().HasData(
            new Ingredient { Id = 1, Name = "Retinol", ConflictsWith = "AHA,BHA,Vitamin C", Notes = "Use at night only. Avoid with acids." },
            new Ingredient { Id = 2, Name = "Vitamin C", ConflictsWith = "Retinol,Niacinamide", Notes = "Best used in AM. Unstable with some actives." },
            new Ingredient { Id = 3, Name = "Niacinamide", ConflictsWith = "Vitamin C", Notes = "Great for pores and brightening." },
            new Ingredient { Id = 4, Name = "AHA", ConflictsWith = "Retinol,BHA", Notes = "Exfoliant. Do not layer with retinol." },
            new Ingredient { Id = 5, Name = "BHA", ConflictsWith = "Retinol,AHA", Notes = "Salicylic acid family. Unclogs pores." },
            new Ingredient { Id = 6, Name = "Hyaluronic Acid", ConflictsWith = "", Notes = "Safe to combine with most actives." },
            new Ingredient { Id = 7, Name = "SPF", ConflictsWith = "", Notes = "Always last step in AM routine." },
            new Ingredient { Id = 8, Name = "Peptides", ConflictsWith = "AHA,BHA", Notes = "Broken down by acids." }
        );
    }
}
