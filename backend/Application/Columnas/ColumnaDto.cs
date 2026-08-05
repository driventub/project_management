namespace ProjectManagement.Application.Columnas;

public record ColumnaDto(
    Guid Id,
    string Nombre,
    int Orden,
    Guid ProyectoId);
