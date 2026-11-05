using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<RoleDetailsDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var roles = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<RoleDetailsDto>>(roles), total);
    }

    public async Task<RoleDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        return role is null ? null : _mapper.Map<RoleDetailsDto>(role);
    }

    public async Task<RoleDetailsDto> CreateAsync(CreateRoleDto dto, CancellationToken ct)
    {
        var role = _mapper.Map<Role>(dto);

        await _repository.AddAsync(role);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(role.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданную роль.");
        return _mapper.Map<RoleDetailsDto>(saved);
    }

    public async Task<RoleDetailsDto?> UpdateAsync(
        int id, UpdateRoleDto dto, CancellationToken ct)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        if (role is null) return null;

        _mapper.Map(dto, role);

        _repository.Update(role);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённую роль.");
        return _mapper.Map<RoleDetailsDto>(saved);
    }

    public async Task<RoleDetailsDto?> PatchAsync(
        int id, PatchRoleDto dto, CancellationToken ct)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        if (role is null) return null;

        if (dto.RoleName is not null) role.RoleName = dto.RoleName;

        _repository.Update(role);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённую роль.");
        return _mapper.Map<RoleDetailsDto>(saved);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        if (role is null) return false;

        _repository.Remove(role);
        await _repository.SaveChangesAsync();
        return true;
    }
}
