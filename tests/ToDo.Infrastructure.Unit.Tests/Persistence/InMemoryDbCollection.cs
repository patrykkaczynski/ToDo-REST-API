namespace ToDo.Infrastructure.Unit.Tests.Persistence;

[CollectionDefinition(nameof(InMemoryDbCollection))]
public class InMemoryDbCollection : ICollectionFixture<InMemoryDbContextFixture>;