using ToDo.Core.Entities;

namespace ToDo.Infrastructure.DAL.Options;

public class PaginationOptions
{
    public static int[] AllowedPageSizes { get; } = [5, 10, 25, 50];
    public static string[] AllowedSortByColumnNames { get; } = [nameof(ToDoTask.ExpirationDate), nameof(ToDoTask.Title),
        nameof(ToDoTask.Description), nameof(ToDoTask.PercentComplete)];
}