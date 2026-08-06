namespace ProjectManagement.Application.Reportes;

public record ProyectoReporteDto(
    Guid ProyectoId,
    string Nombre,
    string Descripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinEsperada,
    string Estado,
    DateTime FechaGeneracion,
    IReadOnlyList<TareaReporteItemDto> Tareas);
