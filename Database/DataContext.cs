using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TwitterApi.Models;


namespace TwitterApi.Database
{
    public class DataContext : IdentityDbContext<TwitterUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TwitterUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<string>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

            modelBuilder.Entity<Tweet>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tweets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tweet>().ToTable("Tweets");

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Tweet)
                .WithMany(t => t.Likes)
                .HasForeignKey(l => l.TweetId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Tweet> Tweets { get; set; } = null!;
        public DbSet<TwitterUser> Users { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
    }
}