namespace ToDo.Core.Exceptions;

public sealed class InvalidExpirationDateException : CustomException
{
    public DateTimeOffset ExpirationDate { get; }

    public InvalidExpirationDateException(DateTimeOffset expirationDate)
        : base($"The provided expiration date '{expirationDate}' is invalid. It cannot be in the past or now.")
        => ExpirationDate = expirationDate;
}