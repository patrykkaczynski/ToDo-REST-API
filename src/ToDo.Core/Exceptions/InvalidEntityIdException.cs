namespace ToDo.Core.Exceptions;

public sealed class InvalidEntityIdException(object id)
    : CustomException($"The value '{id}' is not a valid entity identifier.")
{
    public object Id { get; } = id;
}