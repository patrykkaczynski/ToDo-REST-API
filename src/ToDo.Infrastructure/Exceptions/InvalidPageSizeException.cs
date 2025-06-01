using ToDo.Core.Exceptions;
using ToDo.Infrastructure.DAL.Options;

namespace ToDo.Infrastructure.Exceptions;

public sealed class InvalidPageSizeException(int pageSize) : CustomException(
    $"The provided page size '{pageSize}' is invalid. It must be one of the following: " +
    $"{string.Join(", ", PaginationOptions.AllowedPageSizes)}.");