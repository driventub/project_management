namespace ProjectManagement.Application.Tareas;

public record TareaDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    string Prioridad,
    DateTime FechaCreacion,
    int Orden,
    Guid? ResponsableId,
    string? ResponsableNombre,
    Guid ColumnaId,
    Guid ProyectoId);
