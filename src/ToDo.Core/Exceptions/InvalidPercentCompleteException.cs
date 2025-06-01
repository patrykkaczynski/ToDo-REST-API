namespace ToDo.Core.Exceptions;

public sealed class InvalidPercentCompleteException(int percentComplete) : CustomException(
    $"The provided percent complete value '{percentComplete}' is invalid. It must be between 0 and 100 (inclusive).")
{
    public int PercentComplete { get; } = percentComplete;
}