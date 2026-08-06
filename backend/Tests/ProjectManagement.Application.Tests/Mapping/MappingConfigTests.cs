using ProjectManagement.Application.Proyectos;
using ProjectManagement.Application.Tareas;
using ProjectManagement.Domain.Enums;
using Xunit;

namespace ProjectManagement.Application.Tests.Mapping;

public class MappingConfigTests
{
    [Theory]
    [InlineData("Planificado", EstadoProyecto.Planificado)]
    [InlineData("completado", EstadoProyecto.Completado)]
    public void ParseEstado_ValorValido_ParseaIgnorandoMayusculas(string valor, EstadoProyecto esperado)
    {
        Assert.Equal(esperado, ProyectoMappingConfig.ParseEstado(valor));
    }

    [Fact]
    public void ParseEstado_ValorInvalido_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ProyectoMappingConfig.ParseEstado("EnPausa"));
    }

    [Theory]
    [InlineData("Baja", Prioridad.Baja)]
    [InlineData("urgente", Prioridad.Urgente)]
    public void ParsePrioridad_ValorValido_ParseaIgnorandoMayusculas(string valor, Prioridad esperado)
    {
        Assert.Equal(esperado, TareaMappingConfig.ParsePrioridad(valor));
    }

    [Fact]
    public void ParsePrioridad_ValorInvalido_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TareaMappingConfig.ParsePrioridad("Critica"));
    }
}
