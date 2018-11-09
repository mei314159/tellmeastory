﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TellMe.Shared.Contracts.Enums;
using TellMe.Web.DAL;
using TellMe.Web.DAL.Types.Domain;
using TellMe.Web.DAL.Types.PushNotifications;

namespace TellMe.Web.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<int>("RepliesCount");

                    b.Property<int?>("ReplyToCommentId");

                    b.Property<int>("StoryId");

                    b.Property<string>("Text")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ReplyToCommentId");

                    b.HasIndex("StoryId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<DateTime>("DateUtc");

                    b.Property<string>("Description");

                    b.Property<string>("HostId")
                        .IsRequired();

                    b.Property<bool>("ShareStories");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("HostId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.EventAttendee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<int>("EventId");

                    b.Property<int>("Status");

                    b.Property<int?>("TribeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("TribeId");

                    b.HasIndex("UserId");

                    b.ToTable("EventAttendee");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Friendship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FriendId");

                    b.Property<int>("Status");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FriendId");

                    b.HasIndex("UserId");

                    b.ToTable("Friendship");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<bool>("Handled");

                    b.Property<string>("RecipientId");

                    b.Property<string>("Text");

                    b.Property<int>("Type");

                    b.Property<string>("_extra")
                        .HasColumnName("Extra");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.ObjectionableStory", b =>
                {
                    b.Property<int>("StoryId");

                    b.Property<string>("UserId");

                    b.Property<DateTime>("Date");

                    b.HasKey("StoryId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ObjectionableStory");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PlaylistStory", b =>
                {
                    b.Property<int>("PlaylistId");

                    b.Property<int>("StoryId");

                    b.Property<int>("Order");

                    b.HasKey("PlaylistId", "StoryId");

                    b.HasIndex("StoryId");

                    b.ToTable("PlaylistStory");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PlaylistUser", b =>
                {
                    b.Property<int>("PlaylistId");

                    b.Property<string>("UserId");

                    b.Property<int>("Type");

                    b.HasKey("PlaylistId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("PlaylistUser");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PushNotificationClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppVersion");

                    b.Property<int>("OsType");

                    b.Property<string>("Token");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PushNotificationClient");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId");

                    b.Property<bool>("Expired");

                    b.Property<string>("Token");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Story", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommentsCount");

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<int?>("EventId");

                    b.Property<int>("LikesCount");

                    b.Property<string>("PreviewUrl");

                    b.Property<int?>("RequestId");

                    b.Property<string>("SenderId");

                    b.Property<string>("Title");

                    b.Property<string>("VideoUrl");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("RequestId");

                    b.HasIndex("SenderId");

                    b.ToTable("Story");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryLike", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnName("UserId");

                    b.Property<int>("StoryId")
                        .HasColumnName("StoryId");

                    b.HasKey("UserId", "StoryId");

                    b.HasIndex("StoryId");

                    b.ToTable("StoryLike");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryReceiver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("StoryId");

                    b.Property<int?>("TribeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("StoryId");

                    b.HasIndex("TribeId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryReceiver");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<int?>("EventId");

                    b.Property<string>("SenderId");

                    b.Property<string>("Title");

                    b.Property<int?>("TribeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("SenderId");

                    b.HasIndex("TribeId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryRequest");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryRequestStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RequestId");

                    b.Property<int>("Status");

                    b.Property<int?>("TribeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.HasIndex("TribeId");

                    b.HasIndex("UserId");

                    b.ToTable("StoryRequestStatus");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Tribe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDateUtc");

                    b.Property<string>("CreatorId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Tribe");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.TribeMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Status");

                    b.Property<int>("TribeId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TribeId");

                    b.HasIndex("UserId");

                    b.ToTable("TribeMember");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Comment", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Comment", "ReplyToComment")
                        .WithMany()
                        .HasForeignKey("ReplyToCommentId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Story", "Story")
                        .WithMany("Comments")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Event", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Host")
                        .WithMany("HostedEvents")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.EventAttendee", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Event", "Event")
                        .WithMany("Attendees")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Tribe", "Tribe")
                        .WithMany("Events")
                        .HasForeignKey("TribeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Friendship", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Friend")
                        .WithMany()
                        .HasForeignKey("FriendId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Notification", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.ObjectionableStory", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Story", "Story")
                        .WithMany("ObjectionableStories")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("ObjectionableStories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PlaylistStory", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Playlist", "Playlist")
                        .WithMany("Stories")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Story", "Story")
                        .WithMany("Playlists")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PlaylistUser", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Playlist", "Playlist")
                        .WithMany("Users")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("Playlists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.PushNotificationClient", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("PushNotificationClients")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Story", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.StoryRequest", "Request")
                        .WithMany("Stories")
                        .HasForeignKey("RequestId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Sender")
                        .WithMany("SentStories")
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryLike", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Story", "Story")
                        .WithMany("Likes")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("LikedStories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryReceiver", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Story", "Story")
                        .WithMany("Receivers")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Tribe", "Tribe")
                        .WithMany("Stories")
                        .HasForeignKey("TribeId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryRequest", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Event", "Event")
                        .WithMany("StoryRequests")
                        .HasForeignKey("EventId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Tribe", "Tribe")
                        .WithMany()
                        .HasForeignKey("TribeId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Receiver")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.StoryRequestStatus", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.StoryRequest", "Request")
                        .WithMany("Statuses")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.Tribe", "Tribe")
                        .WithMany()
                        .HasForeignKey("TribeId");

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.Tribe", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("TellMe.Web.DAL.Types.Domain.TribeMember", b =>
                {
                    b.HasOne("TellMe.Web.DAL.Types.Domain.Tribe", "Tribe")
                        .WithMany("Members")
                        .HasForeignKey("TribeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TellMe.Web.DAL.Types.Domain.ApplicationUser", "User")
                        .WithMany("Tribes")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
