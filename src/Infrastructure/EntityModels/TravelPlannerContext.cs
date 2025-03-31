using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityModels;
public partial class TravelPlannerContext : DbContext
{
    public TravelPlannerContext()
    {
    }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Activity>(entity =>
        {
            _ = entity.HasKey(e => new { e.TripId, e.ActivityId });

            _ = entity.ToTable("Activity");

            _ = entity.HasIndex(e => new { e.TripId, e.Sequence }, "UQ_Activity_Sequence").IsUnique();

            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            _ = entity.Property(e => e.ActivityTypeId).HasColumnName("ActivityTypeID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Description).IsUnicode(false);
            _ = entity.Property(e => e.GoogleLink).IsUnicode(false);
            _ = entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.PlannedCost)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.ActivityType).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ActivityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_ActivityType");

            _ = entity.HasOne(d => d.Trip).WithMany(p => p.Activities)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Trip");
        });

        _ = modelBuilder.Entity<ActivityCost>(entity =>
        {
            _ = entity.ToTable("ActivityCost");

            _ = entity.Property(e => e.ActivityCostId).HasColumnName("ActivityCostID");
            _ = entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            _ = entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.Price).HasColumnType("money");
            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.ActivityCosts)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityCost_Currency");

            _ = entity.HasOne(d => d.Activity).WithMany(p => p.ActivityCosts)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityCost_Activity");
        });

        _ = modelBuilder.Entity<ActivityType>(entity =>
        {
            _ = entity.ToTable("ActivityType");

            _ = entity.HasIndex(e => e.Description, "UQ_ActivityType_Description").IsUnique();

            _ = entity.HasIndex(e => e.Description, "UQ__Activity__4EBBBAC98AA0B9A1").IsUnique();

            _ = entity.Property(e => e.ActivityTypeId).HasColumnName("ActivityTypeID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        _ = modelBuilder.Entity<Attendee>(entity =>
        {
            _ = entity.ToTable("Attendee");

            _ = entity.Property(e => e.AttendeeId).HasColumnName("AttendeeID");
            _ = entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.Activity).WithMany(p => p.Attendees)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendee_Activity");
        });

        _ = modelBuilder.Entity<Currency>(entity =>
        {
            _ = entity.HasKey(e => e.CurrencyCode);

            _ = entity.ToTable("Currency");

            _ = entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.ExchangeRate).HasColumnType("decimal(10, 4)");
            _ = entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .IsUnicode(false);
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        _ = modelBuilder.Entity<LogBook>(entity =>
        {
            _ = entity.ToTable("LogBook");

            _ = entity.Property(e => e.LogBookId).HasColumnName("LogBookID");
            _ = entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Description).IsUnicode(false);
            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.TripLogBookNavigation).WithMany(p => p.LogBooks)
                .HasForeignKey(d => d.TripLogBook)
                .HasConstraintName("FK_LogBook_Trip");

            _ = entity.HasOne(d => d.Activity).WithMany(p => p.LogBooks)
                .HasForeignKey(d => new { d.TripId, d.ActivityId })
                .HasConstraintName("FK_LogBook_Activity");
        });

        _ = modelBuilder.Entity<MediaType>(entity =>
        {
            _ = entity.HasKey(e => e.MediaType1);

            _ = entity.ToTable("MediaType");

            _ = entity.HasIndex(e => e.Description, "UQ_MediaType_Description").IsUnique();

            _ = entity.Property(e => e.MediaType1).HasColumnName("MediaType");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        _ = modelBuilder.Entity<Medium>(entity =>
        {
            _ = entity.HasKey(e => e.MediaId);

            _ = entity.Property(e => e.MediaId).HasColumnName("MediaID");
            _ = entity.Property(e => e.ActivityCostId).HasColumnName("ActivityCostID");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.Description).IsUnicode(false);
            _ = entity.Property(e => e.FilePath).IsUnicode(false);
            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.ActivityCost).WithMany(p => p.Media)
                .HasForeignKey(d => d.ActivityCostId)
                .HasConstraintName("FK_Media_ActivityCost");

            _ = entity.HasOne(d => d.MediaTypeNavigation).WithMany(p => p.Media)
                .HasForeignKey(d => d.MediaType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_MediaType");

            _ = entity.HasOne(d => d.Trip).WithMany(p => p.Media)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_Media_Trip");
        });

        _ = modelBuilder.Entity<Trip>(entity =>
        {
            _ = entity.ToTable("Trip");

            _ = entity.Property(e => e.TripId).HasColumnName("TripID");
            _ = entity.Property(e => e.Budget).HasColumnType("money");
            _ = entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            _ = entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            _ = entity.Property(e => e.Description).IsUnicode(false);
            _ = entity.Property(e => e.IsActive).HasDefaultValue(true);
            _ = entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            _ = entity.Property(e => e.TripBackgroundPath).IsUnicode(false);
            _ = entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            _ = entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Trips)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_CurrencyCode");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
