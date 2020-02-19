using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Message;
using static DivingApplication.Entities.Post;
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


            // Dealing with Many-to-Many Relationship


            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });


            modelBuilder.Entity<UserFollow>()
                        .HasOne(u => u.Follower)
                        .WithMany(u => u.Following)
                        .HasForeignKey(uf => uf.FollowerId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                        .HasOne(u => u.Following)
                        .WithMany(u => u.Followers)
                        .HasForeignKey(uf => uf.FollowingId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserChatRoom>()
                .HasKey(uc => new { uc.UserId, uc.ChatRoomId });


            modelBuilder.Entity<UserChatRoom>()
                        .HasOne(u => u.ChatRoom)
                        .WithMany(u => u.UserChatRooms)
                        .HasForeignKey(uc => uc.ChatRoomId);

            modelBuilder.Entity<UserChatRoom>()
                        .HasOne(u => u.User)
                        .WithMany(u => u.UserChatRooms)
                        .HasForeignKey(uc => uc.UserId);


            base.OnModelCreating(modelBuilder);

        }

    }
}
