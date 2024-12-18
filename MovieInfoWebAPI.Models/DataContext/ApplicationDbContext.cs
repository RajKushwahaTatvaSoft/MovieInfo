using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Models.CustomTableModels;
using MovieInfoWebAPI.Models.DataModels;

namespace MovieInfoWebAPI.Models.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cast> Casts { get; set; }

    public virtual DbSet<Crew> Crews { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<MovieDetail> MovieDetails { get; set; }

    public virtual DbSet<MovieGenre> MovieGenres { get; set; }

    public virtual DbSet<MovieGenreMapping> MovieGenreMappings { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRating> UserRatings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=Raj@TatvaSoft;Server=localhost;Port=5432;Database=MovieDB;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cast>(entity =>
        {
            entity.HasKey(e => e.CastId).HasName("Cast_pkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.Casts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cast_MovieId_fkey");

            entity.HasOne(d => d.Person).WithMany(p => p.Casts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cast_PersonId_fkey");
        });

        modelBuilder.Entity<Crew>(entity =>
        {
            entity.HasKey(e => e.CrewId).HasName("Crew_pkey");

            entity.HasOne(d => d.Job).WithMany(p => p.Crews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Crew_JobId_fkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.Crews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Crew_MovieId_fkey");

            entity.HasOne(d => d.Person).WithMany(p => p.Crews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Crew_PersonId_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("Department_pkey");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("Job_pkey");

            entity.HasOne(d => d.Department).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Job_DepartmentId_fkey");
        });

        modelBuilder.Entity<MovieDetail>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("MovieDetail_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("MovieGenre_pkey");

            entity.Property(e => e.GenreId).HasDefaultValueSql("nextval('\"MovieGenre_Genre_seq\"'::regclass)");
        });

        modelBuilder.Entity<MovieGenreMapping>(entity =>
        {
            entity.HasKey(e => e.SerialId).HasName("MovieGenreMapping_pkey");

            entity.HasOne(d => d.Genre).WithMany(p => p.MovieGenreMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MovieGenreMapping_GenreId_fkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieGenreMappings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MovieGenreMapping_MovieId_fkey");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("Person_pkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_RoleId_fkey");
        });

        modelBuilder.Entity<UserRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("UserRating_pkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.UserRatings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserRating_MovieId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserRatings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserRating_UserId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
