namespace ToDo.Core.Exceptions;

public sealed class InvalidEntityIdException : CustomException
{
    public object Id { get; }

    public InvalidEntityIdException(object id)
        : base($"The value '{id}' is not a valid entity identifier.")
        => Id = id;
}