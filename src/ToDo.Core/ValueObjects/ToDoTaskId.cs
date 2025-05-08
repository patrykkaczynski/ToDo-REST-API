using ToDo.Core.Exceptions;

namespace ToDo.Core.ValueObjects;

public sealed record ToDoTaskId
{
    public Guid Value { get; }

    public ToDoTaskId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidEntityIdException(value);
        }

        Value = value;
    }
    
    public static implicit operator Guid(ToDoTaskId id) => id.Value;

    public static implicit operator ToDoTaskId(Guid value) => new(value);
}