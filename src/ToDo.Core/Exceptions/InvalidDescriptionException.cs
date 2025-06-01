namespace ToDo.Core.Exceptions;

public sealed class InvalidDescriptionException(string description)
    : CustomException($"The provided description '{description}' is invalid. It must be 500 characters or fewer.")
{
    public string Description { get; } = description;
}