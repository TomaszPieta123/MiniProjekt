namespace MiniProjekt.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public string OwnerId { get; set; } = default!;
    public ApplicationUser Owner { get; set; } = default!;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
