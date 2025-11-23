using Dominio.DetalleVentas;
using Dominio.EncabezadoVentas;
using Dominio.Productos;
using Dominio.Usuarios;
using Dominio.Vendedores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Infraestructura.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; }

    public virtual DbSet<EncabezadoVenta> EncabezadoVentas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    //public virtual DbSet<Vendedor> Vendedores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Test_24Nov2025_DB;User Id=sa;Password=G1lb3rt0;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.Idde).HasName("DetalleVentas_pk");

            entity.Property(e => e.Idde).HasColumnName("idde");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.Idpro).HasColumnName("idpro");
            entity.Property(e => e.Idventa).HasColumnName("idventa");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("iva");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("precio");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("total");

            entity.HasOne(d => d.Productos).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.Idpro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetalleVentas_Productos   _idpro_fk");

            entity.HasOne(d => d.EncVenta).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.Idventa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DetalleVentas_EncabezadoVentas_idventa_fk");
        });

        modelBuilder.Entity<EncabezadoVenta>(entity =>
        {
            entity.HasKey(e => e.Idventa).HasName("EncabezadoVentas_pk");

            entity.Property(e => e.Idventa).HasColumnName("idventa");
            entity.Property(e => e.Fecha)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.Idvendedor).HasColumnName("idvendedor");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("total");

            // RELACIÓN CORRECTA:
            // EncabezadoVentas.idvendedor (FK) -> Usuarios.idus (PK)
            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.EncabezadoVentas)
                .HasForeignKey(e => e.Idvendedor)   // <--- IMPORTANTE
                .HasPrincipalKey(u => u.idus)       // PK de Usuario
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EncabezadoVentas_Usuarios_idus_fk");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.idpro).HasName("Productos_pk");

            entity.Property(e => e.idpro).HasColumnName("idpro");
            entity.Property(e => e.precio)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.producto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("producto");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.idus).HasName("Usuarios_pk");

            entity.Property(e => e.idus).HasColumnName("idus");
            entity.Property(e => e.clavealgoritmo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("clavealgoritmo");
            entity.Property(e => e.clavehash)
                .HasMaxLength(256)
                .HasColumnName("clavehash");
            entity.Property(e => e.claveiteraciones).HasColumnName("claveiteraciones");
            entity.Property(e => e.clavesalt)
                .HasMaxLength(256)
                .HasColumnName("clavesalt");
            entity.Property(e => e.nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.usuario)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("usuario");
        });

        // modelBuilder.Entity<Vendedor>(entity =>
        // {
        //     entity.HasKey(e => e.Idvendedor).HasName("Vendedores_pk");
        //
        //     entity.Property(e => e.Idvendedor).HasColumnName("idvendedor");
        //     entity.Property(e => e.Nombre)
        //         .HasMaxLength(100)
        //         .IsUnicode(false)
        //         .HasColumnName("nombre");
        // });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
