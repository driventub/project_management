using Mapster;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Tareas;

public class TareaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tarea, TareaDto>()
            .Map(dest => dest.Prioridad, src => src.Prioridad.ToString())
            .Map(dest => dest.ProyectoId, src => src.Columna.ProyectoId)
            .Map(dest => dest.ResponsableNombre, src => src.Responsable == null ? null : src.Responsable.Nombre);

        config.NewConfig<TareaRequest, Tarea>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Orden)
            .Ignore(dest => dest.ColumnaId)
            .Ignore(dest => dest.Columna)
            .Ignore(dest => dest.FechaCreacion)
            .Ignore(dest => dest.Responsable)
            .Map(dest => dest.Prioridad, src => ParsePrioridad(src.Prioridad));
    }

    public static Prioridad ParsePrioridad(string prioridad)
    {
        if (!Enum.TryParse<Prioridad>(prioridad, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Prioridad '{prioridad}' inválida.");
        }

        return parsed;
    }
}
