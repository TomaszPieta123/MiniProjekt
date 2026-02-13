namespace MiniProjekt.Dtos;

public record TaskCreateDto(int ProjectId, string Title, string? Description, string? AssigneeId);
public record TaskUpdateDto(string Title, string? Description, bool IsDone, string? AssigneeId);
public record TaskReadDto(int Id, int ProjectId, string Title, string? Description, bool IsDone, string? AssigneeId);
