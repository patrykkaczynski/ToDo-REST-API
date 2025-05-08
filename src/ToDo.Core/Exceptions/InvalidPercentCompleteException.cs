namespace ToDo.Core.Exceptions;

public sealed class InvalidPercentCompleteException : CustomException
{
    public int PercentComplete { get; }

    public InvalidPercentCompleteException(int percentComplete)
        : base($"The provided percent complete value '{percentComplete}' is invalid. It must be between 0 and 100 (inclusive).")
        => PercentComplete = percentComplete;
}