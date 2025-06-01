namespace ToDo.Core.Exceptions;

public sealed class InvalidExpirationDateException(DateTimeOffset expirationDate)
    : CustomException($"The provided expiration date '{expirationDate}' is invalid. It cannot be in the past or now.")
{
    public DateTimeOffset ExpirationDate { get; } = expirationDate;
}