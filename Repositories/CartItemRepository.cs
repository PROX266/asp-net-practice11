using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly WebstoreContext _context;

    public CartItemRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.CartItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(i => i.Product)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.CartItems.CountAsync(ct);

    public async Task<CartItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.CartItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<Cart?> FindCartAsync(int cartId, CancellationToken ct) =>
        await _context.Carts.FirstOrDefaultAsync(c => c.Id == cartId, ct);

    public async Task<Product?> FindProductAsync(int productId, CancellationToken ct) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);

    public async Task AddAsync(CartItem cartItem) =>
        await _context.CartItems.AddAsync(cartItem);

    public void Update(CartItem cartItem) => _context.CartItems.Update(cartItem);

    public void Remove(CartItem cartItem) => _context.CartItems.Remove(cartItem);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
