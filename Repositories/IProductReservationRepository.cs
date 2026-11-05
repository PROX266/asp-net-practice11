using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IProductReservationRepository
{
    Task<List<ProductReservation>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<ProductReservation?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(ProductReservation reservation);
    void Update(ProductReservation reservation);
    Task SaveChangesAsync();
}
