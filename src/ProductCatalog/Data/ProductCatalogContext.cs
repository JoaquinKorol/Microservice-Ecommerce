using Microsoft.EntityFrameworkCore;
using ProductCatalog.Models;

public class ProductCatalogContext : DbContext
{
    public ProductCatalogContext(DbContextOptions<ProductCatalogContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id); // Clave primaria
            entity.Property(p => p.Id)
                  .ValueGeneratedOnAdd(); // Autoincremental

            entity.Property(p => p.Name)
                  .IsRequired() // No permite nulos
                  .HasMaxLength(100); // Longitud máxima opcional

            entity.Property(p => p.Description)
                  .IsRequired(); // No permite nulos

            entity.Property(p => p.Price)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)"); // Configuración de tipo decimal

            entity.Property(p => p.Stock)
                  .IsRequired(); // No permite nulos
        });

        base.OnModelCreating(modelBuilder);
    }
}
