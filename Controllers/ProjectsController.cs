using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjekt.Data;
using MiniProjekt.Dtos;
using MiniProjekt.Models;

namespace MiniProjekt.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<List<ProjectReadDto>>> GetVisibleProjects()
    {
        var userId = UserId;

        var projects = await _db.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId || p.Tasks.Any(t => t.AssigneeId == userId))
            .Select(p => new ProjectReadDto(p.Id, p.Name, p.Description, p.OwnerId))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectReadDto>> GetById(int id)
    {
        var userId = UserId;

        var project = await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == id && (p.OwnerId == userId || p.Tasks.Any(t => t.AssigneeId == userId)))
            .Select(p => new ProjectReadDto(p.Id, p.Name, p.Description, p.OwnerId))
            .FirstOrDefaultAsync();

        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> Create(ProjectCreateDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = UserId
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        var read = new ProjectReadDto(project.Id, project.Name, project.Description, project.OwnerId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto dto)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();
        if (project.OwnerId != UserId) return Forbid();

        project.Name = dto.Name;
        project.Description = dto.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();
        if (project.OwnerId != UserId) return Forbid();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
