namespace ToDo.Infrastructure.DAL.Options;

public class PaginationOptions
{
    public static int[] AllowedPageSizes { get; } = [5, 10, 25, 50];
}