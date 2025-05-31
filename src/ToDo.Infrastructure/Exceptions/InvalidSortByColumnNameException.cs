using ToDo.Core.Exceptions;
using ToDo.Infrastructure.DAL.Options;

namespace ToDo.Infrastructure.Exceptions;

public sealed class InvalidSortByColumnNameException : CustomException
{
    public InvalidSortByColumnNameException(string message) : 
        base($"Sort by is optional or it must be one of the following: " +
             $"{string.Join(", ", PaginationOptions.AllowedSortByColumnNames)}.")
    {
    }
}