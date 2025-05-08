namespace ToDo.Core.Exceptions;

public sealed class InvalidTitleException : CustomException
{
    public string Title { get; }

    public InvalidTitleException(string title)
        : base($"The provided title '{title}' is invalid. It must be between 3 and 50 characters in length.")
        => Title = title;
}