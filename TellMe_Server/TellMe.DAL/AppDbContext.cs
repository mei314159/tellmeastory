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
            builder.Entity<ApplicationUser>().HasMany(x => x.PushNotificationClients).WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Entity<Story>().HasOne(x => x.Request).WithMany(x => x.Stories).HasForeignKey(x => x.RequestId);
            builder.Entity<Story>().HasOne(x => x.Sender).WithMany(x => x.SentStories).HasForeignKey(x => x.SenderId);

            builder.Entity<StoryLike>().HasOne(x => x.User).WithMany(x => x.LikedStories).HasForeignKey(x => x.UserId)
                .IsRequired();
            builder.Entity<StoryLike>().HasOne(x => x.Story).WithMany(x => x.Likes).HasForeignKey(x => x.StoryId)
                .IsRequired();
            builder.Entity<StoryLike>().HasKey(x => new {x.UserId, x.StoryId});

            builder.Entity<Comment>().HasOne(x => x.Story).WithMany(x => x.Comments).HasForeignKey(x => x.StoryId);
            builder.Entity<Comment>().HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
            builder.Entity<Comment>().HasOne(x => x.ReplyToComment).WithMany()
                .HasForeignKey(x => x.ReplyToCommentId)
                .IsRequired(false);

            builder.Entity<StoryReceiver>().HasOne(x => x.Story).WithMany(x => x.Receivers)
                .HasForeignKey(x => x.StoryId);
            builder.Entity<StoryReceiver>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            builder.Entity<StoryReceiver>().HasOne(x => x.Tribe).WithMany().HasForeignKey(x => x.TribeId);

            builder.Entity<StoryRequest>().HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId);
            builder.Entity<StoryRequest>().HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.UserId);
            builder.Entity<StoryRequest>().HasOne(x => x.Tribe).WithMany().HasForeignKey(x => x.TribeId);
            builder.Entity<StoryRequest>().HasMany(x => x.Stories).WithOne(x => x.Request)
                .HasForeignKey(x => x.RequestId);
            builder.Entity<StoryRequest>().HasMany(x => x.Statuses).WithOne(x => x.Request)
                .HasForeignKey(x => x.RequestId);

            builder.Entity<Tribe>().HasMany(x => x.Members).WithOne(x => x.Tribe).HasForeignKey(x => x.TribeId);
            builder.Entity<Tribe>().HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatorId);
            builder.Entity<TribeMember>().HasOne(x => x.User).WithMany(x => x.Tribes).HasForeignKey(x => x.UserId);

            builder.Entity<Notification>().HasOne(x => x.Recipient).WithMany().HasForeignKey(x => x.RecipientId);
            builder.Entity<Notification>().Property(x => x._extra).HasColumnName("Extra");
            // builder.Entity<ApplicationUser>().Property(x => x.PhoneCountryCode).HasDefaultValue(1);
            // builder.Entity<ApplicationUser>().HasMany(x => x.Contacts).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            base.OnModelCreating(builder);
        }
    }
}