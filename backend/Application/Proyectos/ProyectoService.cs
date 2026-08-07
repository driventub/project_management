using MapsterMapper;
using ProjectManagement.Application.Ports;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Proyectos;

public class ProyectoService
{
    private readonly IProyectoRepository _proyectoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProyectoService(IProyectoRepository proyectoRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _proyectoRepository = proyectoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProyectoDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? nombreFilter,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _proyectoRepository.GetPagedAsync(pageNumber, pageSize, nombreFilter, cancellationToken);
        return new PagedResult<ProyectoDto>(_mapper.Map<List<ProyectoDto>>(items), totalCount, pageNumber, pageSize);
    }

    public async Task<ProyectoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id, cancellationToken);
        return proyecto is null ? null : _mapper.Map<ProyectoDto>(proyecto);
    }

    public async Task<ProyectoDto> CreateAsync(ProyectoRequest request, CancellationToken cancellationToken = default)
    {
        var proyecto = _mapper.Map<Proyecto>(request);
        proyecto.Id = Guid.NewGuid();

        await _proyectoRepository.AddAsync(proyecto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProyectoDto>(proyecto);
    }

    public async Task<ProyectoDto?> UpdateAsync(Guid id, ProyectoRequest request, CancellationToken cancellationToken = default)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id, cancellationToken);
        if (proyecto is null)
        {
            return null;
        }

        _mapper.Map(request, proyecto);

        _proyectoRepository.Update(proyecto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProyectoDto>(proyecto);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proyecto = await _proyectoRepository.GetByIdAsync(id, cancellationToken);
        if (proyecto is null)
        {
            return false;
        }

        _proyectoRepository.Remove(proyecto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
