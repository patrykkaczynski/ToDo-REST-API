using ToDo.Core.Exceptions;

namespace ToDo.Application.Exceptions;

public sealed class ToDoTaskNotFoundException : CustomException
{
    public Guid Id { get; }
    public ToDoTaskNotFoundException(Guid id) 
        : base($"To do task with ID: {id} was not found.")
    {
        Id = id;
    }
}