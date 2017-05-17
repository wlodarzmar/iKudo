using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using iKudo.Domain.Model;

namespace iKudo.Domain.Migrations
{
    [DbContext(typeof(KudoDbContext))]
    [Migration("20170429115516_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BoardId");

                    b.Property<string>("CandidateId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("DecisionDate");

                    b.Property<string>("DecisionUserId");

                    b.Property<bool>("IsAccepted");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("JoinRequests");
                });

            modelBuilder.Entity("iKudo.Domain.Model.JoinRequest", b =>
                {
                    b.HasOne("iKudo.Domain.Model.Board", "Board")
                        .WithMany("JoinRequests")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
