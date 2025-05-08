using ToDo.Core.Exceptions;
using ToDo.Infrastructure.DAL.Options;

namespace ToDo.Infrastructure.Exceptions;

public sealed class InvalidPageSizeException : CustomException
{
    public InvalidPageSizeException(int pageSize) 
        : base($"The provided page size '{pageSize}' is invalid. It must be one of the following: " +
               $"{string.Join(", ", PaginationOptions.AllowedPageSizes)}.")
    {
    }
}