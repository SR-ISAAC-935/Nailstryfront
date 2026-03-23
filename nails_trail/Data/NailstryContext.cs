using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using nails_trail.Models;

namespace nails_trail.Data;

public partial class NailstryContext : DbContext
{
    public NailstryContext()
    {
    }

    public NailstryContext(DbContextOptions<NailstryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DateAgendedResume> DateAgendedResumes { get; set; }

    public virtual DbSet<DateServiceDetail> DateServiceDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-G7PI26O;Initial Catalog=nailstry;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__categori__E548B673358177F7");

            entity.ToTable("categories");

            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<DateAgendedResume>(entity =>
        {
            entity.HasKey(e => e.IdDate).HasName("PK__DateAgen__95283FF4D003D9D1");

            entity.ToTable("DateAgendedResume");

            entity.HasIndex(e => e.Datereserved, "IX_DateAgendedResume_DateReserved");

            entity.HasIndex(e => e.Idissuer, "IX_DateAgendedResume_IdIssuer");

            entity.Property(e => e.IdDate).HasColumnName("id_date");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Datereserved)
                .HasPrecision(0)
                .HasColumnName("datereserved");
            entity.Property(e => e.Idissuer).HasColumnName("idissuer");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente")
                .HasColumnName("status");
            entity.Property(e => e.TotalDuration).HasColumnName("total_duration");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.IdissuerNavigation).WithMany(p => p.DateAgendedResumes)
                .HasForeignKey(d => d.Idissuer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DateAgend__idiss__787EE5A0");
        });

        modelBuilder.Entity<DateServiceDetail>(entity =>
        {
            entity.HasKey(e => e.IdDetail).HasName("PK__DateServ__EA833808AECB9593");

            entity.HasIndex(e => e.IdDate, "IX_DateServiceDetails_IdDate");

            entity.HasIndex(e => e.IdService, "IX_DateServiceDetails_IdService");

            entity.Property(e => e.IdDetail).HasColumnName("id_detail");
            entity.Property(e => e.IdDate).HasColumnName("id_date");
            entity.Property(e => e.IdService).HasColumnName("id_service");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.ServiceDuration).HasColumnName("service_duration");
            entity.Property(e => e.ServicePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("service_price");

            entity.HasOne(d => d.IdDateNavigation).WithMany(p => p.DateServiceDetails)
                .HasForeignKey(d => d.IdDate)
                .HasConstraintName("FK__DateServi__id_da__02084FDA");

            entity.HasOne(d => d.IdServiceNavigation).WithMany(p => p.DateServiceDetails)
                .HasForeignKey(d => d.IdService)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DateServi__id_se__02FC7413");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__products__BA39E84F95374BD6");

            entity.ToTable("products");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.BuyedAt)
                .HasColumnType("money")
                .HasColumnName("buyed_at");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.ProductName)
                .HasMaxLength(75)
                .HasColumnName("product_name");
            entity.Property(e => e.SellsAt)
                .HasColumnType("money")
                .HasColumnName("sells_at");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__products__id_cat__628FA481");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.IdService).HasName("PK__Services__D06FB5A8E34F4CCF");

            entity.Property(e => e.IdService).HasColumnName("id_service");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .HasColumnName("service_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
