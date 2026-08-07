using Mapster;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Usuarios;

public class UsuarioMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Usuario, UsuarioDto>();
    }
}
