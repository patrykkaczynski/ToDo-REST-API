using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.Unit.Tests.Persistence;

namespace ToDo.Infrastructure.Unit.Tests.Base;

[Collection(nameof(InMemoryDbCollection))]
public class TestBase
{
    private protected readonly ToDoDbContext _dbContext;
    private protected readonly DateTime _now;

    public TestBase(InMemoryDbContextFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _now = InMemoryDbContextFixture.Now;
    }

    public TestBase()
    {
        
    }
}