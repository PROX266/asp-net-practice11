using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly WebstoreContext _context;

    public OrderItemRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(i => i.Product)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.OrderItems.CountAsync(ct);

    public async Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.OrderItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<Order?> FindOrderAsync(int orderId, CancellationToken ct) =>
        await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);

    public async Task<Product?> FindProductAsync(int productId, CancellationToken ct) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);

    public async Task AddAsync(OrderItem orderItem) =>
        await _context.OrderItems.AddAsync(orderItem);

    public void Update(OrderItem orderItem) => _context.OrderItems.Update(orderItem);

    public void Remove(OrderItem orderItem) => _context.OrderItems.Remove(orderItem);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
