using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface IRoleService
{
    Task<(List<RoleDetailsDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct);

    Task<RoleDetailsDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<RoleDetailsDto> CreateAsync(CreateRoleDto dto, CancellationToken ct);

    Task<RoleDetailsDto?> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken ct);

    Task<RoleDetailsDto?> PatchAsync(int id, PatchRoleDto dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
