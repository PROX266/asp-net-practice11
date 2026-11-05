using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface ICategoryService
{
    Task<(List<CategoryDetailsDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct);

    Task<CategoryDetailsDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<CategoryDetailsDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct);

    Task<CategoryDetailsDto?> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken ct);

    Task<CategoryDetailsDto?> PatchAsync(int id, PatchCategoryDto dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
