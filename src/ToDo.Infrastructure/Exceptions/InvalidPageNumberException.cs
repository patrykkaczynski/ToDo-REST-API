using ToDo.Core.Exceptions;

namespace ToDo.Infrastructure.Exceptions;

public sealed class InvalidPageNumberException  : CustomException
{
    public InvalidPageNumberException(int pageNumber, int maxPageNumber)
        : base(pageNumber < 1
            ? $"The provided page number '{pageNumber}' is invalid. It must be greater than or equal to 1."
            : $"The provided page number '{pageNumber}' exceeds the maximum allowed page number of {maxPageNumber}.")
    {
    }
}