using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<List<Order>> GetByUserAsync(int userId, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<User?> FindUserAsync(int userId, CancellationToken ct);
    Task AddAsync(Order order);
    void Update(Order order);
    void Remove(Order order);
    Task SaveChangesAsync();
}
