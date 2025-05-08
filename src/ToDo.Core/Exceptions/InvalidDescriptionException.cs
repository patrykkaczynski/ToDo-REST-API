namespace ToDo.Core.Exceptions;

public sealed class InvalidDescriptionException : CustomException
{
    public string Description { get; }

    public InvalidDescriptionException(string description) 
        : base($"The provided description '{description}' is invalid. It must be 500 characters or fewer.")
        => Description = description;
}