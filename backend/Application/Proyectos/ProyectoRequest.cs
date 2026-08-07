namespace ProjectManagement.Application.Proyectos;

public record ProyectoRequest(
    string Nombre,
    string Descripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinEsperada,
    string Estado);
