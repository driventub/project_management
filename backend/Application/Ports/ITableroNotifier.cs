using ProjectManagement.Application.Columnas;
using ProjectManagement.Application.Tareas;

namespace ProjectManagement.Application.Ports;

public interface ITableroNotifier
{
    Task TareaCreadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken cancellationToken = default);
    Task TareaActualizadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken cancellationToken = default);
    Task TareaEliminadaAsync(Guid proyectoId, Guid tareaId, CancellationToken cancellationToken = default);
    Task TareaMovidaAsync(TareaMovidaNotification notificacion, CancellationToken cancellationToken = default);
    Task ColumnaCreadaAsync(Guid proyectoId, ColumnaDto columna, CancellationToken cancellationToken = default);
    Task ColumnaEliminadaAsync(Guid proyectoId, Guid columnaId, CancellationToken cancellationToken = default);
}
