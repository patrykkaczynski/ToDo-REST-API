namespace ToDo.Core.Exceptions;

public sealed class EmptyDescriptionException : CustomException
{
    public EmptyDescriptionException()
        : base("The description cannot be empty or whitespace.")
    {
    }
}