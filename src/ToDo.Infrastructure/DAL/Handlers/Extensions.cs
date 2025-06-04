using ToDo.Application.DTO;
using ToDo.Core.Entities;

namespace ToDo.Infrastructure.DAL.Handlers;

public static class Extensions
{
    public static ToDoTaskDto AsDto(this ToDoTask entity)
        => new()
        {
            ExpirationDate = entity.ExpirationDate.Value.DateTime,
            Description = entity.Description,
            Title = entity.Title,
            PercentComplete = entity.PercentComplete
        };
}