using System;
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StrefaRPServer.UnitTests.Core
{
    public class MockServerContext : ServerContext
    {
        public MockServerContext() : base(new DbContextOptionsBuilder<ServerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(builder => builder.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options)
        { }

        public override void Dispose() {}
    }
}
