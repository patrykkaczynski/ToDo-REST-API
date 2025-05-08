using ToDo.Core.Abstractions;

namespace ToDo.Infrastructure.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Current() => DateTime.UtcNow;
}