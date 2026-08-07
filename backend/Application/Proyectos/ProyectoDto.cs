namespace ProjectManagement.Application.Proyectos;

public record ProyectoDto(
    Guid Id,
    string Nombre,
    string Descripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinEsperada,
    string Estado);
