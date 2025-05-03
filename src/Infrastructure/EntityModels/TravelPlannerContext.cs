using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityModels;

public partial class TravelPlannerContext : DbContext
{
   

    public TravelPlannerContext(DbContextOptions<TravelPlannerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityCost> ActivityCosts { get; set; }

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<Attendee> Attendees { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<LogBook> LogBooks { get; set; }

    public virtual DbSet<MediaType> MediaTypes { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { return; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => new { e.TripId, e.ActivityId });

            entity.ToTable("Activity");

            entity.HasIndex(e => new { e.TripId, e.Sequence }, "UQ_Activity_Sequence").IsUnique();

            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.ActivityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ActivityID");
            entity.Property(e => e.ActivityTypeId).HasColumnName("ActivityTypeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.GoogleLink).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlannedCost)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ActivityType).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ActivityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_ActivityType");

            entity.HasOne(d => d.Trip).WithMany(p => p.Activities)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Trip");
        });

        modelBuilder.Entity<ActivityCost>(entity =>
        {
            entity.ToTable("ActivityCost");

            entity.Property(e => e.ActivityCostId).HasColumnName("ActivityCostID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.ActivityCosts)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityCost_Currency");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityCosts)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityCost_Activity");
        });

        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.ToTable("ActivityType");

            entity.HasIndex(e => e.Description, "UQ_ActivityType_Description").IsUnique();

            entity.HasIndex(e => e.Description, "UQ__Activity__4EBBBAC9B0F1C15F").IsUnique();

            entity.Property(e => e.ActivityTypeId).HasColumnName("ActivityTypeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.ToTable("Attendee");

            entity.Property(e => e.AttendeeId).HasColumnName("AttendeeID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Activity).WithMany(p => p.Attendees)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendee_Activity");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyCode);

            entity.ToTable("Currency");

            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(10, 4)");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LogBook>(entity =>
        {
            entity.ToTable("LogBook");

            entity.Property(e => e.LogBookId).HasColumnName("LogBookID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TripLogBookNavigation).WithMany(p => p.LogBooks)
                .HasForeignKey(d => d.TripLogBook)
                .HasConstraintName("FK_LogBook_Trip");

            entity.HasOne(d => d.Activity).WithMany(p => p.LogBooks)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .HasConstraintName("FK_LogBook_Activity");
        });

        modelBuilder.Entity<MediaType>(entity =>
        {
            entity.HasKey(e => e.MediaType1);

            entity.ToTable("MediaType");

            entity.HasIndex(e => e.Description, "UQ_MediaType_Description").IsUnique();

            entity.Property(e => e.MediaType1).HasColumnName("MediaType");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.HasIndex(e => e.FileGuid, "UQ_FileGUID").IsUnique();

            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.ActivityCostId).HasColumnName("ActivityCostID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.FileGuid).HasColumnName("FileGUID");
            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ActivityCost).WithMany(p => p.Media)
                .HasForeignKey(d => d.ActivityCostId)
                .HasConstraintName("FK_Media_ActivityCost");

            entity.HasOne(d => d.MediaTypeNavigation).WithMany(p => p.Media)
                .HasForeignKey(d => d.MediaType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_MediaType");

            entity.HasOne(d => d.Trip).WithMany(p => p.Media)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_Media_Trip");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("Trip");

            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.Budget).HasColumnType("money");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TripBackgroundGuid).HasColumnName("TripBackgroundGUID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Trips)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_CurrencyCode");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
