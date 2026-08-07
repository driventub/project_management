using Mapster;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Columnas;

public class ColumnaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Columna, ColumnaDto>();

        config.NewConfig<ColumnaRequest, Columna>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Orden)
            .Ignore(dest => dest.ProyectoId)
            .Ignore(dest => dest.Proyecto)
            .Ignore(dest => dest.Tareas);
    }
}
