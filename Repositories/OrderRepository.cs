using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly WebstoreContext _context;

    public OrderRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .ToListAsync(ct);
    }

    public async Task<List<Order>> GetByUserAsync(int userId, CancellationToken ct)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderBy(o => o.Id)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.Orders.CountAsync(ct);

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<User?> FindUserAsync(int userId, CancellationToken ct) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task AddAsync(Order order) =>
        await _context.Orders.AddAsync(order);

    public void Update(Order order) => _context.Orders.Update(order);

    public void Remove(Order order) => _context.Orders.Remove(order);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
