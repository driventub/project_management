using Moq;
using ProjectManagement.Application.Columnas;
using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Tests.TestSupport;
using ProjectManagement.Domain.Entities;
using Xunit;

namespace ProjectManagement.Application.Tests.Columnas;

public class ColumnaServiceTests
{
    private static ColumnaService BuildService(
        Mock<IColumnaRepository> columnaRepository,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        unitOfWork ??= new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var tableroNotifier = new Mock<ITableroNotifier>();
        return new ColumnaService(columnaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), tableroNotifier.Object);
    }

    [Fact]
    public async Task CreateAsync_PrimeraColumnaDelProyecto_OrdenEsCero()
    {
        var proyectoId = Guid.NewGuid();
        var columnaRepository = new Mock<IColumnaRepository>();
        columnaRepository.Setup(r => r.GetByProyectoIdAsync(proyectoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Columna>());

        var service = BuildService(columnaRepository);
        var resultado = await service.CreateAsync(proyectoId, new ColumnaRequest("Todo"));

        Assert.Equal(0, resultado.Orden);
        columnaRepository.Verify(r => r.AddAsync(It.Is<Columna>(c => c.Orden == 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ConColumnasExistentes_OrdenEsElMaximoMasUno()
    {
        var proyectoId = Guid.NewGuid();
        var existentes = new List<Columna>
        {
            new() { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId },
            new() { Id = Guid.NewGuid(), Nombre = "En progreso", Orden = 1, ProyectoId = proyectoId },
            new() { Id = Guid.NewGuid(), Nombre = "Hecho", Orden = 2, ProyectoId = proyectoId }
        };

        var columnaRepository = new Mock<IColumnaRepository>();
        columnaRepository.Setup(r => r.GetByProyectoIdAsync(proyectoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existentes);

        var service = BuildService(columnaRepository);
        var resultado = await service.CreateAsync(proyectoId, new ColumnaRequest("Bloqueado"));

        Assert.Equal(3, resultado.Orden);
    }

    [Fact]
    public async Task DeleteAsync_ColumnaConTareas_RetornaTieneTareasYNoElimina()
    {
        var columna = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = Guid.NewGuid() };

        var columnaRepository = new Mock<IColumnaRepository>();
        columnaRepository.Setup(r => r.GetByIdAsync(columna.Id, It.IsAny<CancellationToken>())).ReturnsAsync(columna);
        columnaRepository.Setup(r => r.HasTareasAsync(columna.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var unitOfWork = new Mock<IUnitOfWork>();
        var service = BuildService(columnaRepository, unitOfWork);

        var resultado = await service.DeleteAsync(columna.Id);

        Assert.Equal(ColumnaDeleteResult.TieneTareas, resultado);
        columnaRepository.Verify(r => r.Remove(It.IsAny<Columna>()), Times.Never);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReorderAsync_IdQueNoPerteneceAlProyecto_LanzaArgumentException()
    {
        var proyectoId = Guid.NewGuid();
        var columna = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId };

        var columnaRepository = new Mock<IColumnaRepository>();
        columnaRepository.Setup(r => r.GetByProyectoIdAsync(proyectoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Columna> { columna });

        var service = BuildService(columnaRepository);

        var request = new ReorderColumnasRequest(new List<ReorderColumnaItem> { new(Guid.NewGuid(), 0) });

        await Assert.ThrowsAsync<ArgumentException>(() => service.ReorderAsync(proyectoId, request));
    }
}
