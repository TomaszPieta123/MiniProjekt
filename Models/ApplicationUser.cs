using Microsoft.AspNetCore.Identity;

namespace MiniProjekt.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
