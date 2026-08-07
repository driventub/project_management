namespace ProjectManagement.Application.Tareas;

public record TareaMovidaNotification(
    Guid ProyectoId,
    Guid ColumnaOrigenId,
    Guid ColumnaDestinoId,
    IReadOnlyList<TareaDto> TareasColumnaOrigen,
    IReadOnlyList<TareaDto> TareasColumnaDestino);
