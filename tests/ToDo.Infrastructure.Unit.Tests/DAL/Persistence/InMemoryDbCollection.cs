namespace ToDo.Infrastructure.Unit.Tests.DAL.Persistence;

[CollectionDefinition(nameof(InMemoryDbCollection))]
public class InMemoryDbCollection : ICollectionFixture<InMemoryDbContextFixture>;