﻿using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
    
namespace AltVStrefaRPServer.Database.Map
{
    public class CharacterMap : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.Ignore(c => c.Player);

            builder.Property(c => c.Gender)
                .HasConversion<int>();
            
            builder.HasOne(c => c.BankAccount)
                .WithOne(b => b.Character)
                .HasForeignKey<BankAccount>(b => b.CharacterId);

            builder.HasOne<Business>(c => c.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(c => c.CurrentBusinessId);
            //.OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Fraction)
                .WithMany(f => f.Employees)
                .HasForeignKey(c => c.CurrentFractionId);
            //.OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Inventory)
                .WithOne(i => i.Owner)
                .HasForeignKey<Character>(i => i.InventoryId);
            //.OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Equipment)
                .WithOne(e => e.Owner)
                .HasForeignKey<Character>(c => c.EquipmentId);

            builder.HasMany(c => c.Houses)
                .WithOne(f => f.Owner)
                .HasForeignKey(h => h.OwnerId)
                .IsRequired(false);

            builder.HasMany(c => c.HotelRooms)
                .WithOne(hR => hR.Owner)
                .HasForeignKey(hR => hR.OwnerId);
        }
    }
}
