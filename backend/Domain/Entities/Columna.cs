namespace ProjectManagement.Domain.Entities;

public class Columna
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }

    public Guid ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
