namespace ToDo.Application.DTO;

public class ToDoTaskDto
{
    public DateTime ExpirationDate { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int PercentComplete { get; set; }
}