using Microsoft.AspNetCore.SignalR;
using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Tareas;

namespace ProjectManagement.Infrastructure.Realtime;

public class TableroNotifier : ITableroNotifier
{
    private readonly IHubContext<TableroHub> _hubContext;

    public TableroNotifier(IHubContext<TableroHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task TareaCreadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken cancellationToken = default) =>
        Clientes(proyectoId).SendAsync("TareaCreada", tarea, cancellationToken);

    public Task TareaActualizadaAsync(Guid proyectoId, TareaDto tarea, CancellationToken cancellationToken = default) =>
        Clientes(proyectoId).SendAsync("TareaActualizada", tarea, cancellationToken);

    public Task TareaEliminadaAsync(Guid proyectoId, Guid tareaId, CancellationToken cancellationToken = default) =>
        Clientes(proyectoId).SendAsync("TareaEliminada", tareaId, cancellationToken);

    public Task TareaMovidaAsync(TareaMovidaNotification notificacion, CancellationToken cancellationToken = default) =>
        Clientes(notificacion.ProyectoId).SendAsync("TareaMovida", notificacion, cancellationToken);

    private IClientProxy Clientes(Guid proyectoId) => _hubContext.Clients.Group(TableroHub.Grupo(proyectoId));
}
