using Mapster;
using MapsterMapper;
using ProjectManagement.Application.Proyectos;

namespace ProjectManagement.Application.Tests.TestSupport;

// Builds a real Mapster IMapper from the Application assembly's IRegister classes
// (ProyectoMappingConfig, ColumnaMappingConfig, TareaMappingConfig, ...) so service
// tests exercise the actual enum-string conversions and Ignore()s instead of a mock.
// A fresh TypeAdapterConfig per call avoids sharing static state across test classes.
internal static class TestMapperFactory
{
    public static IMapper Create()
    {
        var config = new TypeAdapterConfig();
        config.Scan(typeof(ProyectoMappingConfig).Assembly);
        return new Mapper(config);
    }
}
