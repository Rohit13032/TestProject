using Microsoft.EntityFrameworkCore;
using UserRegistrationApp.Models;

namespace UserRegistrationApp.Data
{
    public partial class UserRegistrationDBContext : DbContext
    {
        public UserRegistrationDBContext(DbContextOptions<UserRegistrationDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(25);
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Mobile).HasMaxLength(15);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Hobbies).HasMaxLength(200);
                entity.Property(e => e.TermsAgreed).IsRequired();

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_States");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Cities");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.StateId);
                entity.Property(e => e.StateName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityId);
                entity.Property(e => e.CityName).IsRequired().HasMaxLength(100);
                entity.HasOne(d => d.State)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
} 