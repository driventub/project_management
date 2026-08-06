using Moq;
using ProjectManagement.Application.Ports;
using ProjectManagement.Application.Tareas;
using ProjectManagement.Application.Tests.TestSupport;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using Xunit;

namespace ProjectManagement.Application.Tests.Tareas;

// TareaService.MoverAsync is the position-calc logic flagged for a dedicated test
// (README ordering-index strategy section, Phase 6/9 notes): it reindexes the
// destination column from the client-supplied OrdenIds list and, on a cross-column
// move, also closes the gap left in the origin column.
public class TareaServiceTests
{
    private static Tarea NuevaTarea(Columna columna, int orden)
    {
        var tarea = new Tarea
        {
            Id = Guid.NewGuid(),
            Titulo = "Tarea",
            Descripcion = string.Empty,
            Prioridad = Prioridad.Media,
            FechaCreacion = DateTime.UtcNow,
            Orden = orden,
            ColumnaId = columna.Id,
            Columna = columna
        };
        columna.Tareas.Add(tarea);
        return tarea;
    }

    [Fact]
    public async Task MoverAsync_DentroDeLaMismaColumna_ReindexaSegunOrdenIds()
    {
        var proyectoId = Guid.NewGuid();
        var columna = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId };

        var t1 = NuevaTarea(columna, 0);
        var t2 = NuevaTarea(columna, 1);
        var t3 = NuevaTarea(columna, 2);

        var tareaRepository = new Mock<ITareaRepository>();
        tareaRepository.Setup(r => r.GetByIdAsync(t2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(t2);
        tareaRepository.Setup(r => r.GetByColumnaIdAsync(columna.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tarea> { t1, t2, t3 });

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var notifier = new Mock<ITableroNotifier>();

        var service = new TareaService(tareaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), notifier.Object);

        var request = new MoverTareaRequest(columna.Id, new List<Guid> { t2.Id, t1.Id, t3.Id });
        var resultado = await service.MoverAsync(t2.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal(0, t2.Orden);
        Assert.Equal(1, t1.Orden);
        Assert.Equal(2, t3.Orden);
        notifier.Verify(n => n.TareaMovidaAsync(It.IsAny<TareaMovidaNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MoverAsync_EntreColumnas_ReindexaDestinoYCierraElHuecoEnOrigen()
    {
        var proyectoId = Guid.NewGuid();
        var origen = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId };
        var destino = new Columna { Id = Guid.NewGuid(), Nombre = "Hecho", Orden = 1, ProyectoId = proyectoId };

        var a = NuevaTarea(origen, 0);
        var b = NuevaTarea(origen, 1);
        var c = NuevaTarea(origen, 2);
        var x = NuevaTarea(destino, 0);
        var y = NuevaTarea(destino, 1);

        var tareaRepository = new Mock<ITareaRepository>();
        tareaRepository.Setup(r => r.GetByIdAsync(b.Id, It.IsAny<CancellationToken>())).ReturnsAsync(b);
        tareaRepository.Setup(r => r.GetByColumnaIdAsync(destino.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<Tarea> { x, y });
        tareaRepository.Setup(r => r.GetByColumnaIdAsync(origen.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<Tarea> { a, b, c });

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var notifier = new Mock<ITableroNotifier>();

        var service = new TareaService(tareaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), notifier.Object);

        // b se inserta entre x e y en la columna destino.
        var request = new MoverTareaRequest(destino.Id, new List<Guid> { x.Id, b.Id, y.Id });
        var resultado = await service.MoverAsync(b.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal(destino.Id, b.ColumnaId);
        Assert.Equal(0, x.Orden);
        Assert.Equal(1, b.Orden);
        Assert.Equal(2, y.Orden);

        // La columna de origen cierra el hueco dejado por b: a y c quedan 0..n-1.
        Assert.Equal(0, a.Orden);
        Assert.Equal(1, c.Orden);
    }

    [Fact]
    public async Task MoverAsync_OrdenIdsNoCoincideConLaColumnaDestino_LanzaArgumentException()
    {
        var proyectoId = Guid.NewGuid();
        var columna = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId };
        var t1 = NuevaTarea(columna, 0);
        var t2 = NuevaTarea(columna, 1);

        var tareaRepository = new Mock<ITareaRepository>();
        tareaRepository.Setup(r => r.GetByIdAsync(t1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(t1);
        tareaRepository.Setup(r => r.GetByColumnaIdAsync(columna.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tarea> { t1, t2 });

        var unitOfWork = new Mock<IUnitOfWork>();
        var notifier = new Mock<ITableroNotifier>();
        var service = new TareaService(tareaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), notifier.Object);

        // Guid.NewGuid() ajeno a la columna destino: la lista de orden no coincide.
        var request = new MoverTareaRequest(columna.Id, new List<Guid> { t1.Id, Guid.NewGuid() });

        await Assert.ThrowsAsync<ArgumentException>(() => service.MoverAsync(t1.Id, request));
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MoverAsync_TareaInexistente_RetornaNullYNoNotifica()
    {
        var tareaRepository = new Mock<ITareaRepository>();
        tareaRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Tarea?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var notifier = new Mock<ITableroNotifier>();
        var service = new TareaService(tareaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), notifier.Object);

        var request = new MoverTareaRequest(Guid.NewGuid(), new List<Guid>());
        var resultado = await service.MoverAsync(Guid.NewGuid(), request);

        Assert.Null(resultado);
        notifier.Verify(n => n.TareaMovidaAsync(It.IsAny<TareaMovidaNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_PrimeraTareaDeLaColumna_NoLanzaYAsignaOrdenCero()
    {
        // Regresión Fase 7: cuando la columna destino no tenía tareas, EF nunca
        // completaba el fixup de tarea.Columna y el mapeo a TareaDto (que lee
        // src.Columna.ProyectoId) lanzaba NullReferenceException. CreateAsync
        // ahora re-consulta con GetByIdAsync (que incluye Columna) antes de mapear.
        var proyectoId = Guid.NewGuid();
        var columna = new Columna { Id = Guid.NewGuid(), Nombre = "Todo", Orden = 0, ProyectoId = proyectoId };

        var tareaRepository = new Mock<ITareaRepository>();
        tareaRepository.Setup(r => r.GetByColumnaIdAsync(columna.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tarea>());

        Tarea? creada = null;
        tareaRepository.Setup(r => r.AddAsync(It.IsAny<Tarea>(), It.IsAny<CancellationToken>()))
            .Callback<Tarea, CancellationToken>((t, _) => creada = t)
            .Returns(Task.CompletedTask);
        tareaRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                creada!.Columna = columna;
                return creada;
            });

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var notifier = new Mock<ITableroNotifier>();

        var service = new TareaService(tareaRepository.Object, unitOfWork.Object, TestMapperFactory.Create(), notifier.Object);
        var request = new TareaRequest(columna.Id, "Primera", string.Empty, "Media", null);

        var resultado = await service.CreateAsync(request);

        Assert.Equal(0, resultado.Orden);
        Assert.Equal(proyectoId, resultado.ProyectoId);
    }
}
