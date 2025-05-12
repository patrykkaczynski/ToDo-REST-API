using ToDo.Core.Exceptions;
using ToDo.Core.ValueObjects;

namespace ToDo.Core.Entities;

public class ToDoTask
{
    public ToDoTaskId Id { get; private set; }
    public DateAndTime ExpirationDate { get; private set; }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public PercentComplete PercentComplete { get; private set; }

    private ToDoTask()
    {
    }

    private ToDoTask(ToDoTaskId id, DateAndTime expirationDate, Title title,
        Description description, PercentComplete percentComplete, DateAndTime now)
    {
        Id = id;
        SetToDoTask(expirationDate, title, description, percentComplete, now);
    }

    public static ToDoTask Create(ToDoTaskId id, DateAndTime expirationDate, Title title,
        Description description, PercentComplete percentComplete, DateAndTime now)
        => new(id, expirationDate, title, description, percentComplete, now);

    public void Update(DateAndTime expirationDate, Title title, Description description,
        PercentComplete percentComplete, DateAndTime now)
        => SetToDoTask(expirationDate, title, description, percentComplete, now);

    public void ChangePercentComplete(PercentComplete percentComplete)
        => PercentComplete = percentComplete;

    public void MarkAsDone()
        => ChangePercentComplete(100);

    private void SetToDoTask(DateAndTime expirationDate, Title title, Description description,
        PercentComplete percentComplete, DateAndTime now)
    {
        if (expirationDate <= now)
        {
            throw new InvalidExpirationDateException(expirationDate.Value.Date);
        }

        ExpirationDate = expirationDate;
        Title = title;
        Description = description;
        ChangePercentComplete(percentComplete);
    }
}