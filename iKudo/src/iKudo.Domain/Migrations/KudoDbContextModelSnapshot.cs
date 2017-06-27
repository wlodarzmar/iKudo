using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using iKudo.Domain.Model;
using iKudo.Domain.Enums;

namespace iKudo.Domain.Migrations
{
    [DbContext(typeof(KudoDbContext))]
    partial class KudoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("iKudo.Domain.Model.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("CreatorId")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<DateTime?>("ModificationDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("iKudo.Domain.Model.JoinRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BoardId");

                    b.Property<string>("CandidateId")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("DecisionDate");

                    b.Property<string>("DecisionUserId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("JoinRequests");
                });

            modelBuilder.Entity("iKudo.Domain.Model.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BoardId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("ReadDate");

                    b.Property<string>("ReceiverId")
                        .IsRequired();

                    b.Property<string>("SenderId")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("iKudo.Domain.Model.UserBoard", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("BoardId");

                    b.HasKey("UserId", "BoardId");

                    b.HasIndex("BoardId");

                    b.ToTable("UserBoards");
                });

            modelBuilder.Entity("iKudo.Domain.Model.JoinRequest", b =>
                {
                    b.HasOne("iKudo.Domain.Model.Board", "Board")
                        .WithMany("JoinRequests")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("iKudo.Domain.Model.Notification", b =>
                {
                    b.HasOne("iKudo.Domain.Model.Board", "Board")
                        .WithMany()
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("iKudo.Domain.Model.UserBoard", b =>
                {
                    b.HasOne("iKudo.Domain.Model.Board")
                        .WithMany("UserBoards")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
