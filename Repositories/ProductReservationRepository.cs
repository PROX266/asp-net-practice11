using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class ProductReservationRepository : IProductReservationRepository
{
    private readonly WebstoreContext _context;

    public ProductReservationRepository(WebstoreContext context)
    {
        _context = context;
    }

    public async Task<List<ProductReservation>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return await _context.ProductReservations
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Product)
            .Include(r => r.User)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct) =>
        await _context.ProductReservations.CountAsync(ct);

    public async Task<ProductReservation?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.ProductReservations
            .Include(r => r.Product)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task AddAsync(ProductReservation reservation) =>
        await _context.ProductReservations.AddAsync(reservation);

    public void Update(ProductReservation reservation) =>
        _context.ProductReservations.Update(reservation);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
