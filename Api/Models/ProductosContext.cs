using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

public partial class ProductosContext : DbContext
{
    public ProductosContext()
    {
    }

    public ProductosContext(DbContextOptions<ProductosContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Server=localhost;Database=productos;User Id=base;Password=42218872Anto;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FechaDeCarga)
                .HasColumnType("datetime")
                .HasColumnName("fechaDeCarga");
            entity.Property(e => e.Categoria).HasColumnName("Categoria");
            entity.Property(e => e.Precio)
                .HasColumnName("precio");
            entity.Property(e => e.fechaBaja)
                .HasColumnName("fechaBaja")
                .HasColumnType("datetime");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.User1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
