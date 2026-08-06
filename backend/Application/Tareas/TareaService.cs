using MapsterMapper;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Tareas;

public class TareaService
{
    private readonly ITareaRepository _tareaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITableroNotifier _tableroNotifier;

    public TareaService(ITareaRepository tareaRepository, IUnitOfWork unitOfWork, IMapper mapper, ITableroNotifier tableroNotifier)
    {
        _tareaRepository = tareaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tableroNotifier = tableroNotifier;
    }

    public async Task<IReadOnlyList<TareaDto>> GetByProyectoIdAsync(Guid proyectoId, CancellationToken cancellationToken = default)
    {
        var tareas = await _tareaRepository.GetByProyectoIdAsync(proyectoId, cancellationToken);
        return _mapper.Map<List<TareaDto>>(tareas);
    }

    public async Task<TareaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        return tarea is null ? null : _mapper.Map<TareaDto>(tarea);
    }

    public async Task<TareaDto> CreateAsync(TareaRequest request, CancellationToken cancellationToken = default)
    {
        var existentes = await _tareaRepository.GetByColumnaIdAsync(request.ColumnaId, cancellationToken);

        var tarea = _mapper.Map<Tarea>(request);
        tarea.Id = Guid.NewGuid();
        tarea.ColumnaId = request.ColumnaId;
        tarea.FechaCreacion = DateTime.UtcNow;
        tarea.Orden = existentes.Count == 0 ? 0 : existentes.Max(t => t.Orden) + 1;

        await _tareaRepository.AddAsync(tarea, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // existentes may be empty (first task in the column), in which case tarea.Columna
        // never got loaded into the change tracker via relationship fixup — refetch with
        // its Include(Columna) so the ProyectoId mapping below doesn't NRE.
        var creada = await _tareaRepository.GetByIdAsync(tarea.Id, cancellationToken);
        var dto = _mapper.Map<TareaDto>(creada);

        await _tableroNotifier.TareaCreadaAsync(dto.ProyectoId, dto, cancellationToken);

        return dto;
    }

    public async Task<TareaDto?> UpdateAsync(Guid id, TareaRequest request, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        if (tarea is null)
        {
            return null;
        }

        _mapper.Map(request, tarea);

        _tareaRepository.Update(tarea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<TareaDto>(tarea);

        await _tableroNotifier.TareaActualizadaAsync(dto.ProyectoId, dto, cancellationToken);

        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        if (tarea is null)
        {
            return false;
        }

        var proyectoId = tarea.Columna.ProyectoId;

        _tareaRepository.Remove(tarea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _tableroNotifier.TareaEliminadaAsync(proyectoId, id, cancellationToken);

        return true;
    }

    public async Task<TareaDto?> MoverAsync(Guid tareaId, MoverTareaRequest request, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(tareaId, cancellationToken);
        if (tarea is null)
        {
            return null;
        }

        var columnaOrigenId = tarea.ColumnaId;
        var proyectoId = tarea.Columna.ProyectoId;
        tarea.ColumnaId = request.ColumnaDestinoId;

        var tareasDestino = columnaOrigenId == request.ColumnaDestinoId
            ? (await _tareaRepository.GetByColumnaIdAsync(columnaOrigenId, cancellationToken)).ToList()
            : (await _tareaRepository.GetByColumnaIdAsync(request.ColumnaDestinoId, cancellationToken))
                .Append(tarea)
                .ToList();

        var tareasPorId = tareasDestino.ToDictionary(t => t.Id);

        if (tareasPorId.Count != request.OrdenIds.Count || request.OrdenIds.Any(id => !tareasPorId.ContainsKey(id)))
        {
            throw new ArgumentException("La lista de orden no coincide con las tareas de la columna destino.");
        }

        for (var index = 0; index < request.OrdenIds.Count; index++)
        {
            var tareaOrdenada = tareasPorId[request.OrdenIds[index]];
            tareaOrdenada.Orden = index;
            _tareaRepository.Update(tareaOrdenada);
        }

        var tareasOrigenActualizadas = tareasDestino;
        if (columnaOrigenId != request.ColumnaDestinoId)
        {
            var restantesOrigen = (await _tareaRepository.GetByColumnaIdAsync(columnaOrigenId, cancellationToken))
                .Where(t => t.Id != tarea.Id)
                .OrderBy(t => t.Orden)
                .ToList();

            for (var index = 0; index < restantesOrigen.Count; index++)
            {
                restantesOrigen[index].Orden = index;
                _tareaRepository.Update(restantesOrigen[index]);
            }

            tareasOrigenActualizadas = restantesOrigen;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<TareaDto>(tarea);

        var notificacion = new TareaMovidaNotification(
            proyectoId,
            columnaOrigenId,
            request.ColumnaDestinoId,
            _mapper.Map<List<TareaDto>>(tareasOrigenActualizadas.OrderBy(t => t.Orden)),
            _mapper.Map<List<TareaDto>>(tareasDestino.OrderBy(t => t.Orden)));

        await _tableroNotifier.TareaMovidaAsync(notificacion, cancellationToken);

        return dto;
    }
}
