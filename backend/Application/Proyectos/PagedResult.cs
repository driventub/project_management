namespace ProjectManagement.Application.Proyectos;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize);
