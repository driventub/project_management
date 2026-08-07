using MapsterMapper;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Columnas;

public enum ColumnaDeleteResult
{
    NotFound,
    TieneTareas,
    Eliminada
}

public class ColumnaService
{
    private readonly IColumnaRepository _columnaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITableroNotifier _tableroNotifier;

    public ColumnaService(IColumnaRepository columnaRepository, IUnitOfWork unitOfWork, IMapper mapper, ITableroNotifier tableroNotifier)
    {
        _columnaRepository = columnaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tableroNotifier = tableroNotifier;
    }

    public async Task<IReadOnlyList<ColumnaDto>> GetByProyectoIdAsync(Guid proyectoId, CancellationToken cancellationToken = default)
    {
        var columnas = await _columnaRepository.GetByProyectoIdAsync(proyectoId, cancellationToken);
        return _mapper.Map<List<ColumnaDto>>(columnas);
    }

    public async Task<ColumnaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var columna = await _columnaRepository.GetByIdAsync(id, cancellationToken);
        return columna is null ? null : _mapper.Map<ColumnaDto>(columna);
    }

    public async Task<ColumnaDto> CreateAsync(Guid proyectoId, ColumnaRequest request, CancellationToken cancellationToken = default)
    {
        var existentes = await _columnaRepository.GetByProyectoIdAsync(proyectoId, cancellationToken);

        var columna = _mapper.Map<Columna>(request);
        columna.Id = Guid.NewGuid();
        columna.ProyectoId = proyectoId;
        columna.Orden = existentes.Count == 0 ? 0 : existentes.Max(c => c.Orden) + 1;

        await _columnaRepository.AddAsync(columna, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ColumnaDto>(columna);
        await _tableroNotifier.ColumnaCreadaAsync(proyectoId, dto, cancellationToken);

        return dto;
    }

    public async Task<ColumnaDto?> UpdateAsync(Guid id, ColumnaRequest request, CancellationToken cancellationToken = default)
    {
        var columna = await _columnaRepository.GetByIdAsync(id, cancellationToken);
        if (columna is null)
        {
            return null;
        }

        _mapper.Map(request, columna);

        _columnaRepository.Update(columna);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ColumnaDto>(columna);
    }

    public async Task<ColumnaDeleteResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var columna = await _columnaRepository.GetByIdAsync(id, cancellationToken);
        if (columna is null)
        {
            return ColumnaDeleteResult.NotFound;
        }

        if (await _columnaRepository.HasTareasAsync(id, cancellationToken))
        {
            return ColumnaDeleteResult.TieneTareas;
        }

        _columnaRepository.Remove(columna);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _tableroNotifier.ColumnaEliminadaAsync(columna.ProyectoId, id, cancellationToken);

        return ColumnaDeleteResult.Eliminada;
    }

    public async Task<IReadOnlyList<ColumnaDto>> ReorderAsync(Guid proyectoId, ReorderColumnasRequest request, CancellationToken cancellationToken = default)
    {
        var columnas = await _columnaRepository.GetByProyectoIdAsync(proyectoId, cancellationToken);
        var columnasPorId = columnas.ToDictionary(c => c.Id);

        foreach (var item in request.Items)
        {
            if (!columnasPorId.TryGetValue(item.Id, out var columna))
            {
                throw new ArgumentException($"Columna '{item.Id}' no pertenece al proyecto.");
            }

            columna.Orden = item.Orden;
            _columnaRepository.Update(columna);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<List<ColumnaDto>>(columnas.OrderBy(c => c.Orden));
    }
}
