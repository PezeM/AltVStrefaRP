﻿// <auto-generated />
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AltVStrefaRPServer.Database.Migrations
{
    [DbContext(typeof(ServerContext))]
    [Migration("20190320150653_AccountRelations")]
    partial class AccountRelations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AltVTestResource.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LicenseHash");

                    b.Property<string>("Password");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("AltVTestResource.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<int>("Age");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<float>("Money");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("AltVTestResource.Models.Character", b =>
                {
                    b.HasOne("AltVTestResource.Models.Account", "Account")
                        .WithMany("Characters")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
