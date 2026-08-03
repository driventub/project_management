using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Proyecto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFinEsperada { get; set; }
    public EstadoProyecto Estado { get; set; }

    public ICollection<Columna> Columnas { get; set; } = new List<Columna>();
}
