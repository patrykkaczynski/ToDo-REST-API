using ToDo.Core.Exceptions;

namespace ToDo.Core.ValueObjects;

public sealed record PercentComplete
{
    public int Value { get; }

    public PercentComplete(int value)
    {
        if(value is < 0 or > 100)
        {
            throw new InvalidPercentCompleteException(value);
        }
        
        Value = value;
    }
    
    public static implicit operator int(PercentComplete percentComplete) => percentComplete.Value;
    
    public static implicit operator PercentComplete(int value) => new(value);
}