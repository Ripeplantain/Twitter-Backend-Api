using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TwitterApi.Models;


namespace TwitterApi.Database
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<TwitterUser>(options)
    {
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

            modelBuilder.Entity<Retweet>()
                .HasOne(r => r.Retweeter)
                .WithMany(u => u.Retweets)
                .HasForeignKey(r => r.RetweeterId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Retweet>()
                .HasOne(r => r.Tweet)
                .WithMany(t => t.Retweets)
                .HasForeignKey(r => r.TweetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserChatroom>()
                .HasKey(uc => new { uc.UserId, uc.ChatroomId });
            
            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatrooms)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.Chatroom)
                .WithMany(c => c.UserChatrooms)
                .HasForeignKey(uc => uc.ChatroomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chatroom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatroomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Tweet> Tweets { get; set; } = null!;
        public override DbSet<TwitterUser> Users { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Retweet> Retweets { get; set; } = null!;
        public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
        public DbSet<UserChatroom> UserChatrooms { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
    }
}