using ToDo.Core.Exceptions;

namespace ToDo.Core.ValueObjects;

public sealed record Description
{
    public string Value { get; }

    public Description(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EmptyDescriptionException();
        }

        if (value.Length > 500)
        {
            throw new InvalidDescriptionException(value);
        }
        
        Value = value;
    }
    
    public static implicit operator string(Description description) => description.Value;
    
    public static implicit operator Description(string value) => new(value);
}