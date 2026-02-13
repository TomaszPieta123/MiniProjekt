namespace MiniProjekt.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDone { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public string? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }
}
