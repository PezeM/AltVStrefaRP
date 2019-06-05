﻿// <auto-generated />
using System;
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AltVStrefaRPServer.Database.Migrations
{
    [DbContext(typeof(ServerContext))]
    [Migration("20190605173226_FractionNavigationProperty")]
    partial class FractionNavigationProperty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AltVStrefaRPServer.Models.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdminLevel")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("AccountId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountNumber");

                    b.Property<int>("CharacterId");

                    b.Property<float>("Money");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.Business", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("BlipColor");

                    b.Property<byte>("BlipModel");

                    b.Property<string>("BlipName");

                    b.Property<string>("BusinessName");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("MaxMembersCount");

                    b.Property<int>("MaxRanksCount");

                    b.Property<float>("Money");

                    b.Property<int>("OwnerId");

                    b.Property<int>("Transactions");

                    b.Property<int>("Type");

                    b.Property<float>("X");

                    b.Property<float>("Y");

                    b.Property<float>("Z");

                    b.HasKey("Id");

                    b.ToTable("Businesses");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Business");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.BusinessPermissions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessRankForeignKey");

                    b.Property<bool>("CanInviteNewMembers");

                    b.Property<bool>("CanManageEmployess");

                    b.Property<bool>("CanManageRanks");

                    b.Property<bool>("CanOpenBusinessInventory");

                    b.Property<bool>("CanOpenBusinessMenu");

                    b.Property<bool>("HaveBusinessKeys");

                    b.Property<bool>("HaveVehicleKeys");

                    b.HasKey("Id");

                    b.HasIndex("BusinessRankForeignKey")
                        .IsUnique();

                    b.ToTable("BusinessesPermissions");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.BusinessRank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BusinessId");

                    b.Property<bool>("IsDefaultRank");

                    b.Property<bool>("IsOwnerRank");

                    b.Property<string>("RankName");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("BusinessesRanks");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<int>("Age");

                    b.Property<string>("BackgroundImage");

                    b.Property<int>("BusinessRank");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int?>("CurrentBusinessId");

                    b.Property<int?>("CurrentFractionId");

                    b.Property<short>("Dimension");

                    b.Property<string>("FirstName");

                    b.Property<int>("Gender");

                    b.Property<string>("LastName");

                    b.Property<DateTime>("LastPlayed");

                    b.Property<float>("Money");

                    b.Property<string>("ProfileImage");

                    b.Property<int>("TimePlayed");

                    b.Property<float>("X");

                    b.Property<float>("Y");

                    b.Property<float>("Z");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CurrentBusinessId");

                    b.HasIndex("CurrentFractionId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Fractions.Fraction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("BlipColor");

                    b.Property<byte>("BlipModel");

                    b.Property<string>("BlipName");

                    b.Property<ushort>("BlipSprite");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<float>("Money");

                    b.Property<string>("Name");

                    b.Property<float>("X");

                    b.Property<float>("Y");

                    b.Property<float>("Z");

                    b.HasKey("Id");

                    b.ToTable("Fractions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Fraction");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.MoneyTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("Amount");

                    b.Property<string>("Date");

                    b.Property<string>("Receiver");

                    b.Property<string>("Source");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("MoneyTransactions");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.VehicleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<short>("Dimension");

                    b.Property<float>("Fuel");

                    b.Property<bool>("IsBlocked");

                    b.Property<bool>("IsLocked");

                    b.Property<bool>("IsSpawned");

                    b.Property<float>("MaxFuel");

                    b.Property<float>("MaxOil");

                    b.Property<float>("Mileage");

                    b.Property<string>("Model");

                    b.Property<float>("Oil");

                    b.Property<int>("Owner");

                    b.Property<int>("OwnerType");

                    b.Property<float>("Pitch");

                    b.Property<uint>("PlateNumber");

                    b.Property<string>("PlateText");

                    b.Property<float>("Roll");

                    b.Property<float>("X");

                    b.Property<float>("Y");

                    b.Property<float>("Yaw");

                    b.Property<float>("Z");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.MechanicBusiness", b =>
                {
                    b.HasBaseType("AltVStrefaRPServer.Models.Businesses.Business");

                    b.HasDiscriminator().HasValue("MechanicBusiness");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.RestaurantBusiness", b =>
                {
                    b.HasBaseType("AltVStrefaRPServer.Models.Businesses.Business");

                    b.HasDiscriminator().HasValue("RestaurantBusiness");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Fractions.PoliceFraction", b =>
                {
                    b.HasBaseType("AltVStrefaRPServer.Models.Fractions.Fraction");

                    b.HasDiscriminator().HasValue("PoliceFraction");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Fractions.SamsFraction", b =>
                {
                    b.HasBaseType("AltVStrefaRPServer.Models.Fractions.Fraction");

                    b.HasDiscriminator().HasValue("SamsFraction");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Fractions.TownHallFraction", b =>
                {
                    b.HasBaseType("AltVStrefaRPServer.Models.Fractions.Fraction");

                    b.Property<float>("GlobalTax");

                    b.Property<float>("GunTax");

                    b.Property<float>("ProperyTax");

                    b.Property<float>("VehicleTax");

                    b.HasDiscriminator().HasValue("TownHallFraction");
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.BankAccount", b =>
                {
                    b.HasOne("AltVStrefaRPServer.Models.Character", "Character")
                        .WithOne("BankAccount")
                        .HasForeignKey("AltVStrefaRPServer.Models.BankAccount", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.BusinessPermissions", b =>
                {
                    b.HasOne("AltVStrefaRPServer.Models.Businesses.BusinessRank", "BusinessRank")
                        .WithOne("Permissions")
                        .HasForeignKey("AltVStrefaRPServer.Models.Businesses.BusinessPermissions", "BusinessRankForeignKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Businesses.BusinessRank", b =>
                {
                    b.HasOne("AltVStrefaRPServer.Models.Businesses.Business", "Business")
                        .WithMany("BusinessRanks")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AltVStrefaRPServer.Models.Character", b =>
                {
                    b.HasOne("AltVStrefaRPServer.Models.Account", "Account")
                        .WithMany("Characters")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AltVStrefaRPServer.Models.Businesses.Business", "Business")
                        .WithMany("Employees")
                        .HasForeignKey("CurrentBusinessId");

                    b.HasOne("AltVStrefaRPServer.Models.Fractions.Fraction", "Fraction")
                        .WithMany("Employees")
                        .HasForeignKey("CurrentFractionId");
                });
#pragma warning restore 612, 618
        }
    }
}
