using AltVStrefaRPServer.Models.Businesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class BusinessRankMap : IEntityTypeConfiguration<BusinessRank>
    {
        public void Configure(EntityTypeBuilder<BusinessRank> builder)
        {
            builder.HasOne(b => b.Permissions)
                .WithOne(r=> r.BusinessRank)
                .HasForeignKey<BusinessPermissions>(b => b.BusinessRankForeignKey);
        }
    }
}
