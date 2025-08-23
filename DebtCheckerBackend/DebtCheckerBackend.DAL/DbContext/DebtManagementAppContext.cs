using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DebtCheckerBackend.Model;

public partial class DebtManagementAppContext : DbContext
{
    public DebtManagementAppContext()
    {
    }

    public DebtManagementAppContext(DbContextOptions<DebtManagementAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Debt> Debts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("debts_pkey");

            entity.ToTable("debts");

            entity.HasIndex(e => e.CreatedAt, "idx_debts_created_at");

            entity.HasIndex(e => e.DebtorId, "idx_debts_debtor_id");

            entity.HasIndex(e => e.DueDate, "idx_debts_due_date").HasFilter("(due_date IS NOT NULL)");

            entity.HasIndex(e => e.IsPaid, "idx_debts_is_paid");

            entity.HasIndex(e => e.UserId, "idx_debts_user_id");

            entity.HasIndex(e => new { e.UserId, e.IsPaid }, "idx_debts_user_paid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasDefaultValueSql("'COP'::character varying")
                .HasColumnName("currency");
            entity.Property(e => e.DebtorId).HasColumnName("debtor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.IsPaid)
                .HasDefaultValue(false)
                .HasColumnName("is_paid");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Debtor).WithMany(p => p.DebtDebtors)
                .HasForeignKey(d => d.DebtorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_debts_debtor");

            entity.HasOne(d => d.User).WithMany(p => p.DebtUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_debts_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.CreatedAt, "idx_users_created_at");

            entity.HasIndex(e => e.Email, "idx_users_email").IsUnique();

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
