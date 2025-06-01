using ToDo.Core.Exceptions;

namespace ToDo.Application.Exceptions;

public sealed class ToDoTaskNotFoundException(Guid id) : CustomException($"To do task with ID: {id} was not found.")
{
    public Guid Id { get; } = id;
}