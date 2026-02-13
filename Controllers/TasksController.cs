using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjekt.Data;
using MiniProjekt.Dtos;
using MiniProjekt.Models;

namespace MiniProjekt.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private Task<bool> CanAccessProject(int projectId)
    {
        var userId = UserId;
        return _db.Projects.AnyAsync(p =>
            p.Id == projectId &&
            (p.OwnerId == userId || p.Tasks.Any(t => t.AssigneeId == userId)));
    }

    private Task<bool> IsProjectOwner(int projectId)
    {
        var userId = UserId;
        return _db.Projects.AnyAsync(p => p.Id == projectId && p.OwnerId == userId);
    }

    [HttpGet("by-project/{projectId:int}")]
    public async Task<ActionResult<List<TaskReadDto>>> GetByProject(int projectId)
    {
        if (!await CanAccessProject(projectId)) return Forbid();

        var tasks = await _db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .Select(t => new TaskReadDto(t.Id, t.ProjectId, t.Title, t.Description, t.IsDone, t.AssigneeId))
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskReadDto>> GetById(int id)
    {
        var task = await _db.Tasks
            .AsNoTracking()
            .Select(t => new { t.Id, t.ProjectId, t.Title, t.Description, t.IsDone, t.AssigneeId })
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return NotFound();
        if (!await CanAccessProject(task.ProjectId)) return Forbid();

        return Ok(new TaskReadDto(task.Id, task.ProjectId, task.Title, task.Description, task.IsDone, task.AssigneeId));
    }

    [HttpPost]
    public async Task<ActionResult<TaskReadDto>> Create(TaskCreateDto dto)
    {
        if (!await IsProjectOwner(dto.ProjectId)) return Forbid();

        var task = new TaskItem
        {
            ProjectId = dto.ProjectId,
            Title = dto.Title,
            Description = dto.Description,
            AssigneeId = dto.AssigneeId,
            IsDone = false
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        var read = new TaskReadDto(task.Id, task.ProjectId, task.Title, task.Description, task.IsDone, task.AssigneeId);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
    {
        var task = await _db.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return NotFound();

        var userId = UserId;
        var isOwner = task.Project.OwnerId == userId;
        var isAssignee = task.AssigneeId == userId;

        if (!isOwner && !isAssignee) return Forbid();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsDone = dto.IsDone;

        // tylko owner może zmieniać AssigneeId
        if (isOwner)
            task.AssigneeId = dto.AssigneeId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return NotFound();
        if (task.Project.OwnerId != UserId) return Forbid();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
