
namespace ToDo.Core.ValueObjects;

public sealed record DateAndTime
{
    public DateTimeOffset Value { get; }

    public DateAndTime(DateTimeOffset value)
    {
        Value = value.ToUniversalTime();
    }
    
    public DateAndTime AddDays(int days) => new(Value.AddDays(days));

    public static implicit operator DateTimeOffset(DateAndTime dateAndTime) => dateAndTime.Value;

    public static implicit operator DateAndTime(DateTimeOffset date) => new(date);

    public static bool operator >(DateAndTime date1, DateAndTime date2)
        => date1.Value > date2.Value;

    public static bool operator <(DateAndTime date1, DateAndTime date2)
        => date1.Value < date2.Value;

    public static bool operator >=(DateAndTime date1, DateAndTime date2)
        => date1.Value >= date2.Value;

    public static bool operator <=(DateAndTime date1, DateAndTime date2)
        => date1.Value <= date2.Value;
}