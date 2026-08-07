using Mapster;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Proyectos;

public class ProyectoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Proyecto, ProyectoDto>()
            .Map(dest => dest.Estado, src => src.Estado.ToString());

        config.NewConfig<ProyectoRequest, Proyecto>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Columnas)
            .Map(dest => dest.Estado, src => ParseEstado(src.Estado));
    }

    public static EstadoProyecto ParseEstado(string estado)
    {
        if (!Enum.TryParse<EstadoProyecto>(estado, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Estado '{estado}' inválido.");
        }

        return parsed;
    }
}
