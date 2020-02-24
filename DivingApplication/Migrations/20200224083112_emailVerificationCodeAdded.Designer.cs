﻿// <auto-generated />
using System;
using DivingApplication.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DivingApplication.Migrations
{
    [DbContext(typeof(DivingAPIContext))]
    [Migration("20200224083112_emailVerificationCodeAdded")]
    partial class emailVerificationCodeAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DivingApplication.Entities.ChatRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ChatRooms");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ChatRoom");
                });

            modelBuilder.Entity("DivingApplication.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BelongPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumOfLiks")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BelongPostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserChatRoom", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "ChatRoomId");

                    b.HasIndex("ChatRoomId");

                    b.ToTable("UserChatRoom");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserFollow", b =>
                {
                    b.Property<Guid>("FollowerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FollowerId", "FollowingId");

                    b.HasIndex("FollowingId");

                    b.ToTable("UserFollow");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserMessageLike", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("UserMessageLike");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserMessageRead", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("UserMessageRead");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserPostLike", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("UserPostLike");
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserPostSave", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("UserPostSave");
                });

            modelBuilder.Entity("DivingApplication.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChatRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("MessageType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ChatRoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DivingApplication.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContentURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(2048)")
                        .HasMaxLength(2048);

                    b.Property<string>("PostContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DivingApplication.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailVerificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("ProfileImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("UserGender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserRole")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DivingApplication.Entities.GroupChatRoom", b =>
                {
                    b.HasBaseType("DivingApplication.Entities.ChatRoom");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("GroupPhoto")
                        .HasColumnType("varbinary(max)");

                    b.HasDiscriminator().HasValue("GroupChatRoom");
                });

            modelBuilder.Entity("DivingApplication.Entities.Comment", b =>
                {
                    b.HasOne("DivingApplication.Entities.Post", "BelongPost")
                        .WithMany("Comments")
                        .HasForeignKey("BelongPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserChatRoom", b =>
                {
                    b.HasOne("DivingApplication.Entities.ChatRoom", "ChatRoom")
                        .WithMany("UserChatRooms")
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "User")
                        .WithMany("UserChatRooms")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserFollow", b =>
                {
                    b.HasOne("DivingApplication.Entities.User", "Follower")
                        .WithMany("Following")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "Following")
                        .WithMany("Followers")
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserMessageLike", b =>
                {
                    b.HasOne("DivingApplication.Entities.Message", "Message")
                        .WithMany("LikedBy")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "User")
                        .WithMany("LikeMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserMessageRead", b =>
                {
                    b.HasOne("DivingApplication.Entities.Message", "Message")
                        .WithMany("ReadBy")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "User")
                        .WithMany("ReadMessages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserPostLike", b =>
                {
                    b.HasOne("DivingApplication.Entities.Post", "Post")
                        .WithMany("PostLikedBy")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "User")
                        .WithMany("LikePosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.ManyToManyEntities.UserPostSave", b =>
                {
                    b.HasOne("DivingApplication.Entities.Post", "Post")
                        .WithMany("PostSavedBy")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.User", "User")
                        .WithMany("SavePosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DivingApplication.Entities.Message", b =>
                {
                    b.HasOne("DivingApplication.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DivingApplication.Entities.ChatRoom", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChatRoomId");
                });

            modelBuilder.Entity("DivingApplication.Entities.Post", b =>
                {
                    b.HasOne("DivingApplication.Entities.User", "Author")
                        .WithMany("OwningPosts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}