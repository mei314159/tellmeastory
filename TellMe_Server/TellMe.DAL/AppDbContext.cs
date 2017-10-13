using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Friendship>().HasOne(x => x.User).WithMany(x => x.Friends).HasForeignKey(x => x.UserId);
            builder.Entity<Friendship>().HasOne(x => x.Friend).WithMany().HasForeignKey(x => x.FriendId);
            builder.Entity<ApplicationUser>().HasMany(x => x.PushNotificationClients).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.Entity<Story>().HasOne(x => x.Sender).WithMany(x => x.SentStories).HasForeignKey(x => x.SenderId);
            builder.Entity<Story>().HasOne(x => x.Receiver).WithMany(x => x.ReceivedStories).HasForeignKey(x => x.ReceiverId);
            builder.Entity<Notification>().HasOne(x => x.Recipient).WithMany().HasForeignKey(x => x.RecipientId);
            builder.Entity<Notification>().Property(x => x._extra).HasColumnName("Extra");
            // builder.Entity<ApplicationUser>().Property(x => x.PhoneCountryCode).HasDefaultValue(1);
            // builder.Entity<ApplicationUser>().HasMany(x => x.Contacts).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            base.OnModelCreating(builder);
        }
    }
}