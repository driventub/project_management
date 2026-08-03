using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Tarea
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Prioridad Prioridad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int Orden { get; set; }

    public Guid? ResponsableId { get; set; }
    public Usuario? Responsable { get; set; }

    public Guid ColumnaId { get; set; }
    public Columna Columna { get; set; } = null!;
}
