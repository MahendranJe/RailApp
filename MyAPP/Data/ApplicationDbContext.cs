using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAPP.Models;

namespace MyAPP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrainUpdate> TrainUpdates { get; set; }
        public DbSet<TrainScheduleDay> TrainScheduleDays { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser configuration - Unique indexes
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.MobileNumber).IsUnique().HasFilter("[MobileNumber] IS NOT NULL");
            });

            // TrainUpdate configuration
            modelBuilder.Entity<TrainUpdate>(entity =>
            {
                entity.HasOne(t => t.CreatedBy)
                      .WithMany()
                      .HasForeignKey(t => t.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.ScheduleType)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });

            // TrainScheduleDay configuration
            modelBuilder.Entity<TrainScheduleDay>(entity =>
            {
                entity.HasOne(d => d.TrainUpdate)
                      .WithMany(t => t.ScheduleDays)
                      .HasForeignKey(d => d.TrainUpdateId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => d.TrainUpdateId);
            });

            // Alert configuration
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Alerts)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false); // UserId is optional for broadcast alerts

                entity.HasOne(a => a.CreatedBy)
                      .WithMany()
                      .HasForeignKey(a => a.CreatedByUserId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .IsRequired(false);
            });

            // Subscription configuration
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Subscriptions)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Payments)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Subscription)
                      .WithMany()
                      .HasForeignKey(p => p.SubscriptionId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
