﻿using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Message;
using static DivingApplication.Entities.Post;
using static DivingApplication.Entities.ServiceInfo;
using static DivingApplication.Entities.User;

namespace DivingApplication.DbContexts
{
    public class DivingAPIContext : DbContext
    {
        public DivingAPIContext(DbContextOptions<DivingAPIContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<GroupChatRoom> GroupChatRooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<CoachInfo> CoachInfos { get; set; }
        public DbSet<ServiceInfo> ServiceInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dealing with the Enum Conversion

            modelBuilder.Entity<User>().Property(u => u.UserRole).HasConversion(
                        v => v.ToString(),
                        v => ((Role)Enum.Parse(typeof(Role), v))
                   );

            modelBuilder.Entity<User>().Property(u => u.UserGender).HasConversion(
                        v => v.ToString(),
                        v => ((Gender)Enum.Parse(typeof(Gender), v))
                       );

            modelBuilder.Entity<Post>().Property(p => p.PostContentType).HasConversion(
                        v => v.ToString(),
                        v => ((ContentType)Enum.Parse(typeof(ContentType), v))
                     );

            modelBuilder.Entity<Message>().Property(c => c.MessageType).HasConversion(
                       v => v.ToString(),
                       v => ((MessageContentType)Enum.Parse(typeof(MessageContentType), v))
                     );


            modelBuilder.Entity<ServiceInfo>().Property(s => s.ServiceCenterType).HasConversion(
                    v => v.ToString(),
                    v => ((CenterType)Enum.Parse(typeof(CenterType), v))
                );

            // Dealing with Many-to-Many Relationship


            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });


            modelBuilder.Entity<UserFollow>()
                        .HasOne(uf => uf.Follower)
                        .WithMany(u => u.Following)
                        .HasForeignKey(uf => uf.FollowerId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                        .HasOne(uf => uf.Following)
                        .WithMany(u => u.Followers)
                        .HasForeignKey(uf => uf.FollowingId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserChatRoom>()
                .HasKey(uc => new { uc.UserId, uc.ChatRoomId });


            modelBuilder.Entity<UserChatRoom>()
                        .HasOne(uc => uc.ChatRoom)
                        .WithMany(u => u.UserChatRooms)
                        .HasForeignKey(uc => uc.ChatRoomId);

            modelBuilder.Entity<UserChatRoom>()
                        .HasOne(uc => uc.User)
                        .WithMany(u => u.UserChatRooms)
                        .HasForeignKey(uc => uc.UserId);


            modelBuilder.Entity<UserPostLike>()
                        .HasKey(upl => new { upl.UserId, upl.PostId });


            modelBuilder.Entity<UserPostLike>()
                        .HasOne(upl => upl.Post)
                        .WithMany(p => p.PostLikedBy)
                        .HasForeignKey(upl => upl.PostId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPostLike>()
                        .HasOne(upl => upl.User)
                        .WithMany(u => u.LikePosts)
                        .HasForeignKey(upl => upl.UserId).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<UserPostSave>()
            .HasKey(ups => new { ups.UserId, ups.PostId });


            modelBuilder.Entity<UserPostSave>()
                        .HasOne(ups => ups.Post)
                        .WithMany(p => p.PostSavedBy)
                        .HasForeignKey(ups => ups.PostId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPostSave>()
                        .HasOne(ups => ups.User)
                        .WithMany(u => u.SavePosts)
                        .HasForeignKey(ups => ups.UserId).OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<UserMessageLike>()
            .HasKey(uml => new { uml.UserId, uml.MessageId });


            modelBuilder.Entity<UserMessageLike>()
                        .HasOne(uml => uml.Message)
                        .WithMany(m => m.LikedBy)
                        .HasForeignKey(uml => uml.MessageId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMessageLike>()
                        .HasOne(uml => uml.User)
                        .WithMany(u => u.LikeMessages)
                        .HasForeignKey(uml => uml.UserId).OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<UserMessageRead>()
            .HasKey(uml => new { uml.UserId, uml.MessageId });


            modelBuilder.Entity<UserMessageRead>()
                        .HasOne(uml => uml.Message)
                        .WithMany(m => m.ReadBy)
                        .HasForeignKey(uml => uml.MessageId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMessageRead>()
                        .HasOne(uml => uml.User)
                        .WithMany(u => u.ReadMessages)
                        .HasForeignKey(uml => uml.UserId).OnDelete(DeleteBehavior.Restrict);


            // If the user is deleted, then the comments and posts will not be affected
            modelBuilder.Entity<User>().HasMany(u => u.OwningPosts).WithOne(p => p.Author).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasMany(u => u.OwingComments).WithOne(c => c.Author).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Post>().HasMany(p => p.Comments).WithOne(c => c.BelongPost).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

        }

    }
}
