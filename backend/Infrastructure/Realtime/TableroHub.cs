using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProjectManagement.Infrastructure.Realtime;

[Authorize]
public class TableroHub : Hub
{
    public const string Ruta = "/hubs/tablero";

    public static string Grupo(Guid proyectoId) => $"tablero-{proyectoId}";

    public Task UnirseTablero(Guid proyectoId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, Grupo(proyectoId));

    public Task SalirTablero(Guid proyectoId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, Grupo(proyectoId));
}
