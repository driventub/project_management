namespace ProjectManagement.Application.Tareas;

public record MoverTareaRequest(Guid ColumnaDestinoId, IReadOnlyList<Guid> OrdenIds);
