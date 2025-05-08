namespace ToDo.Core.Exceptions;

public sealed class EmptyTitleException : CustomException
{
    public EmptyTitleException()
        : base("The title cannot be empty or whitespace.")
    {
    }
}