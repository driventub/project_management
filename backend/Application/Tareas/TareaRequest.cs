namespace ProjectManagement.Application.Tareas;

public record TareaRequest(
    Guid ColumnaId,
    string Titulo,
    string Descripcion,
    string Prioridad,
    Guid? ResponsableId);
