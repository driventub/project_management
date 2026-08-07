using Mapster;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Reportes;

public class ReporteMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tarea, TareaReporteItemDto>()
            .Map(dest => dest.ColumnaNombre, src => src.Columna.Nombre)
            .Map(dest => dest.ResponsableNombre, src => src.Responsable == null ? null : src.Responsable.Nombre)
            .Map(dest => dest.Prioridad, src => src.Prioridad.ToString());
    }
}
