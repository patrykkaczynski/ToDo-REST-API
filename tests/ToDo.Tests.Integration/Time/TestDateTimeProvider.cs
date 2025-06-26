using ToDo.Core.Abstractions;

namespace ToDo.Tests.Integration.Time;

public class TestDateTimeProvider : IDateTimeProvider
{
    public DateTime Current() => new DateTime(2025, 4, 28, 0, 0, 0, DateTimeKind.Utc);
}