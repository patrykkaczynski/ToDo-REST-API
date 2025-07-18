using ToDo.Infrastructure.DAL.Persistence;
using ToDo.Infrastructure.Unit.Tests.DAL.Persistence;
using ToDo.Infrastructure.Unit.Tests.Time;

namespace ToDo.Infrastructure.Unit.Tests.DAL.Base;

[Collection(nameof(InMemoryDbCollection))]
public class TestBase
{
    private protected readonly ToDoDbContext DbContext;
    private protected readonly DateTime Now;

    protected TestBase(InMemoryDbContextFixture fixture)
    {
        DbContext = fixture.DbContext;
        Now =  new TestDateTimeProvider().Current();
    }

    public TestBase()
    {
    }
}