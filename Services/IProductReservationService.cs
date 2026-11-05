using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface IProductReservationService
{
    Task<(List<ProductReservationDto> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, CancellationToken ct);

    Task<ProductReservationDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<ProductReservationDto> CreateAsync(CreateProductReservationDto dto, CancellationToken ct);

    Task<ProductReservationDto?> ReleaseAsync(int id, CancellationToken ct);
}
