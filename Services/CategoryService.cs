using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<CategoryDetailsDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var categories = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<CategoryDetailsDto>>(categories), total);
    }

    public async Task<CategoryDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);
        return category is null ? null : _mapper.Map<CategoryDetailsDto>(category);
    }

    public async Task<CategoryDetailsDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct)
    {
        var category = _mapper.Map<Category>(dto);

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(category.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданную категорию.");
        return _mapper.Map<CategoryDetailsDto>(saved);
    }

    public async Task<CategoryDetailsDto?> UpdateAsync(
        int id, UpdateCategoryDto dto, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);
        if (category is null) return null;

        _mapper.Map(dto, category);

        _repository.Update(category);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённую категорию.");
        return _mapper.Map<CategoryDetailsDto>(saved);
    }

    public async Task<CategoryDetailsDto?> PatchAsync(
        int id, PatchCategoryDto dto, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);
        if (category is null) return null;

        if (dto.NameCategories is not null) category.NameCategories = dto.NameCategories;

        _repository.Update(category);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённую категорию.");
        return _mapper.Map<CategoryDetailsDto>(saved);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(id, ct);
        if (category is null) return false;

        _repository.Remove(category);
        await _repository.SaveChangesAsync();
        return true;
    }
}
