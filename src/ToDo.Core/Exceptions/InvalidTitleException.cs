namespace ToDo.Core.Exceptions;

public sealed class InvalidTitleException(string title)
    : CustomException($"The provided title '{title}' is invalid. It must be between 3 and 50 characters in length.")
{
    public string Title { get; } = title;
}