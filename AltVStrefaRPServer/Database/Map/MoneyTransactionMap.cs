using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class MoneyTransactionMap : IEntityTypeConfiguration<MoneyTransaction>
    {
        public void Configure(EntityTypeBuilder<MoneyTransaction> builder)
        {
            builder.Property(m => m.Type)
                .HasConversion<int>();
        }
    }
}
