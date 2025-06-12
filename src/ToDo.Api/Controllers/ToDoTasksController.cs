using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDo.Application.Abstractions;
using ToDo.Application.Commands;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Application.Queries;
using ToDo.Shared.Errors;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("to-do-tasks")]
public class ToDoTasksController(
    IQueryHandler<GetToDoTask, ToDoTaskDto> getToDoTaskHandler,
    IQueryHandler<GetToDoTasks, PagedResult<ToDoTaskDto>> getToDoTasksHandler,
    IQueryHandler<GetIncomingToDoTasks, IEnumerable<ToDoTaskDto>> getIncomingToDoTasksHandler,
    ICommandHandler<CreateToDoTask> createToDoTaskHandler,
    ICommandHandler<UpdateToDoTask> updateToDoTaskHandler,
    ICommandHandler<SetToDoTaskPercentComplete> setToDoTaskPercentCompleteHandler,
    ICommandHandler<DeleteToDoTask> deleteToDoTaskHandler,
    ICommandHandler<MarkToDoTaskAsDone> markToDoTaskAsDoneHandler)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation( 
        Summary = "Retrieves a list of to-do tasks",
        Description = "Fetches all to-do tasks based on the provided query parameters such as filtering, sorting, or pagination."
        )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ToDoTaskDto>>> Get([FromQuery] GetToDoTasks command)
    {
        var toDoTasks = await getToDoTasksHandler.HandleAsync(command);

        return Ok(toDoTasks);
    }

    [HttpGet("{toDoTaskId:guid}")]
    [SwaggerOperation(
        Summary = "Retrieves a specific to-do task",
        Description = "Fetches details of a to-do task by its unique identifier."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ToDoTaskDto>> Get(Guid toDoTaskId)
    {
        var toDoTask = await getToDoTaskHandler.HandleAsync(new GetToDoTask(ToDoTaskId: toDoTaskId));

        return Ok(toDoTask);
    }

    [HttpGet("incoming")]
    [SwaggerOperation(
        Summary = "Retrieves incoming to-do tasks",
        Description = "Returns to-do tasks scheduled for Today, Tomorrow, or the Current Week based on the provided 'incomingFilter' query parameter."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ToDoTaskDto>>> Get([FromQuery] IncomingFilter incomingFilter)
    {
        var toDoTasks = await getIncomingToDoTasksHandler
            .HandleAsync(new GetIncomingToDoTasks(incomingFilter));

        return Ok(toDoTasks);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a new to-do task",
        Description = "Submits a new to-do task and returns a reference to the created resource."
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post(CreateToDoTask command)
    {
        var toDoTaskId = Guid.NewGuid();

        await createToDoTaskHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return CreatedAtAction(nameof(Get), new { toDoTaskId }, null);
    }

    [HttpPut("{toDoTaskId:guid}")]
    [SwaggerOperation(
        Summary = "Updates an existing to-do task",
        Description = "Updates all editable fields of an existing to-do task identified by its ID."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put(Guid toDoTaskId, UpdateToDoTask command)
    {
        await updateToDoTaskHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return Ok();
    }

    [HttpPatch("{toDoTaskId:guid}/percent-complete")]
    [SwaggerOperation(
        Summary = "Updates percent complete of a to-do task",
        Description = "Partially updates the specified to-do task's percent completion field."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Patch(Guid toDoTaskId, SetToDoTaskPercentComplete command)
    {
        await setToDoTaskPercentCompleteHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return Ok();
    }

    [HttpDelete("{toDoTaskId:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a to-do task",
        Description = "Removes a to-do task permanently based on its ID."
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid toDoTaskId)
    {
        await deleteToDoTaskHandler.HandleAsync(new DeleteToDoTask(ToDoTaskId: toDoTaskId));

        return NoContent();
    }

    [HttpPatch("{toDoTaskId:guid}/done")]
    [SwaggerOperation(
        Summary = "Marks a to-do task as done",
        Description = "Updates the task status to indicate it has been completed."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Patch(Guid toDoTaskId)
    {
        await markToDoTaskAsDoneHandler.HandleAsync(new MarkToDoTaskAsDone(ToDoTaskId: toDoTaskId));

        return Ok();
    }
}