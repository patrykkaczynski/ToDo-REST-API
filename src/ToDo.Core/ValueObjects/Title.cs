using ToDo.Core.Exceptions;

namespace ToDo.Core.ValueObjects;

public sealed record Title
{
    public string Value { get; }

    public Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EmptyTitleException();
        }

        if (value.Length is > 50 or < 3)
        {
            throw new InvalidTitleException(value);
        }

        Value = value;
    }

    public static implicit operator string(Title title) => title.Value;

    public static implicit operator Title(string value) => new(value);
}