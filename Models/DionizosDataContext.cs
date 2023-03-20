using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dionizos_backend_app.Models;

public partial class DionizosDataContext : DbContext
{
    public DionizosDataContext()
    {
    }

    public DionizosDataContext(DbContextOptions<DionizosDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Emailcode> Emailcodes { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Eventincategory> Eventincategories { get; set; }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Reservaton> Reservatons { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(System.Environment.GetEnvironmentVariable("POSTGRES_CONNSTRING_TSC"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pk");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Emailcode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("emailcodes_pk");

            entity.ToTable("emailcodes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Emailcodes)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("emailcodes_organizers");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id");

            entity.ToTable("events");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categories).HasColumnName("categories");
            entity.Property(e => e.Endtime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endtime");
            entity.Property(e => e.Latitude)
                .HasMaxLength(20)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(20)
                .HasColumnName("longitude");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Owner).HasColumnName("owner");
            entity.Property(e => e.Placecapacity).HasColumnName("placecapacity");
            entity.Property(e => e.Placeschema).HasColumnName("placeschema");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(250)
                .HasColumnName("title");

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Owner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_organizers");
        });

        modelBuilder.Entity<Eventincategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventincategories_pk");

            entity.ToTable("eventincategories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");

            entity.HasOne(d => d.Categories).WithMany(p => p.Eventincategories)
                .HasForeignKey(d => d.CategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("eventincategories_categories");

            entity.HasOne(d => d.Event).WithMany(p => p.Eventincategories)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("eventincategories_event");
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organizers_pk");

            entity.ToTable("organizers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(320)
                .HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Reservaton>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.PlaceId }).HasName("reservatons_pk");

            entity.ToTable("reservatons");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.Token).HasColumnName("token");

            entity.HasOne(d => d.Event).WithMany(p => p.Reservatons)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservatons_events");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessions_pk");

            entity.ToTable("sessions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sessions_organizers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
