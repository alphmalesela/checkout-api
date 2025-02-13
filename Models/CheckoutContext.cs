using Microsoft.EntityFrameworkCore;

public class CheckoutContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CheckoutItem> CheckoutItems { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }

    // public string DbPath { get; }

    public CheckoutContext(DbContextOptions<CheckoutContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Option 2: Configure the property using Fluent API
        modelBuilder.Entity<User>()
            .Property(u => u.ApiKey)
            .HasColumnName("ApiKey");
    }
}