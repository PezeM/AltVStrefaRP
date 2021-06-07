using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class FractionMap : IEntityTypeConfiguration<Fraction>
    {
        public void Configure(EntityTypeBuilder<Fraction> builder)
        {
            builder.Ignore(f => f.Blip)
                .Ignore(f => f.Invites);

            builder.HasMany<Character>(f => f.Employees)
                .WithOne(c => c.Fraction)
                .HasForeignKey(c => c.CurrentFractionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            var navigation = builder.Metadata.FindNavigation(nameof(Fraction.Employees));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            var fractionRankNavigation = builder.Metadata.FindNavigation(nameof(Fraction.FractionRanks));
            fractionRankNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(f => f.FractionRanks)
                .WithOne(fr => fr.Fraction)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
