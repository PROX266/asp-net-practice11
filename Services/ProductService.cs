using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<ProductDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct)
    {
        var products = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<ProductDto>>(products), total);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct)
    {
        _ = await _repository.FindCategoryAsync(dto.CategoryId, ct)
            ?? throw new InvalidOperationException(
                $"Категория с Id {dto.CategoryId} не найдена.");

        var product = _mapper.Map<Product>(dto);

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(product.Id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить только что созданный товар.");
        return _mapper.Map<ProductDto>(saved);
    }

    public async Task<ProductDto?> UpdateAsync(
        int id, UpdateProductDto dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        _ = await _repository.FindCategoryAsync(dto.CategoryId, ct)
            ?? throw new InvalidOperationException(
                $"Категория с Id {dto.CategoryId} не найдена.");

        _mapper.Map(dto, product);

        _repository.Update(product);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённый товар.");
        return _mapper.Map<ProductDto>(saved);
    }

    public async Task<ProductDto?> PatchAsync(
        int id, PatchProductDto dto, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        if (dto.ProductName is not null) product.ProductName = dto.ProductName;
        if (dto.Price is not null) product.Price = dto.Price.Value;
        if (dto.StockQuantity is not null) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.ImagePath is not null) product.ImagePath = dto.ImagePath;

        if (dto.CategoryId is not null)
        {
            _ = await _repository.FindCategoryAsync(dto.CategoryId.Value, ct)
                ?? throw new InvalidOperationException(
                    $"Категория с Id {dto.CategoryId} не найдена.");
            product.CategoryId = dto.CategoryId.Value;
        }

        _repository.Update(product);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException(
                "Не удалось загрузить обновлённый товар.");
        return _mapper.Map<ProductDto>(saved);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return false;

        _repository.Remove(product);
        await _repository.SaveChangesAsync();
        return true;
    }
}
