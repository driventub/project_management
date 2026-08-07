namespace ProjectManagement.Application.Reportes;

public record TareaReporteItemDto(
    string Titulo,
    string ColumnaNombre,
    string? ResponsableNombre,
    string Prioridad);
