using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Abstractions;
using ToDo.Application.Commands;
using ToDo.Application.Common;
using ToDo.Application.DTO;
using ToDo.Application.Enums;
using ToDo.Application.Queries;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("to-do-tasks")]
public class ToDoTasksController : ControllerBase
{
    private readonly IQueryHandler<GetToDoTask, ToDoTaskDto> _getToDoTaskHandler;
    private readonly IQueryHandler<GetToDoTasks, PagedResult<ToDoTaskDto>> _getToDoTasksHandler;
    private readonly IQueryHandler<GetIncomingToDoTasks, IEnumerable<ToDoTaskDto>> _getIncomingToDoTasksHandler;
    private readonly ICommandHandler<CreateToDoTask> _createToDoTaskHandler;
    private readonly ICommandHandler<UpdateToDoTask> _updateToDoTaskHandler;
    private readonly ICommandHandler<SetToDoTaskPercentComplete> _setToDoTaskPercentCompleteHandler;
    private readonly ICommandHandler<DeleteToDoTask> _deleteToDoTaskHandler;
    private readonly ICommandHandler<MarkToDoTaskAsDone> _markToDoTaskAsDoneHandler;

    public ToDoTasksController(IQueryHandler<GetToDoTask, ToDoTaskDto> getToDoTaskHandler,
        IQueryHandler<GetToDoTasks, PagedResult<ToDoTaskDto>> getToDoTasksHandler,
        IQueryHandler<GetIncomingToDoTasks, IEnumerable<ToDoTaskDto>> getIncomingToDoTasksHandler,
        ICommandHandler<CreateToDoTask> createToDoTaskHandler,
        ICommandHandler<UpdateToDoTask> updateToDoTaskHandler,
        ICommandHandler<SetToDoTaskPercentComplete> setToDoTaskPercentCompleteHandler,
        ICommandHandler<DeleteToDoTask> deleteToDoTaskHandler,
        ICommandHandler<MarkToDoTaskAsDone> markToDoTaskAsDoneHandler)
    {
        _getToDoTaskHandler = getToDoTaskHandler;
        _getToDoTasksHandler = getToDoTasksHandler;
        _getIncomingToDoTasksHandler = getIncomingToDoTasksHandler;
        _createToDoTaskHandler = createToDoTaskHandler;
        _updateToDoTaskHandler = updateToDoTaskHandler;
        _setToDoTaskPercentCompleteHandler = setToDoTaskPercentCompleteHandler;
        _deleteToDoTaskHandler = deleteToDoTaskHandler;
        _markToDoTaskAsDoneHandler = markToDoTaskAsDoneHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoTaskDto>>> Get([FromQuery] GetToDoTasks command)
    {
        var toDoTasks = await _getToDoTasksHandler.HandleAsync(command);

        return Ok(toDoTasks);
    }

    [HttpGet("{toDoTaskId:guid}")]
    public async Task<ActionResult<ToDoTaskDto>> Get(Guid toDoTaskId)
    {
        var toDoTask = await _getToDoTaskHandler.HandleAsync(new GetToDoTask(ToDoTaskId: toDoTaskId));

        return Ok(toDoTask);
    }

    [HttpGet("incoming")]
    public async Task<ActionResult<IEnumerable<ToDoTaskDto>>> Get([FromQuery] IncomingFilter incomingFilter)
    {
        var toDoTasks = await _getIncomingToDoTasksHandler
            .HandleAsync(new GetIncomingToDoTasks(incomingFilter));

        return Ok(toDoTasks);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateToDoTask command)
    {
        var toDoTaskId = Guid.NewGuid();

        await _createToDoTaskHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return CreatedAtAction(nameof(Get), new { toDoTaskId }, null);
    }

    [HttpPut("{toDoTaskId:guid}")]
    public async Task<ActionResult> Put(Guid toDoTaskId, UpdateToDoTask command)
    {
        await _updateToDoTaskHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return Ok();
    }

    [HttpPatch("{toDoTaskId:guid}/percent-complete")]
    public async Task<ActionResult> Patch(Guid toDoTaskId, SetToDoTaskPercentComplete command)
    {
        await _setToDoTaskPercentCompleteHandler.HandleAsync(command with { ToDoTaskId = toDoTaskId });

        return Ok();
    }

    [HttpDelete("{toDoTaskId:guid}")]
    public async Task<ActionResult> Delete(Guid toDoTaskId)
    {
        await _deleteToDoTaskHandler.HandleAsync(new DeleteToDoTask(ToDoTaskId: toDoTaskId));

        return NoContent();
    }

    [HttpPatch("{toDoTaskId:guid}/done")]
    public async Task<ActionResult> Patch(Guid toDoTaskId)
    {
        await _markToDoTaskAsDoneHandler.HandleAsync(new MarkToDoTaskAsDone(ToDoTaskId: toDoTaskId));

        return Ok();
    }
}